# Ascension 11-20 Manual Test Checklist

Project: Spire Plus (`EZMicroBalance` manifest id)
Manifest id: EZMicroBalance  
Status: checklist for A11-A20 single-player private-beta testing. After the 2026-05-25 co-op crash logs, host-multiplayer A11-A20 selection and gameplay fail closed by default unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging. Current 30-field normal Steam-client startup/log verification, historical limited A11 map spot checks, historical A11 saved-map boss-reachability graph proof, and historical targeted A14 Rootblight English/ZHS hover/notice spot checks exist. The 2026-05-14 A11 source-boundary patch now has source-level optional inserted-column route proof, but still needs fresh live visible width/row, route-click, save-load, and co-op verification; live Ascension gameplay not executed yet for this pass.
Last updated: 2026-05-23

## Research-Mode Baseline

- [x] Read `AGENTS.md`.
- [x] Read `docs/features/ascension-11-20/source-design.md`.
- [x] Inspect current Spire Plus architecture under technical id `EZMicroBalance`.
- [x] Run `git status --short --branch`.
- [x] Check whether `SlayTheSpire2.exe` is running before build.
- [x] Run `dotnet build EZMicroBalance.sln`.
- [x] Historical bounded `--force-steam off` smoke verified `EZMicroBalance` loaded after Ascension implementation; historical controlled smoke and normal Steam-client startup/log verification passed for earlier Spire Plus package states.
- [x] Inspect `godot.log` after Ascension implementation for startup errors.

Baseline result on 2026-05-06:

- `SlayTheSpire2.exe` was not running.
- `dotnet build EZMicroBalance.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build` passed after Ascension source guards were added.
- Subagent D diagnostics follow-up: `dotnet build EZMicroBalance.sln` succeeded with 0 warnings and 0 errors; the guard suite passed after one source-guard-shaped code adjustment.
- Subagent E guard refresh: release coverage guards now also check package drift, installed/staging/package hash parity, current-facing doc freshness, false art claims, source-declared localization keys, Ascension selector constraints, and unsupported-system completion claims.
- Historical beta.17 normal Steam-client helper startup/log verification under `.tools/runtime-evidence/beta17-loader-smoke-20260525-194311` initialized only previous package and Spire Plus, registered config, reported `Found 30 previous saved-state registrations`, reached startup completion, found 0 release-blocking signatures, restored settings/moved mods, and left 0 `SlayTheSpire2` processes. Previous beta.93 RitsuLib-only Off/AdditiveBatch1 loader evidence is under `.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`; live Ascension gameplay still needs fresh proof.
- Earlier 2026-05-13 controlled and normal startup/log passes reported `Found 16 previous saved-state registrations`; those are historical for the prior field-count state and are superseded by the historical 22-field smoke plus the current 30-field source state.
- Rootblight I/II/III and Blight Sprout are implemented for A14/A15/A18 after the current standard-lobby selector expansion.
- Firemarked Elite, Forge Token heal/smith payout, Fission, Banner Rooms, source-guarded boss dedicated abilities, and A20 vanilla double-boss map path/Branded Form/recovery/reward hooks are implemented for A12/A13/A16/A19/A20 after the current standard-lobby selector expansion. Forge Token special rest-site action payout is disabled until a safe runtime API is proven.
- A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds late route rows by act: Act 1 +1, Act 2 +1, Act 3 +2. A11 ordinary route nodes do not receive a dedicated marker, icon, or hover tooltip. The current source also patches `ActModel.CreateMap` as an earlier geometry boundary before the run hook. A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available. A20 adds a Boss 1 reward-screen intermission prompt and a fixed courtyard event through the vanilla terminal-reward path; a bespoke full-screen intermission remains deferred.
- Read-only diagnostics are implemented behind `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`.
- Full Ascension gameplay has not been live-tested yet beyond the A11 map spot checks, saved-map boss-reachability graph proof, and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks.

## Gate Controls

