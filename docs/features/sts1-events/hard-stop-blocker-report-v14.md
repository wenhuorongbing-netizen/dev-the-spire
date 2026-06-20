# StS1 Event Port v14 Hard Stop Blocker Report

Date: 2026-05-31
Scope: Mandatory Overnight Run v14 continuation for `docs/goals/event.md`

Superseded note, 2026-06-11: this report is historical hard-stop evidence only. Current beta.85 `v0.107.0` proof covers default-Off loader startup and patch application only; it does not prove CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, image/render, QA, handoff, or release readiness. Current source registration shape is 57 RegisterAll calls and AdditiveBatch1 10 event types / 14 registration calls; older 10/11 and red-loader wording below must stay historical.

Current supersession, 2026-06-20: beta.91 `v0.107.1` proof now covers RitsuLib-only Off and AdditiveBatch1 loader/registration only; it still does not prove gameplay, save-load, replacement, multiplayer, image/render, QA, handoff, or release readiness. Treat any BaseLib dependency wording below as historical, not current instruction.

## Evidence Paths

- Current validation ledger: `docs/reviews/current-validation.md`
- Current status board: `docs/features/sts1-events/status-board.md`
- Registry/count reconciliation: `docs/features/sts1-events/registry-reconciliation.md`
- Asset manifest: `docs/features/sts1-events/asset-manifest.md`
- Runtime smoke checklist: `docs/features/ritsulib-migration/runtime-smoke-checklist.md`

## Current No-Game Evidence

- Focused StS1 guards passed: `dotnet test --filter Sts1EventFeatureGuardTests` -> 31 passed / 0 failed.
- Player-facing naming guard passed: `dotnet test --filter PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable` -> 1 passed / 0 failed.
- Build passed after clearing stale `testhost` locks: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` -> 0 errors / 89 tracked Sts1Events nullable warnings.
- Full no-build tests passed with a single VSTest worker after normal runs intermittently hit the known testhost crash: `dotnet test EZMicroBalance.sln --no-build -- RunConfiguration.MaxCpuCount=1` -> 464 passed / 0 failed / 21 skipped / 485 total.
- Format passed: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`.
- Diff whitespace check passed: `git diff --check` returned 0, with a CRLF normalization warning for existing `docs/patch-inventory.md`.

## Source-Side Fixes Applied

- `AdditiveAllDraft` now requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` before the feature gate reports enabled.
- `ReplaceUnknownEventsPrototype` now reports disabled/fail-closed in normal builds unless `REPLACEMENT_PROTOTYPE_ENABLED` is compiled and `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1` is set.
- Guard tests now cover unsafe-mode override requirements and replacement prototype normal-build fail-closed behavior.
- Active StS1 docs were updated to avoid count/status overclaims and to keep public 52 baseline, 54 canonical rows, 50 registry identities, 48 model files, 47 compiling models, AdditiveBatch1 10 event types / 11 calls, and unsafe-mode gating aligned.

## Non-Green Gates / Blockers

| Gate | Status | Blocker reason | Evidence path | Attempted actions | Required owner/external action |
| --- | --- | --- | --- | --- | --- |
| O18-O22 | BLOCKED | Canary runtime screenshots, result logs, save/load proof, EN/ZHS render proof, and image/render proof require live game execution. | `status-board.md`, `runtime-smoke-checklist.md` | Source guards and docs were updated; no live game evidence exists. | Launch with installed STS2-RitsuLib and `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`, capture screenshots, logs, save/load, EN/ZHS, and image/render evidence. |
| O25-O30 | BLOCKED | Simple batch runtime, result logs, save/load where applicable, ZHS render, asset manifest proof, and image/license decision require live game execution and art/license input. | `simple-batch-specs.md`, `asset-manifest.md`, `status-board.md` | Simple-batch docs now reflect the current source scope; no runtime proof was captured. | Launch with `SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1`; capture Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, and Shining Light evidence; provide art/license strategy. |
| O32-O35 | BLOCKED | Replacement functional proof requires debug symbol, unsafe override, seeded game-run proof of unknown-room replacement, Act bucket behavior, event bag/no-repeat behavior, and save/load persistence. | `multiplayer-fail-closed-guard.md`, `runtime-smoke-checklist.md` | Source gate now fails closed in normal builds; no replacement runtime run exists. | Compile with `REPLACEMENT_PROTOTYPE_ENABLED`, set `SPIREPLUS_STS1_EVENT_MODE=ReplaceUnknownEventsPrototype` and `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`, then capture seeded runtime/save-load proof. |
| O36 | BLOCKED | Multiplayer fail-closed or verified behavior requires a multiplayer runtime session. | `multiplayer-fail-closed-guard.md` | Docs now label host-authoritative behavior as a design assumption, not proof. | Run multiplayer smoke or two-client proof and capture logs/results. |
| O40 | FAIL / BLOCKED | Independent QA/Red-Team cannot pass while runtime gates are missing. | `docs/reviews/refactor-overnight-qa-20260531.md` | Prior QA result remains fail/hard blocked; no new runtime evidence exists. | Rerun independent QA after runtime, art/render, replacement, and multiplayer evidence exists. |
| O41-O46 | BLOCKED | Screenshot bundles, replacement proof bundle, monthly review, handoff, and release evidence cannot be green while runtime/art/replacement/QA gates are blocked. | `status-board.md`, `docs/reviews/current-validation.md` | Active docs were corrected to avoid overclaims. | Start the next run from unresolved runtime and evidence gates, not broad expansion. |

## Hard Stop Reason

Continuation cannot make O0-O50 all green in the current environment. STS2-RitsuLib is now installed locally and fresh loader evidence exists, but the log is not clean and live gameplay, save-load, image/render/license, replacement-pool, multiplayer, and independent QA-pass evidence require further game execution or owner/external action.

## Next Owner Actions

1. Historical action at capture time: rerun loader smoke with the then-required BaseLib + STS2-RitsuLib + Spire Plus stack. Current beta.91 action: use the RitsuLib-only runtime commands from `PROJECT_STATE.md` and `docs/features/sts1-events/README.md`.
2. Capture Off and CanaryOnly `godot.log` evidence before attempting AdditiveBatch1 or replacement-pool proof.
3. Provide an art/license strategy for StS1 event images.
4. Rerun independent QA only after runtime evidence exists.
