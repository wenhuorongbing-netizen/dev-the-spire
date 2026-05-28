# StS1 Events Status Board

## Overall Progress

| Phase | Status | Events | Code Done |
|-------|--------|--------|-----------|
| 0: Infrastructure | Done | — | — |
| 1: Canary | Code Done | 2 | 2 |
| 2: Simple Batch | Code Done | 21 | 21 |
| 3: Card Service | Code Done | 10 | 10 |
| 4: Combat | Code Done | 7 | 7 |
| 5: Custom UI | Code Done | 8 | 8 |
| 6: Pool Replacement | Not Started | — | — |

## Totals

| Item | Count |
|------|-------|
| Event spec documents | 48 |
| Event code files (C#) | 46 |
| Registry file | 1 |
| Localization files (EN + ZHS) | 2 |
| Asset scripts | 2 |
| Manifest files | 3 |

## Build Status

- `dotnet build`: **0 errors**, 45 nullable warnings (safe in practice)

## What's Implemented

Every event has:
- ✅ C# model class with options, effects, A15 logic
- ✅ Detailed spec document with Wiki behavior
- ✅ Localization key definitions

## What's Still TODO (per event)

Many events have `// TODO` comments for:
- Card removal/transform/upgrade UI integration
- Random relic/potion/card reward helpers
- Combat encounter models (Phase 4)
- Curse card model references (Regret, Injury, Doubt, etc.)

## Blockers for Full Functionality

1. **Curse card models**: Regret, Injury, Doubt, Wound, Parasite, Normality, Madness, Decay — check which exist in StS2
2. **Random relic reward helper** — `RelicFactory.PullNextRelicFromFront` exists but needs testing
3. **Card removal/transform/upgrade UI** — need to find StS2 API for these
4. **Combat encounter models** — need to define encounters for Phase 4 events
5. **Event registration** — need to wire up RitsuLib `RegisterActEvent`/`RegisterSharedEvent` attributes or content pack registration
6. **Event pool replacement** — Phase 6: patch event selection to use StS1-only pool

## Next Steps

1. Wire up RitsuLib registration (add attributes or content pack)
2. Resolve curse card model references
3. Implement card UI integration
4. Test canary events in-game via debug spawn
5. Implement Phase 6 pool replacement
