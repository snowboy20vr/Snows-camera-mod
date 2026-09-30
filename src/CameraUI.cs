using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class CameraUI : MonoBehaviour
    {
        private bool visible = true;
        private Vector2 playerScroll;
        private Rect windowRect = new Rect(24, 24, 410, 660);
        private int selectedTab;
        private GUIStyle panel, header, label, small, button, selectedButton;
        private Texture2D panelTexture, accentTexture;

        public void Toggle() => visible = !visible;

        private void Start() => BuildStyles();

        private void OnGUI()
        {
            if (!visible) return;
            if (panel == null) BuildStyles();
            windowRect = GUI.Window(90421, windowRect, DrawWindow, "");
        }

        private void BuildStyles()
        {
            panelTexture = MakeTexture(new Color(0.035f, 0.04f, 0.055f, 0.97f));
            accentTexture = MakeTexture(new Color(1f, 0.48f, 0.06f, 1f));

            panel = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture, textColor = Color.white },
                padding = new RectOffset(18, 18, 16, 16)
            };
            header = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.82f, 0.84f, 0.9f) }
            };
            small = new GUIStyle(label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.58f, 0.61f, 0.7f) }
            };
            button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white, background = MakeTexture(new Color(0.10f, 0.11f, 0.15f, 1f)) },
                hover = { textColor = Color.white, background = MakeTexture(new Color(0.15f, 0.16f, 0.21f, 1f)) },
                active = { textColor = Color.white, background = accentTexture },
                padding = new RectOffset(10, 10, 8, 8)
            };
            selectedButton = new GUIStyle(button)
            {
                normal = { textColor = Color.black, background = accentTexture }
            };
        }

        private void DrawWindow(int id)
        {
            CameraController cam = Plugin.Instance.Camera;

            GUILayout.BeginVertical(panel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("SNOW'S CAMERA", header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("×", button, GUILayout.Width(34), GUILayout.Height(30))) visible = false;
            GUILayout.EndHorizontal();
            GUILayout.Label("PC CAMERA  •  CLEAN CASTING TOOL", small);
            GUILayout.Space(10);

            GUILayout.BeginVertical(panel);
            GUILayout.Label("●  CAMERA ACTIVE", label);
            GUILayout.Label("Mode: " + cam.Mode + "    Target: " + (cam.SelectedPlayer != null ? cam.SelectedPlayer.DisplayName : "None"), small);
            GUILayout.EndVertical();

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            DrawTab("CAMERA", 0);
            DrawTab("PLAYERS", 1);
            DrawTab("SETTINGS", 2);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (selectedTab == 0) DrawCameraTab();
            else if (selectedTab == 1) DrawPlayersTab();
            else DrawSettingsTab();

            GUILayout.Space(8);
            GUILayout.Label("F6 Menu  •  F7 Mode  •  [ / ] Player", small);
            GUI.DragWindow(new Rect(0, 0, 10000, 24));
            GUILayout.EndVertical();
        }

        private void DrawCameraTab()
        {
            CameraController cam = Plugin.Instance.Camera;
            GUILayout.Label("CAMERA MODE", label);

            GUILayout.BeginHorizontal();
            DrawModeButton(cam, CameraMode.FirstPerson, "1ST PERSON");
            DrawModeButton(cam, CameraMode.ThirdPerson, "3RD PERSON");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawModeButton(cam, CameraMode.Freecam, "FREECAM");
            DrawModeButton(cam, CameraMode.Spectator, "SPECTATE");
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            GUILayout.Label("Field of View  " + cam.FieldOfView.ToString("0") + "°", label);
            cam.FieldOfView = GUILayout.HorizontalSlider(cam.FieldOfView, 45f, 130f);

            GUILayout.Label("Smoothness  " + cam.Smoothness.ToString("0.00"), label);
            cam.Smoothness = GUILayout.HorizontalSlider(cam.Smoothness, 0.01f, 0.5f);

            GUILayout.Label("Near Clip  " + cam.NearClip.ToString("0.000"), label);
            cam.NearClip = GUILayout.HorizontalSlider(cam.NearClip, 0.005f, 0.5f);

            if (cam.Mode == CameraMode.ThirdPerson || cam.Mode == CameraMode.Spectator)
            {
                GUILayout.Label("Distance  " + cam.ThirdPersonDistance.ToString("0.00"), label);
                cam.ThirdPersonDistance = GUILayout.HorizontalSlider(cam.ThirdPersonDistance, 0.5f, 12f);
                cam.AutoOrbit = GUILayout.Toggle(cam.AutoOrbit, "  Auto orbit");
                cam.Collision = GUILayout.Toggle(cam.Collision, "  Camera collision");
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("←", button, GUILayout.Width(55))) cam.SelectRelative(-1);
            if (GUILayout.Button("RESET", button)) cam.ResetToLocal();
            if (GUILayout.Button("→", button, GUILayout.Width(55))) cam.SelectRelative(1);
            GUILayout.EndHorizontal();
        }

        private void DrawModeButton(CameraController cam, CameraMode mode, string text)
        {
            if (GUILayout.Button(text, cam.Mode == mode ? selectedButton : button, GUILayout.Height(34)))
                cam.SetMode(mode);
        }

        private void DrawPlayersTab()
        {
            var players = Plugin.Instance.Players.Players;
            GUILayout.Label("DETECTED PLAYERS  •  " + players.Count, label);

            playerScroll = GUILayout.BeginScrollView(playerScroll, GUILayout.Height(420));
            for (int i = 0; i < players.Count; i++)
            {
                TrackedPlayer player = players[i];
                bool selected = Plugin.Instance.Camera.SelectedPlayer == player;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(player.IsLocal ? "YOU  " + player.DisplayName : player.DisplayName,
                    selected ? selectedButton : button, GUILayout.Height(34)))
                    Plugin.Instance.Camera.Select(player);
                GUILayout.Label(player.IsLocal ? "LOCAL" : "PLAYER", small, GUILayout.Width(58));
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            GUILayout.EndScrollView();

            GUILayout.Space(8);
            GUILayout.Label("Use [ and ] to switch targets quickly.", small);
        }

        private void DrawSettingsTab()
        {
            CameraController cam = Plugin.Instance.Camera;
            GUILayout.Label("CASTING", label);
            GUILayout.Label("Independent desktop output. The recording camera does not replace the normal headset view.", small);

            GUILayout.Space(12);
            if (GUILayout.Button("Reset camera settings", button))
            {
                cam.FieldOfView = 80f;
                cam.Smoothness = 0.16f;
                cam.NearClip = 0.03f;
                cam.ThirdPersonDistance = 3.25f;
                cam.ThirdPersonHeight = 0.25f;
                cam.AutoOrbit = false;
                cam.Collision = true;
            }

            GUILayout.Space(14);
            GUILayout.Label("LOCAL CAMERA BADGE", label);
            Plugin.Instance.Badge.Enabled = GUILayout.Toggle(Plugin.Instance.Badge.Enabled, "  Show orange crown badge");
            Plugin.Instance.Badge.ShowCustomName = GUILayout.Toggle(Plugin.Instance.Badge.ShowCustomName, "  Show my custom name");
            if (Plugin.Instance.Badge.ShowCustomName)
            {
                GUILayout.Label("Custom name", small);
                Plugin.Instance.Badge.CustomName = GUILayout.TextField(Plugin.Instance.Badge.CustomName ?? "Snow", GUILayout.Height(26));
            }
            GUILayout.Label("Badge/name are local-only in this build, so other players do not receive a fake name tag or network change.", small);
            GUILayout.Space(8);
            GUILayout.Label("Snow's Camera Mod  •  v1.0.0", small);
        }

        private void DrawTab(string text, int tab)
        {
            if (GUILayout.Button(text, selectedTab == tab ? selectedButton : button, GUILayout.Height(30)))
                selectedTab = tab;
        }

        private Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}