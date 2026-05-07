# Design Operating Brief

## Purpose
Future gameplay work in this workspace should use `D:\Game\FOTN\knowledge` as a source-governed game design skill pack. The working goal is to support Slay the Spire 2 balance changes and new character design for `EzDailyContent` after setup and manual game verification are complete.

This brief is not a feature spec. It does not authorize gameplay implementation during setup.

## Source Boundary
- Treat the knowledge base as draft, source-governed scaffolding unless a specific artifact carries usable evidence.
- Default `source_basis`: `unsupported_draft`.
- Default confidence: `weak`.
- Do not parse private or high-risk source bodies.
- Do not summarize copyrighted chapters or claim book doctrine without evidence.
- Do not invent playtest results, telemetry, player reactions, citations, quotes, project evidence, or legal sidecars.
- Use the KB to route design work, structure artifacts, name assumptions, and define tests.

## Design Mission
When feature work is allowed, act as a systems and character designer for a Slay the Spire 2 content mod:

- Balance changes should be framed as testable hypotheses about player choices, resource flow, power curves, and runaway risks.
- New character design should start from player fantasy, repeated verbs, deck-building decisions, readable feedback, and a bounded prototype question.
- No mechanic is considered stable until it has a written design artifact and a validation plan.
- Small, reviewable increments are preferred over large bundled content drops.

## Primary KB Routes
| Design task | Lead KB route | Expected artifact |
|---|---|---|
| Balance, resources, scaling, economy, or runaway loops | `systems_economy_audit` | system map or economy audit |
| Obvious, blind, fake, or low-stakes choices | `meaningful_decision_audit` | decision audit matrix |
| New character fantasy, core verbs, and pillars | `core_experience_definition` | core experience statement |
| Character theme versus mechanics | `narrative_mechanic_alignment` | narrative-mechanic alignment map |
| First playable slice | `prototype_plan` | prototype question sheet |
| Validation before/after implementation | `playtest_plan` | playtest plan and observation sheet |

## Balance Workflow
Use this sequence before changing numbers or adding balance-affecting content:

1. State the design question in one sentence.
2. Map current system parts: cards, relics, powers, statuses, resources, enemy pressures, rewards, and constraints.
3. Identify sources, sinks, feedback loops, power spikes, and possible runaway loops.
4. Audit the main player decision: options, information available, cost, consequence, reversibility, risk, and reward.
5. Choose one tuning lever and one expected observable effect.
6. Define the smallest validation step before broadening the change.

Balance artifacts should avoid invented rates. If no simulation, telemetry, or playtest exists, mark numeric claims as assumptions.

## New Character Workflow
Use this sequence before implementing a new character:

1. Define the core experience: what the player repeatedly does, what they should feel, and what must not be lost.
2. Define 3 design pillars that can reject future mechanics.
3. List the character's core verbs and deck-building tensions.
4. Map unique resources or states only if they create meaningful decisions and readable feedback.
5. Check narrative/function alignment: player role, character function, mechanical expression, and friction points.
6. Build the smallest prototype slice, not a full character kit.
7. Validate whether players understand the character's decision pattern before expanding content.

Avoid designing a character as a pile of mechanics. The character needs a coherent decision identity first.

## STS2-Specific Working Heuristics
These are project-local hypotheses, not verified KB claims:

- A card should create a reason to choose differently across fights, deck states, or future rewards.
- A powerful effect needs a visible cost, timing limit, deck-building constraint, setup requirement, or risk.
- Repeated scaling should be checked for runaway loops and for whether it crowds out alternative strategies.
- Draw, energy, block, damage, exhaust, statuses, powers, relic triggers, and permanent upgrades should be treated as interacting resource flows.
- Randomness should be legible enough that players can plan around it rather than feel tricked by it.
- New character mechanics should be evaluated against existing character space so the mod adds a distinct play pattern instead of a stronger clone.

## Required Artifact Before Code
Before implementing any gameplay feature after setup:

- One core design question.
- One selected KB route.
- One concrete artifact: system map, economy audit, decision matrix, core experience statement, alignment map, prototype plan, or playtest plan.
- Assumptions, `source_basis`, confidence, evidence gaps, and next action.
- A build/publish validation plan.

## Current Status
- Manual in-game verification succeeded: BaseLib and EzDailyContent appeared in Slay the Spire 2 Mod Settings and were enabled.
- No concrete balance changes, cards, powers, relics, patches, or new character behavior should be implemented yet.
- This brief only establishes future design operating rules.
