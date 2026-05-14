Implemented the Lotha Public Evidence debuff-policy hardening and refreshed the package artifacts.

**Source Evidence**
Used local Core source as primary evidence:
- `PowerModel.GetTypeForAmount(...)` / `PowerType.Debuff` for base classification.
- `PoisonPower` is a Debuff but deals unblockable/unpowered tick damage, so excluded.
- `WeakPower`, `VulnerablePower`, and `FrailPower` are non-damage Debuffs, so still count.
- Excluded source-proven damage/kill Debuffs: Poison, Constrict, Demise, Disintegration, Doom, Magic Bomb, Strangle, and The Gambit.

**Files Changed**
Runtime/text/tests/docs touched by this pass:
`EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs`; EN/ZHS `ancients.json` and `relics.json`; `tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs`; `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`; v2.2 source/risk/manual/work-log/docs; `PROJECT_STATE.md`; `docs/issues.md`; `docs/dev-environment.md`; release/checklist/handoff/hash docs.

**Validation**
- `git status --short --branch`: ran; worktree remains broadly dirty with pre-existing unrelated changes.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 137 passed, 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: initially found two wrapping issues, fixed with `dotnet format`; final verify passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed with the known nested `source code/project.godot` warning.
- `scripts/package-spire-plus.ps1`: refreshed `publish\SpirePlus-v0.1.0-private-beta.0.zip`.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: first run exposed stale hash docs; after updating current hash docs/tests, rerun passed, 155 passed, 0 skipped.

**Package Hashes**
Refreshed current artifacts:
- Zip: `1251965F11FE6CFCF285BDBC6D06E84B9AE8DE71681D17AA233B65EF138929F8`
- DLL: `13DB71D52A391DC40BF47C85D1DE3AD29D2BEC2B64DCBBBE86F55C33D4A50F24`
- PCK: `63489FE6D2E04B664CB5445BD6F4F1EFAFDE6AD72ECBE75CB067EA08E8F6E8C8`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live game, live gameplay, save-load, clicked UI, death/failure-path, co-op, or Image API art testing was run.