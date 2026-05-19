using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WoadNameGen;

public sealed class MarkovNameGenerator
{
    private readonly MarkovNameModel _model;
    private readonly IRandomSource _random;

    public MarkovNameGenerator(MarkovNameModel model)
        : this(model, new SystemRandomSource())
    {
    }

    public MarkovNameGenerator(MarkovNameModel model, int seed)
        : this(model, new SystemRandomSource(seed))
    {
    }

    public MarkovNameGenerator(MarkovNameModel model, IRandomSource random)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public string Generate()
    {
        return Generate(new NameGenerationOptions());
    }

    public string Generate(NameGenerationOptions options)
    {
        ValidateOptions(options);

        for (int attempt = 0; attempt < options.MaxAttempts; attempt++)
        {
            string candidate = GenerateSingle(options);

            if (!NameCandidateValidator.IsValid(candidate, options, _model.WasInTrainingData))
                continue;

            return candidate;
        }

        throw new InvalidOperationException(
            $"Failed to generate a valid name after {options.MaxAttempts} attempts. " +
            "Try relaxing length constraints, increasing MaxAttempts, or allowing training duplicates.");
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        NameGenerationOptions? options = null)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        options ??= new NameGenerationOptions();

        ValidateOptions(options);

        List<string> results = new List<string>(count);
        HashSet<string> used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        int attempts = 0;
        int maxAttempts = Math.Max(options.MaxAttempts, count * options.MaxAttempts);

        while (results.Count < count && attempts < maxAttempts)
        {
            attempts++;

            string candidate;

            try
            {
                candidate = Generate(options);
            }
            catch (InvalidOperationException)
            {
                break;
            }

            if (used.Add(candidate))
                results.Add(candidate);
        }

