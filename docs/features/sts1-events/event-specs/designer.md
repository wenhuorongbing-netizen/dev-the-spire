# Designer — Event Specification

## StS1 Wiki Behavior

**Acts:** 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Upgrade a Card | Choose a card to upgrade. |
| Remove a Card (50 gold) | Pay 50 gold. Choose a card to remove. |
| Transform 2 Cards | Choose 2 cards to transform. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Designer`
- **Registration:** Current source registers Designer as shared with `content.SharedEvent<Sts1Designer>()`; wiki bucket parity and runtime proof remain pending.
- **Layout:** Default

### Localization Keys
```
STS1_DESIGNER.title
STS1_DESIGNER.pages.INITIAL.description
STS1_DESIGNER.pages.INITIAL.options.UPGRADE.title / .description
STS1_DESIGNER.pages.INITIAL.options.REMOVE.title / .description
STS1_DESIGNER.pages.INITIAL.options.TRANSFORM.title / .description
STS1_DESIGNER.pages.INITIAL.options.LEAVE.title / .description
```

### Dependencies
- Card upgrade/remove/transform UI
