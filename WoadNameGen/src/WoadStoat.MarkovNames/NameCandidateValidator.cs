namespace WoadStoat.MarkovNames;

internal static class NameCandidateValidator
{
    public static bool IsValid(
        string candidate,
        NameGenerationOptions options,
        Func<string, bool> wasInTrainingData)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return false;

        if (candidate.Length < options.MinLength)
            return false;

        if (candidate.Length > options.MaxLength)
            return false;

        if (options.AvoidTrainingDuplicates && wasInTrainingData(candidate))
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
}