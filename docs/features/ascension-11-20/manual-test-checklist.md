# Ascension 11-20 Manual Test Checklist

Project: EZ Micro Balance  
Manifest id: EZMicroBalance  
Status: checklist for A11-A20 default-on private-beta multiplayer test candidate; live Ascension gameplay not executed yet
Last updated: 2026-05-08

## Research-Mode Baseline

- [x] Read `AGENTS.md`.
- [x] Read `docs/features/ascension-11-20/source-design.md`.
- [x] Inspect current EZ Micro Balance architecture.
- [x] Run `git status --short --branch`.
- [x] Check whether `SlayTheSpire2.exe` is running before build.
- [x] Run `dotnet build EZMicroBalance.sln`.
- [x] Launch game and verify EZ Micro Balance loads after Ascension implementation in a bounded `--force-steam off` smoke profile.
- [x] Inspect `godot.log` after Ascension implementation for startup errors.

Baseline result on 2026-05-06:

- `SlayTheSpire2.exe` was not running.
- `dotnet build EZMicroBalance.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build` passed after Ascension source guards were added.
- Subagent D diagnostics follow-up: `dotnet build EZMicroBalance.sln` succeeded with 0 warnings and 0 errors; the guard suite passed after one source-guard-shaped code adjustment.
- Subagent E guard refresh: release coverage guards now also check package drift, installed/staging/package hash parity, current-facing doc freshness, false art claims, source-declared localization keys, Ascension selector constraints, and unsupported-system completion claims.
- Current bounded `--force-steam off` smoke after the v0.105.0/BaseLib v3.1.2 package refresh initialized only BaseLib and EZ Micro Balance, reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored the temporary profile settings byte-for-byte.
- Rootblight I/II/III and Blight Sprout are implemented for A14/A15/A18 after the current standard-lobby selector expansion.
- Firemarked Elite, Forge Token heal/smith payout, Fission, Banner Rooms, source-guarded Boss Royal Seals, and A20 vanilla double-boss map path/Brand/recovery/reward hooks are implemented for A12/A13/A16/A19/A20 after the current standard-lobby selector expansion. Forge Token special rest-site action payout is disabled until a safe runtime API is proven.
- A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds late route rows by act: Act 1 +1, Act 2 +1, Act 3 +2. A11 ordinary route nodes do not receive a dedicated marker, icon, or hover tooltip. A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available. A20 adds a Boss 1 reward-screen intermission prompt and a fixed courtyard event through the vanilla terminal-reward path; a bespoke full-screen intermission remains deferred.
- Read-only diagnostics are implemented behind `EZMB_ASCENSION_DIAGNOSTICS=1`.
- No Ascension gameplay has been live-tested in game yet.

## Gate Controls

