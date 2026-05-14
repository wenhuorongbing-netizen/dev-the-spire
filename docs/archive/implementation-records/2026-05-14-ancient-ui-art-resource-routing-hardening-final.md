Goal completed. Tool-reported goal time used: 680 seconds.

No concrete Ancient UI/art routing bug was found; this was guard/docs hardening only. Static audit confirmed event backgrounds, map/run-history icons, option marker relic art, and export coverage are routed separately and correctly.

Changed files:
`PROJECT_STATE.md`; `scripts/audit-ancient-art-assets.ps1`; `scripts/README.md`; `tests/EZMicroBalance.Tests/AncientArtAssetHygieneGuardTests.cs`; `tests/EZMicroBalance.Tests/AncientUiReadinessGuardTests.cs`; `docs/issues.md`; `docs/issues/ancient-expansion-v2.2.md`; `docs/features/ancient-expansion-v2.2/{api-research.md,art-direction.md,manual-test-checklist.md,risk-register.md,work-log.md}`.

Evidence consulted:
local Core source for `EventOption`, `NAncientEventLayout`, `EventModel`, `RelicModel`, `ImageHelper`, and run-history map icon rendering; active Ancient source under `EZMicroBalanceCode/Ancients/Expansion`; resources under `EZMicroBalance/images/events`, `images/ancients`, `images/relics`, `images/powers`, `scenes/events/background_scenes`; `export_presets.cfg`; bilingual Ancient/relic localization.

Validation:
- `scripts\audit-ancient-art-assets.ps1 -FailOnMissingExport`: 68 assets, 0 missing targets, 0 hash mismatches, 0 missing exports, 9 duplicate groups, 57 final-art-pending.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 144 passed, 18 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `git log -1 --oneline --decorate`: `a2183ee (HEAD -> main, origin/main, origin/HEAD) 1`.

Package was not refreshed in this pass. Current artifact hashes remain:
- zip `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`
- DLL `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`
- PCK `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`
- manifest `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live game, clicked Ancient UI, gameplay, save-load, death/failure-path, or co-op testing was run. No Image API final art generation was run; `OPENAI_API_KEY` is not set.