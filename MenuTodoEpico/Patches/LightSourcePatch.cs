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

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))] // tank iu antigravity
        public static class RemoveShadowQuadPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (__instance.ShadowQuad != null && __instance.ShadowQuad.gameObject.activeSelf == States.noShadows)
                {
                    __instance.ShadowQuad.gameObject.SetActive(!States.noShadows);
                }
            }
        }
    }
}
