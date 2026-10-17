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

        [HarmonyPatch(typeof(Constants), nameof(Constants.GetPlatformType))]
        public static class SpoofPlatformPatch
        {
            public static bool Prefix(ref Platforms __result)
            {
                if (!States.spoofPlatform) return true;

                __result = platforms[selectedPlatform];
                return false; 
            }
        }
    }
}
    