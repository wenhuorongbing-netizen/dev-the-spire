# EZ Micro Balance Ancients Rework v4

This folder is the active feature record for the `EZ Micro Balance` Ancient reward rebalance.

## Current Files

- `source-design.md`: user-authored source design. Treat as the behavioral source of truth unless a later user decision supersedes it.
- `completion-audit.md`: current implementation/release audit for this feature, including package and validation evidence.
- `manual-verification-matrix.md`: live manual gameplay matrix. Rows remain pending until actually tested.
- `manual-test-checklist.md`: practical tester checklist derived from the matrix.
- `implementation-plan.md`: phased implementation plan created from the source design.
- `api-discovery.md`: local API evidence, patch-point rationale, state strategy, and runtime-risk notes.
- `work-log.md`: chronological implementation and validation log.
- `external-references.md`: external modding references used during discovery.

## Reference And Archive

- `reference-inputs/sts2_ancients_rework_v4_3_adjustment_plan.md`: v4.3 adjustment input copied from the local Downloads folder for traceability and annotated with current implementation/pending-runtime status. v4.3 is current.
- `archive/sts2_ancients_rework_v4_2_next_plan.md`: historical v4.2 next-plan input. v4.2 is superseded by v4.3.
- `archive/goal-prompts.md`: historical prompts used to drive prior implementation batches; not current source truth.

For the global documentation map, use `../../README.md`.

## Current Implementation State

The Ancient v4.3 rebalance has been implemented in the active independent `EZMicroBalance` project under `EZMicroBalanceCode/Ancients/`.

The current v4.3 pass covers Velvet Choker's retained soft limit, Distinguished Cape's `lose 30% of current Max HP, at least 18` trade gate, Prismatic Gem's "Every second standard card reward contains only off-color cards" behavior, and no-space Simplified Chinese number formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.

No source-design item is currently documented as blocked for lack of local compile-time API evidence. Normal Steam-client Mod Settings verification now has RC1 evidence for BaseLib and EZ Micro Balance, but private beta is not complete until:

- the manual verification matrix has concrete runtime results,
- save/load-sensitive behavior is verified in-game,
- disable-mod gameplay behavior is verified,
- the author placeholder is accepted or replaced,
- the final release package is rebuilt after any new code/resource changes.

## Release Scope

In scope for this feature:

- Ancient reward rebalance only.
- Existing game cards, relics, reward screens, rest-site options, and combat hooks needed by the source design.

Out of scope:

- Ascension 21-30.
- Custom character work.
- New unrelated content systems.

Ascension 11-20 is tracked separately in `docs/features/ascension-11-20/` and must remain gated or independently disableable until public selection/progress and runtime behavior are verified.
