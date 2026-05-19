namespace WoadNameGen;

public sealed class TokenMarkovNameTrainer
{
    private readonly INameTokenizer _tokenizer;

    public int Order { get; }

    public TokenMarkovNameTrainer(
        int order = 2,
        INameTokenizer? tokenizer = null)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;
        _tokenizer = tokenizer ?? new CharacterNameTokenizer();
    }

    public TokenMarkovNameModel Train(IEnumerable<string> samples)
    {
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
                "Training data must contain at least one valid sample.",
                nameof(samples));
        }

        Dictionary<string, Dictionary<string, int>> transitions =
            new Dictionary<string, Dictionary<string, int>>();

        foreach (string sample in cleanedSamples)
        {
            TrainSample(sample, transitions);
        }

        return new TokenMarkovNameModel(
            Order,
            transitions,
            cleanedSamples,
            _tokenizer);
    }

    private void TrainSample(
        string sample,
        Dictionary<string, Dictionary<string, int>> transitions)
    {
        List<string> tokens = new List<string>();

        for (int i = 0; i < Order; i++)
        {
            tokens.Add(TokenMarkovNameModel.StartToken);
        }

        IReadOnlyList<string> sampleTokens =
            _tokenizer.Tokenize(sample.ToLowerInvariant());

        tokens.AddRange(sampleTokens);
        tokens.Add(TokenMarkovNameModel.EndToken);

        for (int i = 0; i <= tokens.Count - Order - 1; i++)
        {
            List<string> stateTokens = tokens
                .Skip(i)
                .Take(Order)
                .ToList();

            string state = TokenMarkovNameModel.BuildStateKey(stateTokens);
            string next = tokens[i + Order];

            if (!transitions.TryGetValue(
                    state,
                    out Dictionary<string, int>? nextTokens))
            {
                nextTokens = new Dictionary<string, int>(StringComparer.Ordinal);
                transitions[state] = nextTokens;
            }

            if (!nextTokens.ContainsKey(next))
                nextTokens[next] = 0;

            nextTokens[next]++;
        }
    }
}