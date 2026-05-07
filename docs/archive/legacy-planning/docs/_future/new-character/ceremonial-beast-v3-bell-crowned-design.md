# Ceremonial Beast V3 - Bell-Crowned Beast

## Status
Design draft only. Do not implement during setup.

This supersedes the earlier Resonance-focused Ceremonial Beast draft as the preferred direction.

## Design Target
Create a Slay the Spire 2 boss-character whose fun comes from ritual escalation:

1. Open the turn with the right bell.
2. Sacrifice real hand material.
3. Maintain a lethal delayed sound on enemies.
4. Double the sound.
5. Cash out in a loud, earned burst.

## Source Boundary
- Public reference: Downfall Collector structure, especially Collection, Reserve, Pyre, Kindling, Doom, Afflicted, Temp HP, and Torchhead.
- Local method: `D:\Game\FOTN\knowledge` contracts for core experience, meaningful decision audit, economy audit, and prototype planning.
- Project references: `docs/downfall-character-reference.md` and `docs/boss-character-concepts-v2.md`.
- `source_basis`: public reference plus unsupported project-local draft.
- Confidence: medium for structure, weak for numbers.
- Do not copy Downfall card names, art, code, assets, or exact card text.

## Downfall Lesson Being Used
Collector is clever because its systems form a loop:

| Collector pattern | What it teaches | V3 translation |
|---|---|---|
| Pyre costs a real hand card | Power should consume something the player wanted. | `Offering` exhausts another hand card. |
| Kindling makes the cost playable | The sacrifice can become an engine, not only a penalty. | `Votive` cards do things when Offered. |
| Doom needs Afflicted to persist | Delayed damage should require upkeep. | `Ringing` persists only if a Rite was completed. |
| Reserve enables future expensive turns | Cross-turn planning creates big payoffs. | `Overtone` stores future Peal repetitions. |
| Torchhead converts safety into pressure | Defense can unlock offense. | Defensive Votives and Toll blocks keep the Rite alive. |

## Core Experience Statement
The player should feel like a giant ritual creature conducting a battle: each turn begins with a chosen bell, feeds the rite with a sacrifice, keeps enemies trapped in sound, and eventually releases a ridiculous crescendo.

## Player Fantasy
You are not fast. You are inevitable. The enemy hears the first bell, then the second, then the room starts doubling the sound back at them.

## Design Pillars
1. Every big turn must be prepared, visible, and earned.
2. Sacrifice should feel dangerous but clever, not like random self-harm.
3. Exponential growth is allowed, but each doubling must spend tempo, cards, or stored setup.
4. The character needs normal hallway competence without rare powers.
5. No passive slot board. The state is in hand, on enemies, and in a small player counter.

## Core Mechanisms

### 1. Toll
If a card with `Toll` is the first card you play this turn, its Toll text triggers.

Design role:
- Turn opener.
- Hermit-like contextual play, but based on first-card order instead of middle-hand position.
- Creates competition between opening with damage, block, draw, setup, or cashout.

Decision question:
- Which card deserves to be first this turn?

### 2. Offering
An `Offering` card requires you to choose another card in hand and Exhaust it before the Offering card resolves.

Rules:
- Offering cards cannot be played if they are the only card in hand.
- Some Offering cards care about the offered card's type, cost, or keywords.
- Offering is the character's main real cost.

Design role:
- Collector Pyre lesson, translated into ritual sacrifice.
- Turns hand contents into a strategic resource.

Decision question:
- Which card am I willing to lose to make this turn powerful?

### 3. Votive
`Votive` is text on cards that triggers when that card is exhausted by Offering.

Examples:
- Votive: gain Block.
- Votive: apply Ringing.
- Votive: draw next turn.
- Votive: gain Overtone.

Design role:
- Collector Kindling lesson.
- Makes "bad to play, good to offer" cards draftable.
- Adds deck-building tension: enough Votives to fuel the engine, not so many that hands brick.

Decision question:
- Is this card worth playing normally, or should I keep it as ritual fuel?

### 4. Ringing
`Ringing` is an enemy debuff.

