# Ancient Reward API Research Notes

Date: 2026-05-02

Scope: local metadata/signature research only. No gameplay implementation was performed.

## Summary
Local inspection found evidence for the core Ancient event model and option representation used by StS2, plus previous framework APIs for defining custom Ancients and their option pools.

The research does not yet prove a verified non-mutating way to tune existing basegame Ancient rewards. The implementation gate remains closed.

An approved no-op logging probe has been implemented for research only. It observes Ancient option generation metadata and must not mutate rewards, options, UI text, player state, run state, room state, act state, save data, or RNG state.

Manual verification reported on 2026-05-03: `AncientRewardNoopProbe` log entries appeared in `godot.log`; previous framework and EzDailyContent were enabled; Ancient options appeared normally; option selection worked normally; the game did not crash; no `AncientRewardNoopProbe` exception appeared; no visible gameplay behavior changed. Exact log lines were not copied into this document.

Most important finding:
- StS2 Ancient events are represented by `MegaCrit.Sts2.Core.Models.AncientEventModel`.
- Ancient reward/choice UI options are represented at the event layer by `MegaCrit.Sts2.Core.Events.EventOption`.
- previous framework `CustomAncientModel` extends `AncientEventModel` and exposes `OptionPools`, `AncientOption`, and `WeightedList` helpers for custom Ancient option generation.
- previous framework metadata shows internal Harmony patches that add custom Ancients to act/model pools, but no direct local evidence was found for mutating existing basegame Ancient rewards.

## Confirmed Facts

| Finding | Evidence Source | Signature or API | Confidence | Fact or Hypothesis | Risk if Wrong | Next Verification |
|---|---|---|---|---|---|---|
| Base Ancient model class exists | Reflection over `D:/Steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64/sts2.dll` | `MegaCrit.Sts2.Core.Models.AncientEventModel : MegaCrit.Sts2.Core.Models.EventModel` | High | Fact | Low; reflection may miss behavior but not type identity | Inspect derived/basegame model data source |
| Ancient events expose generated options | Reflection over `sts2.dll` | `AncientEventModel.GeneratedOptions : List<EventOption>` nonpublic property; `AllPossibleOptions : IEnumerable<EventOption>` public property | High | Fact | Medium; behavior still unknown | Inspect call flow around option generation |
| Event options are represented by `EventOption` | Reflection over `sts2.dll` | `MegaCrit.Sts2.Core.Events.EventOption` with `TextKey`, `Title`, `Description`, `OnChosen`, `Relic`, `Chosen()` | High | Fact | Medium; UI relationship still needs verification | Inspect event layout/button binding |
| Ancient generation is tied to event start flow | Reflection over `sts2.dll` | `EventModel.BeginEvent(...)`, `EventModel.GenerateInitialOptionsWrapper()`, `EventModel.GenerateInitialOptions()`, `AncientEventModel.GenerateInitialOptionsWrapper()` | Medium | Fact for method existence; hypothesis for exact call order | High if patching wrong method | Verify call order with no-op probe or decompilation structure |
| Act model owns Ancient lists/subsets | Reflection over `sts2.dll` | `ActModel.AllAncients`, `ActModel.Ancient`, `_sharedAncientSubset`, `GetUnlockedAncients(...)`, `SetSharedAncientSubset(...)`, `PullAncient()` | High | Fact for member existence | Medium; registry source still unknown | Inspect how basegame act data populates these members |
| previous framework supports custom Ancient models | Reflection and XML docs from `C:/Users/Jack/.nuget/packages/previous-framework-package/3.1.0/lib/net9.0/previous framework.dll` and `previous framework.xml` | `previous framework.Abstracts.CustomAncientModel : AncientEventModel` | High | Fact | Low for custom Ancient support; medium for tuning scope | Inspect examples/source for intended usage |
| previous framework custom Ancient options are relic-model based | Reflection over `previous framework.dll` | `previous framework.Utils.AncientOption.ModelForOption : RelicModel`, `AllVariants : IEnumerable<RelicModel>` | High | Fact | Medium; basegame options may still vary | Compare with basegame Ancient option construction |
| previous framework custom option pools are weighted | Reflection/XML over `previous framework.dll`/`previous framework.xml` | `previous framework.Utils.OptionPools`, `WeightedList<AncientOption>`, `OptionPools.Roll(Rng)` | High | Fact for previous framework custom pool API | Medium; not proven for basegame pool internals | Inspect basegame model behavior |
| previous framework uses internal Harmony to add custom Ancients | Reflection over `previous framework.dll` attributes | `HarmonyPatch(ActModel, GenerateRooms)` prefix in `previous framework.Patches.Content.AddCustomAncientsToPool`; `HarmonyPatch(ModelDb, AllSharedAncients)` postfix in `CustomAncientExistence` | High | Fact | Medium; does not prove mod-level patch need | Inspect whether public API can avoid our own patches |

