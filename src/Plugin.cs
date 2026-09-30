using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace SnowsCameraMod
{
    [BepInPlugin("com.snow.snowscameramod", "Snow's Camera Mod", "2.0.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal CameraController Camera;
        internal PlayerTracker Players;
        internal CameraUI UI;
        internal BadgeOverlay Badge;
        internal WatermarkOverlay Watermark;
        internal LicenseManager Licenses;
        internal ConfigEntry<KeyCode> ToggleKey;
        internal ConfigEntry<KeyCode> CycleModeKey;
        internal ConfigEntry<KeyCode> NextPlayerKey;
        internal ConfigEntry<KeyCode> PreviousPlayerKey;

        private void Awake()
        {
            Instance = this;
            ToggleKey = Config.Bind("Controls", "Toggle Tablet", KeyCode.F6, "Open or close the in-game camera tablet.");
            CycleModeKey = Config.Bind("Controls", "Cycle Camera Mode", KeyCode.F7, "Cycle camera modes.");
            NextPlayerKey = Config.Bind("Controls", "Next Player", KeyCode.RightBracket, "Select next player.");
            PreviousPlayerKey = Config.Bind("Controls", "Previous Player", KeyCode.LeftBracket, "Select previous player.");
            Licenses = new LicenseManager(Config);
            Players = gameObject.AddComponent<PlayerTracker>();
            Camera = gameObject.AddComponent<CameraController>();
            UI = gameObject.AddComponent<CameraUI>();
            Badge = gameObject.AddComponent<BadgeOverlay>();
            Watermark = gameObject.AddComponent<WatermarkOverlay>();
            Logger.LogInfo("Snow's Camera Mod 2.0 loaded.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(ToggleKey.Value)) UI.Toggle();
            if (Input.GetKeyDown(CycleModeKey.Value)) Camera.CycleMode();
            if (Input.GetKeyDown(NextPlayerKey.Value)) Camera.SelectRelative(1);
            if (Input.GetKeyDown(PreviousPlayerKey.Value)) Camera.SelectRelative(-1);
        }
    }
}