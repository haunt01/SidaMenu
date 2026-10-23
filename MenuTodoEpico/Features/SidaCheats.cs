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
        public static void Teleport() 
        {
            if (!States.tpToCursor) return;
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(worldPos);
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.TrueSpeed), MethodType.Getter)]
        public static class SpeedPatch
        {
            public static void Postfix(PlayerPhysics __instance, ref float __result)
            {
                if (__instance.AmOwner && States.speed > 0f)
                {
                    __result *= States.speed;
                }
            }
        }

        [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
        public static class KillImpostorsPatch
        {
            public static void Postfix(NetworkedPlayerInfo target, ref bool __result)
            {
                if (!States.killImpostors) return; 
                if (target != null && !target.Disconnected && !target.IsDead &&
                    target.PlayerId != PlayerControl.LocalPlayer.PlayerId &&
                    target.Object != null && !target.Object.inVent && target.Object.Visible)
                {
                    __result = true;
                }
            }
        }

    }

    public class HostFeatures
    {
        public static void MorePlayers(int maxPlayers)
        {
            if (!States.morePlayers) return;
            GameManager.Instance.LogicOptions.currentGameOptions.SetInt(Int32OptionNames.MaxPlayers, maxPlayers);
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
        public static class NoEndGamePatch
        {
            public static bool Prefix(GameManager __instance)
            {
                return !States.noEndGame;
            }
        }
    }

    public class MiscFeatures
    {
        // sanction messages 
        private static readonly ushort[] sanctionMessages = new ushort[]
        {
        (ushort)StringNames.SanctionCheatingHacking,
        (ushort)StringNames.SanctionSpamming,
        (ushort)StringNames.SanctionBullying,
        (ushort)StringNames.SanctionHarassment,
        (ushort)StringNames.SanctionUnsportsmanlikeConduct,
        (ushort)StringNames.SanctionInappropriateChatDating,
        (ushort)StringNames.SanctionSelfHarmPromotion,
        (ushort)StringNames.SanctionDoxing,
        (ushort)StringNames.SanctionSexualMisconduct,
        (ushort)StringNames.SanctionIllegalActivity,
        (ushort)StringNames.SanctionImpersonationCeleb,
        (ushort)StringNames.SanctionImpersonationDevelopers,
        (ushort)StringNames.SanctionInappropriateNameDerogatory,
        (ushort)StringNames.SanctionInappropriateNameNsfw,
        (ushort)StringNames.SanctionInappropriateNameUnsportsmanlike,
        (ushort)StringNames.SanctionInappropriateChatPersonalInfo,
        (ushort)StringNames.SanctionRepeatOffender3,
        (ushort)StringNames.SanctionRepeatOffender10,
        (ushort)StringNames.SanctionWeaponizingRules,
        (ushort)StringNames.SanctionUnknown,
        (ushort)StringNames.SanctionDuration,
        (ushort)StringNames.SanctionPermanent,
        };
        public static void RedChat()
        {
            var client = AmongUsClient.Instance;
            if (client == null || PlayerControl.LocalPlayer == null) return;
            ushort randomSanction = sanctionMessages[UnityEngine.Random.Range(0, sanctionMessages.Length)];
            var mw = MessageWriter.Get(SendOption.Reliable);
            mw.StartMessage(5);
            mw.Write(client.GameId);
            mw.StartMessage(2);
            mw.WritePacked(PlayerControl.LocalPlayer.NetId);
            mw.Write((byte)RpcCalls.SendQuickChat);
            mw.Write((byte)3);
            mw.Write((ushort)StringNames.NumImpostorsP);
            mw.Write((byte)1);
            mw.Write((byte)2);
            mw.Write(randomSanction);
            mw.EndMessage();
            mw.EndMessage();
            client.SendOrDisconnect(mw);
            mw.Recycle();
        }

        // This feature only works when someone is using HydraMenu or any kind of ModMenu/tool that uses a ChatLogger
        public static void WindowsNotificationExploit()
        {
            if (AmongUsClient.Instance == null) return;
            if (PlayerControl.LocalPlayer == null) return;

            try
            {
                PlayerControl.LocalPlayer.RpcSendChat("\u0007\u0007\u0007");
            }
            catch { }
        }
        public static void Promotion() // Innerslop decided to remove the chat cooldown in RpcSendChat, so we can send 6 messages at ones. Thank you Innersloth.
        {
            PlayerControl.LocalPlayer.RpcSendChat("SidaMenu is the best Mod Menu that you can get RN!\n GO get it at:\n ｈｔｔｐｓ：／／ｇｉｔｈｕｂ．ｃｏｍ／ｈａｕｎｔ０１／ＳｉｄａＭｅｎｕ");
        }
    }
}
