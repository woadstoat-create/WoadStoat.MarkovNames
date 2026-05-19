namespace WoadStoat.MarkovNames;

/// <summary>
/// Identifies the type of model used for a culture/category entry.
/// </summary>
public enum NameModelKind
{
    /// <summary>
    /// A character-based Markov model.
    /// </summary>
    Character,

    /// <summary>
    /// A token-based Markov model.
    /// </summary>
    Token
}