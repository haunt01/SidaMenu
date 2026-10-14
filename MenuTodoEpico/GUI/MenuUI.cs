using AmongUs.Data;
using AmongUs.Data.Player;
using Hazel;
using Steamworks;
using UnityEngine;

namespace MenuTodoEpico
{
    public partial class MenuUI : MonoBehaviour
    {
        public static bool showMenu = true;
        public Rect windowRect = new Rect(20, 20, 667, 467);
        int selectedGroup = 0;
        public string[] groups = new string[] { "Self", "Visuals", "Host", "Misc", "Settings", "About" };
        public string version = "v1.0.0-tuffBeta";
        void Start() {
            if (showMenu)
            {
                selectedGroup = 5; // shows the about tab so the user sees the new content/updates
            }
        }
        void Update() {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                showMenu = !showMenu;
            }
            
        }

        void OnGUI() {
            if (showMenu)
            {
                windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, $"SidaMenu {version}");
            }
        }

        void WindowFunction(int windowID) {
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
            switch (selectedGroup) {
                case 0: // Self
                    States.noClip = GUILayout.Toggle(States.noClip, "NoClip");
                    States.speedHack = GUILayout.Toggle(States.speedHack, "SpeedHack");
                    break;
                case 1: // Visuals

                    break;
                case 2: // Host

                    break;
                case 3: // Misc

                    break;
                case 4: // Settings

                    break;
                case 5: // About
                    GUILayout.Label($"Welcome to SidaMenu!\nWe appreciate you using our ModMenu.\nModMenu's version: {version}\nDeveloper's Github: https://github.com/haunt01/");
                    break;
            }
            GUILayout.EndArea();

            GUI.DragWindow();
        }
    }
}