- Default single-player private-beta test path: no Ascension environment variables are needed. A11-A20 selection is default-on in the original single-player Ascension UI.
- Co-op safety gate: host-multiplayer A11-A20 selection and gameplay fail closed by default. Set `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` only for deliberate two-client debugging of unverified co-op behavior.
- Gate-off comparison: set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` before launch to restore vanilla A1-A10 selection for comparison.
- Multiplayer-only disable comparison: set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` before launch to disable only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- Legacy-compatible opt-in: `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is accepted but no longer required.
- A11 Wide Tower, Long Road / 宽塔长路 map-shape test: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=11` or select A11+ from the original single-player UI.
- A12 firemarked elite and Forge Token: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=12`.
- A13 Fission rewards: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=13`.
- A14 Rootblight Begins internal test: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=14` before launching the game.
- A15 boss Blight Sprout internal test: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=15`.
- A16 Banner Rooms: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=16`.
- A17 Deep Branch route test: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=17` or select A17+ from the original single-player UI. Use single-player; multiplayer branch insertion is skipped until route voting is proven.
- A18 elite Blight Sprout internal test: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=18`.
- A19 dedicated ability source-guarded runtime hooks and fourth boss reward option: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=19`.
- A20 vanilla double-boss path, Boss 2 Branded Form metadata/parameters, Boss 1 recovery, Boss 1 reward, Boss 1 reward-screen prompt, and fixed courtyard event: set `SPIREPLUS_ASCENSION_DEBUG_LEVEL=20`; this uses the vanilla second-boss map path and inserts the courtyard through the vanilla terminal reward/map pause.
- Disable A11 map geometry for comparison: set `SPIREPLUS_ASCENSION_ENABLE_MAP_GEOMETRY=0`.
- Disable A17 Deep Branches for comparison: set `SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES=0`.
- Read-only hook/state diagnostics: set `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`.
- Normal Steam-client Mod Settings has separate RC1 evidence; controlled smoke passed is not the same as live co-op verification.

## Live Evidence Protocol

Use this protocol for A11-A20 live evidence, especially Rootblight/Blight Sprout visual checks, combat-end behavior, map traversal screenshots, save/load rows, and co-op rows. Covered desktop captures, wrong-surface captures, or sessions that never reach the target game surface do not satisfy Rootblight, Ascension, or gameplay rows.

- [ ] Prepare a restore-safe normal Steam session:
  `scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch`
- [ ] Record the evidence directory printed in `session-state.json`.
- [ ] Before every gameplay screenshot batch, require foreground confirmation:
  `scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\window-preflight.json -RequireSpireForeground`
- [ ] If the preflight exits nonzero, do not capture or count screenshots; bring Slay the Spire 2 foreground first and rerun the preflight.
- [ ] Copy the live `godot.log` into the evidence directory after the gameplay row being tested.
- [ ] Audit the copied log:
  `scripts/audit-godot-log.ps1 -Path <evidence-dir>\godot.log -OutFile <evidence-dir>\godot-log-audit.json -FailOnHit`
- [ ] Restore the machine state after any run-start, save/load, continue, or co-op-host test:
  `scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore`
- [ ] Confirm restore output reports settings and moved mods restored, and any test-created `current_run*` files are preserved inside the evidence directory before the original current run is restored.

## Read-Only Ascension Diagnostics

Execute with `SPIREPLUS_ASCENSION_DIAGNOSTICS=1` and no gameplay debug level unless the test case explicitly needs Rootblight behavior.

- [ ] Game loads with STS2-RitsuLib v0.4.34 and Spire Plus enabled, with only STS2-RitsuLib as the shared runtime dependency.
- [ ] Starting a normal run with only diagnostics enabled does not add Rootblight.
- [ ] `godot.log` records run Ascension, act index, debug/public gate state, Rootblight level/card counts, room type, round, and combat Blight Sprout counts.
- [ ] Diagnostics logs appear from the run/combat hook path without mutating gameplay beyond the selected Ascension level.
- [ ] No deck, map, reward, rest-site, boss-flow, or progress state changes occur when only diagnostics are enabled.
- [ ] Diagnostics-only mode must not raise Rootblight from restored Blight Sprout cards.
- [ ] Diagnostics can be combined with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=11` through `20` to inspect gated manual tests.

## A14 Rootblight MVP: Debug Gate Off

Execute only after Rootblight MVP is implemented.

- [x] Build succeeds after implementation.
- [x] Publish succeeds after localization/resources changed.
- [ ] Game loads with STS2-RitsuLib v0.4.34 and Spire Plus enabled, with only STS2-RitsuLib as the shared runtime dependency.
- [ ] Starting a normal run with the debug/internal gate disabled does not add Rootblight.
- [ ] Existing Ancient reward rebalance behavior still loads and does not throw.
- [ ] `godot.log` has no Spire Plus Ascension errors under technical id `EZMicroBalance`.

## A14 Rootblight MVP: Debug Gate On

Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=14`.

