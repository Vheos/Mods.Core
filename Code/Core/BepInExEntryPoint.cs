namespace Vheos.Mods.Core;
using System.Reflection;
using Vheos.Helpers.KeyCodeCache;

abstract public class BepInExEntryPoint : BepInEx.BaseUnityPlugin
{
    // User logic   
    abstract protected Assembly CurrentAssembly
    { get; }
    virtual protected void Initialize()
    { }
    virtual protected void DelayedInitialize()
    { }
    virtual protected bool DelayedInitializeCondition
    => true;
    virtual protected Type[] Whitelist
    => null;
    virtual protected Type[] Blacklist
    => null;
    virtual protected Type[] ModsOrderingList
    => null;
    virtual protected string[] PresetNames
    => null;

    // Privates
    private List<Type> _awakeModTypes;
    private List<Type> _delayedModTypes;
    private List<IUpdatable> _updatableMods;
    protected HashSet<AMod> _mods;
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

        DelayedInitialize();

        Log.Debug("Instantiating delayed mods...");
        InstantiateMods(_delayedModTypes);

        Log.Debug($"Initializing {nameof(Presets)}...");
        Presets.TryInitialize(PresetNames, _mods);

        Log.Debug($"Finished DelayedInit");
        _instantiatedDelayedMods = true;

    }
    private void UpdateMods(ICollection<IUpdatable> updatableMods)
    {
        foreach (var updatableMod in updatableMods)
            if (updatableMod.IsEnabled)
                updatableMod.OnUpdate();
    }

    // Play
    private void Awake()
    {
        _awakeModTypes = new List<Type>();
        _delayedModTypes = new List<Type>();
        _updatableMods = new List<IUpdatable>();
        _mods = new HashSet<AMod>();
        AMod.SetOrderingList(ModsOrderingList);

        Logger.LogDebug($"Initializing {nameof(Log)}...");
        Log.Initialize(Logger);

        Log.Debug($"Initializing {nameof(ConfigHelper)}...");
        ConfigHelper.Initialize(this);

        Log.Debug($"Initializing {nameof(KeyCodeCache)}...");
        KeyCodeCache.Initialize();

        Initialize();

        Log.Debug("Categorizing mods by instantiation time...");
        CategorizeModsByInstantiationTime();

        Log.Debug("Awake:");
        foreach (var modType in _awakeModTypes)
            Log.Debug($"\t{modType.Name}");

        Log.Debug("Delayed:");
        foreach (var modType in _delayedModTypes)
            Log.Debug($"\t{modType.Name}");

        Log.Debug("Instantiating awake mods...");
        InstantiateMods(_awakeModTypes);

        Log.Debug($"Finished AwakeInit");
        Log.Debug($"Waiting for game initialization...");
    }
    private void Update()
    {
        TryInstantiateDelayedMods();
        UpdateMods(_updatableMods);
        ConfigHelper.TryRedrawConfigWindow();
    }
}
