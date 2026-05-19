namespace WoadStoat.MarkovNames;

/// <summary>
/// Splits names into tokens and joins generated tokens back into names.
/// </summary>
public interface INameTokenizer
{
    /// <summary>
    /// Splits a string into generation tokens.
    /// </summary>
    IReadOnlyList<string> Tokenize(string value);

    /// <summary>
    /// Joins tokens into a generated name string.
    /// </summary>
    string JoinTokens(IEnumerable<string> tokens);
}