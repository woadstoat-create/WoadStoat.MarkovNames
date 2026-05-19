namespace WoadStoat.MarkovNames;

public sealed class MarkovNameModel
{
    public const char StartToken = '^';
    public const char EndToken = '$';

    private readonly Dictionary<string, IReadOnlyDictionary<char, int>> _transitions;
    private readonly HashSet<string> _trainingSamples;

    public int Order { get; }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<char, int>> Transitions => _transitions;

    public IReadOnlyCollection<string> TrainingSamples => _trainingSamples;

    public MarkovNameModel(
        int order,
        Dictionary<string, Dictionary<char, int>> transitions,
        IEnumerable<string> trainingSamples)
    {
        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        Order = order;

        _transitions = transitions.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyDictionary<char, int>)pair.Value
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
        string state,
        out IReadOnlyDictionary<char, int>? transitions)
    {
        return _transitions.TryGetValue(state, out transitions);
    }

    private static string NormaliseForComparison(string value)
    {
        return value.Trim();
    }
}