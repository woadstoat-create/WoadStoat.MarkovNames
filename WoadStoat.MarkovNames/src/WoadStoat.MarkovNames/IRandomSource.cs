namespace WoadStoat.MarkovNames;

/// <summary>
/// Provides random integer values for deterministic or custom generation systems.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Returns a random integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound.</param>
    /// <param name="maxExclusive">The exclusive upper bound.</param>
    int NextInt(int minInclusive, int maxExclusive);
}