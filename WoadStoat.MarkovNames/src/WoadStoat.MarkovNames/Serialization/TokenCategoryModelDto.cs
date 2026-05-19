namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenCategoryModelDto
{
    public string CategoryKey { get; set; } = string.Empty;

    public TokenNameModelDto Model { get; set; } = new TokenNameModelDto();
}