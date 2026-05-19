using System;

namespace WoadStoat.MarkovNames;

internal static class SuffixJoiner
{
    public static string Join(string value, string suffix, SuffixJoinMode mode)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return value;

        string trimmedSuffix = suffix.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(value))
            return trimmedSuffix;

        if (value.EndsWith(trimmedSuffix, StringComparison.OrdinalIgnoreCase))
            return value;

        return mode switch
        {
            SuffixJoinMode.Append => value + trimmedSuffix,

            SuffixJoinMode.MergeOverlappingCharacter =>
                JoinWithSingleCharacterOverlap(value, trimmedSuffix),

            SuffixJoinMode.MergeOverlappingSubstring =>
                JoinWithLongestSubstringOverlap(value, trimmedSuffix),

            _ => value + trimmedSuffix
        };
    }

    private static string JoinWithSingleCharacterOverlap(string value, string suffix)
    {
        if (value.Length == 0 || suffix.Length == 0)
            return value + suffix;

        char lastValueChar = char.ToLowerInvariant(value[value.Length - 1]);
        char firstSuffixChar = char.ToLowerInvariant(suffix[0]);

        if (lastValueChar == firstSuffixChar)
            return value + suffix.Substring(1);

        return value + suffix;
    }

    private static string JoinWithLongestSubstringOverlap(string value, string suffix)
    {
        int maxOverlap = Math.Min(value.Length, suffix.Length);

        for (int overlapLength = maxOverlap; overlapLength > 0; overlapLength--)
        {
            if (HasOverlap(value, suffix, overlapLength))
                return value + suffix.Substring(overlapLength);
        }

        return value + suffix;
    }

    private static bool HasOverlap(string value, string suffix, int overlapLength)
    {
        int valueStartIndex = value.Length - overlapLength;

        return string.Compare(
            value,
            valueStartIndex,
            suffix,
            0,
            overlapLength,
            StringComparison.OrdinalIgnoreCase) == 0;
    }
}