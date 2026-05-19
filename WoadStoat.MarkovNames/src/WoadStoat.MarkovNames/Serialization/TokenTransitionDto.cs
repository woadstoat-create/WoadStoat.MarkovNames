using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenTransitionDto
{
    public string State { get; set; } = string.Empty;

    public List<TokenNextTokenDto> NextTokens { get; set; }
        = new List<TokenNextTokenDto>();
}