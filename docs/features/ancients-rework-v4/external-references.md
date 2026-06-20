# External References

Current active release note: implementation now targets the independent `EZMicroBalance` project. Legacy `EzDailyContent` references in older logs are historical; active code is under `EZMicroBalanceCode/Ancients/`, active resources are under `EZMicroBalance/`, and the active project is `EZMicroBalance.csproj`.

Use these references for the Ancients rework implementation. Re-check the live pages when working because Slay the Spire 2 and RitsuLib APIs may change during Early Access. BaseLib links below are retained only for historical comparison; Spire Plus no longer depends on BaseLib.

## Required User Reference

- RitsuLib tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/04-ritsulib/04-07-add-ancient/

Notes:

- The RitsuLib page shows the newer RitsuLib-style Ancient flow.
- It uses patterns such as `ModAncientEventTemplate`, `RegisterActAncient`, `RegisterSharedAncient`, `CreateModRelicOption<T>()`, `AllPossibleOptions`, and `GenerateInitialOptions()`.
- Current Spire Plus has already migrated to RitsuLib for compile and runtime dependency. Prefer RitsuLib and local game source over the historical BaseLib route when new Ancient work needs mod API support.

## Historical BaseLib Reference

- BaseLib tutorial: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/docs/03-baselib/03-07-add-ancient/

Notes:

- Historical May 2026 work referenced `Alchyr.Sts2.BaseLib` `3.1.4`; the current repo references `STS2.RitsuLib` `0.4.29` and has no Spire Plus BaseLib dependency.
- The BaseLib page shows `CustomAncientModel`, `IsValidForAct(ActModel act)`, `CustomScenePath`, map/run-history icon paths, `OptionPools`, `MakePool(...)`, and `AncientOption<T>()`.
- Prefer current local compile-time API evidence over tutorial assumptions when they conflict. Do not reintroduce BaseLib as a Spire Plus dependency without an explicit owner decision, migration document, build, package, and runtime proof.

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