## Hypotheses

| Hypothesis | Evidence | Confidence | Risk | Next Verification |
|---|---|---|---|---|
| `AncientEventModel.GenerateInitialOptionsWrapper()` may be a no-op logging probe candidate | It exists as a nonpublic Ancient-specific wrapper returning `IReadOnlyList<EventOption>` | Medium | Nonpublic method may be fragile or too late/early | Confirm call order via decompiled structure or a separately approved no-op probe |
| `EventOption.TextKey`, `Title`, and `Description` drive preview text | These properties exist on `EventOption`; `EventModel` has `GetOptionTitle` and `GetOptionDescription` | Medium | UI may use additional layout state | Inspect `NEventLayout`/button binding or test in game |
| Existing basegame Ancient reward tuning may require Harmony | No direct previous framework API for mutating existing Ancient rewards was found in local XML/reflection | Low-medium | Could miss source-only extension points or intended APIs | Inspect previous framework source/examples before deciding |
| Additive custom Ancient work may not require our own Harmony patches | previous framework provides `CustomAncientModel` and internal patches for adding custom Ancients | Medium | Does not solve existing reward tuning | Confirm with previous framework examples and a separate approved implementation plan |

## Unknowns

| Unknown | Current Status | Why It Remains Unknown |
|---|---|---|
| Exact basegame Ancient registry data source | UNKNOWN | `ActModel` members are known, but the data source/population path was not fully traced |
| Exact basegame reward pool type | UNKNOWN | previous framework custom pool type is known; basegame pool representation is not yet proven |
| Exact reward generation call order | PARTIAL | Relevant method signatures are known, but method body/call order was not copied or fully traced |
| Exact UI preview to reward resolution relationship | PARTIAL | `EventOption` properties are known, but UI binding and choice resolution path need verification |
| Whether previous framework can modify existing Ancient rewards | NO DIRECT API FOUND | Local XML/reflection showed custom Ancient support, not explicit existing-reward mutation support |
| Whether Harmony is required for existing reward tuning | UNKNOWN | Cannot decide until previous framework source/examples and basegame call flow are checked |
| No-op logging probe point | OBSERVED WORKING NO-OP PROBE | `AncientEventModel.GenerateInitialOptionsWrapper()` has been manually observed in game as a working no-op probe. This does not approve reward tuning. |
| One-Ancient MVP target | UNKNOWN | No observed reward catalog exists yet |
| Repeatable test procedure | UNKNOWN | In-game route to specific Ancient rewards is not documented yet |

## previous framework Findings

Sources:
- `C:/Users/Jack/.nuget/packages/previous-framework-package/3.1.0/lib/net9.0/previous framework.dll`
- `C:/Users/Jack/.nuget/packages/previous-framework-package/3.1.0/lib/net9.0/previous framework.xml`

Relevant signatures:
- `previous framework.Abstracts.CustomAncientModel : MegaCrit.Sts2.Core.Models.AncientEventModel`
- `CustomAncientModel.IsValidForAct(ActModel act)`
- `CustomAncientModel.ShouldForceSpawn(ActModel act, AncientEventModel rngChosenAncient)`
- `CustomAncientModel.GenerateInitialOptions() : IReadOnlyList<EventOption>` nonpublic
- `CustomAncientModel.OptionPools : previous framework.Utils.OptionPools`
- `CustomAncientModel.MakePool(RelicModel[] options) : WeightedList<AncientOption>`
- `CustomAncientModel.MakePool(AncientOption[] options) : WeightedList<AncientOption>`
- `CustomAncientModel.AncientOption<T>(int weight, Func<T, RelicModel> relicPrep, Func<T, IEnumerable<RelicModel>> makeAllVariants)`
- `previous framework.Utils.AncientOption`
- `previous framework.Utils.AncientOption<T>`
- `previous framework.Utils.OptionPools.Roll(Rng rng) : List<AncientOption>`
- `previous framework.Utils.WeightedList<T>.GetRandom(Rng rng)`

