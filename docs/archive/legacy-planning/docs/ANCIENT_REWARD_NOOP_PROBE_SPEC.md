# Ancient Reward No-Op Logging Probe Specification

This document is a design spec only. It does not authorize implementation.

## Current Status
- Probe status: CANDIDATE ONLY.
- Gameplay implementation: not approved.
- Patch implementation: not approved.
- Target game: Slay the Spire 2 public beta `v0.104.0`, date `2026.04.23`.
- Manifest id: `EzDailyContent`.

## 1. Why This Probe Point Is Being Considered
`AncientEventModel.GenerateInitialOptionsWrapper()` is being considered because local metadata shows it is Ancient-specific and returns the generated event options.

Research goal:
- Observe Ancient reward/event options after they are generated.
- Log enough metadata to understand reward generation timing and option shape.
- Avoid mutating options, return values, player state, run state, reward pools, UI, or save data.

This is preferable to broader candidates because:
- It is more specific than patching base `EventModel` methods.
- It returns a list of `EventOption`, which is directly relevant to Ancient reward choices.
- It may allow observation before selecting an MVP target.

Remaining blocker:
- Exact call order is still UNKNOWN.
- Whether this method is the lowest-risk point is still UNKNOWN.
- No-op behavior is not proven until an explicitly approved probe is tested.

## 2. Exact Class and Method Signature Found
Evidence source:
- Local reflection over `D:/Steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64/sts2.dll`.

Found signature:

```text
protected instance virtual final System.Collections.Generic.IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> MegaCrit.Sts2.Core.Models.AncientEventModel.GenerateInitialOptionsWrapper()
```

Related option type:

```text
MegaCrit.Sts2.Core.Events.EventOption
```

Relevant observed `EventOption` members:
- `TextKey`
- `Title`
- `Description`
- `Relic`
- `HoverTips`
- `IsLocked`
- `IsProceed`
- `WasChosen`
- `ShouldSaveChoiceToHistory`
- `Chosen()`

## 3. Visibility, Virtualness, and Staticness
- Class: `MegaCrit.Sts2.Core.Models.AncientEventModel`
- Method: `GenerateInitialOptionsWrapper`
- Visibility: `protected`
- Static or instance: `instance`
- Virtual: `virtual final`
- Return type: `IReadOnlyList<EventOption>`
- Parameters: none

Interpretation:
- The method is not public.
- The method is not static.
- The method appears to be an override/sealed-style method at CLR level.
- Direct normal C# calls from mod code are not an appropriate probe path.

## 4. Whether Harmony Patching Is Required
Current answer: LIKELY for this exact probe point, but not fully proven.

Reason:
- The candidate method is nonpublic/protected.
- No public previous framework hook for observing existing basegame Ancient option generation has been found locally.
- A no-op observation of the returned `IReadOnlyList<EventOption>` would likely require a Harmony postfix or equivalent instrumentation.

Still UNKNOWN:
- Whether previous framework has a diagnostic hook or source-level example that avoids project-level Harmony.
- Whether another lower-risk public method can provide the same evidence.

Rule:
- Do not implement a Harmony patch from this spec. A separate explicit implementation approval is required.

## 5. Whether the Probe Can Log Without Mutating Return Values or Options
Design intent: yes, if implemented as a read-only postfix.

Required no-op constraints:
- Do not replace `__result`.
- Do not reorder options.
- Do not add or remove options.
- Do not call `EventOption.Chosen()`.
- Do not call `EventOption.WithRelic(...)`.
- Do not mutate `EventOption` properties.
- Do not mutate `AncientEventModel.GeneratedOptions`.
- Do not mutate player, deck, relic, reward, run, room, act, or RNG state.

Still UNKNOWN:
- Whether simply reading all candidate properties is side-effect free.
- Whether `Title` or `Description` evaluation can trigger dynamic localization side effects.

Conservative logging should avoid evaluating expensive or dynamic members until verified as read-only.

## 6. Data That Would Be Logged
Allowed candidate log fields:
- Game version if available through an already-known read-only source.
- Mod version if available.
- Ancient model runtime type full name.
- Ancient model id if accessible through verified read-only access.
- Number of returned options.
- For each returned option:
  - option runtime type full name.
  - `TextKey` if non-null and accessible through verified read-only access.
  - whether `Relic` is null.
  - relic model id/name only if accessible through verified read-only access.
  - `IsLocked`.
  - `IsProceed`.
  - `ShouldSaveChoiceToHistory`.

