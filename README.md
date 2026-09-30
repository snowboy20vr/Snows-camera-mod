# Snow's Camera Mod

A clean, modern PC camera and spectator mod for Gorilla Tag.

## Included

- **1st person** camera
- **Normal PC-style 3rd person** camera
- **Spectator** mode with a player selector
- **Freecam** with WASD + mouse look
- Smooth camera movement
- FOV control
- Smoothness control
- Near-clip control
- Third-person distance control
- Camera collision
- Auto-orbit
- Player list with detected names
- Previous/next player hotkeys
- Clean orange-accent desktop UI
- Local orange **♛ CAMERA** badge
- Optional local custom name beside the crown
- No fake network identity changes

## Controls

| Key | Action |
|---|---|
| F6 | Open / close camera UI |
| F7 | Cycle camera mode |
| [ | Previous player |
| ] | Next player |
| W A S D | Freecam movement |
| Q / E | Freecam down / up |
| Hold RMB | Freecam look |
| Shift | Freecam speed |

## Build

This is a **source project**. Game/reference DLLs are intentionally not committed.

1. Install Gorilla Tag on Steam.
2. Install BepInEx 5 with a current Gorilla Tag mod manager.
3. Start Gorilla Tag once, then close it.
4. Copy these DLLs into `libs/`:
   - `BepInEx.dll`
   - `0Harmony.dll`
   - `Assembly-CSharp.dll`
   - `UnityEngine.CoreModule.dll`
   - `UnityEngine.IMGUIModule.dll`
   - `UnityEngine.InputLegacyModule.dll`
   - `UnityEngine.PhysicsModule.dll`
5. Install .NET SDK 8+.
6. Run:

```text
dotnet build -c Release
```

7. Copy `bin/Release/SnowsCameraMod.dll` into:

```text
Gorilla Tag/BepInEx/plugins/
```

## Architecture

- `Plugin.cs` — BepInEx entry point and hotkeys.
- `CameraController.cs` — camera modes, smoothing, orbit, collision and freecam.
- `PlayerTracker.cs` — runtime player/VR rig discovery.
- `CameraUI.cs` — desktop control panel.
- `BadgeOverlay.cs` — local orange crown/camera marker.
- `RuntimeReflection.cs` — compatibility layer for changing Gorilla Tag fields.

The camera is independent from the headset camera, making it suitable for normal PC/desktop recording and OBS capture.

## Safety / lobby use

Use mods according to Gorilla Tag's current rules. The Gorilla Tag modding guide specifically notes that gameplay-interfering mods should be kept to private lobbies. This camera mod is designed around camera/casting functionality rather than gameplay advantages.

## Current limitation

The orange crown/custom name is intentionally **local-only**. It does not pretend to be a real networked name tag and does not write a custom identity into Photon/player data. That keeps the visual feature clean and avoids depending on unstable game-networking internals.

## Roadmap

- Camera presets
- Cinematic keyframes
- Smooth orbit profiles
- OBS clean-output toggle
- Camera bookmarks
- Better VRRig name resolution across game updates
