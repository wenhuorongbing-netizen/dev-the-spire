# EZ Micro Balance Issue Archive

This document records issues that were resolved and removed from active tracking.
The entries here are retained for traceability and not for active action.

## Resolution date: 2026-05-08 to 2026-05-09

### `ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED`

Status: resolved.

- Rootblight marker removal and map marker cleanup are implemented.
- A11 now uses widened map geometry without dedicated long-road route-node markers.
- Inherited marker regression checks are now tracked by other Ascension live-verification items.

### `ISSUE-2026-05-08-V105-PREVIOUS-FRAMEWORK-CREATURE-SHOWSINFINITEHP-API-DRIFT`

Status: resolved for dependency/runtime gate.

- previous framework dependency was pinned to v3.1.2.
- Controlled single-mod smoke and Mod Settings checks are clean for this API drift.
- Gameplay blockers from this missing API are removed after dependency update.

### `ISSUE-2026-05-07-HANDOFF-GIT-STATUS-HYGIENE`

Status: resolved.

- Release handoff status docs were updated with current branch and status checks.
- Handoff docs now avoid stale "not pushed / dirty" statements.

### `ISSUE-2026-05-07-RELEASE-ARTIFACT-TESTS-DEPEND-ON-IGNORED-PUBLISH-OUTPUT`

Status: resolved.

- Release artifact tests are now opt-in via `ReleaseArtifactFactAttribute`.
- Normal `dotnet test` no longer depends on ignored publish artifacts in clean test runs.

### `ISSUE-2026-05-07-CURRENT-PACKAGE-RUNTIME-SMOKE-STALE`

Status: resolved.

- Controlled runtime-smoke freshness is refreshed.
- previous saved-state API count documentation is synchronized with current package output.
- Controlled previous framework + EZMicroBalance startup checks are updated and clean.

### `ISSUE-2026-05-07-A12-FORGE-TOKEN-RESTSITE-CRASH`

Status: resolved by player report.

- Forge Token rest-site crash is reported as fixed in live follow-up.
- It remains in regression memory but not an active blocker.
