Implemented the blocker-reduction pass. No manifest id changes, no A21-A30/custom-character work, and no release-ready claim.

**Source Changed**
- [VakuuFightPatch.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs:113)
- [LothaRunHook.cs](D:/Game/FOTN/dev-the-spire/EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs:162)

**Docs/Tests Changed**
- [VakuuLothaSaveRiskGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/VakuuLothaSaveRiskGuardTests.cs:10)
- [AncientPlayerFacingPolishGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/AncientPlayerFacingPolishGuardTests.cs:139)
- [ReleaseCoverageGuardTests.cs](D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs:827)
- [PROJECT_STATE.md](D:/Game/FOTN/dev-the-spire/PROJECT_STATE.md:38)
- [docs/issues.md](D:/Game/FOTN/dev-the-spire/docs/issues.md:12)
- [docs/issues/ancient-expansion-v2.2.md](D:/Game/FOTN/dev-the-spire/docs/issues/ancient-expansion-v2.2.md:88)
- [api-research.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/api-research.md:1)
- [risk-register.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/risk-register.md:1)
- [manual-test-checklist.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/manual-test-checklist.md:1)
- [source-design.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/source-design.md:141)
- [work-log.md](D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/work-log.md:1)

**Risk Reduction**
Vakuu no longer creates the active fight as an unfinished parent-linked `CombatRoom`; `ParentEventId` is only written during `CombatRoom.ToSerializable()` after the Vakuu fight is prefinished. That removes the known Core-rejected active shape. Live active-fight save/load and prefinished empty-reward parent resume remain unverified.

Lotha Death Reprieve now persists `DeathReprieveUsed` plus phase (`None`, `PendingStart`, `Active`, `Resolved`) through the existing deck-mirrored state and rehydrates pending/active protection state. Exact active-turn restore is still not proven because Core run saves do not source-prove full combat hand/energy/pile/power persistence.

**Validation**
- `git status --short --branch` before and after: `main...origin/main` with a broad pre-existing dirty/untracked worktree.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 119 passed, 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed; CRLF normalization warnings only.
- `dotnet publish`: skipped because no resources/localization/export/package inputs were changed.
- No package artifact tests were run.

No live game launch, live save/load, live gameplay, death/failure-path, or co-op validation was performed.