# M5 Revision L Warning Ledger

Date: 2026-06-10

Revision M supersession note, 2026-06-11: this warning ledger is historical owner-review context for the Revision L burn-down. The nullable warning blocker remains closed in beta.85 validation, but warning-clean source is not enabled-mode, gameplay, save-load, replacement, multiplayer, QA, or release-ready proof. Use `PROJECT_STATE.md`, `docs/goals/warning-ledger.md`, and the Revision M docs for current proof claims.

## Current Count

The current unique warning count is 0 after owner guards across Sts1Events model handlers.

| Code | Count | Current owner |
|---|---:|---|
| `CS8604` | 0 | Closed |
| `CS8602` | 0 | Closed |
| `CS8625` | 0 | Closed |
| **Total** | **0** | Closed |

The canonical per-file table is `docs/goals/warning-ledger.md`.

## Interpretation

- CanaryOnly, AdditiveBatch1, and draft/deferred Sts1Events rows are warning-clean.
- Formalization is still blocked by gameplay, render, save-load, image, replacement, multiplayer, and QA proof even though nullable warning debt is closed.

## Applied Burn-Down Pattern

Use an early owner guard in each event handler:

```csharp
if (Owner is not { } owner)
{
    return;
}
```

Then use the non-null `owner` local instead of `Owner` for player/card/relic helper calls.
