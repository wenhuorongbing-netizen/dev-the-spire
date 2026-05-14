Implemented the narrow follow-up. No C# runtime source changed.

Changed files:
- [LothaPolishGuardTests.cs](<D:/Game/FOTN/dev-the-spire/tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs:353>)
- [work-log.md](<D:/Game/FOTN/dev-the-spire/docs/features/ancient-expansion-v2.2/work-log.md:3>)
- [PROJECT_STATE.md](<D:/Game/FOTN/dev-the-spire/PROJECT_STATE.md:40>)
- [private-beta-verification-handoff.md](<D:/Game/FOTN/dev-the-spire/docs/private-beta-verification-handoff.md:13>)
- [issues.md](<D:/Game/FOTN/dev-the-spire/docs/issues.md:26>)
- [ancient-expansion-v2.2.md](<D:/Game/FOTN/dev-the-spire/docs/issues/ancient-expansion-v2.2.md:77>)

Single Sentence residual risk is closed by method-scoped source guards for the pre-ruling Power fallback, Power-only eligibility, Attack/Skill-only ruling eligibility, remaining-play tracking exclusions, `ShouldPlay` cap behavior, and EN/zhs stale-text prevention. Mirror Rebuttal full-hand handling remained intact and guarded.

Validation:
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 133 passed / 18 skipped / 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.

No publish/package refresh was run, so package hashes did not change. No live gameplay, save-load, clicked Ancient UI, death-path, or co-op testing was run. No final Image API art was generated.

Goal tracker: complete, 405 seconds.