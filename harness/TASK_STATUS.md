# TASK_STATUS - Current Task Short Status

## Current Goal

- Continue the RitsuLib-only cleanup/refactor goal: keep clicked/UI migration truth current, remove stale dependency and migration-batch guidance, and reduce documentation bloat without claiming live readiness.

## Current Facts

- Current package line: Spire Plus `v0.1.0-private-beta.135`.
- Current local game: Slay the Spire 2 `v0.107.1`.
- Current dependency: STS2-RitsuLib `v0.4.34`; the manifest declares only `STS2-RitsuLib >= 0.4.34`.
- Current source shape: 169 migrated RitsuLib `IPatchMethod` patch classes and 0 raw Harmony declarations.
- Latest clicked UI smoke remains previous beta.128 evidence under `.tools/runtime-evidence/monkey-stability-20260623-062913/`; it covered Urda, Morvi, Lotha, and normal Vakuu for that older package.
- Not proven: beta.135 runtime smoke, gameplay, gated Vakuu fight-option/victory return, save-load, replacement functional behavior, multiplayer/fail-closed behavior, current enabled-mode proof, independent QA, release readiness, or tester handoff.
- Recapture worktree status before any handoff, staging, commit, or push decision.

## Verification Result

- Latest pushed cleanup slice: `8814ed19 Use domain names for RitsuLib patch registry`.
- That slice passed build 0 / 0, focused migration/compactness tests 59 / 0, current-doc claims 1331 / 0, static-file hygiene 13 / 0, source-workspace guard 54 checks / 0 mismatches with the retained GDRE warnings, latest RitsuLib package check 9 / 0, format, repository hygiene, diff-check, retired dependency grep, and raw `[HarmonyPatch]` grep.

## Remaining Work

- Keep current docs and harness state aligned with beta.135 / RitsuLib `0.4.34`.
- Continue source-preserving refactors only where they reduce real coupling or stale migration scaffolding.
- Fill or explicitly defer live/manual runtime evidence rows in the handoff and release-status docs after user/runtime evidence exists.
- Keep release-ready and live-ready claims blocked until gameplay/UI/save-load/co-op/QA/handoff evidence exists.