- [ ] Enable the documented debug/internal gate.
- [ ] Start a new single-player run.
- [ ] Rootblight I is added to the local player's master deck.
- [ ] Rootblight I has cost 2, Curse type, no target, the intended title/description, one visible Exhaust keyword, no duplicate `Play: Exhaust` body text, and a Rootblight II hover preview in English.
- [ ] Rootblight II has one visible Exhaust keyword, no duplicate `Play: Exhaust` body text, and Rootblight I / Rootblight III hover previews.
- [ ] Rootblight III has one visible Exhaust keyword, no duplicate `Play: Exhaust` body text, and Rootblight I / Rootblight II hover previews.
- [ ] Rootblight card descriptions render `[gold]` card-name markup correctly and do not show raw tags.
- [ ] Rootblight localization displays in Simplified Chinese when the game language is Simplified Chinese.
- [ ] Simplified Chinese Rootblight descriptions have one visible `消耗` keyword, no duplicate `打出：消耗` body text, and render `[gold]根蚀 I/II/III[/gold]` without raw tags.
- [ ] The player sees the localized `[gold]Rootblight[/gold] added.` / `[gold]根蚀[/gold]已加入。` notice when Rootblight is added to the master deck.
- [ ] Save and load the run before combat; Rootblight does not duplicate.
- [ ] Save/load or re-enter Act 1 after Rootblight has been cleared; Rootblight is not re-added.
- [ ] Enter combat; the combat Rootblight copy links to the master-deck Rootblight card.
- [ ] Playing Rootblight exhausts the combat copy.
- [ ] Playing Rootblight I removes its master-deck card and leaves no replacement after combat.
- [ ] Playing Rootblight II removes its master-deck card and adds Rootblight I after combat.
- [ ] Playing Rootblight III removes its master-deck card and adds Rootblight II after combat.
- [ ] Leaving Rootblight I/II unplayed upgrades that card after combat.
- [ ] Leaving Rootblight III unplayed keeps it at III; the first time only, it adds one Rootblight I while remaining III.
- [ ] A Rootblight III lineage that already split once keeps its hidden split marker through Rootblight III -> II -> I downgrades, so it cannot split again after growing back to III.
- [ ] Rootblight IV never appears.
- [ ] Each Rootblight lineage can split at most once.
- [ ] If the four-card cap blocks a Rootblight III split, the failed add does not consume that card's split marker; it may split once after there is room.
- [ ] The master deck never has more than 4 Rootblight cards; further additions show `Root system full: max [blue]4[/blue] [gold]Rootblights[/gold].` / `根系已满：最多[blue]4[/blue]张[gold]根蚀[/gold]。`.
- [ ] Rootblight cards added during combat-end resolution do not grow again until the next combat.
- [ ] If Rootblight is discarded normally, Rootblight level and the master-deck card remain unchanged.
- [ ] If Rootblight is exhausted by a non-play effect, Rootblight level and the master-deck card remain unchanged.
- [ ] Card removal screens/events can clear Rootblight if they use normal deck-removal APIs.
- [ ] Rootblight is not marked Eternal and is removable.
- [ ] Starting another run does not leak Rootblight state from the prior run.

Targeted normal Steam-client spot checks already executed without `SPIREPLUS_ASCENSION_DEBUG_LEVEL`:

