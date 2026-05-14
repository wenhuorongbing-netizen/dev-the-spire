Implemented the clicked-Ancient UI hardening pass and wrote the final record here: [2026-05-14-ancient-ui-click-hardening-final.md](D:/Game/FOTN/dev-the-spire/docs/archive/implementation-records/2026-05-14-ancient-ui-click-hardening-final.md:1).

Key changes:
- Added explicit fallback logging and expected option counts in Urda/Morvi/Lotha: Urda now shows 4 initial options; Morvi/Lotha stay at 3.
- Added `AncientUiReadinessGuardTests` for scene roots, event-art paths, option marker art/localization, force gates, fallback source shape, and no false clicked-UI claims.
- Documented that no safe automated clicked-Ancient UI path exists yet, with exact manual force/test commands in [manual-test-checklist.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/manual-test-checklist.md:16).
- Updated active docs, package hashes, Urda EN/zhs “four bargains” text, and the Vakuu fight marker relic sentence.

Evidence status:
- No new clicked UI runtime evidence was collected.
- Existing referenced evidence remains `.tools/runtime-evidence/current-package-smoke-20260514-015901`, but that is loader/resource evidence only.

Validation:
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 114 passed / 18 skipped.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed, known nested `source code/project.godot` warning.
- `scripts/package-spire-plus.ps1`: rebuilt package.
- Opt-in artifact tests: first run caught stale hashes, rerun passed 132 / 0 skipped.
- `git diff --check`: passed with CRLF normalization warnings only.
- No `SlayTheSpire2` process was left running.

Remaining blockers are unchanged: actual clicked UI screenshots/logs for Urda/Morvi/Lotha/Vakuu, live gameplay matrix, save/load, Vakuu death/failure path, co-op disposition, and final bespoke art.