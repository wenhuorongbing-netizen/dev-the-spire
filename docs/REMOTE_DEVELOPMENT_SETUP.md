# Remote Development Setup

This guide explains how to clone this private repository on another Windows machine and recreate the local-only development environment.

## 2026-06-22 Current Boundary

Current local installed game evidence is Slay the Spire 2 `v0.107.1` with Spire Plus depending on STS2-RitsuLib `v0.4.34` in direct NuGet runtime layout only. Current beta.115 package parity, runtime preflight, and source-workspace validation are refreshed; clicked UI proof is still the previous beta.108 smoke packet. Previous beta.108 clicked UI proof is `.tools/runtime-evidence/monkey-stability-beta108-20260622-172312/`: it covered Urda, Morvi, Lotha, and normal Vakuu with 4 / 4 iterations, clean audits, exact package/game/Ritsu markers, all 64 Spire Plus patches, and packet verification 1621 / 0. Previous beta.99 settings proof is `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`. Previous beta.99 Off proof is `.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/`: it reached main menu with exactly STS2-RitsuLib and Spire Plus loaded, clean audit, 25/25 Spire Plus patches, StS1Events disabled with 0 registration lines, Off verifier 21 / 0, and packet verifier 43 / 0. Previous beta.96 Off proof is `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`, and previous beta.93 AdditiveBatch1 loader/registration proof is `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/` for previous-package context only. Previous-package/game-version beta.85, beta.87, beta.88, and beta.90 proof remains context only. That proof covers startup, settings, loader registration, or smoke-level clicked UI only; remote setup, enabled modes, gameplay, save-load, replacement, multiplayer, QA, package handoff, and release-ready compatibility still require their own current evidence on the target machine.

## Baseline

- Active mod: `Spire Plus`
- Technical project, manifest id, and install folder: `EZMicroBalance`
- Legacy scaffold id: `EzDailyContent`
- Game: Slay the Spire 2 public/main branch
- Current local version: `v0.107.1`, installed/source-refreshed locally on `2026-06-20`
- RitsuLib runtime: `STS2-RitsuLib` `v0.4.34` direct NuGet runtime layout
- Spire Plus package: `v0.1.0-private-beta.115`
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

## 5. Install STS2-RitsuLib v0.4.34

Install STS2-RitsuLib `v0.4.34` runtime files into:

```text
<GameRoot>\mods\STS2-RitsuLib
```

Required files:

```text
mod_manifest.json
STS2-RitsuLib.dll
STS2-RitsuLib.xml
viewer\
```

Do not commit STS2-RitsuLib runtime binaries into this repository.

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
4. Confirm STS2-RitsuLib appears.
5. Confirm STS2-RitsuLib is enabled.
6. Confirm Spire Plus appears with manifest id `EZMicroBalance`.
7. Confirm Spire Plus is enabled.
8. Test with only STS2-RitsuLib and `Spire Plus` enabled; do not install or enable old scaffold packages.

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