- English hover/text evidence: `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` captured Rootblight I/II/III and Blight Sprout with one visible Exhaust keyword, no raw `[gold]` tags, and expected Rootblight previews. The same directory also captures the English A14 Neow starter Rootblight-added notice with deck count 11.
- ZHS hover/text evidence: `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516` captured Rootblight I/II/III and Blight Sprout with one visible Exhaust keyword, no raw `[gold]` tags, and expected Rootblight previews.
- ZHS starter notice evidence: `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455\07-run-start-06.png` shows the localized Rootblight-added notice at Neow with the starter deck at 11 cards after selecting A14 through the live UI.
- Source status after the final notice hardening: combat-end additions now prefer a top-level overlay notice that ignores mouse input, uses high z-order, displays for 5 seconds, and falls back to the run global UI container. Pre-final-hardening evidence under `.tools\runtime-evidence\rootblight-combat-end-overlay-eng-20260509-053834` showed the Rootblight III split notice above the loot/pause overlay, but this is not a full manual pass.
- Still pending for this section: save/load/duplicate checks, play/unplayed combat-end behavior, clean non-paused combat-end add notices from Rootblight III and Blight Sprout, removal/rest behavior, and co-op ownership/desync checks.

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

Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=15` after A14 Rootblight behavior is verified.

- [ ] Act 1 boss combat does not add Blight Sprout.
- [ ] Act 2 and Act 3 boss combat adds two temporary Blight Sprouts to the relevant discard pile.
- [ ] Blight Sprout does not appear in opening hand unless drawn naturally.
- [ ] Boss Blight Sprouts sprout on rounds 3 and 4; each moves to the top of the draw pile if it has not entered hand.
- [ ] If Blight Sprout enters hand and is not played before combat end, one Rootblight I is added to the master deck, capped by the 4-card Rootblight limit.
- [ ] Blight Sprout has one visible Exhaust keyword, no duplicate `Play: Exhaust` / `打出：消耗` body text, gold-highlighted Draw Pile / `抽牌堆` text, and a Rootblight I hover preview.
- [ ] Seen-but-unplayed Blight Sprout shows the localized rich-text Rootblight added notice when it adds Rootblight I after combat.
- [ ] If Blight Sprout is played before combat end, including before a Boss victory, Rootblight does not increase.
- [ ] If Blight Sprout enters hand and is discarded or exhausted by a non-play effect, it still adds Rootblight I after combat.
- [ ] If combat ends before Blight Sprout enters hand, Blight Sprout withers and does not raise Rootblight.
- [ ] Temporary Blight Sprout never persists in the master deck or save.
- [ ] Rootblight card count never exceeds 4; additional Blight Sprout growth shows the rich-text max-4 root system notice instead of adding another Rootblight.
- [ ] Save/load or re-enter the boss combat after Blight Sprout was seeded; the combat still has at most one Blight Sprout per active player.
- [ ] Save/load or re-enter the boss combat normalizes kept Blight Sprouts to the source-correct round sequence: first bud round 3, second bud round 4, and no extra buds beyond two.
- [ ] Knockout/revive should not raise Rootblight from that combat's Blight Sprout.
- [ ] Multiplayer targeting follows the source-design caps.

## A18 Elite Blight Sprout MVP

Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=18` after A15 boss Blight Sprout behavior is verified.

- [ ] Elite combat adds one temporary Blight Sprout to the relevant discard pile.
- [ ] Act 1 elites do not add Blight Sprout.
- [ ] Act 2 elites in the first 3 route rows do not add Blight Sprout.
- [ ] Elite Blight Sprout uses the same sprout, play-prevention, wither, and growth rules as boss Blight Sprout.
- [ ] Normal monster combats do not add Blight Sprout.
- [ ] Elite Blight Sprout does not stack with another Blight Sprout source in the same combat.
- [ ] Save/load or re-enter the elite combat after Blight Sprout was seeded; the combat still has at most one Blight Sprout per active player.
- [ ] Save/load or re-enter the elite combat normalizes the kept Blight Sprout to round 3 and does not create extras beyond one.

## A11 Wide Tower, Long Road / 宽塔长路
Gated implementation present. RC1 normal Steam-client spot checks executed by selecting A11 through the original single-player UI; Act 1 map/save-load and Act 2/3 DevConsole map-surface observations passed for the prior package. The current 2026-05-14 source patch also hooks `ActModel.CreateMap` before the run hook and logs target/actual rows, columns, inserted-column route evidence, original-route-preserved evidence, and inserted-column route-choice count. Fresh current-build manual proof is still required because the reported real-play symptom was "not visibly longer/wider."

- [ ] Current build log records the `ActModel.CreateMap` or run-map-hook source-boundary check with `columns=8/8`, target rows reached, `insertedColumnRoute=True`, `originalRoutePreserved=True`, and `insertedColumnRouteChoices>=1`.
- [ ] Map width increases from 7 to 8 columns. In the current build, it is visibly wider than a matched A10/control map.
- [ ] Act 1 visible route rows increase by 1, Act 2 visible route rows increase by 1, and Act 3 visible route rows increase by 2.
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

Default fail-closed implementation present; live co-op debugging must be deliberate. Use `docs/features/ascension-11-20/multiplayer-test-runbook.md` for the full two-PC matrix and result template.

