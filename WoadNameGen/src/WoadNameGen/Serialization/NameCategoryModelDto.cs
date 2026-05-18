namespace WoadNameGen.Serialization;

internal sealed class NameCategoryModelDto
{
    public string CategoryKey { get; set; } = string.Empty;

    public MarkovNameModelDto Model { get; set; }
        = new MarkovNameModelDto();
}