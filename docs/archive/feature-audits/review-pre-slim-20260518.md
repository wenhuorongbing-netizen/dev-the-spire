# Release Implementation Static Review

Date: 2026-05-17

Scope: static review of the active release/test-ready implementation: `EZMicroBalanceCode`, legacy `EzDailyContentCode` participation, resources/export lists, localization JSON, tests, packaging scripts, and current documentation. The game was not opened in this review pass.

## Full File Coverage Addendum - 2026-05-17

Scope expansion: current non-archive inventory covered `EZMicroBalanceCode` (110 files), `EZMicroBalance` resources/localization/scenes (199 files), `tests` (40 files), `scripts` (13 files), and current docs (68 files). Legacy scaffold participation was separately inventoried under `EzDailyContentCode` and `EzDailyContent`; it remains traceability-only and is not compiled by `EZMicroBalance.csproj`.

Additional findings from this stricter pass:

- P2: `scripts/bootstrap-windows.ps1` still introduced itself as the old `EzDailyContent` bootstrap, still warned for previous framework v3.1.0, and told testers to confirm `EzDailyContent` in Mod Settings. Fix: retarget script text to Spire Plus / `EZMicroBalance` and previous framework v3.1.2.
- P2: `docs/features/ancient-expansion-v2.2/work-log.md` had widespread corrupted inline-code delimiters: command names, script paths, and resource paths were wrapped in stray leading/trailing `s` characters. Fix: archive the raw historical file under `docs/archive/feature-work-logs/ancient-expansion-v2.2/` and replace the active file with a compact readable current summary.
- P2: README, beta compatibility notes, and changelog still had sentence shapes that made historical 22-field loader evidence sound current. Fix: keep the evidence, but label it historical and keep current 25-field loader parity pending.

Repair guard added: `ReleaseSafetyExpandedGuardTests.BootstrapAndActiveAncientWorkLogStayCurrentAndReadable` now rejects the stale bootstrap strings and common active work-log delimiter corruption.

## Findings And Repair Status

### Fixed P1: Shieldwall Banner Resolved Too Early

`AscensionCombatModifierService.ApplyBannerTurnStart` previously granted Shieldwall Block during player-turn start. The manual rule says Shieldwall protects other enemies each enemy turn, and `ShieldwallLastBlockRound` could suppress the intended enemy-turn grant.

Repair: Shieldwall now resolves from `AfterSideTurnStart(..., CombatSide.Enemy)` through `ApplyShieldwallTurnBlock`. `AscensionV2MilestoneGuardTests` rejects any `case BannerKind.Shieldwall` branch returning to `ApplyBannerTurnStart`.

### Fixed P1: Blood Prize Retaliation Missed The Round-3 End Window

Blood Prize previously applied missed-target retaliation only from player-turn start after `RoundNumber > 3`. The design says the target retaliates if alive when round 3 ends, so that shape gave the target one extra enemy turn before the punishment appeared.

Repair: player-turn end now calls `ApplyBloodPrizePenaltyIfExpired(..., includeCurrentRound: true)`, while player-turn start keeps `includeCurrentRound: false` as a catch-up guard. Source tests assert both paths.

### Fixed P2: Environment Gates Did Not Consistently Trim Whitespace

Ascension and Ancient truthy helpers accept `1/true/yes/on`, but some paths compared the raw environment value. A value like `" 1 "` could fail for Morvi/Lotha/Vakuu/Ascension gates while Urda already opted into trimming.

Repair: truthy candidates and debug-level integer parsing now trim consistently, with static guard coverage.

### Fixed P2: Moss Map Had Two Relic Description Keys With Different Detail

`EZMICROBALANCE-URDA_MOSS_MAP_OPTION_RELIC.description` had the compact exact reward mapping, while legacy class-name key `EZMICROBALANCE-UrdaMossMapOptionRelic.description` still said only that different room types gave different rewards. If runtime resolved the legacy key, the player saw the less useful tooltip.

Repair: both English and Simplified Chinese Moss Map descriptions now share the exact reward mapping, and a guard keeps the duplicate keys equal.

### Fixed P2: Current Docs Overstated Historical Loader Evidence

Current source defines 25 previous saved-state registrations after Seedbed marker work, while the latest loader smoke evidence still reports 22. Some current docs called the 22-field smoke "current" without making the stale field-count boundary obvious.

Repair: current docs now label the 22-field smoke as historical and leave fresh loader parity, live gameplay, save-load, and co-op verification pending.

## Checked Scope

- Active mod source: all files under `EZMicroBalanceCode/**` were included in file inventory and targeted scans. v3.2 Seedbed, Withered Husk, Banner Rooms, Firemarked Elite windows, Root-Sight, Morvi pages/debt/proofread, Lotha powers, and Vakuu fight files were read directly where risk was highest.
- Legacy scaffold: `EzDailyContentCode/**` was inventoried. It is not included by `EZMicroBalance.csproj`, which compiles only `EZMicroBalanceCode/**/*.cs`.
- Resources and export: active event scenes, Vakuu encounter scene, asset path constants, `export_presets.cfg`, art manifest, package script, and release evidence verifier were reviewed for routing and package scope.
- Localization: active English and Simplified Chinese JSON files parse, and targeted player-facing Ancient/Ascension/relic/card/power strings were checked for implementation wording and stale duplicate keys.
- Tests and docs: guard tests, release artifact tests, release checklist, project state, handoff, compatibility, test-ready audit, and changelog were reviewed for hash/evidence drift and manual-gate truthfulness.

## Remaining Live-Only Risks

- Vakuu victory return/no-black-screen, active-fight save/load, prefinished restore, failure/death path, and co-op behavior still require game evidence.
- Urda/Morvi/Lotha live reward flows, clicked Ancient UI, marker relic hover readability, save/load recovery, and co-op ownership remain pending.
- A11 natural route traversal, A12/A16 timing visibility, A19/A20 boss behavior, and root-system combat-end behavior still need live manual rows.
- Static review cannot prove final UI fit, mouse hover cadence, animation timing, or Godot screen layering.

## Validation Plan

