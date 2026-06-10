# ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS

## Status

**Resolved for current source warning debt / runtime evidence still pending.**

Current forced project and solution builds record 0 nullable warnings and 0 errors after owner guards were expanded across the compile-included Sts1Events model set. The former 70-warning snapshot is historical burn-down context only.

## Warning Matrix

| Code | Active count | Scope | Disposition |
| --- | ---: | --- | --- |
| `CS8604` | 0 | Possible null argument passed to player/card/relic helper APIs | Cleared by owner guards |
| `CS8602` | 0 | Possible null dereference in event option handlers | Cleared by owner guards |
| `CS8625` | 0 | Null literal passed to a non-nullable reference | Cleared earlier |

## Evidence

- `dotnet build EZMicroBalance.csproj -m:1 --no-incremental`: passed with 0 warnings / 0 errors on 2026-06-10.
- `dotnet build EZMicroBalance.sln -m:1 --no-incremental`: passed with 0 warnings / 0 errors on 2026-06-10.
- Current fix pattern: event option handlers early-exit when `Owner` is missing, then use a non-null `owner` local.
- Historical per-file burn-down detail is preserved in `docs/goals/warning-ledger.md` and `docs/reviews/warning-triage-matrix.md`.

## Decision

- Nullable warning debt is closed for the current dirty source build.
- Sts1Events remains default Off/prototype-gated until current-runtime, gameplay, save-load, render, replacement, and multiplayer proof exists.
- CanaryOnly and AdditiveBatch1 source warnings are cleared; live canary/AdditiveBatch1 gameplay proof is still pending.

## Cleanup Rules

1. Keep explicit player/run/event-state guards when editing Sts1Events models.
2. Prefer fail-closed event option behavior over null-forgiving operators.
3. Recount warnings only from a forced build when preparing handoff.
4. Keep `docs/reviews/current-validation.md` and this issue aligned on the current warning count.
