using System;
using System.Collections.Generic;

namespace WoadNameGen;

public sealed class NameGenerationOptions
{
    public int MinLength { get; set; } = 3;

    public int MaxLength { get; set; } = 12;

    public int MaxAttempts { get; set; } = 100;

    public bool AvoidTrainingDuplicates { get; set; } = true;

    public bool CapitaliseFirstLetter { get; set; } = true;

    public bool LowercaseRest { get; set; } = true;

    public string? RequiredPrefix { get; set; }

    public string? RequiredSuffix { get; set; }

    /// <summary>
    /// If true, RequiredPrefix is used as the starting point for generation
    /// rather than only as a rejection filter.
    /// </summary>
    public bool UseGuidedPrefix { get; set; } = true;

    /// <summary>
    /// If true, RequiredSuffix is appended after generation when possible,
    /// then the final candidate is validated.
    /// </summary>
    public bool UseGuidedSuffix { get; set; } = true;

    public List<string> ForbiddenSubstrings { get; set; } = new List<string>();

    public List<char> ForbiddenCharacters { get; set; } = new List<char>();

    /// <summary>
    /// If empty, all characters are allowed.
    /// If populated, generated names may only contain these characters.
    /// </summary>
    public List<char> AllowedCharacters { get; set; } = new List<char>();

    /// <summary>
    /// Example: value of 2 allows "ll" but rejects "lll".
    /// Null means no limit.
    /// </summary>
    public int? MaxConsecutiveIdenticalCharacters { get; set; }

    /// <summary>
    /// Optional final validation hook.
    /// Return true to accept the candidate, false to reject it.
    /// </summary>
    public Func<string, bool>? CustomValidator { get; set; }

    
}