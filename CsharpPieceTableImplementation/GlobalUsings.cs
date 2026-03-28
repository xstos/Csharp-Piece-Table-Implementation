global using CommunityToolkit.Diagnostics;

public static class Ext
{
    public static string ToStr(this Span<char> chars) => new(chars);
}