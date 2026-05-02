# Ancient Reward Optimization Specification for StS2 Public Beta v0.104.0

This is a design and safety-gate specification. It does not authorize gameplay implementation.

## Corrected Project Mission
EzDailyContent is a Slay the Spire 2 system expansion mod.

Priority order:
1. Optimize and rebalance Slay the Spire 2 Ancient rewards.
2. Design and eventually implement Ascension 11-20-30.
3. Design and eventually implement a new custom character.

## Current Verified Baseline
- Repository: `wenhuorongbing-netizen/dev-the-spire`
- Branch: `main`
- Game target: Slay the Spire 2 public beta `v0.104.0`, date `2026.04.23`
- BaseLib runtime: `v3.1.0`
- BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.0`
- ModAnalyzers package: `Alchyr.Sts2.ModAnalyzers` `0.1.9`
- Manifest id: `EzDailyContent`
- Build baseline: `dotnet build` succeeds
- Publish baseline: `dotnet publish` succeeds
- Manual verification: BaseLib and EzDailyContent appear and are enabled in Mod Settings
- Gameplay implementation status: no concrete gameplay features implemented

## Why Ancient Rewards Are First
Ancient rewards are systemic, high-impact, and smaller in scope than Ascension expansion or a custom character.

They should be researched first because:
- They affect run power and variance.
- They influence future Ascension 11-30 difficulty targets.
- They may interact with future character design.
- They are likely easier to tune and roll back than large content systems.
- They force API research before deeper patching is attempted.

## Non-Goals
- No gameplay implementation.
- No Harmony patches.
- No Ancient reward code.
- No cards, relics, powers, localization, or config.
- No `MainFile.cs` changes.
- No manifest id change.
- No build setting changes.
- No package version changes.
- No copied original game assets.
- No copied decompiled implementation code.

## Research-First Safety Gate
Implementation is forbidden until every gate item is documented.

| Gate Item | Current Status | Required Before Implementation |
|---|---|---|
| Exact Ancient model class or registry location | UNKNOWN | Class/namespace/registry owner with evidence |
| Exact reward option model or pool type | UNKNOWN | Type name and how options are stored/generated |
| Exact reward generation timing | UNKNOWN | Method/event where Ancient reward options are built |
| Exact UI preview / reward resolution relationship | UNKNOWN | Evidence for how preview text maps to applied effect |
| Whether BaseLib can modify existing Ancient rewards | UNKNOWN | API evidence or documented absence |
| Whether Harmony is required | UNKNOWN | Decision after BaseLib/template research |
| Safest no-op logging probe point | UNKNOWN | Exact non-mutating method/API to log from |
| Rollback plan | UNKNOWN | Touched files, revert path, and runtime disable path |
| One-Ancient MVP target | UNKNOWN | Selected observed reward and minimal proposed change |
| Test procedure | UNKNOWN | Repeatable in-game verification route and log check |

If any status remains `UNKNOWN`, implementation must not begin.

## Reward Design Philosophy
Ancient reward optimization should improve decision quality, not simply increase power.

Principles:
- Prefer clear tradeoffs over hidden strength.
- Prefer small, reversible tuning.
- Preserve run variety.
- Avoid mandatory picks.
- Avoid invisible punishment or invisible power.
- Avoid context-sensitive changes until context availability is proven.
- Avoid UI text changes unless the preview pipeline is understood.
- Avoid random or persistent state changes in the first MVP.

## Reward Taxonomy
This taxonomy is provisional until real reward models are inspected.

| Category | Description | Balance Concern |
|---|---|---|
| Immediate combat reward | A benefit affecting current or next combat | Burst power and clarity |
| Economy reward | Gold, shop, purchase, or future resource effect | Snowball risk |
| Deck-shaping reward | Adds, removes, upgrades, transforms, or modifies cards | Can override deck identity |
| Relic-like reward | Persistent passive or triggered benefit | Mandatory pick risk |
| Risk reward | Upside paired with downside | Downside may be irrelevant or too punishing |
| Scaling reward | Benefit grows across combat, act, or run | Runaway power |
| Contextual reward | Strong under specific deck, character, act, or resource conditions | Useless or overfitted if context is wrong |
| Utility reward | Improves consistency, information, or flexibility | Low-impact reward slot |

## Implementation Strategy Options

### Option A: BaseLib-supported modification
Use BaseLib or template APIs to modify existing Ancient reward behavior or pools.

Status: UNKNOWN.

Required evidence:
- API name.
- Supported target.
- Example or source reference.
- Whether existing rewards can be modified safely.

### Option B: Additive supported content
Use BaseLib/template APIs to add new Ancient reward models or variants without mutating basegame entries.

Status: UNKNOWN.

Required evidence:
- Whether `CustomAncientModel` exists and what it supports.
- Whether additive rewards are visible in the correct pools.
- Whether localization/preview behavior is supported.

### Option C: Narrow Harmony patch
Use a small patch only if Options A and B cannot satisfy the MVP.

Status: FORBIDDEN UNTIL PROVEN.

Required evidence:
- Exact patch method.
- Why no supported API can work.
- Why the patch does not affect unrelated reward systems.
- No-op logging probe verified before mutation.
- Rollback plan.

### Option D: Research-only release
Ship no gameplay changes. Keep design docs and balance map only.

Status: allowed if implementation evidence remains insufficient.

## MVP Proposal
The MVP is intentionally not selected yet.

Current MVP target: `UNKNOWN`.

Selection rules:
- Must be one Ancient reward only.
- Must have observed current behavior.
- Must have clear balance reason.
- Must have minimal tuning lever.
- Must not require broad registry replacement.
- Must not require UI-only patching.
- Must have a test route.

## Balance Methodology
1. Observe reward behavior in game.
2. Confirm model and pool structure through research.
3. Record facts in `docs/ANCIENT_REWARD_BALANCE_MAP.md`.
4. Score power, clarity, context sensitivity, and risk.
5. Select one MVP target.
6. Choose implementation strategy.
7. Add no-op logging probe if patching or uncertain timing is involved.
8. Implement only after explicit approval.

## Data Collection Plan
Collect:
- Ancient name.
- Reward display name.
- Internal id if safely discoverable.
- Effect.
- Timing.
- Trigger/resolution behavior.
- Preview text behavior.
- Act, character, deck, and difficulty context if available.
- Source of evidence.
- Test notes.

Do not collect:
- Copied decompiled method bodies.
- Original game assets.
- Large copied chunks of game text.
- Guess-based implementation plans.

## Technical Architecture
No implementation architecture is approved yet.

Potential future locations, after research:
- `EzDailyContentCode/Ancients/` only if supported Ancient APIs exist.
- `EzDailyContentCode/Patches/` only if Harmony is proven necessary.
- `EzDailyContentCode/Diagnostics/` only if a no-op probe is approved.

Do not create these folders during the current research gate.

## Config Plan
No config should be created during research.

Future config may be considered only if:
- It helps safe rollback.
- Defaults are safe.
- It does not require unstable internal ids.
- It is documented before implementation.

## Logging and Debugging Plan
Logging is research-gated.

Required before logging implementation:
- Exact no-op probe point: UNKNOWN.
- Logging API: UNKNOWN.
- Confirmation that logging does not mutate reward generation: UNKNOWN.

Desired future log facts:
- Ancient reward tuning system initialized.
- Game/BaseLib versions.
- Reward generation point observed.
- Candidate reward id observed.
- No mutation performed during probe.

## Testing Plan

### Current Allowed Test
- `dotnet build`

### Future Implementation Test Procedure
Status: UNKNOWN.

Must include:
- How to reach Ancient reward choices.
- How to identify the target Ancient/reward.
- What logs to inspect.
- What behavior proves the change.
- What behavior proves rollback.

## Public Beta Compatibility Plan
- Target only public beta `v0.104.0`, date `2026.04.23`.
- Treat all Ancient reward APIs as unstable until inspected.
- Re-run research after public beta updates.
- Do not claim compatibility beyond tested versions.
- Prefer BaseLib-supported APIs over Harmony.

## Relation to Ascension 11-20-30
Ascension expansion depends on Ancient reward balance. Do not design final A11-A30 pressure until Ancient reward power, variance, and risk are better understood.

See `docs/ASCENSION_11_30_ROADMAP.md`.

## Relation to New Character
Future custom character work depends on stable Ancient reward and Ascension baselines.

See `docs/NEW_CHARACTER_ROADMAP.md` and future drafts under `docs/_future/new-character/`.

## Risk Register

| Risk | Likelihood | Impact | Mitigation |
|---|---:|---:|---|
| Guessing wrong Ancient model class | High | High | Block implementation until exact class/registry is documented |
| Reward pool type misunderstood | High | High | Require pool type evidence before tuning |
| UI preview desyncs from effect | Medium | High | Require preview/resolution relationship evidence |
| BaseLib support is assumed but absent | Medium | High | Inspect BaseLib source/docs before implementation |
| Harmony patch is too broad | Medium | High | Forbid broad patch points and require no-op probe |
| Public beta update breaks API | High | High | Pin docs to `v0.104.0` and re-research after updates |
| MVP target selected from assumptions | Medium | Medium | Require observed facts in balance map |
| Test procedure is not repeatable | Medium | Medium | Block implementation until manual route exists |

## Rollback Plan
Current status: UNKNOWN for implementation because no implementation path is selected.

Minimum required rollback plan before implementation:
- Git checkpoint before code changes.
- Exact touched files listed.
- How to disable EzDailyContent in Mod Settings.
- How to remove published mod artifacts from `<GameRoot>\mods\EzDailyContent`.
- How to revert the implementation commit.
- How to rerun `dotnet publish` from the previous known-good commit.

## Development Phases

### Phase R0: Research Gate
- Resolve required UNKNOWN items.
- Fill balance map with observed facts.
- No gameplay implementation.

### Phase R1: BaseLib/API Decision
- Decide whether BaseLib can modify existing Ancient rewards.
- Decide whether Harmony is required.
- If Harmony is required, identify no-op probe point.

### Phase R2: MVP Selection
- Select one Ancient reward.
- Define one minimal change.
- Define test procedure.
- Define rollback.

### Phase I0: Implementation Approval
- Starts only after explicit user approval.
- Not part of this task.

## Completion Criteria
The design/research gate is complete when:
- All required gate items are no longer UNKNOWN.
- One-Ancient MVP target is documented.
- A safe implementation strategy is documented.
- A test procedure is documented.
- Rollback is documented.
- No gameplay implementation has occurred before approval.
