using HarmonyLib;

namespace SidaMenu
{
    public static class SidaSpoofer
    {
        public static int selectedPlatform = 5;
        public static Platforms[] platforms = new Platforms[]
        {
        Platforms.StandaloneSteamPC,
        Platforms.StandaloneEpicPC,
        Platforms.Playstation,
        Platforms.Xbox,
        Platforms.Switch,
        Platforms.Android,
        Platforms.IPhone
        };

        [HarmonyPatch(typeof(Constants), nameof(Constants.GetPlatformType))]
        public static class SpoofPlatformTypePatch
        {
            public static bool Prefix(ref Platforms __result)
            {
                if (!States.spoofPlatform) return true;

                __result = platforms[selectedPlatform];
                return false;
            }
        }
        [HarmonyPatch(typeof(Constants), nameof(Constants.GetPlatformData))]
        public static class SpoofPlatformDataPatch
        {
            public static void Postfix(ref PlatformSpecificData __result)
            {
                if (!States.spoofPlatform) return;

                if (__result != null)
                {
                    __result.Platform = platforms[selectedPlatform];
                }
            }
        }
    }

}
