using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class TokenMarkovNameGeneratorTests
{
    [Fact]
    public void TokenGeneratorWithSameSeedProducesSameSequence()
    {
        string[] samples =
        {
            "MacLeod",
            "MacDonald",
            "MacKenzie",
            "MacGregor",
            "MacArthur",
            "Aedan",
            "Caelan",
            "Eoghan",
            "Ruairidh",
            "Donnchadh"
        };

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "dh",
            "ch",
            "gh",
            "ae",
            "eo",
            "ai",
            "ua"
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

        TokenMarkovNameGenerator generatorA = new TokenMarkovNameGenerator(model, seed: 999);
        TokenMarkovNameGenerator generatorB = new TokenMarkovNameGenerator(model, seed: 999);

        List<string> resultsA = new List<string>();
        List<string> resultsB = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            resultsA.Add(generatorA.Generate(options));
            resultsB.Add(generatorB.Generate(options));
        }

        Assert.Equal(resultsA, resultsB);
    }

    [Fact]
    public void TokenGeneratorSupportsGuidedPrefix()
    {
        string[] samples =
        {
            "MacLeod",
            "MacDonald",
            "MacKenzie",
            "MacGregor",
            "MacArthur",
            "MacNab",
            "MacMillan"
        };

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "ae",
            "eo",
            "ch",
            "dh"
        });

        TokenMarkovNameModel model = new TokenMarkovNameTrainer(
            order: 2,
            tokenizer: tokenizer)
            .Train(samples);

        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(model, seed: 500);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 5,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000,
            RequiredPrefix = "Mac",
            UseGuidedPrefix = true
        };

        for (int i = 0; i < 20; i++)
        {
            string name = generator.Generate(options);

            Assert.StartsWith("Mac", name, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void TokenGeneratorGuidedSuffixUsesConfiguredSuffixJoinMode()
    {
        string[] samples =
        {
            "Aureli",
            "Marcu",
            "Flav",
            "Sever"
        };

        INameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "iu",
            "us"
        });

        TokenMarkovNameModel model = new TokenMarkovNameTrainer(
            order: 2,
            tokenizer: tokenizer)
            .Train(samples);

        TokenMarkovNameGenerator generator = new TokenMarkovNameGenerator(model, seed: 10);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 16,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000,
            RequiredSuffix = "ius",
            UseGuidedSuffix = true,
            SuffixJoinMode = SuffixJoinMode.MergeOverlappingSubstring
        };

        string name = generator.Generate(options);

        Assert.EndsWith("ius", name, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("iius", name, StringComparison.OrdinalIgnoreCase);
    }
}