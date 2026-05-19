namespace WoadStoat.MarkovNames;

/// <summary>
/// Random source backed by <see cref="System.Random"/>.
/// </summary>
public sealed class SystemRandomSource : IRandomSource
{
    private readonly Random _random;

    /// <summary>
    /// Creates a random source using a non-deterministic seed.
    /// </summary>
    public SystemRandomSource()
    {
        _random = new Random();
    }

    /// <summary>
    /// Creates a random source using a deterministic seed.
    /// </summary>
    public SystemRandomSource(int seed)
    {
        _random = new Random(seed);
    }

    public int NextInt(int minInclusive, int maxExclusive)
    {
        return _random.Next(minInclusive, maxExclusive);
    }
}