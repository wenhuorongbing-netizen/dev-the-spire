# Ceremonial Beast Character Draft

## Status
Design draft only. Do not implement during setup.

## One Sentence
Ceremonial Beast is a boss-turned-character built around restraint: each turn asks whether to play a normal sequence or make one card ring loudly enough to carry the turn.

## Source Boundary
- Boss reference: public STS2 boss guide descriptions of Ceremonial Beast using Plow, a second phase, and Ringing.
- Design method: local KB contracts for core experience, meaningful decisions, economy audit, and prototype planning.
- `source_basis`: mixed public reference plus unsupported project-local draft.
- Confidence: medium for concept fit, weak for card numbers.

## Core Experience Statement
The player should feel like a ritual beast building pressure through silence, then breaking the turn open with one deliberate card.

## Player Fantasy
You are not a combo engine. You are a procession, a warning bell, and a charging beast. Your strongest turns come from refusing small actions until the right hit, block, or chant matters.

## Core Verbs
- Ring: make the first card of the turn special.
- Restrain: choose to play exactly one card.
- Build: collect Resonance through disciplined turns.
- Break: spend Resonance for a large attack, block, or debuff.
- Recover: use simple commons when the ritual line is not available.

## Design Pillars
1. One card can be a full turn.
2. The first card is a tactical commitment.
3. Resonance rewards restraint, not spam.
4. The character must still play normal Slay the Spire when needed.
5. No passive slot system.

## Core Mechanics
### Toll
`Toll` means: if this is the first card you play this turn, its Toll text also happens.

Design purpose:
- Creates sequencing pressure without creating a new board slot.
- Makes cards readable: first-card bonus is visible on the card.
- Lets the player decide between opening with setup or opening with payoff.

### Resonance
`Resonance` is a combat resource, maximum 10.

Rules:
- At end of turn, if you played exactly 1 card this turn, gain 1 Resonance.
- Some cards gain or spend Resonance directly.
- Resonance resets after combat.
- Resonance is not energy and is not a passive trigger. It only matters when card text uses it.

Design purpose:
- Converts the boss's Ringing restriction into a voluntary reward.
- Avoids Defect-style passive slots.
- Creates a second-axis question: "Do I cash out now or keep building?"

## Starting Relic
**Ceremonial Collar**

At the start of combat, gain 1 Resonance. The first time each combat you trigger Toll, draw 1 card.

Design note:
- Helps the character show its mechanic immediately.
- Does not give free scaling every turn.

## Starter Deck
| Card | Count | Type | Cost | Text |
|---|---:|---|---:|---|
| Strike | 4 | Attack | 1 | Deal 6 damage. |
| Defend | 4 | Skill | 1 | Gain 5 Block. |
| Horn Jab | 1 | Attack | 1 | Deal 7 damage. Toll: deal 4 additional damage. |
| Bellguard | 1 | Skill | 1 | Gain 8 Block. Toll: gain 4 additional Block. |

## Card Pool Shape
This is a 40-card prototype pool, not a final STS2-sized pool.

| Rarity | Count | Purpose |
|---|---:|---|
| Common | 14 | Baseline damage, block, light Toll and Resonance access. |
| Uncommon | 16 | Archetype bridges, scaling, stronger spends, deck smoothing. |
| Rare | 10 | Build-defining payoffs and large restraint rewards. |

## Common Cards
| # | Card | Type | Cost | Text | Upgrade |
|---:|---|---|---:|---|---|
| 1 | Horn Jab | Attack | 1 | Deal 7 damage. Toll: deal 4 additional damage. | 9 damage, Toll +5. |
| 2 | Bellguard | Skill | 1 | Gain 8 Block. Toll: gain 4 additional Block. | 10 Block, Toll +5. |
| 3 | Chime Knife | Attack | 0 | Deal 3 damage twice. Toll: gain 1 Resonance. | 4 damage twice. |
| 4 | Crowned Gore | Attack | 1 | Deal 8 damage. Spend 1 Resonance: apply 1 Vulnerable. | 10 damage. |
| 5 | Heavy Plow | Attack | 2 | Deal 13 damage. Spend 1 Resonance: deal 6 additional damage. | 16 damage, spend bonus 8. |
| 6 | Bell Toss | Attack | 1 | Deal 5 damage to ALL enemies. Toll: apply 1 Weak to ALL enemies. | 7 damage. |
| 7 | Impatient Hoof | Attack | 1 | Deal 10 damage. Lose 1 Resonance. If you have no Resonance, lose 2 HP. | 13 damage. |
| 8 | Rung Hide | Skill | 2 | Gain 16 Block. Toll: gain 1 Resonance. | 19 Block. |
| 9 | Stillness | Skill | 1 | Gain 7 Block. If you have 3 or more Resonance, draw 1 card. | 9 Block. |
| 10 | Ritual Breath | Skill | 1 | Draw 2 cards, then discard 1 card. Toll: gain 1 Resonance. | Cost 0. |
| 11 | Low Chant | Skill | 0 | Your next Attack this turn deals 4 additional damage. If you play no Attack this turn, gain 1 Resonance at end of turn. | +6 damage. |
| 12 | Tin Mask | Skill | 1 | Gain 5 Block. Gain 1 Strength this turn. Toll: gain 1 Resonance. | 7 Block. |
| 13 | Procession Step | Skill | 0 | Gain 1 Resonance. Draw 1 card. Exhaust. | Does not Exhaust. |
| 14 | Echo Hide | Skill | 1 | Gain 6 Block. At end of turn, if this was the only card you played this turn, gain 6 Block next turn. | 8 and 8 Block. |

