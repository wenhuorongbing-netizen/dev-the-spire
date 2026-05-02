# Ancient Reward Research Plan

This is a safety-gated research plan for Ancient reward optimization.

No gameplay implementation is allowed from this document alone.

## Purpose
Prevent random guessing, unsafe Harmony patching, and premature Ancient reward implementation.

The project must identify the real Slay the Spire 2 Ancient reward model, reward pool, generation timing, UI preview behavior, and BaseLib support before writing gameplay code.

## Hard Research Rules
- Do not implement gameplay during research.
- Do not create patches during research.
- Do not create Ancient reward code during research.
- Do not create cards, relics, powers, localization, or config during research.
- Do not change `MainFile.cs`.
- Do not change manifest id `EzDailyContent`.
- Do not change build settings.
- Do not change package versions.
- Do not copy original game assets.
- Do not copy large chunks of decompiled game code.
- Structure and API names may be documented; copied implementation bodies may not.

## 1. What We Know
- Target game branch: public beta.
- Verified game version: `v0.104.0`, date `2026.04.23`.
- BaseLib runtime version: `v3.1.0`.
- BaseLib package version: `Alchyr.Sts2.BaseLib` `3.1.0`.
- ModAnalyzers package version: `Alchyr.Sts2.ModAnalyzers` `0.1.9`.
- Manifest id: `EzDailyContent`.
- `dotnet build` succeeds.
- `dotnet publish` succeeds.
- Manual game verification succeeded: BaseLib and EzDailyContent appear and are enabled in Mod Settings.
- No concrete gameplay features have been implemented.
- The first feature target is Ancient reward optimization.

## 2. What We Do Not Know
Every item in this list blocks implementation until resolved.

| Required Fact | Current Status | Research Task |
|---|---|---|
| Exact Ancient model class or registry location | PARTIAL | Model class is `MegaCrit.Sts2.Core.Models.AncientEventModel`; act-level Ancient members include `ActModel.AllAncients`, `_sharedAncientSubset`, `GetUnlockedAncients`, `SetSharedAncientSubset`, and `PullAncient`. Exact basegame data source/population path remains UNKNOWN. |
| Exact reward option model or pool type | PARTIAL | Event-layer option model is `MegaCrit.Sts2.Core.Events.EventOption`; BaseLib custom Ancient pools use `BaseLib.Utils.OptionPools`, `WeightedList<AncientOption>`, and `RelicModel`. Exact basegame reward pool type remains UNKNOWN. |
| Exact reward generation timing | PARTIAL | Relevant signatures exist: `EventModel.BeginEvent`, `EventModel.GenerateInitialOptionsWrapper`, `EventModel.GenerateInitialOptions`, and `AncientEventModel.GenerateInitialOptionsWrapper`. Exact call order remains UNKNOWN. |
| Exact UI preview / reward resolution relationship | PARTIAL | `EventOption` has `TextKey`, `Title`, `Description`, `OnChosen`, `Relic`, and `Chosen()`. Exact UI binding path remains UNKNOWN. |
| Whether BaseLib can modify existing Ancient rewards | NO DIRECT API FOUND | Local BaseLib XML/reflection shows custom Ancient support, not an explicit API for mutating existing basegame Ancient rewards. Inspect source/examples before deciding. |
| Whether Harmony is required | UNKNOWN | Determine whether BaseLib/template APIs are sufficient |
| Safest no-op logging probe point | CANDIDATE ONLY | Candidate: postfix on nonpublic `AncientEventModel.GenerateInitialOptionsWrapper()` to observe returned options. Not approved until call order and no-op behavior are verified. |
| Rollback plan for first implementation | PARTIAL | Finalize once implementation path and touched files are known |
| One-Ancient MVP target | UNKNOWN | Select after catalog and balance map have observed facts |
| Repeatable test procedure | UNKNOWN | Identify deterministic or practical route to observe Ancient rewards |

## 3. What Must Be Inspected in Decompiled StS2 Code
Use decompilation for structure and API shape only. Do not copy implementation bodies into this repository.

