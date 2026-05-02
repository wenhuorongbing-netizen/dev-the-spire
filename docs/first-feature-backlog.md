# First Feature Backlog

Do not implement these during setup or design-only tasks.

## Corrected project mission
EzDailyContent is a Slay the Spire 2 system expansion mod. The previous Daily Routine / Spark Jab / tiny card pack direction was a temporary placeholder and is no longer the project goal.

Priority order:
1. Optimize and rebalance Slay the Spire 2 Ancient rewards.
2. Design and eventually implement expanded Ascension difficulty from Ascension 11 to 20 to 30.
3. Design and eventually implement a completely new character.

## First Feature Backlog

### Ancient reward research
- Identify all Ancient reward surfaces in public beta `v0.104.0`.
- Catalog reward IDs, effects, trigger timing, constraints, and player-facing text.
- Identify where rewards are defined, registered, granted, and displayed.
- Document which changes can use BaseLib/template APIs and which would require patches.
- Do not copy decompiled code into this repository.

### Ancient reward catalog
- Create a structured catalog of current Ancient rewards.
- Record effect type, power level, run impact, edge cases, and test notes.
- Separate observed facts from design assumptions.
- Track public beta compatibility risks.

### Ancient reward balance principles
- Define what Ancient rewards should do for run variety, risk, build direction, and pacing.
- Identify overpowered, underpowered, unclear, or low-choice rewards.
- Prefer small, reversible tuning levers.
- Avoid broad systemic rewrites for the first MVP.

### One-Ancient MVP reward tuning
- Select one Ancient reward for a minimal tuning experiment.
- Write `docs/ANCIENT_REWARD_SPEC_v0.104.md` before implementation.
- Implement the smallest safe change after explicit implementation approval.
- Run `dotnet build`.
- Run `dotnet publish` if resources or packaging change.
- Verify in game and inspect logs.

### Build / publish / game verification
- Keep `dotnet build` green after every code change.
- Keep `dotnet publish` green after resource, localization, or packaging changes.
- Verify BaseLib and EzDailyContent remain enabled in Mod Settings.
- Record tested game version and branch after verification.

## Later: Ascension 11-20-30 design
- Create an Ascension design spec before implementation.
- Define difficulty goals by band: 11-20 and 21-30.
- Avoid stacking invisible numeric punishment without readable player-facing structure.
- Track compatibility and save/run-state risks.

## Later: new character design
- Use `docs/design-operating-brief.md` before proposing mechanics.
- Define core experience, pillars, verbs, constraints, and prototype question.
- Do not implement a full custom character until Ancient reward and Ascension work have their own stable baselines.

## Deprecated placeholder direction
The old Spark Jab / Focus Tap / tiny card pack ideas are deprecated placeholders. Do not implement them unless the user explicitly revives that direction.
