using System.Collections.Generic;

namespace WoadNameGen.Serialization;

internal sealed class NameModelLibraryDto
{
    public int Order { get; set; }

    public List<NameCultureModelDto> Cultures { get; set; }
        = new List<NameCultureModelDto>();
}