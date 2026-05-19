using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class NameTokenizerDto
{
    public string Type { get; set; } = "character";

    public List<string> Tokens { get; set; } = new List<string>();
}