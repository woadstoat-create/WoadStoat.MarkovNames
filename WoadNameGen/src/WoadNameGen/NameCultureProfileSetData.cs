using System.Collections.Generic;

namespace WoadNameGen;

public sealed class NameCultureProfileSetData
{
    public List<NameCultureProfileData> Profiles { get; set; }
        = new List<NameCultureProfileData>();
}