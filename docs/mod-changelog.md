# Spire Plus Changelog

Append only short tester/player-facing entries here when behavior or release validation changes.

## 2026-05-17

- Release-implementation review fixed two Banner Room timing issues: Shieldwall now protects enemies after the enemy turn, and Blood Prize retaliation now starts when the round 3 deadline is missed.
- Tester environment flags now trim surrounding whitespace, so values like `1`, ` true `, `yes`, and `on` behave consistently across Ancient and Ascension gates.
- Moss Map relic text now always shows the exact Act 1 room reward mapping, even if the game resolves the older class-name localization key.
- Rebuilt the local manual-test package and refreshed package hashes after the static fixes. This is still not live gameplay, save-load, or co-op evidence.
- Urda Trial Branch now offers four rare cards, upgrades the chosen card, applies a visible Trial Branch enchantment, and requires the card to be played in each of the next three combats; missing any combat removes it.
- Lotha Single Sentence now uses live combat hooks and a visible counter Power so the player can see when the ruling is ready, how many plays remain, and when further plays are blocked.
- Urda, Morvi, Lotha, and gated Vakuu combat-only hooks now register through combat-state subscriptions, fixing Ancient effects that were not reached by card play, turn-start, cost-preview, and combat-entry hook paths.
- Humus Pact text now explicitly explains the Act 1 normal-combat Compost Reward button, skipped-card tradeoff, third compost removals, and upgraded-card payoff.
- Vakuu's fight keeps the same Contract/lock/Blood Debt rewards, but its active combat room no longer stores a parent event id while the fight is in progress; parent restoration is now recorded only after victory for the saved prefinished-combat path.
- Vakuu art assets were re-audited against the GPTimage2 manifest and export list; no replacement art was needed.

## 2026-05-14

- Vakuu's fight now offers three Contract choices after your hand draw on turns 1, 3, and 5; Contracts cost HP, break Stolen Vault locks, add Blood Debt, and improve victory blessing choices plus Gold.
- Urda now shows four initial blessing choices in the clicked Ancient screen instead of three.
- Invalid forced blessing environment values now fall back to visible Urda, Morvi, or Lotha options with a clear log warning instead of silently falling through.
- Morvi Debt Settlement now pays missing Gold with nonlethal HP and keeps Debt decreasing by the due amount.
- Morvi Open-Book and Red Ink bookkeeping now has stronger source-side restore markers for the next save/load test round.
- Urda force/disable testing now accepts matching `SPIREPLUS_*` environment-variable aliases.
- Rewrote active Urda, Morvi, Lotha, and Vakuu Ancient text to remove implementation wording and improve English/Simplified Chinese readability.
- Fight Vakuu now clearly says it starts a real no-normal-reward fight, victory offers non-Vakuu Act 3 Ancient blessings, and death ends the run.
- Vakuu fight victory now has an explicit fallback if no unclaimed non-Vakuu blessing remains.
- New custom card text avoids duplicating visible Exhaust/Ethereal/Unplayable keywords already shown by the card UI.
- At that point no final bespoke art was generated because `OPENAI_API_KEY` was not set; the later GPTimage2/browser art pass superseded the temporary option/card art note for the current package.
- Urda, Morvi, and Lotha option/relic hover text now highlights more custom mechanics, and legacy Urda option hovers no longer show internal art-marker wording.

## 2026-05-13