| Topic | Search Terms | Evidence Needed | Output |
|---|---|---|---|
| Ancient model definition | `Ancient`, `AncientModel`, `Ancients` | Exact class name, namespace, fields/properties, registry owner | Add to implementation gate |
| Ancient registry | `Register`, `Registry`, `Catalog`, `Database`, `Collection` | How basegame Ancients are registered or looked up | Add registry note |
| Reward option model | `Reward`, `RewardOption`, `Option`, `Choice`, `AncientReward` | Exact model type used for Ancient rewards | Add reward model note |
| Reward pool type | `Pool`, `Weighted`, `Random`, `Generate`, `Roll` | Exact pool representation and weighting behavior | Add pool note |
| Generation timing | `GenerateRewards`, `CreateReward`, `GetRewards`, `Choose` | When options are generated relative to act/run state | Add timing note |
| Reward resolution | `Apply`, `Resolve`, `Choose`, `Claim`, `OnSelect` | How selected reward applies its effect | Add resolution note |
| UI preview | `Preview`, `Description`, `Tooltip`, `Localization`, `Text` | Whether preview text is generated from model or separate localization | Add preview note |
| Run context | `Run`, `Act`, `Character`, `Deck`, `Player` | Whether act, character, deck context is available safely | Add context note |
| Save/load | `Save`, `Load`, `Serialize`, `Deserialize` | Whether reward choices/effects affect persistent state | Add persistence risk |

## 4. What Must Be Inspected in BaseLib Docs or Source
Prefer official BaseLib APIs or template-supported workflows over Harmony patches.

| Topic | Files or APIs to Inspect | Evidence Needed | Output |
|---|---|---|---|
| `CustomAncientModel` scope | BaseLib source/docs/API metadata | Whether it creates new Ancients only or can override/tune existing ones | Decide additive vs tuning path |
| Existing Ancient modification API | BaseLib registration and extension APIs | Whether basegame Ancient reward pools can be modified directly | Decide if Harmony can be avoided |
| Reward registration API | BaseLib examples and analyzers | Supported registration shape and required localization/assets | API usage note |
| Pool modification API | BaseLib pool helpers if present | Whether weights, filters, or replacements are exposed | Pool tuning feasibility |
| Analyzer expectations | `Alchyr.Sts2.ModAnalyzers` diagnostics | Required patterns and forbidden patterns | Implementation checklist |
| Logging utilities | BaseLib or template logging support | Preferred mod logging path | No-op probe plan |

## 5. What Must Be Tested in Game
These tests are research tests, not implementation tests.

| Test | Purpose | Required Evidence |
|---|---|---|
| Confirm Ancient reward encounter path | Learn how to reliably reach reward choices | Manual steps and screenshots/notes |
| Record current reward options | Build observed catalog | Reward name, effect, timing, context |
| Check act variation | Determine whether rewards vary by act | Same Ancient observed across act contexts if practical |
| Check character/deck variation | Determine whether rewards are context-sensitive | Notes from at least one controlled comparison if practical |
| Check logs during reward generation | Identify useful logging windows | `godot.log` entries and absence of errors |
| Disable/re-enable mod after research build | Confirm no accidental behavior | Mod Settings still clean |

## 6. Hypotheses Only
The following names and concepts are hypotheses until proven by inspection.

| Hypothesis | Status | Do Not Assume |
|---|---|---|
| `CustomAncientModel` can tune existing Ancient rewards | UNPROVEN | Evidence confirms custom Ancient support, not existing basegame reward mutation |
| Ancient rewards are generated from weighted pools | PARTIAL | BaseLib custom Ancient options use weighted pools; basegame pool type remains UNKNOWN |
| Reward options are relic-like | UNPROVEN | Do not use relic hooks unless model evidence supports it |
| Reward effects resolve through command APIs | UNPROVEN | Do not call command APIs until reward resolution is known |
| Act number is available during reward generation | UNPROVEN | Do not design act-sensitive MVP until context evidence exists |
| Character/deck context is available | UNPROVEN | Do not design character-sensitive MVP until context evidence exists |
| Harmony patching will be required | UNPROVEN | Do not create patches until BaseLib path is ruled out |

