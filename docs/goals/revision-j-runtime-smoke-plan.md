# Revision J Runtime Smoke Plan

Date: 2026-05-31
HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`

## Purpose

Capture live runtime evidence for the current RitsuLib dependency setup without expanding gameplay or patch migration scope.

## Preconditions

| Item | Required State | Current State |
|---|---|---|
| Game root | `E:\Steam\steamapps\common\Slay the Spire 2` | Present |
| Steam exe | `E:\Steam\steam.exe` | Present per dependency agent |
| BaseLib | `<GameRoot>\mods\BaseLib` | Present, expected `v3.1.4` |
| STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` | Present, `v0.3.10`, includes `lib\0.106.1` |
| Spire Plus | `<GameRoot>\mods\EZMicroBalance` | Present, package `v0.1.0-private-beta.84` |
| Active log | `%APPDATA%\SlayTheSpire2\logs\godot.log` | Present from fresh loader smoke, but audit has 11 Godot ERROR hits |

## Helper Safety

`scripts\spire-plus-live-session.ps1` must preserve `STS2-RitsuLib` when `-MoveOtherMods` is used. The current dirty helper change adds `STS2-RitsuLib` to the default allowed mod ids and to generated enabled settings. If owner rejects that helper change, do not use `-MoveOtherMods` for RitsuLib smoke.

## Off Mode Smoke

```powershell
Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -GameRoot "E:\Steam\steamapps\common\Slay the Spire 2" -SteamExe "E:\Steam\steam.exe" -SteamUserId 76561199353211250 -Language zhs -MoveOtherMods -MoveCurrentRuns -Launch
```

After the main menu loads and the log is written, the audit must be clean or every error must be explicitly dispositioned:

```powershell
.\scripts\audit-godot-log.ps1 -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log" -FailOnHit
```

Required Off proof:

- BaseLib initialized.
- STS2-RitsuLib initialized and selected the `0.106.1` variant.
- Spire Plus initialized.
- RitsuLib bootstrap applied 25 migrated patches.
- Raw Harmony fallback patches loaded without dependency failures.
- Sts1Events bootstrap gate is disabled.
- 0 `[StS1 Events]` registration lines.
- 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failures, or release-blocking audit hits.

## CanaryOnly Smoke

Start from a restored/clean session, ensure Steam receives the environment variable, then run:

```powershell
$env:SPIREPLUS_STS1_EVENT_MODE = "CanaryOnly"
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -GameRoot "E:\Steam\steamapps\common\Slay the Spire 2" -SteamExe "E:\Steam\steam.exe" -SteamUserId 76561199353211250 -Language zhs -MoveOtherMods -MoveCurrentRuns -Launch
```

Required CanaryOnly proof:

- Same loader/dependency proof as Off mode.
- Exactly four StS1 canary registrations.
- Registered canaries are Big Fish, Golden Idol, The Lab, and Divine Fountain.
- No ActEvent registrations.
- No AdditiveBatch1, AdditiveAllDraft, or ReplacementPrototype registration.

## Restore

```powershell
.\scripts\spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <EvidenceDir from session-state.json> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore
```

## Non-Goals

- No Batch 4c.
- No Batch 5.
- No PR7.
- No debug expansion.
- No Sts1Events formalization.
- No longhaul audit.
- No runtime-ready, live-ready, or release-ready claim.
