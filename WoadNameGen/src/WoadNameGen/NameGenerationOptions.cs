namespace WoadNameGen;

public sealed class NameGenerationOptions
{
    public int MinLength { get; set; } = 3;

    public int MaxLength { get; set; } = 12;

    public int MaxAttempts { get; set; } = 100;

    public bool AvoidTrainingDuplicates { get; set; } = true;

    public bool CapitaliseFirstLetter { get; set; } = true;

    public bool LowercaseRest { get; set; } = true;
}