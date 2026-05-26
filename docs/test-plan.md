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
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

CI or shell variants may set `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` before the test command. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` still works. If either variable is set and package artifacts are missing or stale, the release artifact tests should fail with missing-file/hash mismatch details.

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
- `affects_gameplay` remains `true` for Spire Plus.
- `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` passes after package staging, versioned package directory, and zip artifacts are refreshed. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.

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
10. Inspect `godot.log` for `EZMicroBalance`, `BaseLib`, old scaffold mod names, `error`, and `exception`; specifically confirm no `VelvetChokerSoftLimitTracker.ShouldTax` or `CanonicalModelException` appears after opening the card encyclopedia.

## Spire Plus Feature Verification Matrix

Detailed execution rows are tracked in `docs/features/ancients-rework-v4/manual-verification-matrix.md`.

Each implemented Ancient reward change needs a manual result before private beta:

- Pael's Horn: adds one `Relax` and one `Relax+`.
- Black Star: act 3+ pickup immediately grants one random relic; normal elite bonus remains.
- War Hammer: pickup chooses two cards to upgrade; elite kill upgrades remain.
- Jewelry Box: adds Apotheosis without Innate.
- Preserved Fog / Folly: removes four cards and adds Folly with Unplayable, Innate, Eternal.
- Vakuu's Sere Talon / 瓦库原初之爪: offers four Curses, lets the player choose one, then adds that Curse, two Wish, and one Wish+ as the Vakuu reward. Tanx Claws / 坦克斯利爪 remains the separate Tanx relic that transforms cards into upgraded Maul+ / 撕咬+ cards.
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

- Disable Spire Plus.
- Start or load a run where possible.
- Confirm no Spire Plus logs or patches are active when the mod is disabled.
- Confirm only the intended `Spire Plus` mod is active for this private-beta package.

## Current Status

The prior legacy `EzDailyContent` setup passed build, publish, and Mod Settings verification on public beta `v0.104.0` (`2026.04.23`) with BaseLib `v3.1.0`.

v4.3 is current for Ancient behavior. v4.2 rightmost-slot Prismatic Gem and v4.2 Distinguished Cape 40% min15 are historical only.

- Current automated suite count and command results are recorded in `docs/features/ancients-rework-v4/completion-audit.md` after each validation refresh.
- Historical package smoke/log/resource evidence under `.tools/runtime-evidence/current-package-smoke-20260514-015901` covers the earlier 22-field package, installed-PCK loading for Urda/Morvi/Lotha scenes plus 43 Ancient textures, and a clean normal Steam helper startup with BaseLib plus Spire Plus.
- Historical Steam-client loader evidence under `.tools/runtime-evidence/beta19-loader-smoke-20260525-213336` reports `v0.1.0-private-beta.19`, `Found 30 SavedSpireFields`, only BaseLib plus Spire Plus loaded, clean log audit, stopped game, and restored mod isolation for the beta.19 package. The current beta.26 package still needs fresh loader proof. Older beta.17, beta.13, `20260523-current`, 16-field, and 22-field startup/log passes are historical.
- BaseLib-only plug-off evidence under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-143020` loaded `1 mods (1 total)` and did not initialize Spire Plus. The earlier settings-only disabled attempt is invalid because Spire Plus still initialized.
- Current Mod Settings UI list evidence, historical Mod Settings page evidence, A11 map/save-load spot checks, saved-map boss-reachability proof, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight hover/starter-notice checks have evidence.
- The A14 Rootblight art-hover probe found pre-fix Urda missing asset paths before combat. Urda now uses custom Ancient icon/background-scene paths and the current package resolves Ancient scene/art resources in headless installed-PCK verification, but post-fix live Urda and Rootblight visual/gameplay checks remain pending.
- Use `scripts/spire-plus-live-session.ps1` to prepare and restore normal Steam-client local test sessions. Use `-PreserveNewCurrentRunsOnRestore` so test-created `current_run*` files are preserved in the evidence folder before original current-run files are restored.
- Run `scripts/check-spire-window-preflight.ps1 -RequireSpireForeground` before screenshot evidence so covered desktop captures are rejected before they can be counted.
- No-launch validation confirmed helper restore behavior under `.tools/runtime-evidence/live-helper-preserve-current-run-smoke-20260513-133431`; the BaseLib-only plug-off helper path was no-launch checked under `.tools/runtime-evidence/live-spire-plus-disabled-session-20260513-142957`.
- Rootblight combat-end notices are source-hardened with a top-level overlay path, but private beta status is not complete until the manual feature matrix has runtime gameplay, broader save/load, full Rootblight combat-end/co-op behavior, and multiplayer results.

Ascension 11-20 is now an active development track.

- A11-A20 selection is default-on for single-player standard lobbies. Host-multiplayer A11-A20 selection/gameplay fails closed by default after the 2026-05-25 crash logs unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging. Source tests keep the patch on standard lobby paths and reject global progress getter/save validation patches.
- Gate controls: `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, legacy-compatible `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`, and forced internal checks through `SPIREPLUS_ASCENSION_DEBUG_LEVEL`.
- Host multiplayer A20 selection/start logs a warning that A20 Branded Form / second-boss enhanced dedicated ability gameplay is disabled or downgraded in co-op pending live verification. A20 multiplayer selection is not full A20 co-op support.
- Current normal Steam-client startup/log and historical Mod Settings UI have separate evidence; startup/log checks are not the same as live co-op verification.
- A11 Act 1 map/save-load spot check passed with `columns=8; rows=17`; saved-map graph proof shows a boss path from post-load coord `(3,1)` to boss `(3,17)`.
- Act 2/3 map-surface observation passed with Act 2 `columns=8; rows=16` and Act 3 `columns=8; rows=16`. Targeted A14 Rootblight English/ZHS hover/starter-notice checks passed.
- Natural click-by-click traversal, full live Ascension, full Rootblight combat-end/co-op verification, and co-op verification remain pending. Use `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the multiplayer matrix.
