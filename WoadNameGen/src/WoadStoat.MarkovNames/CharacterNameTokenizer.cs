namespace WoadStoat.MarkovNames;

public sealed class CharacterNameTokenizer : INameTokenizer
{
    public IReadOnlyList<string> Tokenize(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return value.Select(character => character.ToString()).ToList();
    }

    public string JoinTokens(IEnumerable<string> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        return string.Concat(tokens);
    }
}