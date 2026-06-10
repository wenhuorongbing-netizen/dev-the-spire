# M5 Revision L Commit Slices

Date: 2026-06-10

No commit, stage, push, package refresh, or rollback is authorized by this file. It is an owner-review plan only.

## Recommended Order

| Order | Slice | Files | Required validation before commit |
|---:|---|---|---|
| 1 | Source API build fix | Lotha API files, Ancient patch files, Martyr Oath, AdditiveBatch1 owner guards, `AncientBehaviorGuardTests.cs` | `dotnet build`, targeted tests, full `dotnet test`, format, diff-check, patch inventory, batch classifier |
| 2 | RitsuLib truth docs | RitsuLib migration docs, integration doc, Batch 4c candidates | Docs review plus full no-game validation if tests guard wording |
| 3 | Revision L owner-review docs | `docs/goals/m5-revision-l-*`, overnight ledgers, harness status/focus | Diff-check and docs guard tests |
| 4 | Current status alignment | `PROJECT_STATE.md`, `AGENTS.md`, `README.md`, `docs/issues.md`, validation/review/status docs | Full no-game validation |
| 5 | Manual handoff harness | `scripts/prepare-current-manual-test-handoff.ps1`, `tests/EZMicroBalance.Tests/ReleaseEvidenceGateTests.EvidenceHelpers.cs` | Full no-game validation after the shared `testhost` lane is clear |
| 6 | Package handoff | Manifest/package/handoff/hash docs, website metadata | Only if owner requests tester build: version bump, publish, package, opt-in artifact tests, fresh smoke |

## Commit Rules

- Keep source changes separate from broad goal-doc cleanup.
- Keep the manual handoff harness separate from gameplay/source changes unless the owner explicitly wants one harness commit.
- Keep package handoff separate from source fix unless the owner explicitly asks for a tester build in the same push.
- Do not ship two different builds under `v0.1.0-private-beta.84`.
- If slice 1 is packaged for testers, increment to the next private beta version and update all package metadata/hashes.

## Current Recommendation

Stop at owner-review after validation. Do not commit or push this debug pass.
