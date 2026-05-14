Final record:

Updated hash docs only. No gameplay source changed.

Hashes recorded:
- Zip: `83EC2AA5AE6B9EED032A787B625A43B0369ACEA6BB04FAEB3CE6FCE7D99CE7A5`
- DLL: `EB69E895652E610E9C709C1DE9E7929B56AC451C7453F8249A22A85DEFCD719A`
- PCK: `D7FD71CE7AF29DA31DAE464ABE7F94719E104A91D2B67B32B82C191494D61722`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README_INSTALL: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

Files changed:
- `docs/release-checklist.md`
- `docs/private-beta-verification-handoff.md`
- `docs/dev-environment.md`
- `docs/features/ancients-rework-v4/completion-audit.md`
- `docs/test-ready-completion-audit.md`

Validation:
- `dotnet test EZMicroBalance.sln --no-build`: passed, 109 passed / 18 skipped
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 127 passed / 0 skipped
- `git diff --check`: passed, CRLF warnings only

The package hash refresh is documented as automated source validation only; live gameplay, save/load, failure/death-path, and co-op verification remain pending.