# M5 Revision N Owner Commit Packet

Date: 2026-06-19
Status: Prepared for owner review at documentation level only. No commit or push is authorized from the current paused lane.

2026-06-20 supersession: this Revision N packet is previous BaseLib-backed context. Current dependency/package truth is beta.91 on Slay the Spire 2 `v0.107.1` with `STS2-RitsuLib` `v0.4.28`, `lib/0.107.1`, and no Spire Plus BaseLib dependency.

## Revision N Baseline

- Package line: `v0.1.0-private-beta.88`
- Game target: Slay the Spire 2 `v0.107.1`
- BaseLib: `v3.3.0`
- STS2-RitsuLib: `v0.4.24`, selected runtime branch `lib/0.107.0`
- Then-current loader evidence: `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/`
- Proof scope: AdditiveBatch1 loader/registration only

The beta.85, beta.86, beta.87, beta.88, and beta.90 loader packets now remain previous-package, previous-game-version, or previous-dependency context unless a specific document names beta.91 evidence.

## Owner Decisions

| Area | Recommendation | Reason |
| --- | --- | --- |
| BaseLib `v3.3.0` / beta.88 package line | Retain as previous BaseLib-backed loader context only | The beta.87 `v0.107.1` attempt failed on BaseLib `v3.2.1` patch drift; beta.88 with BaseLib `v3.3.0` has clean loader/registration proof but is superseded by beta.91 RitsuLib-only proof. |
| STS2-RitsuLib `v0.4.24` | Superseded by `v0.4.28` for current work | Revision N compile package, manifest floor, and installed runtime were aligned on `0.4.24`; current beta.91 uses `0.4.28` and selected compatibility branch `lib/0.107.1`. |
| StS1 events | Keep staging-only | Current beta.91 AdditiveBatch1 evidence proves registration only. Event gameplay, render, save-load, replacement, multiplayer, QA, and handoff proof remain pending. |
| Debug scaffold | Accept scaffold, do not expand | General diagnostics remain internal-only behind `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`; preview diagnostics remain behind the localized preview setting. `debug.md` explicitly blocks debug expansion. |
| Runtime monkey / AutoSlay hardening | Accept as guard/harness hardening only after validation replay | Pause-safe verifier/analyzer hardening improves future evidence quality but does not create game-native monkey batch proof. |
| Batch 4c migration | Defer | Candidate review remains proposal-only. No migration should occur without owner approval and fresh validation. |
| Commit and push | Defer until validation replay and owner authorization | The current coordination pause prohibits starting overlapping validation/runtime lanes and prohibits handoff actions from this thread. |

## Commit Slice Sketch

Do not treat this as an exact dirty-file manifest. Recapture `git status --short --branch`, `git log -1 --oneline --decorate`, and `git diff --stat` immediately before any commit decision.

Candidate slices after validation replay:

1. Beta.88 dependency/package alignment and release docs.
2. Runtime-harness and AutoSlay verifier/analyzer hardening.
3. StS1 event governance and current-doc alignment.
4. Revision N owner-governance docs and harness state.

Split or defer any slice whose validation fails. Do not include unrelated local changes, ignored `.tools/` evidence, build outputs, local game files, or downloaded runtime binaries.

## Required Owner Inputs

- Whether beta.88 should be accepted as the current loader-registration package context.
- Whether to authorize a commit after a clean coordinated validation replay.
- Whether Batch 4c remains deferred or receives a separate owner-approved implementation lane.
- Whether any pending gameplay/manual rows may be explicitly deferred for tester handoff. Without that decision, release-ready and live-ready claims remain blocked.

## Current Decision State

Not complete. This packet is ready for owner review, but it cannot authorize commit, push, release, or handoff until the paused validation/runtime lane is reconciled and owner approval is explicit.
