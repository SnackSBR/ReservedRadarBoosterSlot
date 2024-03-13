using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using ReservedItemSlotCore.Patches;
using ReservedItemSlotCore;
using ReservedRadarBoosterSlot.Compat;
using System;
using ReservedItemSlotCore.Data;
using System.Collections.Generic;

namespace ReservedRadarBoosterSlot.Input
{
    [HarmonyPatch]
    internal static class Keybinds
    {
        public static InputActionAsset Asset;
        public static InputActionMap ActionMap;
        private static InputAction DropRadarBoosterAction;

        public static PlayerControllerB LocalPlayerController => StartOfRound.Instance?.localPlayerController;

        [HarmonyPatch(typeof(PreInitSceneScript), "Awake")]
        [HarmonyPrefix]
        public static void AddToKeybindMenu()
        {
            if (Settings.DeployRadarBooster.Value)
            {
                if (InputUtils_Compat.Enabled)
                {
                    Asset = InputUtils_Compat.Asset;
                    ActionMap = Asset.actionMaps[0];
                    DropRadarBoosterAction = IngameKeybinds.Instance.DropRadarBoosterAction;
                }
                else
                {
                    Asset = ScriptableObject.CreateInstance<InputActionAsset>();
                    ActionMap = new InputActionMap("ReservedRadarBoosterSlot");
                    InputActionSetupExtensions.AddActionMap(Asset, ActionMap);
                    DropRadarBoosterAction = InputActionSetupExtensions.AddAction(ActionMap, "ReservedRadarBoosterSlot.DropRadarBoosterAction", binding: Settings.DeployRadarBoosterKey.Value, interactions: "Press");
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnEnable")]
        [HarmonyPostfix]
        public static void OnEnable()
        {
            if (Settings.DeployRadarBooster.Value)
            {
                Asset.Enable();
                DropRadarBoosterAction.performed += OnDropRadarBoosterPerformed;
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            if (Settings.DeployRadarBooster.Value)
            {
                Asset.Disable();
                DropRadarBoosterAction.performed -= OnDropRadarBoosterPerformed;
            }
        }

        private static void OnDropRadarBoosterPerformed(InputAction.CallbackContext context)
        {
            if (LocalPlayerController == null || !LocalPlayerController.isPlayerControlled || (LocalPlayerController.IsServer && !LocalPlayerController.isHostPlayerObject))
            {
                return;
            }

            try
            {
                if (SessionManager.unlockedReservedItemSlotsDict.TryGetValue(Plugin.RadarSlot.slotName, out var RadarSlot))
                {
                    List<ReservedItemSlotData> focusReservedItemSlots = [RadarSlot];
                    if (focusReservedItemSlots.Count > 0)
                    {
                        ReservedHotbarManager.ForceToggleReservedHotbar(focusReservedItemSlots.ToArray());
                        LocalPlayerController.StartCoroutine(ShuffleItems());
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.mls.LogError(ex.Message);
                Plugin.mls.LogError(ex.StackTrace);
                HUDManager.Instance.chatText.text += $"Error when dropping Radar Booster";
            }
        }

        private static IEnumerator ShuffleItems()
        {
            yield return new WaitForSeconds(0.2f);
            if (LocalPlayerController.currentlyHeldObjectServer != null)
            {
                ((RadarBoosterItem)LocalPlayerController.currentlyHeldObjectServer).EnableRadarBooster(true);
                yield return new WaitForSeconds(0.1f);
                LocalPlayerController.DiscardHeldObject();
            }
            yield return new WaitForSeconds(0.1f);
            ReservedHotbarManager.FocusReservedHotbarSlots(false);
        }
    }
}