Prefer counts and ids over full localized text.

## 7. Data That Must NOT Be Logged
Do not log:
- Full player deck list.
- Full save data.
- Personal filesystem paths.
- Large localized text dumps.
- Decompiled method bodies.
- Full object serialization.
- RNG state if reading it mutates or exposes sensitive run internals.
- Multiplayer/session identifiers unless explicitly verified as appropriate to log.
- Anything requiring calls to `Chosen()`, reward resolution, or option mutation helpers.

## 8. How to Ensure the Probe Is No-Op
Implementation, if later approved, must obey this checklist:
- Use postfix-only observation unless a better no-op hook is proven.
- Treat `__result` as read-only.
- Store no persistent state.
- Avoid feature flags or config for the first probe unless required for controlled disablement.
- Log once per Ancient event generation or with strict duplicate suppression.
- Do not modify return values.
- Do not catch and hide mutation failures as success.
- If logging throws, fail without changing the generated options.
- Run `dotnet build`.
- Run `dotnet publish` only if implementation/package changes require it.
- Verify Mod Settings still show previous framework and EzDailyContent enabled.

No-op proof required:
- Before/after option count unchanged.
- No visible gameplay behavior change.
- No reward choice differences attributable to the mod.
- No errors in `godot.log`.

## 9. How to Disable or Remove the Probe
Preferred removal:
- Revert the probe implementation commit.
- Re-run `dotnet build`.
- Re-run `dotnet publish` if the DLL was published.
- Confirm game loads with EzDailyContent enabled.

Runtime disable strategy:
- UNKNOWN.

Reason:
- No config system is approved.
- No implementation file exists.
- A future implementation spec must decide whether a compile-time-only probe is acceptable or whether a verified non-mutating runtime flag is needed.

## 10. Files That Would Be Touched If Later Approved
No files are approved for implementation yet.

Likely future touched files, pending explicit approval:
- One new C# probe/patch file, exact path UNKNOWN.
- Possibly `MainFile.cs` only if explicit patch registration is required; this is disfavored and currently UNKNOWN.
- `docs/ANCIENT_REWARD_API_RESEARCH_NOTES.md` to record probe results.
- `docs/ANCIENT_REWARD_RESEARCH_PLAN.md` to update gate status.
- `docs/ANCIENT_REWARD_BALANCE_MAP.md` if observed options become catalog evidence.

Do not create implementation folders or files from this spec.

## 11. Build, Publish, and Test Steps

### Before implementation approval
- `dotnet build`
- Confirm working tree only contains intended documentation changes.

### If implementation is later approved
- Create a git checkpoint before code changes.
- Add the minimal no-op probe.
- Run `dotnet build`.
- Run `dotnet publish` if DLL/published artifacts must be updated.
- Launch Slay the Spire 2.
- Confirm previous framework and EzDailyContent are enabled.
- Reach an Ancient event.
- Confirm logs contain probe entries.
- Confirm reward options still appear and can be selected normally.
- Inspect `godot.log` for errors.

## 12. Rollback Steps
If a later probe implementation causes issues:
- Disable EzDailyContent in Mod Settings if possible.
- Remove `<GameRoot>\mods\EzDailyContent` if the game cannot launch.
- Revert the probe implementation commit.
- Run `dotnet build`.
- Run `dotnet publish`.
- Relaunch and confirm previous framework and EzDailyContent load.
- Record failure in `docs/ANCIENT_REWARD_API_RESEARCH_NOTES.md`.

## 13. Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---:|---:|---|
| Method call order is misunderstood | Medium | High | Verify with no-op-only logging before mutation |
| Nonpublic method changes in a future beta | High | Medium | Keep probe isolated and version-gated in docs |
| Reading properties triggers side effects | Low-medium | Medium | Start with type/count/TextKey only |
| Logging spam obscures real errors | Medium | Low | Log once per generation with concise fields |
| Harmony patch affects generation accidentally | Medium | High | Postfix-only, do not mutate `__result` |
| Probe becomes accidental implementation foundation | Medium | High | Keep implementation gate closed until MVP target is selected |

## 14. Approval Gate Before Implementation
Implementation requires a new explicit user request.

The approval request must explicitly allow:
- Creating one no-op probe patch.
- Which file path may be created.
- Whether `MainFile.cs` may be touched.
- Whether `dotnet publish` should run.
- Whether game launch/manual verification should be performed.

Until then:
- Do not create patches.
- Do not create probe code.
- Do not change gameplay.
- Do not update manifest/build/package settings.
