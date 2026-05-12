# Ancient Expansion v2.2 Issues

Status: current Urda stabilization is source-backed but live-pending, and Morvi has a default-off source prototype with generated-copy/debt-payoff guards. Lotha, extra Urda blessings, and Vakuu fight remain planning-only.

Current-state constraint:

- Urda is already default-on for private-beta testing with source-backed Seedbed, Humus Pact, Molting, and Moss Map hooks.
- Current Urda hardening stays limited to the current four Urda blessings.
- Urda live gameplay/save-load verification is still pending.
- Morvi has a default-off source prototype behind `EZMB_ENABLE_MORVI_V22=1`; no default private-beta run sees Morvi.
- Lotha and Vakuu fight are not active gameplay content.
- Morvi/Lotha event art source files are not present in this repo; no placeholder art or export entry is allowed.
- v2.2 must not be represented as release-ready until source/live/save-load checks pass.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-DESIGN-DOC-INGEST

Priority: P0  
Status: open  
Area: planning / docs / future Ancient expansion roadmap

Goal:

- Ingest Ancient Expansion Pack v2.2 without polluting `docs/issues.md`.
- Preserve the full design under feature docs.
- Convert the design into a maintainable roadmap, issue index, milestones, safety rules, and implementation order.

Closure:

- Full v2.2 design exists in `docs/features/ancient-expansion-v2.2/source-design.md`.
- Milestones are split and gated.
- `docs/issues.md` remains compact.
- Morvi is explicitly default-off prototype content; Lotha and Vakuu fight are planning-only.
- Current Urda state is accurately cross-linked, not overwritten by older prototype wording.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES

Priority: P0  
Status: open  
Area: extra play / copy / ability-card safety

Global rule:

- Extra play, copy, reprint, verdict, echo, and replay effects may target Attacks and Skills.
- Powers must not be copied, extra-played, or replayed by default.
- If such an effect meets a Power card, use replacement rewards: cost becomes 0, draw cards, gain energy, or wait for the next Attack/Skill.
- Extra-played cards must not recursively trigger the same blessing.

Closure:

- Rule is documented in `docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md`.
- Tests/guards exist before implementing Morvi/Lotha extra-play effects.
- Each future blessing states how it handles Powers.

## ISSUE-2026-05-12-MORVI-V22-PLANNING

Priority: P1  
Status: default-off prototype  
Area: Act 2 Ancient / Morvi

Morvi blessing pool:

- `morvi_forbidden_loan` / Forbidden Loan / 禁书借阅
- `morvi_misprint_press` / Misprint Press / 错页印刷机
- `morvi_red_ink_overdraft` / Red Ink Overdraft / 红墨透支
- `morvi_overdue_library` / Overdue Library / 逾期图书馆
- `morvi_open_book_exam` / Open-Book Exam / 开卷考试
- `morvi_paperstorm` / Paperstorm / 纸灰风暴
- `morvi_blueprint_proof` / Blueprint Proofreading / 蓝本校对
- `morvi_debt_settlement` / Debt Settlement / 债务清算

Implemented default-off prototype:

- `morvi_misprint_press`: first Attack or Skill each combat is replayed once as an Exhausting generated copy; Powers and clones are excluded, and failed generated-copy insertion cleans up the unpiled clone.
- `morvi_open_book_exam`: normal Act 2 combat card rewards upgrade one Attack or Skill option.
- `morvi_debt_settlement`: gain 75 Gold, repay three reward installments of up to 25 Gold with nonlethal HP fallback, then receive an upgraded card reward; payoff pending state clears only after the payoff reward resolver succeeds.

Still planning-only:

- Forbidden Loan.
- Red Ink Overdraft.
- Overdue Library.
- Paperstorm.
- Blueprint Proofreading.
- Any default-on Morvi release decision.

## ISSUE-2026-05-12-LOTHA-V22-PLANNING

Priority: P1  
Status: planning-only  
Area: Act 3 Ancient / Lotha

Lotha blessing pool:

- `lotha_mirror_rebuttal` / Mirror Rebuttal / 反证之镜
- `lotha_mirror_hall_echo` / Mirror Hall Echo / 镜厅回声
- `lotha_presumption` / Presumption of Innocence / 无罪推定
- `lotha_closed_court` / Closed Court / 终审封庭
- `lotha_deferred_verdict` / Deferred Verdict / 延期判决
- `lotha_death_reprieve` / Death Reprieve / 死刑缓期
- `lotha_single_sentence` / Single Sentence / 单牌宣判
- `lotha_public_evidence` / Public Evidence / 公开罪证

Important v2.2 changes:

- Closed Court removes hand-limit +3 and uses first-turn burst.
- Deferred Verdict is now Verdict stacks; it does not auto-damage.
- Power cards use replacement rewards instead of extra play/copy.
- Death Reprieve is high-risk and requires careful death-interrupt source research.

Implementation remains blocked in this pass by missing explicit Lotha event art/custom Ancient background source files and by the still-unimplemented default-off gate/test package. Death Reprieve remains blocked separately until lethal-damage source research is complete.

## ISSUE-2026-05-12-VAKUU-FIGHT-V22-PLANNING

Priority: P2  
Status: planning-only  
Area: Act 3 special Vakuu option

Design:

- Add "Fight Vakuu" / `殴打瓦库` as an extra option when Vakuu appears.
- Victory offers three non-Vakuu Act 3 Ancient blessings.
- Failure is death.
- Add `Temptation` status card if implemented later.

Do not implement in this docs-ingest pass.

## ISSUE-2026-05-12-URDA-V22-ALIGNMENT

Priority: P1  
Status: open  
Area: Urda current implementation vs v2.2 design

Current repo state:

- Urda is already default-on for private-beta testing.
- Active ids: `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`.
- These have source gameplay hooks; Humus Pact and Seedbed were source-hardened in the current pass, but manual gameplay/save-load verification is still pending.

Goal:

- Align existing Urda feature docs with v2.2 naming, theme, and full ten-blessing roadmap.
- Do not overwrite existing source-backed Urda implementation docs.
- Mark unimplemented Urda blessings as future milestones: Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, Seed Bank.
- Keep existing live blockers for the current Urda slice.

Closure:

- v2.2 Urda plan links to existing Urda docs and does not claim all ten blessings are implemented.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES

Priority: P1  
Status: open  
Area: implementation gating

Milestone 1 recommendation:

- Urda: verify/finish Seedbed, Humus Pact, Molting, and Moss Map before adding more.
- Morvi: Misprint Press, Open-Book Exam, Debt Settlement.
- Lotha: Mirror Rebuttal, Single Sentence, Death Reprieve.

Implementation beyond current Urda stabilization must be gated behind explicit user approval after docs ingest.

Closure:

- Each milestone has acceptance criteria, tests, and manual checklist rows.
- No future milestone starts until current RC blockers are acknowledged.

## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MORVI-LOTHA-ART-INTEGRATION

Priority: P1  
Status: open / source-file pending  
Area: Ancient event art

Approved direction:

- Morvi should use lender-scribe contract art at `EZMicroBalance/images/events/ezmb_morvi.png`.
- Lotha should use mirror-tribunal art at `EZMicroBalance/images/events/ezmb_lotha.png`.

Requirements:

- Copy only explicit, visually confirmed local source images.
- Do not promote unverified temp/cache files into active resources.
- Add both PNGs to `export_presets.cfg` only after the source files are present.
- Add guard tests for resource/export coverage.
- Bind Morvi portrait when source-backed. Keep Lotha staged or gated until Lotha implementation exists.
- Do not use placeholder art to close this issue.
