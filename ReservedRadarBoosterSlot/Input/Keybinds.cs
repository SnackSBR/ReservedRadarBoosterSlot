using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;
using UnityEngine;
using MoreShipUpgrades.Compat;
using System.Collections;
using ReservedItemSlotCore.Patches;
using ReservedItemSlotCore;

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
                DropRadarBoosterAction = InputActionSetupExtensions.AddAction(ActionMap, "ReservedRadarBoosterSlot.DropRadarBoosterAction", binding: Settings.DropRadarBoosterKey.Value, interactions: "Press");
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnEnable")]
        [HarmonyPostfix]
        public static void OnEnable()
        {
            Asset.Enable();
            DropRadarBoosterAction.performed += OnDropRadarBoosterPerformed;
        }

        [HarmonyPatch(typeof(StartOfRound), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            Asset.Disable();
            DropRadarBoosterAction.performed -= OnDropRadarBoosterPerformed;
        }

        private static void OnDropRadarBoosterPerformed(InputAction.CallbackContext context)
        {
            if(LocalPlayerController == null || !LocalPlayerController.isPlayerControlled || (LocalPlayerController.IsServer && !LocalPlayerController.isHostPlayerObject))
            {
                return;
            }

            ReservedItemInfo radar = Plugin.RadarBoosterInfo;

            ReservedItemSlotCore.Input.Keybinds.holdingModifierKey = true;

            if (!ReservedItemPatcher.IsItemSlotEmpty(radar, LocalPlayerController) && ReservedItemPatcher.CanSwapToReservedHotbarSlot())
            {
                LocalPlayerController.StartCoroutine(ShuffleItems(radar));
            }
        }

        private static IEnumerator ShuffleItems(ReservedItemInfo radar)
        {
            ReservedPlayerData reservedPlayerData = PlayerPatcher.playerDataLocal;
            ReservedItemPatcher.FocusReservedHotbarSlots(true);
            yield return new WaitForSeconds(0.1f);
            ReservedItemPatcher.SwitchToItemSlot(LocalPlayerController, reservedPlayerData.reservedHotbarStartIndex + radar.reservedItemIndex, null);
            yield return new WaitForSeconds(0.1f);
            ((RadarBoosterItem)LocalPlayerController.currentlyHeldObjectServer).EnableRadarBooster(true);
            yield return new WaitForSeconds(0.1f);
            LocalPlayerController.DiscardHeldObject();
            yield return new WaitForSeconds(0.1f);
            ReservedItemSlotCore.Input.Keybinds.holdingModifierKey = false;
            ReservedItemPatcher.FocusReservedHotbarSlots(false);
        }
    }
}
