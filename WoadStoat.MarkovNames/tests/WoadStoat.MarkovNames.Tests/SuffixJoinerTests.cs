using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class SuffixJoinerTests
{
    [Fact]
    public void AppendModeAppendsSuffix()
    {
        string result = SuffixJoiner.Join(
            "aureli",
            "ius",
            SuffixJoinMode.Append);

        Assert.Equal("aureliius", result);
    }

    [Fact]
    public void SuffixIsNotDuplicatedWhenAlreadyPresent()
    {
        string result = SuffixJoiner.Join(
            "severus",
            "us",
            SuffixJoinMode.Append);

        Assert.Equal("severus", result);
    }

    [Fact]
    public void MergeOverlappingCharacterMergesSingleCharacter()
    {
        string result = SuffixJoiner.Join(
            "marcu",
            "us",
            SuffixJoinMode.MergeOverlappingCharacter);

        Assert.Equal("marcus", result);
    }

    [Fact]
    public void MergeOverlappingSubstringMergesLongestOverlap()
    {
        string result = SuffixJoiner.Join(
            "aureli",
            "ius",
            SuffixJoinMode.MergeOverlappingSubstring);

        Assert.Equal("aurelius", result);
    }

    [Fact]
    public void MergeOverlappingSubstringFallsBackToAppendWhenNoOverlapExists()
    {
        string result = SuffixJoiner.Join(
            "flav",
            "ius",
            SuffixJoinMode.MergeOverlappingSubstring);

        Assert.Equal("flavius", result);
    }

    [Fact]
    public void MergeOverlappingSubstringIsCaseInsensitive()
    {
        string result = SuffixJoiner.Join(
            "Aureli",
            "IUS",
            SuffixJoinMode.MergeOverlappingSubstring);

        Assert.Equal("Aurelius", result);
    }
}