namespace WoadStoat.MarkovNames;

public sealed class GreedyNameTokenizer : INameTokenizer
{
    private readonly List<string> _tokens;

    public IReadOnlyList<string> Tokens => _tokens;

    public GreedyNameTokenizer(IEnumerable<string> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        _tokens = tokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .Select(token => token.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .OrderByDescending(token => token.Length)
            .ToList();

        if (_tokens.Count == 0)
            throw new ArgumentException("At least one token must be supplied.", nameof(tokens));
    }

    public IReadOnlyList<string> Tokenize(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        string remaining = value.ToLowerInvariant();
        List<string> result = new List<string>();

        int index = 0;

        while (index < remaining.Length)
        {
            string? matchedToken = null;

            foreach (string token in _tokens)
            {
                if (index + token.Length > remaining.Length)
                    continue;

                if (string.Compare(
                        remaining,
                        index,
                        token,
                        0,
                        token.Length,
                        StringComparison.Ordinal) == 0)
                {
                    matchedToken = token;
                    break;
                }
            }

            if (matchedToken != null)
            {
                result.Add(matchedToken);
                index += matchedToken.Length;
            }
            else
            {
                result.Add(remaining[index].ToString());
                index++;
            }
        }

        return result;
    }

    public string JoinTokens(IEnumerable<string> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        return string.Concat(tokens);
    }
}