- Default private-beta multiplayer test candidate: no Ascension environment variables are needed. A11-A20 selection is now default-on in the original single-player and host-multiplayer Ascension UI, and run-state level gates activate the implemented slices.
- Gate-off comparison: set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` before launch to restore vanilla A1-A10 selection for comparison.
- Multiplayer-only disable comparison: set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` before launch to disable only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- Legacy-compatible opt-in: `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is accepted but no longer required.
- A11 Wide Tower, Long Road / 宽塔长路 map-shape test: set `EZMB_ASCENSION_DEBUG_LEVEL=11` or select A11+ from the original single-player UI.
- A12 firemarked elite and Forge Token: set `EZMB_ASCENSION_DEBUG_LEVEL=12`.
- A13 Fission rewards: set `EZMB_ASCENSION_DEBUG_LEVEL=13`.
- A14 Rootblight Begins internal test: set `EZMB_ASCENSION_DEBUG_LEVEL=14` before launching the game.
- A15 boss Blight Sprout internal test: set `EZMB_ASCENSION_DEBUG_LEVEL=15`.
- A16 Banner Rooms: set `EZMB_ASCENSION_DEBUG_LEVEL=16`.
- A17 Deep Branch route test: set `EZMB_ASCENSION_DEBUG_LEVEL=17` or select A17+ from the original single-player UI. Use single-player; multiplayer branch insertion is skipped until route voting is proven.
- A18 elite Blight Sprout internal test: set `EZMB_ASCENSION_DEBUG_LEVEL=18`.
- A19 Boss Seal source-guarded runtime hooks and fourth boss reward option: set `EZMB_ASCENSION_DEBUG_LEVEL=19`.
- A20 vanilla double-boss path, Boss 2 Brand metadata/parameters, Boss 1 recovery, Boss 1 reward, Boss 1 reward-screen prompt, and fixed courtyard event: set `EZMB_ASCENSION_DEBUG_LEVEL=20`; this uses the vanilla second-boss map path and inserts the courtyard through the vanilla terminal reward/map pause.
- Disable A11 map geometry for comparison: set `EZMB_ASCENSION_ENABLE_MAP_GEOMETRY=0`.
- Disable A17 Deep Branches for comparison: set `EZMB_ASCENSION_ENABLE_DEEP_BRANCHES=0`.
- Read-only hook/state diagnostics: set `EZMB_ASCENSION_DIAGNOSTICS=1`.
- Controlled smoke passed is not the same as normal Steam-client Mod Settings or live co-op verification.

## Read-Only Ascension Diagnostics

Execute with `EZMB_ASCENSION_DIAGNOSTICS=1` and no gameplay debug level unless the test case explicitly needs Rootblight behavior.

- [ ] Game loads with BaseLib v3.1.2 and EZ Micro Balance enabled.
- [ ] Starting a normal run with only diagnostics enabled does not add Rootblight.
- [ ] `godot.log` records run Ascension, act index, debug/public gate state, Rootblight level/card counts, room type, round, and combat Blight Sprout counts.
- [ ] Diagnostics logs appear from the run/combat hook path without mutating gameplay beyond the selected Ascension level.
- [ ] No deck, map, reward, rest-site, boss-flow, or progress state changes occur when only diagnostics are enabled.
- [ ] Diagnostics-only mode must not raise Rootblight from restored Blight Sprout cards.
- [ ] Diagnostics can be combined with `EZMB_ASCENSION_DEBUG_LEVEL=11` through `20` to inspect gated manual tests.

## A14 Rootblight MVP: Debug Gate Off

Execute only after Rootblight MVP is implemented.

- [x] Build succeeds after implementation.
- [x] Publish succeeds after localization/resources changed.
- [ ] Game loads with BaseLib v3.1.2 and EZ Micro Balance enabled.
- [ ] Starting a normal run with the debug/internal gate disabled does not add Rootblight.
- [ ] Existing Ancient reward rebalance behavior still loads and does not throw.
- [ ] `godot.log` has no EZ Micro Balance Ascension errors.

## A14 Rootblight MVP: Debug Gate On

Execute with `EZMB_ASCENSION_DEBUG_LEVEL=14`.

- [ ] Enable the documented debug/internal gate.
- [ ] Start a new single-player run.
- [ ] Rootblight I is added to the local player's master deck.
- [ ] Rootblight I has cost 2, Curse type, no target, the intended title/description, and Exhaust/remove text in English.
- [ ] Rootblight localization displays in Simplified Chinese when the game language is Simplified Chinese.
- [ ] Save and load the run before combat; Rootblight does not duplicate.
- [ ] Save/load or re-enter Act 1 after Rootblight has been cleared; Rootblight is not re-added.
- [ ] Enter combat; the combat Rootblight copy links to the master-deck Rootblight card.
- [ ] Playing Rootblight exhausts the combat copy.
- [ ] Playing Rootblight I removes its master-deck card and leaves no replacement after combat.
- [ ] Playing Rootblight II removes its master-deck card and adds Rootblight I after combat.
- [ ] Playing Rootblight III removes its master-deck card and adds Rootblight II after combat.
- [ ] Leaving Rootblight I/II unplayed upgrades it after combat; ignored Rootblight III stays III and adds one Rootblight I only the first time that specific card grows.
- [ ] Rootblight cards added during combat-end resolution do not grow again until the next combat.
- [ ] If Rootblight is discarded normally, Rootblight level and the master-deck card remain unchanged.
- [ ] If Rootblight is exhausted by a non-play effect, Rootblight level and the master-deck card remain unchanged.
- [ ] Card removal screens/events can clear Rootblight if they use normal deck-removal APIs.
- [ ] Rootblight is not marked Eternal and is removable.
- [ ] Starting another run does not leak Rootblight state from the prior run.

## Rootblight MVP: Rest-Site Cleanup

Execute only if rest cleanup is included in the approved MVP slice.

- [ ] Resting with Rootblight in the deck removes exactly one highest-stage Rootblight card; it does not clear all Rootblight.
- [ ] Smithing with Rootblight in the deck does not clear Rootblight.
- [ ] Special rest-site actions either trigger documented fallback cleanup or explicitly do not, with no errors.
- [ ] In multiplayer, cleanup affects only the player who selected the option unless otherwise documented.
- [ ] Rest cleanup does not interfere with Ancient rest-site behavior.

## Rootblight MVP: Multiplayer Smoke

Execute before private beta if multiplayer is supported for the slice.

- [ ] Start or join a two-player run with the gate enabled.
- [ ] Each player gets only their own Rootblight state/card.
- [ ] One player playing Rootblight downgrades only that player's Rootblight state/card.
- [ ] One player removing Rootblight by card-removal APIs does not affect the other player's deck.
- [ ] Knockout/revive behavior does not clear permanent Rootblight unless documented.
- [ ] No multiplayer desync or ownership warnings appear in logs.

If multiplayer is not smoke-tested, mark the feature as single-player verified only in release notes.

## A15 Boss Blight Sprout MVP

Execute with `EZMB_ASCENSION_DEBUG_LEVEL=15` after A14 Rootblight behavior is verified.

- [ ] Act 1 boss combat does not add Blight Sprout.
- [ ] Act 2 and Act 3 boss combat adds two temporary Blight Sprouts to the relevant discard pile.
- [ ] Blight Sprout does not appear in opening hand unless drawn naturally.
- [ ] Boss Blight Sprouts sprout on rounds 3 and 4; each moves to the top of the draw pile if it has not entered hand.
- [ ] If Blight Sprout enters hand and is not played before combat end, one Rootblight I is added to the master deck.
- [ ] If Blight Sprout is played before combat end, Rootblight does not increase.
- [ ] If combat ends before Blight Sprout enters hand, Blight Sprout withers and does not raise Rootblight.
- [ ] Temporary Blight Sprout never persists in the master deck or save.
- [ ] Rootblight card count never exceeds 4; additional Blight Sprout growth shows the `根系已满。` / `Root system full.` notice instead of adding a fifth Rootblight.
- [ ] Save/load or re-enter the boss combat after Blight Sprout was seeded; the combat still has at most one Blight Sprout per active player.
- [ ] Knockout/revive should not raise Rootblight from that combat's Blight Sprout.
- [ ] Multiplayer targeting follows the source-design caps.

## A18 Elite Blight Sprout MVP

Execute with `EZMB_ASCENSION_DEBUG_LEVEL=18` after A15 boss Blight Sprout behavior is verified.

- [ ] Elite combat adds one temporary Blight Sprout to the relevant discard pile.
- [ ] Act 1 elites do not add Blight Sprout.
- [ ] Elite Blight Sprout uses the same sprout, play-prevention, wither, and growth rules as boss Blight Sprout.
- [ ] Normal monster combats do not add Blight Sprout.
- [ ] Elite Blight Sprout does not stack with another Blight Sprout source in the same combat.
- [ ] Save/load or re-enter the elite combat after Blight Sprout was seeded; the combat still has at most one Blight Sprout per active player.

## A11 Wide Tower, Long Road / 宽塔长路
Gated implementation present; live testing pending. Execute by selecting A11+ in the original single-player UI or with `EZMB_ASCENSION_DEBUG_LEVEL=11`.

- [ ] Gate logs that map width expanded by 1 column and that the act-specific late route row count was applied.
- [ ] Map width increases from 7 to 8 columns.
- [ ] At least one reachable optional node appears in the inserted width column.
- [ ] Act 1 map length increases by exactly 1 late route row before the boss rest row.
- [ ] Act 2 map length increases by exactly 1 late route row before the boss rest row.
- [ ] Act 3 map length increases by exactly 2 late route rows before the boss rest row.
- [ ] Every starting path can still reach the boss.
- [ ] The inserted late row contains normal route rooms, not boss/start nodes.
- [ ] No A11-specific marker, icon, or hover tooltip appears on ordinary route nodes.
- [ ] Save/load preserves the widened map, inserted late rows, and route edges.
- [ ] Map UI renders all rows without overlapping the boss, starting point, or route lines.

## A11-A20 Host-Multiplayer Selection

Default-on implementation present; live co-op testing pending. Use `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the full two-PC matrix and result template.

