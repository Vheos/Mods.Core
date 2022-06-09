namespace Vheos.Mods.Core;

public static class Presets
{
    // Constants
    private const string DEFAULT_PRESET_NAME = "-";
    private const string RESET_TO_DEFAULTS_PRESET_NAME = "Reset to defaults";

    // Privates
    private static string[] _presetNames;
    private static ICollection<AMod> _mods;
    public static ModSetting<string> Preset
    { get; private set; }
    private static void CreateLoadPresetSetting()
    {
        Preset = new ModSetting<string>("", nameof(Preset), DEFAULT_PRESET_NAME, new AcceptableValueList<string>(_presetNames));
        Preset.Format("Load preset");
        Preset.Ordering = 2;
        Preset.IsAdvanced = true;
        Preset.DisplayResetButton = false;
        Preset.AddEvent(LoadPreset);
    }
    private static void LoadPreset()
    {
        Action<AMod> invoke = t => t.LoadPreset(Preset);
        if (Preset == RESET_TO_DEFAULTS_PRESET_NAME)
            invoke = t => t.ResetSettings(true);

        foreach (var mod in _mods)
            invoke(mod);

        Preset.SetSilently(DEFAULT_PRESET_NAME);
    }

    // Initializers
    public static void TryInitialize(string[] presetNames, ICollection<AMod> mods)
    {
        if (presetNames.IsNullOrEmpty() || mods.IsNullOrEmpty())
            return;

        _presetNames = new List<string> { DEFAULT_PRESET_NAME, RESET_TO_DEFAULTS_PRESET_NAME, presetNames }.ToArray();
        _mods = mods;
        CreateLoadPresetSetting();
    }
}
