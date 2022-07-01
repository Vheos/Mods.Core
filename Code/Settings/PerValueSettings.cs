namespace Vheos.Mods.Core;

public abstract class PerValueSettings<TMod, TValue> where TMod : AMod
{
    // Settings
    public ModSetting<bool> Header;
    public PerValueSettings(TMod mod, TValue value, bool isToggle = false)
    {
        _mod = mod;
        Value = value;
        _isToggle = isToggle;

        if (_isToggle)
            Header = _mod.CreateSetting(Prefix + nameof(Header), false);
    }
    public ModSetting<T> CreateSetting<T>(string name, T defaultValue = default, AcceptableValueBase acceptableValues = null)
        => _mod.CreateSetting(Prefix + name, defaultValue, acceptableValues);
    public void FormatHeader()
    {
        if (_isToggle)
            Header.Format(Value.ToString());
        else
            Header = _mod.CreateHeader(Value.ToString());        
    }
    public string Description
    {
        get => Header.Description;
        set => Header.Description = value;
    }

    // Publics
    public TValue Value { get; private set; }

    // Privates
    protected TMod _mod;
    protected bool _isToggle;
    protected string Prefix
    => $"{GetType().Name}_{Value}_";
}
