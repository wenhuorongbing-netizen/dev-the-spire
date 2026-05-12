# Urda Work Log

Project: EZ Micro Balance  
Manifest id: EZMicroBalance

## 2026-05-12 - Urda source gameplay slice

Scope:

- Added source-backed gameplay hooks for the four active Urda blessings.
- Seedbed now adds a Seedbed card reward alternative on normal Act 1 combat card rewards, charges 2 max HP, adds Seedling cards, upgrades the first Seedling, and grants +10 max HP after four accepts.
- Humus Pact now tracks skipped normal Act 1 combat card rewards, grants 15 gold per skip for the first three skips, then opens a 0-2 card removal flow and offers one upgraded card.
- Molting now removes one Strike-like and one Defend-like starter card when selected, adds two Withered Husk cards, and removes deck husks at Act 2+ start.
- Moss Map now grants one small Act 1 reward per first visited room type: normal combat gold, event healing, shop potion, elite upgrade, and rest-site max HP.
- Added Seedling and Withered Husk cards plus EN/ZHS card and card-reward UI localization.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 73 passed, 16 skipped.
- `dotnet publish EZMicroBalance.sln`: passed and refreshed installed DLL/PCK because localization/export resources changed.
- Rebuilt package staging, versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` from installed artifacts.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 89 passed, 0 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.

Current hashes:

- DLL: `EE6B9EE9F2D0D3F4962D6DA11B03E19E6E4806DF08930C1F342BF9530A36A6EF`
- JSON: `479C6AC4C5F9FD5B739C0A2E4442ADD7C0B12FC0514C7CF2153F12553F70FA84`
- PCK: `FCD38F1E5D940D4CDEB94623465FA24D71A75AABFF323586D1B9FBED856D4557`
- Package zip: `2A13A44EA643EA872A8A189883E4EEFFDE8D9DDB8A83A0F5838CE9B6FA8072AD`

Status:

- This is source implementation evidence only.
- No live game, save/load, UI, or co-op Urda verification was run in this pass.
- `URDA-PROTOTYPE` remains open until the manual matrix passes.

## 2026-05-11 - Urda default-on test gate

Scope:

- Changed Urda from `EZMB_FORCE_ANCIENT=URDA`-only activation to default-on private-beta testing.
- Added `EZMB_DISABLE_URDA=1` as the comparison/rollback gate.
- Kept `EZMB_FORCE_URDA_BLESSING=<blessing-id>` for targeted blessing selection diagnostics.
- Updated issue and manual-test docs to keep blessing gameplay blockers open.

Status:

- Urda Act 1 selection is intended to be testable without setting `EZMB_FORCE_ANCIENT`.
- Active blessing effects were implemented in the later 2026-05-12 source slice; no release-ready Urda gameplay claim is made until live checks pass.

## 2026-05-09 - Urda feature documentation skeleton

Scope:

- Created `docs/features/ancient-expansion-urda/` folder.
- Added required feature docs:
  - `README.md`
  - `source-design.md`
  - `implementation-plan.md`
  - `api-research.md`
  - `manual-test-checklist.md`
  - `work-log.md`
- Documentation now defines Urda-only scope, active blessings, and risk boundaries.

Status:

- Source/design records are now present for the Urda overnight vertical slice.
- No gameplay code changes were made in this pass.
- Active Urda issues remain in `docs/issues.md`.

Open follow-up:

- Confirm Urda registration path against local `source code/src/Core`.
- Implement Urda framework and blessing hooks only after API proof updates.
- Add EN and ZHS localization entries for active Urda content.
- Add test logs and manual evidence before private beta release claim.
