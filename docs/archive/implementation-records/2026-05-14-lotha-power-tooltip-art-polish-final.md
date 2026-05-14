Implemented the Lotha v2.2 polish pass as source/test/package-ready, with no live-readiness claim.

**Files changed**
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/LothaRunHook.cs`
- `EZMicroBalance/localization/eng/ancients.json`
- `EZMicroBalance/localization/eng/relics.json`
- `EZMicroBalance/localization/eng/powers.json`
- `EZMicroBalance/localization/zhs/ancients.json`
- `EZMicroBalance/localization/zhs/relics.json`
- `EZMicroBalance/localization/zhs/powers.json`
- `tests/EZMicroBalance.Tests/LothaPolishGuardTests.cs`
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`
- `docs/features/ancient-expansion-v2.2/source-design.md`
- `docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md`
- `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`
- `docs/features/ancient-expansion-v2.2/api-research.md`
- `docs/features/ancient-expansion-v2.2/work-log.md`
- `PROJECT_STATE.md`
- `docs/issues.md`
- `docs/dev-environment.md`
- `docs/release-checklist.md`
- `docs/private-beta-verification-handoff.md`
- `docs/private-beta-release-completion-audit.md`
- `docs/test-ready-completion-audit.md`
- `docs/features/ancients-rework-v4/completion-audit.md`

**Lotha behavior**
Lotha Power replacement now matches the requested v2.2 source semantics in source/tests: Powers are not extra-played, eligible replacement Powers cost `0` through cost hooks, Mirror Rebuttal grants Energy 2 + draw 2 after play, and Mirror Hall Echo / Deferred Verdict / Single Sentence draw 1 with no Energy gain. Single Sentence keeps waiting if the first played card is a Power before the first Attack/Skill.

**Text**
I found no active localization matches for the old weak Single Sentence wording like `gain 10 Block`, `exactly one card`, or the old generic `Power ... gain 1 Energy` fallback. Option descriptions and option relic hovers are paired in EN/ZHS.

**Art**
No art was generated or replaced. `OPENAI_API_KEY` was unset and no repo-local `image_gen.py` helper was found. `scripts/audit-ancient-art-assets.ps1` reported 0 missing targets, 0 hash mismatches, and 9 known duplicate temporary/source-derived groups, so final bespoke option relic/card/power art remains pending.

**Validation**
- `dotnet build EZMicroBalance.sln --no-restore`: exact first run failed because `Godot.NET.Sdk/4.5.1` was not visible; rerun with `NUGET_PACKAGES=C:\Users\Jack\.nuget\packages` passed.
- `dotnet test EZMicroBalance.sln --no-build`: first run found one stale guard assertion; after fix, passed with 125 passed, 18 skipped, 0 failed.
- `dotnet publish EZMicroBalance.sln --no-restore`: real Steam-folder publish failed on sandbox write permissions; workspace-local redirected publish passed.
- `scripts/package-spire-plus.ps1 -GameRoot .tools\publish-redirect2\game-root`: passed; rebuilt `publish/SpirePlus-v0.1.0-private-beta.0.zip`.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: blocked/failed because the opt-in tests require the real installed Steam mod folder and live log directory.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed, with CRLF normalization warnings only.

No live gameplay, save-load, clicked Ancient UI, death/failure-path, or co-op testing was done.