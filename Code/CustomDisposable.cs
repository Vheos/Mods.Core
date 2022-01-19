namespace Vheos.Mods.Core
{
    using System;

    public class CustomDisposable : IDisposable
    {
        // Privates
        private readonly System.Action _onDispose;

        // Initializers
        public CustomDisposable(System.Action onDispose)
        => _onDispose = onDispose;

        // Finalizers
        public void Dispose()
        => _onDispose();
    }
}