Completed the goal file pass.

Runtime C# source changed: yes. I added source-visible restore/fallback logging in:
- [LothaRunHook.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs:1292)
- [VakuuFightPatch.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs:162)

Files I changed in this pass:
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs`
- `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs`
- `tests/EZMicroBalance.Tests/VakuuLothaSaveRiskGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `PROJECT_STATE.md`
- `docs/issues.md`
- `docs/issues/ancient-expansion-v2.2.md`
- `docs/features/ancient-expansion-v2.2/risk-register.md`
- `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`

Reduced risks:
- Lotha Death Reprieve now logs restored pending/active phase, power presence, and live-pending active-turn restore status.
- Guards now assert reprieve phase is persisted before current-turn or pending-start handling.
- Vakuu victory fallback now logs the ownerless restored fallback path.
- Guards now assert Vakuu keeps normal combat rewards / `LinkedRewardSet` out of the custom fight path.
- Current hash doc drift is guarded.

Still live-pending:
- Actual live save/load for Lotha active/pending reprieve.
- Vakuu active fight restore, prefinished restore, parent resume, no-normal-reward runtime flow.
- Live gameplay, clicked Ancient UI, death/failure path, and co-op verification.

Stale current hash claims: found and fixed in `docs/issues.md`. Archived records and the active goal file were left untouched.

Validation:
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: initial new guard failure fixed; rerun passed 136 passed / 18 skipped / 0 failed. Final post-doc rerun also passed 136 / 18 / 0.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: initial whitespace failure fixed with `dotnet format`; final verify passed.
- `git diff --check`: passed with CRLF normalization warnings only.

No `dotnet publish`, package refresh, or opt-in artifact hash tests were run, so package hashes did not change. No live gameplay, save-load, clicked UI, death-path, or co-op testing was run. No Image API art was generated.