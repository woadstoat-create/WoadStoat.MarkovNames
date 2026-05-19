using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class MarkovNameGeneratorTests
{
    [Fact]
    public void SameSeedProducesSameSequence()
    {
        string[] samples =
        {
            "Aedan",
            "Alasdair",
            "Caelan",
            "Duncan",
            "Ewan",
            "Fergus",
            "Finlay",
            "Lachlan",
            "Malcolm",
            "Ruaridh"
        };

        MarkovNameModel model = new MarkovNameTrainer(order: 2).Train(samples);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 12,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 500
        };

        MarkovNameGenerator generatorA = new MarkovNameGenerator(model, seed: 12345);
        MarkovNameGenerator generatorB = new MarkovNameGenerator(model, seed: 12345);

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
    public void GeneratedNamesRespectLengthConstraints()
    {
        string[] samples =
        {
            "Marcus",
            "Lucius",
            "Cassius",
            "Aurelius",
            "Octavius",
            "Valerius",
            "Flavius",
            "Severus"
        };

        MarkovNameModel model = new MarkovNameTrainer(order: 2).Train(samples);
        MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 100);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 8,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000
        };

        for (int i = 0; i < 50; i++)
        {
            string name = generator.Generate(options);

            Assert.InRange(name.Length, options.MinLength, options.MaxLength);
        }
    }

    [Fact]
    public void ImpossibleFilterThrowsInvalidOperationException()
    {
        string[] samples =
        {
            "Ax",
            "Ex",
            "Ix",
            "Ox",
            "Ux"
        };

        MarkovNameModel model = new MarkovNameTrainer(order: 1).Train(samples);
        MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 1);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 2,
            MaxLength = 4,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 10
        };

        options.ForbiddenCharacters.Add('x');

        Assert.Throws<InvalidOperationException>(() => generator.Generate(options));
    }

    [Fact]
    public void GuidedPrefixGeneratesNamesWithRequiredPrefix()
    {
        string[] samples =
        {
            "MacLeod",
            "MacDonald",
            "MacKenzie",
            "MacGregor",
            "MacArthur",
            "MacNab",
            "MacMillan",
            "MacPherson"
        };

        MarkovNameModel model = new MarkovNameTrainer(order: 2).Train(samples);
        MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 42);

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
    public void GuidedSuffixGeneratesNamesWithRequiredSuffix()
    {
        string[] samples =
        {
            "Marc",
            "Luc",
            "Cass",
            "Aurel",
            "Octav",
            "Valer",
            "Flav",
            "Sever"
        };

        MarkovNameModel model = new MarkovNameTrainer(order: 2).Train(samples);
        MarkovNameGenerator generator = new MarkovNameGenerator(model, seed: 77);

        NameGenerationOptions options = new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 14,
            AvoidTrainingDuplicates = false,
            MaxAttempts = 1000,
            RequiredSuffix = "us",
            UseGuidedSuffix = true
        };

        for (int i = 0; i < 20; i++)
        {
            string name = generator.Generate(options);

            Assert.EndsWith("us", name, StringComparison.OrdinalIgnoreCase);
        }
    }
}