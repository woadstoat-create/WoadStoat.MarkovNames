using System.Collections.Generic;

namespace WoadStoat.MarkovNames;

public sealed class NameCultureProfileSetData
{
    public List<NameCultureProfileData> Profiles { get; set; }
        = new List<NameCultureProfileData>();
}