using System;
using System.Collections.Generic;

namespace WoadNameGen;

public sealed class NameCultureProfile
{
    private readonly Dictionary<string, NameCategory> _categories;

    public string CultureKey { get; }

    public IReadOnlyDictionary<string, NameCategory> Categories => _categories;

    public NameCultureProfile(string cultureKey)
    {
        CultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        _categories = new Dictionary<string, NameCategory>(
            StringComparer.OrdinalIgnoreCase);
    }

    public NameCultureProfile AddCategory(
        string categoryKey,
        IEnumerable<string> samples)
    {
        NameCategory category = new NameCategory(categoryKey, samples);

        _categories[category.Key] = category;

        return this;
    }

    public bool HasCategory(string categoryKey)
    {
        string normalisedKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        return _categories.ContainsKey(normalisedKey);
    }

    public NameCategory GetCategory(string categoryKey)
    {
        string normalisedKey = NameKey.Normalise(categoryKey, nameof(categoryKey));

        if (!_categories.TryGetValue(normalisedKey, out NameCategory? category))
        {
            throw new KeyNotFoundException(
                $"Culture '{CultureKey}' does not contain category '{normalisedKey}'.");
        }

        return category;
    }
}