namespace Vheos.Mods.Core;

public abstract class AModSetting
{
    // Publics 
    public void Format(string displayName)
    {
        Name = displayName;
        FormattedName = "";
        if (displayName.IsNotEmpty())
        {
            FormattedName = FormattedName.PadLeft(5 * AMod.IndentLevel, ' ');
            if (AMod.IndentLevel > 0)
                FormattedName += "• ";
            FormattedName += Name;
        }

        Ordering = AMod.NextPosition;
        _visibilityCheck = () => true;
    }
    public void Format<T>(string displayName, ModSetting<T> controller, Func<T, bool> check = null)
    {
        Format(displayName);
        AddVisibilityControl(controller, check);
    }
    public void Format<T>(string displayName, ModSetting<T> controller, T value, bool positiveTest = true) where T : struct
    {
        Format(displayName);
        AddVisibilityControl(controller, value, positiveTest);
    }
    public void Format(string displayName, ModSetting<bool> toggle)
    => Format(displayName, toggle, true);
    public void AddEvent(Action action)
    {
        AddEventSilently(action);
        _events.Add(action);
    }
    public void AddEventSilently(Action action)
    {
        _configEntryBase.ConfigFile.SettingChanged += (sender, eventArgs) =>
        {
            if (eventArgs.ChangedSetting == _configEntryBase)
                action();
        };
    }
    public void Reset()
    => _configEntryBase.BoxedValue = _configEntryBase.DefaultValue;
    public T CastValue<T>()
    => (T)_configEntryBase.BoxedValue;

    // Publics (attributes)
    public string Name
    { get; private set; }
    public string Key
    => _configEntryBase.Definition.Key;
    public string Section
    => _configEntryBase.Definition.Section;
    public string FormattedSection
    {
        get => Attributes.Category;
        set => Attributes.Category = value;
    }
    public string FormattedName
    {
        get => Attributes.DispName;
        set => Attributes.DispName = value;
    }
    public string Description
    {
        get => Attributes.Description;
        set => Attributes.Description = value;
    }
    public int Ordering
    {
        get => -(int)Attributes.Order;
        set => Attributes.Order = -value;
    }
    public bool IsVisible
    {
        get => Attributes.Browsable.HasValue && Attributes.Browsable.Value;
        set => Attributes.Browsable = value;
    }
    public bool IsAdvanced
    {
        get => (bool)Attributes.IsAdvanced;
        set => Attributes.IsAdvanced = value;
    }
    public bool DisplayResetButton
    {
        get => !(Attributes.HideDefaultButton.HasValue && Attributes.HideDefaultButton.Value);
        set => Attributes.HideDefaultButton = !value;
    }
    public bool DrawInPlaceOfName
    {
        get => Attributes.HideSettingName.HasValue && Attributes.HideSettingName.Value;
        set => Attributes.HideSettingName = value;
    }
    public bool FormatAsPercent01
    {
        get => Attributes.ShowRangeAsPercent.HasValue && Attributes.ShowRangeAsPercent.Value;
        set => Attributes.ShowRangeAsPercent = value;
    }
    public bool ReadOnly
    {
        get => Attributes.ReadOnly.HasValue && Attributes.ReadOnly.Value;
        set => Attributes.ReadOnly = value;
    }
    public Action<ConfigEntryBase> CustomDrawer
    {
        get => Attributes.CustomDrawer;
        set => Attributes.CustomDrawer = value;
    }

    // Privates     
    internal void CallAllEvents()
    {
        foreach (var action in _events)
            action();
    }
    internal void UpdateVisibility()
    {
        ConfigHelper.SetDirtyConfigWindow();

        foreach (var controller in _visibilityControllers)
            if (!controller.IsVisible)
            {
                IsVisible = false;
                return;
            }

        IsVisible = _visibilityCheck();
    }
    protected ConfigEntryBase _configEntryBase;
    protected List<Action> _events;
    private ConfigurationManagerAttributes Attributes
    => _configEntryBase.Description.Tags[0] as ConfigurationManagerAttributes;

    // Visibility control
    private Func<bool> _visibilityCheck;
    private readonly List<AModSetting> _visibilityControllers;
    private void AddVisibilityControl<T>(ModSetting<T> controller, Func<T, bool> check = null)
    {
        AddParentVisibilityControllers(controller);
        if (check != null)
            _visibilityCheck = () => check(controller.Value);

        foreach (var visibilityController in _visibilityControllers)
            _configEntryBase.ConfigFile.SettingChanged += (sender, eventArgs) =>
            {
                if (eventArgs.ChangedSetting == visibilityController._configEntryBase)
                    UpdateVisibility();
            };
    }
    private void AddVisibilityControl<T>(ModSetting<T> controller, T value, bool positiveTest = true) where T : struct
    {
        Func<T, bool> check = value is Enum valueAsEnum && valueAsEnum.HasFlagsAttribute()
            ? positiveTest
                ? (v => (v as Enum).HasFlag(valueAsEnum))
                : (v => !(v as Enum).HasFlag(valueAsEnum))
            : positiveTest
                ? (v => v.Equals(value))
                : (v => !v.Equals(value));

        AddVisibilityControl(controller, check);
    }
    private void AddParentVisibilityControllers(AModSetting controller)
    {
        _visibilityControllers.Add(controller);
        foreach (var newParentController in controller._visibilityControllers)
            _visibilityControllers.Add(newParentController);
    }


    // Constructors
    protected AModSetting()
    {
        _events = new List<Action>();
        _visibilityControllers = new List<AModSetting>();
        _visibilityCheck = () => false;
    }
}
