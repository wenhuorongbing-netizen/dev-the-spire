# The Library — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Read | Choose 1 of 20 random cards. Add it to your deck. |
| Rest | Heal 1/3 of max HP. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1TheLibrary`
- **Registration:** `Sts1EventRegistrationService` registers this StS1 Act 2 event with `content.ActEvent<Hive, Sts1TheLibrary>()`.
- **Layout:** Default

### Localization Keys
```
STS1_THE_LIBRARY.title
STS1_THE_LIBRARY.pages.INITIAL.description
STS1_THE_LIBRARY.pages.INITIAL.options.READ.title / .description
STS1_THE_LIBRARY.pages.INITIAL.options.REST.title / .description
STS1_THE_LIBRARY.pages.INITIAL.options.LEAVE.title / .description
STS1_THE_LIBRARY.pages.READ.description
STS1_THE_LIBRARY.pages.REST.description
```

### Dependencies
- Card selection UI (choose 1 of 20)
- Heal (1/3 max HP)
