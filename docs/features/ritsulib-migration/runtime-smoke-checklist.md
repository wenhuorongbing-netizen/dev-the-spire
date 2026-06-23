# Runtime Smoke Checklist

## Purpose

Verify the current RitsuLib-only Spire Plus package in game, without extending
older loader evidence into gameplay or release claims.

This checklist is for the active package line:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.126`
- STS2-RitsuLib `v0.4.34`
- RitsuLib direct NuGet runtime layout
- Stable technical manifest id `EZMicroBalance`

## Current Status

Current beta.126 package parity, runtime preflight, and source-workspace
validation are recorded in `PROJECT_STATE.md` and
`docs/reviews/current-validation.md`; beta.126 still needs game-launch/runtime
patch-count proof. Latest clicked Ancient UI smoke remains beta.123
previous-package evidence under
`.tools/runtime-evidence/monkey-stability-20260622-235746/` with 4 / 4
iterations, all 127 migrated Spire Plus ModPatcher patches from that package
applied, and packet verification 1621 / 0. Beta.99 RitsuLib Mod Settings
clicked UI proof is previous-package context captured under
`.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`;
beta.99 direct Off loader proof is previous-package context captured under
`.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/`.

The beta.99 settings proof proves Settings -> `Mod Settings (RitsuLib)`
visibility for Spire Plus only: the session showed only `RitsuLib` and
`Spire Plus`, opened the Spire Plus page, rendered Migration Status,
`STS2-RitsuLib >= 0.4.34`, evidence-boundary, technical-id, and Preview Tools
controls, retained same-session `godot.log`, audited clean, and passed StS1 Off
runtime shape verification 21 / 0.

The beta.99 Off proof proves startup/loading and default-Off StS1Events
behavior only: direct `--force-steam off` reached main menu with exactly
STS2-RitsuLib and Spire Plus loaded, clean audit, 25/25 Spire Plus patches,
StS1Events disabled with 0 registration lines, Off verifier 21 / 0, and packet
verifier 43 / 0.

The previous beta.96 Off packet is retained at
`.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.
It is previous-package startup/loading context only. Earlier
beta.93 AdditiveBatch1 packets remain older package loader/registration context
only. They do not prove beta.123 enabled-mode gameplay, full gameplay,
save-load, replacement behavior, multiplayer/co-op, QA, or tester handoff.

Coordination boundary: run this checklist's launch, gameplay, build, publish,
package, or release-evidence steps only when a controlled validation lane is
assigned. During a pause, use this checklist only for read-only/static planning,
source-only `-PrintExpected` output, or verification of already-captured logs.

## Prerequisites

1. Clean Steam client install with a game version that has a matching RitsuLib
   variant.
2. STS2-RitsuLib `v0.4.34` or newer installed at
   `<GameRoot>\mods\STS2-RitsuLib`.
3. Spire Plus `v0.1.0-private-beta.126` installed at
   `<GameRoot>\mods\EZMicroBalance` from
   `publish/SpirePlus-v0.1.0-private-beta.126.zip`.
4. Enabled mod set for this lane contains only `STS2-RitsuLib` and
   `EZMicroBalance`.
5. If using `scripts\spire-plus-live-session.ps1`, prepare with explicit
   E-drive paths and restore after evidence capture:

```powershell
$evidence = ".tools\runtime-evidence\<new-beta99-off-proof-dir>"
$steamUserId = "<steam-user-id>"

.\scripts\spire-plus-live-session.ps1 `
  -Mode Prepare `
  -EvidenceDir $evidence `
  -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' `
  -SteamExe 'E:\Steam\steam.exe' `
  -SteamUserId $steamUserId `
  -MoveOtherMods `
  -MoveCurrentRuns `
  -Launch

.\scripts\spire-plus-live-session.ps1 `
  -Mode Restore `
  -EvidenceDir $evidence `
  -StopGameOnRestore `
  -PreserveNewCurrentRunsOnRestore