After the repair batch:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build
```

No live-game evidence will be claimed unless a separate normal Steam/gameplay pass is actually run.

Validation result, 2026-05-17: `dotnet build EZMicroBalance.sln`, normal `dotnet test EZMicroBalance.sln --no-build` (174 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `scripts/audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch -FailOnMissingFinal`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (192 passed / 0 skipped) passed. No live-game evidence was collected in this review pass.

## Root Sight Map UI Follow-up - 2026-05-18

Scope: targeted source follow-up from `C:/Users/Jack/Downloads/future_peek_dev_the_spire_research_plan.md` and the user's live-test report that Root Eyes did not visibly preview map rooms. The game was not opened in this pass.

Repair plan applied:

- Keep Root Eyes inside `Spire Plus` / `EZMicroBalance`; do not create the separate experimental Future Peek mod from the research plan.
- Preserve the existing preview-only safety rule: Unknown previews fork the Unknown-room RNG, store the preview, and commit live RNG/odds only when the stored node is entered.
- Add the missing UI path: clicking the Root Eyes relic opens the map, selecting a node refreshes map visuals, previewed Unknown nodes show the stored room-category icon, and pure Root Eyes markers use the Root Eyes icon.
- Guard the implementation with source tests so future refactors keep map opening, visual refresh, disabled map-point click capture, preview icon routing, and no direct `UnknownMapPoint.Roll` preview call.

Remaining proof gap: live map click, hover readability, save/load restore, and co-op ownership still need the user's manual test.

Validation result: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` (176 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (194 passed / 0 skipped) passed. `git diff --check` reported only the existing CRLF/LF warning for `ReleaseCoverageGuardTests.cs`. The refreshed package hash is `933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D`; installed DLL hash is `ED3E2A7E4A2FA0129082B424D33B5211583377CF909AFE540B67059FFEAA7663`.

## Morvi Hook/State Refactor - 2026-05-18

Scope: no-game refactor slice to reduce `MorviRunHook.cs` coupling after the Debt/Loan split. The game was not opened.

Repair applied:

- Moved `MorviRunHook` and `MorviCombatHook` into `MorviHooks.cs`.
- Moved selected-blessing state, combat-state holder, deck sync, progress parsing, and progress writing into `MorviBlessingService.State.cs`.
- Kept serialized progress layout unchanged: blessing id, debt remaining, borrowed card id, and borrowed-settled flag still use the same separator and order.
- Updated release source inventory guards so the new Morvi partial files are tracked.

Validation result: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` (176 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (194 passed / 0 skipped) passed. `git diff --check` reported only the existing CRLF/LF warning for `ReleaseCoverageGuardTests.cs`. The refreshed package hash is `933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D`; installed DLL hash is `ED3E2A7E4A2FA0129082B424D33B5211583377CF909AFE540B67059FFEAA7663`.

## Lotha Mirror Rebuttal Refactor - 2026-05-18

Scope: no-game refactor slice to reduce `LothaRunHook.cs` after the earlier hook/state/death/public-evidence splits. The game was not opened.

Repair applied:

- Moved Mirror Rebuttal deck-card eligibility, selected-card marker writes, marker cleanup, combat-start hand pull, and selected-card lookup into `LothaBlessingService.MirrorRebuttal.cs`.
- Kept the existing selected-card field, full-hand top-of-draw fallback, and combat-card lookup rule unchanged.
- Updated release source inventory guards so the new Lotha partial file is tracked.

Validation result: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` (176 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (194 passed / 0 skipped) passed. `git diff --check` reported only the existing CRLF/LF warning for `ReleaseCoverageGuardTests.cs`. The refreshed package hash is `933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D`; installed DLL hash is `ED3E2A7E4A2FA0129082B424D33B5211583377CF909AFE540B67059FFEAA7663`.

## Lotha Single Sentence Refactor - 2026-05-18

Scope: no-game refactor slice to reduce `LothaRunHook.cs` after the Mirror Rebuttal split. The game was not opened.

Repair applied:

- Moved Single Sentence's ready amount, remaining-play limit, Power display helper, Power-card fallback helper, and remaining-play tracker into `LothaBlessingService.SingleSentence.cs`.
- Kept the existing turn-start Power setup, first eligible card ruling, Power-card fallback, autoplay/clone exclusions, and remaining-play cap unchanged.
- Updated release source inventory guards so the new Lotha partial file is tracked.

Validation result: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` (176 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (194 passed / 0 skipped) passed. `git diff --check` reported only the existing CRLF/LF warning for `ReleaseCoverageGuardTests.cs`. The refreshed package hash is `933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D`; installed DLL hash is `ED3E2A7E4A2FA0129082B424D33B5211583377CF909AFE540B67059FFEAA7663`.

## Root Eyes Selectable Map Preview - 2026-05-18

Scope: targeted no-game repair for the user's finding that map preview was not visible enough to count as implemented. The game was not opened.

Repair applied:

- Root Eyes selection now makes future reachable Monster, Unknown, or Elite nodes show the Root Eyes marker while selection is active.
- Stored previews still use the existing no-state-mutation path: Unknown rooms are previewed with forked RNG, and the live Unknown RNG/odds are committed only when the previewed node is entered.
- Closing the map now cancels Root Eyes selection, so a stale selection cannot survive into a later map open.
- Current Urda docs now describe the selectable relic-click flow instead of the old automatic no-map-button fallback.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live map hover/click, save-load, and co-op proof remain pending.

## Velvet Choker Source Split And Ascension Checklist Slim - 2026-05-18

Scope: no-game cleanup slice for logic coupling, source bulk, and active-document sprawl. The game was not opened.

Repair applied:

- Moved Velvet Choker soft-limit patches and `VelvetChokerSoftLimitTracker` from the catch-all `VakuRewardPatches.cs` file into `VelvetChokerPatches.cs`.
- Updated Ancient reward guard tests to read `EZMicroBalanceCode/Ancients/Patches/**` where they assert cross-file patch behavior, so later mechanical splits do not force behavior to remain in one oversized file.
- Replaced the active A11-A20 `development-checklist-v2.md` thousand-line planning draft with a compact checklist and archived the full draft at `docs/archive/feature-inputs/ascension-11-20/development-checklist-v2-full-20260518.md`.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Distinguished Cape Source Split - 2026-05-18

Scope: no-game cleanup slice for the remaining Ancient v4 catch-all patch file. The game was not opened.

Repair applied:

- Moved Distinguished Cape dynamic vars, low-Max-HP Vakuu option replacement/locked fallback, max-HP cost calculation, and Apparition pickup payoff from `VakuRewardPatches.cs` into `DistinguishedCapePatches.cs`.
- Kept the v4.3 behavior unchanged: lose 30% current Max HP with an 18 minimum, block selection if current Max HP cannot pay, preserve Vakuu option count by replacing Cape with same-pool choices when possible, and use a localized locked option only as a defensive fallback.
- Added `DistinguishedCapePatches.cs` to the active source inventory guard.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Jewelry Box Source Split - 2026-05-18

Scope: no-game cleanup slice for the remaining Ancient v4 catch-all patch file. The game was not opened.

Repair applied:

- Moved Jewelry Box pickup handling, non-Innate Apotheosis marker state, Apotheosis keyword removal, and Jewelry Box hover preview patches from `VakuRewardPatches.cs` into `JewelryBoxPatches.cs`.
- Kept behavior unchanged: the granted Apotheosis is marked, has Innate removed, the marker is mirrored through `AncientSavedStateFields.JewelryBoxNonInnateApotheosis`, and hover previews use the same marked preview card path.
- Added `JewelryBoxPatches.cs` to the active source inventory guard.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Preserved Fog Source Split - 2026-05-18

Scope: no-game cleanup slice for the remaining Ancient v4 catch-all patch file. The game was not opened.

Repair applied:

- Moved Preserved Fog pickup handling and Folly keyword override from `VakuRewardPatches.cs` into `PreservedFogPatches.cs`.
- Kept behavior unchanged: Preserved Fog removes up to four deck cards, adds Folly without Ethereal/Retain, and Folly exposes Unplayable, Eternal, and Innate.
- Added `PreservedFogPatches.cs` to the active source inventory guard.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Choices Paradox Source Split - 2026-05-18

Scope: no-game cleanup slice for the remaining Ancient v4 catch-all patch file. The game was not opened.

Repair applied:

- Moved Choices Paradox first-turn rare-card generation, Retain marking, simple-grid selection, unselected-card cleanup, and eligibility filter from `VakuRewardPatches.cs` into `ChoicesParadoxPatches.cs`.
- Kept behavior unchanged: it still acts only for the relic owner on round 1, uses all character pools plus colorless, filters to eligible rare generated cards, removes unselected temporary cards from combat, and adds the selected card to hand.
- Added `ChoicesParadoxPatches.cs` to the active source inventory guard.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Jeweled Mask Source Split - 2026-05-18

Scope: no-game cleanup slice for the remaining Ancient v4 catch-all patch file. The game was not opened.

Repair applied:

- Moved Jeweled Mask combat-start marked-Power hand pull from `VakuRewardPatches.cs` into `JeweledMaskPatches.cs`.
- Kept behavior unchanged: it still runs only for the relic owner on round 1, pulls a marked Power from draw pile to hand, and leaves the existing permanent 0-cost `JeweledMaskFreePower` enchantment code in `Common/JeweledMaskFreePower.cs`.
- Added `JeweledMaskPatches.cs` to the active source inventory guard.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, `dotnet test EZMicroBalance.sln --no-build` passed (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, and `git diff --check` passed with CRLF/LF warnings only for touched pre-existing line-ending files. Package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup slice.

## Subagent Review Round 1 - 2026-05-17

Scope: no-game static review only. I did not launch the game, Steam, live loader smoke, manual gameplay, save/load, or co-op verification. I reviewed the required current docs, targeted `EZMicroBalanceCode` source, active localization/resources, and guard tests for issues that can be proven statically. I did not rerun automated tests in this subagent pass.

### Findings

1. P2 - Stale loader/package wording remains in `docs/dev-environment.md`.
   - Location: `docs/dev-environment.md:26`, `docs/dev-environment.md:76`, `docs/dev-environment.md:86`, `docs/dev-environment.md:92`; related guard gap at `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs:181`.
   - Why this is a bug: `PROJECT_STATE.md` now says the current source defines 25 previous saved-state registrations while the latest live loader log still reports the earlier 22-field package. These `dev-environment` lines still describe the 22-field smoke as current normal/current-package verification in places, which can lead a release handoff to claim current-package runtime parity without the required rerun. The existing release-safety guard checks that current docs mention 25 source fields and previous 22-field smoke evidence, but it does not reject this stale phrase shape in `docs/dev-environment.md`.
   - No-game fix: rewrite the current status bullets in `docs/dev-environment.md` to mirror `PROJECT_STATE.md`: the 2026-05-14/15 loader evidence is historical/pre-refresh 22-field evidence, the refreshed 2026-05-17 package/current 25-field source still needs a fresh live loader rerun, and gameplay/save-load/co-op remain pending. Add or extend a docs guard so `Found 22 previous saved-state registrations` cannot be described as current-package/current normal verification unless the same passage explicitly marks it historical or superseded.

No new no-game actionable code issues were found.

### Non-actionable Live-only Blockers

- Urda/Morvi/Lotha clicked Ancient UI, in-run marker relic visibility, hover readability, live save/load recovery, and co-op ownership remain live-only. Source shows visible marker relic acquisition and state mirroring, but runtime persistence and UI behavior cannot be closed statically.
- Vakuu active fight entry, victory return/no-black-screen behavior, failure/death path, active and prefinished save/load restore, and co-op behavior remain live-only. Source keeps the fight hidden by default, avoids active `ParentEventId`, clears the parent event node before combat, and has a prefinished parent restore path, but the actual room stack/UI transition needs game evidence.
- A11-A20 natural route traversal, Rootblight combat-end behavior, Banner/Firemark timing as presented in live UI, and multiplayer desync/ownership behavior remain live-only. The current source shape does not prove final rendered timing, hover cadence, or screen layering.

### No-finding Areas

- Urda/Morvi/Lotha/Vakuu reward marker relics: checked option relic models plus reward/victory selection paths; no static issue found in marker acquisition or hidden/default visibility shape.
- State carriers and save/load source carriers: checked `AncientPlayerState`, Urda/Morvi/Lotha run hooks, previous saved-state API declarations, and mirror guard tests; no direct state-field bypass or missing source carrier found beyond live persistence proof still pending.
- Runtime gate defaults: checked Urda/Morvi/Lotha default-on disable gates and Vakuu hidden-by-default/single-player fight gate; no static gate-default regression found.
- Vakuu fight source path: checked hidden fight option insertion, direct room transition, no active `ParentEventId` assignment, parent-event node clearing, no-reward encounter behavior, victory Lotha choice path, and contract injection guards; no static source bug found.
- A11-A20/Rootblight/banner/firemark v3.2 changes: checked Shieldwall enemy-turn handling, Blood Prize round-3 end/catch-up paths, Firemark host damage/turn-end paths, and RootBud top-of-draw timing against local source evidence; no obvious static timing bug found.
- EN/zhs localization: targeted scans found no player-facing `NOPE`, source-safe/fallback wording, host/debug terms, Firemark Host wording, setup/burst window wording, or stale common/uncommon development language in active localization. Moss Map duplicate keys are synchronized, and Root Sight/Seed Bank text reads as player-facing.
- Tests and docs guards: reviewed the relevant guard surfaces for Ancient state mirrors, Vakuu save risk, player-facing polish, Ascension timing/docs, package drift, and release artifact checks. The only guard gap found is the stale `docs/dev-environment.md` current-package wording issue above.

## Subagent Review Round 2 - 2026-05-17

Scope: no-game static review only. I did not launch the game, Steam, live loader smoke, manual gameplay, save/load, co-op verification, build, or test. I reviewed `docs/review.md`, `docs/issues.md`, `docs/toreview.md`, `docs/dev-environment.md`, `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`, scanned current non-archive docs for stale 22-field evidence wording, and ran targeted active source/localization scans.

### Findings

1. P2 - Stale 22-field loader wording remains in current-facing docs outside `docs/dev-environment.md`.
   - Location: `docs/private-beta-release-completion-audit.md:41`; `docs/private-beta-verification-handoff.md:5`, `docs/private-beta-verification-handoff.md:7`, `docs/private-beta-verification-handoff.md:58`, `docs/private-beta-verification-handoff.md:64`; `docs/test-ready-completion-audit.md:51`; `docs/features/ancients-rework-v4/completion-audit.md:60`; `docs/features/ancients-rework-v4/manual-verification-matrix.md:62`; `docs/features/ascension-11-20/manual-test-checklist.md:26`, `docs/features/ascension-11-20/manual-test-checklist.md:27`; `docs/mod-changelog.md:105`. Related guard gap: `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs:183` through `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs:193` only reject the stale phrase shape in `docs/dev-environment.md`, while `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs:220` through `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs:225` still accepts test-ready audit wording that depends on the old 22-field smoke.
   - Why this is a bug: `PROJECT_STATE.md` and the repaired `docs/dev-environment.md` now correctly say the 22-field loader smoke is historical and current source defines 25 previous saved-state registrations pending a fresh live loader rerun. The listed current-facing docs still describe the 22-field smoke as `Current`, `current-package`, `current normal Steam-client`, or a `Pass for loader` in self-contained rows/bullets. A release handoff or checklist reader can still infer current-package runtime parity from those pages even though the current 25-field package was not live-loader-smoked.
   - No-game fix: rewrite these rows to match `PROJECT_STATE.md`: the 2026-05-14/15 22-field smoke is historical same-manifest/pre-refresh loader evidence; current source/current package defines 24 fields and needs a fresh live loader rerun before loader-parity or current-package runtime-parity claims; installed-PCK resource-load evidence can remain separate from live loader/gameplay proof. Broaden the docs guard so any non-archive current-facing line that contains `Found 22 previous saved-state registrations` plus `Current`, `current-package`, `current normal`, or `Pass for loader` must also mark the evidence historical/previous/superseded or fail.

### No-finding Areas

- `docs/issues.md` and `docs/toreview.md`: `DOC-DEVENV-22FIELD-STALE` has moved to `docs/toreview.md`, and the active blocker table still leaves live/manual proof gates pending.
- `docs/dev-environment.md`: the requested stale phrases are fixed there; it now marks the 22-field evidence as historical and current 25-field package loader parity as pending.
No new no-game actionable P0/P1/P2 was found.

## Subagent Review Round 3 - 2026-05-17

Scope: no-game static review only. I did not launch the game, Steam, live loader smoke, manual gameplay, save-load, co-op verification, publish, or package. I reviewed `PROJECT_STATE.md`, `docs/issues.md`, `docs/toreview.md`, current-facing docs with 22-field smoke mentions, and `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`; I ran the focused release-safety test class.

### Findings

No new no-game actionable P0/P1/P2 was found.

### Checks

- `docs/issues.md`: no active `DOC-CURRENT-22FIELD-STALE` row remains. The active table is limited to live/manual blockers and proof gates.
- `docs/toreview.md`: records `DOC-CURRENT-22FIELD-STALE` as fixed, with the result that current-facing docs now treat the 22-field Steam/package smoke as historical evidence rather than current 25-field loader parity.
- Current-facing 22-field wording: the previously called-out handoff, release audit, test-ready audit, Ancient audit/matrix, and A11-A20 docs now put `Found 22 previous saved-state registrations` behind a historical/previous/pending boundary. I did not find a remaining self-contained 22-field sentence that claims current/current-package/current normal loader parity or `Pass for loader` without also marking it historical or pending a fresh loader rerun.
- Guard coverage: `ReleaseSafetyExpandedGuardTests` now reads a `CurrentFacingDocs` set that includes `README.md`, `docs/dev-environment.md`, `docs/private-beta-verification-handoff.md`, `docs/private-beta-release-completion-audit.md`, `docs/test-plan.md`, `docs/test-ready-completion-audit.md`, `docs/release-checklist.md`, `docs/features/ancients-rework-v4/completion-audit.md`, `docs/features/ancients-rework-v4/manual-verification-matrix.md`, `docs/features/ascension-11-20/api-research.md`, and `docs/features/ascension-11-20/manual-test-checklist.md`; this is broader than only `dev-environment`.
- Guard behavior: `AssertNoCurrentFacing22FieldSmokePassClaims` fails lines that combine `Found 22 previous saved-state registrations` with current/current-package/current normal/loader-pass language unless the same line marks the evidence historical, previous, earlier, superseded, not refreshed, or pending a fresh loader rerun. That shape avoids the obvious false positive on historical evidence rows. I did not find an obvious false negative inside the guard's current-facing doc set for the requested stale phrase shapes.
- Coverage boundary noted: `docs/BETA_COMPATIBILITY.md` and `docs/mod-changelog.md` are not in the guard's `CurrentFacingDocs` array. Their inspected 22-field/current-package mentions include a historical/pending boundary, so I am not raising this as a no-game P2 in this round.
- Focused validation: `dotnet test EZMicroBalance.sln --no-build --filter FullyQualifiedName~ReleaseSafetyExpandedGuardTests` passed with 13 passed, 6 skipped, 0 failed.

### Residual live-only gates

- Fresh live loader smoke is still required before claiming current 25-field package loader parity.
- Live gameplay/manual verification remains pending for Ancient clicked UI, reward behavior, save-load, natural A11-A20 traversal, Rootblight behavior, Vakuu victory/failure/death paths, disable-gameplay, and co-op.
- Mod Settings and loader evidence that predate the 25-field package remain historical support only, not current runtime parity proof.

## Full File Coverage Addendum 2 - 2026-05-17

Scope: continued strict no-launch review of current mod development files after the previous cleanup. Reviewed high-risk Urda, Morvi, Lotha, Vakuu, Ascension, inline localization, tests, package/resource guards, and current docs. Build was rerun after the text-source repair and passed.

### New Findings

1. P1 - Several inline C# zhs localization providers contained mojibake or overly old wording.
   - Location: `EZMicroBalanceCode/Ascension/Powers/FiremarkPowers.cs`, `EZMicroBalanceCode/Ascension/Powers/BannerPowers.cs`, `EZMicroBalanceCode/Ascension/Powers/BossSealPowers.cs`, `EZMicroBalanceCode/Ascension/Enchantments/FissionEnchantment.cs`, `EZMicroBalanceCode/Ascension/Enchantments/RoyalDecreeEnchantment.cs`, `EZMicroBalanceCode/Ascension/Relics/ForgeTokenRelic.cs`, `EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaTrialBranchEnchantment.cs`.
   - Fix: replaced inline Simplified Chinese strings with short player-facing text for Firemarks, Banners, Boss Seals, Fission, Royal Decree, Forge Token, and Trial Branch.
   - Guard: added a source-level mojibake guard so active C# source is checked, not only JSON localization.

2. P2 - Current docs still had stale art-status wording.
   - Location: `docs/issues/ancient-expansion-v2.2.md`, `docs/mod-changelog.md`, `docs/features/ancient-expansion-v2.2/implementation-plan.md`.
   - Fix: clarified that temporary option/icon art notes were historical and that the current package uses the browser GPTimage2 rebuilt small art, while live UI preview remains pending.

### Source Review Notes

- Root Sight now has a selectable relic-click path, allows future reachable Monster/Unknown/Elite nodes, stores a concrete preview model, adds a hover marker, and patches room creation to use the stored result. Unknown-room preview uses local deterministic preview RNG and does not call the live `UnknownMapPoint.Roll` path, so previewing does not consume real Unknown-room odds or RNG state. Entering a previewed Unknown room commits one live Unknown RNG step and applies the same base-odds reset/increase pattern as vanilla for the stored room type. Event previews respect `ModifyNextEvent`, and entering a previewed event advances the vanilla event queue before marking the stored event visited. The map-click path also catches `NClickableControl._GuiInput` mouse releases so Root Sight selection can work when the map is open for inspection and normal travel clicks are disabled. The entry-commit dedupe table is scoped by `RunState` instance and Root Sight grant clears transient Root Sight state, avoiding stale in-process keys after reloads or same-seed reruns. Live map hover and click evidence is still pending.
- Seed Bank exposes stored-card hover tips and relic-click extraction with cancel support. Static shape is reasonable; live Boss-room freeze verification remains pending because no game launch was performed.
- Morvi Blueprint Proof and Overdue Library now use explicit card/page powers and source-backed card flow. Static build shape is clean; live play-card freeze reports still require runtime log confirmation if they recur.
- Vakuu remains hidden by default, single-player gated, and uses a dedicated monster plus encounter scene. Static source clears the parent event node before entering combat; post-victory black-screen closure still needs live proof.

### Validation Result

Final no-launch validation after this addendum: `dotnet build EZMicroBalance.sln`, `dotnet test EZMicroBalance.sln --no-build` (175 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `scripts/audit-ancient-art-assets.ps1 -FailOnMissingExport -FailOnInvalidGenerationMode -FailOnHashMismatch -FailOnMissingFinal`, `dotnet publish EZMicroBalance.sln`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (193 passed / 0 skipped) passed. The rebuilt package is `publish/SpirePlus-v0.1.0-private-beta.0.zip` with SHA256 `933235A60DD3D82B3EC76FCF066B034AA48908B5BCB959F4A117480D4E3B6C5D`. No live game, save-load, death/failure path, or co-op evidence was collected.

## Refactor Hygiene Pass - 2026-05-17

Scope: no-game cleanup pass for reducing logic coupling, code bulk, and active-document sprawl before further feature work.

### Changes

- Replaced `docs/test-ready-development-goal.md` with a compact current-control document. It now keeps the test-ready stop line, manual evidence gates, active refactor plan, and validation commands without embedding another large one-shot prompt.
- Split shared Ascension combat helper responsibilities out of `AscensionCombatModifierService.cs` into `AscensionCombatModifierService.Helpers.cs`. This is a source-preserving move: activation checks, act-scaling values, primary-enemy filters, and enemy command helpers moved together.
- Split A16 Banner Room behavior out of `AscensionCombatModifierService.cs` into `AscensionCombatModifierService.Banners.cs`. The main file keeps lifecycle dispatch, while Banner fallback, turn, death, card-play, and reward logic now sits in a Banner-owned partial.
- Split A12 Firemarked Elite behavior out of `AscensionCombatModifierService.cs` into `AscensionCombatModifierService.Firemarks.cs`. Host selection, Heat, Molten Core, Forge Armor, Constant Heal, and Firemark turn/damage handling now sit in a Firemark-owned partial.
- Split A19/A20 Boss Seal behavior out of `AscensionCombatModifierService.cs` into `AscensionCombatModifierService.BossSeals.cs`. Combat start, turn boundaries, damage, death, card-play, phase carryover, and courtyard recovery now sit in a Boss-Seal-owned partial.
- Split the oversized Boss Seal partial into lifecycle dispatch, monster-window effects, card-pressure effects, and phase/courtyard carryover files. This keeps each Boss Seal file under 350 lines without changing method bodies.
- Split Root Sight preview/hover/room-routing logic from `UrdaRunHook.cs` into `UrdaBlessingService.RootSight.cs` without changing the `Progress` serialized field layout.
- Split Shallow-Root Relic and Rooted Route reward logic from `UrdaRunHook.cs` into `UrdaBlessingService.RouteRewards.cs` without changing the `Progress` serialized field layout. Removed the stale `ShallowRootSettlementMaxHpLoss` source constant because the implemented and documented fallback is Act 2 relic removal plus 75 Gold refund.
- Split Seed Bank reward storage, relic extraction, stored-card parsing, and relic status refresh from `UrdaRunHook.cs` into `UrdaBlessingService.SeedBank.cs` without changing the `Progress` serialized field layout.
- Split Seedbed rest-option, combat catch, planted-root marking, combat slot persistence, and payoff logic from `UrdaRunHook.cs` into `UrdaBlessingService.Seedbed.cs` without changing the `Progress` serialized field layout.
- Split Trial Branch card offers, play tracking, missed-combat removal, and visible enchantment progress from `UrdaRunHook.cs` into `UrdaBlessingService.TrialBranch.cs` without changing the `Progress` serialized field layout.
- Split shared Urda map helpers into `UrdaBlessingService.MapHelpers.cs` and progress serialization/mirror state into `UrdaBlessingService.State.cs`. State mirror guards now read the state file for Urda and the run hook only for hook registration.
- Split After Rain death prevention, Elite gold, and Act 2 compensation from `UrdaRunHook.cs` into `UrdaBlessingService.AfterRain.cs` without changing the `Progress` serialized field layout.
- Split Moss Map room rewards from `UrdaRunHook.cs` into `UrdaBlessingService.MossMap.cs` without changing room reward values or trigger conditions.
- Split Humus Pact reward-skip and completion flow from `UrdaRunHook.cs` into `UrdaBlessingService.HumusPact.cs` without changing the shared card-reward context or completion order.
- Split Molting starter-card removal and Withered Husk addition from `UrdaRunHook.cs` into `UrdaBlessingService.Molting.cs` without changing preview behavior.
- Split Urda card-reward tracking and alternative routing from `UrdaRunHook.cs` into `UrdaBlessingService.CardRewards.cs`, keeping the shared reward context available to Seedbed, Humus Pact, and Seed Bank.
- Split Root Sight setup and relic-status refresh from `UrdaRunHook.cs` into `UrdaBlessingService.RootSightStatus.cs`, keeping Root Sight selection/hover logic separate in `UrdaBlessingService.RootSight.cs`.
- Split run lifecycle handlers from `UrdaRunHook.cs` into `UrdaBlessingService.RunLifecycle.cs`, moved the shared Urda loc helper into `UrdaBlessingService.Localization.cs`, and moved the Rooted Route map marker into `UrdaRootedRouteMapQuestMarker.cs`. `UrdaRunHook.cs` now contains hook classes only.
- Split Root Sight hover-tip generation from `UrdaBlessingService.RootSight.cs` into `UrdaBlessingService.RootSightHover.cs`, keeping preview/title lookup behavior unchanged.
- Split Root Sight room-type/model routing from `UrdaBlessingService.RootSight.cs` into `UrdaBlessingService.RootSightRouting.cs`, keeping RunManager patch entry points unchanged.
- Split Root Sight preview record parsing and formatting from `UrdaBlessingService.RootSight.cs` into `UrdaBlessingService.RootSightPreviewStore.cs`, preserving the existing serialized `RootSightPreviewRecords` layout.
- Split Root Sight preview generation from `UrdaBlessingService.RootSight.cs` into `UrdaBlessingService.RootSightPreviewGeneration.cs`, preserving Unknown-room blacklists, encounter/event selection, and the ActModel room-set reflection.
- Moved the Root Sight map marker model into `UrdaRootSightMapQuestMarker.cs`, matching the existing Rooted Route marker split.
- Hardened Root Sight preview behavior against the Future Peek no-state-mutation rule: Unknown-room previews now fork `runState.Rng.UnknownMapPoint`, replay the room-type choice from current odds without mutating the live odds object, and mark seen coordinates per Act so the five Root Eyes can work across later acts. When a previewed Unknown room is actually entered, Root Sight commits the live Unknown RNG/odds state once for the stored result instead of leaving later Unknown-room odds stale. Event previews now call the same event hook used by vanilla and commit the event queue on room entry.
- Moved Root Sight entry-state commits out of the room-type lookup path and into the room-model creation path, after the stored preview model resolves. A transient run/act/coord key prevents duplicate Unknown RNG, Unknown odds, or event-queue commits if the room model hook is queried more than once.
- Hardened the Root Sight map-click path by patching `NClickableControl._GuiInput` for `NMapPoint` left-button releases while Root Sight selection is active. This avoids relying only on `NMapPoint.OnRelease`, which depends on vanilla map-point enabled/travel state.
- Split Shallow-Root Relic logic from `UrdaBlessingService.RouteRewards.cs` into `UrdaBlessingService.ShallowRootRelic.cs`, and moved route coordinate/reachability helpers into `UrdaBlessingService.RouteMapHelpers.cs`.
- Split Trial Branch offer flow from `UrdaBlessingService.TrialBranch.cs` into `UrdaBlessingService.TrialBranchOffer.cs`, and split Trial Branch display/enchantment refresh into `UrdaBlessingService.TrialBranchDisplay.cs`. `UrdaBlessingService.TrialBranch.cs` now holds combat tracking and settlement.
- Split Morvi Forbidden Loan, Red Ink Overdraft, and Debt Settlement flow from `MorviRunHook.cs` into `MorviBlessingService.DebtAndLoan.cs`. `MorviRunHook.cs` still owns shared hook flow, archive pages, Blueprint Proof, temporary-card cleanup, and progress serialization.
- Split Lotha run/combat hook wrapper classes from `LothaRunHook.cs` into `LothaHooks.cs`, and changed Lotha source guards to read the Lotha source tree. `LothaRunHook.cs` now contains the blessing service only. During this cut, `LothaPolishGuardTests.cs` was normalized back to valid UTF-8 and its broken mojibake sentinel strings were replaced with stable `\uFFFD` checks.
- Split Lotha selected-blessing state, deck mirror sync, Progress parsing/writing, transient combat state, and Death Reprieve hydration/resolution from `LothaRunHook.cs` into `LothaBlessingService.State.cs`, preserving the serialized Progress layout.
- Split Lotha Death Reprieve behavior from `LothaRunHook.cs` into `LothaBlessingService.DeathReprieve.cs`, keeping the hook entry points and card-play cost checks on the same partial service.
- Split Lotha Public Evidence Power hooks, Enlightenment turn-start payoff, and damage-debuff exclusion policy from `LothaRunHook.cs` into `LothaBlessingService.PublicEvidence.cs`.
- Split A17 Deep Branch map insertion, detection, route-safety checks, and branch metadata restore from `AscensionMapService.cs` into `AscensionMapService.DeepBranches.cs`. Ascension map guard tests now read the whole `Ascension/Map` source tree so later A11 or marker splits do not weaken assertions.
- Split A11 map geometry application, source-boundary diagnostics, route-row insertion, width-choice insertion, geometry proof conversion, and serializable-map bridge helpers from `AscensionMapService.cs` into `AscensionMapService.A11Geometry.cs`. This keeps A11 geometry separate from marker assignment and boss metadata without changing the public map hook entry points.
- Split A12 Firemark, A16 Banner, A19/A20 Boss marker assignment, stable marker ordering, quest-marker attachment, and map path-choice helpers from `AscensionMapService.cs` into `AscensionMapService.Markers.cs`. `AscensionMapService.cs` now contains only entry flow, metadata lookup, and applied-map side-table state.
- Updated Ascension combat guard tests to read the whole combat source tree, so future Banner/Firemark/Boss Seal partial splits do not require behavior assertions to stay in one oversized file.
- Moved package-directory and small transparent PNG checks from `ReleaseCoverageGuardTests.cs` into shared `TestRepo` helpers, and extended the infrastructure guard/readme so future tests reuse those helpers instead of copying local versions.
- Slimmed the active `ancients-rework-v4` and `ascension-11-20` work logs to current summaries and moved the long chronological histories into `docs/archive/feature-work-logs/`.

### Next Low-Risk Cuts

- Continue with larger test-data/source-inventory extraction only after a safer mechanical split is prepared; then consider Humus/Molting/Moss/After Rain partials only if another source issue justifies the extra movement.

### Validation So Far

- `dotnet build EZMicroBalance.sln`: passed, 0 warnings, 0 errors.
- Focused doc guard filter before the code split: 42 passed / 6 skipped.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 175 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed.
- After the Banner partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Firemark partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Boss Seal partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the finer Boss Seal effect split, the same build, normal test, format, and diff-check sequence passed again.
- After the Root Sight partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Seed Bank partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Seedbed partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Trial Branch partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Route Rewards partial split, the same build, normal test, format, and diff-check sequence passed again.
- After the Urda state/map-helper split, the same build, normal test, format, and diff-check sequence passed again.
- After the first shared test-helper extraction, the same build, normal test, format, and diff-check sequence passed again.
- After the After Rain partial split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Moss Map, Humus Pact, and Molting partial splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the CardReward, Root Sight status, and run lifecycle splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Root Sight hover and room-routing splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Root Sight preview-store, preview-generation, and marker splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Root Sight no-state-mutation, Unknown-room commit, event-queue, disabled-map-click, one-shot entry commit, and `RunState`-scoped transient-state fixes, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Shallow-Root Relic and route-map helper splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Trial Branch offer/display splits, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Morvi debt/loan split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the work-log slimming pass, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Lotha hook split and Lotha test UTF-8 cleanup, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Lotha state split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Lotha Death Reprieve behavior split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the Lotha Public Evidence split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the A17 Deep Branch map-service split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the A11 map geometry split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- After the map marker assignment split, the same build, normal test, format, and diff-check sequence passed again. `git diff --check` still reports only the existing CRLF/LF warning for `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`.
- Publish, package refresh, artifact tests, live game, save-load, death/failure path, and co-op were not run in this cleanup pass.

## Root Sight Map Restore - 2026-05-18

Scope: source-only repair for the Root Eyes map-preview path after reviewing `future_peek_dev_the_spire_research_plan.md`. The game was not opened.

Repair applied:
- `UrdaRunHook` now forwards `AfterMapGenerated` to the Urda service.
- `RootSightPreviewRecords` are replayed during map generation/load to reattach `UrdaRootSightMapQuestMarker` to saved Monster, Unknown, or Elite preview nodes.
- The restore path ignores malformed coordinates, missing nodes, and point-type mismatches, so stale saved preview text cannot mark the wrong room after a map rebuild.
- The existing Future Peek rule is preserved: Unknown-room preview still uses a forked `UnknownMapPoint` RNG and does not call the live `UnknownMapPoint.Roll` path while choosing a preview.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed after the later package refresh. Live game, save-load, death/failure path, and co-op remain pending.

## Root Sight Encounter Peek Hardening - 2026-05-18

Scope: source-only follow-up after the user reported that level preview was still not implemented according to `future_peek_dev_the_spire_research_plan.md`. The game was not opened.

Repair applied:
- Root Eyes still stays inside `Spire Plus` / `EZMicroBalance`; the separate experimental `EZFuturePeek` mod from the research plan was not added to this private-beta package.
- Normal and Elite room previews now choose from the already-generated Act `RoomSet` with a local deterministic preview RNG keyed by Act and map coordinate.
- Root Eyes preview generation no longer calls `Act.PullNextEncounter(...)` while choosing a preview, matching the Future Peek rule that preview code should not use APIs that sound like state advancement.
- `AncientHighRiskSourceGuardTests` now requires the read-only encounter peek path and rejects `PullNextEncounter` / `PullNextEvent` inside `UrdaBlessingService.RootSightPreviewGeneration.cs`.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, focused Root Sight/high-risk/player-facing/release guard tests (23 passed), full `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live map hover/click, save-load, death/failure path, and co-op remain pending.

## Root Eyes Future Queue Commit And Text Cleanup - 2026-05-18

Scope: strict no-game review of the Root Eyes future-room preview path and active player-facing text guards.

Findings and repair:
- Normal/elite Root Eyes previews locked a concrete encounter, but entering the marked room did not connect that encounter to the Core encounter queue. The fix keeps preview read-only, then swaps the selected encounter into the current `RoomSet` slot only when the marked room is entered, so Core's existing room-visited flow consumes that encounter normally.
- Trial Branch's inline Simplified Chinese `CardModifierLoc` still contained mojibake. It now uses readable `试炼枝条` text.
- `docs/features/ancients-rework-v4/manual-verification-matrix.md` still had mojibake in the Ancient reward and zhs spot-check rows. The active matrix now uses readable player terms such as `迅捷`, `放松`, `神化`, `愚行`, `许愿`, `债务`, `首领`, `保留`, `虚无`, `消耗`, `固有`, `永恒`, and `力量`.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed, full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped, `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed, `git diff --check` passed with CRLF/LF warnings only, `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts, `scripts/package-spire-plus.ps1` rebuilt the local test zip, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`; current DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live map click/hover, save-load, and co-op proof remain pending.

## Root Sight Hover Clarity - 2026-05-18

Scope: player-facing Root Eyes clarification after the user noted the preview result was not visible on mouse hover. The game was not opened.

Repair applied:
- Root Eyes option and relic text now explicitly tells the player to hover the marked room to see the concrete result.
- Source guard coverage now requires Root Sight hover text to resolve exact `EventModel.Title` and `EncounterModel.Title` through `ModelDb`, so the visible hover path cannot silently fall back to generic room text.
- Localization guard coverage now requires the English and Simplified Chinese Root Eyes text to mention the hover action.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, focused Root Sight/high-risk/player-facing/release guard tests (46 passed / 3 skipped), full `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed PCK hash is `190D697498B6B84293D3469662D33134B7F8DE7A095436B7521E012C2E4C54EC`. Live map hover/click, save-load, death/failure path, and co-op remain pending.

## Lotha Presumption And Closed Court Split - 2026-05-18

Scope: no-game refactor slice to reduce `LothaRunHook.cs` without changing player-visible Lotha behavior.

Refactor applied:
- Moved Presumption/Innocent constants, combat-start Power application, turn-start draw/Energy/Block, break-on-unblocked-enemy-attack handling, and damage classifier into `LothaBlessingService.Presumption.cs`.
- Moved Closed Court first-turn full-hand draw, Energy grant, discount arming, post-combat card reward suppression, cost discount helper, and discount-use tracking into `LothaBlessingService.ClosedCourt.cs`.
- Updated Lotha guard tests to read the source tree or the owning partial file instead of relying on unrelated helper order in `LothaRunHook.cs`.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, focused Lotha/source guard tests (13 passed), full `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live gameplay, save-load, death/failure path, and co-op proof remain pending.

## Morvi Open-Book And Paperstorm Split - 2026-05-18

Scope: no-game refactor slice to reduce `MorviRunHook.cs` while preserving Morvi v2.2 behavior.

Refactor applied:
- Moved Open-Book Exam turn-1 draw/Energy, end-turn seal, turn-3 return, sealed-card marker restore, and marker cleanup helpers into `MorviBlessingService.OpenBook.cs`.
- Moved Paperstorm combat-start Waste Paper shuffling, per-turn counter refresh, Status-card conversion, and visible counter refresh into `MorviBlessingService.Paperstorm.cs`.
- Updated active-source guard coverage so both new Morvi partial files are tracked by the manifest.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, focused Morvi/source guard tests (8 passed), full `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live gameplay, save-load, and co-op proof remain pending.

## Morvi Misprint And Temporary Cleanup Split - 2026-05-18

Scope: no-game refactor slice to finish reducing `MorviRunHook.cs` after the Open-Book/Paperstorm split.

Refactor applied:
- Moved Misprint Press play-count replay, autoplay recursion guard, cost-threshold draw marker, and post-play draw settlement into `MorviBlessingService.MisprintPress.cs`.
- Moved Morvi generated/temporary combat-card id list and combat-end cleanup into `MorviBlessingService.TemporaryCards.cs`.
- Updated active-source guard coverage so both new Morvi partial files are tracked by the manifest.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, focused Morvi/source guard tests (8 passed), full `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. `git diff --check` reported CRLF/LF warnings only. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live gameplay, save-load, and co-op proof remain pending.

## Morvi Blueprint And Library Split - 2026-05-18

Scope: behavior-preserving source cleanup for Morvi's remaining high-risk combat effects. The game was not opened.

Refactor applied:

- Moved Blueprint Proof constants, initialization guard, temporary-upgrade/draw/block settlement, and eligibility helper from `MorviRunHook.cs` into `MorviBlessingService.BlueprintProof.cs`.
- Moved Overdue Library archive-page type list, 3-page combat-start generation, discount arming, and discount consumption from `MorviRunHook.cs` into `MorviBlessingService.OverdueLibrary.cs`.
- Kept the existing `MorviCombatState` fields, selected-blessing state, hook entry points, progress serialization, generated-card cleanup, and player-visible behavior unchanged.

Validation result: `dotnet build EZMicroBalance.sln --no-restore`, `dotnet test EZMicroBalance.sln --no-build` (177 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, and `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` (195 passed / 0 skipped) passed. This slice is included in the current package; latest zip hash is `952E41713B9F2D87DB2D57FDF5EBFCE21108606019EA087CF0190E03FE09057B`, and latest installed DLL hash is `6B27470D339561BDC584BB0DA53C6BB20129CF6B3A4D08F2558A178077020542`. Live gameplay, save-load, and co-op proof remain pending.

## Lotha Echo And Verdict Split - 2026-05-18

Scope: behavior-preserving source cleanup for Lotha's remaining high-density combat effects. The game was not opened.

Refactor applied:

- Moved Mirror Hall Echo extra-play count, Power replacement fallback, previous-turn type recording, and recordable-card-type helper from `LothaRunHook.cs` into `LothaBlessingService.MirrorHallEcho.cs`.
- Moved Deferred Verdict turn-4 constants, Verdict consumption, Power replacement fallback, and Verdict-stack helper from `LothaRunHook.cs` into `LothaBlessingService.DeferredVerdict.cs`.
- Kept hook entry points, transient combat state, progress serialization, Power replacement helper shape, Single Sentence tracking, Presumption, Closed Court, and player-visible behavior unchanged.

Validation result so far: `dotnet build EZMicroBalance.sln --no-restore` passed, and focused Lotha/save-risk/active-source guard tests passed with 20 tests. Full validation, publish, package refresh, artifact tests, live gameplay, save-load, and co-op proof remain pending for this slice.

## Strict Review Risk Fixes - 2026-05-18

Scope: no-game strict review of high-risk Ancient/Vakuu paths after the user reported confusing Root Eyes behavior, Seed Bank risk, Forbidden Loan edge cases, and Vakuu black-screen risk. The game was not opened.

Findings and repair:
- Root Eyes preview entry now revalidates the stored preview when the marked room is entered. Event previews must still exist, be allowed for the current run, and not already be visited; encounter previews must still resolve to an `EncounterModel`.
- Seed Bank relic extraction now has a per-player re-entry guard, so repeated relic clicks cannot open overlapping selection flows against the same stored cards.
- Morvi Forbidden Loan no longer commits the blessing if the Ancient card pool has no selectable card. The option is filtered out when no candidates exist, and selection failure clears transient state.
- Vakuu Stolen Vault now applies to the initial custom monster via `AfterAddedToRoom`, not only to creatures added after combat hooks start.
- Vakuu no-reward victory no longer silently returns on malformed/pre-finished state. It logs the fallback and either resumes the parent reward screen or reopens the map instead of leaving the run stuck on a finished combat.
- Inline Simplified Chinese PowerLoc text for Firemark and Banner powers was restored to readable player-facing text.

Subagent review findings addressed: Root Eyes stale event entry, Seed Bank click re-entry, Forbidden Loan empty reward pool, Vakuu no-reward black-screen fallback, and Vakuu initial Stolen Vault visibility. Active-fight save/load and live UI/gameplay remain manual proof gates, not closed release evidence.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ancient/Vakuu/Morvi/player-facing/release guard tests passed with 65 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash is `9F0C09FDF1F498567CCDA891D6A7AE2527885E2AD5281B53DF3123E894656E48`; current DLL hash is `20749CC0C0763BBA00DB4521719556D3C17AB039C94BE30F9DF560C2A2A8D243`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## Vakuu Fight Service Split - 2026-05-18

Scope: low-risk source cleanup after the strict review fixes. The game was not opened.

Refactor applied:
- Made `VakuuFightService` partial.
- Moved victory event-state restoration, no-normal-reward resume, malformed victory fallback, source Ancient reward choice lookup, and Lotha fallback reward choices from `VakuuFightPatch.cs` into `VakuuFightVictory.cs`.
- Kept force/option/resume/save patches, fight start, Stolen Vault power maintenance, damage-lock breaking, contract signing, pre-finished parent save restore, and asset paths in `VakuuFightPatch.cs`.
- Updated active-source and Vakuu guard tests so the new file is tracked and the behavior assertions follow the new file boundary.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Vakuu/UI/player-facing/source-manifest guard tests passed with 41 passed; full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash was `F062EB5CDBA92C12D76C05E639A8B0F7B6404B4010CD5A0568F5A94CAC5C2F59`; current DLL hash was `D8BA52CDF946287C9FFA813727C8640829C00338AD224642C27D28759CD693FC`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## RootBud Combat Hook Helper Split - 2026-05-18

Scope: low-risk source cleanup for the Rootblight/Blight Sprout combat hook. The game was not opened.

Refactor applied:
- Made `RootBudCombatHook` partial.
- Moved current combat-state lookup, room eligibility, RootBud count/round helpers, tracker lookup, pile scans, sprout eligibility, entered-hand marking, and combat-end Rootblight resolution from `RootBudCombatHook.cs` into `RootBudCombatHook.Helpers.cs`.
- Kept combat hook entry flow, per-room seeding, draw/pile/play/death hooks, and Rootblight/Blight Sprout behavior unchanged.
- Updated active-source coverage and RootBud source guards so behavior assertions read both partial files.
- Subagent review found the initial guard was too loose after the split. The guard now separately checks the main hook file and helper file, forbids combat overrides in the helper, and slices key entrypoint method bodies to prove seeding, draw, and combat-end calls remain in the intended hooks.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension RootBud/source-manifest guard tests passed with 21 passed / 1 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash was `7CC56CE1E432BC4E4FA523566AED729ED1232207B4885F76F541A822815FAA20`; current DLL hash was `4BD0058BEA3E3EB785F67DE3DA38B2E7D63FD29C3010998C6053AEAD78BC381F`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## A11 Serializable Map Helper Split - 2026-05-18

Scope: behavior-preserving source cleanup for A11 map geometry. The game was not opened.

Refactor applied:
- Moved serializable-map lookup, geometry graph conversion, path traversal, coordinate shifting, bridge-point creation, and child-edge helpers from `AscensionMapService.A11Geometry.cs` into `AscensionMapService.A11SerializableHelpers.cs`.
- Kept A11 map boundary application, target row/column checks, route-row insertion, width-choice insertion, and diagnostic logging in `AscensionMapService.A11Geometry.cs`.
- Added the new helper file to active source coverage. `AscensionMapService.A11Geometry.cs` is now 331 lines, and `AscensionMapService.A11SerializableHelpers.cs` is 230 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused A11/Ascension/source-manifest guard tests passed with 24 passed / 1 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash was `8D22B85BA8FAD4C44218C3DA51A12AE017A596178B17C1F657BCC68E6E435CD7`; current DLL hash was `5A97D0A1B6F811F890E538282B9A2FD341E2BD3B68F36A6F267A1EA07BD11F4F`. Live route-click, save-load, clicked UI, death/failure path, and co-op proof remain pending.

## Ascension Map Marker Helper Split - 2026-05-18

Scope: behavior-preserving source cleanup for Ascension map marker assignment. The game was not opened.

Refactor applied:
- Moved metadata creation, quest-marker attachment, stable marker ordering, stable marker hashing, Firemark optional-route selection, route adjacency checks, rest-site row helpers, and path-avoidance traversal from `AscensionMapService.Markers.cs` into `AscensionMapService.MarkerHelpers.cs`.
- Kept A12 Firemarked Elite, A16 Banner Room, A19 Boss Seal, A20 King Brand assignment, marker logging, and diagnostics summary in `AscensionMapService.Markers.cs`.
- Added the new helper file to active source coverage. `AscensionMapService.Markers.cs` is now 197 lines, and `AscensionMapService.MarkerHelpers.cs` is 264 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/source-manifest/release-safety guard tests passed with 36 passed / 7 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 177 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 195 passed / 0 skipped. Current zip hash is `7450B97AD93C56A411EE916BE0113AC83126B2605C42F7F87688E716EF6951B8`; current DLL hash is `FF5AEA96C5B3049E095A8ADC352F8B98E5A137DAD65AD6CB1045FD8EC147DAD3`. Live route-click, save-load, clicked UI, death/failure path, and co-op proof remain pending.

## Strict Subagent Review Closure - 2026-05-18

Scope: follow-up to the Dirac and Faraday source reviews. The game was not opened.

Findings and repair:
- Rootblight played-card downgrades now persist through a save-backed player field, so the pending combat-end downgrade queue no longer disappears if a run is saved mid-combat.
- RootBud boss timing is deterministic again: existing boss Blight Sprouts are normalized by scan order to round 3 then round 4.
- Rootblight III split semantics are explicit: the hidden split marker is consumed only when a Rootblight I is actually added. A four-card cap failure keeps the marker available.
- Prismatic Gem now replaces reward slots between the early and late reward-modifier loops. Early added slots are included, late modifiers apply to the final cards, and replacement first tries to preserve type plus rarity.
- Active docs and current-facing text notes were scrubbed for the mojibake fragments found by review, and a non-archive active-doc mojibake guard was added.
- Withered Husk remains aligned with v3.2 as a playable 0-cost temporary Skill that grants 3 Block, exhausts, and removes after combat.

Subagent review result: Dirac found no P0 and raised RootBud/Rootblight state risks; Faraday found no P0 and raised Prismatic hook-order plus active-doc mojibake risks. The actionable static findings from both reviews are addressed. Remaining proof gates are live gameplay, clicked UI, save-load, death/failure path, and co-op.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/Ancient/player-facing/release guard tests passed with 72 passed / 9 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 178 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 196 passed / 0 skipped. Current zip hash is `8FCDFEA0618A97CCACBEC236F1CF8E25683C43CAC28AED0D8709ED13B5914644`; current DLL hash is `6CDFCF16F9CAF2AC8EC53B2362422BC2E4ED05668EB61374AE384FFED9A5402E`; current PCK hash is `9A45F929C093DDA621CE9093919BF8B0CC61963AD8FE67DD8C9151EF5006B8B8`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## Prismatic Gem Hint Split - 2026-05-18

Scope: behavior-preserving source cleanup after the Prismatic hook-order repair. The game was not opened.

Refactor applied:
- Moved reward-screen banner hint logic from `PrismaticGemPatches.cs` into `PrismaticGemRewardScreenHintPatch.cs`.
- Kept pool/counter/reward replacement, off-color selection, failed-replacement cleanup, counter restoration, and relic hover count in `PrismaticGemPatches.cs`.
- Updated Ancient and release guard tests so Prismatic source coverage follows the patch source tree and the new hint file boundary.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ancient/Prismatic/release guard tests passed with 59 passed / 11 skipped; the later full validation chain passed with `dotnet test EZMicroBalance.sln --no-build` (178 passed / 18 skipped), `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`, `git diff --check`, `dotnet publish EZMicroBalance.sln --no-restore`, `scripts/package-spire-plus.ps1`, opt-in artifact tests (196 passed / 0 skipped), and `scripts/check-installed-ezmb-package.ps1`. Live reward-screen visual proof remains pending.

## Root Sight RNG Source Audit - 2026-05-18

Scope: strict source follow-up for Root Eyes / future-room preview after comparing the implementation against local Core `UnknownMapPointOdds.Roll`. The game was not opened.

Finding and repair:
- Core stores Unknown-room event probability as the remaining probability after Monster/Elite/Treasure/Shop odds. Root Eyes already mirrors that shape by defaulting to Event and only overriding when a non-event odds roll hits.
- The preview RNG fork was still seeded from `runState.Rng.Niche`, while the docs and Core semantics point to the Unknown-room RNG stream. The fork source is now `runState.Rng.UnknownMapPoint`. This keeps preview creation read-only, but ties the deterministic preview dice to the same RNG stream that owns Unknown-room outcomes.
- `AncientHighRiskSourceGuardTests` now requires the Root Sight preview generator to reference `runState.Rng.UnknownMapPoint`, while continuing to reject direct `UnknownMapPoint.Roll`, `PullNextEncounter`, and `PullNextEvent` calls during preview creation.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Root Sight/player-facing/release guard tests passed; full `dotnet test EZMicroBalance.sln --no-build` passed with 178 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` refreshed installed artifacts; `scripts/package-spire-plus.ps1` rebuilt the local test zip; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 196 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` confirmed installed DLL/manifest/PCK hashes match the handoff. Current zip hash is `8FCDFEA0618A97CCACBEC236F1CF8E25683C43CAC28AED0D8709ED13B5914644`; current DLL hash is `6CDFCF16F9CAF2AC8EC53B2362422BC2E4ED05668EB61374AE384FFED9A5402E`. Live map click/hover, save-load, death/failure path, and co-op proof remain pending.

## Ascension V3.2 Source Design Sync - 2026-05-18

Scope: active documentation follow-up while auditing A12 Firemark and A16 Banner implementation against source constants. The game was not opened.

Finding and repair:
- `docs/features/ascension-11-20/source-design.md` still carried earlier prototype Firemark and Banner Room values and listed only the older three-banner set.
- The active source design now matches the current v3.2 implementation: Firemark Might +1/+2/+4, Giant +20%/+30%/+45%, Forge Armor 5/10/20, Constant Heal 4/8/16 with 12/24/48 interrupt thresholds; Banner Rooms list Vanguard, Shieldwall, Blood Prize, Pressing Line, and Last Stand with their current Act-scaled values and single-enemy conversion note.

Validation result: focused Ascension/release/player-facing guard tests passed; full `dotnet test EZMicroBalance.sln --no-build` passed with 178 passed / 18 skipped; opt-in artifact tests passed with 196 passed / 0 skipped; `git diff --check` passed with CRLF/LF warnings only. Live Firemark/Banner combat timing remains pending.

## Strict Source Audit Follow-Up - 2026-05-18

Scope: follow-up to the latest Dirac/Faraday strict source reviews. The game was not opened.

Findings and repair:
- Rooted Route used zero-based row checks in a player-facing floor rule. It now limits routed targets to visible floor 7.
- Blight Sprout timing ran after normal hand draw. Due sprouts now move to the top of the draw pile in `BeforeHandDraw`, so the intended turn's opening draw can see them.
- Vakuu victory Ancient reward choices now assign the relic owner before building `EventOption.FromRelic`, matching other visible relic option paths.
- Prismatic Gem and Fiddle direct hooks now ignore melted relic instances, so expired option relics do not keep applying hidden effects.
- Lotha Closed Court now consumes a discount use only for a card that actually received and played a Closed Court discount.
- Lotha Single Sentence and Morvi Paperstorm now hydrate transient weak-table state from visible Powers when source state has been lost during a mid-combat reload.
- Firemark and Banner inline Simplified Chinese power text is readable and aligned with the v3.2 player-facing terms.

Residual risks:
- Ascension combat tracker, A12/A14-A20 combat markers, Rooted Route, Root Eyes, and shared reward state still need live save-load and co-op evidence.
- This pass proves source/package consistency only. It is test-ready material, not a release-ready claim.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `9A57756A2EB9A9911C72F440F531E97B073E12BBBBAB966AC7D289BA7A91AAC1`; current DLL hash is `642186C336B09091287FD948745557B4EE194502EC3F4ACE1CF04A938FFD428D`; current PCK hash is `9A45F929C093DDA621CE9093919BF8B0CC61963AD8FE67DD8C9151EF5006B8B8`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## RootDeck Notice Split - 2026-05-18

Scope: behavior-preserving cleanup for the A14/A15/A18 Rootblight deck service. The game was not opened.

Refactor applied:
- `RootDeckService.cs` now keeps root-family deck state, growth, pending downgrade serialization, card creation, and cap trimming.
- `RootDeckService.Notices.cs` owns the local Rootblight notice UI paths: creature thought bubble, event-room VFX bubble, and run overlay bubble.
- This removes direct Godot node/VFX dependencies from the core deck-state service. The core file dropped from 575 lines to 441 lines; the new notice partial is 141 lines.
- Ascension source guards now read both RootDeckService partial files, and active source coverage includes the new file.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage guard tests passed with 43 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `9A57756A2EB9A9911C72F440F531E97B073E12BBBBAB966AC7D289BA7A91AAC1`; current DLL hash is `642186C336B09091287FD948745557B4EE194502EC3F4ACE1CF04A938FFD428D`.

## Lotha Combat Lifecycle Split - 2026-05-18

Scope: behavior-preserving cleanup for Lotha's remaining oversized shared service. The game was not opened.

Refactor applied:
- Moved Lotha combat lifecycle entry flow from `LothaRunHook.cs` into `LothaBlessingService.CombatLifecycle.cs`: combat-start setup, player-turn start setup, player-turn end cleanup, and combat-end cleanup.
- At the time, the remaining card-play/cost/reset helpers stayed in `LothaRunHook.cs`. This historical state is superseded by `Lotha Card Rules Split - 2026-05-18`, which removed the old file and moved those helpers into focused partials.
- Added the new lifecycle partial to active source coverage. Historical line-count note: `LothaRunHook.cs` dropped from 502 lines to 330 lines before it was later removed.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Lotha/Vakuu save-risk/ReleaseCoverage guard tests passed with 42 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C`; current DLL hash is `3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10`. Live gameplay, clicked Ancient UI, save-load, death/failure path, and co-op proof remain pending.

## RootDeck Deck-Card Split - 2026-05-18

Scope: behavior-preserving cleanup for the A14/A15/A18 Rootblight deck service after the notice split. The game was not opened.

Refactor applied:
- Moved Rootblight deck-card lookup, internal removal marker table, add/replace, card creation, deck enumeration, and four-card cap trimming from `RootDeckService.cs` into `RootDeckService.DeckCards.cs`.
- Kept public Rootblight flow, combat-end growth, pending downgrade serialization, and diagnostic level state in `RootDeckService.cs`.
- Updated Ascension source guards to read the rewards source tree, so RootDeck behavior assertions follow the partial files instead of pinning behavior to one file. Active source coverage includes the new deck-card file.
- `RootDeckService.cs` dropped from 441 lines to 332 lines; the new deck-card partial is 115 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage guard tests passed with 43 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C`; current DLL hash is `3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10`. Live Rootblight gameplay, save-load, clicked UI, death/failure path, and co-op proof remain pending.

## RootDeck Pending-Downgrade Split - 2026-05-18

Scope: behavior-preserving cleanup for Rootblight combat-end downgrade persistence after the deck-card split. The game was not opened.

Refactor applied:
- Moved pending combat downgrade queue serialization, parsing, clearing, and the queue record into `RootDeckService.PendingDowngrades.cs`.
- Kept public Rootblight entry points, combat-end growth, and diagnostic level state in `RootDeckService.cs`.
- Active source coverage includes the new pending-downgrade file. Existing Ascension source guards read the rewards source tree, so behavior assertions still cover the moved queue helpers.
- `RootDeckService.cs` dropped from 332 lines to 283 lines; the new pending-downgrade partial is 55 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage guard tests passed with 43 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C`; current DLL hash is `3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10`. Live Rootblight gameplay, save-load, clicked UI, death/failure path, and co-op proof remain pending.

## Banner Pressing Line Split - 2026-05-18

Scope: behavior-preserving cleanup for the A16 Banner Room implementation. The game was not opened.

Refactor applied:
- Moved Pressing Line card-count tracking, per-player layer cap constants, turn-end Block settlement, and strike-power application into `AscensionCombatModifierService.Banners.PressingLine.cs`.
- Kept Vanguard, Shieldwall, Blood Prize, Last Stand, target selection, single-enemy banner fallback, and shared Banner entry points in `AscensionCombatModifierService.Banners.cs`.
- Active source coverage includes the new Pressing Line file. Existing Ascension guard tests read the combat source tree, so the v3.2 Pressing Line assertions still follow the moved methods.
- `AscensionCombatModifierService.Banners.cs` dropped from 389 lines to 308 lines; the new Pressing Line partial is 87 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage guard tests passed with 43 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C`; current DLL hash is `3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10`. Live Banner combat timing, save-load, route traversal, and co-op proof remain pending.

## Vakuu Combat-State Split - 2026-05-18

Scope: behavior-preserving cleanup for the gated Vakuu fight service. The game was not opened.

Refactor applied:
- Moved Stolen Vault power synchronization, damage-threshold lock breaking, contract signing, Blood Debt application, and Vakuu creature lookup into `VakuuFightService.CombatState.cs`.
- Kept Harmony patches, fight option creation, fight start, parent event node clearing, pre-finished parent save/restore guards, and asset paths in `VakuuFightPatch.cs`.
- Victory/no-reward return flow remains in `VakuuFightVictory.cs`.
- Vakuu guard tests now read the Vakuu source tree for behavior that can move between partial files, while `StartFight` safety checks still inspect the patch file's room-transition slice.
- `VakuuFightPatch.cs` dropped from 394 lines to 251 lines; the new combat-state partial is 148 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Vakuu/UI/ReleaseCoverage guard tests passed with 47 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F9077D752D38C91566C84E9CB78D642E98366AB2ADFAE33C7C05582DFE2B773C`; current DLL hash is `3A70790EE2978FC7FC0BA1C8BEC3CC83F4AE05AA8FB44B2E5325EA624EFC3E10`. Live Vakuu victory return, no-black-screen, save-load, death/failure path, clicked UI, and co-op proof remain pending.

## Prismatic Gem Hover Split - 2026-05-18

Scope: behavior-preserving cleanup for Prismatic Gem after the previous reward-screen banner split. The game was not opened.

Refactor applied:
- Moved Prismatic Gem relic hover count creation and the two `RelicModel` hover patches into `PrismaticGemHoverPatches.cs`.
- Kept normal reward counting, reroll-safe all-slot off-color replacement, failed-replacement cleanup, and reward-screen marker state in `PrismaticGemPatches.cs`.
- Kept reward-screen banner text, guarded `_banner` reflection, `UI/Banner` fallback, and diagnostics in `PrismaticGemRewardScreenHintPatch.cs`.
- Active source coverage now includes the new hover file. `PrismaticGemPatches.cs` dropped from 371 lines to 322 lines; the new hover file is 53 lines.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Prismatic/release guard tests passed with 45 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `084976FDD7338D9939E9D1BF358195E536E89441123D462E00EF609E324015F2`; current DLL hash is `2A0B5FE6BD194B7BDECD4CB095DCED2166D509FDB7CEF1098650B1F804EA3FDC`. Live reward-screen visual proof and relic-hover visual proof remain pending.

## Root Sight Event Queue Fix - 2026-05-18

Scope: strict source audit follow-up after comparing Root Eyes event preview entry against Core `ActModel.PullNextEvent`, `RoomSet.MarkVisited`, and `RunManager.CreateRoom`. The game was not opened.

Finding and repair:
- Root Eyes could preview a later allowed event and pass that `EventModel` directly into `RunManager.CreateRoom`. The previous commit path only called `RoomSet.EnsureNextEventIsValid`, then let Core mark one event visited. That showed the selected event, but the event queue pointer did not move to the selected event first.
- Root Eyes event entry now mirrors the existing encounter entry rule. Before room creation completes, it calls `CommitRootSightEventQueueForEntry(runState, eventModel)`, validates the next legal event, swaps the selected event into `rooms.eventsVisited % rooms.events.Count`, and then lets Core `MarkRoomVisited(RoomType.Event)` advance the queue normally.
- Source guards now require the event queue swap and the event-specific commit call.

Independent read-only review result: no blocker. The reviewer confirmed that `RunManager.CreateRoom` skips `Act.PullNextEvent` when Root Sight supplies an `EventModel`, so `runState.AddVisitedEvent(eventModel)` is the correct replacement for vanilla's visited-event write. The reviewer also confirmed that `AddVisitedEvent` does not advance `eventsVisited`; Core still advances the event queue once through `MarkRoomVisited(RoomType.Event)`.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Root Sight/high-risk/release/player-facing guard tests passed with 55 passed / 9 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 179 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` passed; `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 197 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F797FA55FC623E6C55FACD109CC29831E633F9E42446081C1D2B22052A003464`; current DLL hash is `9903F9F8C196BEA61E3955F9093C5B1FDF9C19C2B6D369507DE57D39762477E2`. Live Root Eyes hover/click/save-load proof remains pending.

## Strict Hook And Text Audit Follow-Up - 2026-05-18

Scope: targeted strict audit after the Root Sight event queue fix and hook ownership review. The game was not opened.

Findings and repair:
- Root Eyes now accepts a repeated event at entry only when Core would also allow repeated events because every unique allowed event in the active room set has already been exhausted. This closes the gap between preview generation and room entry validation.
- Banner, Firemark, and Boss Seal inline Simplified Chinese `PowerLoc` strings were repaired in source. These hover tooltips now use readable v3.2 player terms instead of mojibake.
- Morvi, Lotha, and Urda RunHook classes no longer duplicate combat-only hook overrides that Core dispatches through `combatState.IterateHookListeners()`. CombatHooks own card play, turn, cost, draw, and Power-combat paths; RunHooks keep run lifecycle, reward, damage, death-prevention, and cleanup paths.
- A guard now locks that ownership split so future refactors do not reintroduce silent duplicate hook entry points.

Independent read-only review result: no blocker. The reviewer confirmed the duplicated RunHook overrides were dead ownership noise, not an active double-trigger. Remaining risks are live-only sequencing checks for Morvi Blueprint/Overdue/Misprint, Lotha lethal/save-load timing, Root Eyes hover/click/save-load, and Vakuu victory/save-load paths.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Root Sight/player-facing/Morvi/Lotha/Ascension high-risk guard tests passed with 57 passed / 0 failed; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only; `dotnet publish EZMicroBalance.sln --no-restore` and `scripts/package-spire-plus.ps1` rebuilt the local package; `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passed with 198 passed / 0 skipped; `scripts/check-installed-ezmb-package.ps1` passed for installed DLL, manifest, and PCK. Current zip hash is `F797FA55FC623E6C55FACD109CC29831E633F9E42446081C1D2B22052A003464`; current DLL hash is `9903F9F8C196BEA61E3955F9093C5B1FDF9C19C2B6D369507DE57D39762477E2`. Live gameplay, Root Eyes hover/click/save-load, clicked Ancient UI, death/failure path, and co-op proof remain pending.

## Ancients v4 Source Design Slim - 2026-05-18

Scope: documentation cleanup after the active `docs/features/ancients-rework-v4/source-design.md` was found to be both oversized and mojibake-corrupted. The game was not opened.

Cleanup applied:
- Archived the corrupted long draft to `docs/archive/feature-inputs/ancients-rework-v4/source-design-mojibake-pre-slim-20260518.md`.
- Replaced the active source-design file with a compact readable v4.3 summary covering Velvet Choker, Distinguished Cape, Prismatic Gem, major Ancient reward rows, superseded v4.2 rules, implementation evidence, and manual-proof boundaries.
- Updated `docs/features/ancients-rework-v4/README.md` so future agents do not treat the archived corrupted draft as current source truth.

Validation result: active non-archive Markdown UTF-8 scan passed; targeted source-design mojibake scan passed; active `source-design.md` is 29 lines; `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt because this slice changed docs/tests only.

## Waiting Tests Queue Slim - 2026-05-18

Scope: documentation cleanup for the largest active manual-evidence document. The game was not opened.

Cleanup applied:
- Archived the full historical queue to `docs/archive/issues/waiting-tests-pre-slim-20260518.md`.
- Replaced `docs/issues/waiting-tests.md` with a compact manual verification table that preserves every open row as an actionable evidence target.
- Updated the docs index and project map to describe the active file as a compact support queue.
- Added a guard so `waiting-tests.md` stays compact and keeps its archive pointer.

Validation result: active non-archive Markdown UTF-8 scan passed; targeted waiting-tests mojibake scan passed; active `docs/issues/waiting-tests.md` is 37 lines; `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt because this slice changed docs/tests only.

## Root Sight Unknown Split - 2026-05-18

Scope: behavior-preserving cleanup for Root Eyes future-room preview generation. The game was not opened.

Refactor applied:
- Moved Unknown-room blacklist, preview roll, Unknown odds reads, and one-shot live Unknown RNG/odds commit helpers from `UrdaBlessingService.RootSightPreviewGeneration.cs` into `UrdaBlessingService.RootSightUnknown.cs`.
- Kept concrete encounter and event preview selection in `UrdaBlessingService.RootSightPreviewGeneration.cs`.
- Updated high-risk Root Sight guards to assert across both preview-generation and Unknown-room partials, so future movement does not weaken the no-live-roll rule. The guard now rejects any `.Roll(` call in the combined Root Sight preview source, not only the direct `UnknownMapPoint.Roll` shape.
- Added `UrdaBlessingService.RootSightUnknown.cs` to the active source manifest.

Independent read-only review result: no blocker. The reviewer confirmed the split preserves the current call path, the new partial is included by the project glob, the active source manifest covers the file, and the source guard still blocks live Unknown-room roll / encounter / event pull calls during preview generation. The suggested unused using cleanup and wider `.Roll(` guard were applied.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Root Sight/ReleaseCoverage guards passed with 33 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice. Live Root Eyes hover/click/save-load proof remains pending.

## A11 Geometry Diagnostics Split - 2026-05-18

Scope: behavior-preserving cleanup for A11 map geometry. The game was not opened.

Refactor applied:
- Moved A11 target column/row helpers, extra-row lookup, geometry evidence lookup, source-boundary logging, and proof wrappers from `AscensionMapService.A11Geometry.cs` into `AscensionMapService.A11GeometryDiagnostics.cs`.
- Kept actual map mutation in `AscensionMapService.A11Geometry.cs`: width expansion, route-row insertion, and inserted-column optional route creation.
- Added `AscensionMapService.A11GeometryDiagnostics.cs` to the active source manifest.
- Removed an unused private `HasA11InsertedColumnRouteChoice(ActMap map)` overload found during read-only review.

Independent read-only review result: no blocker. The reviewer confirmed the split preserves current partial-call boundaries, the new file is included by the project glob and active source manifest, and A11/A17 map guard tests read the full map source tree. Live route traversal, save-load, and co-op proof remain live-only.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused A11/Ascension/ReleaseCoverage guards passed with 39 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice.

## A17 Deep Branch Planning Split - 2026-05-18

Scope: behavior-preserving cleanup for A17 Deep Branch map geometry. The game was not opened.

Refactor applied:
- Kept Deep Branch insertion and metadata marking in `AscensionMapService.DeepBranches.cs`.
- Moved branch plan creation, existing-branch matching, branch-column enumeration, branch route-safety checks, node type selection, and the private `DeepBranchPlan` record into `AscensionMapService.DeepBranches.Planning.cs`.
- Added `AscensionMapService.DeepBranches.Planning.cs` to the active source manifest.

Independent read-only review result: no blocker. The reviewer confirmed the split preserves the `TryInsertDeepBranch` / `MarkDeepBranch` call chain, the private partial helpers and nested plan record remain visible, the project glob includes the new file, and A11/A17 guard tests still read the full map source tree. A local `Saves.Runs` using was added to keep `DeepBranches.cs` self-contained after the split.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage/ReleaseSafety guards passed with 58 passed / 10 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice. Live A17 route generation, click traversal, save-load, and co-op proof remain pending.

## RootBud Combat Lifecycle Split - 2026-05-18

Scope: behavior-preserving cleanup for A14/A15/A18 Blight Sprout combat hook ownership. The game was not opened.

Refactor applied:
- Moved combat-start Blight Sprout seeding and combat-end Rootblight growth from `RootBudCombatHook.cs` into `RootBudCombatHook.Lifecycle.cs`.
- Kept immediate hook dispatch for draw, hand-entry, card-play, damage, turn, death, and tracker ownership in `RootBudCombatHook.cs`.
- Kept room eligibility, tracker lookup, pile scans, sprout timing, entered-hand marking, and combat-end Rootblight service call helpers in `RootBudCombatHook.Helpers.cs`.
- Updated source guards to include the lifecycle partial instead of pinning lifecycle behavior to the core hook file, and added the new file to active source coverage. The combat-end guard now also locks the existing Seedbed rule: Blight Sprouts planted in Seedbed do not grow Rootblight and have their transient flag cleared at combat end.

Independent read-only review result: no partial/visibility/manifest blocker. The reviewer flagged the Seedbed combat-end condition as a scope risk because the split is documented as behavior-preserving; the current pre-split worktree already contained that condition, and the guard was widened so the existing behavior is now explicit.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused RootBud/Ascension/ReleaseCoverage guards passed with 43 passed / 4 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice. Live Blight Sprout timing, Rootblight combat-end behavior, save-load, and co-op proof remain pending.

## Ascension Reward Fission Split - 2026-05-18

Scope: behavior-preserving cleanup for A13 Fission reward mutation. The game was not opened.

Refactor applied:
- Moved Fission source chance selection, eligibility checks, reward-card mutation, and diagnostics from `AscensionRewardService.cs` into `AscensionRewardService.Fission.cs`.
- Kept reward service entry flow, Firemarked Elite extra reward option, Boss Seal extra reward option, A20 first-boss terminal card reward, and A17 enhanced treasure payout in `AscensionRewardService.cs`.
- Added `AscensionRewardService.Fission.cs` to active source coverage. Existing Ascension reward guard tests already read the full `Ascension/Rewards` source tree.

Independent read-only review result: no blocker. The reviewer confirmed the split preserves the entry call path and private partial visibility, source-tree reward guards still see the moved Fission logic, and active source coverage contains the new file. The only notes were that the new file relies on existing project-level usings, matching the current reward-service style, and that the untracked split file must be included before commit/package.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Ascension/ReleaseCoverage/ReleaseSafety guards passed with 58 passed / 10 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice. Live reward-screen/gameplay/save-load/co-op proof remains pending.

## Lotha Card Rules Split - 2026-05-18

Scope: behavior-preserving cleanup for the last misleading `LothaRunHook.cs` service partial. The game was not opened.

Refactor applied:
- Moved Lotha card-play count, `ShouldPlay`, post-play handling, combat cost modification, shared card eligibility, auto-play guard, and Power replacement helpers into `LothaBlessingService.CardRules.cs`.
- Moved combat/turn state reset helpers into `LothaBlessingService.CombatLifecycle.cs`, beside the lifecycle code that calls them.
- Removed `LothaRunHook.cs`; hook wrappers remain in `LothaHooks.cs`, and source guards read the Lotha source tree or focused owning partials.
- Added `LothaBlessingService.CardRules.cs` to active source coverage and removed `LothaRunHook.cs` from that manifest.

Independent read-only review result: no blocker. The reviewer confirmed hook wrappers still route through `LothaHooks.cs`, private partial helpers and constants remain visible, active source coverage has no missing/extra Lotha file, and the updated Lotha tests read the owning partials or full source tree.

Validation result: `dotnet build EZMicroBalance.sln --no-restore` passed with 0 warnings/errors; focused Lotha/Vakuu save-risk/ReleaseCoverage/high-risk guards passed with 52 passed / 3 skipped; full `dotnet test EZMicroBalance.sln --no-build` passed with 180 passed / 18 skipped; `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` passed; `git diff --check` passed with CRLF/LF warnings only. The publish/package artifacts were not rebuilt for this source-only refactor slice. Live Lotha gameplay/save-load/co-op proof remains pending.
