namespace Vheos.Mods.Core;
using BepInEx;

static public class ConfigHelper
{
    // Publics
    static public void AddEventOnConfigOpened(Action action)
    {
        _configManager.DisplayingWindowChanged += (sender, eventArgs) =>
        {
            if (eventArgs.NewValue)
                action();
        };
    }
    static public void AddEventOnConfigClosed(Action action)
    {
        _configManager.DisplayingWindowChanged += (sender, eventArgs) =>
        {
            if (!eventArgs.NewValue)
                action();
        };
    }
    static public bool IsConfigOpen
    {
        get => _configManager.DisplayingWindow;
        set => _configManager.DisplayingWindow = value;
    }
    static public bool AdvancedSettings
    {
        get => _configManager._showAdvanced.Value;
        set => _configManager._showAdvanced.Value = value;
    }

    // Privates
    static internal ConfigFile ConfigFile
    { get; private set; }
    static internal void SetDirtyConfigWindow()
    => _isConfigWindowDirty = true;
    static internal void TryRedrawConfigWindow()
    {
        if (IsConfigOpen && _isConfigWindowDirty)
        {
            _configManager.BuildSettingList();
            _isConfigWindowDirty = false;
        }
    }
    static private ConfigurationManager.ConfigurationManager _configManager;
    static private bool _isConfigWindowDirty;
    static public ModSetting<bool> UnlockSettingLimits
    { get; private set; }
    static public ModSetting<int> NumericalColorRange
    { get; private set; }
    static private void CreateUnlockLimitsSetting()
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
    static private void CreateNumericalColorEditingSetting()
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
    static public void Initialize(BaseUnityPlugin pluginComponent)
    {
        ConfigFile = pluginComponent.Config;
        _configManager = pluginComponent.GetComponent<ConfigurationManager.ConfigurationManager>();
        CreateUnlockLimitsSetting();
        CreateNumericalColorEditingSetting();
    }
}
