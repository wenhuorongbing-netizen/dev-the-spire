# Ascension 11-20 Work Log

Status: active summary only. The full chronological log was archived to `docs/archive/feature-work-logs/ascension-11-20/work-log-20260518-pre-slim.md`.

## Current Summary

- A11 map generation keeps the wider, longer route shape and guards against forced chokepoints.
- A12 Firemarked Elites, A13 Fission, A14-A18 Rootbud/Rootblight, A17 deep route, A19 boss dedicated abilities, and A20 Branded Form stay within the current A11-A20 scope.
- A21-A30 and custom characters remain out of scope.
- Firemark, Banner, boss dedicated ability, and shared Ascension combat helpers have been split out of the central combat modifier file.
- Dedicated ability effect groups are below the current refactor size threshold.
- The 2026-05-22 art pass promoted GPTimage2 Rootbud/Rootblight card portraits and 12 per-boss dedicated ability / Branded Form transparent icons; export/import metadata and manifest tracking are updated, while live in-game visual proof remains pending.

## Manual Gates

- Natural A11 traversal still needs live map-click proof.
- A12 Firemark counterplay windows need live combat UI proof.
- A16 Banner room behavior needs single-enemy and multi-enemy live proof.
- A19/A20 boss behavior needs live Act 3 and final-boss evidence.
- Multiplayer ownership and save/load remain pending.

## Validation Baseline

Recent no-game cleanup validation:

- `dotnet build EZMicroBalance.sln`
- `dotnet test EZMicroBalance.sln --no-build`
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
- `git diff --check`

Package refresh, artifact tests, and live game evidence are separate gates.
