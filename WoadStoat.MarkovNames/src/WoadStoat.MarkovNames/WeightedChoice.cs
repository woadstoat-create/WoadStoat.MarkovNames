namespace WoadStoat.MarkovNames;

internal static class WeightedChoice
{
    public static T Choose<T>(
        IReadOnlyDictionary<T, int> weightedItems,
        IRandomSource random)
        where T : notnull
    {
        if (weightedItems.Count == 0)
            throw new InvalidOperationException("Cannot choose from an empty weighted collection.");

        int totalWeight = 0;

        foreach (int weight in weightedItems.Values)
        {
            if (weight > 0)
                totalWeight += weight;
        }

        if (totalWeight <= 0)
            throw new InvalidOperationException("Weighted collection has no positive weights.");

        int roll = random.NextInt(0, totalWeight);
        int cumulative = 0;

        foreach (KeyValuePair<T, int> item in weightedItems)
        {
            if (item.Value <= 0)
                continue;

            cumulative += item.Value;

            if (roll < cumulative)
                return item.Key;
        }

        throw new InvalidOperationException("Weighted choice failed unexpectedly.");
    }
}