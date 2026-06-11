# M5 Revision M Version Decision

Date: 2026-06-11
Status: pending owner decision for dependency metadata; no dependency bump performed in this lane.

## Current Versions

- Source manifest version: `v0.1.0-private-beta.85`.
- Compile package: `STS2.RitsuLib` `0.3.2`.
- Manifest dependency: `STS2-RitsuLib` `min_version: 0.3.2`.
- Installed runtime dependency: official `STS2-RitsuLib` `v0.4.16` with `lib\0.107.0`.
- Installed game: Slay the Spire 2 `v0.107.0`.

## Decision

Keep compile and manifest dependency metadata at `0.3.2` for the current dirty source unless the owner deliberately accepts a dependency-version slice. The runtime install can remain `v0.4.16` because it satisfies `min_version: 0.3.2`.

If the owner wants the next tester handoff to require the current RitsuLib package line, bump both:

- `EZMicroBalance.csproj`: `STS2.RitsuLib` to `0.4.16`.
- `EZMicroBalance.json`: `STS2-RitsuLib` `min_version` to `0.4.16`.

That bump must be part of a versioned package pass with static validation, publish/package refresh, package checker, opt-in release-artifact tests, and fresh `v0.107.0` Off smoke.

## Current Boundary

Beta.85 now has recorded Off-loader proof under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` and package-checker pass recorded in `PROJECT_STATE.md`. Do not use beta.85 version docs as gameplay, live-ready, or release-ready proof.
