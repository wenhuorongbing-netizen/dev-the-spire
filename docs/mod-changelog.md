# Spire Plus Changelog

Append only short tester/player-facing entries here when behavior or release validation changes.

## 2026-05-14

- Vakuu's fight now adds Temptation Status cards to the top of your draw pile after your hand draw on turns 1, 3, 5, and onward; exhausting one gives 1 Energy and costs 3 HP.
- Urda now shows four initial blessing choices in the clicked Ancient screen instead of three.
- Invalid forced blessing environment values now fall back to visible Urda, Morvi, or Lotha options with a clear log warning instead of silently falling through.
- Morvi Debt Settlement now pays missing Gold with nonlethal HP and keeps Debt decreasing by the due amount.
- Morvi Open-Book and Red Ink bookkeeping now has stronger source-side restore markers for the next save/load test round.
- Urda force/disable testing now accepts matching `SPIREPLUS_*` environment-variable aliases.
- Rewrote active Urda, Morvi, Lotha, and Vakuu Ancient text to remove implementation wording and improve English/Simplified Chinese readability.
- Fight Vakuu now clearly says it starts a real no-normal-reward fight, victory offers non-Vakuu Act 3 Ancient blessings, and death ends the run.
- Vakuu fight victory now has an explicit fallback if no unclaimed non-Vakuu blessing remains.
- New custom card text avoids duplicating visible Exhaust/Ethereal/Unplayable keywords already shown by the card UI.
- No final bespoke art was generated in this pass because `OPENAI_API_KEY` was not set; active Urda/Morvi/Lotha/Vakuu option/card art remains temporary.
- Urda, Morvi, and Lotha option/relic hover text now highlights more custom mechanics, and legacy Urda option hovers no longer show internal art-marker wording.

## 2026-05-13

- The player-facing mod name is now `Spire Plus`.
- The stable manifest id and package folder remain `EZMicroBalance` for compatibility.
- Added `docs/test-ready-development-goal.md` as the canonical next-development prompt for building the current mod to a source-complete test-ready candidate.
- Repaired active Simplified Chinese settings, card-reward, event, rest-site, and relic text, and added a guard against known mojibake fragments in every active zhs localization JSON file.
- Current normal Steam-client startup/log verification now confirms `Spire Plus (EZMicroBalance)`, config registration, 16 SavedSpireFields, and a clean main-menu load. Refreshed visible Mod Settings UI evidence now shows `Spire Plus` in the Mods list.
- Urda and Lotha now use custom Ancient background scenes, separate map/run-history icons, option art, and dialogue/localization for the active test slice.
- Lotha event art now uses the local generated mirror-tribunal background instead of the geometric placeholder, and Lotha map/option/power art now uses temporary source-derived crops pending bespoke generated relic-style replacements.
- Repaired broken Simplified Chinese Ancient expansion localization JSON so the active zhs files parse again.
- Lotha is source-complete/default-on for private-beta testing with eight blessing options and `EZMB_DISABLE_LOTHA` / `SPIREPLUS_DISABLE_LOTHA` emergency gates.
- Lotha corrective polish now covers all eight blessings: Mirror Rebuttal chosen-card setup with 2 Energy / draw 2 Power fallback; Mirror Hall Echo turn-end type recording; Presumption Innocent state; Closed Court no-card-rewards plus first-turn burst; Deferred Verdict player-owned turn-4 Verdict stacks; Death Reprieve reprieve turn; Single Sentence first Attack/Skill ruling plus four-card cap; and Public Evidence Debuff/Enlightenment hooks.
- Death Reprieve has a source-safe timing note for testers: player-turn lethal starts the reprieve immediately, while enemy-turn lethal starts it on the next player turn because safe enemy-turn interruption is not proven by local source.
- Lotha English and Simplified Chinese option/relic/power text now highlights Attack/Skill/Power, Energy, Block, Verdict, Debuff, Enlightenment, Innocent, and Death Reprieve terms.
- Morvi is source-complete/default-on for private-beta Act 2 testing with all eight v2.2 blessing options, custom event art, option icons, English/Simplified Chinese localization, hover powers, `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`, force-Ancient gates, and force-blessing gates.
- Morvi source-safe tester notes: Forbidden Loan auto-settles after the Act 2 boss instead of opening a post-boss choice, Red Ink Overdraft appears as a temporary active card instead of a native combat button, Open-Book sealed cards return from an exhaust-pile holding path when hand space allows, and temporary Morvi option/icon art is not final bespoke art.
- Morvi live gameplay, save/load, and co-op verification remain pending.
- Urda now has ten source-backed default-on v2.2 blessing ids: Seedbed, Humus Pact, Molting, Moss Map, Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank.
- Urda source-safe tester notes: Trial Branch uses a simple 4-card selection grid, Shallow-Root Relic uses Act 2 removal/refund instead of the unproven `lose 6 Max HP` choice UI, Rooted Route auto-marks a reachable normal-combat node without graph mutation, Root-Sight auto-marks non-Boss nodes instead of adding a map button, and Seed Bank stores by consuming the reward.
- Urda's six new option icons are temporary source-derived crops because `OPENAI_API_KEY` was not set; bespoke relic-style icons remain pending.
- Fight Vakuu is source-complete/default-on for single-player private-beta testing with `EZMB_DISABLE_VAKUU_FIGHT` / `SPIREPLUS_DISABLE_VAKUU_FIGHT`, force-Ancient gates, force-fight gates, a custom Event combat, and three non-Vakuu Act 3 Ancient blessing choices on victory.
- Live gameplay, save/load, failure/death, and co-op verification for Urda, Lotha, and Vakuu remain pending.

