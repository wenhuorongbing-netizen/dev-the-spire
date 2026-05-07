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

The active test project covers release identity, active localization JSON/key parity, zhs no-space numeric formatting, selected-resource PCK contents, installed manifest parity, installed/staging/package hash parity, Harmony patch target resolution against the installed game API, Prismatic Gem v4.3 reroll/all-slot documentation, detached-banner rejection, banner fallback diagnostics, fallback evidence, and manual-test coverage, Velvet Choker and Distinguished Cape no-shrink/max-HP source guards, Ancient behavior source guards, stale current-doc behavior guards, Ascension selector/source guards, A12 firemark map/token/power source guards, A13 Fission icon/text/eligibility source guards, current setup/compatibility/manual-checklist doc targeting, false release-art claim guards, unsupported-system completion guards, and active project/export isolation from legacy sources.

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
- `affects_gameplay` remains `true` for EZ Micro Balance.

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
6. Confirm EZ Micro Balance appears under its release mod id.
7. Confirm EZ Micro Balance can be enabled.
8. Open the card encyclopedia / Card Library and confirm card lists render, sort, and filter without errors.
9. Start a run and reach Ancient rewards.
10. Inspect `godot.log` for `EZMicroBalance`, `EzDailyContent`, `BaseLib`, `error`, and `exception`; specifically confirm no `VelvetChokerSoftLimitTracker.ShouldTax` or `CanonicalModelException` appears after opening the card encyclopedia.

## EZ Micro Balance Feature Verification Matrix

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
- Pumpkin Candle: act 3 extinguish randomly upgrades two cards.
- Meat Cleaver: Cook removes two cards and loses five current HP; disabled when unavailable.
- Blood-Soaked Rose / Enthralled: Enthralled gains 10 Block while preserving forced-priority behavior.

## Save/Load Checks

Required for:

- Prismatic Gem saved standard reward counter and screen-scoped reroll trigger state.
- Pael's Tooth stored removed cards and combat counter.
- Jeweled Mask persistent free-power enchantment.
- Debt loaded from save.
- Folly loaded from save.

## Disable Checks

- Disable EZ Micro Balance.
- Start or load a run where possible.
- Confirm no EZ Micro Balance logs or patches are active when the mod is disabled.
- Confirm future mods remain independently enableable.

## Current Status

The prior legacy `EzDailyContent` setup passed build, publish, and Mod Settings verification on public beta `v0.104.0` (`2026.04.23`) with BaseLib `v3.1.0`.

v4.3 is current for Ancient behavior. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only. The current automated suite count and command results are recorded in `docs/features/ancients-rework-v4/completion-audit.md` after each validation refresh. Final bounded smoke loading with `--force-steam off` previously reached main menu with BaseLib and EZ Micro Balance initialized from the installed artifacts. Private beta status is not complete until normal Steam-client Mod Settings verification and the manual feature matrix have runtime results.

Ascension 11-20 is now an active development track. The A11-A20 single-player and host-multiplayer selection patch is implemented but private-beta default-disabled, guarded by automated source tests that require the patch to stay on standard lobby paths and avoid global progress getter/save validation patches. Enable it only for development testing with `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`, disable host-multiplayer selection separately with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, or force slices with `EZMB_ASCENSION_DEBUG_LEVEL`. Full live Ascension and co-op verification is pending.
