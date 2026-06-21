# RitsuLib Runtime Boundary Report

## Current Boundary

This file is the active pointer for the old RitsuLib runtime hard-block lane.
The original May 31 blocker was the absence of a usable RitsuLib runtime
install. That specific blocker is no longer current.

Current package line:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.96`
- STS2-RitsuLib `v0.4.31`
- RitsuLib runtime variant `lib\0.107.1`
- Stable technical manifest id `EZMicroBalance`

Spire Plus is RitsuLib-only for this package line: the project references
`STS2.RitsuLib` `0.4.31`, the manifest declares `STS2-RitsuLib >= 0.4.31`,
and current settings/content/patch/saved-marker integration routes through
RitsuLib APIs.

## Evidence

- Current beta.96 package parity and source-workspace checks are summarized in
  `PROJECT_STATE.md` and `docs/reviews/current-validation.md`.
- Current beta.96 clicked settings proof is retained at
  `.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/`.
  It proves Settings -> `Mod Settings (RitsuLib)` visibility for Spire Plus.
- Older beta.93 Off/AdditiveBatch1 packets are retained only as older package
  loader/registration context. They do not prove beta.96 gameplay or tester
  readiness.

## Still Blocked

The migration is not release-ready. Current hard blocks are beta.96 loader
proof, gameplay, clicked Ancient UI, save-load, replacement behavior,
multiplayer/co-op, independent QA, and versioned tester-package handoff.

Batch 4c and any high-risk patch migration remain proposal-only until those
runtime/manual gates have current evidence and owner approval.

## Required Runtime Proof

1. Keep the official STS2-RitsuLib install under
   `<GameRoot>\mods\STS2-RitsuLib`.
2. Enable only STS2-RitsuLib and Spire Plus for controlled RitsuLib-only proof.
3. Recapture beta.96 loader proof if the package, source, game version, or
   RitsuLib runtime changes.
4. Treat settings screenshots as UI visibility proof only.
5. Withhold live-ready and release-ready claims until gameplay, save-load,
   co-op, and QA evidence exists.

## Next Action

After coordination clears, capture beta.96 loader, gameplay, save-load, render,
replacement, multiplayer, and QA evidence, or record the exact blocker.
