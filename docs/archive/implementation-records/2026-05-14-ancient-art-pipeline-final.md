Implemented the Ancient art-hygiene pass. Actual image generation was not available because `OPENAI_API_KEY` is absent, though a local imagegen helper path exists. No final art was generated or integrated.

Changed files:
- `docs/features/ancient-expansion-v2.2/art-asset-manifest.json`
- `docs/features/ancient-expansion-v2.2/art-generation-prompts.md`
- `scripts/audit-ancient-art-assets.ps1`
- `tests/EZMicroBalance.Tests/AncientArtAssetHygieneGuardTests.cs`
- `docs/features/ancient-expansion-v2.2/art-direction.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `docs/issues.md`
- `PROJECT_STATE.md`

Manifest summary:
- Final/generated or user-supplied records: `0`
- Source-local background records: `3`
- Temporary records: `52` (`40` source-derived, `12` generic)
- Missing records: `0`
- Audit found `0` missing targets, `0` hash mismatches, and `9` documented duplicate byte groups.

Validation:
- `git status --short --branch`: run before and after; worktree remains broadly dirty with pre-existing unrelated changes.
- `scripts/audit-ancient-art-assets.ps1`: passed in default informational mode.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 124 passed, 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with existing CRLF normalization warnings only.
- Did not run publish/package because no final image bytes, export presets, or package inputs were changed.

No live game, clicked UI, save-load, or co-op validation was performed.