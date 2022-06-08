namespace Vheos.Mods.Core
{
    using System;
    using System.Collections.Generic;
    using BepInEx.Configuration;
    using Mods.Core;
    using Tools.Utilities;
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
                foreach (var kvp in _keyCodesByName)
                    yield return kvp.Value;
            }
        }
        static public IEnumerable<string> AllStrings
        {
            get
            {
                foreach (var kvp in _keyCodesByName)
                    yield return kvp.Key;
            }
        }

        // Extensions
        static public bool IsValidKeyCode(this string t)
            => _keyCodesByName.ContainsKey(t);
        static public KeyCode ToKeyCode(this string t)
            => _keyCodesByName.TryGet(t, out var keyCode) ? keyCode : KeyCode.None;

        static public bool IsValidKeyCode(this ModSetting<string> t)
            => t != null && t.Value.IsValidKeyCode();
        static public KeyCode ToKeyCode(this ModSetting<string> t)
            => t != null ? t.Value.ToKeyCode() : KeyCode.None;
    }
}