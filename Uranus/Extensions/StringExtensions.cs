namespace Uranus.Extensions;

public static class StringExtensions
{
    public static string PadBoth(this string str, int length)
    {
        var spaces = length - str.Length;
        var padRight = spaces / 2 + str.Length;
        return str.PadRight(padRight).PadLeft(length);
    }
}