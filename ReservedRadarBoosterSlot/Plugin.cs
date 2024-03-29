﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ReservedItemSlotCore;
using ReservedItemSlotCore.Config;
using ReservedItemSlotCore.Data;
using ReservedRadarBoosterSlot.Compat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReservedRadarBoosterSlot
{
    [BepInPlugin(Metadata.GUID, Metadata.NAME, Metadata.VERSION)]
    [BepInDependency("FlipMods.ReservedItemSlotCore", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        readonly Harmony harmony = new(Metadata.GUID);
        internal static readonly ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(Metadata.NAME);
        public static ReservedItemSlotData RadarSlot;
        public static ReservedItemData RadarItemData;

        private void Awake()
        {
            Instance = this;

            Settings.Init();

            RadarItemData = new("Radar-booster");
            RadarSlot = ReservedItemSlotData.CreateReservedItemSlotData("RadarSlot", 667, 100);
            RadarSlot.purchasePrice = Settings.RadarPrice.Value;
            RadarSlot.AddItemToReservedItemSlot(RadarItemData);

            IEnumerable<Type> types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            if (Settings.DeployRadarBooster.Value && InputUtils_Compat.Enabled)
            {
                InputUtils_Compat.Init();
            }

            harmony.PatchAll();
        }
    }
}