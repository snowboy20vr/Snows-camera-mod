using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace SnowsCameraMod
{
    [BepInPlugin("com.snow.snowscameramod", "Snow's Camera Mod", "1.0.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal CameraController Camera;
        internal PlayerTracker Players;
        internal CameraUI UI;
        internal ConfigEntry<KeyCode> ToggleKey;
        internal ConfigEntry<KeyCode> CycleModeKey;
        internal ConfigEntry<KeyCode> NextPlayerKey;
        internal ConfigEntry<KeyCode> PreviousPlayerKey;

        private void Awake()
        {
            Instance = this;
            ToggleKey = Config.Bind("Controls", "Toggle Menu", KeyCode.F6, "Open or close Snow's Camera Mod.");
            CycleModeKey = Config.Bind("Controls", "Cycle Camera Mode", KeyCode.F7, "Cycle camera modes.");
            NextPlayerKey = Config.Bind("Controls", "Next Player", KeyCode.RightBracket, "Select next player.");
            PreviousPlayerKey = Config.Bind("Controls", "Previous Player", KeyCode.LeftBracket, "Select previous player.");

            Players = gameObject.AddComponent<PlayerTracker>();
            Camera = gameObject.AddComponent<CameraController>();
            UI = gameObject.AddComponent<CameraUI>();
            Logger.LogInfo("Snow's Camera Mod loaded.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(ToggleKey.Value)) UI.Toggle();
            if (Input.GetKeyDown(CycleModeKey.Value)) Camera.CycleMode();
            if (Input.GetKeyDown(NextPlayerKey.Value)) Camera.SelectRelative(1);
            if (Input.GetKeyDown(PreviousPlayerKey.Value)) Camera.SelectRelative(-1);
        }

        internal static void Log(string message)
        {
            if (Instance != null) Instance.Logger.LogInfo(message);
        }
    }
}