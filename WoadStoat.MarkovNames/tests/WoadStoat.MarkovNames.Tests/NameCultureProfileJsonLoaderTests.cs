using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class NameCultureProfileJsonLoaderTests
{
    [Fact]
    public void CanTrainTokenLibraryFromProfileFile()
    {
        string json = """
        {
          "cultureKey": "gaelic",
          "order": 2,
          "useTokens": true,
          "tokens": [
            "mac",
            "dh",
            "ch",
            "gh",
            "ae",
            "eo",
            "ai"
          ],
          "categories": {
            "people": [
              "Aedan",
              "Caelan",
              "Eoghan",
              "Ruairidh",
              "Donnchadh"
            ],
            "clans": [
              "MacLeod",
              "MacDonald",
              "MacKenzie",
              "MacGregor"
            ]
          }
        }
        """;

        string filePath = WriteTempJsonFile(json);

        try
        {
            TokenNameModelLibrary library =
                NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileFile(filePath);

            Assert.Contains("gaelic", library.CultureKeys);
            Assert.Contains("people", library.GetCategoryKeys("gaelic"));
            Assert.Contains("clans", library.GetCategoryKeys("gaelic"));

            NameGenerationOptions options = new NameGenerationOptions
            {
                MinLength = 4,
                MaxLength = 16,
                AvoidTrainingDuplicates = false,
                MaxAttempts = 1000
            };

            string generated = library.Generate(
                "gaelic",
                "people",
                seed: 123,
                options);

            Assert.False(string.IsNullOrWhiteSpace(generated));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void InvalidProfileWithoutCategoriesThrows()
    {
        string json = """
        {
          "cultureKey": "broken",
          "order": 2,
          "useTokens": false,
          "categories": {}
        }
        """;

        string filePath = WriteTempJsonFile(json);

        try
        {
            Assert.Throws<InvalidOperationException>(
                () => NameCultureProfileJsonLoader.LoadProfileData(filePath));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void CanTrainTokenLibraryFromProfileSetFile()
    {
        string json = """
        {
          "profiles": [
            {
              "cultureKey": "gaelic",
              "order": 2,
              "useTokens": true,
              "tokens": ["mac", "dh", "ch", "ae", "eo"],
              "categories": {
                "people": ["Aedan", "Caelan", "Eoghan"],
                "clans": ["MacLeod", "MacDonald", "MacGregor"]
              }
            },
            {
              "cultureKey": "roman",
              "order": 2,
              "useTokens": true,
              "tokens": ["us", "ius", "ae", "qu"],
              "categories": {
                "people": ["Marcus", "Lucius", "Gaius", "Aurelius"]
              }
            }
          ]
        }
        """;

        string filePath = WriteTempJsonFile(json);

        try
        {
            TokenNameModelLibrary library =
                NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileSetFile(filePath);

            Assert.Contains("gaelic", library.CultureKeys);
            Assert.Contains("roman", library.CultureKeys);
            Assert.Contains("people", library.GetCategoryKeys("gaelic"));
            Assert.Contains("people", library.GetCategoryKeys("roman"));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    private static string WriteTempJsonFile(string json)
    {
        string filePath = Path.Combine(
            Path.GetTempPath(),
            $"woad-namegen-test-{Guid.NewGuid():N}.json");

        File.WriteAllText(filePath, json);

        return filePath;
    }

    [Fact]
    public void CanLoadProfileDataFromJsonString()
    {
        string json = """
        {
          "cultureKey": "gaelic",
          "order": 2,
          "useTokens": true,
          "tokens": ["mac", "dh", "ch", "ae", "eo"],
          "categories": {
            "people": ["Aedan", "Caelan", "Eoghan"],
            "clans": ["MacLeod", "MacDonald", "MacGregor"]
          }
        }
        """;

        NameCultureProfileData data =
            NameCultureProfileJsonLoader.LoadProfileDataFromJson(json);

        Assert.Equal("gaelic", data.CultureKey);
        Assert.Equal(2, data.Order);
        Assert.True(data.UseTokens);
        Assert.Contains("people", data.Categories.Keys);
        Assert.Contains("clans", data.Categories.Keys);
    }

    [Fact]
    public void CanTrainTokenLibraryFromProfileJsonString()
    {
        string json = """
        {
          "cultureKey": "gaelic",
          "order": 2,
          "useTokens": true,
          "tokens": ["mac", "dh", "ch", "ae", "eo"],
          "categories": {
            "people": ["Aedan", "Caelan", "Eoghan", "Ruairidh"],
            "clans": ["MacLeod", "MacDonald", "MacGregor", "MacArthur"]
          }
        }
        """;

        TokenNameModelLibrary library =
            NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileJson(json);

        Assert.True(library.HasCulture("gaelic"));
        Assert.True(library.HasModel("gaelic", "people"));
        Assert.True(library.HasModel("gaelic", "clans"));

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        string name = library.Generate(
            "gaelic",
            "people",
            seed: 123,
            options);

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void CanTrainCharacterLibraryFromProfileJsonString()
    {
        string json = """
        {
          "cultureKey": "roman",
          "order": 2,
          "useTokens": false,
          "categories": {
            "people": ["Marcus", "Lucius", "Gaius", "Aurelius"],
            "places": ["Roma", "Capua", "Ostia", "Ravenna"]
          }
        }
        """;

        NameModelLibrary library =
            NameCultureProfileJsonLoader.TrainCharacterLibraryFromProfileJson(json);

        Assert.True(library.HasCulture("roman"));
        Assert.True(library.HasModel("roman", "people"));
        Assert.True(library.HasModel("roman", "places"));

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
            seed: 456,
            options);

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void CanTrainTokenLibraryFromProfileSetJsonString()
    {
        string json = """
        {
          "profiles": [
            {
              "cultureKey": "gaelic",
              "order": 2,
              "useTokens": true,
              "tokens": ["mac", "dh", "ch", "ae", "eo"],
              "categories": {
                "people": ["Aedan", "Caelan", "Eoghan"],
                "clans": ["MacLeod", "MacDonald", "MacGregor"]
              }
            },
            {
              "cultureKey": "roman",
              "order": 2,
              "useTokens": true,
              "tokens": ["us", "ius", "ae", "qu"],
              "categories": {
                "people": ["Marcus", "Lucius", "Gaius", "Aurelius"]
              }
            }
          ]
        }
        """;

        TokenNameModelLibrary library =
            NameCultureProfileJsonLoader.TrainTokenLibraryFromProfileSetJson(json);

        Assert.True(library.HasCulture("gaelic"));
        Assert.True(library.HasCulture("roman"));
        Assert.True(library.HasModel("gaelic", "people"));
        Assert.True(library.HasModel("gaelic", "clans"));
        Assert.True(library.HasModel("roman", "people"));
    }

    [Fact]
    public void EmptyJsonStringThrows()
    {
        Assert.Throws<ArgumentException>(
            () => NameCultureProfileJsonLoader.LoadProfileDataFromJson(""));
    }

    [Fact]
    public void InvalidJsonStringThrowsInvalidOperationException()
    {
        string json = """
        {
          "cultureKey": "broken",
          "order":
        }
        """;

        Assert.Throws<InvalidOperationException>(
            () => NameCultureProfileJsonLoader.LoadProfileDataFromJson(json));
    }
}