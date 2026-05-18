# Private Beta Release Checklist

Target mod: `Spire Plus`
Target manifest id: `EZMicroBalance`

## Architecture

- [x] Existing `EzDailyContent` manifest id remains unchanged.
- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.
- [x] Enabling `EZMicroBalance` does not require enabling legacy `EzDailyContent`.
- [x] Custom-character work is not included in this private beta.
- [x] A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Host multiplayer A20 selection/start logs a downgrade warning because Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification. Full live Ascension and co-op verification is pending.

## Build And Publish

- [x] `dotnet build` succeeds.
- [x] Latest source/package build check passed with `dotnet build EZMicroBalance.sln --no-restore` after the Morvi reward/state lifecycle hardening pass.
- [x] Publish/package/hash refresh has been rerun for the latest source/text/resource slices with `dotnet publish EZMicroBalance.sln --no-restore` and a rebuilt `SpirePlus` private-beta zip.
- [x] `dotnet publish` succeeds.
- [x] Published `EZMicroBalance.json` exists.
- [x] Published `EZMicroBalance.dll` exists.
- [x] Published `EZMicroBalance.pck` exists.
- [x] Manifest declares structured `BaseLib` dependency with `min_version: v3.1.2`.
- [x] Manifest has `affects_gameplay: true`.
- [x] PCK audit excludes legacy `EzDailyContent`, C# source, docs, art, asset, and archive folders.
- [x] Normal source/localization/documentation guard tests do not require ignored publish/package artifacts.
- [x] Release artifact tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` after publish and package refresh.
- [x] Release artifact, installed DLL/PCK, package hash, and runtime-smoke evidence tests pass after the latest package refresh with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [x] `publish/SpirePlus-v0.1.0-private-beta.0.zip` was rebuilt from the configured installed artifacts on 2026-05-18 after the strict hook/text audit, Banner temporary Strength cleanup, Ascension side-turn state fix, Vakuu combat hook ownership cleanup, Vakuu no-reward resume split, Vakuu parent-stack resume guard, Vakuu/Lotha victory eligibility filter, Morvi payment split, Morvi Debt/Overdraft counter Artifact safety, Morvi Overdue Library exact discount source, Morvi constant ownership split, Seed Bank extraction failure preservation, Ancient option hover completion, Root Eyes stale-preview cleanup/map-screen guard, Root Eyes preview consumption, duplicate-reservation avoidance, Claws pickup split, Morvi/Lotha empty-choice guards, Meat Cleaver Cook split, and A13 Fission reward filter repair, Soul Tide Beckon pre-flush fix, Boiling Critical tooltip correction, Chosen Decree summary correction, and Ascension map marker ordering split. Current package DLL SHA256 `54524355D5F6986A017E06E7F3EC996BF5B4F8A23870B518E40D38AB91EF1096`; manifest SHA256 `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2`; PCK SHA256 `9547FD17CEAC9719A3BA044A9E47D65A7F3F942C248559136D380EFD75AB2B86`; README SHA256 `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4`; zip SHA256 `5AEE65325C4248E8BFB86268E360E24BB68B428B2CBC6CDA96F8F86DA483228A`. This hash refresh records automated source/package validation only; live gameplay, save-load, natural A11 route-click traversal, failure/death-path, clicked Ancient UI, and co-op verification remain pending.

## Runtime

- [x] BaseLib appears in Mod Settings.
- [x] BaseLib loads when enabled in a controlled smoke profile.
- [x] Spire Plus / `EZMicroBalance` appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.
- [x] Spire Plus appears in a refreshed Mod Settings UI screenshot after the display-name refresh package is installed.
- [x] Historical EZ Micro Balance display-name Mod Settings evidence exists for the same `EZMicroBalance` manifest id.
- [x] Spire Plus / `EZMicroBalance` loads when enabled in a controlled smoke profile for the current package.
- [x] Game reaches main menu with only BaseLib and Spire Plus / `EZMicroBalance` loaded in the controlled smoke profile for the current package; unrelated disabled local-mod manifest/name noise is still present in the developer mods folder.
- [x] Historical normal Steam helper startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with only BaseLib and Spire Plus / `EZMicroBalance`, reported `Found 22 SavedSpireFields`, reached main menu in `13,539ms`, restored settings, 24 moved mod entries, and 2 current-run files, left 0 `SlayTheSpire2` processes, and audited clean. current source defines 25 SavedSpireFields after the 2026-05-17 static fixes, so a fresh live loader smoke is pending for the current package.
- [x] Repeat helper-driven normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` reached main menu with only BaseLib and Spire Plus / `EZMicroBalance`, reported `Found 16 SavedSpireFields`, restored settings plus 24 moved mod entries, and audited clean.
- [x] BaseLib-only plug-off normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` reached main menu with `EZMicroBalance` temporarily isolated out of the mods folder, loaded `1 mods (1 total)`, initialized BaseLib only, did not initialize Spire Plus / `EZMicroBalance`, restored settings plus 25 moved entries and the current-run save, and audited clean.
- [x] `godot.log` reviewed for controlled smoke-test initializer errors.
- [x] `godot.log` reviewed after current normal Steam-client isolated startup/log verification.
- [ ] `godot.log` reviewed after full normal Steam-client gameplay/manual verification.

## Content Verification

- [x] Every implemented Ancient reward change has a manual checklist row.
- [ ] Every implemented Ancient reward change has a completed manual runtime result.
- [ ] Save/load-sensitive behavior is tested.
- [ ] Disable-mod gameplay behavior is tested in a run.
- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.
- [x] Rootblight I/II/III and Blight Sprout generated portrait art is integrated and packaged; live in-game visual verification remains part of the manual matrix.
- [x] Urda, Loamweaver has a source-backed first gameplay slice for Seedbed, Humus Pact, Molting, and Moss Map, filters run-state player loops through `Player.IsActiveForHooks`, uses custom Ancient icon/background scene asset paths, packages the Urda background scene, and has a clean headless installed-PCK resource-load check for the custom scene/icon under `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345`; live Urda gameplay and save/load checks remain pending.
- [x] Current installed-PCK Ancient resource smoke under `.tools/runtime-evidence/current-package-smoke-20260514-015901` loads Urda/Morvi/Lotha background scenes and 43 Ancient textures with 0 errors/warnings, verifies map/event/option art paths are distinct and exported, and finds 0 missing EN/zhs localization keys; clicked live Ancient UI remains pending.
- [x] Lotha is default-on in source for Act 3 private-beta testing with all eight v2.2 blessing ids, custom scene/art, option marker relics, English/zhs localization, and disable/force gates; live gameplay, save/load, lethal-path, and co-op checks remain pending.
- [ ] Fight Vakuu remains hidden by default. It now has a dedicated Vakuu enemy and encounter scene, but still needs live post-victory no-black-screen, save-load, failure/death, and co-op checks before it can be exposed to normal testers.

## Proof Audit

This section maps the current goal to concrete evidence. Passing automated tests is not enough by itself; each row needs the named proof before the package can be treated as publish-proven.

Before any release-ready claim, fill a manual release evidence manifest and run:

```powershell
.\scripts\verify-spire-plus-release-evidence.ps1
```

The verifier hashes the package under test and requires current package hash parity, manifest/evidence dirs/files that stay inside the declared evidence root, matching row kinds, `command.txt` for every passed row, clicked-UI screenshots with foreground preflight, clean `godot-log-audit.json` files, result notes, save/load rows, Vakuu victory/failure rows, Rootblight/A11/disable-mod rows, and co-op disposition. Row `RequiredFiles` may add files but cannot remove the defaults. Unknown or blank manifest rows are ignored but reported as warnings. Deferred rows fail unless rerun with `-AllowDeferred` after an explicit owner-approved release note.

| Requirement | Current artifacts / evidence | Status |
| --- | --- | --- |
| Dedicated Vakuu combat loop | Source: `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuTrialMonster.cs`, `VakuuFightEncounter.cs`, `VakuuFightPatch.cs`, `VakuuFightService.CombatState.cs`, `VakuuFightRunHook.cs`; resources: `EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn`, `EZMicroBalance/images/encounters/`, `EZMicroBalance/images/monsters/`; tests: `VakuuTemptationGuardTests`, `VakuuLothaSaveRiskGuardTests`, `AncientUiReadinessGuardTests`. | Source/package guarded; live victory return, no-black-screen proof, active-fight/pre-finished save-load, active-fight save/load behavior, death/failure path, and co-op disposition are still blocking. |
| Ancient rewards visible to players | Source: `AncientRewardRelicService` plus Urda/Morvi/Lotha/Vakuu option relic flows; package README and handoff say selected Ancient rewards grant visible marker relics; tests cover option/relic text and package handoff. | Source/package guarded; user still needs to confirm relic-bar visibility and hover readability in live runs. |
| Player text and tooltip polish | Active EN/zhs localization under `EZMicroBalance/localization/**`; guards reject stale development terms, old Cook wording, raw tokens, mojibake fragments, and mismatched option/relic descriptions. | Static-validated; live UI fit, tooltip readability, and hover behavior remain manual rows. |
| UI and art resource routing | Event backgrounds, map/run-history icons, option/relic icons, card portraits, power art, and Vakuu encounter art have separate paths in source/export/manifest; art audit and UI readiness guards cover dimensions, hashes, and export paths. | Source/package guarded; clicked Ancient screenshots and combat-scene screenshots remain required before visual closure. |
| Automation and package parity | `dotnet build`, normal `dotnet test`, `dotnet format --verify-no-changes`, `git diff --check`, opt-in artifact tests, and `scripts/check-installed-ezmb-package.ps1` passed after the current package hash refresh. | Automated coverage is green for the installed/package artifacts; it does not replace gameplay/manual proof. |
| Documented publish blockers | `PROJECT_STATE.md`, `docs/issues.md`, this checklist, `docs/test-ready-development-goal.md`, and `docs/features/ancient-expansion-v2.2/manual-test-checklist.md` keep live/manual rows open. | Current package is a user-test-ready handoff; publish-proof requires the open manual rows to be completed with evidence. |

## Release Hygiene

- [x] Debug probes are removed from active behavior or gated behind an explicit debug flag.
- [x] No original Slay the Spire 2 assets are included in the active `EZMicroBalance` publish package.
- [x] Active `mod_image.png` is original generated art with no text, numbers, logos, or official game assets.
- [x] No large decompiled game code bodies are copied into the active source.
- [x] Author placeholder is replaced for this private beta; `EZMicroBalance.json` author is `wenhuorongbing-netizen`.
- [ ] Worktree is clean.
- [ ] Commit is created.
- [ ] Push to `origin/main` is performed only after explicit user approval.

## Known Issues

- Latest normal Steam-client startup/log verification is historical for the pre-review Spire Plus display-name package under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` with exactly BaseLib and `EZMicroBalance` loaded, `Registered config for mod EZMicroBalance`, `Found 22 SavedSpireFields`, `Time to main menu: 13,539ms`, and 0 `ERROR` / release-blocking signature hits. The run restored settings, 24 moved mods, and 2 current-run files, and left 0 `SlayTheSpire2` processes. current source defines 25 SavedSpireFields after the 2026-05-17 static fixes, so fresh live loader parity remains pending. The repeat 2026-05-13 helper-driven startup/log pass at `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` is now historical field-count evidence for the earlier 16-field package state. BaseLib-only plug-off startup/log evidence at `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` loaded only BaseLib and did not initialize Spire Plus / `EZMicroBalance`; the earlier settings-only disabled attempt at `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-142835` is invalid because Spire Plus still initialized. Refreshed normal Steam-client Mod Settings UI evidence at `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` shows `Spire Plus` in the Mods list; RC1 normal Steam-client Mod Settings UI verification remains historical evidence for the old EZ Micro Balance display name.
- Manual feature results are pending; `docs/features/ancients-rework-v4/manual-verification-matrix.md`, `docs/features/ancient-expansion-urda/manual-test-checklist.md`, and `docs/features/ascension-11-20/manual-test-checklist.md` remain the current manual surfaces.
- A11 source now inserts a reachable optional route node in the new column and adds Act 1/2/3 route rows, while ordinary A11 route nodes no longer receive a dedicated marker or hover tooltip. The existing A11 live save has a saved-map graph proof from the post-load first-node coord to the boss; natural click-by-click traversal remains pending.
- Host multiplayer A20 development selection logs an explicit downgrade warning. This is not live co-op support for Dual King Brands; A20 co-op boss-path behavior remains pending manual verification.
- The misleading multiplayer "game version differs" popup can also mean the vanilla `ModelDb` hash check failed after the visible game version matched. The current package logs host/local version, ModelDb hash, and gameplay-relevant mod-list differences before vanilla disconnects; it does not bypass the hash check.
- Urda source behavior is packaged, but live selection, reward-screen timing, room-entry rewards, act-transition cleanup, save/load, UI, and co-op behavior remain pending.
- Latest source hardening also filters Urda/Morvi player loops to active hook players and recovers deck-mirrored state only from owned, non-removed cards. This source pass is build/publish/package-refreshed, and release-artifact tests pass; live verification remains pending.
- The 2026-05-13 A14 Rootblight art-hover probe found pre-fix missing Urda vanilla-derived asset paths before combat. The Urda custom Ancient asset-path fix is packaged and the installed PCK resolves the custom Urda scene/icon in headless Godot, but post-fix live Urda and Rootblight visual/gameplay checks remain pending.
- Forge Token no longer wraps special rest-site options; live A12 rest/Smith regression testing is still needed before closing that issue.
- Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools, no-pool/no-model-modification rewards, elites, bosses, and events; on every second standard reward every visible reward option becomes off-color. If the reward banner hint cannot be updated, `godot.log` should contain a `PrismaticGem reward-screen hint fallback` diagnostic and testers should use the relic hover count plus visible off-color cards as fallback evidence.

## Unsupported Cases

- Enabling legacy `EzDailyContent` and `EZMicroBalance` together is unsupported.
- Other mods that alter card rewards, card pools, rest-site options, Ancient rewards, maps, or Ascension selectors are not compatibility-tested.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate. The selector patch touches only standard single-player and host-multiplayer lobby selection/start paths, temporarily raises the local single-player run-start max only while launching A11-A20, temporarily expands multiplayer lobby unlock caps only during max recomputation, and skips A11-A20 preferred-progress writes. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison; set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips. A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available and gives enhanced treasure nodes an extra Uncommon relic reward. A19/A20 Boss map points now have Royal Seal / King Brand hover text. A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss, adds Boss 2 Brand metadata/parameters, restores 25% missing HP after Boss 1, adds one Boss card reward before Boss 2, and updates the Boss 1 reward screen header/proceed wording for the inter-boss pause. A20 inserts a fixed courtyard event between Boss 1 rewards and Boss 2. A bespoke full-screen intermission remains unimplemented pending live verification needs.
- Ascension 21-30 and custom-character content are not included.
- Former root `art_pipeline/` and `asset/` generated art/calibration folders are archived under ignored `.tools/archive/local-art-and-calibration-20260515/` and are not part of the active publish package.
