using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class BadgeOverlay : MonoBehaviour
    {
        public bool Enabled = true;
        public bool ShowCustomName;
        public string CustomName = "Snow";
        private GUIStyle badgeStyle;
        private Texture2D shadowTexture;

        private void Start()
        {
            badgeStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            shadowTexture = MakeTexture(new Color(0f, 0f, 0f, 0.55f));
        }

        private void OnGUI()
        {
            if (!Enabled || Plugin.Instance == null || Plugin.Instance.Camera == null) return;
            Camera cam = Plugin.Instance.Camera.ActiveCamera;
            TrackedPlayer local = Plugin.Instance.Players.GetLocal();
            if (cam == null || local == null || local.Head == null) return;

            Vector3 screen = cam.WorldToScreenPoint(local.Head.position + Vector3.up * 0.42f);
            if (screen.z <= 0f) return;
            screen.y = Screen.height - screen.y;

            string title = "♛";
            if (ShowCustomName && !string.IsNullOrWhiteSpace(CustomName)) title += " " + CustomName;

            badgeStyle.normal.textColor = new Color(1f, 0.48f, 0.06f, 1f);
            GUI.color = new Color(1f, 1f, 1f, 0.35f);
            GUI.DrawTexture(new Rect(screen.x - 38f, screen.y - 15f, 76f, 24f), shadowTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(screen.x - 100f, screen.y - 17f, 200f, 28f), title, badgeStyle);
        }

        private Texture2D MakeTexture(Color color)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, color);
            t.Apply();
            return t;
        }
    }
}