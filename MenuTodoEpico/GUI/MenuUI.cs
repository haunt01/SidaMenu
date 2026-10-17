using AmongUs.Data;
using AmongUs.Data.Player;
using Hazel;
using Rewired.UI.ControlMapper;
using Steamworks;
using System.Threading.Tasks;
using UnityEngine;

namespace SidaMenu
{
    public partial class MenuUI : MonoBehaviour
    {
        public static bool showMenu = true;
        public Rect windowRect = new Rect(20, 20, 667, 467);
        int selectedGroup = 0;
        public string[] groups = new string[] { "Self", "Visuals", "Host", "Misc", "About" };
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
            HostFeatures.MorePlayers(255);
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
                    States.noClip = GUI.Toggle(new Rect(0, 20, 467, 15), States.noClip, "No Clip");
                    States.tpToCursor = GUI.Toggle(new Rect(0, 40, 467, 15), States.tpToCursor, "Teleport to Cursor");
                    break;
                case 1: // Visuals

                    break;
                case 2: // Host
                    GUILayout.Label("Ship:");
                    if (GUI.Button(new Rect(0, 20, 467, 30), "Reset Tasks"))
                    {
                        if (ShipStatus.Instance != null && AmongUsClient.Instance.AmHost == true)
                        {
                            ShipStatus.Instance.Begin(); // this resets shipstatus and makes the players think the tasks has been resetted 
                        }
                    }
                    GUI.Label(new Rect(0, 60, 467, 30), "Lobby:");
                    States.morePlayers = GUI.Toggle(new Rect(0, 80, 467, 15), States.morePlayers, "Set Max Players to 255");
                    States.noEndGame = GUI.Toggle(new Rect(0, 100, 467, 15), States.noEndGame, "Force No End Game");
                    if (GUI.Button(new Rect(0, 140, 467, 30), "Force Start Game"))
                    {
                        if (AmongUsClient.Instance.AmHost)
                        {
                            AmongUsClient.Instance.StartGame(); 
                        }
                    }
                    if (GUI.Button(new Rect(0, 180, 467, 30), "Force End Game"))
                    {
                        if (AmongUsClient.Instance.AmHost)
                        {
                            GameManager.Instance.RpcEndGame(GameOverReason.CrewmateDisconnect, true);
                        }
                    }
                    break;
                case 3: // Misc
                    GUILayout.Label("Semi Exploit:");
                    if (GUI.Button(new Rect(0, 20, 467, 30), "Invisible"))
                    {
                        AmongUsClient.Instance.Despawn(PlayerControl.LocalPlayer.MyPhysics); // despawns player control and makes the user invisible for others
                    }
                    if (GUI.Button(new Rect(0, 60, 467, 30), "Quick Red Chat"))
                    {
                        MiscFeatures.RedChat();
                    }
                    GUI.Label(new Rect(0, 100, 467, 30), "Spoofing:");
                    if (GUI.Button(new Rect(0, 120, 467, 20), $"Spoof Platform To {SidaSpoofer.platforms[SidaSpoofer.selectedPlatform]}"))
                    {
                        SidaSpoofer.selectedPlatform = (SidaSpoofer.selectedPlatform + 1) % SidaSpoofer.platforms.Length;
                    }
                    break;
                case 4: // About
                    GUILayout.Label($"Welcome to SidaMenu!\nWe appreciate you using our ModMenu.\nModMenu's version: {version}\nDeveloper's Github: https://github.com/haunt01/");
                    if (GUI.Button(new Rect(0, 120, 467, 30), "Promote Cheat")) // Innerslop decided to remove the chat cooldown in RpcSendChat, so we can send 6 messages at ones. Thank you Innersloth.
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
