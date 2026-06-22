# StS1 Events Test Plan

Current coordination note, 2026-06-21: the v19 trace at `docs/features/sts1-events/hard-stop-blocker-report-v19-validation-coordination-20260611.md` is historical beta.85/v0.107.0 coordination-blocker context only; beta.93 RitsuLib-only loader/registration evidence is the current proof boundary. Do not start new `dotnet build`, `dotnet test`, `dotnet publish`, package/release-evidence validation, or game/runtime smoke from a parallel same-repository thread while a migration validation lane is active. Use this plan only after the coordination pause is lifted, or for read-only/static planning. The current v20 hard-stop trace is `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`.

Previous beta.93 proof covers AdditiveBatch1 enabled-mode startup on Slay the Spire 2 `v0.107.1` with RitsuLib `v0.4.31` / `lib\0.107.1` and only STS2-RitsuLib as the shared runtime dependency. It does not prove event gameplay, save/load, EN/ZHS render, image/license, replacement-pool behavior, multiplayer behavior, or QA. The previous beta.93 packet passed with 10 observed event types / 14 registered-event lines and exact act/shared tuple parity. Retained beta.87 proof remains clean `v0.107.0` previous-game-version context only.

Gate mapping after the 2026-06-21 enabled-mode lane: `O33` (current AdditiveBatch1 smoke) is pass for `v0.107.1` loader/registration with retained verifier reports. `O25` (CanaryOnly smoke) remains beta.85 previous-package context, and `O26-O29` plus `O31-O41` (canary gameplay/save-load/render/docs/owner rows), `O42-O52` (simple-batch gameplay/save-load/render/QA rows), `O54-O57` (replacement functional proof), `O58` and `O64` (multiplayer/ZHS runtime rows), and `O65` plus `O72-O75` (independent QA/final handoff rows) remain blocked or current-pending in this thread. `O53` is source-guarded only, `O59-O63` are static classification/safety rows, `O66-O71` are documentation-in-progress rows, and `O76` is a static non-completion invariant; none of those static/documentation rows close runtime or handoff gates.

## Automated Tests

### 1. Manifest Integrity Test
Verify that all events in the manifest have corresponding:
- Source file in `EZMicroBalanceCode/Sts1Events/`
- Localization entries in both `eng/sts1_events.json` and `zhs/sts1_events.json`
- Documentation in `event-specs/`

### 2. Localization Completeness Test
Verify that every event class has all required localization keys:
- `{ENTRY}.title`
- `{ENTRY}.pages.INITIAL.description`
- At least one `{ENTRY}.pages.INITIAL.options.*.title`
- Every source-referenced `L10NLookup("...")` result-page key
- Every `InitialOptionKey("...")` title/description key
- Every custom `OptionKey(page, option)` title/description key

Current known gap: `docs/features/sts1-events/localization-source-gap-scan-20260611.md` lists 33 source-referenced keys missing from both EN and ZHS. `STS1_GOLDEN_IDOL.pages.LEAVE.description` affects current CanaryOnly/AdditiveBatch1 directly and should be fixed first. `docs/features/sts1-events/localization-gap-closure-plan.md` records the safe validated resource-pass order. Close the full gap before claiming source-complete localization. Fixing the direct Golden Idol key only removes the missing-key blocker; it does not prove gameplay behavior or replace the enabled-mode log verifier/runtime evidence packet.

Static checker:

```powershell
.\scripts\check-sts1-localization-source-keys.ps1
```

Use `-FailOnMissing` only after the known gap is expected to be closed.

### 3. Build Verification
Before starting build/test/runtime validation, run the aggregate static event suite:

```powershell
.\scripts\check-sts1-event-static-suite.ps1
```

By default, the suite treats the current 33 source-referenced localization keys as a known non-failing gap. Once those keys are expected to be closed, run:

```powershell
.\scripts\check-sts1-event-static-suite.ps1 -FailOnLocalizationMissing
```

Targeted static checks are also available for narrower debugging.

Reproduce current doc-claim drift coverage with:

```powershell
.\scripts\check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
```

This verifies active current-facing docs keep the `57 / 14` count matrix, beta.86 AdditiveBatch1 proof boundary, and current gameplay/handoff rows open.

Reproduce the individual O0-O76 gate-status ledger with:

```powershell
.\scripts\check-sts1-v19-gate-ledger.ps1 -FailOnMismatch
```

The ledger file is `docs/features/sts1-events/v19-gate-ledger.csv`. This verifies every O0-O76 gate has one row and that current runtime/gameplay gates are not accidentally marked as passing.

Reproduce the v20 final-gate overlay for `docs/goals/event.md` O76-O84 with:

```powershell
.\scripts\check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch
```