- [ ] With no Ascension env var, single-player selection can reach A11-A20 from the original Ascension UI.
- [ ] With no Ascension env var, host-multiplayer selection can reach A11-A20 from the original Ascension UI.
- [ ] With `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, single-player and multiplayer host selection remain capped by vanilla A1-A10 progress.
- [ ] With `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, host-multiplayer selection returns to the vanilla cap while single-player A11-A20 selection remains available.
- [ ] With legacy `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` set and no disable env vars, selection behavior remains the same as default-on.
- [ ] A11-A20 host selection does not persist to `PreferredMultiplayerAscension` after leaving the lobby.
- [ ] A11-A20 host selection survives a client joining the lobby without being clamped back to A10.
- [ ] A client sees the host-selected A11-A20 value.
- [ ] Gate off via `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`: single-player and multiplayer selection remain normal A1-A10.
- [ ] Gate default-on: host multiplayer can select A11-A20.
- [ ] Disable multiplayer selection env var returns host multiplayer to the vanilla cap.
- [ ] Client join does not clamp host A11-A20 selection back to A10.
- [ ] Host creates a multiplayer lobby, selects A20 before any client joins, and `godot.log` immediately records the A20 development-testing downgrade warning.
- [ ] After a client joins without changing Ascension, starting the A20 run records the A20 development-testing downgrade warning again.
- [ ] Multiplayer A11 starts with widened/longer map geometry and no A11 marker/tooltip on ordinary route nodes.
- [ ] Multiplayer A12 Firemarked Elite marker remains visible and host/client agree on the marked node.
- [ ] Multiplayer A16 Banner marker/hover remains visible and host/client agree on the marked node.
- [ ] Multiplayer A14/A15/A18 Rootblight and Blight Sprout state remains independently owned per player.
- [ ] Starting a two-player A11/A12/A14/A16 run reaches run load without ownership warnings, checksum divergence, or desync in `godot.log`.
- [ ] A20 multiplayer selection logs a clear limitation: multiplayer A20 selection is enabled for development testing; Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification; A11-A19 inherited systems may still apply if their gates are enabled.
- [ ] A20 multiplayer selection is not treated as full A20 co-op support.
- [ ] Starting a two-player A20 run does not silently apply single-player-only Dual King Brands behavior without warning.
- [ ] `godot.log` has no desync, checksum divergence, ownership, or multiplayer state warnings after the co-op pass.

