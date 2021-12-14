namespace Vheos.Tools.ModdingCore
{
    using System;
    using System.Collections.Generic;
    using BepInEx;
    using BepInEx.Configuration;
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
        static internal bool AreSettingLimitsUnlocked
        => _unlockSettingLimits != null && _unlockSettingLimits.Value;
        static private ConfigurationManager.ConfigurationManager _configManager;
        static private bool _isConfigWindowDirty;
        static private ModSetting<bool> _unlockSettingLimits;
        static private void CreateUnlockLimitsSetting()
        {
            _unlockSettingLimits = new ModSetting<bool>("", nameof(_unlockSettingLimits), false);
            _unlockSettingLimits.Format("Unlock settings' limits");
            _unlockSettingLimits.Description = "Each setting that uses a value slider will use an input box instead.\n" +
                                               "This allows you to enter ANY value, unlimited by the slider limits.\n" +
                                               "However, some extreme values on some settings might produce unexpected results or even crash the game.\n" +
                                               "(requires game restart)";
            _unlockSettingLimits.Ordering = 0;
            _unlockSettingLimits.IsAdvanced = true;
            _unlockSettingLimits.DisplayResetButton = false;
        }

        // Initializers
        static public void Initialize(BaseUnityPlugin pluginComponent)
        {
            ConfigFile = pluginComponent.Config;
            _configManager = pluginComponent.GetComponent<ConfigurationManager.ConfigurationManager>();
            CreateUnlockLimitsSetting();
        }
    }
}