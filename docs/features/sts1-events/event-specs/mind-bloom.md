# Mind Bloom — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| "I am War" | Fight a boss from Act 1. Reward: relic. |
| "I am Awake" | Upgrade all cards in your deck. |
| "I am Rich" | Gain 999 gold. Obtain 2 Curses (Normality). |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: "I am Rich" gives 3 Curses (Normality) instead of 2.

## StS2 Implementation

### Class: `Sts1MindBloom`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 3 event with `content.ActEvent<Glory, Sts1MindBloom>()`.
- **Layout:** Default / Combat hybrid

### Localization Keys
```
STS1_MIND_BLOOM.title
STS1_MIND_BLOOM.pages.INITIAL.description
STS1_MIND_BLOOM.pages.INITIAL.options.WAR.title / .description
STS1_MIND_BLOOM.pages.INITIAL.options.AWAKE.title / .description
STS1_MIND_BLOOM.pages.INITIAL.options.RICH.title / .description
STS1_MIND_BLOOM.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Act 1 boss encounter model
- Upgrade all cards command
- Normality curse card model (×2 normal, ×3 A15+)
- Gold reward (999)