The overlay file is `docs/features/sts1-events/v20-final-gate-overlay.csv`. This does not replace the v19 O0-O76 ledger; it tracks the v20 final documentation, owner-action, no-unsupported-commit/push, release-claim, final-summary, and next-run boundaries without closing runtime or handoff gates.

Reproduce v19 subagent role coverage with:

```powershell
.\scripts\check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch
```

The coverage file is `docs/features/sts1-events/v19-subagent-coverage.md`. This verifies all 15 `docs/goals/event.md` subagent roles are represented without treating read-only/static subagent work as runtime, gameplay, or QA proof.

Reproduce the source count matrix with the static registry-shape checker:

```powershell
.\scripts\check-sts1-event-registry-shape.ps1 -FailOnMismatch
```

This verifies the current `52 / 54 / 50 / 48 / 47 / 57 / 14` source shape, CanaryOnly registration identity, and AdditiveBatch1 registration identity without starting `dotnet`.

Reproduce the current localization gap impact split with:

```powershell
.\scripts\check-sts1-localization-gap-baseline.ps1 -FailOnMismatch
```

This verifies the known 33 missing source-referenced localization keys, including the single current CanaryOnly/AdditiveBatch1 blocker, the later RegisterAll/combat/custom-UI buckets, and closure-plan cue coverage for every missing key. Update this baseline only in the same pass that intentionally closes or reclassifies the localization gap. The intended closure order is documented in `docs/features/sts1-events/localization-gap-closure-plan.md`.

Reproduce per-event spec registration-note coverage with:

```powershell
.\scripts\check-sts1-event-spec-registration-notes.ps1 -FailOnMismatch
```

Reproduce feature-gate safety coverage with:

```powershell
.\scripts\check-sts1-event-feature-gates.ps1 -FailOnMismatch
```

Reproduce temporary-substitute, combat-blocker, and non-combat classification coverage with:

```powershell
.\scripts\check-sts1-event-parity-blockers.ps1 -FailOnMismatch
```

Reproduce StS1 event asset safety and no-tracked-original-art coverage with:

```powershell
.\scripts\check-sts1-event-asset-safety.ps1 -FailOnMismatch
```

Reproduce source-level multiplayer/`IsShared` classification coverage with:

```powershell
.\scripts\check-sts1-event-multiplayer-shape.ps1 -FailOnMismatch
```

Print the source-derived expected enabled-mode log shapes without launching the game:

```powershell
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -PrintExpected
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -PrintExpected
```

This prints the current expected CanaryOnly 4 event types / 6 source registration calls and AdditiveBatch1 10 event types / 14 source registration calls. It is not runtime proof.

Check installed runtime prerequisites without launching the game:

```powershell
.\scripts\check-sts1-runtime-preflight.ps1 -FailOnMismatch
```

This reads the game `release_info.json`, installed `STS2-RitsuLib` manifest and compat target, installed Spire Plus manifest, and source-only CanaryOnly/AdditiveBatch1 expected shapes. It is a prerequisite check only; it does not launch the game, audit a runtime log, or prove enabled-mode runtime/gameplay.

Verify an already-captured helper evidence packet without launching the game:

```powershell
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621" -LogFileName "godot.log.current-iteration" -ExpectedPackageVersion v0.1.0-private-beta.93 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.31 -ExpectedGameVersion 0.107.1 -OutFile ".tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621\runtime-evidence-packet-check.json" -FailOnMismatch
```

Use the same packet checker with `-Mode CanaryOnly` or `-Mode AdditiveBatch1` after future enabled-mode smoke folders exist. This validates the packet shape, StS1 mode environment metadata, and nested log/audit result; it does not launch the game.

`dotnet build` must succeed with all event files included.

Current validation target after the coordination pause is lifted:

- Build: 0 errors / 0 warnings.
- Tests: split no-build lanes at the current accurate count, last recorded as 489 passed / 0 failed / 39 skipped / 528 total.
- Opt-in artifact subset: last recorded as 67 passed / 0 failed / 2 skipped / 69 total.
- `git diff --check` clean.

Automated tests and package checks are not gameplay evidence.

## Manual Testing

### Current Enabled-Mode Smoke Order

Before event screenshots or gameplay proof can count as current evidence:

1. Preserve beta.85 Off and CanaryOnly proof as previous-package loader context only.
2. Run `.\scripts\check-sts1-runtime-preflight.ps1 -FailOnMismatch` and stop before launching if the installed game, RitsuLib, repo/installed Spire Plus package manifests, or source-only expected shapes do not match the documented post-pause package target.
3. Treat `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621` as previous beta.93 `v0.107.1` AdditiveBatch1 loader smoke only: 10 event types / 14 registration calls, retained log verifier 31 / 0, retained packet verifier 61 / 0. It still does not prove beta.96 loader behavior or gameplay.
4. The first beta.86 Steam-client AdditiveBatch1 attempt at `.tools\runtime-evidence\v01070-beta86-additive-batch1-20260618-031043` is diagnostic only because StS1 stayed disabled when the already-running Steam client did not propagate the transient PowerShell environment. Use direct launch with a temporary `steam_appid.txt` when validating enabled modes if Steam is already running.
5. Only after the relevant enabled-mode smoke is clean, capture event encounters, result logs, pre/post state, save/load, EN/ZHS render, and image/license disposition.

