# Shining Light — Event Specification

## StS1 Wiki Behavior

**Acts:** 1 only (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Enter | Take 30% of max HP as damage. Upgrade 2 random cards. |
| Leave | Nothing happens. |

### Ascension Differences
- A15+: Take 40% of max HP as damage instead of 30%.

## StS2 Implementation

### Class: `Sts1ShiningLight`
- **Registration:** `Sts1EventRegistrationService` registers Shining Light into both StS2 Act 1 buckets with `content.ActEvent<Overgrowth, Sts1ShiningLight>()` and `content.ActEvent<Underdocks, Sts1ShiningLight>()`.
- **Layout:** Default
- **Source guard:** `Enter()` calls `Sts1EventHelpers.UpgradeRandomCards(owner, Rng, count: 2)` after damage. The helper filters upgradable deck cards, samples with the event RNG, and upgrades with `CardPreviewStyle.EventLayout`; it does not open the manual deck-upgrade selector.

### Localization Keys
```
STS1_SHINING_LIGHT.title
STS1_SHINING_LIGHT.pages.INITIAL.description
STS1_SHINING_LIGHT.pages.INITIAL.options.ENTER.title / .description
STS1_SHINING_LIGHT.pages.INITIAL.options.LEAVE.title / .description
STS1_SHINING_LIGHT.pages.ENTER.description
```

### Dependencies
- Upgrade random upgradable deck cards
- Damage (30% normal, 40% A15+)
