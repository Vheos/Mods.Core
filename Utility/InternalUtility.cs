using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;



namespace Vheos.Tools.ModdingCore
{
    static internal class InternalUtility
    {
        static public string SplitCamelCase(this string t)
        => Regex.Replace(t, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
    }
}