Use a fresh evidence folder for each mode. Replace `REPLACE_WITH_STEAM_USER_ID` and timestamp placeholders before running:

```powershell
$steamUserId = 'REPLACE_WITH_STEAM_USER_ID'
$env:SPIREPLUS_STS1_EVENT_MODE='CanaryOnly'
$evidence = '.tools\runtime-evidence\sts1-canary-v01071-YYYYMMDD-HHMMSS'
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir $evidence -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId $steamUserId -MoveOtherMods -MoveCurrentRuns -Launch
# After the main menu loads:
Copy-Item "$env:APPDATA\SlayTheSpire2\logs\godot.log" "$evidence\godot.log.after-launch" -Force
.\scripts\spire-plus-live-session.ps1 -Mode Restore -EvidenceDir $evidence -StopGameOnRestore -PreserveNewCurrentRunsOnRestore
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode CanaryOnly -EvidenceDir $evidence -ExpectedPackageVersion v0.1.0-private-beta.104 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.33 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\runtime-evidence-packet-check.json" -FailOnMismatch
.\scripts\audit-godot-log.ps1 "$evidence\godot.log.current-iteration" -OutFile "$evidence\godot-log-current-iteration-audit.json" -FailOnHit
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode CanaryOnly -LogPath "$evidence\godot.log.current-iteration" -AuditPath "$evidence\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.104 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.33 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\enabled-mode-log-check.json" -FailOnMismatch
Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
```

```powershell
$steamUserId = 'REPLACE_WITH_STEAM_USER_ID'
$env:SPIREPLUS_STS1_EVENT_MODE='AdditiveBatch1'
$evidence = '.tools\runtime-evidence\sts1-additive-batch1-v01071-YYYYMMDD-HHMMSS'
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -EvidenceDir $evidence -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -SteamExe 'E:\Steam\steam.exe' -SteamUserId $steamUserId -MoveOtherMods -MoveCurrentRuns -Launch
# After the main menu loads:
Copy-Item "$env:APPDATA\SlayTheSpire2\logs\godot.log" "$evidence\godot.log.after-launch" -Force
.\scripts\spire-plus-live-session.ps1 -Mode Restore -EvidenceDir $evidence -StopGameOnRestore -PreserveNewCurrentRunsOnRestore
.\scripts\check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir $evidence -ExpectedPackageVersion v0.1.0-private-beta.104 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.33 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\runtime-evidence-packet-check.json" -FailOnMismatch
.\scripts\audit-godot-log.ps1 "$evidence\godot.log.current-iteration" -OutFile "$evidence\godot-log-current-iteration-audit.json" -FailOnHit
.\scripts\check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath "$evidence\godot.log.current-iteration" -AuditPath "$evidence\godot-log-current-iteration-audit.json" -ExpectedPackageVersion v0.1.0-private-beta.104 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.33 -ExpectedGameVersion 0.107.1 -OutFile "$evidence\enabled-mode-log-check.json" -FailOnMismatch
Remove-Item Env:\SPIREPLUS_STS1_EVENT_MODE -ErrorAction SilentlyContinue
```

For each smoke, keep `session-state.json`, `settings.save.before`, `game-release-info.json`, `godot.log.before`, `godot.log.after-launch`, `godot.log.current-iteration`, `godot-log-current-iteration-audit.json`, `enabled-mode-log-check.json`, `runtime-evidence-packet-check.json`, and `restore-state.json`. Current helper-created `session-state.json` records `Sts1EventModeEnvironment`; the packet verifier requires explicit `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, and `-ExpectedGameVersion` checks for enabled-mode evidence, and enabled-mode packet checks must not use `-AllowMissingSessionState` or `-AllowMissingRestoreState`. The packet verifier uses `godot.log.current-iteration` as the canonical enabled-mode log; retained current slices must byte-match `godot.log.after-launch` after the `godot.log.before` prefix, and if the helper did not retain that slice, it derives the slice only when `godot.log.before` is a byte prefix of `godot.log.after-launch`, generates `godot-log-current-iteration-audit.json`, and runs the copied-log verifier against the derived slice rather than the full log. The standalone copied-log verifier command above is a retained current-slice check; the packet verifier remains the canonical enabled-mode packet check. The packet verifier requires the session StS1 mode metadata to match `CanaryOnly` or `AdditiveBatch1` and rejects unsafe-mode environment leakage. If the current slice, game release info, or session metadata does not show the requested StS1 mode and runtime target, stop the game and Steam client, clear the environment variable, and rerun in a fresh evidence folder rather than reusing ambiguous evidence. The copied-log verifier requires the observed registered event-line count to match the source-derived registration-call count, the observed event classes to match the source-derived class set, and observed `Registered act event` / `Registered shared event` tuples to match the source-derived tuple set when tuple detail is present. If future logs lose act/shared tuple detail, Act-bucket proof remains source-derived until gameplay evidence proves those targets directly. Replace the hardcoded beta.86 package version with the newly built/installed package version after any versioned code, resource, localization, package, or handoff change.

For enabled-mode evidence, the packet verifier requires explicit `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, and `-ExpectedGameVersion` checks for enabled-mode evidence.

