# Urda Work Log

Project: Spire Plus (`EZMicroBalance` manifest id)
Manifest id: EZMicroBalance

## 2026-05-13 - Urda v2.2 ten-blessing source completion

Scope:

- Promoted Urda from four active blessing ids to ten default-on source-backed ids: `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, and `urda_seed_bank`.
- Kept `EZMB_DISABLE_URDA=1`, `EZMB_FORCE_ANCIENT=URDA`, and `EZMB_FORCE_URDA_BLESSING` behavior available for focused testing.
- Preserved existing Seedbed, Humus Pact, Molting, and Moss Map source behavior.
- Added Trial Branch with a 4-card rare source-safe grid, visible Trial Branch enchantment/marker, three-combat tracking window, three-success keep path, and removal path after any missed combat.
- Added Shallow-Root Relic with two common relic choices, 75 Gold, Act 1 elite rooting for 35 Gold, and a documented Act 2 removal/refund fallback instead of the unproven `lose 6 Max HP` settlement UI.
- Added Rooted Route with automatic reachable normal-combat marking in the first seven floors, quest markers only, no map graph mutation, three card rewards plus potion-if-slot on success, and an 8 HP / 25 Gold wither fallback.
- Added After the Rain. Current v3.3 source behavior supersedes the original death-prevention draft: Act 1 combat triggers add Rain Breath after the first unblocked enemy attack damage, and Act 2 pays out 75 Gold or heal 8 plus one upgrade based on trigger count.
- Root-Sight now uses the Root Eyes relic as its map control. Clicking it opens map selection, highlights future reachable Monster, Unknown, or Elite rooms, and stores the chosen room's concrete enemy group or event.
- Added Seed Bank with a `Store Seed` reward alternative, max three stored Seeds, and pre-Boss settlement for up to two Seeds.
- Added English/zhs player text, option relics, temporary source-derived option icons, export entries, and guard tests. `OPENAI_API_KEY` was not set, so no bespoke Image API icons were generated.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: first run exposed one stale art-direction guard string, then passed with 98 passed, 18 skipped, 0 failed.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: passed; Godot emitted the known nested `source code/project.godot` ignore warning and regenerated `.uid` metadata for existing new Lotha/Morvi test files while importing the six new Urda option PNGs.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed with 98 passed, 18 skipped, 0 failed.
- No live game, save-load, or co-op testing was run in this implementation pass.

## 2026-05-13 - Urda custom Ancient asset-path fix

Scope:

- Investigated a current A14 Rootblight generated-art hover probe that entered the default-on Urda Ancient event before combat.
- Treated `.tools\runtime-evidence\current-rootblight-art-hover-20260513-114103` as negative evidence: `godot-live.log` reported missing vanilla-derived Urda map icon, run-history icon, and background-scene paths.
- Switched Urda to BaseLib `CustomAncientModel` with `autoAdd: false`, preserving the existing explicit Act 1 registration path.
- Added mod-owned Urda icon/run-history/background scene path overrides and packaged `EZMicroBalance/scenes/events/background_scenes/ezmb_urda.tscn`.
- Added clean headless installed-PCK resource-load evidence at `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345`; Godot loaded the installed `EZMicroBalance.pck` as `--main-pack`, resolved the custom Urda scene/icon resources, emitted `URDA_RESOURCE_LOAD_OK`, and logged 0 `ERROR` / `WARNING` lines.

Validation:

- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 81 passed, 17 skipped after the private-beta release completion audit guard was added.
- `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build`: passed, 98 passed, 0 skipped after the private-beta release completion audit guard was added.
- `dotnet publish EZMicroBalance.sln`: passed and refreshed the installed DLL/PCK.
- Rebuilt package staging, versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.1.zip` from installed artifacts.

Current hashes:

