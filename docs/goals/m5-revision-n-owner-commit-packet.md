# M5 Revision N Owner Commit Packet

Date: 2026-06-19
Status: Prepared for owner review at documentation level only. No commit or push is authorized from the current paused lane.

## Current Baseline

- Package line: `v0.1.0-private-beta.88`
- Game target: Slay the Spire 2 `v0.107.1`
- BaseLib: `v3.3.0`
- STS2-RitsuLib: `v0.4.24`, selected runtime branch `lib/0.107.0`
- Current loader evidence: `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/`
- Current proof scope: AdditiveBatch1 loader/registration only

The beta.85, beta.86, and beta.87 loader packets remain previous-package or previous-game-version context unless a specific document names the beta.88 evidence path.

## Owner Decisions

| Area | Recommendation | Reason |
| --- | --- | --- |
| BaseLib `v3.3.0` / beta.88 package line | Accept as current clean-loader package context only | The beta.87 `v0.107.1` attempt failed on BaseLib `v3.2.1` patch drift; beta.88 with BaseLib `v3.3.0` has clean current loader/registration proof. |
| STS2-RitsuLib `v0.4.24` | Keep | Compile package, manifest floor, and installed runtime are aligned on `0.4.24`; the selected compatibility branch remains `lib/0.107.0`. |
| StS1 events | Keep staging-only | Current beta.88 AdditiveBatch1 evidence proves registration only. Event gameplay, render, save-load, replacement, multiplayer, QA, and handoff proof remain pending. |
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
