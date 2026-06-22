# StS1 Events Goal

Status: compact active boundary for the StS1 event prototype.
Full archived record: `docs/archive/feature-audits/event-goal-full-20260622.md`.

## Current Truth

- Current package truth is beta.115 on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `v0.4.34`.
- Spire Plus is RitsuLib-only. `EZMicroBalance.csproj` references `STS2.RitsuLib` `0.4.34`, and `EZMicroBalance.json` declares only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.
- Future StS1 event work must start from RitsuLib docs/XML, active StS1 feature docs, and the unpacked local game source under `source code/src/Core/`.
- The StS1 event prototype remains default Off. Do not use StS1 event work to widen Ancient, Ascension, save/load, replacement, or multiplayer claims.

## Current Evidence Boundary

- Previous beta.108 clicked Ancient UI smoke is retained at `.tools/runtime-evidence/monkey-stability-beta108-20260622-172312/`: 4 / 4 Urda, Morvi, Lotha, and normal Vakuu iterations, clean audits, StS1 Off verifier pass, exact game/Ritsu/package markers, and packet verification 1621 / 0.
- This is Ancient clicked-UI smoke evidence only. It does not close StS1 event gameplay, game-native AutoSlay batch proof, save-load, EN/ZHS runtime render, image/license/render, replacement functional behavior, multiplayer/fail-closed behavior, independent QA, release, or tester handoff gates.
- Previous beta.99 settings/Off proof, beta.96 Off proof, beta.93 AdditiveBatch1 proof, beta.88 smoke-level UI proof, and beta.85-beta.90 rows are retained previous-package or previous-game-version context only.
- Retained beta.85 CanaryOnly loader proof remains previous-package/game-version context and used 4 event types / 6 registration calls. Recapture current CanaryOnly before broader current-runtime claims.
- Previous beta.93 RitsuLib-only AdditiveBatch1 loader registration remains previous-package registration context with 10 event types / 14 registration calls. It must not be extended to gameplay, save-load, replacement, multiplayer, QA, release, or handoff proof.

## Active Gate Map

- Current StS1 event work routes through `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`.
- The O0-O76 gate map and ledger remain the current runtime/static split. The O76-O84 final overlay is static-only and does not close runtime or handoff gates.
- `docs/features/sts1-events/localization-gap-closure-plan.md` remains the StS1 localization gap closure plan. Fixing `STS1_GOLDEN_IDOL.pages.LEAVE.description` only removes the direct localization missing-key blocker and does not prove O25, O33, runtime render, or gameplay.

## Runtime Proof Rules

- Game-native AutoSlay and runtime-monkey packets must use current package/game/Ritsu target switches, including Spire Plus `v0.1.0-private-beta.115`, Slay the Spire 2 `v0.107.1`, and STS2-RitsuLib `0.4.34`.
- Proof-mode AutoSlay commands must include `-ExpectedAncientIds VAKUU,URDA,MORVI,LOTHA`; retained `autoslay-plan.json`, retained `autoslay-summary.json`, and sidecar-plus-current-log event traversal must bind to the same Ancient id before future gameplay proof can count.
- A clean static suite, current-doc scan, v19 ledger, v20 overlay, or subagent coverage run is no-launch governance evidence only. It is not gameplay, save-load, replacement, multiplayer, QA, release, or handoff proof.
- Do not start overlapping validation lanes when a long `dotnet`, package, or game-runtime validation lane is already active.

## Current Open Proof

- Capture current-package CanaryOnly and AdditiveBatch1 runtime proof before claiming current StS1 enabled-mode status.
- Capture StS1 event encounter gameplay, save-load, EN/ZHS runtime render, replacement functional behavior, multiplayer/fail-closed behavior, independent QA, and handoff proof before any completion claim.
- Keep the prototype default Off until the owner explicitly approves a broader runtime/gameplay validation lane.

## Current No-Go Wording

Do not write: all tasks complete, all StS1 events complete, full parity, gameplay-ready, release-ready, or the original game experience is fully matched.
