# RitsuLib Runtime Boundary Report

## Current Boundary

This file is the active pointer for the old RitsuLib runtime hard-block lane.
The original May 31 blocker was the absence of a usable RitsuLib runtime
install. That specific blocker is no longer current.

Current package line:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.99`
- STS2-RitsuLib `v0.4.32`
- RitsuLib runtime variant `lib\0.107.1`
- Stable technical manifest id `EZMicroBalance`

Spire Plus is RitsuLib-only for this package line: the project references
`STS2.RitsuLib` `0.4.32`, the manifest declares `STS2-RitsuLib >= 0.4.32`,
and current settings/content/patch/saved-marker integration routes through
RitsuLib APIs.

## Evidence

- Current beta.99 package parity is summarized in `PROJECT_STATE.md` and
  `docs/reviews/current-validation.md`.
- Current beta.99 clicked settings proof is retained at
  `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
  It proves Settings -> `Mod Settings (RitsuLib)` visibility for Spire Plus,
  rendered the current `STS2-RitsuLib >= 0.4.32` settings page, and includes a
  clean same-session log audit.
- Previous beta.96 Off loader proof is retained at
  `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.
  It proves startup/loading and default-Off StS1Events behavior only.
- Older beta.93 AdditiveBatch1 packets are retained only as older package
  loader/registration context. They do not prove beta.99 enabled-mode gameplay
  or tester readiness.

## Still Blocked

The migration is not release-ready. Current hard blocks are gameplay, clicked
Ancient UI, save-load, replacement behavior, current beta.99 enabled-mode
registration/gameplay proof, multiplayer/co-op, independent QA, and versioned
tester-package handoff.

Batch 4c and any high-risk patch migration remain proposal-only until those
runtime/manual gates have current evidence and owner approval.

## Required Runtime Proof

1. Keep the official STS2-RitsuLib install under
   `<GameRoot>\mods\STS2-RitsuLib`.
2. Enable only STS2-RitsuLib and Spire Plus for controlled RitsuLib-only proof.
3. Recapture beta.99 direct Off loader proof after the settings-page I18N resource
   migration.
4. Treat settings screenshots as UI visibility proof only.
5. Withhold live-ready and release-ready claims until gameplay, save-load,
   co-op, and QA evidence exists.

## Next Action

After coordination clears, capture current enabled-mode, gameplay, save-load,
render, replacement, multiplayer, and QA evidence, or record the exact blocker.