Rules:
- At the start of the enemy turn, that enemy loses HP equal to its Ringing.
- After that HP loss, Ringing is removed unless you completed a Rite during your turn.
- A Rite is completed if you triggered Toll, played an Offering card, or played a Peal card.

Design role:
- Collector Doom/Afflicted lesson.
- Delayed damage with maintenance pressure.
- Gives the player a reason to keep performing the ritual instead of only front-loading damage.

Decision question:
- Do I maintain the sound for future growth, or let it vanish after this hit?

### 5. Reverberate
`Reverberate` doubles the target's Ringing.

Prototype constraints:
- Ringing cap: 64.
- A target can only be Reverberated once per player turn.
- If the target has 0 Ringing, Reverberate applies 2 Ringing instead.

Design role:
- The first exponential axis: amount doubles.
- Creates timing math: apply more Ringing before doubling, or double now to survive.

Decision question:
- Is now the right time to double, or should I add more base Ringing first?

### 6. Peal
`Peal` triggers a target's Ringing immediately.

Rules:
- Peal causes HP loss equal to Ringing now.
- After Peal, reduce that target's Ringing by half, rounded down, unless the card says otherwise.
- Peal completes a Rite.

Design role:
- Cashout valve.
- Lets the player convert delayed damage into immediate tempo.
- Prevents Ringing from being only passive damage over time.

Decision question:
- Do I cash out now and lose part of the stack, or wait for enemy turn and maintain it?

### 7. Overtone
`Overtone` is a player buff.

Rules:
- The next Peal repeats one additional time for each Overtone.
- After any Peal uses Overtone, remove all Overtone.
- Prototype cap: 4 Overtone.

Design role:
- The second exponential axis: trigger count grows.
- Creates the "double explosion" feel when combined with Reverberate.

Decision question:
- Do I store Overtone for a Grand Peal, or spend it to solve this fight now?

## Why This Can Be Deep And Still Playable
The player only sees three main live objects:

- first-card state for Toll;
- enemy Ringing counters;
- player Overtone counter.

Offering adds a hand prompt, but no board slots. Votive is card text. Reverberate and Peal are action words on cards.

The complexity comes from interactions, not from screen clutter:

```text
Toll starts a Rite
Offering pays a real hand cost
Votive makes the cost profitable
Ringing creates delayed pressure
Reverberate doubles the amount
Overtone doubles the number of triggers
Peal cashes out and spends the setup
```

## Power Curve Example
This is an illustrative tuning target, not validated balance.

| Turn | Action | Result |
|---:|---|---|
| 1 | Toll card applies 4 Ringing. | Enemy will lose 4 HP and Ringing persists. |
| 2 | Offering uses a Votive, applies 4 more Ringing, then Reverberate. | Ringing goes from 8 to 16. |
| 3 | Gain 2 Overtone, then Peal. | Peal triggers 3 times for 16 each, dealing 48 now, then Ringing drops to 8. |
| 4 | Reverberate remaining Ringing to 16, apply more Ringing, keep Rite alive. | The second crescendo starts. |

The satisfying part is that the burst is earned through several decisions:

- opening with the right card;
- keeping fuel in hand;
- choosing which card to sacrifice;
- maintaining the Rite;
- doubling before or after adding Ringing;
- deciding when the Peal is worth losing part of the stack.

## Economy Audit

| Resource | Sources | Sinks | Player incentive | Inflation risk | Tuning lever |
|---|---|---|---|---|---|
| Hand cards | draw, retain, reward drafting | Offering exhausts cards | Draft enough fuel without bricking | Excess draw makes Offering free | limit zero-cost draw, make Offering require another card |
| Votive cards | card rewards, generated Votives | Offering exhausts them | Build around sacrifice value | Too many Votives reduce real cost | make many Votives weak when played normally |
| Ringing | Toll, Offering, Votive, powers | Peal halves it, failure to Rite removes it | Maintain and double delayed damage | Doubling can trivialize bosses | cap at 64, once-per-turn Reverberate |
| Overtone | rare powers, Votives, risky skills | next Peal consumes all | Save for burst turn | Multi-trigger one-shots too early | cap at 4, attach HP/tempo costs |
| HP | normal game systems | risky Offering and overcharge cards | Trade life for faster crescendo | Healing/relics can erase risk | avoid repeatable free healing in base kit |
| Energy | normal game systems, rare tools | expensive Peal/Reverberate turns | Plan big turns | Reserve-like energy can enable infinites | keep energy gain Exhausting or delayed |

