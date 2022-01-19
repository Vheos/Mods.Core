namespace Vheos.Mods.Core
{
    using System;
    using System.Collections.Generic;
    using BepInEx.Configuration;
    using Mods.Core;
    using Tools.UtilityN;
    using Tools.Extensions.Collections;
    using UnityEngine;

    static public class KeyCodeCache
    {
        // Privates
        static private Dictionary<string, KeyCode> _keyCodesByName;

        // Publics
        static public void Initialize()
        {
            _keyCodesByName = new Dictionary<string, KeyCode>();
            foreach (var keyCodeName in Utility.GetEnumNames<KeyCode>())
                _keyCodesByName.Add(keyCodeName, Utility.ParseEnum<KeyCode>(keyCodeName));
        }
        static public IEnumerable<KeyCode> AllKeyCodes
        {
            get
            {
                foreach (var keyCode in _keyCodesByName)
                    yield return keyCode.Value;
            }
        }
        static public IEnumerable<string> AllStrings
        {
            get
            {
                foreach (var keyCode in _keyCodesByName)
                    yield return keyCode.Key;
            }
        }

        // Extensions
        static public KeyCode ToKeyCode(this string t)
        {
            if (_keyCodesByName.TryGet(t, out var keyCode))
                return keyCode;
            return KeyCode.None;
        }
        static public KeyCode ToKeyCode(this ModSetting<string> t)
        {
            if (t != null)
                return t.Value.ToKeyCode();
            return KeyCode.None;
        }

    }
}