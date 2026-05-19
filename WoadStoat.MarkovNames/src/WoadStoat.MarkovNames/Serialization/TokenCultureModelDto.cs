using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenCultureModelDto
{
    public string CultureKey { get; set; } = string.Empty;

    public List<TokenCategoryModelDto> Categories { get; set; }
        = new List<TokenCategoryModelDto>();
}