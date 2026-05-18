using System.Collections.Generic;

namespace WoadNameGen.Serialization;

internal sealed class NameCultureModelDto
{
    public string CultureKey { get; set; } = string.Empty;

    public List<NameCategoryModelDto> Categories { get; set; }
        = new List<NameCategoryModelDto>();
}