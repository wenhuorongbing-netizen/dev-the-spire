# New Character Roadmap

This document is a future roadmap only. It does not authorize implementation.

## Current Status
- New custom character work is the final major project phase.
- Current priority is Ancient reward optimization.
- Second priority is Ascension 11-30.
- No character code, cards, relics, powers, patches, localization, or art should be implemented yet.

## Existing Future Drafts
Future character planning material currently lives under:
- `docs/_future/planning/design-operating-brief.md`
- `docs/_future/new-character/boss-character-concepts-v2.md`
- `docs/_future/new-character/boss-character-design-knowledgebase.md`
- `docs/_future/new-character/ceremonial-beast-character-draft.md`
- `docs/_future/new-character/ceremonial-beast-v3-bell-crowned-design.md`
- `docs/_future/new-character/downfall-character-reference.md`

Future character design should use `docs/_future/planning/design-operating-brief.md` before proposing mechanics.

## Why Character Work Comes Last
A custom character is the largest and highest-risk content target.

It depends on:
- Stable Ancient reward balance.
- Known Ascension pressure from A11-A30.
- Clear compatibility expectations for public beta APIs.
- Localization and art policy decisions.
- A proven build, publish, and manual game verification workflow.

## Compatibility Requirements
The future character must account for:
- Ancient reward eligibility and power level.
- Ascension 11-30 difficulty pressure.
- BaseLib version compatibility.
- Public beta API instability.
- Save/load behavior.
- Mod Settings visibility.
- No multiplayer compatibility claims unless tested.

## Design Requirements Before Implementation
Before any new character implementation:
- Create a dedicated character design spec.
- Define design pillars and core play pattern.
- Define prototype question.
- Define minimum viable card/relic/power set.
- Define localization requirements.
- Define original or permissively licensed asset plan.
- Define test matrix against Ancient reward and Ascension systems.

## Asset and Source Rules
- Do not copy original Slay the Spire 2 assets.
- Do not copy assets from other mods without permission.
- Do not copy large chunks of decompiled game code.
- Placeholder art is acceptable only for internal testing if clearly marked.
- Final release assets should be original, generated, commissioned, or permissively licensed.

## Future Phases

### Phase C0: Character Design Refresh
- Re-read future drafts.
- Select or reject Ceremonial Beast direction.
- Produce one current character spec.

### Phase C1: Prototype Architecture
- Research template-supported character APIs.
- Define minimal implementation plan.
- Avoid deep patches where possible.

### Phase C2: MVP Character
- Implement only after explicit approval.
- Keep scope small.
- Build and publish after each increment.

### Phase C3: Integration Testing
- Test with tuned Ancient rewards.
- Test against Ascension design bands.
- Verify logs and Mod Settings.

### Phase C4: Balance and Release
- Iterate only after stable baseline.
- Package with clear compatibility notes.

## No Implementation Yet
Do not implement a new character until Ancient reward and Ascension baselines are stable and a dedicated character implementation task is approved.
