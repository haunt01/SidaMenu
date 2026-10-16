using HarmonyLib;

namespace SidaMenu
{
    public static class SidaSpoofer
    {
        public static int selectedPlatform = 0;
        public static Platforms[] platforms = new Platforms[]
        {
            Platforms.StandaloneSteamPC,
            Platforms.StandaloneEpicPC,
            Platforms.Playstation,
            Platforms.Xbox,
            Platforms.Switch
        };

        [HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
        public static class SpoofPlatformPatch
        {
            public static void Prefix(PlatformSpecificData __instance)
            {
                if (!States.spoofPlatform) return;
                __instance.Platform = platforms[selectedPlatform];
            }
        }
    }
}
    