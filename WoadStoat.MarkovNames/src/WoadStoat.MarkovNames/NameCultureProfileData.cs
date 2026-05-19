using System.Collections.Generic;

namespace WoadStoat.MarkovNames;

public sealed class NameCultureProfileData
{
    public string CultureKey { get; set; } = string.Empty;

    public int Order { get; set; } = 2;

    public bool UseTokens { get; set; }

    public List<string> Tokens { get; set; } = new List<string>();

    public Dictionary<string, List<string>> Categories { get; set; }
        = new Dictionary<string, List<string>>();
}