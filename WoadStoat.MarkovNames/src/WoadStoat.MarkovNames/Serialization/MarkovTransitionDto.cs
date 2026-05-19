using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class MarkovTransitionDto
{
    public string State { get; set; } = string.Empty;

    public List<MarkovNextCharacterDto> NextCharacters { get; set; }
        = new List<MarkovNextCharacterDto>();
}