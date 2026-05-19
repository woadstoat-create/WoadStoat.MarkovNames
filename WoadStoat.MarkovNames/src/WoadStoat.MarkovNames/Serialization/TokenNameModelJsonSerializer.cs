using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Serialization;

public static class TokenNameModelJsonSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static void SaveModel(TokenMarkovNameModel model, string filePath)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        File.WriteAllText(filePath, ToJson(model));
    }

    public static TokenMarkovNameModel LoadModel(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        return ModelFromJson(File.ReadAllText(filePath));
    }

    public static string ToJson(TokenMarkovNameModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        TokenNameModelDto dto = ToDto(model);

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public static TokenMarkovNameModel ModelFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be null, empty, or whitespace.", nameof(json));

        TokenNameModelDto? dto = JsonSerializer.Deserialize<TokenNameModelDto>(json, JsonOptions);

        if (dto == null)
            throw new InvalidOperationException("Failed to deserialize token Markov name model.");

        return FromDto(dto);
    }

    public static void SaveLibrary(TokenNameModelLibrary library, string filePath)
    {
        if (library == null)
            throw new ArgumentNullException(nameof(library));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        File.WriteAllText(filePath, LibraryToJson(library));
    }

    public static TokenNameModelLibrary LoadLibrary(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        return LibraryFromJson(File.ReadAllText(filePath));
    }

    public static string LibraryToJson(TokenNameModelLibrary library)
    {
        if (library == null)
            throw new ArgumentNullException(nameof(library));

        TokenNameModelLibraryDto dto = ToDto(library);

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    public static TokenNameModelLibrary LibraryFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be null, empty, or whitespace.", nameof(json));

        TokenNameModelLibraryDto? dto =
            JsonSerializer.Deserialize<TokenNameModelLibraryDto>(json, JsonOptions);

        if (dto == null)
            throw new InvalidOperationException("Failed to deserialize token name model library.");

        return FromDto(dto);
    }

    private static TokenNameModelDto ToDto(TokenMarkovNameModel model)
    {
        return new TokenNameModelDto
        {
            Order = model.Order,
            Tokenizer = ToDto(model.Tokenizer),
            TrainingSamples = model.TrainingSamples.ToList(),
            Transitions = model.Transitions
                .Select(transition => new TokenTransitionDto
                {
                    State = transition.Key,
                    NextTokens = transition.Value
                        .Select(next => new TokenNextTokenDto
                        {
                            Token = next.Key,
                            Weight = next.Value
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private static TokenMarkovNameModel FromDto(TokenNameModelDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Order < 1)
            throw new InvalidOperationException("Serialized token model has an invalid order.");

        INameTokenizer tokenizer = TokenizerFromDto(dto.Tokenizer);

        Dictionary<string, Dictionary<string, int>> transitions =
            new Dictionary<string, Dictionary<string, int>>();

        foreach (TokenTransitionDto transitionDto in dto.Transitions)
        {
            if (string.IsNullOrEmpty(transitionDto.State))
                throw new InvalidOperationException("Serialized transition contains an empty state.");

            Dictionary<string, int> nextTokens =
                new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (TokenNextTokenDto nextDto in transitionDto.NextTokens)
            {
                if (string.IsNullOrEmpty(nextDto.Token))
                    throw new InvalidOperationException("Serialized transition contains an empty token.");

                if (nextDto.Weight <= 0)
                    continue;

                nextTokens[nextDto.Token] = nextDto.Weight;
            }

            transitions[transitionDto.State] = nextTokens;
        }

        return new TokenMarkovNameModel(
            dto.Order,
            transitions,
            dto.TrainingSamples ?? new List<string>(),
            tokenizer);
    }

    private static TokenNameModelLibraryDto ToDto(TokenNameModelLibrary library)
    {
        TokenNameModelLibraryDto dto = new TokenNameModelLibraryDto
        {
            Order = library.Order
        };

        foreach (string cultureKey in library.CultureKeys)
        {
            TokenCultureModelDto cultureDto = new TokenCultureModelDto
            {
                CultureKey = cultureKey
            };

            foreach (string categoryKey in library.GetCategoryKeys(cultureKey))
            {
                TokenMarkovNameModel model = library.GetModel(cultureKey, categoryKey);

                cultureDto.Categories.Add(new TokenCategoryModelDto
                {
                    CategoryKey = categoryKey,
                    Model = ToDto(model)
                });
            }

            dto.Cultures.Add(cultureDto);
        }

        return dto;
    }

    private static TokenNameModelLibrary FromDto(TokenNameModelLibraryDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Order < 1)
            throw new InvalidOperationException("Serialized token name model library has an invalid order.");

        TokenNameModelLibrary library = new TokenNameModelLibrary(dto.Order);

        foreach (TokenCultureModelDto cultureDto in dto.Cultures)
        {
            if (string.IsNullOrWhiteSpace(cultureDto.CultureKey))
                throw new InvalidOperationException("Serialized culture contains an empty key.");

            foreach (TokenCategoryModelDto categoryDto in cultureDto.Categories)
            {
                if (string.IsNullOrWhiteSpace(categoryDto.CategoryKey))
                    throw new InvalidOperationException("Serialized category contains an empty key.");

                TokenMarkovNameModel model = FromDto(categoryDto.Model);

                library.AddModel(
                    cultureDto.CultureKey,
                    categoryDto.CategoryKey,
                    model);
            }
        }

        return library;
    }

    private static NameTokenizerDto ToDto(INameTokenizer tokenizer)
    {
        if (tokenizer == null)
            throw new ArgumentNullException(nameof(tokenizer));

        if (tokenizer is CharacterNameTokenizer)
        {
            return new NameTokenizerDto
            {
                Type = "character"
            };
        }

        if (tokenizer is GreedyNameTokenizer greedyTokenizer)
        {
            return new NameTokenizerDto
            {
                Type = "greedy",
                Tokens = greedyTokenizer.Tokens.ToList()
            };
        }

        throw new NotSupportedException(
            $"Tokenizer type '{tokenizer.GetType().FullName}' cannot be serialized. " +
            "Only CharacterNameTokenizer and GreedyNameTokenizer are supported by the default serializer.");
    }

    private static INameTokenizer TokenizerFromDto(NameTokenizerDto dto)
    {
        if (dto == null)
            throw new InvalidOperationException("Serialized token model is missing tokenizer data.");

        string type = dto.Type?.Trim().ToLowerInvariant() ?? string.Empty;

        if (type == "character")
            return new CharacterNameTokenizer();

        if (type == "greedy")
        {
            if (dto.Tokens == null || dto.Tokens.Count == 0)
                throw new InvalidOperationException("Greedy tokenizer requires at least one token.");

            return new GreedyNameTokenizer(dto.Tokens);
        }

        throw new InvalidOperationException($"Unknown tokenizer type '{dto.Type}'.");
    }
}