# Ancient Expansion Pack v2.2 Source Design

Status: Urda stabilization plus a default-off Morvi prototype. Lotha, Vakuu fight, and extra Urda blessing implementation remain planning-only.

Working title: **Sowing, Borrowing, and Judgment** (`播种、借阅与审判`).

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
- Do not make Lotha or Vakuu fight appear until their implementation milestone is explicitly approved.
- Morvi may appear only when the default-off `EZMB_ENABLE_MORVI_V22=1` test gate is enabled.
- Current Urda remains the only active expansion Ancient in the private-beta test slice.
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

The current repository already has a default-on private-beta Urda slice. This document does not overwrite it.

Current source-backed blessings:

| Blessing | Id | Current State |
| --- | --- | --- |
| Seedbed | `urda_seedbed` | Source hook exists; live gameplay/save-load pending. |
| Humus Pact | `urda_humus_pact` | Source hook exists; live gameplay/save-load pending. |
| Molting | `urda_molting` | Source hook exists with `Withered Husk`; live gameplay/save-load pending. |
| Moss Map | `urda_moss_map` | Source hook exists; live gameplay/save-load pending. |

## 4. Full Urda v2.2 Roadmap

Urda's full v2.2 design contains ten blessings. The four current source-backed blessings remain first-class; the six additional blessings are future work only.

| Blessing | Planned Id | Status | Design Intent |
| --- | --- | --- | --- |
| Seedbed | `urda_seedbed` | Current source-backed slice | Trade max HP for seeded growth rewards. |
| Humus Pact | `urda_humus_pact` | Current source-backed slice | Skip card rewards for gold/removal/upgraded-card payoff. |
| Molting | `urda_molting` | Current source-backed slice | Remove starter basics and accept temporary Husk clutter. |
| Moss Map | `urda_moss_map` | Current source-backed slice | First-time room-type memory rewards. |
| Trial Branch | `urda_trial_branch` | Future | Small early test that upgrades into a stronger branch only after the player proves route discipline. |
| Shallow-Root Relic | `urda_shallow_root_relic` | Future | A light relic-like effect with a clear decay or condition so it does not become free scaling. |
| Rooted Route | `urda_rooted_route` | Future | Route commitment reward that should never hard-lock a path or remove low-risk alternatives. |
| After the Rain | `urda_after_the_rain` | Future | Recovery/growth after taking a costly fight or event. |
| Root-Sight | `urda_root_sight` | Future | Extra preview information, not hidden power. |
| Seed Bank | `urda_seed_bank` | Future | Stored value that must be capped and visible to avoid runaway compounding. |

Future Urda work must keep the current four active blessings stable unless a dedicated Urda refactor milestone says otherwise.

## 5. Morvi v2.2 Prototype

Morvi is default-off. It appears in Act 2 only when `EZMB_ENABLE_MORVI_V22=1` is set, and it remains a prototype until live reward UI, save/load, and co-op checks pass.

| Blessing | Planned Id | Design Notes |
| --- | --- | --- |
| Forbidden Loan / 禁书借阅 | `morvi_forbidden_loan` | Borrow a powerful reward with strict repayment. Power cards must use a safe fallback, not copy/replay. |
| Misprint Press / 错页印刷机 | `morvi_misprint_press` | Prototype implemented default-off: the first Attack or Skill each combat is replayed once as an Exhausting generated copy; clone recursion, Power extra-play, and failed generated-copy leftovers are blocked. |
| Red Ink Overdraft / 红墨透支 | `morvi_red_ink_overdraft` | Active button, not forced. Requires UI/source proof before implementation. |
| Overdue Library / 逾期图书馆 | `morvi_overdue_library` | Archive Pages should create planning pressure; no punishment for unplayed archive pages. |
| Open-Book Exam / 开卷考试 | `morvi_open_book_exam` | Prototype implemented default-off: normal Act 2 combat card rewards upgrade one Attack or Skill option. |
| Paperstorm / 纸灰风暴 | `morvi_paperstorm` | Temporary paper/status pressure with clear cleanup. |
| Blueprint Proofreading / 蓝本校对 | `morvi_blueprint_proof` | Modify or correct a chosen card/reward in a controlled way. |
| Debt Settlement / 债务清算 | `morvi_debt_settlement` | Prototype implemented default-off: gain 75 Gold, then repay three Act 2 combat reward installments of up to 25 Gold with nonlethal HP fallback; payoff is an upgraded card reward and pending payoff state clears only after the reward resolver succeeds. |

## 6. Lotha v2.2 Planning

Lotha is not active. Lotha should be implemented only after source evidence proves the needed combat, card-play, death, and UI hooks. This pass also found an event-art/background blocker: local Ancient UI source loads Ancient background scenes, and no explicit Lotha art/custom scene source file is present.

| Blessing | Planned Id | Design Notes |
| --- | --- | --- |
| Mirror Rebuttal / 反证之镜 | `lotha_mirror_rebuttal` | Reflects or answers a visible enemy/player condition without hidden targeting. |
| Mirror Hall Echo / 镜厅回声 | `lotha_mirror_hall_echo` | Echo-style effect with strict recursion and Power-card exclusions. |
| Presumption of Innocence / 无罪推定 | `lotha_presumption` | Defensive/legal framing; must show condition clearly. |
| Closed Court / 终审封庭 | `lotha_closed_court` | v2.2 removes hand-limit +3 and uses first-turn burst instead. |
| Deferred Verdict / 延期判决 | `lotha_deferred_verdict` | Uses Verdict stacks; does not auto-damage. |
| Death Reprieve / 死刑缓期 | `lotha_death_reprieve` | High-risk death-interrupt effect; requires deep local source proof before coding. |
| Single Sentence / 单牌宣判 | `lotha_single_sentence` | One-card turn or one-card judgment pattern; must avoid softlocks. |
| Public Evidence / 公开罪证 | `lotha_public_evidence` | Debuff/evidence detection must be source-backed; ambiguity tracked in risk register. |

## 7. Vakuu Fight Planning

Vakuu fight is not active.

Planned behavior:

- When Vakuu appears, add an extra option: "Fight Vakuu" / `殴打瓦库`.
- Victory offers three non-Vakuu Act 3 Ancient blessings.
- Failure is death.
- If implemented, `Temptation` can be used as a visible status/reminder card for the risk.

This needs a dedicated death/failure path proof. Do not implement as a reward-screen shortcut without proving the live event and defeat consequences.

## 8. New Cards And Statuses

| Card / Status | Status | Purpose |
| --- | --- | --- |
| Withered Husk | Current Urda source-backed slice | Temporary/unplayable Molting card. Current live verification pending. |
| Waste Paper | Future | Morvi paper/status pressure. Must be temporary unless design explicitly says otherwise. |
| Archive Pages | Future | Morvi archive planning card/state. No punishment for merely leaving pages unplayed unless text says so. |
| Temptation | Future | Vakuu fight risk marker/status. Must not silently kill or trap the player. |

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
