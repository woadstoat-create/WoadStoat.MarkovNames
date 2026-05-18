namespace WoadNameGen;

public interface IRandomSource
{
    int NextInt(int minInclusive, int maxExclusive);
}