- The player-facing mod name is now `Spire Plus`.
- The stable manifest id and package folder remain `EZMicroBalance` for compatibility.
- Added `docs/test-ready-development-goal.md` as the canonical next-development prompt for building the current mod to a source-complete test-ready candidate.
- Repaired active Simplified Chinese settings, card-reward, event, rest-site, and relic text, and added a guard against known mojibake fragments in every active zhs localization JSON file.
- Historical normal Steam-client startup/log verification at that time confirmed `Spire Plus (EZMicroBalance)`, config registration, 16 previous saved-state registrations, and a clean main-menu load. Previous beta.93 startup/log evidence is RitsuLib-only loader proof and does not replace the still-pending live Mod Settings page refresh.
- Urda and Lotha now use custom Ancient background scenes, separate map/run-history icons, option art, and dialogue/localization for the active test slice.
- At that point Lotha event art used the local generated mirror-tribunal background and temporary source-derived map/option/power crops; the later GPTimage2/browser art pass superseded the temporary-crop note for the current package.
- Repaired broken Simplified Chinese Ancient expansion localization JSON so the active zhs files parse again.
- Lotha is source-complete/default-on for private-beta testing with eight blessing options and preferred `SPIREPLUS_DISABLE_LOTHA` plus legacy `EZMB_DISABLE_LOTHA` emergency gates.
- Lotha corrective polish now covers all eight blessings: Mirror Rebuttal chosen-card setup with a 0-cost Power fallback; Mirror Hall Echo turn-end type recording; Presumption Innocent state; Closed Court no-card-rewards plus split turn-1 and turn-4 resources; Deferred Verdict player-owned turn-4 Verdict stacks; Death Reprieve reprieve turn; Single Sentence first Attack/Skill ruling plus four-card cap; and Public Evidence Debuff/Enlightenment hooks.
- Death Reprieve has a source-safe timing note for testers: player-turn lethal starts the reprieve immediately, while enemy-turn lethal starts it on the next player turn because safe enemy-turn interruption is not proven by local source.
- Lotha English and Simplified Chinese option/relic/power text now highlights Attack/Skill/Power, Energy, Block, Verdict, Debuff, Enlightenment, Innocent, and Death Reprieve terms.
- Morvi is source-complete/default-on for private-beta Act 2 testing with all eight v2.2 blessing options, custom event art, option icons, English/Simplified Chinese localization, hover powers, preferred `SPIREPLUS_DISABLE_MORVI` plus legacy `EZMB_DISABLE_MORVI`, force-Ancient gates, and force-blessing gates.
- Morvi source-safe tester notes: Forbidden Loan auto-settles after the Act 2 boss instead of opening a post-boss choice, Red Ink Overdraft appears as a temporary active card instead of a native combat button, and Open-Book sealed cards return from an exhaust-pile holding path when hand space allows. At that time Morvi option/icon art was still temporary; the later GPTimage2/browser art pass superseded it for the current package.
- Morvi live gameplay, save/load, and co-op verification remain pending.
- Urda now has eleven source-backed default-on blessing ids: Seedbed, Humus Pact, Molting, Moss Map, Trial Branch, Shallow-Root Relic, Elite Root, Rooted Route, After the Rain, Root-Sight, and Seed Bank.
- Urda source-safe tester notes: Trial Branch uses a simple 4-card selection grid, Shallow-Root Relic uses Act 2 removal/refund instead of the unproven `lose 6 Max HP` choice UI, Rooted Route auto-marks a reachable normal-combat node without graph mutation, Root-Sight uses the Root Eyes relic to choose a future reachable Monster, Unknown, or Elite room, and Seed Bank stores by consuming the reward.
- At that point Urda's six new option icons were temporary source-derived crops because `OPENAI_API_KEY` was not set; the later GPTimage2/browser art pass superseded this for the current package.
- Fight Vakuu is hidden by default behind `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` or a force-fight gate. Legacy `EZMB_ENABLE_VAKUU_FIGHT=1` still works. It now uses a dedicated Vakuu enemy and encounter scene, but still needs live post-victory, save-load, and failure-path proof before normal exposure.
- Live gameplay, save/load, failure/death, and co-op verification for Urda, Lotha, and Vakuu remain pending.

## 2026-05-12

