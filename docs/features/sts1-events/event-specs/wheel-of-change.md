# Wheel of Change — Event Specification

## StS1 Wiki Behavior

**Acts:** 1, 2, 3 (Unknown room pool)

### Options

| Option | Effect |
|--------|--------|
| Spin the Wheel | Random outcome (6 possibilities). |

Random outcomes (equal chance):
1. **Gold**: Gain 100 gold
2. **Damage**: Take 30% of max HP as damage
3. **Relic**: Obtain a random relic
4. **Curse**: Obtain a Curse (Decay)
5. **Heal**: Heal to full HP
6. **Card Removal**: Remove a card from your deck

### Ascension Differences
- A15+: Damage outcome deals 40% max HP instead of 30%.

## StS2 Implementation

### Class: `Sts1WheelOfChange`
- **Registration:** `[RegisterSharedEvent]`
- **Layout:** Default (simplified — single spin option, random result)

### Localization Keys
```
STS1_WHEEL_OF_CHANGE.title
STS1_WHEEL_OF_CHANGE.pages.INITIAL.description
STS1_WHEEL_OF_CHANGE.pages.INITIAL.options.SPIN.title / .description
STS1_WHEEL_OF_CHANGE.pages.GOLD.description
STS1_WHEEL_OF_CHANGE.pages.DAMAGE.description
STS1_WHEEL_OF_CHANGE.pages.RELIC.description
STS1_WHEEL_OF_CHANGE.pages.CURSE.description
STS1_WHEEL_OF_CHANGE.pages.HEAL.description
STS1_WHEEL_OF_CHANGE.pages.REMOVE.description
```

### Dependencies
- Decay curse card model
- Random relic reward helper
- Card removal UI
