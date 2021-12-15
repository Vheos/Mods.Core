﻿namespace Vheos.Tools.ModdingCore
{
    using BepInEx.Configuration;
    using Extensions.Reflection;
    public class ModSetting<T> : AModSetting
    {
        // Publics
        public T Value
        {
            get => _configEntry.Value;
            set => _configEntry.Value = value;
        }
        public T DefaultValue
        => (T)_configEntry.DefaultValue;
        public void SetSilently(T value)
        {
            _configEntry.SetField("_typedValue", value);
            _configEntry.ConfigFile.Save();
        }

        // Privates
        private readonly ConfigEntry<T> _configEntry;

        // Constructors
        public ModSetting(string section, string name, T defaultValue = default, AcceptableValueBase acceptableValues = null) : base()
        {
            if (ConfigHelper.AreSettingLimitsUnlocked)
                acceptableValues = null;

            ConfigDescription description = new ConfigDescription("", acceptableValues, new ConfigurationManagerAttributes());
            _configEntryBase = _configEntry = ConfigHelper.ConfigFile.Bind(section, name, defaultValue, description);
            IsVisible = true;
        }

        // Operators
        public static implicit operator T(ModSetting<T> setting)
        => setting.Value;
    }
}