# Building the two DLLs

## Free DLL

From the repository root:

```powershell
dotnet build Free/SnowsCameraFree.csproj -c Release
```

Output:

`Free/bin/Release/SnowsCameraFree.dll`

Install it at:

`Gorilla Tag/BepInEx/plugins/SnowsCameraFree.dll`

## Plus DLL

Build Free first, then:

```powershell
dotnet build Plus/SnowsCameraPlus.csproj -c Release
```

Output:

`Plus/bin/Release/SnowsCameraPlus.dll`

Install it beside the Free DLL:

`Gorilla Tag/BepInEx/plugins/SnowsCameraPlus.dll`

## In-game tablet

The Free DLL creates a physical Snow's Camera tablet and a dock marker near a detected Stump/Treehouse object. Press the configured tablet button to call the tablet to the player.

Default:
- Keyboard: Y
- VR: Y-style controller button (left secondary / right primary fallback)

F6 remains available as a desktop fallback for the existing tablet UI.

## Plus PC GUI

With the Plus DLL installed, press F8 to open the Plus activation GUI and enter:

`SCM-XXXX-XXXX-XXXX-XXXX-XXXX`

This GUI is part of the Plus DLL. The Plus DLL depends on the Free DLL and does not replace it.

## Important

Do not install multiple copies of the old `SnowsCameraMod.dll` at the same time as the new Free DLL. Remove the old DLL first.
