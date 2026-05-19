using System;
using System.Collections.Generic;

namespace WoadStoat.MarkovNames;

/// <summary>
/// Defines a culture or language profile made up of named training categories.
/// </summary>
public sealed class NameCultureProfile
{
    private readonly Dictionary<string, NameCategory> _categories;

    /// <summary>
    /// Gets the normalised culture key.
    /// </summary>
    public string CultureKey { get; }

    /// <summary>
    /// Gets the categories registered on this culture profile.
    /// </summary>
    public IReadOnlyDictionary<string, NameCategory> Categories => _categories;

    /// <summary>
    /// Creates a new culture profile.
    /// </summary>
    public NameCultureProfile(string cultureKey)
    {
        CultureKey = NameKey.Normalise(cultureKey, nameof(cultureKey));

        _categories = new Dictionary<string, NameCategory>(
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Adds or replaces a category on this culture profile.
    /// </summary>
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