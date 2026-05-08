# External References

Current active release note: implementation now targets the independent `EZMicroBalance` project. Legacy `EzDailyContent` references in older logs are historical; active code is under `EZMicroBalanceCode/Ancients/`, active resources are under `EZMicroBalance/`, and the active project is `EZMicroBalance.csproj`.

Use these references for the Ancients rework implementation. Re-check the live pages when working because Slay the Spire 2, BaseLib, and RitsuLib APIs may change during Early Access.

## Required User Reference

- RitsuLib tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/04-ritsulib/04-07-add-ancient/

Notes:

- The RitsuLib page shows the newer RitsuLib-style Ancient flow.
- It uses patterns such as `ModAncientEventTemplate`, `RegisterActAncient`, `RegisterSharedAncient`, `CreateModRelicOption<T>()`, `AllPossibleOptions`, and `GenerateInitialOptions()`.
- RitsuLib is allowed if it is the cleanest practical way to implement this feature. If using it requires migrating away from BaseLib, create a documented migration subphase first and record package, API, build, and runtime implications before broad gameplay changes.

## Current Project Library Reference

- BaseLib tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/03-baselib/03-07-add-ancient/

Notes:

- This repo currently references `Alchyr.Sts2.BaseLib` `3.1.2`, so BaseLib-compatible APIs are the first implementation target.
- The BaseLib page shows `CustomAncientModel`, `IsValidForAct(ActModel act)`, `CustomScenePath`, map/run-history icon paths, `OptionPools`, `MakePool(...)`, and `AncientOption<T>()`.
- Prefer current local compile-time API evidence over tutorial assumptions when they conflict. If BaseLib blocks the implementation and RitsuLib solves it cleanly, document the reason and switch only through a build-verified migration step.

## Local Source References

- `docs/features/ancients-rework-v4/source-design.md`
- `docs/features/ancients-rework-v4/implementation-plan.md`
- `docs/features/ancients-rework-v4/api-discovery.md`
- `EZMicroBalanceCode/Ancients/`
- `EZMicroBalance.csproj`
- Legacy probe, not active release code: `EzDailyContentCode/AncientRewardNoopProbe.cs`
- `docs/dev-environment.md`

## Logging Requirement

Every implementation goal should leave a short work record in `docs/features/ancients-rework-v4/work-log.md`:

- timestamp/date
- goal prompt summary
- files read
- external references checked
- commands run
- files changed
- build/publish result
- blockers and next prompt