Older RitsuLib logs were class-only, so Act-bucket tuple proof for those historical packets remains source-derived. Previous beta.93 AdditiveBatch1 direct evidence includes tuple-aware verifier parity, but gameplay evidence is still required before claiming event-pool behavior.

### Debug Spawn Test (Canary Events)

Only run this section after the validation coordination pause is lifted and after a successful build/publish/install pass.

1. Build with `dotnet build`.
2. Publish with `dotnet publish`.
3. Install to `<GameRoot>/mods/EZMicroBalance/`.
4. Start a new run.
5. Use debug console to spawn events:
   - `event sts1_big_fish`
   - `event sts1_golden_idol`
   - `event sts1_the_lab`
   - `event sts1_divine_fountain`
6. Verify all options work correctly.
7. Verify localization displays properly.

Debug-spawn proof is useful for option behavior, but it does not replace random-pool enabled-mode smoke or replacement-pool proof.

### Big Fish Test Matrix

| Option | Expected Result | Pass/Fail |
|--------|----------------|-----------|
| Banana | Heal 1/3 max HP | |
| Donut | +5 max HP | |
| Box | Random relic + Regret curse | |

### Golden Idol Test Matrix

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Take -> Outrun | Injury curse | Same | |
| Take -> Smash | Lose 25% max HP as HP damage | Lose 35% max HP as HP damage | |
| Take -> Hide | Lose 8% max HP | Lose 10% max HP | |
| Leave | Nothing | Same | |

### Golden Shrine AdditiveBatch1 Check

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Pray | Gain 100 gold | Gain 50 gold | |
| Desecrate | Gain 275 gold and obtain Regret | Same | |
| Leave | Nothing | Same | |

### The Cleric AdditiveBatch1 Check

| State | Option | Expected Result | A15+ | Pass/Fail |
|-------|--------|----------------|------|-----------|
| Player has 35+ gold | Heal | Spend 35 gold, heal 25% max HP | Same | |
| Player has fewer than 35 gold | Encounter eligibility | Event should not appear from the random pool | Same | |
| Normal, player has 50+ gold | Purify | Spend 50 gold, then remove 1 card | N/A | |
| A15+, player has 75+ gold | Purify | N/A | Spend 75 gold, then remove 1 card | |
| Player has less than Purify cost | Purify | Option is unavailable; no card-removal UI opens | Same | |
| Any | Leave | Nothing | Same | |

### The Lab Test Matrix

| Option | Expected Result | A15+ | Pass/Fail |
|--------|----------------|------|-----------|
| Open | Obtain 3 random potions | Obtain 2 random potions | |

### Old Beggar AdditiveBatch1 Check

| State | Option | Expected Result | Pass/Fail |
|-------|--------|----------------|-----------|
| Player has 75+ gold | Offer Gold | Spend 75 gold, then remove 1 card | |
| Player has fewer than 75 gold | Offer Gold | Option is unavailable; no card-removal UI opens | |

### Shining Light AdditiveBatch1 Check

| State | Option | Expected Result | Pass/Fail |
|-------|--------|----------------|-----------|
| Normal | Enter | Lose 30% max HP as unblockable damage, then 2 random upgradable deck cards upgrade without opening the card picker | |
| Ascension 15+ | Enter | Lose 40% max HP as unblockable damage, then 2 random upgradable deck cards upgrade without opening the card picker | |
| Fewer than 2 upgradable cards | Enter | Upgrade every available upgradable deck card, up to 2 | |

### Localization Test

1. Switch game language to English -> verify all text displays
2. Switch game language to Chinese -> verify all text displays
3. Verify no missing/placeholder text

## Test File

`tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs`

Tests:
- All manifest events have source files
- All manifest events have localization entries
- Localization key format is valid
- No duplicate event entries
