# Ancient Reward Balance Map

This document separates observed facts from assumptions. It is not an implementation plan.

## Scope
- Target game: Slay the Spire 2 public beta `v0.104.0`, date `2026.04.23`
- Current phase: research and design only
- Implementation status: none

## Safety Gate Status

| Gate Item | Status | Notes |
|---|---|---|
| Exact Ancient model class or registry location | UNKNOWN | Required before implementation |
| Exact reward option model or pool type | UNKNOWN | Required before implementation |
| Exact reward generation timing | UNKNOWN | Required before implementation |
| Exact UI preview / reward resolution relationship | UNKNOWN | Required before implementation |
| Whether BaseLib can modify existing Ancient rewards | UNKNOWN | Required before implementation |
| Whether Harmony is required | UNKNOWN | Required before implementation |
| Safest no-op logging probe point | UNKNOWN | Required before implementation |
| Rollback plan | UNKNOWN | Required before implementation |
| One-Ancient MVP target | UNKNOWN | Required before implementation |
| Test procedure | UNKNOWN | Required before implementation |

## Reward Taxonomy
This taxonomy is provisional until real Ancient reward abstractions are confirmed.

| Taxonomy | Definition | Main Balance Question |
|---|---|---|
| Immediate combat reward | Changes near-term combat power | Is the burst appropriate for timing and cost? |
| Economy reward | Changes gold, shops, purchases, or future resources | Does it create runaway scaling? |
| Deck-shaping reward | Adds, removes, upgrades, transforms, or modifies cards | Does it preserve meaningful deck direction? |
| Relic-like reward | Persistent passive or triggered benefit | Is it mandatory or too passive? |
| Risk reward | Pairs upside with downside | Is the downside real, clear, and fair? |
| Scaling reward | Grows across combat, act, or run | Is there a cap or natural limiter? |
| Contextual reward | Depends on deck, character, act, or resources | Is it too narrow or too universally strong? |
| Utility reward | Improves consistency, information, or flexibility | Is the effect worth a reward slot? |

## Balance Principles
- Facts first, tuning second.
- Prefer small, reversible changes.
- Prefer clarity and choice quality over raw power.
- Avoid context-sensitive tuning until context availability is proven.
- Avoid text/effect changes until preview and resolution relationship is known.
- Avoid Harmony until BaseLib and template options are ruled out.

## Evaluation Rubric

| Score | Power | Clarity | Context Sensitivity | Risk |
|---:|---|---|---|---|
| 1 | Too weak or mostly irrelevant | Confusing or hidden | Rarely relevant | Breakage or exploit likely |
| 2 | Below target | Partly clear | Narrow | Some edge cases |
| 3 | Acceptable | Clear in normal play | Meaningful contexts | Manageable |
| 4 | Strong | Clear and attractive | Rewards planning | Needs monitoring |
| 5 | Overpowered or mandatory | Clear but too efficient | Too broadly good | High balance or stability risk |

## Observed Facts
Only add rows backed by in-game observation, API inspection, or documented source evidence.

| Ancient | Reward Name | Internal ID | Effect | Timing | Preview Behavior | Resolution Behavior | Source | Notes |
|---|---|---|---|---|---|---|---|---|
| TODO | TODO | UNKNOWN | TODO | UNKNOWN | UNKNOWN | UNKNOWN | TODO | TODO |

## Assumptions
Assumptions are not implementation evidence.

| Assumption | Reason | Verification Needed | Risk if False |
|---|---|---|---|
| Ancient rewards may come from a reward pool | Choice systems commonly use pools | Inspect StS2 reward generation model | Wrong tuning layer |
| Some rewards may be tunable without UI changes | MVP should prefer simple changes | Confirm preview/effect relationship | Text and behavior desync |
| BaseLib may expose Ancient APIs | BaseLib package exists and includes modding abstractions | Inspect BaseLib docs/source/API | Unnecessary Harmony patch |
| Harmony may be needed | Existing basegame rewards may not be mutable through API | Rule out supported API first | Unsafe patching if assumed too early |

## Research Evidence Log

| Date | Evidence Type | Source | Finding | Confidence | Follow-up |
|---|---|---|---|---|---|
| TODO | TODO | TODO | TODO | TODO | TODO |

## Candidate Reward Evaluation
Do not select an MVP until observed facts are available.

| Ancient | Reward | Taxonomy | Effect | Timing | Power Score | Clarity Score | Context Score | Risk Score | Proposed Change | Evidence Status | Test Notes |
|---|---|---|---|---|---:|---:|---:|---:|---|---|---|
| TODO | TODO | TODO | TODO | UNKNOWN | TODO | TODO | TODO | TODO | TODO | UNKNOWN | TODO |

## One-Ancient MVP Selection
Current MVP target: `UNKNOWN`.

Selection requires:
- Observed current behavior.
- Known model/pool type.
- Known generation timing.
- Known preview/resolution relationship.
- Known BaseLib vs Harmony implementation path.
- Known test procedure.
- Known rollback plan.

## Forbidden Until Proven
- Broad reward pool patches.
- Run initialization patches.
- Save/load patches.
- UI-only patches.
- Transpiler patches.
- Direct player/deck state mutation.
- Global randomization changes.

## Test Notes Template

### Reward
- Ancient:
- Reward:
- Version observed:
- Current behavior:
- Evidence source:
- Proposed behavior:
- Implementation path:
- Build result:
- Publish result:
- Manual game result:
- Log result:
- Rollback result:
- Decision:

## Open Research Tasks
- Identify exact Ancient model class or registry location.
- Identify exact reward option model or pool type.
- Identify exact reward generation timing.
- Identify preview and resolution relationship.
- Determine whether BaseLib can modify existing Ancient rewards.
- Determine whether Harmony is required.
- Identify safest no-op logging probe point.
- Select one-Ancient MVP target.
- Write repeatable test procedure.
