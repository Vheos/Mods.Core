﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Diagnostics;



namespace Vheos.Tools.ModdingCore
{
    static public class Tools
    {
        // Publics
        static public void Log(object text)
        => _logger.Log(LogLevel.Debug, text);
        static public ConfigFile ConfigFile
        { get; private set; }
        static public void SetDirtyConfigWindow()
        => _isConfigWindowDirty = true;
        static public void TryRedrawConfigWindow()
        {
            if (IsConfigOpen && _isConfigWindowDirty)
            {
                _configManager.BuildSettingList();
                _isConfigWindowDirty = false;
            }
        }
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
        static public bool AreSettingLimitsUnlocked
        => _unlockSettingLimits;

        // Privates
        static private ManualLogSource _logger;
        static private ConfigurationManager.ConfigurationManager _configManager;
        static private bool _isConfigWindowDirty;
        static private ModSetting<bool> _unlockSettingLimits;
        static private void CreateUnlockLimitsSetting()
        {
            _unlockSettingLimits = new ModSetting<bool>("", nameof(_unlockSettingLimits), false);
            _unlockSettingLimits.Format("Unlock settings' limits");
            _unlockSettingLimits.Description = "Each setting that uses a value slider will use an input box instead\n" +
                                               "This allows you to enter ANY value, unlimited by the slider limits.\n" +
                                               "However, some extreme values on some settings might produce unexpected results or even crash the game.\n" +
                                               "(requires game restart)";
            _unlockSettingLimits.IsAdvanced = true;
            _unlockSettingLimits.DisplayResetButton = false;
        }

        // Initializers
        static public void Initialize(BaseUnityPlugin pluginComponent, ManualLogSource logger)
        {
            _logger = logger;
            ConfigFile = pluginComponent.Config;
            _configManager = pluginComponent.GetComponent<ConfigurationManager.ConfigurationManager>();
            CreateUnlockLimitsSetting();
        }
    }
}