- Morvi default-off prototype hardening: Misprint Press now cleans up generated copies if they cannot enter combat, and Debt Settlement keeps its final payoff pending until the upgraded-card reward is actually offered.
- Morvi Debt Settlement text now tells players that missing Gold is paid with nonlethal HP.
- At that time Morvi/Lotha event art remained pending because no explicit local source PNGs were available; no placeholder art was added. Later 2026-05-13/15 art passes superseded this for the current package.
- Morvi, the Lender-Scribe now has a default-off Act 2 prototype for focused testing.
- Historical test gate: `EZMB_ENABLE_MORVI_V22=1` included Morvi in Act 2 Ancient selection at that time; current builds use default-on Morvi unless `SPIREPLUS_DISABLE_MORVI=1` is set.
- Set `SPIREPLUS_FORCE_MORVI_BLESSING=morvi_misprint_press`, `morvi_open_book_exam`, or `morvi_debt_settlement` to force one prototype blessing. Legacy `EZMB_FORCE_MORVI_BLESSING` still works.
- Morvi prototype blessings: Misprint Press replays your first Attack or Skill each combat, Open-Book Exam upgrades one eligible Act 2 combat reward card, and Debt Settlement trades immediate gold for three later repayments.
- Lotha and Vakuu fight content remained unimplemented at that time; the six additional Urda blessings were implemented later on 2026-05-13.
- Live gameplay, save/load, and co-op verification for Morvi remain pending.

## 2026-05-11

- Urda, Loamweaver is now enabled by default for private-beta testing in Act 1.
- Set `SPIREPLUS_DISABLE_URDA=1` only when you want to hide Urda for comparison. Legacy `EZMB_DISABLE_URDA=1` still works.
- Urda now has first-pass gameplay for Seedbed, Humus Pact, Molting, and Moss Map; live gameplay and save/load testing are still pending.
- Seedbed can trade max HP for Seedlings from normal Act 1 combat card rewards, counts accepted Seedbed choices only, and rewards four accepts with max HP without healing.
- Humus Pact uses a Compost Reward card-reward option for normal Act 1 combat rewards, then opens a small removal plus upgraded-card payoff after the third compost.
- Molting removes one Strike and one Defend, adds two Withered Husk cards, then clears those husks at Act 2.
- Moss Map grants one small Act 1 reward per first visited room type.

## 2026-05-08

- Multiplayer A20: fixed a black-screen run-start crash caused by hard references to optional Early Access dedicated ability types missing from the current installed game DLL.
- Compatibility: Debt patch paths avoid direct assumptions about game members that changed across local Early Access builds. Pumpkin Candle and Door Wedge EZMB overrides are removed from the active v0.106.1 package; Pumpkin Candle is vanilla and Doormaker/Door Wedge is replaced by Aeonglass Time Sand Reflow.
- A11-A20 selection is default-on for single-player standard lobbies. Host-multiplayer A11-A20 selection/gameplay now fails closed by default after the 2026-05-25 co-op crash logs unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging.
- Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. A20 Branded Form / second-boss enhanced dedicated ability gameplay remains disabled or downgraded in co-op pending live verification.
- Normal Steam-client Mod Settings has historical RC1 UI evidence that predates the current display-name refresh. Previous beta.93 startup/log evidence confirms Spire Plus loads as technical id `EZMicroBalance` with only STS2-RitsuLib as the required runtime dependency. Startup/log checks are not the same as live co-op or gameplay verification.

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
- A19/A20: boss dedicated ability / Branded Form map hover text remains available; A20 Branded Form gameplay remains single-player gated.
- Multiplayer A20: host selection is enabled only for development testing when the public Ascension gate is on; logs now warn that A20 Branded Form / second-boss enhanced dedicated ability gameplay is disabled or downgraded in co-op pending live verification.
- Tests: release artifact/package/runtime-smoke checks are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`; normal developer tests no longer require ignored `publish/` package artifacts. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- Status: the 2026-05-24 Steam-client helper startup/log verification under `.tools/runtime-evidence/manual-test-handoff-20260524-161744/release/fresh-current-package-loader-smoke` is historical previous package loader context only. Previous beta.93 startup/log evidence loads only `STS2-RitsuLib` and Spire Plus for the required framework/mod pair; live feature verification, save-load, and live co-op verification remain pending until actually executed.
