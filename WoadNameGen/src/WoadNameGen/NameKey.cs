using System;

namespace WoadNameGen;

internal static class NameKey
{
    public static string Normalise(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Key cannot be null, empty, or whitespace.", parameterName);

        return value.Trim().ToLowerInvariant();
    }
}