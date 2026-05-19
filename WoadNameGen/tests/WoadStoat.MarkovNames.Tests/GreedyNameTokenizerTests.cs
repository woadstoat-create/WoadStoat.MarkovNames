using WoadStoat.MarkovNames;

namespace WoadStoat.MarkovNames.Tests;

public sealed class GreedyNameTokenizerTests
{
    [Fact]
    public void GreedyTokenizerUsesLongestMatchingTokens()
    {
        GreedyNameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "mac",
            "ae",
            "eo",
            "ch"
        });

        IReadOnlyList<string> tokens = tokenizer.Tokenize("MacLeod");

        Assert.Equal(
            new[] { "mac", "l", "eo", "d" },
            tokens);
    }

    [Fact]
    public void CharacterTokenizerSplitsIntoSingleCharacters()
    {
        CharacterNameTokenizer tokenizer = new CharacterNameTokenizer();

        IReadOnlyList<string> tokens = tokenizer.Tokenize("Aed");

        Assert.Equal(
            new[] { "A", "e", "d" },
            tokens);
    }

    [Fact]
    public void GreedyTokenizerFallsBackToSingleCharacters()
    {
        GreedyNameTokenizer tokenizer = new GreedyNameTokenizer(new[]
        {
            "kh",
            "zh"
        });

        IReadOnlyList<string> tokens = tokenizer.Tokenize("Kharon");

        Assert.Equal(
            new[] { "kh", "a", "r", "o", "n" },
            tokens);
    }
}