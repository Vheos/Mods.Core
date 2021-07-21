namespace Vheos.Tools.ModdingCore
{
    using System;
    using System.Collections.Generic;
    using BepInEx.Configuration;
    using HarmonyLib;
    using Extensions.General;
    using Extensions.Math;
    public abstract class AMod
    {
        #region const
        private const int MAX_SETTINGS_PER_MOD = 1000;
        #endregion
        #region enum
        [Flags]
        protected enum Toggles
        {
            None = 0,
            Apply = 1 << 1,
            Collapse = 1 << 2,
            Hide = 1 << 3,
        }
        #endregion

        // Privates
        private readonly Harmony _patcher;
        private readonly List<AModSetting> _settings;
        private readonly List<Action> _onConfigClosedEvents;
        private readonly List<Action> _onEnabledEvents;
        private readonly List<Action> _onDisabledEvents;
        private bool _isInitialized;
        static private Type[] _modsOrderingList;
        private string SectionName
        => GetType().Name;
        private int ModOrderingOffset
        => _modsOrderingList != null && GetType().IsContainedIn(_modsOrderingList)
         ? Array.IndexOf(_modsOrderingList, GetType()).Add(1).Mul(MAX_SETTINGS_PER_MOD)
         : 0;           
        virtual protected string SectionOverride
        => "";
        virtual protected string Description
        => "";
        virtual protected string ModName
        => null;

        // Toggles
        private ModSetting<Toggles> _mainToggle;
        private Toggles _previousMainToggle;
        private void CreateMainToggle()
        {
            _mainToggle = new ModSetting<Toggles>(SectionName, nameof(_mainToggle), Toggles.None)
            {
                SectionOverride = SectionOverride,
                DisplayResetButton = false,
                Description = Description,
            };
            _mainToggle.Format(ModName ?? SectionName.SplitCamelCase());
            _mainToggle.AddEvent(OnTogglesChanged);
            _previousMainToggle = _mainToggle;
        }
        private void OnTogglesChanged()
        {
            Toggles option = _mainToggle ^ _previousMainToggle;
            bool newState = _mainToggle.Value.HasFlag(option);
            switch (option)
            {
                case Toggles.Apply:
                    if (IsHidden)
                        ResetApplySilently();
                    else if (newState)
                        OnEnable();
                    else
                        OnDisable();
                    break;

                case Toggles.Collapse:
                    if (!IsEnabled || IsHidden)
                        ResetCollapseSilently();
                    else if (newState)
                        OnCollapse();
                    else
                        OnExpand();
                    break;

                case Toggles.Hide:
                    if (newState)
                        OnHide();
                    else
                        OnUnhide();
                    break;
            }

            _previousMainToggle = _mainToggle;
            ConfigHelper.SetDirtyConfigWindow();
        }
        private void OnEnable()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                ResetSettingPosition();

                Log.Debug($"\t[{GetType().Name}] Initializing...");
                Initialize();
                Indent++;
                Log.Debug($"\t[{GetType().Name}] Formatting...");
                SetFormatting();
                Indent--;
            }

            Log.Debug($"\t[{GetType().Name}] Patching...");
            _patcher.PatchAll(GetType());

            Log.Debug($"\t[{GetType().Name}] Calling events...");
            foreach (var onEnabled in _onEnabledEvents)
                onEnabled.Invoke();
            foreach (var setting in _settings)
                setting.CallAllEvents();
            foreach (var onConfigClosed in _onConfigClosedEvents)
                onConfigClosed.Invoke();

            OnExpand();
        }
        private void OnDisable()
        {
            foreach (var onDisabled in _onDisabledEvents)
                onDisabled.Invoke();

            _patcher.UnpatchSelf();
            if (!IsCollapsed)
                OnCollapse();
            else
                ResetCollapseSilently();
        }
        private void OnCollapse()
        {
            foreach (var setting in _settings)
                setting.IsVisible = false;
        }
        private void OnExpand()
        {
            foreach (var setting in _settings)
                setting.UpdateVisibility();
        }
        private void OnHide()
        {
            _mainToggle.IsAdvanced = true;
            if (IsEnabled)
            {
                OnDisable();
                ResetApplySilently();
            }
        }
        private void OnUnhide()
        {
            _mainToggle.IsAdvanced = false;
        }
        private void ResetApplySilently()
        => _mainToggle.SetSilently(_mainToggle & ~Toggles.Apply);
        private void ResetCollapseSilently()
        => _mainToggle.SetSilently(_mainToggle & ~Toggles.Collapse);
        private void ResetHideSilently()
        => _mainToggle.SetSilently(_mainToggle & ~Toggles.Hide);

        // Constructors
        protected AMod()
        {
            _patcher = new Harmony(GetType().Name);
            _settings = new List<AModSetting>();
            _onConfigClosedEvents = new List<Action>();
            _onEnabledEvents = new List<Action>();
            _onDisabledEvents = new List<Action>();

            ResetSettingPosition(-1);
            CreateMainToggle();
            Log.Debug($"\t[{GetType().Name}] Main toggle: {_mainToggle.Value}");

            if (IsEnabled)
                OnEnable();
            if (IsCollapsed)
                OnCollapse();
            if (IsHidden)
                OnHide();
        }
        abstract protected void Initialize();
        abstract protected void SetFormatting();
        virtual public void LoadPreset(int preset)
        { }

        // Utility     
        public bool IsEnabled
        {
            get => _mainToggle.Value.HasFlag(Toggles.Apply);
            protected set
            {
                if (value)
                    _mainToggle.Value |= Toggles.Apply;
                else
                    _mainToggle.Value &= ~Toggles.Apply;
            }
        }
        protected bool IsCollapsed
        {
            get => _mainToggle.Value.HasFlag(Toggles.Collapse);
            set
            {
                if (value)
                    _mainToggle.Value |= Toggles.Collapse;
                else
                    _mainToggle.Value &= ~Toggles.Collapse;
            }
        }
        protected bool IsHidden
        {
            get => _mainToggle.Value.HasFlag(Toggles.Hide);
            set
            {
                if (value)
                    _mainToggle.Value |= Toggles.Hide;
                else
                    _mainToggle.Value &= ~Toggles.Hide;
            }
        }
        protected void ForceApply()
        {
            IsHidden = false;
            IsEnabled = true;
            IsCollapsed = true;
        }
        public void ResetSettings(bool resetMainToggle = false)
        {
            foreach (var setting in _settings)
                setting.Reset();

            if (resetMainToggle)
            {
                IsEnabled = false;
                IsCollapsed = false;
                IsHidden = false;
            }
        }
        protected void ResetSettingPosition(int offset = 0)
        => AModSetting.NextPosition = ModOrderingOffset + offset;
        protected int Indent
        {
            get => AModSetting.Indent;
            set => AModSetting.Indent = value;
        }
        protected void AddEventOnConfigOpened(Action action)
        {
            _onConfigClosedEvents.Add(action);
            ConfigHelper.AddEventOnConfigOpened(() =>
            {
                if (IsEnabled)
                    action();
            });
        }
        protected void AddEventOnConfigClosed(Action action)
        {
            _onConfigClosedEvents.Add(action);
            ConfigHelper.AddEventOnConfigClosed(() =>
            {
                if (IsEnabled)
                    action();
            });
        }
        protected void AddEventOnEnabled(Action action)
        => _onEnabledEvents.Add(action);
        protected void AddEventOnDisabled(Action action)
        => _onDisabledEvents.Add(action);
        protected ModSetting<T> CreateSetting<T>(string name, T defaultValue = default, AcceptableValueBase acceptableValues = null)
        {
            ModSetting<T> newSetting = new ModSetting<T>(SectionName, name, defaultValue, acceptableValues)
            {
                SectionOverride = SectionOverride,
                FormatAsPercent = false,
            };
            _settings.Add(newSetting);
            return newSetting;
        }
        protected AcceptableValueRange<int> IntRange(int from, int to)
        => new AcceptableValueRange<int>(from, to);
        protected AcceptableValueRange<float> FloatRange(float from, float to)
        => new AcceptableValueRange<float>(from, to);
        static public void SetOrderingList(params Type[] modTypes)
        => _modsOrderingList = modTypes;
    }
}