previous framework patch metadata:
- `previous framework.Patches.Content.AddCustomAncientsToPool` has `HarmonyPatch(ActModel, GenerateRooms)` and a Harmony prefix method.
- `previous framework.Patches.Content.CustomAncientExistence` has `HarmonyPatch(ModelDb, AllSharedAncients)` and a Harmony postfix method.

Interpretation:
- previous framework clearly supports adding custom Ancient models and options.
- previous framework likely manages custom Ancient pool insertion through its own patches.
- Local metadata does not prove an API for modifying existing basegame Ancient reward options.

## StS2 Assembly Findings

Source:
- `D:/Steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64/sts2.dll`

Relevant signatures:
- `MegaCrit.Sts2.Core.Models.AncientEventModel : EventModel`
- `AncientEventModel.GeneratedOptions : List<EventOption>` nonpublic
- `AncientEventModel.AllPossibleOptions : IEnumerable<EventOption>`
- `AncientEventModel.GenerateInitialOptionsWrapper() : IReadOnlyList<EventOption>` nonpublic
- `AncientEventModel.RelicOption(...) : EventOption` nonpublic overloads
- `MegaCrit.Sts2.Core.Models.EventModel.CurrentOptions : IReadOnlyList<EventOption>`
- `EventModel.BeginEvent(Player player, bool isPreFinished) : Task`
- `EventModel.GenerateInitialOptionsWrapper() : IReadOnlyList<EventOption>` nonpublic
- `EventModel.GenerateInitialOptions() : IReadOnlyList<EventOption>` nonpublic
- `EventModel.SetEventState(LocString description, IEnumerable<EventOption> eventOptions)` nonpublic
- `MegaCrit.Sts2.Core.Events.EventOption`
- `EventOption.Chosen() : Task`
- `EventOption.FromRelic(RelicModel relic, EventModel eventModel, Func<Task> onChosen, string textKey) : EventOption`
- `EventOption.WithRelic(...) : EventOption`
- `MegaCrit.Sts2.Core.Models.ActModel.AllAncients : IEnumerable<AncientEventModel>`
- `ActModel._sharedAncientSubset : List<AncientEventModel>` nonpublic field
- `ActModel.GetUnlockedAncients(UnlockState state) : IEnumerable<AncientEventModel>`
- `ActModel.SetSharedAncientSubset(List<AncientEventModel> sharedAncientSubset)`
- `ActModel.PullAncient() : EventModel`
- `MegaCrit.Sts2.Core.Nodes.Events.NAncientEventLayout`
- `NAncientEventLayout.SetDialogue(IReadOnlyList<AncientDialogueLine> lines)`
- `NAncientEventLayout.OnSetupComplete()`

Interpretation:
- The event-layer model and option classes are now known.
- Exact basegame reward pool population and option-generation behavior still require call-flow verification.
- UI preview behavior is partially mapped through `EventOption`, but not fully verified.

## Candidate No-Op Probe Points

These are candidates only. They are not approved implementation points.

| Candidate | Evidence | Why Candidate | Risk | Required Before Use |
|---|---|---|---|---|
| Postfix on `AncientEventModel.GenerateInitialOptionsWrapper()` | Reflection signature: `protected instance virtual final IReadOnlyList<EventOption> MegaCrit.Sts2.Core.Models.AncientEventModel.GenerateInitialOptionsWrapper()` | Could log generated options without mutating them | Nonpublic and beta-fragile; call order not verified | Explicit approval, no-op-only patch plan, rollback; see `docs/ANCIENT_REWARD_NOOP_PROBE_SPEC.md` |
| Postfix on `EventModel.SetEventState(...)`, filtered to `AncientEventModel` | Nonpublic method receives event options | Could observe final current options | Broad base event method; filtering mistake could affect all events | Prefer Ancient-specific point first |
| Logging through previous framework diagnostics if available | previous framework includes diagnostics/logging-related types | Could avoid direct game patching | Not yet tied to Ancient generation | Inspect previous framework logging examples |

## Implemented No-Op Probe

Implementation file:
- `EzDailyContentCode/AncientRewardNoopProbe.cs`

Approved target:
- `MegaCrit.Sts2.Core.Models.AncientEventModel.GenerateInitialOptionsWrapper()`

