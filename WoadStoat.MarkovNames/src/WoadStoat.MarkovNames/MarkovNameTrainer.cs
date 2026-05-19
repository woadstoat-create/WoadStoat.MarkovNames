namespace WoadStoat.MarkovNames;

/// <summary>
/// Trains character-based Markov name models from sample names.
/// </summary>
public sealed class MarkovNameTrainer
{
    /// <summary>
    /// Gets the Markov order used by this trainer.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Creates a new character-based Markov name trainer.
    /// </summary>
    /// <param name="order">
    /// The Markov order. Higher values produce names closer to the training data.
    /// </param>
    public MarkovNameTrainer(int order = 2)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;
    }

    /// <summary>
    /// Trains a character-based Markov model from the supplied samples.
    /// </summary>
    /// <param name="samples">The training samples.</param>
    /// <returns>A trained Markov name model.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when no valid samples are supplied.</exception>
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