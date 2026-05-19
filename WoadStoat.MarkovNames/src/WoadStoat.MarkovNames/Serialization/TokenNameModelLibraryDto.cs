using System.Collections.Generic;

namespace WoadStoat.MarkovNames.Serialization;

internal sealed class TokenNameModelLibraryDto
{
    public int Order { get; set; }

    public List<TokenCultureModelDto> Cultures { get; set; }
        = new List<TokenCultureModelDto>();
}