# Cursed Tome — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Read the Book | Take 10 HP damage. Obtain a random rare relic. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Take 15 HP damage instead of 10.

## StS2 Implementation

### Class: `Sts1CursedTome`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1CursedTome>()`.
- **Layout:** Default

### Localization Keys
```
STS1_CURSED_TOME.title
STS1_CURSED_TOME.pages.INITIAL.description
STS1_CURSED_TOME.pages.INITIAL.options.READ.title / .description
STS1_CURSED_TOME.pages.INITIAL.options.LEAVE.title / .description
STS1_CURSED_TOME.pages.READ.description
```

### Dependencies
- Random rare relic reward
- HP damage (10 normal, 15 A15+)