        return results;
    }

    private string GenerateSingle(NameGenerationOptions options)
    {
        bool useGuidedPrefix =
            options.UseGuidedPrefix &&
            !string.IsNullOrWhiteSpace(options.RequiredPrefix);

        string rawPrefix = useGuidedPrefix
            ? options.RequiredPrefix!.Trim().ToLowerInvariant()
            : string.Empty;

        StringBuilder builder = new StringBuilder(rawPrefix);

        string state = useGuidedPrefix
            ? BuildStateFromPrefix(rawPrefix, _model.Order)
            : new string(MarkovNameModel.StartToken, _model.Order);

        if (useGuidedPrefix && !_model.TryGetTransitions(state, out _))
            return string.Empty;

        while (builder.Length < options.MaxLength)
        {
            if (!_model.TryGetTransitions(
                    state,
                    out IReadOnlyDictionary<char, int>? transitions))
            {
                break;
            }

            char next = WeightedChoice.Choose(transitions, _random);

            if (next == MarkovNameModel.EndToken)
                break;

            builder.Append(next);

            state = state.Substring(1) + next;
        }

        string result = builder.ToString();

        if (options.UseGuidedSuffix &&
            !string.IsNullOrWhiteSpace(options.RequiredSuffix))
        {
            result = AppendSuffixIfNeeded(result, options.RequiredSuffix!);
        }

        return ApplyFormatting(result, options);
    }

    private bool IsCandidateValid(string candidate, NameGenerationOptions options)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return false;

        if (candidate.Length < options.MinLength)
            return false;

        if (candidate.Length > options.MaxLength)
            return false;

        if (options.AvoidTrainingDuplicates && _model.WasInTrainingData(candidate))
            return false;

        if (!string.IsNullOrWhiteSpace(options.RequiredPrefix)
            && !candidate.StartsWith(options.RequiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.RequiredSuffix)
            && !candidate.EndsWith(options.RequiredSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (ContainsForbiddenSubstring(candidate, options.ForbiddenSubstrings))
            return false;

        if (ContainsForbiddenCharacter(candidate, options.ForbiddenCharacters))
            return false;

        if (!UsesOnlyAllowedCharacters(candidate, options.AllowedCharacters))
            return false;

        if (options.MaxConsecutiveIdenticalCharacters.HasValue
            && ExceedsMaxConsecutiveIdenticalCharacters(
                candidate,
                options.MaxConsecutiveIdenticalCharacters.Value))
        {
            return false;
        }

        if (options.CustomValidator != null && !options.CustomValidator(candidate))
            return false;

        return true;
    }

    private static bool ContainsForbiddenSubstring(
        string candidate,
        IEnumerable<string> forbiddenSubstrings)
    {
        foreach (string forbidden in forbiddenSubstrings)
        {
            if (string.IsNullOrWhiteSpace(forbidden))
                continue;

            if (candidate.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    private static bool ContainsForbiddenCharacter(
        string candidate,
        IEnumerable<char> forbiddenCharacters)
    {
        HashSet<char> forbiddenSet = new HashSet<char>(forbiddenCharacters);

        foreach (char character in candidate)
        {
            if (forbiddenSet.Contains(character))
                return true;
        }

        return false;
    }

    private static bool UsesOnlyAllowedCharacters(
        string candidate,
        IReadOnlyCollection<char> allowedCharacters)
    {
        if (allowedCharacters.Count == 0)
            return true;

        HashSet<char> allowedSet = new HashSet<char>(allowedCharacters);

        foreach (char character in candidate)
        {
            if (!allowedSet.Contains(character))
                return false;
        }

        return true;
    }

    private static bool ExceedsMaxConsecutiveIdenticalCharacters(
        string candidate,
        int maxConsecutive)
    {
        if (candidate.Length == 0)
            return false;

        int currentRunLength = 1;

        for (int i = 1; i < candidate.Length; i++)
        {
            if (char.ToLowerInvariant(candidate[i]) ==
                char.ToLowerInvariant(candidate[i - 1]))
            {
                currentRunLength++;

                if (currentRunLength > maxConsecutive)
                    return true;
            }
            else
            {
                currentRunLength = 1;
            }
        }

        return false;
    }

    private static string ApplyFormatting(
        string value,
        NameGenerationOptions options)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string result = value.Trim();

        if (options.LowercaseRest)
            result = result.ToLowerInvariant();

        if (options.CapitaliseFirstLetter)
        {
            result = char.ToUpperInvariant(result[0])
                     + (result.Length > 1 ? result.Substring(1) : string.Empty);
        }

        return result;
    }

    private static void ValidateOptions(NameGenerationOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.MinLength < 0)
            throw new ArgumentOutOfRangeException(nameof(options.MinLength));

        if (options.MaxLength < options.MinLength)
            throw new ArgumentException(
                "MaxLength must be greater than or equal to MinLength.");

        if (options.MaxAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(options.MaxAttempts));

        if (options.ForbiddenSubstrings == null)
            throw new ArgumentNullException(nameof(options.ForbiddenSubstrings));

        if (options.ForbiddenCharacters == null)
            throw new ArgumentNullException(nameof(options.ForbiddenCharacters));

        if (options.AllowedCharacters == null)
            throw new ArgumentNullException(nameof(options.AllowedCharacters));

        if (options.MaxConsecutiveIdenticalCharacters.HasValue
            && options.MaxConsecutiveIdenticalCharacters.Value < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options.MaxConsecutiveIdenticalCharacters),
                "MaxConsecutiveIdenticalCharacters must be at least 1 when set.");
        }
    }

    private static string BuildStateFromPrefix(string prefix, int order)
    {
        if (string.IsNullOrEmpty(prefix))
            return new string(MarkovNameModel.StartToken, order);

        string padded = new string(MarkovNameModel.StartToken, order) + prefix;

        if (padded.Length <= order)
            return padded.PadLeft(order, MarkovNameModel.StartToken);

        return padded.Substring(padded.Length - order, order);
    }

    private static string AppendSuffixIfNeeded(string value, string suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return value;

        string trimmedSuffix = suffix.Trim().ToLowerInvariant();

        if (value.EndsWith(trimmedSuffix, StringComparison.OrdinalIgnoreCase))
            return value;

        if (value.Length > 0 && trimmedSuffix.Length > 0)
        {
            char lastValueChar = char.ToLowerInvariant(value[value.Length - 1]);
            char firstSuffixChar = char.ToLowerInvariant(trimmedSuffix[0]);

            if (lastValueChar == firstSuffixChar)
                return value + trimmedSuffix.Substring(1);
        }

        return value + trimmedSuffix;
    }
}