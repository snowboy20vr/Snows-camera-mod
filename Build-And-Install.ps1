$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  SNOW'S CAMERA MOD - AUTO BUILD/INSTALL" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

$ScriptRoot = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ScriptRoot)) { $ScriptRoot = (Get-Location).Path }

$Root = $null
$probe = Get-Item -LiteralPath $ScriptRoot
for ($i = 0; $i -lt 5 -and $probe; $i++) {
    if (Test-Path (Join-Path $probe.FullName "Free\SnowsCameraFree.csproj")) {
        $Root = $probe.FullName
        break
    }
    $probe = $probe.Parent
}
if (-not $Root) {
    $candidate = Get-ChildItem -LiteralPath $ScriptRoot -Filter "SnowsCameraFree.csproj" -Recurse -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($candidate) { $Root = $candidate.Directory.Parent.FullName }
}
if (-not $Root) { throw "Could not find Free\SnowsCameraFree.csproj. Download the latest repository ZIP and run this script again." }
Write-Host "Project root: $Root" -ForegroundColor DarkCyan

function Find-GorillaTag {
    $candidates = @(
        "$env:ProgramFiles(x86)\Steam\steamapps\common\Gorilla Tag",
        "$env:ProgramFiles\Steam\steamapps\common\Gorilla Tag",
        "$env:ProgramFiles(x86)\Steam\steamapps\common\Gorilla Tag",
        "D:\SteamLibrary\steamapps\common\Gorilla Tag",
        "C:\SteamLibrary\steamapps\common\Gorilla Tag",
        "$env:USERPROFILE\OneDrive\Desktop\SteamLibrary\steamapps\common\Gorilla Tag"
    )

    foreach ($path in $candidates) {
        if (Test-Path "$path\Gorilla Tag.exe") { return $path }
    }

    return $null
}

$Game = Find-GorillaTag

if (-not $Game) {
    Write-Host "Could not automatically find Gorilla Tag." -ForegroundColor Yellow
    $Game = Read-Host "Paste your Gorilla Tag folder path"
}

if (-not (Test-Path "$Game\Gorilla Tag.exe")) {
    throw "Gorilla Tag.exe was not found at: $Game"
}

$Managed = Join-Path $Game "Gorilla Tag_Data\Managed"
$BepCore = Join-Path $Game "BepInEx\core"
$Libs = Join-Path $Root "libs"

New-Item -ItemType Directory -Force -Path $Libs | Out-Null

$required = @(
    @{ Name="UnityEngine.dll"; Path=(Join-Path $Managed "UnityEngine.dll") },
    @{ Name="UnityEngine.ParticleSystemModule.dll"; Path=(Join-Path $Managed "UnityEngine.ParticleSystemModule.dll") },
    @{ Name="UnityEngine.TextRenderingModule.dll"; Path=(Join-Path $Managed "UnityEngine.TextRenderingModule.dll") },
    @{ Name="Assembly-CSharp.dll"; Path=(Join-Path $Managed "Assembly-CSharp.dll") },
    @{ Name="UnityEngine.CoreModule.dll"; Path=(Join-Path $Managed "UnityEngine.CoreModule.dll") },
    @{ Name="UnityEngine.IMGUIModule.dll"; Path=(Join-Path $Managed "UnityEngine.IMGUIModule.dll") },
    @{ Name="UnityEngine.InputLegacyModule.dll"; Path=(Join-Path $Managed "UnityEngine.InputLegacyModule.dll") },
    @{ Name="UnityEngine.PhysicsModule.dll"; Path=(Join-Path $Managed "UnityEngine.PhysicsModule.dll") },
    @{ Name="UnityEngine.XRModule.dll"; Path=(Join-Path $Managed "UnityEngine.XRModule.dll") },
    @{ Name="BepInEx.dll"; Path=(Join-Path $BepCore "BepInEx.dll") },
    @{ Name="0Harmony.dll"; Path=(Join-Path $BepCore "0Harmony.dll") }
)

Write-Host "Checking Gorilla Tag references..." -ForegroundColor Cyan

foreach ($item in $required) {
    if (-not (Test-Path $item.Path)) {
        throw "Missing required DLL: $($item.Path)"
    }

    Copy-Item $item.Path (Join-Path $Libs $item.Name) -Force
    Write-Host "  OK $($item.Name)" -ForegroundColor Green
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK is not installed. Install it from https://dotnet.microsoft.com/download/dotnet"
}

Write-Host ""
Write-Host "Building Free..." -ForegroundColor Cyan
dotnet build (Join-Path $Root "Free\SnowsCameraFree.csproj") -c Release
if ($LASTEXITCODE -ne 0) { throw "Free build failed." }

$FreeDll = Join-Path $Root "Free\bin\Release\SnowsCameraFree.dll"
if (-not (Test-Path $FreeDll)) { throw "Free DLL was not produced." }

Write-Host ""
Write-Host "Building Plus..." -ForegroundColor Cyan
dotnet build (Join-Path $Root "Plus\SnowsCameraPlus.csproj") -c Release
if ($LASTEXITCODE -ne 0) { throw "Plus build failed." }

$PlusDll = Join-Path $Root "Plus\bin\Release\SnowsCameraPlus.dll"
if (-not (Test-Path $PlusDll)) { throw "Plus DLL was not produced." }

$Plugins = Join-Path $Game "BepInEx\plugins"
New-Item -ItemType Directory -Force -Path $Plugins | Out-Null

Write-Host ""
Write-Host "Installing DLLs..." -ForegroundColor Cyan

Remove-Item (Join-Path $Plugins "SnowsCameraMod.dll") -Force -ErrorAction SilentlyContinue
Remove-Item (Join-Path $Plugins "SnowsCameraFree.dll") -Force -ErrorAction SilentlyContinue
Remove-Item (Join-Path $Plugins "SnowsCameraPlus.dll") -Force -ErrorAction SilentlyContinue

Copy-Item $FreeDll (Join-Path $Plugins "SnowsCameraFree.dll") -Force
Copy-Item $PlusDll (Join-Path $Plugins "SnowsCameraPlus.dll") -Force

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host " BUILD + INSTALL COMPLETE" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Free: $Plugins\SnowsCameraFree.dll"
Write-Host "Plus: $Plugins\SnowsCameraPlus.dll"
Write-Host ""
Write-Host "Launch Gorilla Tag and press Y to call the tablet."
Write-Host "Press F8 for the Plus activation GUI."
Write-Host ""
