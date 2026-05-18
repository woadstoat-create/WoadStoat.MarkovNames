namespace WoadNameGen.Serialization;

internal sealed class MarkovNextCharacterDto
{
    public string Character { get; set; } = string.Empty;

    public int Weight { get; set; }
}