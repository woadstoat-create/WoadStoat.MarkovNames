using System;
using System.Collections.Generic;
using System.Linq;

namespace WoadNameGen;

public sealed class TokenNameModelLibrary
{
    private readonly Dictionary<string, Dictionary<string, TokenMarkovNameModel>> _models;

    public int Order { get; }

    public IReadOnlyCollection<string> CultureKeys => _models.Keys.ToList();

    public TokenNameModelLibrary(int order = 2)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;

        _models = new Dictionary<string, Dictionary<string, TokenMarkovNameModel>>(
            StringComparer.OrdinalIgnoreCase);
    }

    public void AddProfile(NameCultureProfile profile, INameTokenizer tokenizer)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        if (tokenizer == null)
            throw new ArgumentNullException(nameof(tokenizer));

        TokenMarkovNameTrainer trainer = new TokenMarkovNameTrainer(Order, tokenizer);

        foreach (NameCategory category in profile.Categories.Values)
        {
            TokenMarkovNameModel model = trainer.Train(category.Samples);

            AddModel(profile.CultureKey, category.Key, model);
        }
    }

    public void AddModel(
        string cultureKey,
        string categoryKey,
        TokenMarkovNameModel model)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.Order != Order)
        {
            throw new ArgumentException(
                $"Model order '{model.Order}' does not match library order '{Order}'.",
                nameof(model));
        }

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, TokenMarkovNameModel>? categories))
        {
            categories = new Dictionary<string, TokenMarkovNameModel>(
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

        return _models.TryGetValue(normalisedCultureKey, out Dictionary<string, TokenMarkovNameModel>? categories)
               && categories.ContainsKey(normalisedCategoryKey);
    }

    public IReadOnlyCollection<string> GetCategoryKeys(string cultureKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, TokenMarkovNameModel>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this token name model library.");
        }

        return categories.Keys.ToList();
    }

    public TokenMarkovNameModel GetModel(string cultureKey, string categoryKey)
    {
        string normalisedCultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));
        string normalisedCategoryKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (!_models.TryGetValue(
                normalisedCultureKey,
                out Dictionary<string, TokenMarkovNameModel>? categories))
        {
            throw new KeyNotFoundException(
                $"No culture named '{normalisedCultureKey}' exists in this token name model library.");
        }

        if (!categories.TryGetValue(
                normalisedCategoryKey,
                out TokenMarkovNameModel? model))
        {
            throw new KeyNotFoundException(
                $"Culture '{normalisedCultureKey}' does not contain category '{normalisedCategoryKey}'.");
        }

        return model;
    }

    public TokenMarkovNameGenerator CreateGenerator(
        string cultureKey,
        string categoryKey)
    {
        TokenMarkovNameModel model = GetModel(cultureKey, categoryKey);

        return new TokenMarkovNameGenerator(model);
    }

    public TokenMarkovNameGenerator CreateGenerator(
        string cultureKey,
        string categoryKey,
        int seed)
    {
        TokenMarkovNameModel model = GetModel(cultureKey, categoryKey);

        return new TokenMarkovNameGenerator(model, seed);
    }

    public string Generate(
        string cultureKey,
        string categoryKey,
        int seed,
        NameGenerationOptions? options = null)
    {
        TokenMarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey, seed);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public IReadOnlyList<string> GenerateMany(
        string cultureKey,
        string categoryKey,
        int count,
        int seed,
        NameGenerationOptions? options = null)
    {
        TokenMarkovNameGenerator generator = CreateGenerator(cultureKey, categoryKey, seed);

        return generator.GenerateMany(count, options);
    }
}