## A12 Firemarked Elite and Forge Token

Gated implementation present; live testing pending. Execute with `EZMB_ASCENSION_DEBUG_LEVEL=12`.

- [ ] Act 1 selects 2 eligible firemarked elites, and Acts 2/3 select 3 when enough safe nodes exist.
- [ ] No two firemarked elites are on the same floor or directly adjacent.
- [ ] A greedy route can plan for 2 firemarked elites when route geometry allows it.
- [ ] Firemarked elite uses the dedicated red firemark indicator, not the generic quest marker used by Fur Coat / Spoils-style markers.
- [ ] Firemarked elite is visible before route commitment.
- [ ] Firemarked elite is not forced into the only route.
- [ ] Act 1 firemarked elite appears only after the first rest-site row.
- [ ] Combat shows one Firemark Host with the active firemark type as a visible enemy power.
- [ ] Might Mark grants only the host +2/+3/+4 Strength by act.
- [ ] Giant Mark increases only the host's max/current HP by 30%.
- [ ] Forge Armor Mark gives only the host 8/13/18 Block at the end of its turn by act.
- [ ] Constant Heal Mark heals only the host for 6/10/14 HP at the end of its turn by act.
- [ ] Firemarked Elite card rewards show one additional card option.
- [ ] Defeating firemarked elite grants one visible Forge Token status relic with counter 1.
- [ ] Forge Token hover text explains Rest, Smith, fallback heal, max-one cap, and random upgrade targeting, without claiming special-action payout.
- [ ] Forge Token cap of 1 is enforced.
- [ ] Duplicate Forge Token converts to gold.
- [ ] Forge Token after heal rest randomly upgrades one upgradable common/uncommon card, or fallback-heals if none exists.
- [ ] Heal rest option shows extra Forge Token text before selection.
- [ ] Forge Token after smith rest heals 7 HP and removes the visible token.
- [ ] Special rest-site actions do not spend Forge Token in this build and do not crash.
- [ ] Forge Token save/load behavior is stable.

