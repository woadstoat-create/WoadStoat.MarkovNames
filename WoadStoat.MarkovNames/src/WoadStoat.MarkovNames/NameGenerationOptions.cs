namespace WoadStoat.MarkovNames;

/// <summary>
/// Defines validation, formatting, and guidance rules used when generating names.
/// </summary>
public sealed class NameGenerationOptions
{
    /// <summary>
    /// Gets or sets the minimum accepted length of a generated name.
    /// </summary>
    public int MinLength { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum accepted length of a generated name.
    /// </summary>
    public int MaxLength { get; set; } = 12;

    /// <summary>
    /// Gets or sets the maximum number of attempts the generator will make before failing.
    /// </summary>
    public int MaxAttempts { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether generated names that exactly match training samples should be rejected.
    /// </summary>
    public bool AvoidTrainingDuplicates { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the first character of the generated name should be capitalised.
    /// </summary>
    public bool CapitaliseFirstLetter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether all characters after formatting should be lower-cased before capitalisation.
    /// </summary>
    public bool LowercaseRest { get; set; } = true;

    /// <summary>
    /// Gets or sets a required prefix for generated names.
    /// </summary>
    public string? RequiredPrefix { get; set; }

    /// <summary>
    /// Gets or sets a required suffix for generated names.
    /// </summary>
    public string? RequiredSuffix { get; set; }

    /// <summary>
    /// Gets or sets whether <see cref="RequiredPrefix"/> should be used as a generation starting point.
    /// </summary>
    public bool UseGuidedPrefix { get; set; } = true;

    /// <summary>
    /// Gets or sets whether <see cref="RequiredSuffix"/> should be appended and validated after generation.
    /// </summary>
    public bool UseGuidedSuffix { get; set; } = true;

    /// <summary>
    /// Gets substrings that must not appear in generated names.
    /// </summary>
    public List<string> ForbiddenSubstrings { get; set; } = new List<string>();

    /// <summary>
    /// Gets characters that must not appear in generated names.
    /// </summary>
    public List<char> ForbiddenCharacters { get; set; } = new List<char>();

    /// <summary>
    /// Gets the optional allowed character set. If empty, all generated characters are allowed.
    /// </summary>
    public List<char> AllowedCharacters { get; set; } = new List<char>();

    /// <summary>
    /// Gets or sets the maximum number of identical consecutive characters allowed.
    /// For example, a value of 2 allows "ll" but rejects "lll".
    /// </summary>
    public int? MaxConsecutiveIdenticalCharacters { get; set; }

    /// <summary>
    /// Gets or sets an optional project-specific validation function.
    /// Return true to accept a candidate, or false to reject it.
    /// </summary>
    public Func<string, bool>? CustomValidator { get; set; }
}