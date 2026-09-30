using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class CameraUI : MonoBehaviour
    {
        private bool visible = false;
        private Vector2 playerScroll;
        private Rect tabletRect = new Rect(26, 26, 510, 700);
        private int selectedTab;
        private string keyInput = "";
        private string activationMessage = "";

        public bool HideWatermark;

        private GUIStyle panel, header, subHeader, label, small, button, selectedButton, premiumButton;
        private Texture2D panelTexture, cardTexture, accentTexture, premiumTexture;

        public void Toggle() => visible = !visible;

        private void Start() => BuildStyles();

        private void OnGUI()
        {
            if (!visible) return;
            if (panel == null) BuildStyles();
            tabletRect = GUI.Window(90421, tabletRect, DrawTablet, "");
        }

        private void BuildStyles()
        {
            panelTexture = MakeTexture(new Color(0.028f, 0.032f, 0.045f, 0.985f));
            cardTexture = MakeTexture(new Color(0.065f, 0.072f, 0.095f, 0.98f));
            accentTexture = MakeTexture(new Color(1f, 0.48f, 0.06f, 1f));
            premiumTexture = MakeTexture(new Color(0.22f, 0.16f, 0.09f, 1f));

            panel = new GUIStyle(GUI.skin.box) { normal = { background = panelTexture, textColor = Color.white }, padding = new RectOffset(20, 20, 18, 18) };
            header = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            subHeader = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, normal = { textColor = new Color(1f, 0.48f, 0.06f, 1f) } };
            label = new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = new Color(0.84f, 0.86f, 0.92f) } };
            small = new GUIStyle(label) { fontSize = 11, normal = { textColor = new Color(0.55f, 0.59f, 0.69f) } };
            button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12, alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white, background = cardTexture },
                hover = { textColor = Color.white, background = MakeTexture(new Color(0.12f, 0.13f, 0.17f, 1f)) },
                active = { textColor = Color.black, background = accentTexture },
                padding = new RectOffset(10, 10, 9, 9)
            };
            selectedButton = new GUIStyle(button) { normal = { textColor = Color.black, background = accentTexture } };
            premiumButton = new GUIStyle(button) { normal = { textColor = new Color(1f, 0.78f, 0.45f, 1f), background = premiumTexture } };
        }

        private void DrawTablet(int id)
        {
            CameraController cam = Plugin.Instance.Camera;
            bool plus = Plugin.Instance.Licenses.IsPlus;

            GUILayout.BeginVertical(panel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("SNOW'S CAMERA", header);
            GUILayout.FlexibleSpace();
            GUILayout.Label(plus ? "PLUS" : "FREE", plus ? subHeader : small, GUILayout.Width(55));
            if (GUILayout.Button("×", button, GUILayout.Width(34), GUILayout.Height(30))) visible = false;
            GUILayout.EndHorizontal();
            GUILayout.Label("SNOW TABLET  •  PC CASTING", small);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            DrawTab("CAMERA", 0); DrawTab("SPECTATE", 1); DrawTab("STYLE", 2); DrawTab("INJECTOR", 3); DrawTab("PLUS", 4);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (selectedTab == 0) DrawCameraTab();
            else if (selectedTab == 1) DrawSpectateTab();
            else if (selectedTab == 2) DrawStyleTab(plus);
            else if (selectedTab == 3) DrawInjectorTab(plus);
            else DrawPlusTab(plus);

            GUILayout.Space(8);
            GUILayout.Label("Y  Call Tablet  •  F6  Desktop UI  •  F7 Mode", small);
            GUI.DragWindow(new Rect(0, 0, 10000, 24));
            GUILayout.EndVertical();
        }

        private void DrawCameraTab()
        {
            CameraController cam = Plugin.Instance.Camera;
            GUILayout.Label("CAMERA MODES", subHeader);
            GUILayout.BeginHorizontal();
            DrawModeButton(cam, CameraMode.FirstPerson, "1ST");
            DrawModeButton(cam, CameraMode.ThirdPerson, "3RD");
            DrawModeButton(cam, CameraMode.Freecam, "FREE");
            DrawModeButton(cam, CameraMode.Spectator, "SPEC");
            GUILayout.EndHorizontal();

            GUILayout.Space(12);
            DrawSlider("FOV", ref cam.FieldOfView, 45f, 140f, "0");
            DrawSlider("SMOOTHNESS", ref cam.Smoothness, 0.01f, 0.5f, "0.00");
            DrawSlider("NEAR CLIP", ref cam.NearClip, 0.005f, 0.5f, "0.000");

            if (cam.Mode == CameraMode.ThirdPerson || cam.Mode == CameraMode.Spectator)
            {
                DrawSlider("DISTANCE", ref cam.ThirdPersonDistance, 0.5f, 12f, "0.00");
                DrawSlider("HEIGHT", ref cam.ThirdPersonHeight, -1f, 2f, "0.00");
                cam.AutoOrbit = GUILayout.Toggle(cam.AutoOrbit, "  Auto orbit");
                cam.Collision = GUILayout.Toggle(cam.Collision, "  Camera collision");
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("← TARGET", button)) cam.SelectRelative(-1);
            if (GUILayout.Button("RESET", button)) cam.ResetToLocal();
            if (GUILayout.Button("TARGET →", button)) cam.SelectRelative(1);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("QUICK PRESETS", subHeader);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("CINEMA", Plugin.Instance.Licenses.IsPlus ? selectedButton : premiumButton) && Plugin.Instance.Licenses.IsPlus)
            {
                cam.FieldOfView = 58f; cam.Smoothness = 0.09f; cam.ThirdPersonDistance = 4.5f; cam.ThirdPersonHeight = 0.45f;
            }
            if (GUILayout.Button("ACTION", button))
            {
                cam.FieldOfView = 96f; cam.Smoothness = 0.22f; cam.ThirdPersonDistance = 2.6f; cam.ThirdPersonHeight = 0.2f;
            }
            if (GUILayout.Button("CLOSE", button))
            {
                cam.FieldOfView = 72f; cam.Smoothness = 0.15f; cam.ThirdPersonDistance = 3.25f; cam.ThirdPersonHeight = 0.25f;
            }
            GUILayout.EndHorizontal();
            if (!Plugin.Instance.Licenses.IsPlus) GUILayout.Label("Cinema preset is a Plus feature.", small);
        }

        private void DrawSpectateTab()
        {
            var players = Plugin.Instance.Players.Players;
            GUILayout.Label("SPECTATOR TARGETS  •  " + players.Count, subHeader);
            playerScroll = GUILayout.BeginScrollView(playerScroll, GUILayout.Height(440));
            for (int i = 0; i < players.Count; i++)
            {
                TrackedPlayer player = players[i];
                bool selected = Plugin.Instance.Camera.SelectedPlayer == player;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button((player.IsLocal ? "YOU  " : "") + player.DisplayName, selected ? selectedButton : button, GUILayout.Height(34)))
                {
                    Plugin.Instance.Camera.Select(player);
                    Plugin.Instance.Camera.SetMode(CameraMode.Spectator);
                }
                GUILayout.Label(player.IsLocal ? "LOCAL" : "PLAYER", small, GUILayout.Width(58));
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            GUILayout.EndScrollView();
            GUILayout.Label("The crown is always attached to the local player, never the spectator target.", small);
        }

        private void DrawStyleTab(bool plus)
        {
            GUILayout.Label("WATERMARK", subHeader);
            GUILayout.Label("Default is a tiny, faint white snow pattern in the bottom-left.", small);

            GUILayout.BeginHorizontal();
            DrawThemeButton("SNOW", WatermarkTheme.Snow, true);
            DrawThemeButton("MINIMAL", WatermarkTheme.Minimal, true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawThemeButton("ORANGE", WatermarkTheme.Orange, true);
            DrawThemeButton("MIDNIGHT", WatermarkTheme.Midnight, true);
            GUILayout.EndHorizontal();
            DrawThemeButton("AURORA", WatermarkTheme.Aurora, plus);

            GUILayout.Space(10);
            if (plus)
            {
                HideWatermark = GUILayout.Toggle(HideWatermark, "  Remove watermark");
                GUILayout.Label("Plus removes the watermark completely.", small);
            }
            else GUILayout.Label("Remove watermark is a Plus feature.", small);

            GUILayout.Space(12);
            GUILayout.Label("LOCAL CROWN", subHeader);
            Plugin.Instance.Badge.Enabled = GUILayout.Toggle(Plugin.Instance.Badge.Enabled, "  Show orange crown");
            Plugin.Instance.Badge.ShowCustomName = GUILayout.Toggle(Plugin.Instance.Badge.ShowCustomName, "  Show my custom name");
            if (Plugin.Instance.Badge.ShowCustomName)
            {
                GUILayout.Label("Custom name", small);
                Plugin.Instance.Badge.CustomName = GUILayout.TextField(Plugin.Instance.Badge.CustomName ?? "Snow", GUILayout.Height(27));
            }
            GUILayout.Label("Local-only visual. It never changes network identity.", small);
        }

        private void DrawInjectorTab(bool plus)
        {
            GUILayout.Label("CAMERA INJECTOR", subHeader);
            GUILayout.Label("Safe in-game activator for the already-loaded Snow's Camera plugin. It does not load arbitrary DLLs.", small);
            GUILayout.Space(12);

            GUILayout.BeginVertical(panel);
            GUILayout.Label(Plugin.Instance.Licenses.IsInjected ? "● CAMERA CORE READY" : "○ CAMERA CORE NOT ACTIVATED", label);
            GUILayout.Label("BepInEx loads the plugin at startup. Inject activates this camera session.", small);
            GUILayout.Space(8);
            if (GUILayout.Button(Plugin.Instance.Licenses.IsInjected ? "RELOAD CAMERA CORE" : "INJECT CAMERA CORE", selectedButton, GUILayout.Height(42)))
            {
                Plugin.Instance.Licenses.Inject();
                Plugin.Instance.Camera.ResetToLocal();
            }
            GUILayout.EndVertical();

            GUILayout.Space(12);
            GUILayout.Label("PLUS KEY", subHeader);
            keyInput = GUILayout.TextField(keyInput, GUILayout.Height(32));
            if (GUILayout.Button("ACTIVATE PLUS", plus ? selectedButton : button, GUILayout.Height(36)))
            {
                activationMessage = Plugin.Instance.Licenses.Activate(keyInput)
                    ? "Plus activated on this installation."
                    : "Invalid Plus key. Format: SCM-XXXX-XXXX-XXXX-XXXX-XXXX";
                keyInput = "";
            }
            if (!string.IsNullOrEmpty(activationMessage)) GUILayout.Label(activationMessage, small);
            GUILayout.Label("Uppercase A-F and 0-9 only inside each block.", small);
        }

        private void DrawPlusTab(bool plus)
        {
            GUILayout.Label(plus ? "SNOW'S CAMERA PLUS" : "SNOW'S CAMERA FREE", subHeader);
            if (plus)
            {
                GUILayout.Label("PLUS ACTIVE  •  premium camera tools unlocked", label);
                GUILayout.Space(8);
                DrawFeature("✓", "Remove watermark");
                DrawFeature("✓", "Cinema preset");
                DrawFeature("✓", "Aurora theme");
                DrawFeature("✓", "Premium camera profiles");
                DrawFeature("✓", "Advanced casting controls");
                GUILayout.Space(10);
                if (GUILayout.Button("DEACTIVATE PLUS", button)) Plugin.Instance.Licenses.Deactivate();
            }
            else
            {
                GUILayout.Label("Free includes the core camera, player tracking, spectator mode, freecam, FOV, smoothness, near clip, presets and Snow watermark.", small);
                GUILayout.Space(10);
                GUILayout.BeginVertical(panel);
                GUILayout.Label("PLUS UNLOCKS", subHeader);
                DrawFeature("★", "Remove watermark");
                DrawFeature("★", "Cinema preset");
                DrawFeature("★", "Aurora theme");
                DrawFeature("★", "Premium camera profiles");
                DrawFeature("★", "Advanced casting controls");
                GUILayout.EndVertical();
                GUILayout.Space(10);
                if (GUILayout.Button("OPEN ACTIVATOR", premiumButton, GUILayout.Height(40))) selectedTab = 3;
            }
        }

        private void DrawFeature(string icon, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(icon, subHeader, GUILayout.Width(22));
            GUILayout.Label(text, label);
            GUILayout.EndHorizontal();
        }

        private void DrawSlider(string name, ref float value, float min, float max, string format)
        {
            GUILayout.Label(name + "  " + value.ToString(format), label);
            value = GUILayout.HorizontalSlider(value, min, max);
        }

        private void DrawThemeButton(string text, WatermarkTheme theme, bool unlocked)
        {
            if (GUILayout.Button(text, unlocked && Plugin.Instance.Watermark.Theme == theme ? selectedButton : unlocked ? button : premiumButton, GUILayout.Height(32)))
            {
                if (unlocked) Plugin.Instance.Watermark.Theme = theme;
            }
        }

        private void DrawModeButton(CameraController cam, CameraMode mode, string text)
        {
            if (GUILayout.Button(text, cam.Mode == mode ? selectedButton : button, GUILayout.Height(34))) cam.SetMode(mode);
        }

        private void DrawTab(string text, int tab)
        {
            if (GUILayout.Button(text, selectedTab == tab ? selectedButton : button, GUILayout.Height(31))) selectedTab = tab;
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