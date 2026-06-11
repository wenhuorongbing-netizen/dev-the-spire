# TASK_STATUS - Current Task Short Status

## Current Goal

- M5 Revision M: Slay the Spire 2 `v0.107.0` runtime/package API drift closure + owner-review + runtime truth.

## Current Facts

- Current baseline HEAD must be refreshed before final handoff; existing docs still cite `f32c6767` / later dirty worktree states as validation context.
- Worktree: dirty source/resource/docs/test state; no commit, push, reset, checkout, stash, or broad clean is authorized for this pass.
- Runtime dependency: E-drive BaseLib, STS2-RitsuLib `v0.4.16`, and EZMicroBalance are installed; the current local game is `v0.107.0` and the RitsuLib install includes `lib\0.107.0`.
- Runtime proof: historical clean Off, CanaryOnly, and AdditiveBatch1 diagnostic logs exist for older package/game states; current beta.85 Off loader proof under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` is clean.
- Red root-cause packet: `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` reached main menu but logged 17/25 ModPatcher patches, 8 optional failures, and an `EctoplasmGoldGatePatch` initializer exception.
- Source-fix context: `.tools/runtime-evidence/v01070-current-source-getter-targets-20260610-1000/` applied 25/25 patches and audited clean while still logging beta.84.
- RitsuLib status: installed and beta.85 Off-loader validated on `v0.107.0`; not gameplay/live/release validated.
- Sts1Events recommendation: staging-only; formalization is blocked by gameplay/render/save-load/image/replacement/multiplayer proof, not current build warnings.
- Sts1Events June 11 source changes: Big Fish option identity is now Box/`BOX`; Divine Fountain natural eligibility now requires every run participant to have at least one curse. These remain default-Off/staged and have not been build/test/runtime validated in this paused lane.
- Debug recommendation: accept-scaffold; unused `SpirePlusDebug.LogPreview` removed, broad info diagnostics are internal-only behind `SPIREPLUS_ENABLE_DEBUG_LOGS=1` / `EZMB_ENABLE_DEBUG_LOGS=1`, and preview diagnostics stay behind the localized preview diagnostics setting.
- Patch migration: Batch 4c remains proposal-only pending gameplay proof and owner decision.
- Coordination note: do not start overlapping `dotnet test`, `dotnet build`, publish, package, game-launch, or release-evidence validation processes while another same-repo validation lane is active.

## Verification Result

- Latest completed no-game validation recorded in `PROJECT_STATE.md`: split no-build lanes passed with 475 passed / 0 failed / 21 skipped / 496 total after clearing stale current-repo `testhost` locks.
- `PROJECT_STATE.md` records installed beta.85 package parity as passed via `scripts\check-installed-spire-plus-package.ps1`.
- Beta.85 Off smoke is clean: Spire Plus `v0.1.0-private-beta.85`, RitsuLib compat branch `0.107.0`, 25/25 patches, StS1Events default Off, main menu reached, clean audit.

## Remaining Work

- Reconcile the active `dotnet` / `testhost` validation lane when it reports.
- Run fresh beta.85 CanaryOnly/AdditiveBatch1 smokes only after process coordination is clear and only if StS1 staging proof is needed.
- Owner decisions on source API fix, RitsuLib dependency metadata, Sts1Events governance, Debug governance, and Batch 4c progression.
- Owner approval of commit slices.
- Fresh non-Off runtime smoke before any StS1 staging handoff.
- Gameplay verification (live run, Ancient UI, save-load, route traversal).
- Co-op verification.
- Clicked UI verification.
- Versioned tester-package handoff.
- Independent QA rerun.
