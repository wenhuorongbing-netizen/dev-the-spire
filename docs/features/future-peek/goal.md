# Future Peek Compatibility Goal

This file is a compatibility pointer for old task links that still refer to
`docs/features/future-peek/goal.md`.

Future Peek is not a second mod in this repository. The active implementation
is folded into the single gameplay-affecting Spire Plus mod:

- Manifest id: `EZMicroBalance`
- Player-facing name: `Spire Plus`
- Source: `EZMicroBalanceCode/Preview/`
- Active docs: `docs/features/preview-tools/README.md`

## Current Verified Package

The current manual-test package line is
`publish/SpirePlus-v0.1.0-private-beta.93.zip`.

Static guards for this compatibility goal live in
`tests/EZMicroBalance.Tests/PreviewToolsGuardTests.cs`. They verify the single
Spire Plus manifest decision, Crystal Sphere mask-only behavior, transform
preview forked-RNG behavior, co-op local UI-only behavior, fail-open fallback,
and player-facing claims that avoid map, reward, relic, or choice foresight
overpromises.

## Implemented In This Pass

- Crystal Sphere peek is a local UI-only preview. It only changes the local
  `%ScryMask` alpha and must not reveal cells, spend charges, or grant rewards.
- Transform preview is a local UI-only preview backed by a forked RNG snapshot.
  It must not advance real transform RNG, create real cards, add reward
  alternatives, or create player choices.
- Both preview tools may run in co-op only as local UI previews. They are still
  pending two-client live evidence.

## Not Implemented From The Archived Intake

Map foresight and reward foresight are not implemented by this compatibility
goal. They can change future room or reward outcomes, so they need a separate
deterministic or host-authoritative precommit plan before implementation.

Do not implement map or reward foresight by adding local-only rolls, reward
choices, `CardRewardAlternative`, reward index changes, `PlayerChoice`, or real
RNG consumption. Those would create desync and save/load risk.