- [ ] With no Ascension env var, single-player selection can reach A11-A20 from the original Ascension UI.
- [ ] With no Ascension env var, host-multiplayer selection remains capped by vanilla A1-A10 and logs the co-op gameplay gate.
- [ ] With `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, single-player and multiplayer host selection remain capped by vanilla A1-A10 progress.
- [ ] With `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, host-multiplayer selection returns to the vanilla cap while single-player A11-A20 selection remains available.
- [ ] With legacy `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` set and no disable env vars, single-player selection behavior remains the same as default-on.
- [ ] With `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` deliberately set, host-multiplayer selection can reach A11-A20 for focused two-client debugging only.
- [ ] A11-A20 host selection does not persist to `PreferredMultiplayerAscension` after leaving the lobby.
- [ ] A11-A20 host selection survives a client joining the lobby without being clamped back to A10.
- [ ] A client sees the host-selected A11-A20 value.
- [ ] Gate off via `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1`: single-player and multiplayer selection remain normal A1-A10.
- [ ] Co-op default gate: host multiplayer cannot select A11-A20 unless the deliberate debugging override is set.
- [ ] Disable multiplayer selection env var returns host multiplayer to the vanilla cap.
- [ ] Client join does not clamp host A11-A20 selection back to A10.
- [ ] Host creates a multiplayer lobby, selects A20 before any client joins, and `godot.log` immediately records the A20 development-testing downgrade warning.
- [ ] After a client joins without changing Ascension, starting the A20 run records the A20 development-testing downgrade warning again.
- [ ] Multiplayer A11 starts with widened/longer map geometry and no A11 marker/tooltip on ordinary route nodes.
- [ ] Multiplayer A12 Firemarked Elite marker remains visible and host/client agree on the marked node.
- [ ] Multiplayer A16 Banner marker/hover remains visible and host/client agree on the marked node.
- [ ] Multiplayer A14/A15/A18 Rootblight and Blight Sprout state remains independently owned per player.
- [ ] Starting a two-player A11/A12/A14/A16 run reaches run load without ownership warnings, checksum divergence, or desync in `godot.log`.
- [ ] A20 multiplayer selection logs a clear limitation: multiplayer A20 selection is enabled for development testing; A20 Branded Form / second-boss enhanced dedicated ability gameplay is disabled or downgraded in co-op pending live verification; A11-A19 inherited systems may still apply if their gates are enabled.
- [ ] A20 multiplayer selection is not treated as full A20 co-op support.
- [ ] Starting a two-player A20 run does not silently apply single-player-only Branded Form behavior without warning.
- [ ] `godot.log` has no desync, checksum divergence, ownership, or multiplayer state warnings after the co-op pass.

## A12 Firemarked Elite and Forge Token

Gated implementation present; live testing pending. Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=12`.

- [ ] Act 1 selects 2 eligible firemarked elites, and Acts 2/3 select 3 when enough safe nodes exist.
- [ ] Across multiple fresh seeds/runs, Act 1's first Firemarked Elite is not always Might.
- [ ] Save and Continue from the same map; Firemarked Elite kind assignments do not change.
- [ ] Firemarked Elite map hover names the exact Firemark kind, summary, and any secondary-target overflow effect before route commitment.
- [ ] Firemarked Elite hover renders in English and Simplified Chinese without raw localization keys or raw rich-text tags.
- [ ] No two firemarked elites are on the same floor or directly adjacent.
- [ ] A greedy route can plan for 2 firemarked elites when route geometry allows it.
- [ ] Firemarked elite uses the dedicated red firemark indicator, not the generic quest marker used by Fur Coat / Spoils-style markers.
- [ ] Firemarked elite is visible before route commitment.
- [ ] Firemarked elite is not forced into the only route.
- [ ] At least one route to the boss remains available that avoids all selected Firemarked Elite candidates.
- [ ] Act 1 firemarked elite appears only after the first rest-site row.
- [ ] Combat shows one Firemarked Elite with the active firemark type as a visible enemy power.
- [ ] Might Mark grants only the marked enemy +1/+2/+4 Strength by act; unblocked attack damage builds Heat, and 2 Heat makes the next first attack deal +1/+2/+4 damage.
- [ ] Giant Mark increases only the marked enemy's max/current HP by +20%/+30%/+45% by act; dropping below half HP exposes Molten Core, which breaks after 20%/25%/30% original Max HP damage by act and removes 10% Max HP.
- [ ] Firemarked Elite picks one Firemark Host. Overflow affects at most one secondary non-summon enemy at a time and stops after the host dies.
- [ ] Forge Armor Mark gives only the host 8/14/24 Molten Armor at player turn start; if the host has no Block at turn end, the next armor gain is skipped, up to twice.
- [ ] Constant Heal Mark heals only the marked enemy for 4/8/16 HP at enemy turn end by act; dealing 18/36/72 damage before its next heal prevents that heal.
- [ ] Firemarked Elite card rewards show one additional card option.
- [ ] Defeating firemarked elite grants one visible Forge Token status relic with counter 1.
- [ ] Forge Token hover text explains Rest, Smith, fallback heal, max-one cap, and random upgrade targeting, without claiming special-action payout.
- [ ] Forge Token cap of 1 is enforced.
- [ ] Duplicate Forge Token converts to gold and the extra Firemarked Elite card reward option is upgraded when an upgradable candidate exists.
- [ ] Forge Token after heal rest randomly upgrades one upgradable common/uncommon card, or fallback-heals if none exists.
- [ ] Heal rest option shows extra Forge Token text before selection.
- [ ] Forge Token after smith rest heals 7 HP and removes the visible token.
- [ ] Special rest-site actions do not spend Forge Token in this build and do not crash; `SpecialRestSiteActionPayoutEnabled` remains `false` until a safe generic rest-option hook is runtime-proven.
- [ ] Forge Token save/load behavior is stable.

## A13 Fission Enchantment

Gated implementation present; live testing pending. Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=13`.

