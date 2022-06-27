namespace Vheos.Mods.Core;

public abstract class PerValueSettings<TMod, TValue> where TMod : AMod
{
    // Settings
    public ModSetting<bool> Header;
    public PerValueSettings(TMod mod, TValue value, bool isToggle = false)
    {
        _mod = mod;
        _value = value;
        _isToggle = isToggle;

        if (_isToggle)
            Header = _mod.CreateSetting(Prefix + nameof(Header), false);
    }
    public virtual void Format()
    {
        if (_isToggle)
            Header.Format(_value.ToString());
        else
            Header = _mod.CreateHeader(_value.ToString());        
    }
    public string Description
    {
        get => Header.Description;
        set => Header.Description = value;
    }
    public ModSetting<T> CreateSetting<T>(string name, T defaultValue = default, AcceptableValueBase acceptableValues = null)
    => _mod.CreateSetting(Prefix + name, defaultValue, acceptableValues);

    // Privates
    protected TMod _mod;
    protected TValue _value;
    protected bool _isToggle;
    protected string Prefix
    => $"{GetType().Name}_{_value}_";
}
