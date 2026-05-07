# Downfall Character Reference

## Purpose
This document summarizes public design lessons from Slay the Spire: Downfall characters for future Slay the Spire 2 boss-to-character design in `EzDailyContent`.

It is a design reference only. It does not authorize gameplay implementation during setup.

## Source Boundary
- Public references: Downfall Steam page, Downfall wiki snippets/pages indexed by public search, and community discussions.
- Local design method: `docs/design-operating-brief.md` and `docs/boss-character-design-knowledgebase.md`.
- `source_basis`: public reference plus unsupported project-local interpretation.
- Confidence: medium for mechanic summaries that appear in public pages; weak for balance claims and player-feel interpretation.
- Do not copy Downfall card text, art, code, assets, or full card pools.

## Current Character Scope
Public Downfall sources now list the Hermit plus a Downfall campaign roster including Slime Boss, Guardian, Hexaghost, Champ, Automaton, Collector, Awakened, Gremlins, and Snecko. Older Steam copy describes seven villain characters "with more to come"; treat the wiki roster as newer but still public/community-maintained.

## Design Lens
For each character, ask:

1. What boss fantasy is being preserved?
2. What repeatable player decision replaces the boss AI?
3. What cost keeps the fantasy from becoming free power?
4. What makes the card pool draftable before the engine is complete?
5. What part is worth referencing, and what part should not be copied?

## Character Notes

### Hermit
Core concept:
- Undead gunslinger; aiming is expressed through hand position.

Mechanic cluster:
- `Dead On`: cards gain bonus effects when in the middle of the hand.
- `Concentrate`: temporarily lets Dead On trigger regardless of position.
- Supporting space includes curses, self-debuffs, Bruise, Rugged, and Bounty.

Gameplay question:
- How do I spend, draw, retain, discard, or exhaust cards so the important card lands in the middle at the right time?

Reference value:
- Excellent example of turning a physical idea, "aiming", into a hand-management puzzle.
- The mechanic is spatial but does not create a board slot or passive engine.
- The best reference for our Ceremonial Beast is not the curse package; it is the idea that a card can care about its play context, not only its text.

Risk to avoid:
- Position logic can become fiddly if UI feedback is unclear.
- Curse support can drift into "bad cards are secretly always good" if the downside is too easily erased.

### Slime Boss
Core concept:
- A boss that divides, rams things, and weaponizes ooze.

Mechanic cluster:
- Slime minions, splitting, and commanding minions.
- Tackle-style attacks that hurt the Slime Boss as well as the enemy.
- Goop and Consume loops that convert setup into damage and healing.

Gameplay question:
- How much HP or tempo do I risk now to create enough ooze/minion pressure to heal back and end the fight?

Reference value:
- Strong model for converting boss identity into three clear verbs: split, coat, consume.
- Self-damage plus recovery is a clean risk loop.
- Goop is a useful pattern for a boss-derived "mark" that is spent rather than passively ticking forever.

Risk to avoid:
- Minion systems create UI and balance load quickly.
- If self-damage is always refunded, the risk disappears; if it is not, the character feels reckless and brittle.

### Guardian
Core concept:
- Defensive machine that curls up, reflects damage, and modifies its tools.

Mechanic cluster:
- `Mode Shift`: damage or Brace pushes Guardian toward Defensive Mode.
- `Brace`: intentionally advances the mode threshold.
- Defensive Mode gives block/thorns-style protection.
- Stasis temporarily holds cards and later returns them cheaper.
- Gems can be socketed into cards to customize them.

Gameplay question:
- Do I trigger defense mode now for safety, or delay it so the payoff lands on a better enemy turn?

Reference value:
- Great example of a threshold defense system that the player can advance voluntarily.
- Gems are a strong deck-customization pattern because they make drafted cards become canvases.
- Stasis shows how delayed cards can become future tempo without simply drawing more.

Risk to avoid:
- Guardian carries several large systems at once; copying that density into a first STS2 character would be too much.
- Stasis can feel close to orb/slot systems if implemented as board storage.
- Socket systems need UI and persistence rules before they are safe to implement.

### Hexaghost
Core concept:
- Six-fire ritual engine; correct timing lights flames and delayed burn finishes enemies.

Mechanic cluster:
- Six Ghostflames arranged in a ring.
- Ignite, Extinguish, Advance, Retract, and Intensity manipulate flame state and position.
- Soulburn is delayed HP loss after a timer.
- Afterlife lets Ethereal cards still do something when they exhaust.

