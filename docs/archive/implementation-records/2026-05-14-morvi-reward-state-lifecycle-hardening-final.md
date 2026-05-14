Implemented the Morvi reward/state lifecycle hardening.

Fixed concrete source bugs:
- `AncientCardHelpers.TryAddGeneratedCardToCombat` no longer uses Core’s single-card wrapper that can index `[0]` on an empty generated-card result. It now guards combat state and cleans up failed generated cards.
- Red Ink Overdraft no longer generates into a full hand or silently redirects to discard; it verifies the generated card lands in hand and removes wrong-pile results.
- Red Ink unpaid debt and Debt Settlement HP fallback now share a nonlethal damage helper capped at leaving the player at 1 HP.

Updated source, tests, localization, package hash docs, and current feature docs, including Morvi source design/API/risk/manual/work-log status. Core evidence consulted included `CardPileCmd`, `CombatManager`, `CreatureCmd`, rewards/card reward alternatives, card play/model source, and BaseLib `SavedSpireField` docs.

Validation run:
- `git status --short --branch`: `main...origin/main`, already heavily dirty.
- `dotnet build EZMicroBalance.sln --no-restore`: passed, 0 warnings/errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 142 passed / 18 skipped / 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed, CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln --no-restore`: passed with the known nested `source code/project.godot` warning.
- `scripts\package-spire-plus.ps1`: rebuilt `publish\SpirePlus-v0.1.0-private-beta.0.zip`.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 160 passed / 0 skipped / 0 failed.

Refreshed package hashes:
- Zip: `A147B2850C011DDF04D1D12F6817DFC89BDE58193192B524D5B2385986706C72`
- DLL: `EAFBAB44B8AB70C1DC81CC878B1ED1E9C270E799AA2637EEABA16F76E3CBC911`
- PCK: `F279CD94C6BFB0D92B675E5546D937A08C1A121D7B8284549FAD1FD527272377`
- Manifest: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- README: `5B1194440F6B212471E05F0EE117EE7F30E597FAAA916DF91F9378CD529DDCBB`

No live game launch, save-load testing, clicked Ancient UI testing, death/failure-path testing, or co-op testing was run.