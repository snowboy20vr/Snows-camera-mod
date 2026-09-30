using UnityEngine;

namespace SnowsCameraMod
{
    public sealed class BadgeOverlay : MonoBehaviour
    {
        public bool Enabled = true;
        public bool ShowCustomName = false;
        public string CustomName = "Snow";
        private GUIStyle badgeStyle;

        private void Start()
        {
            badgeStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
        }

        private void OnGUI()
        {
            if (!Enabled || Plugin.Instance == null || Plugin.Instance.Camera == null) return;
            Camera cam = Plugin.Instance.Camera.ActiveCamera;
            TrackedPlayer target = Plugin.Instance.Players.GetLocal();
            if (cam == null || target == null || target.Head == null) return;

            Vector3 screen = cam.WorldToScreenPoint(target.Head.position + Vector3.up * 0.38f);
            if (screen.z <= 0f) return;
            screen.y = Screen.height - screen.y;

            string title = "♛  CAMERA";
            if (ShowCustomName && !string.IsNullOrWhiteSpace(CustomName))
                title += "  " + CustomName;

            badgeStyle.normal.textColor = new Color(1f, 0.48f, 0.06f, 1f);
            GUI.Label(new Rect(screen.x - 100f, screen.y - 18f, 200f, 28f), title, badgeStyle);
        }
    }
}