using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace SnowsCameraMod
{
    [BepInPlugin("com.snow.snowscamerafree", "Snow's Camera Free", "2.1.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public CameraController Camera;
        public PlayerTracker Players;
        public CameraUI UI;
        public BadgeOverlay Badge;
        public WatermarkOverlay Watermark;
        public LicenseManager Licenses;
        public CameraTabletWorld Tablet;
        public VRButtonInput VRInput;

        internal ConfigEntry<KeyCode> TabletButtonKey;
        internal ConfigEntry<KeyCode> CycleModeKey;
        internal ConfigEntry<KeyCode> NextPlayerKey;
        internal ConfigEntry<KeyCode> PreviousPlayerKey;

        private void Awake()
        {
            Instance = this;

            TabletButtonKey = Config.Bind(
                "Controls",
                "Tablet Keyboard Button",
                KeyCode.Y,
                "Keyboard button used to call the physical camera tablet. Change this in BepInEx/config.");

            CycleModeKey = Config.Bind("Controls", "Cycle Camera Mode", KeyCode.F7, "Cycle camera modes.");
            NextPlayerKey = Config.Bind("Controls", "Next Player", KeyCode.RightBracket, "Select next player.");
            PreviousPlayerKey = Config.Bind("Controls", "Previous Player", KeyCode.LeftBracket, "Select previous player.");

            Licenses = new LicenseManager(Config);
            Players = gameObject.AddComponent<PlayerTracker>();
            Camera = gameObject.AddComponent<CameraController>();
            UI = gameObject.AddComponent<CameraUI>();
            Badge = gameObject.AddComponent<BadgeOverlay>();
            Watermark = gameObject.AddComponent<WatermarkOverlay>();
            VRInput = gameObject.AddComponent<VRButtonInput>();
            Tablet = gameObject.AddComponent<CameraTabletWorld>();

            Logger.LogInfo("Snow's Camera Free 2.1.0 loaded. Press Y to call the physical tablet.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(TabletButtonKey.Value))
                Tablet.Toggle();

            if (Input.GetKeyDown(CycleModeKey.Value)) Camera.CycleMode();
            if (Input.GetKeyDown(NextPlayerKey.Value)) Camera.SelectRelative(1);
            if (Input.GetKeyDown(PreviousPlayerKey.Value)) Camera.SelectRelative(-1);
        }

        public bool TabletButtonPressed()
        {
            return VRInput != null && VRInput.YPressedThisFrame();
        }
    }
}