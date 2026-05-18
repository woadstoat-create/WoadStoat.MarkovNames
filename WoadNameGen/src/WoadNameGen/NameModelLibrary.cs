using System;
using System.Collections.Generic;
using System.Linq;

namespace WoadNameGen;

public sealed class NameModelLibrary
{
    private readonly Dictionary<string, Dictionary<string, MarkovNameModel>> _models;

    public int Order { get; }

    public IReadOnlyCollection<string> CultureKeys => _models.Keys.ToList();

    public NameModelLibrary(int order = 2)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;

        _models = new Dictionary<string, Dictionary<string, MarkovNameModel>>(
            StringComparer.OrdinalIgnoreCase);
    }

    public static NameModelLibrary Train(NameCultureProfile profile, int order = 2)
    {
        NameModelLibrary library = new NameModelLibrary(order);
        library.AddProfile(profile);
        return library;
    }

    public static NameModelLibrary TrainMany(
        IEnumerable<NameCultureProfile> profiles,
        int order = 2)
    {
        if (profiles == null)
            throw new ArgumentNullException(nameof(profiles));

        NameModelLibrary library = new NameModelLibrary(order);

        foreach (NameCultureProfile profile in profiles)
        {
            library.AddProfile(profile);
        }

        return library;
    }

    public void AddProfile(NameCultureProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        MarkovNameTrainer trainer = new MarkovNameTrainer(Order);

        foreach (NameCategory category in profile.Categories.Values)
        {
            MarkovNameModel model = trainer.Train(category.Samples);

            AddModel(profile.CultureKey, category.Key, model);
        }
    }

    public void AddModel(
        string cultureKey,
        string categoryKey,
        MarkovNameModel model)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, MarkovNameModel>? categories))
        {
            categories = new Dictionary<string, MarkovNameModel>(
                StringComparer.OrdinalIgnoreCase);

            _models[normalisedCultureKey] = categories;
        }

        categories[normalisedCategoryKey] = model;
    }

    public bool HasCulture(string cultureKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        return _models.ContainsKey(normalisedCultureKey);
    }

    public bool HasModel(string cultureKey, string categoryKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        return _models.TryGetValue(normalisedCultureKey, out Dictionary<string, MarkovNameModel>? categories)
               && categories.ContainsKey(normalisedCategoryKey);
    }

    public IReadOnlyCollection<string> GetCategoryKeys(string cultureKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, MarkovNameModel>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this name model library.");
        }

        return categories.Keys.ToList();
    }

    public MarkovNameModel GetModel(string cultureKey, string categoryKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, MarkovNameModel>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this name model library.");
        }

        if (!categories.TryGetValue(
                normalisedCategoryKey,
                out MarkovNameModel? model))
        {
            throw new KeyNotFoundException(
                $"Culture '{normalisedCultureKey}' does not contain category '{normalisedCategoryKey}'.");
        }

        return model;
    }

    public MarkovNameGenerator CreateGenerator(
        string cultureKey,
        string categoryKey)
    {
        MarkovNameModel model = GetModel(cultureKey, categoryKey);

        return new MarkovNameGenerator(model);
    }

    public MarkovNameGenerator CreateGenerator(
        string cultureKey,
        string categoryKey,
        int seed)
    {
        MarkovNameModel model = GetModel(cultureKey, categoryKey);

        return new MarkovNameGenerator(model, seed);
    }

    public MarkovNameGenerator CreateGenerator(
        string cultureKey,
        string categoryKey,
        IRandomSource random)
    {
        MarkovNameModel model = GetModel(cultureKey, categoryKey);

        return new MarkovNameGenerator(model, random);
    }

    public string Generate(
        string cultureKey,
        string categoryKey,
        NameGenerationOptions? options = null)
    {
        MarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public string Generate(
        string cultureKey,
        string categoryKey,
        int seed,
        NameGenerationOptions? options = null)
    {
        MarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey, seed);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public IReadOnlyList<string> GenerateMany(
        string cultureKey,
        string categoryKey,
        int count,
        NameGenerationOptions? options = null)
    {
        MarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey);

        return generator.GenerateMany(count, options);
    }

    public IReadOnlyList<string> GenerateMany(
        string cultureKey,
        string categoryKey,
        int count,
        int seed,
        NameGenerationOptions? options = null)
    {
        MarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey, seed);

        return generator.GenerateMany(count, options);
    }
}