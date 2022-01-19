namespace Vheos.Tools.ModdingCore
{
    public interface IDelayedInit
    { }

    public interface IUpdatable
    {
        void OnUpdate();
        bool IsEnabled { get; }
    }
}