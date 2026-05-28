# StS1 Events Status Board

## Overall Progress

| Phase | Status | Events | Done |
|-------|--------|--------|------|
| 0: Infrastructure | In Progress | — | — |
| 1: Canary | Not Started | 2 | 0 |
| 2: Simple Batch | Not Started | 21 | 0 |
| 3: Card Service | Not Started | 10 | 0 |
| 4: Combat | Not Started | 7 | 0 |
| 5: Custom UI | Not Started | 8 | 0 |
| 6: Pool Replacement | Not Started | — | — |

## Phase 1: Canary Events

| Event | Source | Loc EN | Loc ZHS | Asset | Test |
|-------|--------|--------|---------|-------|------|
| Big Fish | ❌ | ❌ | ❌ | ❌ | ❌ |
| Golden Idol | ❌ | ❌ | ❌ | ❌ | ❌ |

## Blockers

- Regret curse card model — needed for Big Fish
- Injury curse card model — needed for Golden Idol
- Golden Idol relic model — needed for Golden Idol (check if StS2 has one)
- Random relic reward helper — needed for Big Fish

## Notes

- All events use RitsuLib `ModEventTemplate` + `[RegisterSharedEvent]` or `[RegisterActEvent]`
- No original StS1 art committed; use extraction script
- Localization in `EZMicroBalance/localization/{lang}/sts1_events.json`
