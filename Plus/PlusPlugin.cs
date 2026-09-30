using BepInEx;
using UnityEngine;

namespace SnowsCameraPlus
{
    [BepInPlugin("com.snow.snowscameraplus", "Snow's Camera Plus", "1.0.0")]
    [BepInDependency("com.snow.snowscamerafree", BepInDependency.DependencyFlags.HardDependency)]
    public sealed class PlusPlugin : BaseUnityPlugin
    {
        private bool open;
        private string key = "";
        private string status = "";
        private GUIStyle title;
        private GUIStyle panel;
        private GUIStyle button;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
                open = !open;
        }

        private void OnGUI()
        {
            if (!open) return;
            if (title == null) BuildStyles();

            GUI.Box(new Rect(0, 0, 1, 1), GUIContent.none);
            Rect window = new Rect(Screen.width / 2f - 230f, Screen.height / 2f - 150f, 460f, 300f);
            GUI.Window(73120, window, DrawWindow, "Snow's Camera Plus");
        }

        private void BuildStyles()
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 23, fontStyle = FontStyle.Bold };
            panel = new GUIStyle(GUI.skin.box) { fontSize = 13 };
            button = new GUIStyle(GUI.skin.button) { fontSize = 14 };
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical(panel);
            GUILayout.Label("SNOW'S CAMERA PLUS", title);
            GUILayout.Label("Activate your Plus license on this installation.");
            GUILayout.Space(12);
            GUILayout.Label("PLUS KEY");
            key = GUILayout.TextField(key, GUILayout.Height(34));
            GUILayout.Space(8);

            if (GUILayout.Button("ACTIVATE PLUS", button, GUILayout.Height(42)))
            {
                status = Activate(key) ? "Plus activated." : "Invalid key. Use SCM-XXXX-XXXX-XXXX-XXXX-XXXX.";
                key = "";
            }

            GUILayout.Space(10);
            GUILayout.Label(status);
            GUILayout.Space(12);
            GUILayout.Label("F8  •  Open / close Plus activation");
            GUI.DragWindow(new Rect(0, 0, 10000, 26));
            GUILayout.EndVertical();
        }

        private bool Activate(string value)
        {
            if (SnowsCameraMod.Plugin.Instance == null || SnowsCameraMod.Plugin.Instance.Licenses == null)
                return false;

            return SnowsCameraMod.Plugin.Instance.Licenses.Activate(value);
        }
    }
}