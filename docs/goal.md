# Spire Plus Goal Guard

This file is the compact guardrail for the active `/goal` thread. Historical `/goal`
intake text was archived to
`docs/archive/feature-inputs/goal-md-mojibake-intake-20260523.md` because the active
copy contained unreadable prompt remnants and duplicated current docs.

Current target:

- Keep `Spire Plus / EZMicroBalance` as a test-ready manual build, not a release-ready build.
- Treat `docs/test-ready-development-goal.md`, `docs/issues.md`, `docs/toreview.md`, and `docs/review.md` as the active work path.
- Preserve `EZMicroBalance` as the stable manifest id.

Closure rules:

- Live proof required before closing runtime rows.
- Source review may close only source-level issues such as compile errors, stale API signatures, localization drift, missing resource paths, manifest drift, hash drift, and guard-test failures.
- Runtime rows need game logs, screenshots, manual notes, or two-client evidence. They include live loader parity, clicked Ancient UI, Urda/Morvi/Lotha/Vakuu gameplay, save/load, Vakuu victory/no-black-screen, A11-A20 traversal/combat behavior, co-op ownership/desync, and Crystal Sphere and transform-preview live proof inside Spire Plus.
- No release-ready claim is made until those runtime rows have direct evidence.

Current static progress already recorded:

- v0.106 / BaseLib 3.1.4 API drift is documented in `docs/audits/v0.106-source-api-drift.md`.
- A19/A20 dedicated boss abilities and Branded Form are source-locked by `BossDedicatedAbilityV41GuardTests`.
- Cross-platform package checks are documented in `docs/platform-testing.md`.

Current stop line:

- No source-only pass may mark this goal complete while the Manual Proof Gates in `docs/issues.md` and `docs/toreview.md` remain open.
