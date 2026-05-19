using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class NameModelLibraryTests
{
    [Fact]
    public void CanTrainLibraryFromCultureProfile()
    {
        NameCultureProfile profile = new NameCultureProfile("roman")
            .AddCategory("people", new[]
            {
                "Marcus",
                "Lucius",
                "Gaius",
                "Titus",
                "Aurelius"
            })
            .AddCategory("places", new[]
            {
                "Roma",
                "Capua",
                "Ostia",
                "Ravenna"
            });

        NameModelLibrary library = NameModelLibrary.Train(profile, order: 2);

        Assert.True(library.HasCulture("roman"));
        Assert.True(library.HasModel("roman", "people"));
        Assert.True(library.HasModel("roman", "places"));
    }

    [Fact]
    public void LibraryGenerateManyUsesCultureAndCategory()
    {
        NameCultureProfile profile = new NameCultureProfile("roman")
            .AddCategory("people", new[]
            {
                "Marcus",
                "Lucius",
                "Gaius",
                "Titus",
                "Aurelius",
                "Cassius",
                "Valerius"
            });

        NameModelLibrary library = NameModelLibrary.Train(profile, order: 2);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 12,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        IReadOnlyList<string> names = library.GenerateMany(
            "roman",
            "people",
            count: 10,
            seed: 321,
            options);

        Assert.NotEmpty(names);
        Assert.All(names, name => Assert.False(string.IsNullOrWhiteSpace(name)));
    }

    [Fact]
    public void UnknownCultureThrows()
    {
        NameCultureProfile profile = new NameCultureProfile("roman")
            .AddCategory("people", new[]
            {
                "Marcus",
                "Lucius",
                "Gaius"
            });

        NameModelLibrary library = NameModelLibrary.Train(profile, order: 2);

        Assert.Throws<KeyNotFoundException>(
            () => library.GetModel("gaelic", "people"));
    }

    [Fact]
    public void UnknownCategoryThrows()
    {
        NameCultureProfile profile = new NameCultureProfile("roman")
            .AddCategory("people", new[]
            {
                "Marcus",
                "Lucius",
                "Gaius"
            });

        NameModelLibrary library = NameModelLibrary.Train(profile, order: 2);

        Assert.Throws<KeyNotFoundException>(
            () => library.GetModel("roman", "places"));
    }
}