using AmongUs.Data;
using AmongUs.Data.Player;
using Hazel;
using Rewired.UI.ControlMapper;
using Steamworks;
using System.Threading.Tasks;
using UnityEngine;
using static SidaMenu.SelfFeatures;

namespace SidaMenu
{
    public partial class MenuUI : MonoBehaviour
    {
        public static bool showMenu = true;
        public Rect windowRect = new Rect(20, 20, 667, 467);
        int selectedGroup = 0;
        public string[] groups = new string[] { "Self","Game", "Host", "Misc", "About" };
        public string version = "v1.0.0-tuffBeta";
        void Start() 
        {
            if (showMenu)
            {
                selectedGroup = 4; // shows the about tab so the user sees the new content/updates
            }
        }
        void Update() 
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                showMenu = !showMenu;
            }
            DataManager.Player.Ban.BanPoints = 0; // removes ban points so the player doesn't get banned
            
            if (PlayerControl.LocalPlayer != null)
            {
                // noclip
                if (States.noClip)
                {
                    PlayerControl.LocalPlayer.Collider.enabled = !States.noClip; 
                }
                else
                {
                    PlayerControl.LocalPlayer.Collider.enabled = true;
                }
            }

            //calls
            SelfFeatures.Teleport();
            BepInEx.Logging.Logger.CreateLogSource("SidaMenu").LogInfo($"ShipStatus: {ShipStatus.Instance}");
            if (ShipStatus.Instance != null)
            {
                GameFeatures.Sabotages(ShipStatus.Instance);
            }

