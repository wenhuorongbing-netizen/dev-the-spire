# Augmenter — Event Specification

## StS1 Wiki Behavior

**Acts:** 2 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Transform | Transform 2 random cards. |
| Mutate | Choose a card to upgrade. |
| Reject | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Augmenter`
- **Registration:** `[RegisterActEvent(typeof(Act2Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_AUGMENTER.title
STS1_AUGMENTER.pages.INITIAL.description
STS1_AUGMENTER.pages.INITIAL.options.TRANSFORM.title / .description
STS1_AUGMENTER.pages.INITIAL.options.MUTATE.title / .description
STS1_AUGMENTER.pages.INITIAL.options.REJECT.title / .description
```

### Dependencies
- Transform cards command (×2)
- Card upgrade UI
