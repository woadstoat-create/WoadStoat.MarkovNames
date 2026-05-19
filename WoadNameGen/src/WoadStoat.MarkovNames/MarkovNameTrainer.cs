namespace WoadStoat.MarkovNames;

public sealed class MarkovNameTrainer
{
    public int Order { get; }

    public MarkovNameTrainer(int order = 2)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;
    }

    public MarkovNameModel Train(IEnumerable<string> samples)
    {
        if (samples == null)
            throw new ArgumentNullException(nameof(samples));

        List<string> cleanedSamples = samples
            .Where(sample => !string.IsNullOrWhiteSpace(sample))
            .Select(sample => sample.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (cleanedSamples.Count == 0)
            throw new ArgumentException(
                "Training data must contain at least one valid sample.",
                nameof(samples));

        Dictionary<string, Dictionary<char, int>> transitions = new();

        foreach (string sample in cleanedSamples)
        {
            TrainSample(sample, transitions);
        }

        return new MarkovNameModel(Order, transitions, cleanedSamples);
    }

    private void TrainSample(
        string sample,
        Dictionary<string, Dictionary<char, int>> transitions)
    {
        string paddedStart = new(MarkovNameModel.StartToken, Order);
        string paddedSample = paddedStart
                              + sample.ToLowerInvariant()
                              + MarkovNameModel.EndToken;

        for (int i = 0; i <= paddedSample.Length - Order - 1; i++)
        {
            string state = paddedSample.Substring(i, Order);
            char next = paddedSample[i + Order];

            if (!transitions.TryGetValue(state, out Dictionary<char, int>? nextCharacters))
            {
                nextCharacters = new Dictionary<char, int>();
                transitions[state] = nextCharacters;
            }

            if (!nextCharacters.ContainsKey(next))
                nextCharacters[next] = 0;

            nextCharacters[next]++;
        }
    }
}