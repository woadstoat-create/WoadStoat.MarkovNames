using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WoadNameGen;

namespace WoadNameGen.Serialization;

public static class NameModelJsonSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void SaveModel(MarkovNameModel model, string filePath)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = ToJson(model);
        File.WriteAllText(filePath, json);
    }

    public static MarkovNameModel LoadModel(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = File.ReadAllText(filePath);
        return ModelFromJson(json);
    }

    public static string ToJson(MarkovNameModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        MarkovNameModelDto dto = ToDto(model);

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public static MarkovNameModel ModelFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be null, empty, or whitespace.", nameof(json));

        MarkovNameModelDto? dto = JsonSerializer.Deserialize<MarkovNameModelDto>(json, JsonOptions);

        if (dto == null)
            throw new InvalidOperationException("Failed to deserialize Markov name model.");

        return FromDto(dto);
    }

    public static void SaveLibrary(NameModelLibrary library, string filePath)
    {
        if (library == null)
            throw new ArgumentNullException(nameof(library));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = LibraryToJson(library);
        File.WriteAllText(filePath, json);
    }

    public static NameModelLibrary LoadLibrary(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = File.ReadAllText(filePath);
        return LibraryFromJson(json);
    }

    public static string LibraryToJson(NameModelLibrary library)
    {
        if (library == null)
            throw new ArgumentNullException(nameof(library));

        NameModelLibraryDto dto = ToDto(library);

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public static NameModelLibrary LibraryFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be null, empty, or whitespace.", nameof(json));

        NameModelLibraryDto? dto = JsonSerializer.Deserialize<NameModelLibraryDto>(json, JsonOptions);

        if (dto == null)
            throw new InvalidOperationException("Failed to deserialize name model library.");

        return FromDto(dto);
    }

    private static MarkovNameModelDto ToDto(MarkovNameModel model)
    {
        return new MarkovNameModelDto
        {
            Order = model.Order,
            TrainingSamples = model.TrainingSamples.ToList(),
            Transitions = model.Transitions
                .Select(transition => new MarkovTransitionDto
                {
                    State = transition.Key,
                    NextCharacters = transition.Value
                        .Select(next => new MarkovNextCharacterDto
                        {
                            Character = next.Key.ToString(),
                            Weight = next.Value
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private static MarkovNameModel FromDto(MarkovNameModelDto dto)
    {
        if (dto.Order < 1)
            throw new InvalidOperationException("Serialized Markov model has an invalid order.");

        Dictionary<string, Dictionary<char, int>> transitions =
            new Dictionary<string, Dictionary<char, int>>();

        foreach (MarkovTransitionDto transitionDto in dto.Transitions)
        {
            if (string.IsNullOrEmpty(transitionDto.State))
                throw new InvalidOperationException("Serialized transition contains an empty state.");

            Dictionary<char, int> nextCharacters = new Dictionary<char, int>();

            foreach (MarkovNextCharacterDto nextDto in transitionDto.NextCharacters)
            {
                if (string.IsNullOrEmpty(nextDto.Character))
                    throw new InvalidOperationException("Serialized transition contains an empty character.");

                if (nextDto.Character.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"Serialized character '{nextDto.Character}' must be exactly one character long.");
                }

                if (nextDto.Weight <= 0)
                    continue;

                char character = nextDto.Character[0];
                nextCharacters[character] = nextDto.Weight;
            }

            transitions[transitionDto.State] = nextCharacters;
        }

        return new MarkovNameModel(
            dto.Order,
            transitions,
            dto.TrainingSamples ?? new List<string>());
    }

    private static NameModelLibraryDto ToDto(NameModelLibrary library)
    {
        NameModelLibraryDto dto = new NameModelLibraryDto
        {
            Order = library.Order
        };

        foreach (string cultureKey in library.CultureKeys)
        {
            NameCultureModelDto cultureDto = new NameCultureModelDto
            {
                CultureKey = cultureKey
            };

            foreach (string categoryKey in library.GetCategoryKeys(cultureKey))
            {
                MarkovNameModel model = library.GetModel(cultureKey, categoryKey);

                cultureDto.Categories.Add(new NameCategoryModelDto
                {
                    CategoryKey = categoryKey,
                    Model = ToDto(model)
                });
            }

            dto.Cultures.Add(cultureDto);
        }

        return dto;
    }

    private static NameModelLibrary FromDto(NameModelLibraryDto dto)
    {
        if (dto.Order < 1)
            throw new InvalidOperationException("Serialized name model library has an invalid order.");

        NameModelLibrary library = new NameModelLibrary(dto.Order);

        foreach (NameCultureModelDto cultureDto in dto.Cultures)
        {
            if (string.IsNullOrWhiteSpace(cultureDto.CultureKey))
                throw new InvalidOperationException("Serialized culture contains an empty key.");

            foreach (NameCategoryModelDto categoryDto in cultureDto.Categories)
            {
                if (string.IsNullOrWhiteSpace(categoryDto.CategoryKey))
                    throw new InvalidOperationException("Serialized category contains an empty key.");

                MarkovNameModel model = FromDto(categoryDto.Model);

                library.AddModel(
                    cultureDto.CultureKey,
                    categoryDto.CategoryKey,
                    model);
            }
        }

        return library;
    }
}