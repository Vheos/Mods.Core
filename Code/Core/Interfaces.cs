namespace Vheos.Mods.Core;

public interface IDelayedInit
{ }

public interface IUpdatable
{
    void OnUpdate();
    bool IsEnabled { get; }
}