## 7. Forbidden Patch Points Until Proven
These patch areas are forbidden until exact evidence shows they are necessary and safe.

| Patch Area | Status | Reason |
|---|---|---|
| Global reward randomization | FORBIDDEN | Too broad; can affect unrelated reward systems |
| Run initialization | FORBIDDEN | High save/run-state risk |
| Character/deck state mutation | FORBIDDEN | Can desync reward logic and player state |
| UI rendering only | FORBIDDEN | Preview-only changes can desync text and effect |
| Save/load serialization | FORBIDDEN | High persistence risk for MVP |
| Broad Ancient registry replacement | FORBIDDEN | Can break all Ancients if registry assumptions are wrong |
| Any transpiler patch | FORBIDDEN | Too fragile for first Ancient MVP |

Allowed later only with evidence:
- Narrow prefix/postfix at exact reward generation boundary.
- Narrow no-op logging probe that does not mutate behavior.
- Narrow replacement through BaseLib-supported API.

## 8. Evidence Required Before Implementation
Implementation is blocked until all entries are filled with evidence.

| Gate Item | Required Evidence | Current Value |
|---|---|---|
| Exact Ancient model class or registry location | Class/namespace/registry owner and source of evidence | UNKNOWN |
| Exact reward option model or pool type | Class/namespace/type shape and source of evidence | UNKNOWN |
| Exact reward generation timing | Method/event where choices are generated | UNKNOWN |
| Exact UI preview / reward resolution relationship | How display text maps to applied effect | UNKNOWN |
| Whether BaseLib can modify existing Ancient rewards | API evidence or explicit absence | UNKNOWN |
| Whether Harmony is required | Decision after BaseLib/template review | UNKNOWN |
| Safest no-op logging probe point | Exact method/API and why no mutation occurs | UNKNOWN |
| Rollback plan | Touched files, feature flag if any, revert path | UNKNOWN |
| One-Ancient MVP target | Observed reward, rationale, proposed minimal change | UNKNOWN |
| Test procedure | Steps to observe, trigger, verify, and inspect logs | UNKNOWN |

## 9. API Research Notes
See `docs/ANCIENT_REWARD_API_RESEARCH_NOTES.md`.

Key local findings:
- StS2 model class: `MegaCrit.Sts2.Core.Models.AncientEventModel`.
- StS2 event option class: `MegaCrit.Sts2.Core.Events.EventOption`.
- StS2 act-level Ancient members: `ActModel.AllAncients`, `_sharedAncientSubset`, `GetUnlockedAncients`, `SetSharedAncientSubset`, and `PullAncient`.
- BaseLib custom Ancient class: `BaseLib.Abstracts.CustomAncientModel : AncientEventModel`.
- BaseLib custom option helpers: `BaseLib.Utils.OptionPools`, `BaseLib.Utils.AncientOption`, `BaseLib.Utils.WeightedList<T>`.
- BaseLib internal patches add custom Ancients through `ActModel.GenerateRooms` and `ModelDb.AllSharedAncients`.

Remaining blocker:
- No local evidence yet proves that BaseLib can modify existing basegame Ancient reward options without a project-level Harmony patch.

## Research Output Checklist
- Update `docs/ANCIENT_REWARD_BALANCE_MAP.md` with observed facts.
- Update `docs/ANCIENT_REWARD_SPEC_v0.104.md` with chosen implementation strategy.
- Record all UNKNOWN items before implementation discussion.
- If any gate item remains UNKNOWN, implementation must not start.

## Completion Criteria
Research is complete only when:
- Every gate item has evidence or a documented blocker.
- A one-Ancient MVP target is selected.
- The implementation path avoids Harmony if a supported API exists.
- If Harmony is required, the patch point is exact, narrow, and justified.
- A no-op probe plan exists before mutation.
- A rollback plan exists.
- A manual test procedure exists.
