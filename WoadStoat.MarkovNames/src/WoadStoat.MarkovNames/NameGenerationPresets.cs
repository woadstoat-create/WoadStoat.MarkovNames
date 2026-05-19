namespace WoadStoat.MarkovNames;

public static class NameGenerationPresets
{
    public static NameGenerationOptions Default()
    {
        return new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 12,
            AvoidTrainingDuplicates = true,
            MaxAttempts = 100
        };
    }

    public static NameGenerationOptions Short()
    {
        return new NameGenerationOptions
        {
            MinLength = 3,
            MaxLength = 6,
            AvoidTrainingDuplicates = true,
            MaxAttempts = 150
        };
    }

    public static NameGenerationOptions Long()
    {
        return new NameGenerationOptions
        {
            MinLength = 8,
            MaxLength = 18,
            AvoidTrainingDuplicates = true,
            MaxAttempts = 250
        };
    }

    public static NameGenerationOptions Strict()
    {
        return new NameGenerationOptions
        {
            MinLength = 4,
            MaxLength = 12,
            AvoidTrainingDuplicates = true,
            MaxAttempts = 500,
            MaxConsecutiveIdenticalCharacters = 2
        };
    }
}