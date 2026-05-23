# Remote Development Setup

This guide explains how to clone this private repository on another Windows machine and recreate the local-only development environment.

## Baseline

- Active project: `EZMicroBalance`
- Player-facing mod name: `Spire Plus`
- Legacy scaffold id: `EzDailyContent`
- Game: Slay the Spire 2 public beta
- Verified version: `v0.106.0`, installed/source-refreshed locally on `2026-05-22`
- BaseLib runtime: `v3.1.4`
- Build command: `dotnet build`
- Publish command: `dotnet publish`

## 1. Clone the repository

```powershell
git clone <PRIVATE_REPO_URL>
cd dev-the-spire
```

## 2. Install .NET 9 SDK

Install the .NET 9 SDK from Microsoft, then verify:

```powershell
dotnet --list-sdks
dotnet --info
```

## 3. Install Godot .NET / Mono 4.5.1

Install or download the Godot .NET / Mono 4.5.1 Windows x86_64 build.

You may place it anywhere local, for example under `.tools\`, but `.tools/` is ignored and should not be committed.

Record the full executable path for `GodotPath`.

## 4. Install Slay the Spire 2 through Steam

Install Slay the Spire 2 through Steam and record the game root, for example:

```text
D:\Steam\steamapps\common\Slay the Spire 2
```

## 5. Install BaseLib v3.1.4

Install BaseLib `v3.1.4` runtime files into:

```text
<GameRoot>\mods\BaseLib
```

Required files:

```text
BaseLib.json
BaseLib.dll
BaseLib.pck
```

Do not commit BaseLib runtime binaries into this repository.

## 6. Create local Directory.Build.props

Copy:

```text
Directory.Build.props.example
```

To:

```text
Directory.Build.props
```

Then fill in:

```xml
<GodotPath>...</GodotPath>
<Sts2Path>...</Sts2Path>
```

`Directory.Build.props` is local and must not be committed.

## 7. Build and publish

```powershell
dotnet build
dotnet publish
```

Expected published mod output:

```text
<GameRoot>\mods\EZMicroBalance\EZMicroBalance.json
<GameRoot>\mods\EZMicroBalance\EZMicroBalance.dll
<GameRoot>\mods\EZMicroBalance\EZMicroBalance.pck
```

## 8. Launch and verify in game

1. Launch Slay the Spire 2.
2. Open Settings.
3. Open Mod Settings.
4. Confirm BaseLib appears.
5. Confirm BaseLib is enabled.
6. Confirm Spire Plus appears with manifest id `EZMicroBalance`.
7. Confirm Spire Plus is enabled.
8. Test with only BaseLib and `Spire Plus / EZMicroBalance` enabled; do not install or enable old scaffold packages.

## Local-only files that must not be committed

- `Directory.Build.props`
- `.tools/`
- `.godot/`
- `bin/`
- `obj/`
- downloaded archives such as `.zip`, `.7z`, `.rar`
- tool binaries such as `.exe`
- generated or runtime `.dll` and `.pck` files
- personal tokens or credentials

## Bootstrap helper

A conservative helper script is available:

```powershell
.\scripts\bootstrap-windows.ps1 -GameRoot "D:\Steam\steamapps\common\Slay the Spire 2" -GodotExePath "D:\path\to\Godot_v4.5.1-stable_mono_win64.exe"
```

The script does not delete files and does not download dependencies.
