# M5 Revision L Runtime Smoke Plan

Date: 2026-06-10

## Purpose

Define the next runtime smoke needed before any current dirty source or future tester package can be called runtime-loader validated.

## Existing Historical Evidence

| Mode | Evidence path | Current value |
|---|---|---|
| Off | `.tools/runtime-evidence/ritsulib-off-after-target-fix-20260531-2325/` and `.tools/runtime-evidence/smoke-k1-off-20260602-145938/` | Main menu, clean audit, 0 Sts1Events registrations |
| CanaryOnly | `.tools/runtime-evidence/ritsulib-canary-after-target-fix-20260531-2327/` and `.tools/runtime-evidence/smoke-k1-canary3-20260602-151104/` | Main menu, clean audit, exactly 4 canary events |
| AdditiveBatch1 | `.tools/runtime-evidence/additive-batch1-20260602-150445/` | Main menu, clean audit, historical 10 event types / 11 registration calls; current source expects 10 event types / 14 calls |

These rows are useful loader-gate evidence, but they are not fresh proof for the current dirty source or for a new tester package.

## Next Smoke Sequence

Run only after the owner approves a package/versioned handoff or explicitly asks for a fresh local runtime launch, and only after one of these runtime-compatibility conditions is true:

- a matching RitsuLib `lib\0.107.0` variant is installed for the current local game; or
- the local game is deliberately rolled back to the previously validated `v0.106.1` target; or
- the owner explicitly accepts a documented compatibility experiment that is not current-proof.

```powershell
dotnet build EZMicroBalance.sln --no-incremental
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
.\scripts\check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance' -SkipGameRootZipCheck
```

Then capture isolated loader evidence:

```powershell
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir .tools\runtime-evidence\owner-review-off-YYYYMMDD-HHMMSS -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId 76561199353211250 -MoveOtherMods -MoveCurrentRuns -Launch
```

After the main menu:

```powershell
Copy-Item "$env:APPDATA\SlayTheSpire2\logs\godot.log" ".tools\runtime-evidence\owner-review-off-YYYYMMDD-HHMMSS\godot.log.after-launch" -Force
.\scripts\audit-godot-log.ps1 ".tools\runtime-evidence\owner-review-off-YYYYMMDD-HHMMSS\godot.log.after-launch" -OutFile ".tools\runtime-evidence\owner-review-off-YYYYMMDD-HHMMSS\godot-log-audit.json" -FailOnHit
.\scripts\spire-plus-live-session.ps1 -Mode Restore -EvidenceDir ".tools\runtime-evidence\owner-review-off-YYYYMMDD-HHMMSS" -StopGameOnRestore -PreserveNewCurrentRunsOnRestore
```

Repeat with explicit Sts1Events modes:

```powershell
$env:SPIREPLUS_STS1_EVENT_MODE='CanaryOnly'
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir .tools\runtime-evidence\owner-review-canary-YYYYMMDD-HHMMSS -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId 76561199353211250 -MoveOtherMods -MoveCurrentRuns -Launch
Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE

$env:SPIREPLUS_STS1_EVENT_MODE='AdditiveBatch1'
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir .tools\runtime-evidence\owner-review-additive-batch1-YYYYMMDD-HHMMSS -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId 76561199353211250 -MoveOtherMods -MoveCurrentRuns -Launch
Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE
```

## Required Audit Checks

- BaseLib, RitsuLib, and Spire Plus load.
- Spire Plus reports 25/25 ModPatcher patches applied.
- Off mode logs Sts1Events disabled and 0 StS1 registrations.
- CanaryOnly logs exactly the 4 canary events.
- AdditiveBatch1 logs exactly 10 event types through 14 calls.
- `godot-log-audit.json` reports no Godot engine errors, no `MissingMethodException`, no `TypeLoadException`, and no Spire Plus error/exception hits.

## Non-Smoke Proof Still Needed

Runtime smoke does not prove gameplay. Before release-ready claims, still capture event screenshots, EN/ZHS render proof, save-load proof, image/render proof, replacement proof, multiplayer fail-closed proof, and independent QA.