## Uncommon Cards
| # | Card | Type | Cost | Text | Upgrade |
|---:|---|---|---:|---|---|
| 15 | Feast of Bells | Skill | 1 | Gain 2 Resonance. Gain 2 Block for each Resonance above 5. | Cost 0. |
| 16 | Break the Line | Attack | 1 | Deal 8 damage. Toll: repeat this against a random enemy. | 10 damage. |
| 17 | Solemn Charge | Attack | 2 | Deal 18 damage. At end of turn, if this was the only card you played this turn, gain 2 Strength. | 22 damage. |
| 18 | Iron Litany | Skill | 1 | Gain 12 Block. Spend up to 3 Resonance: gain 4 Block for each spent. | 14 Block, 5 per spent. |
| 19 | Thundering Antler | Attack | 2 | Deal 10 damage to ALL enemies. Toll: spend all Resonance; deal 3 additional damage per Resonance spent. | 12 damage, 4 per spent. |
| 20 | Forbidden Cadence | Skill | 0 | Spend 2 Resonance: gain 2 Energy. Exhaust. | Spend 1 Resonance instead. |
| 21 | Aftertone | Skill | 1 | Return a card you played this turn to your hand. It costs 1 more this turn. Exhaust. | Cost 0. |
| 22 | Hearing Horn | Power | 1 | At the start of your turn, if you gained Resonance from restraint last turn, draw 1 card. | Innate. |
| 23 | Ritual Scar | Attack | 1 | Deal 7 damage. If the enemy intends to attack, gain 5 Block. Toll: gain 1 Resonance. | 9 damage, 7 Block. |
| 24 | Hollow Bell | Skill | 2 | Gain 11 Block. Draw 2 cards. Toll: the cards drawn cost 1 less this turn. | 14 Block. |
| 25 | Shatter Chime | Attack | 1 | Deal 6 damage. Apply 1 Weak. Spend 1 Resonance: also apply 1 Vulnerable. | 8 damage. |
| 26 | Plow Through | Attack | X | Deal 5 damage X times. Toll: X is increased by 1. | 6 damage. |
| 27 | Sacred Restraint | Power | 2 | When you gain Resonance from playing exactly 1 card, gain 1 additional Resonance. | Cost 1. |
| 28 | Ringing Blood | Skill | 1 | Lose 4 HP. Gain 3 Resonance. | Lose 3 HP, gain 4 Resonance. |
| 29 | Temple Weight | Skill | 1 | Retain. Gain 9 Block. If Retained, gain 1 additional Block this combat. | 11 Block. |
| 30 | Splintered Bell | Attack | 0 | Deal 4 damage. If this is not the first card you played this turn, gain 1 Resonance. | 6 damage. |