- [ ] Fission appears only on eligible reward cards.
- [ ] Fission source rates are visibly plausible over repeated debug rolls: normal combat 10%, Banner Room 15%, Firemarked Elite 20%, Boss 5%.
- [ ] Each reward screen contains at most one Fission card.
- [ ] Fission does not appear on Powers, X-cost cards, star-cost cards, original/current 0-cost cards, cards with Exhaust, cards that already exhaust on next play, quest/special/story cards, unmodifiable cards, or incompatible cards.
- [ ] Cost reduction is correct.
- [ ] Exhaust behavior is correct after play.
- [ ] Fission has a non-missing enchantment icon.
- [ ] Tooltip/card text is correct in English and Simplified Chinese, uses energy-cost wording, does not show raw `{energyPrefix:energyIcons(...)}` templates, does not duplicate the added Exhaust line, and does not use the Chinese word "费用" for Fission.
- [ ] Rerolling card rewards does not duplicate or lose state incorrectly.
- [ ] Picked Fission cards save/load correctly.
- [ ] With `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`, sample 20 normal combat reward screens and record source label, eligible candidate count, roll, applied count, and applied card id when present.
- [ ] With `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`, sample 10 Banner Room reward screens and record source label, eligible candidate count, roll, applied count, and applied card id when present.
- [ ] With `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`, sample 10 Firemarked Elite reward screens and record source label, eligible candidate count, roll, applied count, and applied card id when present.
- [ ] With `SPIREPLUS_ASCENSION_DIAGNOSTICS=1`, sample Boss reward screens and record source label, eligible candidate count, roll, applied count, and applied card id when present.
- [ ] Do not change Fission probabilities or add a pity counter unless the diagnostic sampling shows eligible reward droughts.

## A16 Banner Rooms

Gated implementation present; live testing pending. Execute with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=16`.

- [ ] Banner rooms are visible before route commitment.
- [ ] Across multiple fresh seeds/runs, Act 1's Banner Room is not always Vanguard.
- [ ] Save and Continue from the same map; Banner Room kind assignments do not change.
- [ ] Banner room hover text names the exact Banner kind and rule before route commitment.
- [ ] Banner Room hover renders in English and Simplified Chinese without raw localization keys or raw rich-text tags.
- [ ] Banner rooms do not stack with firemarked elites.
- [ ] Vanguard Banner grants enemies +1/+2/+4 temporary Strength by act and removes it at the start of round 3.
- [ ] Shieldwall Banner only has an effect in multi-enemy fights: one bannerbearer protects the other enemies for 3/7/14 Block each enemy turn, then gives them 5/10/20 Block when it dies. If the combat has one primary enemy, it converts to Blood Prize instead of doing nothing.
- [ ] Blood Prize Banner marks one target, grants 15/30/55 Gold by act if killed before round 3 ends, and gives lasting retaliation if missed: +1/+2/+4 Strength plus 1/1/2 Artifact, or half Strength rounded up to all primary enemies when the target is a support enemy.
- [ ] Pressing Line Banner starts counting from each player's 4th card each turn, caps at 3 layers per player, resolves only the highest 2 players in co-op, and gives enemies 4/8/16 or 6/12/24 Block plus +1/+2/+4 next-attack damage at 3 layers.
- [ ] Last Stand Banner only has an effect in multi-enemy fights: the first enemy death gives remaining enemies 6/12/24 Block and +1/+2/+4 temporary Strength next enemy turn. If the combat has one primary enemy, it converts to Blood Prize instead of doing nothing.
- [ ] Banners do not modify monster action tables.
- [ ] Banner modifiers apply only to the intended combat.
- [ ] Banner modifiers do not persist into later combats.
- [ ] Multiplayer target caps are respected.

## A17 Deep Branches

Gated implementation present; live testing pending. Execute in single-player by selecting A17+ in the original UI or with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=17`. Keep A11 map geometry enabled for the primary test.

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
- [ ] With `SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES=0`, A17 does not insert branch nodes.
- [ ] Multiplayer branch insertion is skipped until route voting is proven; no multiplayer route desync is introduced by this slice.