- DLL: `C64B5787625F497E930D4470AB4758950F59D9574D22847996FBCF55E0DACF71` after the later no-test Urda/Morvi hook-state package refresh
- JSON: `9CB73137A04958D0DC0278E854CA1E0E1AC187C125E938DF7C3734F23F7B6A02`
- PCK: `39F0ED5E592BC9131BE7C317450357F9ACC82D7031D97C92C71C59C8B5109736`
- Package zip: `8AA5F65BECF6672B7B41F3B474851A828BFAF60250F04FB2C58061F52747D128` after the later no-test Urda/Morvi hook-state package refresh

Status:

- The missing-asset root cause is source/package-mitigated.
- Post-fix live Urda selection, Rootblight visual/gameplay checks, save/load, and co-op checks remain pending.
- `URDA-PROTOTYPE` remains open.

## 2026-05-12 - Urda acceptance hardening pass

Scope:

- Kept the pass limited to Urda's four current active blessing ids: `urda_seedbed`, `urda_humus_pact`, `urda_molting`, and `urda_moss_map`.
- Did not add Morvi, Lotha, Vakuu, or the six future Urda v2.2 blessings.
- Did not add a new gameplay path; changed Humus Pact completion handling so the third payoff pending bit is cleared only after resolver success.

Source/API evidence:

- Rechecked local Core reward flow: `Reward.SelectUnsynchronized()` calls `Hook.AfterRewardTaken(...)` before the reward set is completed by `RewardsSetSynchronizer`, so Humus Pact still needs explicit pending/reentry guards even after moving off `CardReward.OnSkipped`.
- Rechecked local Core save flow for `SavedSpireField<Player,string>`: `Player.ToSerializable()` writes fixed `SerializablePlayer` fields, `SerializablePlayer` has no general `SavedProperties`/`Props`, `ExtraPlayerFields` is fixed-shape, and inspected `SavedProperties.From(...)` call sites are card/relic/modifier paths. Player-field persistence remains not source-proven.

Validation:

- `git status --short --branch`: branch `main...origin/main` with a dirty worktree already containing unrelated modified files before this pass.
- `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 75 passed, 16 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: not run; this pass did not change resources, localization, export presets, or package logic.

Status:

- Humus `OnSkipped` dependency, Seedbed accept-only counting, future-Urda active-source exclusion, Morvi/Lotha/Vakuu active-source exclusion, and live/save-load docs status now have automated guards.
- Live gameplay, save/load, and co-op checks were not run.
- `URDA-PROTOTYPE` remains open.

## 2026-05-12 - Urda source gameplay slice

Follow-up stabilization:

- Removed the Humus Pact `CardReward.OnSkipped` postfix after local Core source review showed skipped reward finalization can happen during reward-set abandonment/room-exit cleanup.
- Added an explicit `Compost Reward` card reward alternative for Humus Pact.
- Delayed Humus Pact's third-trigger removal and upgraded-card payoff to `AfterRewardTaken`, after the card reward screen has completed.
- Made the Humus payoff custom reward unskippable.
- Suppressed card reward modification hooks for the Humus payoff card and upgraded it explicitly.
- Changed Seedbed accounting so reward generation/reroll does not spend a check; only accepted Seedbed choices advance counters.
- Hid/blocked Seedbed when max HP is not greater than its 2 max HP cost.
- Changed Seedbed's fourth-accept bonus to `SetMaxHp` so the +10 max HP bonus does not also heal current HP.
- Added source guards and EN/ZHS localization for the Humus Pact card reward option.
- Added a backward-compatible `UrdaStateKey` read path for the prior eight-field shape and a new Humus completion-pending bit.
- Source evidence still does not prove `SavedSpireField<Player,string>` persistence; live save/load remains required.
- Validation for the stabilization pass:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 74 passed, 16 skipped.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed because localization/resources changed. Godot emitted the known nested `source code/project.godot` ignore warning during export.
- Release artifact tests were not run because release artifact logic was not changed.
- Live gameplay, save/load, and co-op checks were not run.

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
- Rebuilt package staging, versioned package folder, and `publish\EZMicroBalance-v0.1.0-private-beta.1.zip` from installed artifacts.
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