## Rare Cards
| # | Card | Type | Cost | Text | Upgrade |
|---:|---|---|---:|---|---|
| 31 | The Ceremony | Power | 3 | At the start of your turn, if you have 5 or more Resonance, spend 2 Resonance and your first card this turn triggers Toll twice. | Cost 2. |
| 32 | Avalanche Bell | Attack | 3 | Retain. Deal 28 damage. Whenever this is Retained, it gains 6 damage this combat. Toll: gain 1 Resonance. | 34 damage, gains 8. |
| 33 | One Sound | Skill | 2 | End your turn. At the start of your next turn, gain 3 Energy, draw 3 cards, and gain 3 Resonance. Exhaust. | Cost 1. |
| 34 | Godhoof Impact | Attack | 2 | Deal 8 damage plus 4 damage per Resonance. Lose all Resonance. | 10 plus 5 per Resonance. |
| 35 | Silence the World | Skill | 2 | Gain 20 Block. Spend half your Resonance, rounded down. Enemies lose Strength this turn equal to Resonance spent. | 24 Block. |
| 36 | Funeral Procession | Power | 2 | Whenever a card's Toll triggers, deal 3 damage to ALL enemies. | 4 damage. |
| 37 | Bell Eclipse | Attack | 1 | Deal 12 damage. If this is the only card you play this turn, repeat it at end of turn once for each 3 Resonance you have. | 15 damage. |
| 38 | Rapture of Restraint | Power | 2 | At end of turn, if you played exactly 1 card, gain 1 Strength and 1 Dexterity. | Cost 1. |
| 39 | Beast Below the Bells | Skill | 1 | Spend all Resonance. Gain that much Strength this turn and 3 Block per Resonance spent. Exhaust. | 4 Block per Resonance. |
| 40 | Final Toll | Attack | 3 | Can only be played if you have 5 or more Resonance. Spend 5 Resonance. Deal 40 damage. | 50 damage. |

## Archetype Map
| Archetype | Cards | Intended decision |
|---|---|---|
| Toll tempo | Horn Jab, Bellguard, Break the Line, Ritual Scar, Hollow Bell, Funeral Procession | Which card deserves to be first this turn? |
| Restraint scaling | Echo Hide, Solemn Charge, Hearing Horn, Sacred Restraint, Rapture of Restraint | Can I afford to play exactly one card? |
| Resonance cashout | Heavy Plow, Thundering Antler, Godhoof Impact, Beast Below the Bells, Final Toll | Do I spend now or hold for a larger turn? |
| Recovery and smoothing | Stillness, Ritual Breath, Procession Step, Aftertone, One Sound | How do I avoid dying when the ritual line is not available? |
| Risk pressure | Impatient Hoof, Forbidden Cadence, Ringing Blood, The Ceremony | How much future stability do I trade for tempo? |

## First 12-Card Prototype Slice
Use this subset before implementing the full 40-card draft:

| Card | Reason |
|---|---|
| Horn Jab | Baseline Toll attack. |
| Bellguard | Baseline Toll block. |
| Chime Knife | Cheap Toll generator. |
| Heavy Plow | Resonance spend attack. |
| Bell Toss | AoE and weak access. |
| Stillness | Defensive smoothing. |
| Ritual Breath | Draw/discard smoothing. |
| Echo Hide | One-card turn defense. |
| Break the Line | Clear Toll payoff. |
| Iron Litany | Resonance block spend. |
| Hearing Horn | Restraint engine. |
| Godhoof Impact | Rare Resonance cashout. |

## Design Risks
| Risk | Why it matters | Mitigation |
|---|---|---|
| One-card turns are too weak | The player abandons the identity. | Add common defensive one-card payoffs before adding rare damage. |
| One-card turns are mandatory | The character becomes scripted. | Keep non-Toll baseline values acceptable. |
| Resonance becomes Stars | The role overlaps Regent. | Resonance is capped, combat-only, and mainly gained from restraint. |
| Toll becomes trivial | The first card is almost always obvious. | Add competing first-card candidates: draw, block, damage, debuff. |
| Energy loops break the pool | Forbidden Cadence and One Sound may enable infinites. | Keep Exhaust, Resonance costs, and test draw-energy loops early. |

## Prototype Plan
Prototype question:
Can a player understand and enjoy the choice between normal turns and exactly-one-card turns within the first three combats?

Hypothesis:
If common cards have acceptable baseline values and clear Toll bonuses, players will choose one-card turns opportunistically rather than feeling locked out.

Minimum build scope:
- Starting relic.
- Starter deck.
- First 12-card prototype slice.
- UI text for Toll and Resonance.

Excluded features:
- Full card pool.
- Character-specific relics.
- Custom art.
- Unlocks.
- Boss-as-enemy changes.

Success signal:
In playtest notes, the player reports at least two moments where they intentionally ended after one card for future value.

Failure signal:
The player either ignores Resonance for the full run or complains that the character prevents them from playing cards.

Next decision:
If the 12-card slice works, expand the pool by adding one risk card, one draw card, one AoE card, and one rare payoff at a time.

## Evidence Gaps
- No implementation exists.
- No playtest has been run.
- No numeric tuning has been validated.
- Exact STS2 terminology for Strength, Dexterity, Vulnerable, Weak, Retain, Exhaust, and X-cost should be verified against previous framework/template APIs before code.
