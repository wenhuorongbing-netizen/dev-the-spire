# Ancient Expansion Pack v2.2 Source Design

Status: default-on Urda eleven-blessing source-complete test slice, default-on Morvi source-complete test slice, default-on Lotha source-complete test slice, and hidden-by-default source-dedicated Vakuu fight slice with Contract, Stolen Vault, and Blood Debt pressure. Live gameplay, save/load, and co-op verification remain pending.

Working title: **Sowing, Borrowing, and Judgment / 播种、借阅与审判**.

Readable current names: Forbidden Loan / 禁书借阅; Misprint Press / 错页印刷机; Overdue Library / 逾期书库; Paperstorm / 纸屑风暴; Blueprint Proofreading / 蓝图校样; Mirror Rebuttal / 反证之镜; Mirror Hall Echo / 镜厅回声; Closed Court / 终审封庭; Death Reprieve / 死刑缓期; Public Evidence / 公开罪证.

## 1. Design Goals

The v2.2 Ancient expansion should add clear, learnable high-stakes choices without turning rewards into opaque traps. Each Ancient should have a distinct player-facing promise:

| Ancient | Role | Player Question |
| --- | --- | --- |
| Urda, Loamweaver | Growth, map memory, seed rewards | How much long-term growth can I afford now? |
| Morvi, the Lender-Scribe | Borrowed power, debt, archive manipulation | When is short-term power worth future accounting? |
| Lotha, the Judge | Verdicts, evidence, one-turn rulings | Can I build a turn around a strict legal condition? |
| Vakuu fight option | Optional lethal challenge | Is a second Ancient blessing worth risking the run? |

The design should remain modular. Each blessing must be independently disableable during development, source-guarded, and live-tested before entering the active pool.

## 2. Global Rules

### Appearance Rules

