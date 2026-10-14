using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.Analytics;
namespace SidaMenu
{
    [BepInAutoPlugin]
    [BepInProcess("Among Us.exe")]
    public partial class SidaMenu : BasePlugin
    {
        public static bool noTelemetry = true; 
        public override void Load()
        {
            AddComponent<MenuUI>();
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("com.sida.menu");
            harmony.PatchAll();
            /* Disables Telemetry (haven't fully tested if it works, but according to Unity docs it should) 
            found here: https://github.com/scp222thj/MalumMenu/blob/main/src/MalumMenu.cs at line 205*/
            if (noTelemetry)
            {
                Analytics.enabled = false;
                Analytics.deviceStatsEnabled = false;
                PerformanceReporting.enabled = false;
            }
        }

        public void PanicMode() // me da paja despues se hace jioajkjakajkjakja que vago q soy
        { 
            
        }
    }
}
