using AmongUs;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;
namespace SidaMenu
{
    public class SelfFeatures
    {

    }   

    public class VisualsFeatures
    {

    }

    public class MiscFeatures
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class OverflowPatch
        {

            public static void Postfix()
            {
                if (States.overflow == true) return;
                for (int i = 0; i < 1000; i++)
                {
                    var go = new UnityEngine.GameObject();
                    UnityEngine.Object.Destroy(go);
                }
            }
        }
    }
}
