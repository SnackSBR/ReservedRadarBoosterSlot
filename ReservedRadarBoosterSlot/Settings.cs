using BepInEx.Configuration;
using System.Collections.Generic;

namespace ReservedRadarBoosterSlot
{
    public static class Settings
    {
        public static Dictionary<string, ConfigEntryBase> ConfigEntries = [];
        public static ConfigEntry<string> DeployRadarBoosterKey;
        public static ConfigEntry<bool> DeployRadarBooster;
        public static ConfigEntry<int> RadarPrice;
        public static void Init()
        {
            DeployRadarBoosterKey = Plugin.Instance.Config.Bind("ReservedRadarBoosterSlot", "DropRadarBoosterKey", "<Keyboard>/r", "Keybind to drop the Radar Booster on the ground. (will be ignored if InputUtils is present)");
            DeployRadarBooster = Plugin.Instance.Config.Bind("ReservedRadarBoosterSlot", "DeployRadarBooster", true, "Enable the keybind to activate and drop the Radar Booster.");
            RadarPrice = Plugin.Instance.Config.Bind("ReservedPeeperSlot", "RadarPrice", 0, "Price for the Radar Slot. If set to 0 the slot will unlock automatically");

            ConfigEntries.Add(DeployRadarBoosterKey.Definition.Key, DeployRadarBoosterKey);
            ConfigEntries.Add(DeployRadarBooster.Definition.Key, DeployRadarBooster);
        }
    }
}
