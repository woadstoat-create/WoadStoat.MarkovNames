namespace WoadNameGen;

public sealed class TokenMarkovNameModel
{
    internal const string StartToken = "\u0002";
    internal const string EndToken = "\u0003";
    internal const string StateSeparator = "\u001F";

    private readonly Dictionary<string, IReadOnlyDictionary<string, int>> _transitions;
    private readonly HashSet<string> _trainingSamples;

    public int Order { get; }

    public INameTokenizer Tokenizer { get; }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> Transitions => _transitions;

    public IReadOnlyCollection<string> TrainingSamples => _trainingSamples;

    public TokenMarkovNameModel(
        int order,
        Dictionary<string, Dictionary<string, int>> transitions,
        IEnumerable<string> trainingSamples,
        INameTokenizer tokenizer)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));

        _transitions = transitions.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyDictionary<string, int>)pair.Value
        );

        _trainingSamples = new HashSet<string>(
            trainingSamples.Select(NormaliseForComparison),
            StringComparer.OrdinalIgnoreCase
        );
    }

    public bool WasInTrainingData(string value)
    {
        return _trainingSamples.Contains(NormaliseForComparison(value));
    }

    internal bool TryGetTransitions(
        IReadOnlyList<string> stateTokens,
        out IReadOnlyDictionary<string, int>? transitions)
    {
        string stateKey = BuildStateKey(stateTokens);

        return _transitions.TryGetValue(stateKey, out transitions);
    }

    internal static string BuildStateKey(IEnumerable<string> stateTokens)
    {
        return string.Join(StateSeparator, stateTokens);
    }

    private static string NormaliseForComparison(string value)
    {
        return value.Trim();
    }
}