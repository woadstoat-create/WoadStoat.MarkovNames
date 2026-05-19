namespace WoadStoat.MarkovNames;

internal sealed class CharacterNameGeneratorEntry : INameGeneratorEntry
{
    private readonly MarkovNameModel _model;

    public int Order => _model.Order;

    public NameModelKind Kind => NameModelKind.Character;

    public CharacterNameGeneratorEntry(MarkovNameModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public string Generate(NameGenerationOptions? options)
    {
        MarkovNameGenerator generator = new MarkovNameGenerator(_model);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public string Generate(int seed, NameGenerationOptions? options)
    {
        MarkovNameGenerator generator = new MarkovNameGenerator(_model, seed);

        return options == null
            ? generator.Generate()
            : generator.Generate(options);
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        NameGenerationOptions? options)
    {
        MarkovNameGenerator generator = new MarkovNameGenerator(_model);

        return generator.GenerateMany(count, options);
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        int seed,
        NameGenerationOptions? options)
    {
        MarkovNameGenerator generator = new MarkovNameGenerator(_model, seed);

        return generator.GenerateMany(count, options);
    }
}