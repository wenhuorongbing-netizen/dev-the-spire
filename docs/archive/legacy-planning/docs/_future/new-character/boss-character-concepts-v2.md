# Boss Character Concepts V2

## Purpose
This document revises the first boss-character concepts after studying Downfall's Collector structure.

It is a design artifact only. It does not authorize gameplay implementation during setup.

## Source Boundary
- Public reference: Downfall Collector wiki pages for Collection, Reserve, Pyre, Doom, Temp HP, and Torchhead.
- Project references: `docs/downfall-character-reference.md`, `docs/boss-character-design-knowledgebase.md`, and `docs/ceremonial-beast-character-draft.md`.
- `source_basis`: public reference plus unsupported project-local draft.
- Confidence: medium for structural lessons, weak for card numbers and final fun.
- Do not copy Downfall card names, code, assets, art, or exact values.

## Collector Structural Lesson
Downfall's Collector is useful because its mechanisms are layered but not random:

| Collector layer | What it does | Design role |
|---|---|---|
| Collection + Essence | Converts defeated enemies into a side deck over the run. | Long-term identity and draft novelty. |
| Reserve | Stores alternate energy across turns and spends it only when normal energy is short. | Cross-turn planning and expensive-turn support. |
| Pyre | Requires exhausting another card in hand as an extra play cost. | Real hand-material cost for stronger effects. |
| Kindling | Cards with effects when exhausted. | Turns the cost system into a build-around engine. |
| Doom + Afflicted | Delayed HP loss that persists only if Weak and Vulnerable are maintained. | Conditional damage engine with maintenance pressure. |
| Temp HP + Torchhead | Temporary HP enables attack-triggered helper effects. | Defensive shell that converts safety into pressure. |

The important pattern is not "use many keywords." The pattern is:

1. One mechanism creates a cost.
2. One mechanism rewards paying that cost.
3. One mechanism creates cross-turn identity.

For an `EzDailyContent` boss character, a good target is three mechanisms, not five:

- a sequencing rule;
- a resource or cost rule;
- a payoff/maintenance rule.

## Concept A: Ceremonial Beast V2

### Core Experience
You are a ritual beast. Your strongest turns come from ringing the first card, offering something from the hand, and keeping enemies trapped in the sound.

### Mechanism 1: Toll
If a card with `Toll` is the first card you play this turn, its Toll text triggers.

Design role:
- Sequencing rule.
- Reference: Hermit's contextual card logic, not Collector.
- Main question: which card deserves to open the turn?

### Mechanism 2: Offering
Some cards have `Offering`.

When you play an Offering card, choose another card in your hand and exhaust it before resolving the Offering effect. Offering cards cannot be played if there is no other card in hand.

Offering side effects:

| Offered card | Bonus |
|---|---|
| Attack | Deal 4 damage to the target. |
| Skill | Gain 4 Block. |
| Power | Gain 1 Strength this turn. |
| Status/Curse | Apply 1 Ringing to a random enemy. |

Design role:
- Real hand-material cost.
- Reference: Collector's Pyre and Kindling pattern, but simplified.
- Main question: what am I willing to burn to make this turn stronger?

### Mechanism 3: Ringing
`Ringing` is an enemy debuff.

At the start of the enemy turn, the enemy loses HP equal to its Ringing. Then Ringing is removed unless you completed a ritual this turn.

You complete a ritual if either:
- you triggered Toll this turn; or
- you used Offering this turn.

Design role:
- Delayed payoff with maintenance.
- Reference: Collector's Doom persists only under a maintained condition.
- Main question: can I keep the enemy ringing while still blocking and attacking?

### Why This Is Better Than V1
V1 had Toll plus Resonance, but Resonance risked becoming another generic combat counter. V2 makes the character more tactile:

- Toll tells you when the turn begins.
- Offering tells you what the turn costs.
- Ringing tells you what you are maintaining across turns.

### Starter Deck Sketch
| Card | Count | Type | Cost | Text |
|---|---:|---|---:|---|
| Strike | 4 | Attack | 1 | Deal 6 damage. |
| Defend | 4 | Skill | 1 | Gain 5 Block. |
| Opening Bell | 1 | Attack | 1 | Deal 7 damage. Toll: apply 3 Ringing. |
| Blood Chime | 1 | Skill | 1 | Offering. Gain 9 Block. Apply 2 Ringing. |

### Prototype Cards
| Card | Type | Cost | Text |
|---|---|---:|---|
| Opening Bell | Attack | 1 | Deal 7. Toll: apply 3 Ringing. |
| Blood Chime | Skill | 1 | Offering. Gain 9 Block. Apply 2 Ringing. |
| Jaw Incense | Attack | 1 | Deal 8. If the target has Ringing, gain 1 Energy next turn. |
| Quiet Procession | Skill | 0 | Your next Offering this turn does not exhaust the chosen card; it Exhausts after resolving instead. |
| Hollow Hide | Skill | 1 | Gain 7 Block. Toll: gain 7 more Block. |
| Dissonant Gore | Attack | 2 | Deal 14. Offering: trigger the target's Ringing immediately. |
| Keep the Rite | Skill | 1 | Gain 8 Block. Ringing will not be removed from enemies this turn. |
| Second Bell | Attack | 0 | Deal damage equal to the target's Ringing. Cannot be the first card played this turn. |
| Throat of Bronze | Power | 1 | The first time each turn you use Offering, apply 1 Ringing to ALL enemies. |
| Final Peal | Attack | 3 | Trigger ALL enemy Ringing twice, then remove it. |

