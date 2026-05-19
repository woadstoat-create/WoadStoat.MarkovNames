using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WoadStoat.MarkovNames;

/// <summary>
/// High-level facade for generating names from character-based and token-based culture models.
/// </summary>
public sealed class NameGeneratorLibrary
{
    private readonly Dictionary<string, Dictionary<string, INameGeneratorEntry>> _entries;

    /// <summary>
    /// Gets the available culture keys.
    /// </summary>
    public IReadOnlyCollection<string> CultureKeys => _entries.Keys.ToList();

    /// <summary>
    /// Creates an empty name generator library.
    /// </summary>
    public NameGeneratorLibrary()
    {
        _entries = new Dictionary<string, Dictionary<string, INameGeneratorEntry>>(
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a unified library from a single JSON culture profile.
    /// </summary>
    public static NameGeneratorLibrary FromProfileJson(string json)
    {
        NameCultureProfileData data =
            NameCultureProfileJsonLoader.LoadProfileDataFromJson(json);

        NameGeneratorLibrary library = new NameGeneratorLibrary();
        library.AddProfileData(data);

        return library;
    }

    /// <summary>
    /// Creates a unified library from a JSON profile set.
    /// </summary>
    public static NameGeneratorLibrary FromProfileSetJson(string json)
    {
        NameCultureProfileSetData setData =
            NameCultureProfileJsonLoader.LoadProfileSetDataFromJson(json);

        NameGeneratorLibrary library = new NameGeneratorLibrary();

        foreach (NameCultureProfileData profileData in setData.Profiles)
        {
            library.AddProfileData(profileData);
        }

        return library;
    }

    /// <summary>
    /// Creates a unified library from a single JSON culture profile file.
    /// </summary>
    public static NameGeneratorLibrary FromProfileFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        return FromProfileJson(File.ReadAllText(filePath));
    }

    /// <summary>
    /// Creates a unified library from a JSON profile set file.
    /// </summary>
    public static NameGeneratorLibrary FromProfileSetFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(filePath));

        return FromProfileSetJson(File.ReadAllText(filePath));
    }

    /// <summary>
    /// Adds a profile using either a character-based or token-based model depending on the profile data.
    /// </summary>
    public void AddProfileData(NameCultureProfileData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        NameCultureProfile profile = NameCultureProfileJsonLoader.ToProfile(data);

        if (data.UseTokens)
        {
            INameTokenizer tokenizer = NameCultureProfileJsonLoader.CreateTokenizer(data);
            AddTokenProfile(profile, tokenizer, data.Order);
        }
        else
        {
            AddCharacterProfile(profile, data.Order);
        }
    }

    /// <summary>
    /// Adds a character-based culture profile.
    /// </summary>
    public void AddCharacterProfile(NameCultureProfile profile, int order = 2)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        MarkovNameTrainer trainer = new MarkovNameTrainer(order);

        foreach (NameCategory category in profile.Categories.Values)
        {
            MarkovNameModel model = trainer.Train(category.Samples);

            AddCharacterModel(profile.CultureKey, category.Key, model);
        }
    }

    /// <summary>
    /// Adds a token-based culture profile.
    /// </summary>
    public void AddTokenProfile(
        NameCultureProfile profile,
        INameTokenizer tokenizer,
        int order = 2)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        if (tokenizer == null)
            throw new ArgumentNullException(nameof(tokenizer));

        TokenMarkovNameTrainer trainer = new TokenMarkovNameTrainer(order, tokenizer);

        foreach (NameCategory category in profile.Categories.Values)
        {
            TokenMarkovNameModel model = trainer.Train(category.Samples);

            AddTokenModel(profile.CultureKey, category.Key, model);
        }
    }

    /// <summary>
    /// Adds a trained character-based model.
    /// </summary>
    public void AddCharacterModel(
        string cultureKey,
        string categoryKey,
        MarkovNameModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        AddEntry(
            cultureKey,
            categoryKey,
            new CharacterNameGeneratorEntry(model));
    }

    /// <summary>
    /// Adds a trained token-based model.
    /// </summary>
    public void AddTokenModel(
        string cultureKey,
        string categoryKey,
        TokenMarkovNameModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        AddEntry(
            cultureKey,
            categoryKey,
            new TokenNameGeneratorEntry(model));
    }

    /// <summary>
    /// Returns true if the library contains the specified culture.
    /// </summary>
    public bool HasCulture(string cultureKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        return _entries.ContainsKey(normalisedCultureKey);
    }

    /// <summary>
    /// Returns true if the library contains the specified culture/category pair.
    /// </summary>
    public bool HasModel(string cultureKey, string categoryKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        return _entries.TryGetValue(normalisedCultureKey, out Dictionary<string, INameGeneratorEntry>? categories)
               && categories.ContainsKey(normalisedCategoryKey);
    }

    /// <summary>
    /// Gets the category keys available for a culture.
    /// </summary>
    public IReadOnlyCollection<string> GetCategoryKeys(string cultureKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        if (!_entries.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, INameGeneratorEntry>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this name generator library.");
        }

        return categories.Keys.ToList();
    }

    /// <summary>
    /// Gets the model kind used by a culture/category pair.
    /// </summary>
    public NameModelKind GetModelKind(string cultureKey, string categoryKey)
    {
        return GetEntry(cultureKey, categoryKey).Kind;
    }

    /// <summary>
    /// Generates one name using a non-deterministic random source.
    /// </summary>
    public string Generate(
        string cultureKey,
        string categoryKey,
        NameGenerationOptions? options = null)
    {
        return GetEntry(cultureKey, categoryKey).Generate(options);
    }

    /// <summary>
    /// Generates one name using a deterministic seed.
    /// </summary>
    public string Generate(
        string cultureKey,
        string categoryKey,
        int seed,
        NameGenerationOptions? options = null)
    {
        return GetEntry(cultureKey, categoryKey).Generate(seed, options);
    }

    /// <summary>
    /// Generates multiple unique names using a non-deterministic random source.
    /// </summary>
    public IReadOnlyList<string> GenerateMany(
        string cultureKey,
        string categoryKey,
        int count,
        NameGenerationOptions? options = null)
    {
        return GetEntry(cultureKey, categoryKey).GenerateMany(count, options);
    }

    /// <summary>
    /// Generates multiple unique names using a deterministic seed.
    /// </summary>
    public IReadOnlyList<string> GenerateMany(
        string cultureKey,
        string categoryKey,
        int count,
        int seed,
        NameGenerationOptions? options = null)
    {
        return GetEntry(cultureKey, categoryKey).GenerateMany(count, seed, options);
    }

    private void AddEntry(
        string cultureKey,
        string categoryKey,
        INameGeneratorEntry entry)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (!_entries.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, INameGeneratorEntry>? categories))
        {
            categories = new Dictionary<string, INameGeneratorEntry>(
                StringComparer.OrdinalIgnoreCase);

            _entries[normalisedCultureKey] = categories;
        }

        categories[normalisedCategoryKey] = entry;
    }

    private INameGeneratorEntry GetEntry(string cultureKey, string categoryKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (!_entries.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, INameGeneratorEntry>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this name generator library.");
        }

        if (!categories.TryGetValue(
                normalisedCategoryKey,
                out INameGeneratorEntry? entry))
        {
            throw new KeyNotFoundException(
                $"Culture '{normalisedCultureKey}' does not contain category '{normalisedCategoryKey}'.");
        }

        return entry;
    }
}