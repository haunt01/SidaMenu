using HarmonyLib;

namespace SidaMenu
{
    public static class LightSourcePatch
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        public static class FullBrightPatch
        {
            public static void Postfix(ref float __result)
            {
                if (!States.fullBright) return;
                __result = 50f;
            }
        }
    }
}