## A13 Fission Enchantment

Gated implementation present; live testing pending. Execute with `EZMB_ASCENSION_DEBUG_LEVEL=13`.

- [ ] Fission appears only on eligible reward cards.
- [ ] Fission source rates are visibly plausible over repeated debug rolls: normal combat 25%, Banner Room 35%, Firemarked Elite 40%, Boss 15%.
- [ ] Each reward screen contains at most one Fission card.
- [ ] Fission does not appear on Powers, X-cost cards, star-cost cards, cards with Exhaust, cards that already exhaust on next play, quest/special cards, or incompatible cards.
- [ ] Cost reduction is correct.
- [ ] Exhaust behavior is correct after play.
- [ ] Fission has a non-missing enchantment icon.
- [ ] Tooltip/card text is correct in English and Simplified Chinese, uses energy-cost wording, does not show raw `{energyPrefix:energyIcons(...)}` templates, does not duplicate the added Exhaust line, and does not use the Chinese word "费用" for Fission.
- [ ] Rerolling card rewards does not duplicate or lose state incorrectly.
- [ ] Picked Fission cards save/load correctly.

## A16 Banner Rooms

Gated implementation present; live testing pending. Execute with `EZMB_ASCENSION_DEBUG_LEVEL=16`.

- [ ] Banner rooms are visible before route commitment.
- [ ] Banner room hover text names the rule before route commitment.
- [ ] Banner rooms do not stack with firemarked elites.
- [ ] Vanguard Banner grants enemies 2 temporary Strength and removes it at the start of round 3.
- [ ] Shield Formation Banner marks a non-minion bannerbearer and gives other enemies Block while the bannerbearer lives.
- [ ] Bounty Banner marks one target, grants 15 Gold as an extra room-end reward if killed before round 3 ends, and grants the missed-deadline Block/Artifact if not.
- [ ] Banners do not modify monster action tables.
- [ ] Banner modifiers apply only to the intended combat.
- [ ] Banner modifiers do not persist into later combats.
- [ ] Multiplayer target caps are respected.

## A17 Deep Branches

Gated implementation present; live testing pending. Execute in single-player by selecting A17+ in the original UI or with `EZMB_ASCENSION_DEBUG_LEVEL=17`. Keep A11 map geometry enabled for the primary test.

- [ ] Gate logs one optional 3-4 node Deep Branch with safe-route reconnect in Act 2.
- [ ] Gate logs one optional 3-4 node Deep Branch with safe-route reconnect in Act 3.
- [ ] No Deep Branch is inserted in Act 1.
- [ ] Each Deep Branch has exactly 3 or 4 nodes.
- [ ] Each Deep Branch includes at least one risk node and one enhanced reward node.
- [ ] The branch contains higher-risk rooms before the enhanced reward.
- [ ] The branch reconnects to an existing route after the reward node.
- [ ] A safer parallel route from the branch parent to reconnect remains available without entering the branch.
- [ ] Branch nodes are optional and do not replace all routes to the boss.
- [ ] Save/load preserves all branch nodes and edges, and metadata/markers restore after load.
- [ ] With `EZMB_ASCENSION_ENABLE_DEEP_BRANCHES=0`, A17 does not insert branch nodes.
- [ ] Multiplayer branch insertion is skipped until route voting is proven; no multiplayer route desync is introduced by this slice.

## A19/A20 Boss Systems

Gated implementation present as BossSeal definitions plus source-guarded runtime hooks; live testing pending. Execute A19 with `EZMB_ASCENSION_DEBUG_LEVEL=19`; execute A20 partial checks with `EZMB_ASCENSION_DEBUG_LEVEL=20` only after a second boss map point exists through vanilla/proven flow.

