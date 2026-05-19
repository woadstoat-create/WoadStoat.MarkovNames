namespace WoadStoat.MarkovNames;

/// <summary>
/// Defines how a required suffix should be joined to a generated name stem.
/// </summary>
public enum SuffixJoinMode
{
    /// <summary>
    /// Always appends the suffix unless the generated value already ends with it.
    /// </summary>
    Append,

    /// <summary>
    /// Merges one overlapping character between the generated value and suffix.
    /// For example, "Marcu" + "us" becomes "Marcus".
    /// </summary>
    MergeOverlappingCharacter,

    /// <summary>
    /// Merges the longest overlapping substring between the generated value and suffix.
    /// For example, "Aureli" + "ius" becomes "Aurelius".
    /// </summary>
    MergeOverlappingSubstring
}