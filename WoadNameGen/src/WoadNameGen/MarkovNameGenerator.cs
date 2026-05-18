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

            if (candidate.Length < options.MinLength)
                continue;

            if (candidate.Length > options.MaxLength)
                continue;

            if (options.AvoidTrainingDuplicates && _model.WasInTrainingData(candidate))
                continue;

            return candidate;
        }

        throw new InvalidOperationException(
            $"Failed to generate a valid name after {options.MaxAttempts} attempts. " +
            "Try relaxing length constraints or allowing training duplicates."
        );
    }

    public IReadOnlyList<string> GenerateMany(
        int count,
        NameGenerationOptions? options = null)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        options ??= new NameGenerationOptions();

        List<string> results = new(count);
        HashSet<string> used = new(StringComparer.OrdinalIgnoreCase);

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
        string state = new(MarkovNameModel.StartToken, _model.Order);
        StringBuilder builder = new();

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

        return ApplyFormatting(builder.ToString(), options);
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
    }
}