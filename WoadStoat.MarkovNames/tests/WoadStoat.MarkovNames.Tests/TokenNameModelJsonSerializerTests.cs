using WoadStoat.MarkovNames;
using WoadStoat.MarkovNames.Serialization;

namespace WoadStoat.MarkovNames.Tests;

public sealed class TokenNameModelJsonSerializerTests
{
    [Fact]
    public void TokenModelRoundTripPreservesDeterministicGeneration()
    {
        string[] samples =
        {
            "MacLeod",
            "MacDonald",
            "MacKenzie",
            "MacGregor",
            "MacArthur",
            "Aedan",
            "Eoghan",
            "Donnchadh"
        };

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "dh",
            "ch",
            "ae",
            "eo"
        });

        TokenMarkovNameModel model = new TokenMarkovNameTrainer(
            order: 2,
            tokenizer: tokenizer)
            .Train(samples);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        TokenMarkovNameGenerator beforeGenerator =
            new TokenMarkovNameGenerator(model, seed: 12345);

        IReadOnlyList<string> before = beforeGenerator.GenerateMany(20, options);

        string json = TokenNameModelJsonSerializer.ToJson(model);

        TokenMarkovNameModel loaded =
            TokenNameModelJsonSerializer.ModelFromJson(json);

        TokenMarkovNameGenerator afterGenerator =
            new TokenMarkovNameGenerator(loaded, seed: 12345);

        IReadOnlyList<string> after = afterGenerator.GenerateMany(20, options);

        Assert.Equal(before, after);
    }

    [Fact]
    public void TokenLibraryRoundTripPreservesCultureAndCategoryKeys()
    {
        NameCultureProfile profile = new NameCultureProfile("gaelic")
            .AddCategory("people", new[]
            {
                "Aedan",
                "Caelan",
                "Eoghan",
                "Donnchadh"
            })
            .AddCategory("clans", new[]
            {
                "MacLeod",
                "MacDonald",
                "MacKenzie",
                "MacGregor"
            });

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "dh",
            "ch",
            "ae",
            "eo"
        });

        TokenNameModelLibrary library = new TokenNameModelLibrary(order: 2);
        library.AddProfile(profile, tokenizer);

        string json = TokenNameModelJsonSerializer.LibraryToJson(library);

        TokenNameModelLibrary loaded =
            TokenNameModelJsonSerializer.LibraryFromJson(json);

        Assert.True(loaded.HasCulture("gaelic"));
        Assert.True(loaded.HasModel("gaelic", "people"));
        Assert.True(loaded.HasModel("gaelic", "clans"));
    }

    [Fact]
    public void TokenLibraryRoundTripPreservesGuidedGeneration()
    {
        NameCultureProfile profile = new NameCultureProfile("gaelic")
            .AddCategory("clans", new[]
            {
                "MacLeod",
                "MacDonald",
                "MacKenzie",
                "MacGregor",
                "MacArthur",
                "MacNab"
            });

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "dh",
            "ch",
            "ae",
            "eo"
        });

        TokenNameModelLibrary library = new TokenNameModelLibrary(order: 2);
        library.AddProfile(profile, tokenizer);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 5,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000,
            RequiredPrefix = "Mac",
            UseGuidedPrefix = true
        };

        IReadOnlyList<string> before = library.GenerateMany(
            "gaelic",
            "clans",
            count: 20,
            seed: 999,
            options);

        string json = TokenNameModelJsonSerializer.LibraryToJson(library);

        TokenNameModelLibrary loaded =
            TokenNameModelJsonSerializer.LibraryFromJson(json);

        IReadOnlyList<string> after = loaded.GenerateMany(
            "gaelic",
            "clans",
            count: 20,
            seed: 999,
            options);

        Assert.Equal(before, after);
    }

    [Fact]
    public void TokenModelRoundTripPreservesGreedyTokenizerTokens()
    {
        string[] samples =
        {
            "MacLeod",
            "MacDonald",
            "MacGregor",
            "Donnchadh"
        };

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "dh",
            "ch"
        });

        TokenMarkovNameModel model = new TokenMarkovNameTrainer(
            order: 2,
            tokenizer: tokenizer)
            .Train(samples);

        string json = TokenNameModelJsonSerializer.ToJson(model);

        TokenMarkovNameModel loaded =
            TokenNameModelJsonSerializer.ModelFromJson(json);

        GreedyNameTokenizer loadedTokenizer =
            Assert.IsType<GreedyNameTokenizer>(loaded.Tokenizer);

        Assert.Contains("mac", loadedTokenizer.Tokens);
        Assert.Contains("dh", loadedTokenizer.Tokens);
        Assert.Contains("ch", loadedTokenizer.Tokens);
    }

    [Fact]
    public void InvalidTokenizerTypeThrows()
    {
        string json = """
        {
          "order": 2,
          "tokenizer": {
            "type": "unknown",
            "tokens": []
          },
          "transitions": [],
          "trainingSamples": []
        }
        """;

        Assert.Throws<InvalidOperationException>(
            () => TokenNameModelJsonSerializer.ModelFromJson(json));
    }
}