            // toggle calls
            States.unlockSkins = true;
        }

        void OnGUI() 
        {
            if (showMenu)
            {
                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, $"SidaMenu {version}");
            }
        }

        void WindowFunction(int windowID) 
        {
            GUILayout.BeginVertical(); // holy ai moment
            for (int i = 0; i < groups.Length; i++)
            {
                if (GUILayout.Button(groups[i], GUILayout.Width(167), GUILayout.Height(17))) // jajajajajajajaj 67 jajajaja sin semen ajkajakjakj
                {
                    selectedGroup = i;
                }
            }
            GUILayout.EndVertical(); // end of holy ai moment

            GUILayout.BeginArea(new Rect(180, 20, 515, 570)); // Shows where the content is gonna be (in this case is the right side of the menu)
            switch (selectedGroup) 
            {
                case 0: // Self
                    GUILayout.Label("Movement:");
                    GUI.Label(new Rect(0, 20, 467, 30), $"Speed: {States.speed}x");
                    States.speed = GUI.HorizontalSlider(new Rect(0, 40, 467, 30), States.speed, 1.0f, 10.0f);
                    States.noClip = GUI.Toggle(new Rect(0, 60, 467, 17), States.noClip, "No Clip");
                    States.tpToCursor = GUI.Toggle(new Rect(0, 80, 467, 17), States.tpToCursor, "Teleport to Cursor");
                    GUI.Label(new Rect(0, 100, 467, 30), "Visuals:");
                    States.fullBright = GUI.Toggle(new Rect(0, 120, 467, 17), States.fullBright, "Full Bright");
                    States.noShadows = GUI.Toggle(new Rect(0, 140, 467, 17), States.noShadows, "No Shadows");
                    GUI.Label(new Rect(0, 160, 467, 30), "Game:");
                    States.killImpostors = GUI.Toggle(new Rect(0, 180, 467, 17), States.killImpostors, "Kill Impostors");
                    break;
                case 1: // Game
                    GUILayout.Label("Ship:");
                    if (GUI.Button(new Rect(0, 20, 467, 30), "Call Meeting"))
                    {
                        GameFeatures.CallMeeting();
                    }
                    if (GUI.Button(new Rect(0, 60, 467, 30), "Kick Players of Vent"))
                    {
                        GameFeatures.KickVent();
                    }
                    GUI.Label(new Rect(0, 100, 467, 30), "Sabotages:");
                    States.sabotageElectricty = GUI.Toggle(new Rect(0, 120, 467, 20), States.sabotageElectricty, "Sabotage Electricity");
                    States.sabotageReactor = GUI.Toggle(new Rect(0, 140, 467, 20), States.sabotageReactor, "Sabotage Reactor");
                    States.sabotageOxygen = GUI.Toggle(new Rect(0, 160, 467, 20), States.sabotageOxygen, "Sabotage Oxygen");
                    States.sabotageComms = GUI.Toggle(new Rect(0, 180, 467, 20), States.sabotageComms, "Sabotage Comms");
                    States.sabotageLab = GUI.Toggle(new Rect(0, 200, 467, 20), States.sabotageLab, "Sabotage Lab");
                    States.sabotageHeli = GUI.Toggle(new Rect(0, 220, 467, 20), States.sabotageHeli, "Sabotage Heli");
                    States.sabotageMushroomMixup = GUI.Toggle(new Rect(0, 240, 467, 20), States.sabotageMushroomMixup, "Mushroom Mixup");
                    break;
                case 2: // Host
                    GUILayout.Label("Game:");
                    if (GUI.Button(new Rect(0, 20, 467, 30), "Reset Tasks"))
                    {
                        if (ShipStatus.Instance != null && AmongUsClient.Instance.AmHost == true)
                        {
                            ShipStatus.Instance.Begin(); // this resets shipstatus and makes the players think the tasks has been resetted 
                        }
                    }
                    GUI.Label(new Rect(0, 60, 467, 30), "Lobby:");
                    States.noEndGame = GUI.Toggle(new Rect(0, 80, 467, 17), States.noEndGame, "Force No End Game");
                    if (GUI.Button(new Rect(0, 100, 467, 30), "Force Start Game"))
                    {
                        if (AmongUsClient.Instance.AmHost)
                        {
                            AmongUsClient.Instance.StartGame();
                        }
                    }
                    if (GUI.Button(new Rect(0, 140, 467, 30), "Force End Game"))
                    {
                        if (AmongUsClient.Instance.AmHost)
                        {
                            GameManager.Instance.RpcEndGame(GameOverReason.CrewmateDisconnect, true);
                        }
                    }
                    break;
                case 3: // misc
                    GUILayout.Label("<color=red>HOLD ON! Some of these features can get you banned from the lobby.</color>");
                    if (GUI.Button(new Rect(0, 20, 467, 30), "Invisible"))
                    {
                        AmongUsClient.Instance.Despawn(PlayerControl.LocalPlayer.MyPhysics); // despawns player control and makes the user invisible for others
                    }
                    if (GUI.Button(new Rect(0, 60, 467, 30), "Send Red Messages")) // the user doesnt see it but others can do see it
                    {
                        MiscFeatures.RedChat();
                    }
                    if (GUI.Button(new Rect(0, 100, 467, 30), "Windows Sound Exploit (DOES NOT WORK ON VANILLA USERS)")) // phack you astral I did it
                    {
                        MiscFeatures.WindowsNotificationExploit();
                    }
                    if (GUI.Button(new Rect(0, 140, 467, 30), "VoteKick Host"))
                    {
                        DestroyableSingleton<VoteBanSystem>.Instance.CmdAddVote(AmongUsClient.Instance.HostId);
                    }
                    GUI.Label(new Rect(0, 180, 467, 30), "Spoofing:");
                    if (GUI.Button(new Rect(0, 200, 467, 20), $"Spoof Platform To {SidaSpoofer.platforms[SidaSpoofer.selectedPlatform]}"))
                    {
                        SidaSpoofer.selectedPlatform = (SidaSpoofer.selectedPlatform + 1) % SidaSpoofer.platforms.Length;
                    }
                    break;
                case 4: // about 
                    GUILayout.Label($"Welcome to SidaMenu!\nWe appreciate you using our ModMenu.\nModMenu's version: {version}\nDeveloper's Github: https://github.com/haunt01/");
                    if (GUI.Button(new Rect(0, 120, 467, 30), "Promote Cheat"))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            MiscFeatures.Promotion();
                        }
                    }
                    break;
                    
            }
            GUILayout.EndArea();
            GUI.DragWindow();
        }
    }
}