Patch shape:
- Harmony postfix observer.
- No prefix.
- No transpiler.
- No finalizer.
- No return-value replacement.

Logged fields:
- Ancient runtime type.
- Option count.
- Option runtime type.
- `TextKey`, if accessible.
- Whether `Relic` is null.
- `IsLocked`.
- `IsProceed`.
- `ShouldSaveChoiceToHistory`.

Not logged:
- `Title`.
- `Description`.
- Full localized text.
- Full object serialization.
- Player deck.
- Save data.
- Personal filesystem paths.

No-op constraints:
- Does not call `EventOption.Chosen()`.
- Does not call `EventOption.WithRelic(...)`.
- Does not mutate `EventOption` properties.
- Does not mutate `AncientEventModel.GeneratedOptions`.
- Does not mutate player, deck, relic, reward, run, room, act, save, or RNG state.
- If logging fails, it logs the exception type/message if possible and does not change return values.

What remains UNKNOWN:
- Exact call order around `GenerateInitialOptionsWrapper()`.
- Whether this is the lowest-risk long-term observation point.
- Whether all logged getters are permanently side-effect free across public beta updates.
- Whether previous framework has a better public diagnostic hook.
- Whether previous framework can modify existing basegame Ancient rewards.
- Whether Harmony is required for future reward tuning.
- One-Ancient MVP target.
- Repeatable Ancient reward test path is PARTIAL: probe logs have been collected for Neow, Pael, and Tanx, but a route to reliably reproduce specific Ancients is not fully documented.

Manual verification status:
- Status: observed in game as a working no-op probe.
- Evidence: user reported `AncientRewardNoopProbe` entries in `godot.log`.
- previous framework: enabled.
- EzDailyContent: enabled.
- Ancient options: appeared normally.
- Option selection: worked normally.
- Crash: none observed.
- Probe exception: none observed.
- Visible gameplay behavior change: none observed.
- Reward tuning gate: still closed.

## Observed Probe Log Catalog Entries

Source: user-provided `godot.log` lines from `AncientRewardNoopProbe`.

Observed facts only:
- Ancient runtime types observed:
  - `MegaCrit.Sts2.Core.Models.Events.Neow`
  - `MegaCrit.Sts2.Core.Models.Events.Pael`
  - `MegaCrit.Sts2.Core.Models.Events.Tanx`
- Each observed Ancient generated `OptionCount=3`.
- All observed options used `OptionType=MegaCrit.Sts2.Core.Events.EventOption`.
- All observed options had `RelicIsNull=False`.
- All observed options had `IsLocked=False`.
- All observed options had `IsProceed=False`.
- All observed options had `ShouldSaveChoiceToHistory=True`.

Observed option rows:

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

Do not infer reward effects, reward strength, or final implementation path from these `TextKey` values.
One-Ancient MVP target remains UNKNOWN.
previous framework existing reward modification remains NO DIRECT API FOUND / UNKNOWN.
Harmony requirement for reward tuning remains UNKNOWN.

Manual game test steps:
1. Run `dotnet build`.
2. Run `dotnet publish`.
3. Launch Slay the Spire 2 public beta `v0.104.0`.
4. Confirm previous framework is enabled in Mod Settings.
5. Confirm EzDailyContent is enabled in Mod Settings.
6. Reach an Ancient event.
7. Confirm the Ancient options appear normally.
8. Select an option normally.
9. Inspect `godot.log` for `[AncientRewardNoopProbe]` entries.
10. Confirm no log errors, no option-count changes, and no visible gameplay behavior change.

## Forbidden Patch Points
Remain forbidden:
- Global reward randomization.
- Run initialization.
- Save/load serialization.
- UI-only rendering changes.
- Transpiler patches.
- Direct player/deck state mutation.
- Broad Ancient registry replacement.
- Any mutation before a no-op probe confirms timing.

## Next Research Tasks
1. Inspect previous framework source/examples for intended `CustomAncientModel` usage.
2. Inspect StS2 call flow around `AncientEventModel.GenerateInitialOptionsWrapper()` without copying method bodies.
3. Inspect how `NAncientEventLayout` binds `EventOption.Title`, `Description`, and `Chosen()`.
4. Determine whether basegame Ancient options are generated from relic models, event options, or another data source.
5. Document a manual test path to reach Ancient reward choices in game.
6. Build an observed Ancient reward catalog before selecting an MVP target.
