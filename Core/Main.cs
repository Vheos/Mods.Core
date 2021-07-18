﻿/*
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using HarmonyLib;
using Vheos.ModdingCore;
using BepInEx;
using Vheos.Extensions.Math;
using Vheos.Extensions.General;
using Vheos.Extensions.Collections;



namespace Vheos.ModdingCore
{
    public class Main : BaseUnityPlugin
    {
        // Utility
        private List<Type> _awakeModTypes;
        private List<Type> _delayedModTypes;
        private List<IUpdatable> _updatableMods;
        private List<AMod> _mods;
        private bool _finishedDelayedInitialize;
        private void CategorizeModsByInstantiationTime(Type[] whitelist = null, Type[] blacklist = null)
        {
            foreach (var modType in Utility.GetDerivedTypes<AMod>())
                if (blacklist.IsNullOrEmpty() || modType.IsNotContainedIn(blacklist)
                && (whitelist.IsNullOrEmpty() || modType.IsContainedIn(whitelist)))
                    if (modType.IsAssignableTo<IDelayedInit>())
                        _delayedModTypes.Add(modType);
                    else
                        _awakeModTypes.Add(modType);
        }
        private void TryDelayedInitialize()
        {
            if (_finishedDelayedInitialize || !IsGameInitialized)
                return;

            InstantiateMods(_delayedModTypes);
            _finishedDelayedInitialize = true;
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
        private void UpdateMods(ICollection<IUpdatable> updatableMods)
        {
            foreach (var updatableMod in updatableMods)
                if (updatableMod.IsEnabled)
                    updatableMod.OnUpdate();
        }
        private bool IsGameInitialized
        => true;

        // Mono
#pragma warning disable IDE0051 // Remove unused private members
        private void Awake()
        {
            _awakeModTypes = new List<Type>();
            _delayedModTypes = new List<Type>();
            _updatableMods = new List<IUpdatable>();
            _mods = new List<AMod>();

            Tools.Initialize(this, Logger);

            Tools.Log("Categorizing mods by instantiation time...");
            CategorizeModsByInstantiationTime();

            Tools.Log("Awake:");
            foreach (var modType in _awakeModTypes)
                Tools.Log($"\t{modType.Name}");

            Tools.Log("Delayed:");
            foreach (var modType in _delayedModTypes)
                Tools.Log($"\t{modType.Name}");

            Tools.Log("Instantiating awake mods...");
            InstantiateMods(_awakeModTypes);

            Tools.Log($"Finished AwakeInit");
            Tools.Log("");

            Tools.Log($"Waiting for game initialization...");
        }
        private void Update()
        {
            TryDelayedInitialize();
            UpdateMods(_updatableMods);
            Tools.TryRedrawConfigWindow();
        }
    }
}
*/