# RitsuLib Runtime Boundary Report

## Current Boundary

This file is the active pointer for the old RitsuLib runtime hard-block lane.
The original May 31 blocker was the absence of a usable RitsuLib runtime
install. That specific blocker is no longer current.

Current package line:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.134`
- STS2-RitsuLib `v0.4.34`
- RitsuLib runtime variant `lib\0.107.1`
- Stable technical manifest id `EZMicroBalance`

Spire Plus is RitsuLib-only for this package line: the project references
`STS2.RitsuLib` `0.4.34`, the manifest declares `STS2-RitsuLib >= 0.4.34`,
and current settings/content/patch/saved-marker integration routes through
RitsuLib APIs.

## Evidence

- Current beta.134 package parity, runtime preflight, and source-workspace
  validation are summarized in `PROJECT_STATE.md` and
  `docs/reviews/current-validation.md`.
- Previous beta.128 clicked Ancient UI smoke is captured at
  `.tools/runtime-evidence/monkey-stability-20260623-062913/`. It proves
  smoke-level Ancient UI navigation for Urda, Morvi, Lotha, and normal Vakuu
  only.
- Previous beta.99 clicked settings proof is retained at
  `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
  It proves Settings -> `Mod Settings (RitsuLib)` visibility for Spire Plus,
  rendered the `STS2-RitsuLib >= 0.4.34` settings page, and includes a
  clean same-session log audit.
- Previous beta.96 Off loader proof is retained at
  `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.
  It proves startup/loading and default-Off StS1Events behavior only.
- Older beta.93 AdditiveBatch1 packets are retained only as older package
  loader/registration context. They do not prove beta.134 enabled-mode gameplay
  or tester readiness.

## Still Blocked

The migration is not release-ready. Current hard blocks are gameplay, save-load,
replacement behavior, current beta.134 enabled-mode
registration/gameplay proof, multiplayer/co-op, independent QA, and versioned
tester-package handoff.

Current source has 168 migrated patch classes and 1 raw Harmony declaration,
with beta.134 package/source validation. Previous beta.128 clicked UI smoke
applied 152/152 registered patches from that older package because the StS1
replacement prototype is compile-symbol gated. It still does not prove beta.134
runtime smoke, enabled-mode gameplay, save-load, replacement, co-op, QA, or
release readiness.
Any high-risk patch migration remains proposal-only until those runtime/manual
gates have current evidence and owner approval.

## Required Runtime Proof

1. Keep the official STS2-RitsuLib install under
   `<GameRoot>\mods\STS2-RitsuLib`.
2. Enable only STS2-RitsuLib and Spire Plus for controlled RitsuLib-only proof.
3. Treat beta.128 clicked UI smoke and beta.99 previous-package direct Off
   loader proof as scoped startup/UI/default-Off evidence only.
4. Treat settings screenshots as UI visibility proof only.
5. Withhold live-ready and release-ready claims until enabled-mode, gameplay, save-load,
   co-op, and QA evidence exists.

## Next Action

After coordination clears, capture current enabled-mode, gameplay, save-load,
render, replacement, multiplayer, and QA evidence, or record the exact blocker.
