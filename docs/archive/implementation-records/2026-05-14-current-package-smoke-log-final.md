Final record is archived at [2026-05-14-current-package-smoke-log-resource-verification.md](D:/Game/FOTN/dev-the-spire/docs/archive/implementation-records/2026-05-14-current-package-smoke-log-resource-verification.md). I did not find a caller-configured output-file env var, so I wrote the record there and updated the active docs.

Evidence directory: `.tools/runtime-evidence/current-package-smoke-20260514-015901`.

Final results:
- Package/artifact parity passed. Zip/DLL/PCK hashes match expected values.
- Fixed one local installed-artifact drift: synced installed `README_INSTALL.txt` to match staging/package README.
- Ancient resource smoke passed: Urda/Morvi/Lotha scenes + 43 textures loaded from installed PCK with 0 errors/warnings.
- Steam helper startup passed with only BaseLib + Spire Plus / `EZMicroBalance`: `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, main menu reached, clean audit/manual scan.
- Restore completed: settings/current-run files/moved mods restored, 0 `SlayTheSpire2` processes left running.
- Final validation:
  - `dotnet test EZMicroBalance.sln --no-build`: 109 passed / 18 skipped
  - opt-in artifact tests: 127 passed / 0 skipped
  - `git diff --check`: passed, CRLF warnings only

Changed in this pass: current status/release docs, Ancient/Ascension verification docs, `ReleaseSafetyExpandedGuardTests.cs`, the archived run record, and one final evidence marker file. No gameplay code, exported resources, or package archive contents were changed.

Remaining blockers are still live gameplay/manual matrix, clicked Ancient UI verification, save/load, Vakuu failure/death path, disable-mod gameplay, Rootblight full behavior, natural A11 traversal, and two-client co-op. This is loader/log/resource evidence only, not release readiness.