Gameplay question:
- Can I satisfy the current flame objective while still solving this turn's damage/block problem?

Reference value:
- Very strong ritual-design reference: each turn can contain a small "side quest".
- Afterlife is a useful pattern: not playing a card can still be an intentional action.
- Soulburn shows delayed damage that asks whether to stack, detonate, or wait.

Risk to avoid:
- A rotating six-object UI is expensive and can overwhelm new players.
- Too many independent keywords can obscure the actual turn decision.
- Delayed damage needs clear countdown feedback or it becomes invisible power.

### Champ
Core concept:
- Arena fighter who changes stance and ends exchanges with finishers.

Mechanic cluster:
- Stances: Berserker, Defensive, Ultimate, plus no stance.
- Skill Bonus charges are limited during a stance.
- Finishers use stance-specific effects and leave stance.
- Combo effects care about stance history or current stance.

Gameplay question:
- Which stance should I enter, how many skill bonuses do I extract, and when do I cash out with a finisher?

Reference value:
- Better reference than Watcher if the goal is "martial cadence" rather than damage multiplier stance.
- Limited charges are a good way to prevent one stance from becoming permanent.
- Finisher is a strong verb: it tells the player when a sequence should end.

Risk to avoid:
- Too close to existing stance characters if the stance effects are only attack/block/damage multipliers.
- Requires enough finishers and stance entry cards; otherwise drafts become parasitic.

### Automaton
Core concept:
- Machine that compiles cards into a new function.

Mechanic cluster:
- Encode cards into a three-slot queue.
- When the queue fills, encoded effects merge into a new 1-cost Function.
- Compile-related cards modify, clean, copy, or accelerate the encoded queue.
- Status cards and errors can become either junk or fuel.

Gameplay question:
- Which effects do I assemble into one future card, and can I survive the delay before it runs?

Reference value:
- Excellent example of "card text as material".
- The player creates a bespoke payoff rather than only drawing predefined payoffs.
- Strong lesson for future advanced characters: composition can be more interesting than accumulation.

Risk to avoid:
- High implementation and balance complexity because arbitrary effects combine.
- Text length and rules exceptions can become unreadable.
- Not suitable as our first boss character unless heavily simplified.

### Collector
Core concept:
- Hoarder and soul-collector who burns hand resources for tribute.

Mechanic cluster:
- Collection and Essence create a side-deck/hoard identity.
- Reserve gives stored resource access.
- Pyre requires exhausting another card from hand as an extra cost.
- Doom deals HP loss and can persist if enemies are Afflicted through Weak and Vulnerable.
- Torchhead/temp HP elements support the boss fantasy.

Gameplay question:
- Which card in my hand is worth burning to power this stronger effect, and can I keep the enemy Afflicted long enough for Doom to matter?

Reference value:
- Pyre is a very clean cost model: extra power requires sacrificing real hand material.
- Doom is a useful conditional damage-over-time model because the player must maintain Weak/Vulnerable.
- Collection is a good fantasy layer, but should be kept secondary unless the whole character is about hoarding.

Risk to avoid:
- Exhaust-as-cost can accidentally delete too much of the player's deck plan.
- Too many resource names make the role feel like several characters in one.
- Doom can become "poison with homework" if the Afflicted condition is not rewarding enough.

### Awakened
Core concept:
- Power-hungry cult sorcerer with spellbook, void magic, and eventual awakening.

Mechanic cluster:
- Spellbook contains spells that can be Conjured into hand.
- Awaken happens after a power threshold and upgrades spells for the combat.
- Manaburn rewards energy-drain/Void-style costs.
- Chant effects activate after Power cards and then remain enhanced for the combat.
- Ceremony supports the ritual/power trigger package.

Gameplay question:
- Which powers and spells do I set up now so my later awakened turns are worth the early tempo loss?

Reference value:
- Spellbook is a good way to add class identity without bloating the main deck.
- Chant is a strong "activation memory" pattern: do a setup once, then future copies matter.
- Manaburn is a useful example of turning energy loss into a damage economy.

Risk to avoid:
- Public community reactions suggest the character can feel confusing when Conjure, Manaburn, Ceremony, Chant, and Awakening compete at once.
- A late transformation threshold must pay off clearly in hallway fights, not only bosses.
- Energy-loss mechanics are dangerous if the reward is not immediate and legible.

### Gremlins
Core concept:
- A squad of small monsters, each with its own role, sharing a run.

