# Boss Character Design Knowledge Base

## Purpose
This document is a project-local design knowledge base for turning a Slay the Spire 2 boss into a playable `EzDailyContent` character.

It is a design artifact only. It does not authorize gameplay implementation during setup.

## Source Boundary
- Local source: `D:\Game\FOTN\knowledge`, especially core experience, meaningful decision, economy audit, and prototype output contracts.
- Web sources: public Slay the Spire 2, Downfall, and design-process references listed below.
- `source_basis`: mixed public reference plus unsupported project-local draft.
- Confidence: medium for high-level design constraints, weak for any card numbers.
- Do not copy original Slay the Spire 2 assets, decompiled code, or Downfall card text.
- Treat every numeric value in card drafts as a tuning placeholder until tested.

## Public Reference Notes
1. Slay the Spire 2 is Early Access, and the Steam page explicitly frames balance, feedback, experimental features, metrics, and in-game feedback as part of development.
2. Mega Crit's public design process emphasizes aggressive card idea culling: PC Gamer reports that 100-200 card ideas per character were considered before cutting down to about 60.
3. Older Slay the Spire balance coverage emphasizes data plus subjective feedback: pick rate, winning-deck presence, damage taken, and well-reasoned player feedback all matter.
4. Downfall is the closest public precedent for playable bosses. Its Steam page describes each boss character through a small number of mechanical hooks: Slime Boss uses minions, tackle, goop, and consume; Guardian uses defensive mode, thorns, and gems; Hexaghost uses ghostflames, soulburn, and ethereal/afterlife.
5. Current STS2 boss references suggest useful boss-to-character seeds: Vantom tests multi-hit through Slippery, Soul Fysh tests hand tax through Beckon, Ceremonial Beast uses Plow and Ringing, Waterfall Giant builds Steam Eruption, The Insatiable uses Sandpit pressure, and Doormaker uses doors/phases.

## Translation Rules
Use these rules before drafting a boss character.

| Rule | Meaning | Reject if |
|---|---|---|
| Extract the decision, not the stat block | A playable boss should preserve the player question created by the boss fight, not boss HP or damage. | The concept mainly says "big attacks" or "lots of health". |
| Keep two core mechanisms | STS-style roles need a small repeatable decision grammar. | The concept needs three resources, a pet, a stance, and a slot system to explain. |
| Avoid occupied passive slots for now | Do not drift toward Defect orbs, Regent blade storage, or Necrobinder companion space unless that is the point. | The mechanic has slots that tick every turn and evoke/trigger passively. |
| Translate punishment into voluntary risk | Enemy afflictions feel bad when forced on the player; player versions should be chosen for upside. | The optimal play is to ignore the mechanic or the mechanic only punishes. |
| Every archetype needs floor cards | A 40-card draft must include damage, block, draw, scaling, AoE, and recovery from bad hands. | A card only works after two rares or a specific relic. |
| Design for draft flexibility | The player should be able to take useful cards before committing to an archetype. | Most cards are dead without the character resource. |
| Test one question first | The first playable slice should answer whether the core decision is fun and readable. | The prototype requires the whole 40-card pool. |

## Existing Character Space To Avoid
This is a working map, not a verified taxonomy.

| Character space | Avoided overlap |
|---|---|
| Ironclad | Do not make self-exhaust plus strength the whole identity. |
| Silent | Do not make discard/shiv-like velocity the main payoff. |
| Necrobinder | Do not rely on a companion, execution threshold, or token flood as the core. |
| Regent | Do not make a second persistent currency that simply stores power like Stars. |
| Defect | Do not use passive slots that trigger each turn and can be evoked. |

## Boss Candidate Matrix
| STS2 boss | Playable hook | Fit | Main risk |
|---|---|---:|---|
| Ceremonial Beast | Voluntary one-card turns, first-card impact, ritual resonance. | High | Too slow if it cannot defend while playing few cards. |
| The Insatiable | Hunger clock, devour cards/statuses, feed now versus starve later. | High | Could become Ironclad exhaust plus Regent resource. |
| Soul Fysh | Beckon-style hand tax converted into chosen burdens and cleansing windows. | Medium | Status management may feel annoying instead of empowering. |
| Vantom | Slippery as self-protection or hit-count economy. | Medium | Could become either intangible-lite or shiv bait. |
| Waterfall Giant | Steam pressure, venting, delayed explosion. | Medium | Risk of one-note delayed damage. |
| Doormaker | Phase windows and temporary rules. | Low for first pass | Too close to slot/phase systems and current community controversy. |

## Selected First Target
Start with Ceremonial Beast.

Reason:
- Its boss identity can be translated without passive slots.
- Ringing can become a voluntary "play fewer cards for stronger impact" playstyle.
- Plow can become first-card momentum rather than another stored weapon.
- It creates a clear question every turn: play a normal sequence, or commit to one decisive card.

## Design Pillars For Ceremonial Beast
1. Restraint is power: the character should reward not spending every card immediately.
2. The first card matters: sequencing should be obvious but still skill-testing.
3. No passive slot engine: all major payoffs must come from played cards, turn count, or player restraint.
4. Bad hands must still function: the character needs common block and damage that are acceptable without Resonance.
5. Payoffs should be legible: a player should understand why a large turn happened.

## Prototype Question
Does rewarding exactly-one-card turns create interesting decisions in normal Slay the Spire 2 combats without making the player feel locked out of playing the game?

## First Prototype Scope
- Character relic.
- Starter deck.
- 12-card slice from the 40-card draft.
- Two mechanics only: `Toll` and `Resonance`.

## Excluded From First Prototype
- New relic pool.
- Boss art or copied STS2 assets.
- Custom map mode.
- Enemy replacements.
- Multi-character co-op balance.
- Full unlock progression.

## Validation Signals
Success signals:
- Players can explain `Toll` and `Resonance` after one combat.
- At least once per combat, the player considers ending after one card.
- The player can survive bad draws without always abandoning the mechanic.
- Early attacks and blocks are pickable even without rare powers.

Failure signals:
- The best play is always to ignore one-card turns.
- The best play is always to force one-card turns.
- The character feels like a weaker Regent, Defect, or Ironclad.
- Common cards are too parasitic and fail before the engine appears.

## Evidence Gaps
- No in-game prototype yet.
- No playtest observations.
- No card pool simulation.
- No STS2 API feasibility check for exact card-play counting.
- No numeric balance validation.

## Next Action
Use `docs/ceremonial-beast-character-draft.md` as the first full draft, then cut it down to a 12-card prototype before implementation.
