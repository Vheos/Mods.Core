using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;



namespace Vheos.ModdingCore
{
    static internal class Utility
    {
        static public IEnumerable<Type> GetDerivedTypes<T>()
        => Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(T)));
    }
}