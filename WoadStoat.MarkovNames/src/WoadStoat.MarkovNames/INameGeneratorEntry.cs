namespace WoadStoat.MarkovNames;

internal interface INameGeneratorEntry
{
    int Order { get; }

    NameModelKind Kind { get; }

    string Generate(NameGenerationOptions? options);

    string Generate(int seed, NameGenerationOptions? options);

    IReadOnlyList<string> GenerateMany(
        int count,
        NameGenerationOptions? options);

    IReadOnlyList<string> GenerateMany(
        int count,
        int seed,
        NameGenerationOptions? options);
}