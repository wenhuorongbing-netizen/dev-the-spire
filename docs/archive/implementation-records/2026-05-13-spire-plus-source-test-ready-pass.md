# 2026-05-13 Spire Plus Source/Test-Ready Implementation Record

Historical archive.

This record summarizes the large source-complete test-ready implementation work that preceded the current documentation cleanup. It is not a release-ready claim.

## Implemented Before This Cleanup

- Player-facing mod name refreshed to `Spire Plus` while preserving the stable `EZMicroBalance` manifest id for this cycle.
- Urda stayed default-on for private-beta testing with the current four source-backed blessings: Seedbed, Humus Pact, Molting, and Moss Map.
- Lotha became default-on for private-beta testing with all eight v2.2 blessing ids, custom event art paths, option art paths, dialogue/localization, disable gates, force-Ancient gates, and force-blessing gates.
- The Lotha geometric placeholder background was replaced with the local generated mirror-tribunal background.
- Lotha map/run-history icons, option images, and `lotha_verdict` power art were replaced with temporary source-derived crops. They are not final bespoke generated relic/card art.
- Vakuu fight became source-complete/live-pending for the first single-player test slice: a Fight Vakuu option, custom Event combat, parent-event resume, and three non-Vakuu Act 3 Ancient blessing choices on victory.
- Active Simplified Chinese localization JSON syntax was repaired and guard-tested.
- Release/source guards were expanded around localization JSON validity, Lotha art regression, active resource export coverage, gates, and stale release claims.
- `dotnet build`, `dotnet test --no-build`, `dotnet format --verify-no-changes`, `git diff --check`, and `dotnet publish` passed during the implementation/asset pass.

## Still Not Complete

- No live Lotha gameplay, save/load, lethal-path, or co-op verification was completed.
- No live Vakuu fight UI/gameplay, save/load, failure/death, or co-op verification was completed.
- No final bespoke Image API art was generated because the local `OPENAI_API_KEY` was not set.
- Morvi remains a default-off prototype; the full v2.2 Morvi pool is not implemented.
- Six future Urda blessings remain unimplemented.
- Rich tooltip/highlight polish is incomplete for several new mechanics.
- The package zip/hash evidence must be refreshed after any future resource/code change before release claims.

## Current Replacement

Use `docs/test-ready-development-goal.md` and `docs/issues.md` for the next development pass. Use this archive only for historical traceability.
