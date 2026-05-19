namespace WoadStoat.MarkovNames;

public interface INameTokenizer
{
    IReadOnlyList<string> Tokenize(string value);

    string JoinTokens(IEnumerable<string> tokens);
}