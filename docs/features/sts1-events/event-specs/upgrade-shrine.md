# Upgrade Shrine — Event Specification

## StS1 Wiki Behavior

**Acts:** 3 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Pray | Choose a card to upgrade. |
| Leave | Nothing happens. |

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1UpgradeShrine`
- **Registration:** `[RegisterActEvent(typeof(Act3Model))]`
- **Layout:** Default

### Localization Keys
```
STS1_UPGRADE_SHRINE.title
STS1_UPGRADE_SHRINE.pages.INITIAL.description
STS1_UPGRADE_SHRINE.pages.INITIAL.options.PRAY.title / .description
STS1_UPGRADE_SHRINE.pages.INITIAL.options.LEAVE.title / .description
STS1_UPGRADE_SHRINE.pages.PRAY.description
```

### Dependencies
- Card upgrade UI
