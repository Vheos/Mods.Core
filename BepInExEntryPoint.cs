namespace Vheos.Tools.ModdingCore
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Vheos.Tools.UtilityNS;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.Extensions.Collections;
    abstract public class BepInExEntryPoint : BepInEx.BaseUnityPlugin
    {
        // Privates
        private List<Type> _awakeModTypes;
        private List<Type> _delayedModTypes;
        private List<IUpdatable> _updatableMods;
        protected List<AMod> _mods;
        private bool _instantiatedDelayedMods;
        private void CategorizeModsByInstantiationTime()
        {
            foreach (var modType in Utility.GetDerivedTypes<AMod>(CurrentAssembly))
                if (Blacklist.IsNullOrEmpty() || modType.IsNotContainedIn(Blacklist)
                && (Whitelist.IsNullOrEmpty() || modType.IsContainedIn(Whitelist)))
                    if (modType.IsAssignableTo<IDelayedInit>())
                        _delayedModTypes.Add(modType);
                    else
                        _awakeModTypes.Add(modType);
        }
        private void InstantiateMods(ICollection<Type> modTypes)
        {
            foreach (var modType in modTypes)
            {
                AMod newMod = (AMod)Activator.CreateInstance(modType);
                _mods.Add(newMod);
                if (modType.IsAssignableTo<IUpdatable>())
                    _updatableMods.Add(newMod as IUpdatable);
            }
        }
        private void TryInstantiateDelayedMods()
        {
            if (_instantiatedDelayedMods || !DelayedInitializeCondition)
                return;

            Log.Debug($"Finished waiting");
            Log.Debug("");

            DelayedInitialize();

            Log.Debug("Instantiating delayed mods...");
            InstantiateMods(_delayedModTypes);

            Log.Debug($"Finished DelayedInit");
            _instantiatedDelayedMods = true;
        }
        private void UpdateMods(ICollection<IUpdatable> updatableMods)
        {
            foreach (var updatableMod in updatableMods)
                if (updatableMod.IsEnabled)
                    updatableMod.OnUpdate();
        }

        // User logic   
        abstract public Assembly CurrentAssembly
        { get; }
        virtual public void Initialize()
        { }
        virtual public void DelayedInitialize()
        { }
        virtual public bool DelayedInitializeCondition
        => true;
        virtual public Type[] Whitelist
        => null;
        virtual public Type[] Blacklist
        => null;

        // Mono
#pragma warning disable IDE0051 // Remove unused private members
        private void Awake()
        {
            _awakeModTypes = new List<Type>();
            _delayedModTypes = new List<Type>();
            _updatableMods = new List<IUpdatable>();
            _mods = new List<AMod>();

            Logger.LogDebug("Initializing Log...");
            Log.Initialize(Logger);

            Log.Debug("Initializing ConfigHelper...");
            ConfigHelper.Initialize(this);

            Log.Debug("Categorizing mods by instantiation time...");
            CategorizeModsByInstantiationTime();

            Log.Debug("Awake:");
            foreach (var modType in _awakeModTypes)
                Log.Debug($"\t{modType.Name}");

            Log.Debug("Delayed:");
            foreach (var modType in _delayedModTypes)
                Log.Debug($"\t{modType.Name}");

            Initialize();

            Log.Debug("Instantiating awake mods...");
            InstantiateMods(_awakeModTypes);

            Log.Debug($"Finished AwakeInit");
            Log.Debug("");

            Log.Debug($"Waiting for game initialization...");
        }
        private void Update()
        {
            TryInstantiateDelayedMods();
            UpdateMods(_updatableMods);
            ConfigHelper.TryRedrawConfigWindow();
        }
    }
}