# Snow's Camera Mod

**Snow's Camera Mod 2.0** is a PC-focused Gorilla Tag camera/casting mod with an in-game tablet, spectator tools, freecam, local branding, a tiny snow watermark, and an optional Plus tier.

## Free
- In-game camera tablet with F6
- 1st person, normal PC-style 3rd person, spectator and freecam
- Player list and quick target switching
- FOV, smoothness, near clip, distance and height controls
- Camera collision and auto orbit
- Action and close presets
- Orange local crown badge with optional local custom name
- Tiny faint white snow watermark
- Snow, Minimal, Orange and Midnight watermark themes
- Safe in-game camera-core injector/activator

## Plus
- Remove watermark
- Cinema preset
- Aurora theme
- Premium camera profiles
- Advanced casting controls
- Persistent local Plus activation

## Tablet
**CAMERA** controls the camera.

**SPECTATE** shows detected players and lets you select a target.

**STYLE** controls the tiny watermark, themes, crown and local custom name.

**INJECTOR** activates the camera core and accepts Plus keys.

**PLUS** shows the current license and Plus feature list.

## Controls

| Key | Action |
|---|---|
| F6 | Open / close camera tablet |
| F7 | Cycle camera mode |
| [ | Previous player |
| ] | Next player |
| W A S D | Freecam movement |
| Q / E | Freecam down / up |
| Hold RMB | Freecam look |
| Shift | Freecam speed |

## Plus key format

Keys use exactly:

`SCM-XXXX-XXXX-XXXX-XXXX-XXXX`

Only uppercase **A-F** and **0-9** are accepted in each block.

The repository stores SHA-256 hashes rather than plaintext keys.

**Important:** this is client-side licensing, not secure commercial DRM. A user who modifies the local DLL can bypass a local check. A real commercial Plus service should use a server-backed activation system.

## Build

Game/reference DLLs are intentionally not committed.

1. Install Gorilla Tag on Steam.
2. Install BepInEx 5.
3. Start Gorilla Tag once, then close it.
4. Put these DLLs in `libs/`:
   - `BepInEx.dll`
   - `0Harmony.dll`
   - `Assembly-CSharp.dll`
   - `UnityEngine.CoreModule.dll`
   - `UnityEngine.IMGUIModule.dll`
   - `UnityEngine.InputLegacyModule.dll`
   - `UnityEngine.PhysicsModule.dll`
5. Install .NET SDK 8+.
6. Run `dotnet build -c Release`.
7. Copy `bin/Release/SnowsCameraMod.dll` into `Gorilla Tag/BepInEx/plugins/`.

## Architecture

- `Plugin.cs` — entry point and hotkeys
- `CameraController.cs` — camera modes and movement
- `PlayerTracker.cs` — player/VR rig discovery
- `CameraUI.cs` — in-game tablet
- `BadgeOverlay.cs` — local crown
- `WatermarkOverlay.cs` — watermark and themes
- `LicenseManager.cs` — Plus key validation
- `RuntimeReflection.cs` — compatibility layer

The camera is independent from the headset camera for PC/desktop recording and OBS capture.

## Branding behavior

The orange crown/custom name is local-only and always belongs to the local player. It never creates a fake network identity or claims another player is using the mod.

## Modding note

Use mods according to Gorilla Tag's current rules and keep gameplay-interfering mods out of public lobbies. This project is designed for camera/casting functionality, not gameplay advantages.
