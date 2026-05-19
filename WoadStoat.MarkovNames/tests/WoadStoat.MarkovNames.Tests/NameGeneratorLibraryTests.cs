using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class NameGeneratorLibraryTests
{
    [Fact]
    public void FromProfileJsonCreatesCharacterModelWhenUseTokensIsFalse()
    {
        string json = """
        {
          "cultureKey": "roman",
          "order": 2,
          "useTokens": false,
          "categories": {
            "people": ["Marcus", "Lucius", "Gaius", "Aurelius"]
          }
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileJson(json);

        Assert.True(library.HasCulture("roman"));
        Assert.True(library.HasModel("roman", "people"));
        Assert.Equal(NameModelKind.Character, library.GetModelKind("roman", "people"));
    }

    [Fact]
    public void FromProfileJsonCreatesTokenModelWhenUseTokensIsTrue()
    {
        string json = """
        {
          "cultureKey": "gaelic",
          "order": 2,
          "useTokens": true,
          "tokens": ["mac", "dh", "ch", "ae", "eo"],
          "categories": {
            "clans": ["MacLeod", "MacDonald", "MacKenzie", "MacGregor"]
          }
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileJson(json);

        Assert.True(library.HasCulture("gaelic"));
        Assert.True(library.HasModel("gaelic", "clans"));
        Assert.Equal(NameModelKind.Token, library.GetModelKind("gaelic", "clans"));
    }

    [Fact]
    public void UnifiedLibraryCanGenerateFromCharacterModel()
    {
        string json = """
        {
          "cultureKey": "roman",
          "order": 2,
          "useTokens": false,
          "categories": {
            "people": ["Marcus", "Lucius", "Gaius", "Aurelius", "Cassius"]
          }
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileJson(json);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 12,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        string name = library.Generate(
            "roman",
            "people",
            seed: 123,
            options);

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void UnifiedLibraryCanGenerateFromTokenModel()
    {
        string json = """
        {
          "cultureKey": "gaelic",
          "order": 2,
          "useTokens": true,
          "tokens": ["mac", "dh", "ch", "ae", "eo"],
          "categories": {
            "clans": ["MacLeod", "MacDonald", "MacKenzie", "MacGregor", "MacArthur"]
          }
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileJson(json);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 5,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000,
            RequiredPrefix = "Mac",
            UseGuidedPrefix = true
        };

        string name = library.Generate(
            "gaelic",
            "clans",
            seed: 123,
            options);

        Assert.StartsWith("Mac", name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProfileSetCanContainCharacterAndTokenModels()
    {
        string json = """
        {
          "profiles": [
            {
              "cultureKey": "roman",
              "order": 2,
              "useTokens": false,
              "categories": {
                "people": ["Marcus", "Lucius", "Gaius", "Aurelius"]
              }
            },
            {
              "cultureKey": "gaelic",
              "order": 2,
              "useTokens": true,
              "tokens": ["mac", "dh", "ch", "ae", "eo"],
              "categories": {
                "clans": ["MacLeod", "MacDonald", "MacKenzie", "MacGregor"]
              }
            }
          ]
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileSetJson(json);

        Assert.True(library.HasModel("roman", "people"));
        Assert.True(library.HasModel("gaelic", "clans"));

        Assert.Equal(NameModelKind.Character, library.GetModelKind("roman", "people"));
        Assert.Equal(NameModelKind.Token, library.GetModelKind("gaelic", "clans"));
    }

    [Fact]
    public void SameSeedProducesSameResultsThroughUnifiedApi()
    {
        string json = """
        {
          "cultureKey": "roman",
          "order": 2,
          "useTokens": false,
          "categories": {
            "people": ["Marcus", "Lucius", "Gaius", "Aurelius", "Cassius"]
          }
        }
        """;

        NameGeneratorLibrary library =
            NameGeneratorLibrary.FromProfileJson(json);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 12,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        IReadOnlyList<string> first = library.GenerateMany(
            "roman",
            "people",
            count: 10,
            seed: 777,
            options);

        IReadOnlyList<string> second = library.GenerateMany(
            "roman",
            "people",
            count: 10,
            seed: 777,
            options);

        Assert.Equal(first, second);
    }
}