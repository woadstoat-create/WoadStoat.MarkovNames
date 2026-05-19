namespace WoadStoat.MarkovNames;

internal sealed class TokenNameGeneratorEntry : INameGeneratorEntry
{
    private readonly TokenMarkovNameModel _model;

    public int Order => _model.Order;

    public NameModelKind Kind => NameModelKind.Token;

    public TokenNameGeneratorEntry(TokenMarkovNameModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public string Generate(NameGenerationOptions? options)
    {
        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(_model);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public string Generate(int seed, NameGenerationOptions? options)
    {
        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(_model, seed);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        NameGenerationOptions? options)
    {
        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(_model);

        return generator.GenerateMany(count, options);
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        int seed,
        NameGenerationOptions? options)
    {
        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(_model, seed);

        return generator.GenerateMany(count, options);
    }
}