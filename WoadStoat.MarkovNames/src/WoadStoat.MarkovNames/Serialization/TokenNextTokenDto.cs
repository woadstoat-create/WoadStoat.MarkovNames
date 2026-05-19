namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenNextTokenDto
{
    public string Token { get; set; } = string.Empty;

    public int Weight { get; set; }
}