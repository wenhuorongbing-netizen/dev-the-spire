# Knowing Skull — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| "Who are you?" | Take 6 HP damage. Learn the skull's story. |
| "What is your purpose?" | Take 6 HP damage. Learn more. |
| "What do you want?" | Take 6 HP damage. Obtain a random rare card. |
| Leave | Nothing happens. |

Each question can be asked multiple times. Each successive question costs 6 HP.

### Ascension Differences
- A15+: Each question costs 10 HP instead of 6.

## StS2 Implementation

### Class: `Sts1KnowingSkull`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1KnowingSkull>()`.
- **Layout:** Default

### Localization Keys
```
STS1_KNOWING_SKULL.title
STS1_KNOWING_SKULL.pages.INITIAL.description
STS1_KNOWING_SKULL.pages.INITIAL.options.QUESTION_1.title / .description
STS1_KNOWING_SKULL.pages.INITIAL.options.QUESTION_2.title / .description
STS1_KNOWING_SKULL.pages.INITIAL.options.QUESTION_3.title / .description
STS1_KNOWING_SKULL.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Random rare card reward
- HP damage (6 normal, 10 A15+)
