using WoadNameGen;

namespace WoadNameGen.Tests;

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
}