## A19/A20 Boss Systems

Gated implementation present as BossSeal definitions plus source-guarded runtime hooks; live testing pending. Execute A19 with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=19`; execute A20 partial checks with `SPIREPLUS_ASCENSION_DEBUG_LEVEL=20` only after a second boss map point exists through vanilla/proven flow.

- [ ] A19 boss-specific dedicated ability metadata is assigned at map generation.
- [ ] The assigned ability name matches the active boss encounter in `BossSealCatalog`.
- [ ] Boss map point hover previews the current dedicated ability name and summary before entering combat.
- [ ] Boss combat logs the dedicated ability / Branded Form as armed with source-guarded evidence before applying only the currently guarded hook path.
- [ ] Boss map point hover text names the dedicated ability or Branded Form and includes the matching per-boss summary without raw localization keys.
- [ ] Boss card rewards improve as documented.
- [ ] Attack-changing dedicated abilities show final enemy intent before damage resolves: Martyr Oath attacks, Claw Calibration attacks, Escape Fatigue Vigor attacks, and Branded Form Eye Lasers extra hit. Martyr Oath and Claw Calibration add damage to each hit of the next attack, so multi-hit intents must show the boosted per-hit value.
- [ ] Holy Daze triggers only for Ceremonial Beast's first stun: each hit is capped at 1 damage, then the Boss gains 1 Strength; Branded Form grants 2 Strength instead.
- [ ] Martyr Oath triggers only for The Kin: each of the two real followers can grant 1 Oath, up to 2. Oath extends the next debuff by 1 per stack or adds 3 damage per hit per stack to the next attack. Branded Form changes the hit bonus to 4 and grants exactly 1 Artifact if both followers die in one player turn.
- [ ] Ink Return triggers only for Vantom: the first full Slippery removal returns 25% of the cleared amount next enemy turn, min 3, max 12. Branded Form returns 35%, min 5, max 18.
- [ ] Plating Wake triggers only for Lagavulin Matriarch: player-hit wake grants 4 Plating, natural wake grants 8, and the first Soul Siphon halves current Plating. Branded Form changes this to 6/10 Plating and removes only one third. Multiplayer uses the game's boss Plating scaling and the visible final value is sensible.
- [ ] Soul Tide triggers only for Soul Fysh: entering Intangible grants exactly 1 Artifact. Beckons left in hand are counted before their turn-end in-hand damage resolves, then grant Block after Soul Fysh's turn so the Block is visible before the next player turn begins. A19 uses 2 Block each with team caps solo 8, 2 players 12, 3-4 players 16. Branded Form uses 3 Block each with caps solo 12, 2 players 16, 3-4 players 20.
- [ ] Unweakenable triggers only for Waterfall Giant's explosion turn: Weak and attack-down are cleared, the explosion ignores those attack reductions, the temporary Artifact protection does not persist after the explosion resolves, and affected players gain 1 Vulnerable. Branded Form applies 2 Vulnerable and does not raise base explosion damage.
- [ ] Claw Calibration triggers only for Kaiser Crab: if claw HP percentages differ by at least 35% at player turn end, the healthier claw gains Calibration. At 2 Calibration, that claw's next attack gains 4 damage per hit, once per claw. Branded Form uses a 30% threshold and 5 damage per hit.
- [ ] Marginal Note triggers only for Knowledge Demon: Curse of Knowledge adds temporary Marginal Notes to discard. Unplayed Notes become Deep Thought, capped at 2 and at most 2 gained per turn. Deep Thought adds side costs to the next Knowledge curse. Branded Form cap is 3, while Sloth and Waste Away side costs resolve at most once per Knowledge curse.
- [ ] Escape Fatigue triggers only for The Insatiable: Strength gain or Sandpit advance adds ability-made Frantic Escape to one affected player's discard. Every third ability-made Escape played by the team grants 2 Vigor, at most once each player turn. Branded Form grants 3 Vigor.
- [ ] Time Sand Reflow triggers only for Aeonglass: after Ebb, 2 shared Time Sand appears; each energy spent clears 1, and remaining Time Sand adds extra Wither to the next Increasing Intensity. Branded Form creates 3 Time Sand and gives Eye Lasers 1 extra hit when Time Sand remains, at most twice per fight.
- [ ] Royal Decree triggers only for Queen: one visible Bound card per active player receives Royal Decree, the fight continues without lockup or incorrect runtime state, playing it avoids penalty, playing another Bound card grants Majesty, and playing no Bound card grants Majesty plus Torch Head Strength. Majesty caps at 2, with per-round team caps. Branded Form Majesty cap is 3 and one defense action can spend at most 2.
- [ ] Experimental Record triggers only for Test Subject phase changes: the next phase receives one sample from Strength Residue, Skill Adaptation, Attack Adaptation, Antibody Sample, or Contaminated Sample, with a visible short notice. Branded Form receives two different samples.
- [ ] No dedicated ability applies to the wrong boss, persists into later combats, or uses generic placeholder Armor/Rage/Barrier/Chaos behavior.
- [ ] A20 creates the final-act second Boss through the vanilla double-boss map path when the A20 gate is active.
- [ ] The Act 3 map shows both Boss map points with vanilla Boss icons/names before route commitment.
- [ ] Boss 2 receives Branded Form metadata and parameters.
- [ ] Boss 2 Branded Form parameters differ from A19 dedicated ability parameters where documented: Martyr Oath attack bonus/same-turn Artifact, Ink Return final Slippery percentage, Plating Wake Plating/Soul Siphon reduction, Soul Tide Block cap, Unweakenable explosion Vulnerable, Claw Calibration threshold/damage, Deep Thought side-cost cap, Escape Fatigue Vigor, Time Sand/Wither/Eye Laser echo, Royal Decree Majesty cap, and Experimental Record sample count.
- [ ] Boss 2 map point hover text warns that Branded Form is active.
- [ ] Boss 2 map point hover shows the exact Branded Form name and summary before entering combat.
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

- [ ] Disable Spire Plus and confirm the game reaches main menu; technical id is `EZMicroBalance`.
- [ ] Re-enable Spire Plus and confirm current supported saves behave as documented; technical id is `EZMicroBalance`.
- [ ] Remove only `EZMicroBalance` from mods folder and confirm STS2-RitsuLib and other mods still load.
- [ ] Confirm no official game assets were copied into the repository.
- [ ] Confirm release notes list any unsupported multiplayer or Ascension-selection limitations.

## Release Artifact and Runtime Smoke Hygiene

Planning checks for the next release-engineering pass; do not mark these complete without running the commands on the current artifacts.

- [ ] In a clean clone or clean workspace, normal `dotnet test EZMicroBalance.sln --no-build` passes without ignored publish artifacts; release artifact/runtime evidence tests are skipped by `ReleaseArtifactFactAttribute`.
- [ ] Release artifact parity tests run only after the documented publish/package refresh sequence and with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`. The old `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` variable remains a compatibility alias, not the preferred command in new notes.
- [ ] Publish the current package before runtime smoke.
- [ ] Launch controlled `--force-steam off` with only STS2-RitsuLib and Spire Plus enabled.
- [x] Inspect `godot.log` and record the current RitsuLib saved-state registration count/shape; previous beta.93 RitsuLib-only Off/AdditiveBatch1 packets cover startup/log registration shape. Historical package smokes reported `Found 30 previous saved-state registrations` before the RitsuLib-only migration.
- [ ] Confirm the controlled smoke has no Spire Plus startup exception or error under technical id `EZMicroBalance`.
- [ ] Keep normal Steam-client Mod Settings verification separate from controlled smoke.