## Meaningful Decision Audit

| Decision point | Player options | Information available | Consequence | Failure mode | Repair |
|---|---|---|---|---|---|
| First card of turn | Toll damage, Toll block, Toll draw, non-Toll survival | hand, enemy intent, Ringing values | Defines turn opener and Rite status | obvious opener every turn | create competing Toll openers at common |
| Offering target | offer dead card, useful card, Votive, Status/Curse | hand value, discard plan, fight length | Gain strong effect but lose material | Offering feels like pure upside | make non-Votive Offering materially painful |
| Ringing target | stack one enemy or spread | enemy HP, intents, AoE needs | Focus kill versus multi-enemy pressure | always stack boss/leader | add Chorus cards that reward all enemies Ringing |
| Reverberate timing | double now or add more first | current Ringing, available Ringing sources | Bigger future stack versus immediate tempo | players always wait | enemy pressure and Peal cards reward earlier use |
| Peal timing | cash out now or wait | enemy HP, incoming damage, Overtone count | immediate burst but reduces stack | hoarding until fight ends | add cards that reward smaller Peals |
| Overtone storage | save for huge Peal or spend | upcoming draw, enemy HP, safety | big crescendo versus current survival | only rare combo matters | common Peal cards must be useful at 0 Overtone |

## Archetypes

| Archetype | Core cards | Play style |
|---|---|---|
| Toll Tempo | Toll attacks, Toll block, opener draw | Reliable hallway play; wins by good sequencing. |
| Offering Furnace | Offering cards plus Votives | Turns hand sacrifice into value and card flow. |
| Ringing Lock | Ringing application plus Rite maintenance | Boss/elite plan; keeps delayed damage alive. |
| Peal Burst | Peal cards, Overtone, Reverberate | Big numbers; the main "I earned this" build. |
| Chorus Control | Ringing spread, weak/vulnerable, all-enemy Peal | Multi-enemy build; avoids single-target-only weakness. |

## Starter Relic
**Bronze Tongue**

At the start of combat, apply 2 Ringing to the enemy with the lowest HP. The first time each combat you complete a Rite, gain 1 Overtone.

Reason:
- Shows Ringing immediately.
- Gives one small Peal payoff in early combats.
- Does not create free doubling every turn.

## Starter Deck
| Card | Count | Type | Cost | Text |
|---|---:|---|---:|---|
| Strike | 4 | Attack | 1 | Deal 6 damage. |
| Defend | 4 | Skill | 1 | Gain 5 Block. |
| Opening Bell | 1 | Attack | 1 | Deal 7 damage. Toll: apply 4 Ringing. |
| Blood Rite | 1 | Skill | 1 | Offering. Gain 8 Block. Apply 2 Ringing. |

## Prototype Card Seed
Numbers are placeholders.

