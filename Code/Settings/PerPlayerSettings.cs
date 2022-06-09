namespace Vheos.Mods.Core;

public abstract class PerPlayerSettings<T> where T : AMod
{
    // Settings
    public readonly ModSetting<bool> Toggle;
    public PerPlayerSettings(T mod, int playerID)
    {
        _mod = mod;
        _playerID = playerID;
        Toggle = mod.CreateSetting(PlayerPrefix + nameof(Toggle), false);
    }
    public virtual void Format()
    {
        Toggle.DisplayResetButton = false;
        Toggle.Format($"Player {_playerID + 1}");
    }
    public string Description
    {
        get => Toggle.Description;
        set => Toggle.Description = value;
    }

    // Privates
    protected T _mod;
    protected int _playerID;
    protected string PlayerPrefix
    => $"{GetType().Name}_Player{_playerID + 1}_";
}
