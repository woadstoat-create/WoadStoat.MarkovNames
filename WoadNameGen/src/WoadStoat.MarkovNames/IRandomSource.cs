namespace WoadStoat.MarkovNames;

public interface IRandomSource
{
    int NextInt(int minInclusive, int maxExclusive);
}