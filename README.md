# Snow's Camera Mod

A PC/VR Gorilla Tag camera mod with a physical in-game tablet, spectator camera, freecam, smooth PC-style third person, local orange crown, snowflake trail and separate Free/Plus builds.

## Builds

### Free
Output:

`Free/bin/Release/SnowsCameraFree.dll`

Includes:
- Physical Snow's Camera tablet
- Tablet dock near the Stump/Treehouse
- Y button summon
- Configurable keyboard summon key
- VR Y-style controller input
- Snowflake particle trail
- 1st person
- PC-style 3rd person
- Freecam
- Spectator mode
- Player list
- FOV
- Smoothness
- Near clip
- Distance/height
- Camera collision
- Auto orbit
- Orange local crown
- Custom local name
- Free watermark themes

### Plus
Output:

`Plus/bin/Release/SnowsCameraPlus.dll`

The Plus build depends on the Free build and adds:
- PC activation GUI
- Plus key entry
- Premium camera features
- Remove-watermark support
- Premium presets/themes

## Physical tablet

The Free build creates a 3D tablet object in-game.

When a Stump/Treehouse object is detected, a small orange dock marker is placed nearby. The tablet is stored there while inactive.

Press **Y** to call the tablet.

When called, the tablet moves into the player's camera view and emits a small white snowflake trail.

Press Y again to put it away.

F6 remains available as a desktop fallback for the existing camera interface.

## Changing the summon key

The default keyboard summon key is:

`Y`

BepInEx stores the setting in:

`BepInEx/config/com.snow.snowscamerafree.cfg`

Change:

`Tablet Keyboard Button = Y`

to another keyboard key if desired.

VR input also supports the common Y-style controller button mapping.

## Plus PC activation

Install both:

`SnowsCameraFree.dll`

and

`SnowsCameraPlus.dll`

Then launch Gorilla Tag and press:

**F8**

The Plus PC panel lets you enter:

`SCM-XXXX-XXXX-XXXX-XXXX-XXXX`

The key is checked by the existing local license manager.

## Build Free

From the repository root:

```powershell
dotnet build Free/SnowsCameraFree.csproj -c Release
```

## Build Plus

Build Free first:

```powershell
dotnet build Free/SnowsCameraFree.csproj -c Release
dotnet build Plus/SnowsCameraPlus.csproj -c Release
```

## Installation

Put both DLLs in:

`Gorilla Tag/BepInEx/plugins/`

Do not install the old `SnowsCameraMod.dll` at the same time. Use the new Free + Plus pair.

## Controls

| Input | Action |
|---|---|
| Y | Call / hide physical tablet |
| F6 | Desktop camera interface |
| F7 | Cycle camera mode |
| [ | Previous player |
| ] | Next player |
| W A S D | Freecam movement |
| Q / E | Freecam vertical movement |
| Right mouse | Freecam look |
| Shift | Freecam speed |
| F8 | Plus PC activation panel |

## Important

The Plus key system is local client-side licensing. It is not unbreakable DRM. A user who modifies their own local assembly can bypass a local-only check. A real commercial licensing system should use a server-backed activation service.

Use camera/casting mods according to Gorilla Tag's current modding rules and avoid gameplay advantages in public lobbies.
