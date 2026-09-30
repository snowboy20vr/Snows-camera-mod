using UnityEngine;

namespace SnowsCameraMod
{
    public enum WatermarkTheme { Snow, Minimal, Orange, Midnight, Aurora }

    public sealed class WatermarkOverlay : MonoBehaviour
    {
        public WatermarkTheme Theme = WatermarkTheme.Snow;
        public bool Visible = true;
        public float Opacity = 0.16f;

        private GUIStyle textStyle;
        private Texture2D lineTexture;

        private void Start()
        {
            textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft
            };
            lineTexture = MakeTexture(Color.white);
        }

        private void OnGUI()
        {
            if (!Visible || Plugin.Instance == null || Plugin.Instance.Licenses == null) return;
            if (Plugin.Instance.Licenses.IsPlus && Plugin.Instance.UI != null && Plugin.Instance.UI.HideWatermark) return;
            if (textStyle == null) Start();

            float alpha = Mathf.Clamp01(Opacity);
            textStyle.normal.textColor = GetTextColor(alpha);
            GUI.Label(new Rect(12f, Screen.height - 26f, 260f, 16f), GetLabel(), textStyle);

            if (Theme == WatermarkTheme.Snow) DrawSnowPattern(alpha);
        }

        private string GetLabel()
        {
            switch (Theme)
            {
                case WatermarkTheme.Orange: return "SNOW'S CAMERA  •  CAMERA ACTIVE";
                case WatermarkTheme.Minimal: return "SNOW'S CAMERA";
                case WatermarkTheme.Midnight: return "SNOW'S CAMERA  /  PC CAST";
                case WatermarkTheme.Aurora: return "SNOW'S CAMERA  •  LIVE";
                default: return "❄ SNOW'S CAMERA";
            }
        }

        private Color GetTextColor(float alpha)
        {
            if (Theme == WatermarkTheme.Orange) return new Color(1f, 0.48f, 0.06f, alpha);
            if (Theme == WatermarkTheme.Aurora) return new Color(0.78f, 0.95f, 1f, alpha);
            return new Color(1f, 1f, 1f, alpha);
        }

        private void DrawSnowPattern(float alpha)
        {
            Color c = new Color(1f, 1f, 1f, alpha * 0.42f);
            for (int i = 0; i < 7; i++)
            {
                float x = 12f + i * 27f;
                float y = Screen.height - 11f - (i % 3) * 4f;
                GUI.color = c;
                GUI.DrawTexture(new Rect(x, y, 1f, 9f), lineTexture);
                GUI.DrawTexture(new Rect(x - 4f, y + 4f, 9f, 1f), lineTexture);
                GUI.DrawTexture(new Rect(x - 3f, y + 1f, 1f, 7f), lineTexture);
                GUI.DrawTexture(new Rect(x + 3f, y + 1f, 1f, 7f), lineTexture);
            }
            GUI.color = Color.white;
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