```

Ensure `STS2-RitsuLib` is not moved out by any mod-isolation step.

## Loader Smoke

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Install STS2-RitsuLib | `<GameRoot>\mods\STS2-RitsuLib` exists and manifest version satisfies `>= 0.4.34` | PASS: E-drive install is `v0.4.34` in direct NuGet runtime layout |
| 2 | Install Spire Plus beta.123 | Installed folder, manifest, DLL, PCK, and package hashes match beta.123 handoff docs | PASS: package parity is recorded in `PROJECT_STATE.md` |
| 3 | Launch beta.123 with only the two allowed mods | Main menu loads without crash | PASS: `.tools/runtime-evidence/monkey-stability-20260622-235746/` |
| 4 | Check `godot.log` for RitsuLib init | RitsuLib initializes and reports no dependency errors | PASS: beta.123 monkey packet verifier 1621 / 0 |
| 5 | Check `godot.log` for Spire Plus init | Single Spire Plus initialization line, technical id `EZMicroBalance`, package `v0.1.0-private-beta.126` | PASS: beta.123 monkey packet verifier 1621 / 0 |
| 6 | Check `godot.log` for ModPatcher count | 127 migrated patch classes register through RitsuLib and remaining raw Harmony patches load without dependency failures | PASS: beta.123 smoke applied all 127 migrated patch classes |
| 7 | Check release-blocking signatures | 0 `MissingMethodException`, `TypeLoadException`, manifest dependency failure, or release-blocking audit hits | PASS: beta.123 clean log audit |
| 8 | Check saved attached-state registration | RitsuLib saved attached-state registration succeeds | PASS: beta.123 clean log audit |

## StS1Events Runtime Gates

| Mode | Required env | Expected | Evidence |
| --- | --- | --- | --- |
| Off | unset / empty / invalid `SPIREPLUS_STS1_EVENT_MODE` | 0 StS1Events registrations, no `[StS1 Events]` registration lines | PASS: beta.123 Ancient UI smoke Off verifier found 0 StS1 registration lines |
| CanaryOnly | `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly` | 4 canary event types / 6 registration calls: Big Fish and Golden Idol in both Act 1 buckets, plus The Lab and Divine Fountain as shared events | [PENDING beta.123 recapture] |
| AdditiveBatch1 | `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1` | 14 registration calls / 10 event types, no TODO/BLOCKED events | [PENDING beta.123 recapture] |
| AdditiveAllDraft | `SPIREPLUS_STS1_EVENT_MODE=AdditiveAllDraft` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; dev-only all-draft mode includes TODO/BLOCKED content | [DO NOT USE for tester/release paths] |
| ReplaceUnknownEventsPrototype | `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` plus `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` | Not release-safe; debug-only replacement prototype; normal builds fail closed | [DO NOT USE for tester/release paths] |

After any future enabled-mode smoke copies `godot.log` and writes
`godot-log-audit.json`, verify the copied files without launching anything:

```powershell
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.126 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.34 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath "<evidence>\godot.log.current-iteration" -AuditPath "<evidence>\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.126 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.34 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\enabled-mode-log-check.json" -FailOnMismatch
```

For helper-created evidence folders, prefer the packet verifier to verify the
packet has the expected copied files, session state, restore state, isolated-mod
list, and clean nested log/audit result. For enabled modes it uses
`godot.log.current-iteration` as canonical proof; retained current slices must
byte-match `godot.log.after-launch` after the `godot.log.before` prefix, and
when that retained slice is absent, it derives the slice only if
`godot.log.before` is a byte prefix of `godot.log.after-launch`, then generates
`godot-log-current-iteration-audit.json` and runs the nested log verifier
against that current slice rather than the full copied log:

```powershell
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.126 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.34 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir "<evidence>" -ExpectedPackageVersion v0.1.0-private-beta.126 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.34 -ExpectedGameVersion 0.107.1 -OutFile "<evidence>\runtime-evidence-packet-check.json" -FailOnMismatch
```

For enabled-mode copied logs, the log verifier requires explicit expected
package-version, Ritsu compat-branch, RitsuLib package-version, and game-version
checks, and the observed registered event-line count matches the source-derived
registration-call count. It verifies registration-call count, event class set,
and observed registration tuples parsed from `Registered act event` /
`Registered shared event` log lines when those tuple details are present. If
future logs lose act/shared tuple detail, Act-bucket proof remains
source-derived until gameplay evidence proves those targets directly.

For enabled-mode packets, the helper-created `session-state.json` must record
`Sts1EventModeEnvironment` equal to the requested mode, `AllowedModIds` exactly
equal to STS2-RitsuLib and EZMicroBalance for the current RitsuLib-only lane,
moved-mod source/destination paths stay under the recorded mods root and
evidence `isolated-mods` folder, restore counts match the session moved-mod and
moved-current-run lists, and the helper-copied `game-release-info.json` must
match the expected game version. The packet verifier rejects unsafe-mode
environment leakage, rejects full-log-only canonical verifier input, rejects
`-AllowMissingSessionState` / `-AllowMissingRestoreState` for enabled-mode
packets, and requires explicit expected package-version, Ritsu compat-branch,
RitsuLib package-version, and game-version checks.

Keep `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` in
the same evidence folder as the copied logs and current-slice audit so verifier
decisions remain reviewable.

Source-only expected shapes can be printed during the coordination pause with
`-PrintExpected`. That output is not enabled-mode proof; it only preserves the
current expected class set and source-derived registration-call count.

## Mod Settings UI

Use the focused helper for this gate so screenshots, route notes, package
hashes, and the checklist stay in one evidence folder:

```powershell
.\scripts\collect-mod-settings-evidence.ps1 -NoLaunch
# Launch through the normal Steam-client live-session path, open Settings -> Mod Settings (RitsuLib), then capture the list.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "<evidence-dir>" -Capture List -RequireSpireForeground
# Open the Spire Plus config page, then capture the page.
.\scripts\collect-mod-settings-evidence.ps1 -EvidenceDir "<evidence-dir>" -Capture Page -RequireSpireForeground
```

The helper does not launch the game, navigate UI, audit logs, or mark this row
passed by itself. A passed settings proof folder must include same-session
`godot.log`, `godot-log-audit.json`, screenshots, route note, and filled
checklist.

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Navigate to Mod Settings | Spire Plus appears in the RitsuLib Mods tree for the current package | PASS: beta.99 settings proof retained as previous-package context; rerun if settings code/resources change |
| 2 | Open Spire Plus settings | Settings UI renders without errors and shows the Migration Status section | PASS: beta.99 settings proof retained as previous-package context |
| 3 | Verify RitsuLib-only status | Runtime dependency card shows `STS2-RitsuLib >= 0.4.34` and the evidence-boundary card states that screenshots prove UI visibility only | PASS: beta.99 settings proof retained as previous-package context |
| 4 | Verify feature toggles | Preview Tools controls render for Crystal Sphere peek, mask alpha, transform prediction, always-show prediction, and preview debug logs | PASS: beta.99 settings proof retained as previous-package context |

## Basic Gameplay

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Start new run | Run starts without errors | [PENDING] |
| 2 | Play first combat | Combat resolves normally | [PENDING] |
| 3 | Visit first shop | Shop renders, no errors | [PENDING] |
| 4 | Check Ancient reward visibility | Default-on Ancients show rebalanced rewards | [PENDING] |
| 5 | Save and reload | Save/load succeeds, no data loss | [PENDING] |

## Multiplayer Disposition

| # | Step | Expected | Evidence |
|---|------|----------|----------|
| 1 | Attempt co-op with Spire Plus enabled | Co-op fails closed for unverified shared-state gameplay | [PENDING] |
| 2 | Check multiplayer diagnostics log | No crash, clean fail-closed message | [PENDING] |

## Exit Criteria

- All beta.123 loader smoke items pass.
- Off mode proves 0 StS1Events registrations in `godot.log`.
- CanaryOnly proves 4 canary event types through 6 registration calls in
  `godot.log`.
- AdditiveBatch1 proves 10 event types through 14 registration calls.
- Mod Settings UI visibility remains verified or explicitly retained as previous-package context when settings code/resources have not changed.
- At least 3 of 5 basic gameplay items pass, with shop and save-load mandatory.
- Multiplayer disposition confirmed fail-closed.
- `godot.log` contains 0 release-blocking hits.

Current exit status: beta.126 package parity, runtime preflight, and
source-workspace validation pass. beta.126 game-launch/runtime patch-count proof
and clicked Ancient UI smoke are pending. Current enabled-mode proof, gameplay,
save-load, replacement behavior, multiplayer/co-op, independent QA, and tester
handoff remain pending.

## Notes

- This checklist supplements `docs/test-plan.md`,
  `docs/features/ritsulib-migration/runtime-hard-block-report-20260531.md`,
  and `docs/release-checklist.md`.
- Evidence should be retained in `.tools/runtime-evidence/` with verifier JSON
  beside copied logs/screenshots.
- If any beta.123 loader smoke item fails, do not proceed to gameplay items;
  diagnose first.
- beta.123 event-option, event-visual, inline-localization, Ascension
  selection/lobby, Batch 4c localization, and visual-hover UI migrations are
  source/package validated with smoke proof that all 127 migrated patch classes
  apply in the installed game. High-risk patch migration still requires a new
  owner decision and fresh validation.