## Concept B: The Insatiable

### Core Experience
You are always hungry. The run is about holding a monster at the edge of starvation and turning that danger into burst turns.

### Mechanism 1: Hunger
`Hunger` is a combat resource from 0 to 10.

Rules:
- At end of turn, gain 1 Hunger.
- At 7 or more Hunger, your Bite cards gain bonuses.
- At 10 Hunger, lose 3 HP and reset Hunger to 5.

Design role:
- Cross-turn pressure clock.
- Main question: how close to the edge can I stay?

### Mechanism 2: Devour
`Devour` cards exhaust a card in hand or consume a Status/Curse for extra effects.

Design role:
- Hand-material cost and cleanup.
- Reference: Collector Pyre, but with hunger and status eating.
- Main question: which bad card or useful card becomes food?

### Mechanism 3: Sandpit
`Sandpit` is an enemy debuff.

When an enemy with Sandpit takes attack damage, reduce Sandpit by 1 and gain 1 Hunger. If Sandpit reaches 0 this way, apply 1 Vulnerable and deal 8 damage.

Design role:
- Target-management payoff.
- Main question: do I spread Sandpit or focus one target to collapse it?

### Why It Is Interesting
This has three linked loops:
- Hunger makes you dangerous and unstable.
- Devour controls Hunger and converts cards into power.
- Sandpit gives attacks a tactical target and feeds Hunger.

### Risk
It may overlap Ironclad exhaust if Devour only says "exhaust for damage." It needs hunger thresholds and Sandpit collapse to stay distinct.

## Concept C: Soul Fysh

### Core Experience
You bait yourself with unwanted calls, then release them into drowning turns.

### Mechanism 1: Beckon
`Beckon` adds a special status-like card, `Call`, into your draw pile.

`Call` is Unplayable. When retained or exhausted by Soul Fysh cards, it creates value.

Design role:
- Chosen burden.
- Main question: how many Calls can my deck carry?

### Mechanism 2: Release
`Release` exhausts one or more Calls from your hand, discard pile, or draw pile for effects.

Design role:
- Burden payoff.
- Reference: Collector Kindling, but the fuel is self-created.
- Main question: do I wait for a bigger release or clear the Calls now?

### Mechanism 3: Undertow
`Undertow` marks cards that were not played this turn.

At end of turn, the leftmost unplayed card in your hand gains Undertow. Some cards consume Undertow cards for bonus effects.

Design role:
- Makes "not playing" an action.
- Reference: Hexaghost Afterlife and Hermit positional play.
- Main question: which card do I intentionally leave behind?

### Risk
This character can become annoying if Call cards are mostly clog. Every early Call generator needs a cleanup path.

## Concept D: Waterfall Giant

### Core Experience
You are a pressure system. Block, hits, and heavy cards build Steam; Venting converts pressure into controlled violence.

### Mechanism 1: Steam
`Steam` ranges from 0 to 12.

Sources:
- Gain 1 Steam when you gain 10 or more Block in a turn.
- Gain 1 Steam when you take unblocked attack damage.
- Some cards directly add Steam.

At 12 Steam, trigger `Overflow`: lose 2 HP, deal 8 to ALL enemies, and reduce Steam to 6.

### Mechanism 2: Vent
`Vent X` spends up to X Steam for an effect.

Design role:
- Controlled release.
- Main question: spend now or tolerate higher pressure?

### Mechanism 3: Condense
`Condense` means: if you ended last turn with 8 or more Steam, this card has a bonus effect.

Design role:
- Rewards holding pressure across turns.
- Main question: can I afford to end the turn hot?

### Risk
Steam must not become another generic energy counter. It should be tied to defense, damage taken, and pressure thresholds.

## Recommended Direction
The best next candidate is **Ceremonial Beast V2**, not the old Resonance version.

Reason:
- It has three mechanisms, but they are tightly linked.
- It uses Collector's best lessons without copying Collector's identity.
- It avoids Defect-like passive slots.
- It creates several different card families: Toll openers, Offering costs, Ringing maintenance, and Ringing cashout.

## Design Standard For Future 40-Card Drafts
Every 40-card boss character draft should include:

| Bucket | Count target | Purpose |
|---|---:|---|
| Plain survival | 6-8 | Cards that work without the engine. |
| Mechanism enablers | 8-10 | Cards that create the core state or resource. |
| Mechanism payoffs | 8-10 | Cards that spend, trigger, or cash out the core state. |
| Bridges | 6-8 | Cards that connect two mechanisms. |
| Build-defining powers/rares | 5-7 | Cards that create archetype identity. |

For Ceremonial Beast V2:

| Bucket | Examples |
|---|---|
| Plain survival | Hollow Hide, Beast's Guard, Horn Jab. |
| Toll enablers | Opening Bell, Crowned Start, First Procession. |
| Offering enablers | Blood Chime, Incense Maw, Bronze Offering. |
| Ringing payoffs | Second Bell, Dissonant Gore, Final Peal. |
| Bridges | Keep the Rite, Throat of Bronze, Quiet Procession. |

## Next Action
Replace the older Ceremonial Beast 40-card Resonance draft with a V2 pool built around Toll, Offering, and Ringing.
