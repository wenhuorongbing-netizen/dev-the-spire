# Neow — Event Specification

## StS1 Wiki Behavior

**Acts:** Act 1 only (start of run, Boss room)

Neow appears at the start of every run and offers blessings. The player chooses one of four blessings, which vary based on whether the previous run reached the first boss.

### Blessings (reached boss in previous run):
| Blessing | Effect |
|----------|--------|
| Upgrade a Card | Choose a card to upgrade. |
| Transform a Card | Choose a card to transform. |
| Choose a Card to Remove | Remove a card from your deck. |
| Obtain a Random Rare Card | Add a random rare card to your deck. |
| Obtain 3 random potions | Get 3 random potions. |
| +7 Max HP | Gain 7 max HP. |
| Gain 100 Gold | Gain 100 gold. |
| Obtain a random Relic | Get a random relic. |
| Choose 1 of 3 Rare Cards | Pick 1 of 3 rare cards. |

### Blessings (did NOT reach boss):
| Blessing | Effect |
|----------|--------|
| +8 Max HP | Gain 8 max HP. |
| +50 Gold | Gain 50 gold. |
| Remove a Card | Remove a card from your deck. |
| Obtain a random Common Relic | Get a random common relic. |

### Curses (after taking 3+ blessings):
Neow's Lament: Enemies in the first 3 combats have 1 HP.

### Ascension Differences
None.

## StS2 Implementation

### Class: `Sts1Neow`
- **Registration:** Act 1 special (start of run)
- **Layout:** Ancient-style or Default

### Notes
- This is the most complex event — it's the run starter
- May need special handling for the blessing selection UI
- StS2 has its own Neow equivalent; this would be an alternative
