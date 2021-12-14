namespace Vheos.Tools.ModdingCore
{
    using System;
    using System.Collections.Generic;
    using BepInEx.Configuration;
    using Tools.ModdingCore;
    using Tools.UtilityN;
    using Tools.Extensions.Collections;

    static public class Presets
    {
        // Constants
        private const string DEFAULT_PRESET_NAME = "-";
        private const string RESET_TO_DEFAULTS_PRESET_NAME = "Reset to defaults";

        // Privates
        static private string[] _presetNames;
        static private ICollection<AMod> _mods;
        static private ModSetting<string> _presetToLoad;
        static private void CreateLoadPresetSetting()
        {
            _presetToLoad = new ModSetting<string>("", nameof(_presetToLoad), DEFAULT_PRESET_NAME, new AcceptableValueList<string>(_presetNames));
            _presetToLoad.Format("Load preset");
            _presetToLoad.Ordering = 1;
            _presetToLoad.IsAdvanced = true;
            _presetToLoad.DisplayResetButton = false;
            _presetToLoad.AddEvent(LoadPreset);
        }
        static private void LoadPreset()
        {
            Action<AMod> invoke = t => t.LoadPreset(_presetToLoad);
            if (_presetToLoad == RESET_TO_DEFAULTS_PRESET_NAME)
                invoke = t => t.ResetSettings(true);

            foreach (var mod in _mods)
                invoke(mod);

            _presetToLoad.SetSilently(DEFAULT_PRESET_NAME);
        }

        // Initializers
        static public void TryInitialize(string[] presetNames, ICollection<AMod> mods)
        {
            if (presetNames.IsNullOrEmpty() || mods.IsNullOrEmpty())
                return;

            _presetNames = new List<string> { DEFAULT_PRESET_NAME, RESET_TO_DEFAULTS_PRESET_NAME, presetNames }.ToArray();
            _mods = mods;
            CreateLoadPresetSetting();
        }
    }
}