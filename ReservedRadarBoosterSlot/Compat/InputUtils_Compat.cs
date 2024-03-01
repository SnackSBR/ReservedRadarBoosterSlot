using ReservedRadarBoosterSlot.Input;
using UnityEngine.InputSystem;

namespace ReservedRadarBoosterSlot.Compat
{
    public static class InputUtils_Compat
    {
        internal static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.LethalCompanyInputUtils");
        internal static InputActionAsset Asset => IngameKeybinds.GetAsset();
        public static InputAction DropRadarBoosterAction => IngameKeybinds.Instance.DropRadarBoosterAction;

        internal static void Init()
        {
            if (Enabled && IngameKeybinds.Instance == null)
            {
                IngameKeybinds.Instance = new();
            }
        }
    }
}