- [ ] A19 boss-specific Royal Seal metadata is assigned at map generation.
- [ ] The assigned Seal name matches the active boss encounter in `BossSealCatalog`.
- [ ] Boss combat logs the Royal Seal/Brand as armed with source-guarded evidence before applying only the currently guarded hook path.
- [ ] Boss map point hover text names Royal Seal or King Brand and includes the matching per-boss Royal Seal or Brand summary without raw localization keys.
- [ ] Boss card rewards improve as documented.
- [ ] Holy Daze, Martyr Oath, Ink Return, Startled Shell, Soul Tide, Boiling Critical, Misaligned Shell, Marginal Note, Struggle Bait, Aeonglass Strength, Chosen Decree, and Residual Sample each trigger only on the matching boss and remain documented as pending live verification. Aeonglass Strength should apply exactly +5 Strength to the `AEONGLASS` monster at combat start; no complex Brand/Seal mechanic is implemented for Aeonglass yet.
- [ ] No generic Armor/Rage/Barrier/Chaos placeholder effect applies.
- [ ] A20 creates the final-act second Boss through the vanilla double-boss map path when the A20 gate is active.
- [ ] The Act 3 map shows both Boss map points with vanilla Boss icons/names before route commitment.
- [ ] Boss 2 receives Brand metadata and Brand parameters.
- [ ] Boss 2 Brand parameters differ from A19 Royal Seal parameters where documented: Martyr Oath trigger cap/block, Ink Return restored Slippery/Strength, Startled Shell Plating/Soul Siphon reduction, Soul Tide Artifact/Beckon cap, Boiling Critical Steam threshold/warning Block, Misaligned Shell Block/Artifact, Marginal Note count, Struggle Bait timer Block, Aeonglass +5 Strength, Chosen Decree Queen/player Block, and Residual Sample first phase count.
- [ ] Boss 2 map point hover text warns that the stronger Brand is active.
- [ ] A20 Boss 1 reward screen offers one Boss card reward before the second Boss.
- [ ] Boss 1 reward screen shows the A20 intermission header and second-Boss proceed text.
- [ ] Boss 1 reward screen opens the A20 courtyard event before the second Boss.
- [ ] Boss 1 post-combat recovery restores 25% of missing HP before Boss 2.
- [ ] The fixed A20 courtyard event displays the second Boss name, Brand name, and Brand summary without raw localization keys.
- [ ] Save/load from the fixed A20 courtyard event returns to the courtyard or the Boss 2 map path without duplicate Boss 1 rewards.
- [ ] A full custom intermission screen after Boss 1 remains deferred. The current gated A20 slice uses a default-layout event room inserted from the vanilla terminal reward/map pause between Boss 1 and Boss 2.
- [ ] Boss 2 flow and victory/defeat/end-run flow remain unmodified unless vanilla already owns the second-boss route.

## Disable and Uninstall

Execute before private beta release.

- [ ] Disable EZ Micro Balance and confirm the game reaches main menu.
- [ ] Re-enable EZ Micro Balance and confirm current supported saves behave as documented.
- [ ] Remove only EZ Micro Balance from mods folder and confirm BaseLib and other mods still load.
- [ ] Confirm no official game assets were copied into the repository.
- [ ] Confirm release notes list any unsupported multiplayer or Ascension-selection limitations.

## Release Artifact and Runtime Smoke Hygiene

Planning checks for the next release-engineering pass; do not mark these complete without running the commands on the current artifacts.

- [ ] In a clean clone or clean workspace, normal `dotnet test EZMicroBalance.sln --no-build` passes without ignored publish artifacts; release artifact/runtime evidence tests are skipped by `ReleaseArtifactFactAttribute`.
- [ ] Release artifact parity tests run only after the documented publish/package refresh sequence and with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [ ] Publish the current package before runtime smoke.
- [ ] Launch controlled `--force-steam off` with only BaseLib and EZ Micro Balance enabled.
- [ ] Inspect `godot.log` and record the current SavedSpireField count; current source defines 12 fields and the latest controlled smoke reported 12. Rerun this check after future SavedSpireField, source, package, or BaseLib changes.
- [ ] Confirm the controlled smoke has no EZ Micro Balance startup exception or error.
- [ ] Keep normal Steam-client Mod Settings verification separate from controlled smoke.
