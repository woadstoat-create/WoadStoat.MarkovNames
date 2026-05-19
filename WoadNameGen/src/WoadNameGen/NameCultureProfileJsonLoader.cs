using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WoadNameGen;

public static class NameCultureProfileJsonLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static NameCultureProfileData LoadProfileData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = File.ReadAllText(filePath);

        NameCultureProfileData? data =
            JsonSerializer.Deserialize<NameCultureProfileData>(json, JsonOptions);

        if (data == null)
            throw new InvalidOperationException($"Failed to load culture profile from '{filePath}'.");

        ValidateProfileData(data);

        return data;
    }

    public static NameCultureProfileSetData LoadProfileSetData(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        string json = File.ReadAllText(filePath);

        NameCultureProfileSetData? data =
            JsonSerializer.Deserialize<NameCultureProfileSetData>(json, JsonOptions);

        if (data == null)
            throw new InvalidOperationException($"Failed to load culture profile set from '{filePath}'.");

        if (data.Profiles == null || data.Profiles.Count == 0)
            throw new InvalidOperationException("Profile set must contain at least one profile.");

        foreach (NameCultureProfileData profile in data.Profiles)
        {
            ValidateProfileData(profile);
        }

        return data;
    }

    public static NameCultureProfile ToProfile(NameCultureProfileData data)
    {
        ValidateProfileData(data);

        NameCultureProfile profile = new NameCultureProfile(data.CultureKey);

        foreach (KeyValuePair<string, List<string>> category in data.Categories)
        {
            profile.AddCategory(category.Key, category.Value);
        }

        return profile;
    }

    public static INameTokenizer CreateTokenizer(NameCultureProfileData data)
    {
        ValidateProfileData(data);

        if (!data.UseTokens)
            return new CharacterNameTokenizer();

        if (data.Tokens == null || data.Tokens.Count == 0)
            return new CharacterNameTokenizer();

        return new GreedyNameTokenizer(data.Tokens);
    }

    public static NameModelLibrary TrainCharacterLibraryFromProfileFile(string filePath)
    {
        NameCultureProfileData data = LoadProfileData(filePath);
        NameCultureProfile profile = ToProfile(data);

        return NameModelLibrary.Train(profile, data.Order);
    }

    public static NameModelLibrary TrainCharacterLibraryFromProfileSetFile(string filePath)
    {
        NameCultureProfileSetData setData = LoadProfileSetData(filePath);

        int order = GetSharedOrder(setData.Profiles);

        List<NameCultureProfile> profiles = setData.Profiles
            .Select(ToProfile)
            .ToList();

        return NameModelLibrary.TrainMany(profiles, order);
    }

    public static TokenNameModelLibrary TrainTokenLibraryFromProfileFile(string filePath)
    {
        NameCultureProfileData data = LoadProfileData(filePath);
        NameCultureProfile profile = ToProfile(data);
        INameTokenizer tokenizer = CreateTokenizer(data);

        TokenNameModelLibrary library = new TokenNameModelLibrary(data.Order);
        library.AddProfile(profile, tokenizer);

        return library;
    }

    public static TokenNameModelLibrary TrainTokenLibraryFromProfileSetFile(string filePath)
    {
        NameCultureProfileSetData setData = LoadProfileSetData(filePath);

        int order = GetSharedOrder(setData.Profiles);

        TokenNameModelLibrary library = new TokenNameModelLibrary(order);

        foreach (NameCultureProfileData data in setData.Profiles)
        {
            NameCultureProfile profile = ToProfile(data);
            INameTokenizer tokenizer = CreateTokenizer(data);

            library.AddProfile(profile, tokenizer);
        }

        return library;
    }

    private static int GetSharedOrder(IEnumerable<NameCultureProfileData> profiles)
    {
        List<int> orders = profiles
            .Select(profile => profile.Order)
            .Distinct()
            .ToList();

        if (orders.Count != 1)
        {
            throw new InvalidOperationException(
                "All profiles in one library file must use the same Markov order. " +
                "Use separate libraries if you need mixed orders.");
        }

        return orders[0];
    }

    private static void ValidateProfileData(NameCultureProfileData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (string.IsNullOrWhiteSpace(data.CultureKey))
            throw new InvalidOperationException("Culture profile must have a cultureKey.");

        if (data.Order < 1)
            throw new InvalidOperationException("Culture profile order must be at least 1.");

        if (data.Categories == null || data.Categories.Count == 0)
            throw new InvalidOperationException(
                $"Culture profile '{data.CultureKey}' must contain at least one category.");

        foreach (KeyValuePair<string, List<string>> category in data.Categories)
        {
            if (string.IsNullOrWhiteSpace(category.Key))
                throw new InvalidOperationException(
                    $"Culture profile '{data.CultureKey}' contains an empty category key.");

            if (category.Value == null || category.Value.Count == 0)
                throw new InvalidOperationException(
                    $"Culture profile '{data.CultureKey}' category '{category.Key}' must contain at least one sample.");

            bool hasValidSample = category.Value.Any(sample => !string.IsNullOrWhiteSpace(sample));

            if (!hasValidSample)
                throw new InvalidOperationException(
                    $"Culture profile '{data.CultureKey}' category '{category.Key}' must contain at least one valid sample.");
        }
    }
}