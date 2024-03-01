using LethalCompanyInputUtils.Api;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace ReservedRadarBoosterSlot.Input
{
    internal class IngameKeybinds : LcInputActions
    {
        public static IngameKeybinds Instance = new();

        internal static InputActionAsset GetAsset()
        {
            return Instance.Asset;
        }

        [InputAction("<Keyboard>/r", Name = "[ReservedRadarBoosterSlot]\nDrop Radar Booster")]
        public InputAction DropRadarBoosterAction { get; set; }
    }
}
