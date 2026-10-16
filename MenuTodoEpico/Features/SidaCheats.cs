using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace SidaMenu
{
    public class SelfFeatures
    {
        public static void noClip() 
        {

        }
        public static void Teleport() 
        {
            if (!States.tpToCursor) return;
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldPos);
            }
        }
    }   

    public class VisualsFeatures
    {

    }

    public class HostFeatures
    {
        public static void MorePlayers(int maxPlayers)
        {
            if (!States.morePlayers) return;
            GameManager.Instance.LogicOptions.currentGameOptions.SetInt(Int32OptionNames.MaxPlayers, maxPlayers);
        }
    }

    public class MiscFeatures
    {

    }
}
