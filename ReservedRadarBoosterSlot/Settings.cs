using BepInEx.Configuration;
using System.Collections.Generic;

namespace ReservedRadarBoosterSlot
{
    public static class Settings
    {
        public static Dictionary<string, ConfigEntryBase> ConfigEntries = [];
        public static ConfigEntry<string> DropRadarBoosterKey;
        public static void Init()
        {
            DropRadarBoosterKey = Plugin.Instance.Config.Bind("ReservedRadarBoosterSlot", "DropRadarBoosterKey", "<Keyboard>/r", "Keybind to drop the RadarBooster on the ground. (will be ignored if InputUtils is present)");

            ConfigEntries.Add(DropRadarBoosterKey.Definition.Key, DropRadarBoosterKey);
        }
    }
}
