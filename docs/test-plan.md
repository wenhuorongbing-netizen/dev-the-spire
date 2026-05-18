# Test Plan

## Automated Checks

Run after code/config changes:

```powershell
dotnet build
```

Run after build succeeds:

```powershell
dotnet test EZMicroBalance.sln --no-build
```

Normal developer tests cover release identity, active localization JSON/key parity, zhs no-space numeric formatting, Prismatic Gem v4.3 reroll/all-slot documentation, detached-banner rejection, banner fallback diagnostics, fallback evidence, manual-test coverage, Velvet Choker and Distinguished Cape no-shrink/max-HP source guards, Ancient behavior source guards, stale current-doc behavior guards, Ascension selector/source guards, A12 firemark map/token/power source guards, A13 Fission icon/text/eligibility source guards, A20 multiplayer downgrade warning source guards, current setup/compatibility/manual-checklist doc targeting, false release-art claim guards, unsupported-system completion guards, and active project/export isolation from legacy sources.

Release artifact, installed DLL/PCK, package hash, and runtime-smoke evidence tests are skipped in normal developer test runs because `publish/`, `.godot/`, `.zip`, `.dll`, and `.pck` outputs are ignored. Run them only after publish and package staging/zip refresh:

```powershell
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

CI or shell variants may set `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` before the test command. If that variable is set and package artifacts are missing or stale, the release artifact tests should fail with missing-file/hash mismatch details.

Run after resource, localization, manifest, project, or packaging changes:

```powershell
dotnet publish
```

Required artifact checks after publish:

- Manifest exists in the game `mods/<ModId>/` folder.
- DLL exists in the game `mods/<ModId>/` folder.
- PCK exists when `has_pck` is `true`.
- Manifest id matches the intended stable id.
- Dependencies include `BaseLib`.
- `affects_gameplay` remains `true` for Spire Plus / `EZMicroBalance`.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passes after package staging, versioned package directory, and zip artifacts are refreshed.

## Localization Checks

- Parse all English JSON files.
- Parse all Simplified Chinese JSON files using explicit UTF-8.
- Confirm keys match the tables actually loaded by the game.
- Confirm text matches implemented behavior, especially changed relic/card/rest-site behavior.

## Manual Game Load Checks

1. Launch Slay the Spire 2 public beta.
2. Open Settings.
3. Open Mod Settings.
4. Confirm BaseLib appears.
5. Confirm BaseLib is enabled.
6. Confirm Spire Plus appears under its release mod id `EZMicroBalance`.
7. Confirm Spire Plus can be enabled.
8. Open the card encyclopedia / Card Library and confirm card lists render, sort, and filter without errors.
9. Start a run and reach Ancient rewards.
10. Inspect `godot.log` for `EZMicroBalance`, `EzDailyContent`, `BaseLib`, `error`, and `exception`; specifically confirm no `VelvetChokerSoftLimitTracker.ShouldTax` or `CanonicalModelException` appears after opening the card encyclopedia.

## Spire Plus Feature Verification Matrix

Detailed execution rows are tracked in `docs/features/ancients-rework-v4/manual-verification-matrix.md`.

Each implemented Ancient reward change needs a manual result before private beta:

- Pael's Horn: adds one `Relax` and one upgraded `Relax+`.
- Black Star: act 3+ pickup immediately grants one random relic; normal elite bonus remains.
- War Hammer: pickup chooses two cards to upgrade; elite kill upgrades remain.
- Jewelry Box: adds Apotheosis without Innate.
- Preserved Fog / Folly: removes four cards and adds Folly with Unplayable, Innate, Eternal.
- Claws: chooses one curse from four and adds two Wish plus one upgraded Wish+.
- Choices Paradox: combat-start five rare choices, Retain, combat temporary.
- Jeweled Mask: selected/drafted power permanently costs 0 and is moved from draw pile to hand at combat start.
- Prismatic Gem: Every second standard card reward contains only off-color cards; reroll preserves trigger/non-trigger state; non-normal rewards do not count; the reward-screen hint logs a fallback if the banner cannot be updated.
- Distinguished Cape: pickup uses `lose 30% of current Max HP, at least 18`; current max HP must be greater than the calculated cost before the trade can be selected. If an unaffordable Cape would roll, Vakuu uses a same-pool replacement without shrinking the three visible reward options; a localized locked Cape is only a defensive fallback. Pickup adds 3 Apparition / `灵体` cards, and the max-HP cost is not damage.
- Velvet Choker: no hard six-card cap; only the 7th and later manual from-hand card plays cost +1 after other cost changes; copied, autoplayed, and repeated plays do not advance the counter.
- Pael's Tooth: removes five cards, returns one stored card upgraded every two non-boss combats, clears remaining after act boss/act transition.
- Sovereign Blade / Forge: forged temporary Sovereign Blade gains Exhaust; permanent Refine Blade is unchanged.
- Seal of Gold / Debt: grants energy and two playable Debt curses; Debt loses gold only on exhaust.
- Sozu: fills empty potion slots on pickup, then blocks future potion gain.
- Ectoplasm: grants 250 gold on pickup, then blocks future gold gain.
- Fiddle: draws toward seven at turn start and caps player-turn draw above seven.
- Iron Club: draws one card every five cards played.
- Brilliant Scarf: sixth card each turn costs 0.
- Beautiful Bracelet: selected cards gain Swift 2.
- Music Box: first attack each turn creates a discounted Ethereal Exhaust copy.
- Crossbow: turn-start random attack offer can be accepted or skipped; skipped generated card does not linger.
- Toasty Mittens: top draw-pile card can be exhausted for Strength or kept.
- Whispering Earring: first three turns auto-play one highest-cost playable hand card after draw.
- Meat Cleaver: Cleaver / 切肉 removes two cards and loses five current HP; disabled when unavailable.
- Blood-Soaked Rose / Enthralled: Enthralled gains 10 Block while preserving forced-priority behavior.

## Save/Load Checks

Required for:

- Prismatic Gem saved standard reward counter and screen-scoped reroll trigger state.
- Pael's Tooth stored removed cards and combat counter.
- Jeweled Mask persistent free-power enchantment.
- Debt loaded from save.
- Folly loaded from save.

## Disable Checks

- Disable Spire Plus / `EZMicroBalance`.
- Start or load a run where possible.
- Confirm no Spire Plus / `EZMicroBalance` logs or patches are active when the mod is disabled.
- Confirm future mods remain independently enableable.

## Current Status

The prior legacy `EzDailyContent` setup passed build, publish, and Mod Settings verification on public beta `v0.104.0` (`2026.04.23`) with BaseLib `v3.1.0`.

v4.3 is current for Ancient behavior. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only. The current automated suite count and command results are recorded in `docs/features/ancients-rework-v4/completion-audit.md` after each validation refresh. Historical package smoke/log/resource evidence under `.tools/runtime-evidence/current-package-smoke-20260514-015901` verifies artifact hash parity for the earlier 22-field package, installed-PCK loading for Urda/Morvi/Lotha scenes and 43 Ancient textures, 0 missing Ancient resource/localization keys, and a normal Steam helper startup with exactly BaseLib plus Spire Plus / `EZMicroBalance`; the log records `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, and `Time to main menu: 14,045ms`, with clean audit/manual scans and restore leaving 0 `SlayTheSpire2` processes. current source defines 25 SavedSpireFields after the 2026-05-17 static fixes, so fresh live loader parity remains pending for the refreshed package. Older 2026-05-13 controlled and helper startup/log passes reporting `Found 16 SavedSpireFields` are historical for the earlier field-count state. A BaseLib-only plug-off normal Steam startup/log pass under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` temporarily isolated `EZMicroBalance`, loaded `1 mods (1 total)`, did not initialize Spire Plus / `EZMicroBalance`, restored settings plus 25 moved entries and the current-run save, and audited clean; the earlier settings-only disabled attempt under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142835` is invalid plug-off evidence because Spire Plus still initialized. Current Mod Settings UI list evidence, historical normal Steam-client Mod Settings page evidence, the Act 1 A11 map/save-load spot check, saved-map boss-reachability graph proof, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks now have evidence. The 2026-05-13 A14 Rootblight art-hover probe found pre-fix Urda missing asset paths before combat; Urda now uses custom Ancient icon/background-scene paths and the current package resolves Ancient scene/art resources in headless installed-PCK verification, but post-fix live Urda and Rootblight visual/gameplay checks remain pending. Use `scripts/spire-plus-live-session.ps1` to prepare and restore normal Steam-client local test sessions; restore sessions that start or continue a run with `-PreserveNewCurrentRunsOnRestore` so test-created `current_run*` files are preserved in the evidence folder before original current-run files are restored. Run `scripts/check-spire-window-preflight.ps1 -RequireSpireForeground` before screenshot evidence so covered desktop captures are rejected before they can be counted. No-launch validation on 2026-05-13 confirmed settings restore, 24-entry mod isolation restore, current-run isolation no-op restore, and preserve-new-current-run restore under `.tools/runtime-evidence/live-helper-preserve-current-run-smoke-20260513-133431`; the BaseLib-only plug-off helper path was no-launch checked under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142957` with 25 moved/restored entries including `EZMicroBalance`. Rootblight combat-end notices are source-hardened with a top-level overlay path, but private beta status is not complete until the manual feature matrix has runtime gameplay, broader save/load, full Rootblight combat-end/co-op behavior, and multiplayer results.

Ascension 11-20 is now an active development track. A11-A20 selection is now default-on in this private-beta multiplayer test candidate, guarded by automated source tests that require the patch to stay on standard lobby paths and avoid global progress getter/save validation patches. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. `EZMB_ASCENSION_DEBUG_LEVEL` remains available for forced internal slice checks. Host multiplayer A20 selection/start now logs a warning that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification. A20 multiplayer selection is not full A20 co-op support. Current normal Steam-client startup/log and historical Mod Settings UI have separate evidence; startup/log checks are not the same as live co-op verification. A11 Act 1 map/save-load spot check passed with `columns=8; rows=17`; saved-map graph proof shows a boss path from post-load coord `(3,1)` to boss `(3,17)`; Act 2/3 map-surface observation passed with Act 2 `columns=8; rows=16` and Act 3 `columns=8; rows=16`; targeted A14 Rootblight English/ZHS hover/starter-notice checks passed; natural click-by-click traversal, full live Ascension, full Rootblight combat-end/co-op verification, and co-op verification remain pending. Use `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the multiplayer matrix.
