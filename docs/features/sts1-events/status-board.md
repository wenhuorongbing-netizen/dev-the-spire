# StS1 Events Status Board

## Overall Progress

| Phase | Status | Events | Done |
|-------|--------|--------|------|
| 0: Infrastructure | Done | — | — |
| 1: Canary | In Progress | 2 | 0 |
| 2: Simple Batch | Not Started | 21 | 0 |
| 3: Card Service | Not Started | 10 | 0 |
| 4: Combat | Not Started | 7 | 0 |
| 5: Custom UI | Not Started | 8 | 0 |
| 6: Pool Replacement | Not Started | — | — |

## Documentation Status

| Item | Status |
|------|--------|
| README.md | Done |
| goal.md | Done |
| wiki-event-catalog.md | Done |
| implementation-plan.md | Done |
| source-research/sts2-event-engine.md | Done |
| event-specs/ (48 unique events) | Done |
| assets.md | Done |
| localization.md | Done |
| test-plan.md | Done |

## Phase 1: Canary Events

| Event | Source | Loc EN | Loc ZHS | Asset | Test |
|-------|--------|--------|---------|-------|------|
| Big Fish | Done | Done | Done | Pending | Pending |
| Golden Idol | Done | Done | Done | Pending | Pending |

## Phase 2: Simple Batch (spec only)

| Event | Spec |
|-------|------|
| The Cleric | Done |
| Golden Wing | Done |
| Living Wall | Done |
| Old Beggar | Done |
| Bonfire Spirits | Done |
| Divine Fountain | Done |
| Duplicator | Done |
| Fountain of Cleansing | Done |
| The Lab | Done |
| Shining Light | Done |
| Mushrooms | Done |
| Altar | Done |
| Drug Dealer | Done |
| The Library | Done |
| Ancient Writing | Done |
| Augmenter | Done |
| Sensory Stone | Done |
| Moai Head | Done |
| Transmogrifier | Done |
| Upgrade Shrine | Done |

## Phase 3: Card Service Batch (spec only)

| Event | Spec |
|-------|------|
| Face Trader | Done |
| The Mausoleum | Done |
| Council of Ghosts | Done |
| Cursed Tome | Done |
| Knowing Skull | Done |
| Nest | Done |
| Vampires | Done |
| Falling | Done |
| Mind Bloom | Done |

## Phase 4: Combat Batch (spec only)

| Event | Spec |
|-------|------|
| Dead Adventurer | Done |
| Scorpion Nest | Done |
| Treasure Ooze | Done |
| Joust | Done |
| The Ssssserpent | Done |
| Masked Bandits | Done |
| Mysterious Sphere | Done |

## Phase 5: Custom UI Batch (spec only)

| Event | Spec |
|-------|------|
| The Woman in Blue | Done |
| Wheel of Change | Done |
| Designer | Done |
| Forgotten Altar | Done |
| The Ghost | Done |
| N'loth | Done |
| Tomb of Lord Red Mask | Done |
| Winding Halls | Done |

## Special Events (spec only)

| Event | Spec |
|-------|------|
| Neow | Done |
| Combat Start | Done (skip — StS2 has its own) |

## Blockers

- Regret curse card model — needed for Big Fish
- Injury curse card model — needed for Golden Idol
- Random relic reward helper — needed for Big Fish
- Card removal/transform/upgrade UI — needed for many events
- Combat encounter models — needed for Phase 4 events
