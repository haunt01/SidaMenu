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

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsKillTimerEnabled), MethodType.Getter)]
        public static class CooldownInVentsPatch
        {
            public static void Postfix(PlayerControl __instance, ref bool __result)
            {
                if (__result) return;
                if (States.cooldownInVents && __instance.inVent && __instance.Data != null && !__instance.Data.IsDead)
                {
                    if (MeetingHud.Instance == null
                        && ExileController.Instance == null
                        && IntroCutscene.Instance == null
                        && PlayerCustomizationMenu.Instance == null)
                    {
                        if (!DestroyableSingleton<HudManager>.InstanceExists ||
                            (!DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !DestroyableSingleton<HudManager>.Instance.IsIntroDisplayed))
                        {
                            __result = true;
                        }
                    }
                }
            }
        }


    }

    public class GameFeatures
    {
        public static void CallMeeting() // calls a meeting
        {
            PlayerControl.LocalPlayer.CmdReportDeadBody(null);
        }

        public static void KickVent()
        {
            foreach (var vent in ShipStatus.Instance.AllVents)
            {
                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);
            }
        }

        private static bool lastElecState = false;
        private static bool lastReactorState = false;
        private static bool lastOxygenState = false;
        private static bool lastCommsState = false;
        private static bool lastMushroomState = false;
        public static void Sabotages(ShipStatus shipStatus)
        {
            if (shipStatus == null || shipStatus.Systems == null) return;
            if (States.sabotageElectricty != lastElecState)
            {
                if (shipStatus.Systems.ContainsKey(SystemTypes.Electrical))
                {
                    var elecSys = shipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (States.sabotageElectricty)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)(0x1F | 128));
                    }
                    else
                    {
                        byte diff = (byte)(elecSys.ActualSwitches ^ elecSys.ExpectedSwitches);
                        if (diff != 0)
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)(diff | 128));
                        }
                    }
                }
                lastElecState = States.sabotageElectricty;
            }
            if (States.sabotageReactor != lastReactorState)
            {
                if (shipStatus.Systems.ContainsKey(SystemTypes.HeliSabotage))
                {
                    var heliSys = shipStatus.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                    if (States.sabotageReactor)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)58);
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);
                    }
                    else
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, (byte)(16 | 0));
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, (byte)(16 | 1));
                        heliSys.ClearSabotage();
                    }
                }
                else if (shipStatus.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    if (States.sabotageReactor)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)21);
                        shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, (byte)128);
                    }
                    else
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, (byte)16);
                    }
                }
                else if (shipStatus.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    if (States.sabotageReactor)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)3);
                        shipStatus.RpcUpdateSystem(SystemTypes.Reactor, (byte)128);
                    }
                    else
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Reactor, (byte)16);
                    }
                }
                lastReactorState = States.sabotageReactor;
            }
            if (States.sabotageOxygen != lastOxygenState)
            {
                if (shipStatus.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, States.sabotageOxygen ? (byte)128 : (byte)16);
                }
                else if (States.sabotageOxygen)
                {
                    HudManager.Instance?.Notifier?.AddDisconnectMessage("Oxygen not present on this map");
                    States.sabotageOxygen = false;
                }
                lastOxygenState = States.sabotageOxygen;
            }
            if (States.sabotageComms != lastCommsState)
            {
                if (shipStatus.Systems.ContainsKey(SystemTypes.Comms))
                {
                    byte mapId = (byte)GameOptionsManager.Instance.CurrentGameOptions.MapId;
                    bool isHqComms = (mapId == 1 || mapId == 5);
                    if (isHqComms)
                    {
                        if (States.sabotageComms)
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128);
                        }
                        else
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Comms, (byte)(16 | 0));
                            shipStatus.RpcUpdateSystem(SystemTypes.Comms, (byte)(16 | 1));
                        }
                    }
                    else
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.Comms, States.sabotageComms ? (byte)128 : (byte)0);
                    }
                }
                lastCommsState = States.sabotageComms;
            }
            if (States.sabotageMushroomMixup != lastMushroomState)
            {
                if (shipStatus.Systems.ContainsKey(SystemTypes.MushroomMixupSabotage))
                {
                    if (States.sabotageMushroomMixup)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1);
                    }
                    else
                    {
                        var mushSys = shipStatus.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                        mushSys.Deteriorate(9999f);
                    }
                }
                else if (States.sabotageMushroomMixup)
                {
                    HudManager.Instance?.Notifier?.AddDisconnectMessage("Fungle not present on this map");
                    States.sabotageMushroomMixup = false;
                }
                lastMushroomState = States.sabotageMushroomMixup;
            }
        }
    }

    public class HostFeatures
    {
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
        public static class NoEndGamePatch
        {
            public static bool Prefix(GameManager __instance)
            {
                return !States.noEndGame;
            }
        }

        private static bool wasUnlocked = false;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdatePatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer?.Data?.Role == null) return;
                var role = PlayerControl.LocalPlayer.Data.Role;
                if (!States.unlockKillButton)
                {
                    if (wasUnlocked)
                    {
                        wasUnlocked = false;
                        if (!role.IsImpostor)
                        {
                            role.CanUseKillButton = false;
                            if (__instance.KillButton != null)
                            {
                                __instance.KillButton.SetTarget(null);
                                __instance.KillButton.Hide();
                            }
                        }
                    }
                    return;
                }
                if (!wasUnlocked)
                {
                    wasUnlocked = true;
                    role.CanUseKillButton = true;
                    PlayerControl.LocalPlayer.SetKillTimer(0f); 
                }
                role.CanUseKillButton = true;
                // Asegura que esté visible mientras esté activo
                if (__instance.KillButton != null && !__instance.KillButton.gameObject.activeSelf)
                {
                    __instance.KillButton.Show();
                }
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
        public static class KillButtonSetTargetPatch
        {
            public static void Prefix(KillButton __instance, ref PlayerControl target)
            {
                if (!States.unlockKillButton) return;
                if (target == null && PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    target = GetClosestLivingPlayer(PlayerControl.LocalPlayer);
                }
            }
            private static PlayerControl GetClosestLivingPlayer(PlayerControl localPlayer)
            {
                if (GameManager.Instance?.LogicOptions == null) return null;
                float maxDistance = GameManager.Instance.LogicOptions.GetKillDistance();
                Vector2 myPos = localPlayer.GetTruePosition();
                PlayerControl closest = null;
                float closestDist = float.MaxValue;
                foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
                {
                    if (pc == null || pc == localPlayer) continue;
                    if (pc.Data == null || pc.Data.IsDead || pc.Data.Disconnected) continue;
                    if (pc.inVent || !pc.Visible || pc.Collider == null || !((Behaviour)pc.Collider).enabled) continue;
                    Vector2 diff = pc.GetTruePosition() - myPos;
                    float dist = diff.magnitude;
                    if (dist <= maxDistance && dist < closestDist)
                    {
                        if (!PhysicsHelpers.AnyNonTriggersBetween(myPos, diff.normalized, dist, Constants.ShipAndObjectsMask))
                        {
                            closestDist = dist;
                            closest = pc;
                        }
                    }
                }
                return closest;
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public static class KillButtonDoClickPatch
        {
            public static bool Prefix(KillButton __instance)
            {
                if (!States.unlockKillButton) return true;
                if (__instance.isActiveAndEnabled && __instance.Target != null && !__instance.IsOnCooldown
                    && PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove)
                {
                    PlayerControl target = __instance.Target;
                    PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
                    float killCooldown = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
                    PlayerControl.LocalPlayer.SetKillTimer(killCooldown);
                    __instance.SetTarget(null);
                    return false;
                }
                return true;
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
