# Council of Ghosts — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Accept | Obtain 5 Apparition cards (Ethereal). Lose 50% max HP. |
| Refuse | Nothing happens. |

Apparition is a 1-cost Skill: Gain 1 Intangible. Ethereal.

### Ascension Differences
- A15+: Obtain 3 Apparitions instead of 5 (same max HP loss).

## StS2 Implementation

### Class: `Sts1CouncilOfGhosts`
- **Registration:** `[RegisterActEvent(typeof(Act2Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_COUNCIL_OF_GHOSTS.title
STS1_COUNCIL_OF_GHOSTS.pages.INITIAL.description
STS1_COUNCIL_OF_GHOSTS.pages.INITIAL.options.ACCEPT.title / .description
STS1_COUNCIL_OF_GHOSTS.pages.INITIAL.options.REFUSE.title / .description
STS1_COUNCIL_OF_GHOSTS.pages.ACCEPT.description
```

### Dependencies
- Apparition card model (×5 normal, ×3 A15+)
- 50% max HP loss
