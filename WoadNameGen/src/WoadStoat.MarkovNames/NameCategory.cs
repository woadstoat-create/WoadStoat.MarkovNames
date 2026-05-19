using System;
using System.Collections.Generic;
using System.Linq;

namespace WoadStoat.MarkovNames;

public sealed class NameCategory
{
    public string Key { get; }

    public IReadOnlyList<string> Samples { get; }

    public NameCategory(string key, IEnumerable<string> samples)
    {
        Key = NameKey.Normalise(key, nameof(key));

        if (samples == null)
            throw new ArgumentNullException(nameof(samples));

        List<string> cleanedSamples = samples
            .Where(sample => !string.IsNullOrWhiteSpace(sample))
            .Select(sample => sample.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (cleanedSamples.Count == 0)
        {
            throw new ArgumentException(
                "A name category must contain at least one valid sample.",
                nameof(samples));
        }

        Samples = cleanedSamples;
    }
}