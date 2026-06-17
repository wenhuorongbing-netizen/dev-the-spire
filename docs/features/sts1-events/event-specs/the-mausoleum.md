# The Mausoleum — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Open the Coffin | 50% chance: Obtain a random relic. 50% chance: Obtain a Curse (Wound). |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Always obtain the Curse (Wound), no relic chance.

## StS2 Implementation

### Class: `Sts1TheMausoleum`
- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1TheMausoleum>()`.
- **Layout:** Default

### Localization Keys
```
STS1_THE_MAUSOLEUM.title
STS1_THE_MAUSOLEUM.pages.INITIAL.description
STS1_THE_MAUSOLEUM.pages.INITIAL.options.OPEN.title / .description
STS1_THE_MAUSOLEUM.pages.INITIAL.options.LEAVE.title / .description
STS1_THE_MAUSOLEUM.pages.OPEN_RELIC.description
STS1_THE_MAUSOLEUM.pages.OPEN_CURSE.description
```

### Dependencies
- Wound curse card model
- Random relic reward helper
- RNG for 50/50 chance
