namespace Vheos.Mods.Core;
using BepInEx;

public static class ConfigHelper
{
    // Publics
    public static void AddEventOnConfigOpened(Action action)
    {
        _configManager.DisplayingWindowChanged += (sender, eventArgs) =>
        {
            if (eventArgs.NewValue)
                action();
        };
    }
    public static void AddEventOnConfigClosed(Action action)
    {
        _configManager.DisplayingWindowChanged += (sender, eventArgs) =>
        {
            if (!eventArgs.NewValue)
                action();
        };
    }
    public static bool IsConfigOpen
    {
        get => _configManager.DisplayingWindow;
        set => _configManager.DisplayingWindow = value;
    }
    public static bool AdvancedSettings
    {
        get => _configManager._showAdvanced.Value;
        set => _configManager._showAdvanced.Value = value;
    }

    // Privates
    internal static ConfigFile ConfigFile
    { get; private set; }
    internal static void SetDirtyConfigWindow()
    => _isConfigWindowDirty = true;
    internal static void TryRedrawConfigWindow()
    {
        if (IsConfigOpen && _isConfigWindowDirty)
        {
            _configManager.BuildSettingList();
            _isConfigWindowDirty = false;
        }
    }
    private static ConfigurationManager.ConfigurationManager _configManager;
    private static bool _isConfigWindowDirty;
    public static ModSetting<bool> UnlockSettingLimits
    { get; private set; }
    public static ModSetting<int> NumericalColorRange
    { get; private set; }
    private static void CreateUnlockLimitsSetting()
    {
        UnlockSettingLimits = new ModSetting<bool>("", nameof(UnlockSettingLimits), false);
        UnlockSettingLimits.Format("Unlock settings' limits");
        UnlockSettingLimits.Description =
            "Each setting that uses a value slider will use an input box instead" +
            "\nThis allows you to enter ANY value, unlimited by the slider limits" +
            "\nHowever, some extreme values on some settings might produce unexpected results or even crash the game" +
            "\n(requires game restart to take effect)";
        UnlockSettingLimits.Ordering = 0;
        UnlockSettingLimits.IsAdvanced = true;
        UnlockSettingLimits.DisplayResetButton = false;
    }
    private static void CreateNumericalColorEditingSetting()
    {
        NumericalColorRange = new ModSetting<int>("", nameof(NumericalColorRange), 0, new AcceptableValueRange<int>(0, 255));
        NumericalColorRange.Format("Numerical color range");
        NumericalColorRange.Description =
            "";
        NumericalColorRange.Ordering = 1;
        NumericalColorRange.IsAdvanced = true;
        NumericalColorRange.DisplayResetButton = false;
    }

    // Initializers
    public static void Initialize(BaseUnityPlugin pluginComponent)
    {
        ConfigFile = pluginComponent.Config;
        _configManager = pluginComponent.GetComponent<ConfigurationManager.ConfigurationManager>();
        CreateUnlockLimitsSetting();
        CreateNumericalColorEditingSetting();
    }
}
