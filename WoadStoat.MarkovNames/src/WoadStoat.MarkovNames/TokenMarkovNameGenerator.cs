using System.Linq;

namespace WoadStoat.MarkovNames;

public sealed class TokenMarkovNameGenerator
{
    private readonly TokenMarkovNameModel _model;
    private readonly IRandomSource _random;

    public TokenMarkovNameGenerator(TokenMarkovNameModel model)
        : this(model, new SystemRandomSource())
    {
    }

    public TokenMarkovNameGenerator(TokenMarkovNameModel model, int seed)
        : this(model, new SystemRandomSource(seed))
    {
    }

    public TokenMarkovNameGenerator(
        TokenMarkovNameModel model,
        IRandomSource random)
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

            if (!NameCandidateValidator.IsValid(
                    candidate,
                    options,
                    _model.WasInTrainingData))
            {
                continue;
            }

            return candidate;
        }

        throw new InvalidOperationException(
            $"Failed to generate a valid token-based name after {options.MaxAttempts} attempts. " +
            "Try relaxing constraints or increasing MaxAttempts.");
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

        List<string> state = new List<string>();

        for (int i = 0; i < _model.Order; i++)
        {
            state.Add(TokenMarkovNameModel.StartToken);
        }

        List<string> outputTokens = new List<string>();

        if (useGuidedPrefix)
        {
            IReadOnlyList<string> prefixTokens =
                _model.Tokenizer.Tokenize(options.RequiredPrefix!.Trim().ToLowerInvariant());

            outputTokens.AddRange(prefixTokens);

            List<string> paddedPrefixState = new List<string>();

            for (int i = 0; i < _model.Order; i++)
            {
                paddedPrefixState.Add(TokenMarkovNameModel.StartToken);
            }

            paddedPrefixState.AddRange(prefixTokens);

            state = paddedPrefixState
                .Skip(Math.Max(0, paddedPrefixState.Count - _model.Order))
                .Take(_model.Order)
                .ToList();

            if (!_model.TryGetTransitions(state, out _))
                return string.Empty;
        }

        int safetyLimit = Math.Max(16, options.MaxLength * 4);
        int steps = 0;

        while (steps < safetyLimit)
        {
            steps++;

            if (!_model.TryGetTransitions(
                    state,
                    out IReadOnlyDictionary<string, int>? transitions))
            {
                break;
            }

            string next = WeightedChoice.Choose(transitions, _random);

            if (next == TokenMarkovNameModel.EndToken)
                break;

            outputTokens.Add(next);

            string currentOutput = _model.Tokenizer.JoinTokens(outputTokens);

            if (currentOutput.Length >= options.MaxLength)
                break;

            state.RemoveAt(0);
            state.Add(next);
        }

        string raw = _model.Tokenizer.JoinTokens(outputTokens);

        if (options.UseGuidedSuffix &&
            !string.IsNullOrWhiteSpace(options.RequiredSuffix))
        {
            raw = AppendSuffixIfNeeded(raw, options.RequiredSuffix!);
        }

        return ApplyFormatting(raw, options);
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
        {
            throw new ArgumentException(
                "MaxLength must be greater than or equal to MinLength.");
        }

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