Mechanic cluster:
- The player has five Gremlins instead of one character body.
- Each Gremlin has separate HP and an ability.
- Shared combat resources and buffs/debuffs, but deaths remove individual Gremlins until act-end revival.
- Switching Gremlins changes tactical role.

Gameplay question:
- Which body should take the turn, which body can risk damage, and how do I preserve the squad over the act?

Reference value:
- Strongest structural lesson: a character can be a roster, not a single unit.
- Separate HP pools create unusual risk distribution.
- Role switching gives the player a tactical identity without inventing many abstract resources.

Risk to avoid:
- High UI, animation, save-state, and balance complexity.
- Damage variance is hard to tune; multi-hit enemies can cause death spirals.
- Not a good first STS2 character unless reduced to one active form and one reserve form.

### Snecko
Core concept:
- Controlled chaos through card cost uncertainty and off-class gifts.

Mechanic cluster:
- Muddle rerolls card costs.
- Overflow rewards high-cost/random-cost outcomes.
- Offclass and Gift generate cards from outside the normal pool.
- Venom provides a debuff/damage line.

Gameplay question:
- Can I steer randomness enough that cost chaos becomes a build plan instead of a coin flip?

Reference value:
- Good reference for "random but steerable".
- Active Muddle is more interesting than passive Snecko Eye because the player chooses when and what to reroll.
- Off-class generation creates huge novelty, but it is not cheap content; it changes balance across the whole game.

Risk to avoid:
- Random generation can become unreadable or impossible to balance.
- Off-class cards are expensive to support in STS2 because mechanics may be character-dependent.
- If chaos is not controllable, the character feels like the game is playing itself.

## Cross-Character Lessons
| Lesson | Downfall examples | Application to STS2 boss characters |
|---|---|---|
| A boss needs one player-facing question | Hermit asks hand-position; Slime asks risk/recover; Champ asks when to finish. | Start every boss character with one repeated question, then reject mechanics that do not sharpen it. |
| Voluntary cost beats forced punishment | Collector Pyre, Slime Tackle, Snecko Muddle. | Convert enemy afflictions into chosen costs with visible upside. |
| Delayed payoff must have interim value | Automaton Functions, Hexaghost Soulburn, Awakened Awakening. | Give common cards enough baseline value so the character survives before the engine completes. |
| Side systems need UI budget | Guardian Gems/Stasis, Hexaghost flames, Gremlins roster. | For `EzDailyContent`, prefer one new UI element at most in the first character. |
| "Not playing" can be action | Hexaghost Afterlife, Automaton Encode delay, Hermit positioning. | Ceremonial Beast can use exactly-one-card turns if the payoff is visible and optional. |
| More mechanics is not more identity | Collector/Awakened show the risk of many named systems. | A STS2 boss character should start with two mechanics and only add a third if playtests prove a gap. |

## Practical Borrowing Rules
Use:
- Hermit's contextual card bonuses.
- Slime Boss's risk/recovery loop.
- Guardian's voluntary threshold.
- Hexaghost's small turn objectives.
- Champ's finisher cadence.
- Collector's hand-sacrifice cost.
- Snecko's steerable randomness.

Use later, not first:
- Automaton-style card assembly.
- Guardian-style socket customization.
- Gremlins multi-body survival.
- Awakened spellbook plus transformation.

Avoid:
- Copying exact Downfall mechanics, card names, art, or values.
- Starting with more than two named mechanics.
- Creating passive slots unless the whole character is explicitly about passive slots.
- Making a mechanic that is only fun after a rare power appears.

## Implication For Current Ceremonial Beast Draft
The current Ceremonial Beast draft should borrow primarily from:

- Hermit: contextual card bonuses, but using first-card position instead of middle-hand position.
- Champ: finisher/cadence thinking, but replacing stance dancing with exactly-one-card restraint.
- Hexaghost: ritual side objective, but reducing six flames to a single Resonance resource.
- Slime Boss: a small risk/recovery line could be added later, but not in the first prototype.

Do not borrow from:
- Automaton Encode/Function, because it is too complex for the first STS2 boss character.
- Guardian Stasis/Gems, because the current goal is to avoid Defect-like slots and custom card-socket UI.
- Awakened Spellbook, because it would create a second deck-management layer too early.
- Gremlins roster, because it is a structural character experiment, not a small mechanic.

## Next Design Move
Before changing the 40-card Ceremonial Beast draft, audit each card against this question:

Does this card make the player care whether it is first, only, or followed by a cashout?

If the answer is no, the card should either become a generic survival card intentionally marked as such, or be cut.
