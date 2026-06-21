# Ancient Reward Balance Map

This document separates observed facts from assumptions. It is not an implementation plan.

## Scope
- Target game: Slay the Spire 2 public beta `v0.104.0`, date `2026.04.23`
- Current phase: research and design only
- Implementation status: none

## Safety Gate Status

| Gate Item | Status | Notes |
|---|---|---|
| Exact Ancient model class or registry location | PARTIAL | `AncientEventModel` confirmed; exact basegame data source/population path still required |
| Exact reward option model or pool type | PARTIAL | `EventOption` confirmed; previous framework custom `OptionPools` confirmed; basegame pool type still required |
| Exact reward generation timing | PARTIAL | Relevant signatures confirmed; exact call order still required |
| Exact UI preview / reward resolution relationship | PARTIAL | `EventOption` text/resolution members confirmed; UI binding still required |
| Whether previous framework can modify existing Ancient rewards | NO DIRECT API FOUND | Custom Ancient support found; no explicit existing-reward mutation API found locally |
| Whether Harmony is required | UNKNOWN | Required before implementation |
| No-op logging probe point | OBSERVED WORKING NO-OP PROBE | `AncientEventModel.GenerateInitialOptionsWrapper()` logging probe observed in game; reward tuning still not approved |
| Rollback plan | UNKNOWN | Required before implementation |
| One-Ancient MVP target | UNKNOWN | Required before implementation |
| Test procedure | PARTIAL | Probe logs provide observed output for Neow, Pael, and Tanx; repeatable route to specific Ancients remains incomplete |

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
- Avoid Harmony until previous framework and template options are ruled out.

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

### Observed AncientRewardNoopProbe option metadata
Source: user-provided `godot.log` lines from `AncientRewardNoopProbe`.

Do not infer reward effects, reward strength, or implementation path from these `TextKey` values.

| Ancient Runtime Type | Option Count | Option Index | Option Runtime Type | TextKey | RelicIsNull | IsLocked | IsProceed | ShouldSaveChoiceToHistory |
|---|---:|---:|---|---|---|---|---|---|
| `MegaCrit.Sts2.Core.Models.Events.Neow` | 3 | 0 | `MegaCrit.Sts2.Core.Events.EventOption` | `NEOW.pages.INITIAL.options.ARCANE_SCROLL` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Neow` | 3 | 1 | `MegaCrit.Sts2.Core.Events.EventOption` | `NEOW.pages.INITIAL.options.POMANDER` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Neow` | 3 | 2 | `MegaCrit.Sts2.Core.Events.EventOption` | `NEOW.pages.INITIAL.options.LEAFY_POULTICE` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Pael` | 3 | 0 | `MegaCrit.Sts2.Core.Events.EventOption` | `PAEL.pages.INITIAL.options.PAELS_HORN` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Pael` | 3 | 1 | `MegaCrit.Sts2.Core.Events.EventOption` | `PAEL.pages.INITIAL.options.PAELS_CLAW` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Pael` | 3 | 2 | `MegaCrit.Sts2.Core.Events.EventOption` | `PAEL.pages.INITIAL.options.PAELS_LEGION` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Tanx` | 3 | 0 | `MegaCrit.Sts2.Core.Events.EventOption` | `TANX.pages.INITIAL.options.SAI` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Tanx` | 3 | 1 | `MegaCrit.Sts2.Core.Events.EventOption` | `TANX.pages.INITIAL.options.THROWING_AXE` | False | False | False | True |
| `MegaCrit.Sts2.Core.Models.Events.Tanx` | 3 | 2 | `MegaCrit.Sts2.Core.Events.EventOption` | `TANX.pages.INITIAL.options.MEAT_CLEAVER` | False | False | False | True |

### Observed Ancient summary

| Ancient Runtime Type | Observed Option Count | Shared Observed Option Flags |
|---|---:|---|
| `MegaCrit.Sts2.Core.Models.Events.Neow` | 3 | All observed options: `OptionType=EventOption`, `RelicIsNull=False`, `IsLocked=False`, `IsProceed=False`, `ShouldSaveChoiceToHistory=True` |
| `MegaCrit.Sts2.Core.Models.Events.Pael` | 3 | All observed options: `OptionType=EventOption`, `RelicIsNull=False`, `IsLocked=False`, `IsProceed=False`, `ShouldSaveChoiceToHistory=True` |
| `MegaCrit.Sts2.Core.Models.Events.Tanx` | 3 | All observed options: `OptionType=EventOption`, `RelicIsNull=False`, `IsLocked=False`, `IsProceed=False`, `ShouldSaveChoiceToHistory=True` |

## Assumptions
Assumptions are not implementation evidence.

| Assumption | Reason | Verification Needed | Risk if False |
|---|---|---|---|
| Ancient rewards may come from a reward pool | Choice systems commonly use pools | Inspect StS2 reward generation model | Wrong tuning layer |
| Some rewards may be tunable without UI changes | MVP should prefer simple changes | Confirm preview/effect relationship | Text and behavior desync |
| previous framework may expose Ancient APIs | previous framework package exists and includes modding abstractions | Inspect previous framework docs/source/API | Unnecessary Harmony patch |
| Harmony may be needed | Existing basegame rewards may not be mutable through API | Rule out supported API first | Unsafe patching if assumed too early |

## Research Evidence Log

| Date | Evidence Type | Source | Finding | Confidence | Follow-up |
|---|---|---|---|---|---|
| TODO | TODO | TODO | TODO | TODO | TODO |
| 2026-05-02 | Local reflection/XML | `sts2.dll`, `previous framework.dll`, `previous framework.xml` | `AncientEventModel`, `EventOption`, previous framework `CustomAncientModel`, `OptionPools`, and `AncientOption` signatures confirmed | Medium-high | Inspect call flow and previous framework source/examples |
| 2026-05-03 | Manual game verification | `godot.log` and in-game Ancient event observation | `AncientRewardNoopProbe` log entries appeared; previous framework and EzDailyContent were enabled; Ancient options appeared and selected normally; no probe exception or visible behavior change was observed | Medium | Use probe logs to build observed Ancient reward catalog before selecting MVP |
| 2026-05-03 | Probe log catalog observation | User-provided `AncientRewardNoopProbe` lines | Neow, Pael, and Tanx runtime types observed; each generated 3 `EventOption` options; all observed options had `RelicIsNull=False`, `IsLocked=False`, `IsProceed=False`, and `ShouldSaveChoiceToHistory=True` | Medium | Continue collecting observed options; do not infer effects from TextKey alone |

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
- Known previous framework vs Harmony implementation path.
- Known test procedure.
- Known rollback plan.

Current status:
- No One-Ancient MVP target selected.
- Reward tuning gate remains closed.
- Catalog progress: PARTIAL / IN PROGRESS.

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
- Determine whether previous framework can modify existing Ancient rewards.
- Determine whether Harmony is required.
- Identify safest no-op logging probe point.
- Select one-Ancient MVP target.
- Write repeatable test procedure.
