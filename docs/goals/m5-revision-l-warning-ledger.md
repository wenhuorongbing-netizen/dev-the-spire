# M5 Revision L Warning Ledger

Date: 2026-06-10

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
