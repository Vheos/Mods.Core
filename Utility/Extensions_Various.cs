using System;
using System.Text.RegularExpressions;



namespace Vheos.ModdingCore
{
    static internal class Extensions_Various
    {
        static public bool HasFlagsAttribute(this Enum enumeration)
        => enumeration.GetType().IsDefined(typeof(FlagsAttribute), false);
        static public string SplitCamelCase(this string t)
        => Regex.Replace(t, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        static public bool IsEmpty(this string text)
        => string.IsNullOrEmpty(text);
        static public bool IsNotEmpty(this string text)
        => !string.IsNullOrEmpty(text);
    }
}