| Card | Rarity | Type | Cost | Text | Purpose |
|---|---|---|---:|---|---|
| Opening Bell | Basic | Attack | 1 | Deal 7. Toll: apply 4 Ringing. | Starter Toll. |
| Blood Rite | Basic | Skill | 1 | Offering. Gain 8 Block. Apply 2 Ringing. | Starter Offering. |
| Hollow Hide | Common | Skill | 1 | Gain 7 Block. Toll: gain 7 more Block. | Defensive opener. |
| Jaw Chime | Common | Attack | 1 | Deal 8. If the target has Ringing, deal 4 more. | Basic payoff. |
| Votive Ash | Common | Skill | 0 | Gain 3 Block. Votive: gain 9 Block. | Offering fuel. |
| Votive Fang | Common | Attack | 0 | Deal 3 damage. Votive: deal 8 damage to the target. | Offering fuel. |
| Bell Rope | Common | Skill | 0 | Draw 1. If this is not the first card this turn, apply 2 Ringing. | Turn smoothing. |
| Keep the Rite | Common | Skill | 1 | Gain 8 Block. Ringing will not be removed this turn. | Maintenance safety. |
| Second Peal | Common | Attack | 1 | Peal. Deal 5 damage. | Early cashout. |
| Bronze Antler | Uncommon | Attack | 2 | Deal 14. Reverberate. | Doubling tool. |
| Incense Maw | Uncommon | Skill | 1 | Offering. Gain 1 Overtone. Draw 1 next turn. | Overtone source. |
| Choir Mark | Uncommon | Skill | 1 | Apply 3 Ringing to ALL enemies. If all enemies have Ringing, gain 1 Overtone. | AoE setup. |
| Dissonant Gore | Uncommon | Attack | 2 | Deal 12. Peal. If this kills, gain 1 Overtone. | Tempo burst. |
| Votive Marrow | Uncommon | Skill | 1 | Gain 6 Block. Votive: Reverberate the enemy with the most Ringing. | Premium fuel. |
| Split the Bell | Uncommon | Skill | 1 | Move half of one enemy's Ringing to all other enemies. | Spread/control. |
| Throat of Bronze | Rare | Power | 1 | The first time each turn you use Offering, apply 2 Ringing to ALL enemies. | Offering engine. |
| Double Tongue | Rare | Power | 2 | The first time each turn you Reverberate, gain 1 Overtone. | Exponential build. |
| Grand Peal | Rare | Attack | 3 | Peal ALL enemies. Overtone repeats this Peal. Then remove all Overtone. | Main payoff. |

## Candidate 40-Card Pool Shape
Do not implement the full pool first. This is the design budget.

| Bucket | Count | Notes |
|---|---:|---|
| Plain survival | 6 | Must work with no Ringing engine. |
| Toll openers | 7 | Competing first-card choices. |
| Offering cards | 7 | Real hand sacrifice costs. |
| Votive cards | 6 | Fuel that keeps Offering from being pure downside. |
| Ringing maintenance/spread | 6 | Prevents boss-only single-target play. |
| Reverberate/Peal payoffs | 5 | Exponential burst tools. |
| Build-defining powers | 3 | Engines, not mandatory baseline. |

## Prototype Plan
Prototype question:
Can Toll, Offering, Votive, Ringing, Reverberate, Peal, and Overtone create a readable crescendo without making hallway fights feel slow or confusing?

Hypothesis:
If the first 18-card slice includes enough plain block/damage and only two burst rares, players will understand the ritual loop before the exponential pieces appear.

Minimum build scope:
- Starter relic.
- Starter deck.
- 18-card prototype seed.
- Text/UI for Ringing and Overtone.
- Offering selection prompt.

Excluded features:
- Full 40-card pool.
- Unique relic pool.
- Custom reward screens.
- Run-level Collection-like side deck.
- Boss art or copied game assets.

Success signal:
- Player can explain the loop after three combats.
- Player intentionally delays a Peal at least once to double Ringing first.
- Player intentionally Offers a useful card at least once because the payoff is worth it.
- At least one combat ends with a large Peal that feels earned rather than accidental.

Failure signal:
- Player ignores Offering because it feels too costly.
- Player ignores Ringing because attack damage is simpler.
- Player cannot predict why Ringing vanished.
- Player hoards Overtone and never spends it.

Timebox:
- One prototype pass should cover three hallway fights, one elite-style fight, and one boss-style long fight.

Next decision:
- If readable and fun, expand the Votive and Chorus packages first.
- If too complex, remove Overtone from commons/uncommons and keep it as a rare-only engine.
- If too weak, improve Toll block and common Peal cards before buffing rare doubling.

## Evidence Gaps
- No implementation.
- No playtest notes.
- No telemetry.
- No numeric simulation.
- STS2 API feasibility for Offering prompts, Ringing debuff persistence, once-per-turn Reverberate tracking, and Peal repetition must be verified before code.
