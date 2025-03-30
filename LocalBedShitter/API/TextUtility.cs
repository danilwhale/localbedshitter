using System.Text.RegularExpressions;

namespace LocalBedShitter.API;

public static partial class TextUtility
{
    public static string Sanitize(string source)
    {
        return ColorCodeRegex().Replace(source, "");
    }
    
    [GeneratedRegex("&[0-9a-fA-F]")]
    private static partial Regex ColorCodeRegex();
}