- Do not replace the existing Ancient reward rebalance v4 behavior.
- Morvi is default-on for the current private-beta test slice, with preferred `SPIREPLUS_DISABLE_MORVI` plus legacy `EZMB_DISABLE_MORVI` emergency gates and force-test env vars.
- Lotha is default-on for the current private-beta test slice, with preferred `SPIREPLUS_DISABLE_LOTHA` plus legacy `EZMB_DISABLE_LOTHA` emergency gates and force-test env vars.
- Vakuu fight is hidden by default and requires preferred `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, a force-fight gate, or legacy `EZMB_ENABLE_VAKUU_FIGHT=1`. It also keeps preferred `SPIREPLUS_DISABLE_VAKUU_FIGHT` plus legacy `EZMB_DISABLE_VAKUU_FIGHT` rollback gates.
- Current Urda, Morvi, and Lotha are active expansion test slices. The single-player Vakuu fight is a source-dedicated opt-in slice only.
- Future Ancient additions must avoid silently changing existing run setup, reward, map, or save/load behavior.

### Punishment Principles

- The player should understand the cost before accepting the blessing.
- Punishment should follow from a visible decision, not from hidden enemy AI rewrites.
- Do not add unavoidable early-run death checks.
- Do not make card rewards worse without showing the compensation or tradeoff.
- Do not make unplayed temporary cards punish the player unless the card text says so clearly.

### Power-Card Safety Rule

Any extra-play, copy, reprint, verdict, echo, or replay effect may target Attack and Skill cards. Power cards are not copied, extra-played, or replayed by default. Use a replacement reward instead: temporary cost reduction, draw, energy, or waiting for the next Attack/Skill.

Detailed rules are in `card-and-power-safety-rules.md`.

## 3. Current Urda Slice

The current repository has a default-on private-beta Urda slice with eleven source-backed blessing ids. It remains live-pending and several new rows use narrower source-safe UI fallbacks.

Current source-backed blessings:

| Blessing | Id | Current State |
| --- | --- | --- |
| Seedbed | `urda_seedbed` | Source hook exists; live gameplay/save-load pending. |
| Humus Pact | `urda_humus_pact` | Source hook exists; live gameplay/save-load pending. |
| Molting | `urda_molting` | Source hook exists with `Withered Husk`; live gameplay/save-load pending. |
| Moss Map | `urda_moss_map` | Source hook exists; live gameplay/save-load pending. |
| Trial Branch | `urda_trial_branch` | Source hook exists with 4 rare-card grid, upgraded Trial Branch enchantment marker, and strict 3-combat/3-success settlement; missing any trial combat removes the card. Live gameplay/save-load pending. |
| Shallow-Root Relic | `urda_shallow_root_relic` | Source hook exists with 2 common relic choices, gold/rooting, and deterministic Act 2 removal/refund fallback; live gameplay/save-load pending. |
| Elite Root | `urda_elite_root` | Source hook exists; after each Elite combat, including Firemarked Elite combats, the selected player heals 10 HP. Live gameplay/save-load pending. |
| Rooted Route | `urda_rooted_route` | Source hook exists with automatic reachable normal-combat mark, no map graph mutation, success rewards, and wither fallback; live gameplay/save-load pending. |
| After the Rain | `urda_after_rain` | Source hook exists with Act 1 Rain Breath triggers and an Act 2 trigger-count payoff; live gameplay/save-load pending. |
| Root-Sight | `urda_root_sight` | Source hook exists with 5 Root Eyes, relic-click map selection, and stored previews for future reachable Monster/Unknown/Elite nodes; live gameplay/save-load pending. |
| Seed Bank | `urda_seed_bank` | Source hook exists with Store Seed reward alternative and relic-click extraction of up to 2 stored cards; live gameplay/save-load pending. |

## 4. Full Urda v2.2 Roadmap

Urda's active test slice now contains eleven blessings after the Elite Root addition. All eleven are source-backed for the test slice; live evidence is still required before any release-ready claim.

| Blessing | Planned Id | Status | Design Intent |
| --- | --- | --- | --- |
| Seedbed | `urda_seedbed` | Current source-backed slice | Trade max HP for a Seedbed that catches later Temporary negative cards and Blight Sprouts. |
| Humus Pact | `urda_humus_pact` | Current source-backed slice | Skip card rewards for gold/removal/upgraded-card payoff. |
| Molting | `urda_molting` | Current source-backed slice | Remove starter basics and accept temporary Husk clutter. |
| Moss Map | `urda_moss_map` | Current source-backed slice | First-time room-type memory rewards. |
| Trial Branch | `urda_trial_branch` | Current source-backed slice | Small early test that keeps an upgraded chosen card only after the player proves route discipline. |
| Shallow-Root Relic | `urda_shallow_root_relic` | Current source-backed slice | A common relic choice with rooting and a deterministic Act 2 fallback. |
| Elite Root | `urda_elite_root` | Current source-backed slice | A first-tier Urda reward that tempts elite routes by healing 10 HP after every Elite victory. |
| Rooted Route | `urda_rooted_route` | Current source-backed slice | Route commitment reward that never mutates the map graph. |
| After the Rain | `urda_after_rain` | Current source-backed slice | First unblocked enemy attack damage in each Act 1 combat adds Rain Breath; Act 2 grants 75 Gold if this triggered fewer than 3 times, otherwise heal 8 and upgrade 1 card. |
| Root-Sight | `urda_root_sight` | Current source-backed slice | source-safe automatic preview marking, not hidden power. |
| Seed Bank | `urda_seed_bank` | Current source-backed slice | Capped stored card value with player-triggered relic extraction. |

Future Urda work must keep the eleven active blessings stable unless a dedicated Urda refactor milestone says otherwise.

## 5. Morvi v2.2 Source-Complete Test Slice

Morvi is default-on. It appears in Act 2 unless `SPIREPLUS_DISABLE_MORVI=1` is set. `SPIREPLUS_FORCE_ANCIENT=MORVI` and `SPIREPLUS_FORCE_MORVI_BLESSING` support focused testing. Legacy `EZMB_*` aliases still work. It remains live-pending until reward UI, gameplay, save/load, and co-op checks pass.

| Blessing | Planned Id | Design Notes |
| --- | --- | --- |
| Forbidden Loan | `morvi_forbidden_loan` | Source-complete: choose from three class Ancient cards, add the upgraded chosen card with a Borrowed Ancient marker, charge HP on borrowed-card plays, and auto-settle after the Act 2 boss by paying 180 Gold if possible or removing the borrowed card otherwise. source-safe deviation: no post-boss choice UI is claimed. |
| Misprint Press | `morvi_misprint_press` | Source-complete: first player-played Attack or Skill each turn uses play-count modification on the original card, draws 1 when original/base cost is at least 1, and ignores Powers, Statuses, Curses, generated cards, autoplay, and recursive executions. |
| Red Ink Overdraft | `morvi_red_ink_overdraft` | Source-complete with UI deviation: a temporary 0-cost Overdraft action card is added at player-turn start only when hand space allows and can be played only once per turn at 0 Energy; combat end pays 12 Gold per debt or 3 nonlethal HP per unpaid debt. |
| Overdue Library | `morvi_overdue_library` | Source-complete: each combat adds three random temporary Archive Pages from Draw, Veil, Burn, Discount, Bravery, and Dexterity pages; unplayed pages carry no extra punishment. |
| Open-Book Exam | `morvi_open_book_exam` | Source-complete with tracking deviation: turn 1 draws up to five extra cards and gains 2 Energy; tracked Open Book cards still in hand at turn end are sealed through an Exhaust Pile holding path and return on turn 3 with cost 0 when hand space allows. Source now marks sealed cards so reload can attempt pile recovery, but live restore proof remains pending. |
| Paperstorm | `morvi_paperstorm` | Source-complete: shuffle four Waste Paper status cards into the Draw Pile; the first two Status cards drawn from the Draw Pile each turn are consumed for draw 1 and Energy 1. |
| Blueprint Proofreading | `morvi_blueprint_proof` | Source-complete: gain 3 Proofread stacks; first three non-Status, non-Curse player-played deck cards temporarily upgrade and draw 1 if unupgraded, or cost 1 less and grant 4 Block if already upgraded. Power cards are never extra-played. |
| Debt Settlement | `morvi_debt_settlement` | Source-complete: immediately gain 220 Gold, remove up to two cards, upgrade two cards, set Debt to 320, then each combat end pays due `min(40, Debt)` with Gold first and nonlethal 3 HP per 10 Gold short rounded up while Debt decreases by the full due. |

## 6. Lotha v2.2 Source-Complete Test Slice

Lotha is default-on. The source slice uses a custom Control-based Ancient background scene, separate map/run-history art, marker relic option art, English/zhs localization, and run/combat hooks registered through canonical `ModelDb` instances. Source guards cover all eight blessings. Live gameplay, save/load, lethal-path, co-op, and post-publish game-load evidence remain pending.

Implementation summary:

- `lotha_mirror_rebuttal`: choose one Attack, Skill, or Power card from the deck when taking the blessing. On the first player turn each combat, after normal draw, move the matching combat card to hand when source-safe. The first time that marked card is played each combat, Attack/Skill cards play one additional time. Power cards are not extra-played; the marked Power costs 0 for that play.
- `lotha_mirror_hall_echo`: at the end of each player turn, record the last player-played non-Status Attack, Skill, or Power. On the next player turn, the first player-played card of that type triggers once and clears the echo. Attack/Skill plays one additional time; Power is not extra-played, costs 0 for that play, and draws 1. Autoplay/generated clone plays neither set nor consume the echo.
- `lotha_presumption`: at combat start, apply visible Innocent state. At each player turn start while Innocent, draw 2, gain 1 Energy, and gain 8 Block. When source-detected unblocked enemy attack damage is taken, Innocent is removed, the player loses 8 HP immediately, and Innocent cannot be regained that combat. Source detection is conservative: enemy dealer, `ValueProp.Move`, unblocked damage, and no card source.
- `lotha_closed_court`: for the rest of the run, post-combat card rewards are removed from combat reward sets only; gold, potions, and relic rewards are left intact. On turn 1, draw 4 and gain 2 Energy. On turn 4, draw 2 and gain 2 Energy. It no longer discounts the first three cards.
- `lotha_deferred_verdict`: on turn 4, draw 4 cards, gain 4 Energy, and gain 3 player-owned Verdict stacks. This turn, each next non-Status card consumes 1 Verdict. Attack/Skill cards play one additional time. Power cards are not extra-played; they cost 0 for that play and draw 1. Verdict is removed at turn end and combat end. If combat ends before turn 4, heal 4 HP when source-safe.
- `lotha_death_reprieve`: once per run, prevents death and sets HP to 1. During the reprieve player turn, draw 10, gain 10 Energy, all card costs are 0, and further death is prevented. At that player turn end, if enemies remain, the player is killed with `force: true`; if all enemies are dead, the run continues. source-safe deviation: local turn-flow evidence did not prove a safe immediate interruption into a new player turn during enemy turn damage, so enemy-turn lethal starts the reprieve at the next player turn; player-turn lethal starts it immediately in the current player turn.
- `lotha_single_sentence`: the first player-driven Attack/Skill each turn plays two additional times. After that ruling, the player can play at most four more normal player-played cards that turn. A visible Single Sentence Power starts at 5 while the ruling is ready, switches to 4 after the ruling, counts down later normal plays, and reaches 0 when more plays are blocked. The first Power before that ruling costs 0 for that play and draws 1 without consuming the sentence. Autoplay, generated clones, and extra play executions do not consume the four-card cap.
- `lotha_public_evidence`: when the player applies a non-damaging negative status to enemies, those layers double and the player gains Enlightenment. When enemies apply a non-damaging negative status to the player, those layers double and one Enlightenment is removed. At turn start, consume up to 3 Enlightenment; each consumed draws 1 and grants 4 Block. Source policy uses `PowerModel.GetTypeForAmount(amount) == PowerType.Debuff` as the base gate, keeps Weak, Vulnerable, Frail, and other non-damage Debuff applications eligible, and excludes source-proven damage/kill Debuffs such as Poison, Constrict, Demise, Disintegration, Doom, Magic Bomb, Strangle, and The Gambit.

| Blessing | Planned Id | Design Notes |
| --- | --- | --- |
| Mirror Rebuttal | `lotha_mirror_rebuttal` | Chosen-card rebuttal with strict source-state marker and Power fallback. |
| Mirror Hall Echo | `lotha_mirror_hall_echo` | Echo-style effect with strict recursion and Power-card exclusions. |
| Presumption of Innocence | `lotha_presumption` | Defensive/legal framing; must show condition clearly. |
| Closed Court | `lotha_closed_court` | Removes standard card rewards and splits combat resources between turn 1 and turn 4. |
| Deferred Verdict | `lotha_deferred_verdict` | Uses player-owned turn-4 Verdict stacks; does not auto-damage. |
| Death Reprieve | `lotha_death_reprieve` | High-risk death-prevention effect; source-safe implementation has an enemy-turn timing deviation and needs live lethal-path proof. |
| Single Sentence | `lotha_single_sentence` | First Attack/Skill judgment plus a visible four-card remaining cap; Powers use replacement reward. |
| Public Evidence | `lotha_public_evidence` | Non-damaging negative status/evidence detection uses source power-amount hooks; Poison, damage-over-time, countdown damage, and source-proven damage/kill Debuffs are excluded; consumes Enlightenment at turn start. |

## 7. Vakuu Fight Hidden Opt-In Slice

Vakuu fight is hidden by default for single-player private-beta testing. It now has a dedicated source enemy and encounter scene, remains live-pending, and is not claimed multiplayer-safe.

Implemented behavior:

- When explicitly enabled and Vakuu appears, add an extra fight option.
- The current encounter uses a dedicated `EzmbVakuuTrialMonster`, a custom `ezmb_vakuu_trial.tscn` encounter scene, and a simple four-move Vakuu action loop. Live victory and restore behavior still need proof.
- The option text tells the player Vakuu is a Stolen Locks trial: Contracts are chosen on turns 1/3/5, Cash Out can end the fight after a lock breaks, normal combat rewards are disabled, Blood Debt reduces loot and raises attack pressure, and death ends the run.
- On player turns 1, 3, and 5, after the normal hand draw, the fight offers three 0-cost Contracts when source-safe. Knife and Gold support lock breaking, Shelter Contract reduces Blood Debt, and Fraud breaks a lock at higher risk.
- The Gold Contract is the localized face of the internal `EZMB_VAKUU_TEMPTATION` / Temptation card; it is implemented as a hidden Contract token, not future content.
- Vakuu starts with three Stolen Vault locks. Contracts can break locks, and dealing at least 40 unblocked damage to Vakuu in one player turn can also break one lock without adding Blood Debt.
- Blood Debt increases each of Vakuu's powered attack hits by 2 damage per stack. At victory settlement, each Blood Debt removes 15 loot Gold. If loot is not enough, each unpaid 15 Gold costs 3 nonlethal HP.
- Victory resumes the parent Vakuu event and offers extra non-Vakuu Act 3 Ancient blessing choices based on broken locks, from existing Nonupeipe/Tanx rewards plus custom Lotha option relics when unclaimed choices remain. Each broken lock creates 50 loot Gold before Blood Debt settlement. Custom Lotha victory choices route through the same Lotha selection service as the Lotha event, so the player receives the visible marker relic and the run hook state together.
- If no unclaimed non-Vakuu options are available, the victory page uses an explicit fallback instead of silently finishing the event with zero options; broken-lock Gold is still granted through that fallback.
- Failure is presented as lethal; live failure/death verification is pending.

Source shape:

- The fight option is injected through `Vakuu.GenerateInitialOptions`.
- The fight gate requires `ShouldEnableFight`, so normal Vakuu remains at three standard options unless `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_FORCE_VAKUU_FIGHT=1`, or a legacy `EZMB_*` fight alias is set.
- Combat entry does not call Core's `EnterCombatWithoutExitingEvent(...)`, because the local Core helper rejects non-shared events before it reaches the transition. Vakuu instead explicitly creates an `EzmbVakuuTrialEncounter` `CombatRoom`, sets `ShouldResumeParentEventAfterCombat = true`, clears the parent event `Node`, then awaits direct `EnterRoomWithoutExitingCurrentRoom(...)`.
- Clearing the parent event `Node` addresses the reported post-victory black-screen risk where `NEventRoom` rejects an event that still has an attached node. The active fight no longer assigns `ParentEventId`; the active combat room does not store `ParentEventId` while the combat room is active; the parent id is written only after prefinished combat serialization.
- A narrow `CombatRoom.ToSerializable()` postfix records the Vakuu parent id only for prefinished `EzmbVakuuTrialEncounter` combat rooms that still intend to resume the parent event.
- The prefinished parent-restore path also skips the duplicate Ancient heal that Core would otherwise apply when it reconstructs the parent Vakuu `EventRoom` below the finished combat.
- The custom encounter is `RoomType.Monster` and `ShouldGiveRewards => false`, matching Core's event-launched combat shape while avoiding normal combat rewards and nonserializable linked reward sets.
- The prefinished Vakuu trial restore path patches `CombatRoom.OfferRoomEndRewards()` so it resumes the parent Vakuu event instead of generating normal combat rewards.
- Parent-event resume patches `EventModel.Resume(...)` and calls the protected `SetEventState(...)` path by reflection to present the victory blessing choices.
- Run-state hooks cover parent-room and run-level behavior, while combat-state hooks cover combat-only paths. For Vakuu, the gated combat hook injects Contracts only when `combatState.Encounter is EzmbVakuuTrialEncounter`, applies the Stolen Vault power when Vakuu enters combat, and tracks player-turn unblocked damage for lock breaks.
- Contract cards are hidden 0-cost Skill tokens with Ethereal and Exhaust. They use normal card play commands plus a shared signing service for HP loss, Blood Debt, and lock breaks.
- Multiplayer is gated off by requiring `runState.Players.Count == 1`.
- Local `CombatRoom.ToSerializable()` throws for active combat rooms with `ParentEventId`; current Vakuu source no longer stores `ParentEventId` while the combat room is active, avoiding that known source-level blocker. Do not claim fight save/load readiness until live testing proves active-fight behavior if saving is available there, plus the patched prefinished no-reward parent-resume path.

## 8. New Cards And Statuses

| Card / Status | Status | Purpose |
| --- | --- | --- |
| Withered Husk | Current Urda source-backed slice | 0-cost Ethereal/Exhaust Curse. It gives 3 Block when exhausted. Current live verification pending. |
| Waste Paper | Current Morvi source-backed slice | Temporary Status used by Paperstorm; no extra punishment beyond Paperstorm consuming drawn Status cards. |
| Archive Pages | Current Morvi source-backed slice | Temporary 0-cost Ethereal/Exhaust pages from Overdue Library; unplayed pages have no extra punishment. |
| Vakuu Contracts | Current Vakuu source-backed slice | Hidden 0-cost Skill token Contracts offered by the Vakuu fight on turns 1/3/5. Ethereal + Exhaust; individual contracts either push lock breaking, reduce Blood Debt, or add higher-risk Blood Debt pressure. Uses the browser GPTimage2 rebuilt custom card portrait. |

## 9. Hook Requirements

Future implementation must inspect local game source before coding. Required research surfaces include:

- Ancient registration and act selection.
- Reward generation, reward alternatives, and skipped reward callbacks.
- Card copy/extra-play/replay command paths.
- Active button or relic-like UI action support.
- Death prevention/interruption and failure-state transitions.
- Temporary card storage zones and save/load serialization.
- Multiplayer player-owned state and desync-sensitive reward paths.

Local `source code/src/Core/` is primary evidence. BaseLib/RitsuLib/template source and local references are second. The tutorial index is only a secondary guide: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.

## 10. Test Focus

Before activation, each blessing needs:

- Source guard tests for the exact hook path.
- Localization guard tests for English and Simplified Chinese keys.
- Save/load plan and manual row.
- Multiplayer ownership stance.
- Disable-gate behavior.
- Runtime checklist rows that distinguish source guards from live proof.