## 2026-05-12

- Morvi default-off prototype hardening: Misprint Press now cleans up generated copies if they cannot enter combat, and Debt Settlement keeps its final payoff pending until the upgraded-card reward is actually offered.
- Morvi Debt Settlement text now tells players that missing Gold is paid with nonlethal HP.
- Morvi/Lotha event art remains pending because no explicit local source PNGs were available; no placeholder art was added.
- Morvi, the Lender-Scribe now has a default-off Act 2 prototype for focused testing.
- Set `EZMB_ENABLE_MORVI_V22=1` to include Morvi in Act 2 Ancient selection.
- Set `EZMB_FORCE_MORVI_BLESSING=morvi_misprint_press`, `morvi_open_book_exam`, or `morvi_debt_settlement` to force one prototype blessing.
- Morvi prototype blessings: Misprint Press replays your first Attack or Skill each combat, Open-Book Exam upgrades one eligible Act 2 combat reward card, and Debt Settlement trades immediate gold for three later repayments.
- Lotha and Vakuu fight content remained unimplemented at that time; the six additional Urda blessings were implemented later on 2026-05-13.
- Live gameplay, save/load, and co-op verification for Morvi remain pending.

## 2026-05-11

- Urda, Loamweaver is now enabled by default for private-beta testing in Act 1.
- Set `EZMB_DISABLE_URDA=1` only when you want to hide Urda for comparison.
- Urda now has first-pass gameplay for Seedbed, Humus Pact, Molting, and Moss Map; live gameplay and save/load testing are still pending.
- Seedbed can trade max HP for Seedlings from normal Act 1 combat card rewards, counts accepted Seedbed choices only, and rewards four accepts with max HP without healing.
- Humus Pact uses a Compost Reward card-reward option for normal Act 1 combat rewards, then opens a small removal plus upgraded-card payoff after the third compost.
- Molting removes one Strike and one Defend, adds two Withered Husk cards, then clears those husks at Act 2.
- Moss Map grants one small Act 1 reward per first visited room type.

## 2026-05-08

- Multiplayer A20: fixed a black-screen run-start crash caused by hard references to optional Early Access Boss Seal types missing from the current installed game DLL.
- Compatibility: Debt patch paths avoid direct assumptions about game members that changed across local Early Access builds. Pumpkin Candle and Door Wedge EZMB overrides are removed from the active v0.105.0 package; Pumpkin Candle is vanilla and Doormaker/Door Wedge is replaced by Aeonglass +5 Strength.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification.
- Normal Steam-client Mod Settings has historical RC1 UI evidence for BaseLib and the old EZ Micro Balance display name; current normal Steam startup/log evidence confirms the Spire Plus display-name package. Startup/log checks are not the same as live co-op or gameplay verification.

## 2026-05-07

- Ancient rewards: completed the active v4.3 rebalance and major boundary fixes.
- Package structure: the mod remains independent with manifest id `EZMicroBalance`.
- Release art: active cover art is original, with no visible text, numbers, logos, or official game assets.
- A11: Wide Tower, Long Road now changes vanilla-looking map geometry only: width +1, Act 1 +1 route row, Act 2 +1 route row, and Act 3 +2 route rows.
- A11: removed the dedicated Long Road marker/icon/hover tooltip; ordinary route nodes still look vanilla.
- A12: Firemarked Elites use dedicated route indicators, a visible Firemark Host, Forge Token rewards, and Heal/Smith token payouts.
- A12: Firemark, Forge Token, and Banner text now uses native rich-text color markup for important values and terms.
- A13: Fission reward appearance rates were raised for development testing, with stricter card eligibility and icon support.
- A14/A15/A18: Rootblight and Blight Sprout were migrated to the v2.0 card-state design and still need live verification.
- A16: Banner Rooms are visible enhanced normal combats with public rule text.
- A17: Deep Branches are optional Act 2/3 side branches in single-player only until route-voting behavior is proven.
- A19/A20: Boss Royal Seal / King Brand map hover text remains available; A20 Dual King Brands gameplay remains single-player gated.
- Multiplayer A20: host selection is enabled only for development testing when the public Ascension gate is on; logs now warn that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification.
- Tests: release artifact/package/runtime-smoke checks are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`; normal developer tests no longer require ignored `publish/` package artifacts.
- Status: current-package normal Steam helper startup/log/resource verification under `.tools/runtime-evidence/current-package-smoke-20260514-015901` passed with BaseLib and Spire Plus / `EZMicroBalance` loaded, `Found 22 SavedSpireFields`, clean audit/manual scans, and installed-PCK Ancient resource coverage; the refreshed Mod Settings UI list screenshot shows `Spire Plus`. Live feature verification, save/load, and live co-op verification remain pending until actually executed.
