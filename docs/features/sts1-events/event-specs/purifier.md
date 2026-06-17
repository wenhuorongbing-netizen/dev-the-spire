# Purifier - Event Specification

## StS1 Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Purify | Remove a card from your deck for free. |
| Leave | Nothing happens. |

### Ascension Differences

None.

## StS2 Implementation

### Class: `Sts1Purifier`

- **Registration:** `Sts1EventRegistrationService` registers this shared event with `content.SharedEvent<Sts1Purifier>()`; included in `RegisterAll()` and `RegisterAdditiveBatch1()`.
- **Layout:** Default
- **Source behavior:** `Purify()` calls `Sts1EventHelpers.OpenCardRemoval(owner)` and then finishes the event.
- **Runtime proof:** Pending. AdditiveBatch1 encounter UI, result log, EN/ZHS render, and save-load proof still need live evidence.

### Localization Keys

```text
STS1_PURIFIER.title
STS1_PURIFIER.pages.INITIAL.description
STS1_PURIFIER.pages.INITIAL.options.PURIFY.title / .description
STS1_PURIFIER.pages.INITIAL.options.LEAVE.title / .description
STS1_PURIFIER.pages.PURIFY.description
STS1_PURIFIER.pages.LEAVE.description
```

### Dependencies

- Card removal UI

### Dynamic Variables

None.
