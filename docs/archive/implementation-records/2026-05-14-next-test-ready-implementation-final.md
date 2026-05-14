Goal completed. Final goal usage: 2399 seconds.

**Implemented**
- Runtime polish: option card hovers for Urda, Morvi, and Vakuu; Forge Token wording; Boss Seal/Brand rich-text descriptions.
- Localization truth pass: EN/zhs Seed Bank, Rootblight notices, Vakuu timing, Lotha Power cost wording, card/relic/power text.
- Art hygiene: manifest now tracks Rootblight/Blight Sprout exported portraits and generic power/relic fallback art; audit/test guards updated.
- Tests/docs: added/updated guards for hovers, localization truth, art manifest coverage, release hashes, and stale-doc prevention.
- Package refreshed: `publish/SpirePlus-v0.1.0-private-beta.0.zip`.

**Changed Files Grouped**
- Code: `EZMicroBalanceCode/Ancients/Expansion/{Urda,Morvi,Vakuu}/`, `EZMicroBalanceCode/Ascension/{Powers,Relics}/`.
- Localization/resources: `EZMicroBalance/localization/eng/*.json`, `EZMicroBalance/localization/zhs/*.json`.
- Art tooling/manifest: `docs/features/ancient-expansion-v2.2/art-asset-manifest.json`, `scripts/audit-ancient-art-assets.ps1`.
- Tests: `tests/EZMicroBalance.Tests/*GuardTests.cs`.
- Docs/status/hashes: `PROJECT_STATE.md`, `docs/issues.md`, `docs/dev-environment.md`, `docs/release-checklist.md`, `docs/private-beta-*`, v2.2/Urda/Ascension/Ancients feature docs.

**Validation**
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 132 passed / 18 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed with the known nested `source code/project.godot` warning.
- Post-publish normal tests: passed, 132 passed / 18 skipped.
- `scripts/package-spire-plus.ps1`: passed.
- Opt-in artifact tests with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`: passed, 150 passed / 0 skipped.
- Final normal tests after docs: passed, 132 passed / 18 skipped.
- `scripts/audit-ancient-art-assets.ps1`: 68 entries, 0 missing targets, 0 hash mismatches, 9 duplicate groups, 57 final-art-pending entries.
- `git diff --check`: passed, CRLF normalization warnings only.
- Stale hash sweep for prior package hashes: no matches.

**Current Hashes**
- Zip: `A96D592E5E244743D1DD0FC58035E34AC263743FFEC98F54CE8D4B31CD9C2432`
- DLL: `A56CF2044A736DFF4E7BEACB55D63388C4DE72AC9C7A99418708D7F2776FE9D9`
- PCK: `1057CD8DEA452226E7872F82A69240E8C8B7321FEC616BE9FB636523A427AFBA`
- Manifest unchanged: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README unchanged: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

**Not Done / Explicitly Pending**
- `OPENAI_API_KEY` was not set, so no image generation ran and no final bespoke art claim was made.
- No live game launch, clicked Ancient UI, gameplay, save/load, death/failure-path, or co-op verification was run.
- Residual source-review risks remain noted: Mirror Rebuttal full-hand log mismatch and a narrower branch-specific Single Sentence guard gap.

**Subagents**
- Art/provenance reviewer found manifest gaps for Rootblight/Blight Sprout and generic power/relic art; addressed in manifest/audit/tests.
- Localization reviewer found stale Seed Bank text, missing hovers, and rich-text gaps; addressed.
- Source red-team found Lotha cost wording, Vakuu timing, and stale-doc issues; addressed except the residual risks above.