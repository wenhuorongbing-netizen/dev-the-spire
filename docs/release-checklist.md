# Private Beta Release Checklist

Target mod: `Spire Plus`
Target manifest id: `EZMicroBalance`

## Architecture

- [x] The active release surface is one mod: `Spire Plus`.
- [x] Legacy `EzDailyContent` and standalone `EZFuturePeek` root mod surfaces have been removed from the active tree.
- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.
- [x] Custom-character work is not included in this private beta.
- [x] A11-A20 selection is default-on only for single-player standard lobbies. After the 2026-05-25 co-op crash logs, host-multiplayer A11-A20 selection and gameplay fail closed by default unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging. Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Full live Ascension and co-op verification is pending.

## Build And Publish

- [x] `dotnet build` succeeds.
- [x] Latest source/package build check passed with `dotnet build EZMicroBalance.sln` after the beta.27 manifest/localization UTF-8 refresh.
- [x] Publish/package/hash refresh has been rerun for the latest source/text/resource slices with a rebuilt `SpirePlus` private-beta zip.
- [x] `dotnet publish` succeeds.
- [x] Published `EZMicroBalance.json` exists.
- [x] Published `EZMicroBalance.dll` exists.
- [x] Published `EZMicroBalance.pck` exists.
- [x] Manifest declares structured `BaseLib` dependency with `min_version: v3.1.4`.
- [x] Manifest has `affects_gameplay: true`.
- [x] PCK audit packages only `EZMicroBalance` installable resources and excludes C# source, docs, art, asset, and archive folders.
- [x] Normal source/localization/documentation guard tests do not require ignored publish/package artifacts.
- [x] Release artifact tests are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` after publish and package refresh. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- [x] Release artifact, installed DLL/PCK, package hash, and runtime-evidence guard tests pass after the latest package refresh with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [x] `publish/SpirePlus-v0.1.0-private-beta.27.zip` was rebuilt from the current Seedbed strength/planting source and copied to the local game root for manual testing.
  - Current package hashes: DLL `B6F91BC3079CAA342C27690E7DCC056E958CF927D41147EE78207EF717553F04`; manifest `E6E708FF4564A1F288391260D9C8FE53C28503A1B2F565CB24E176EC1D1556B7`; PCK `C62CBC8B9DAC1C4522FA1A30B4BA35B95C53EDF4628FD6B8135F4B9BBD83281A`; README `DCAB3CE1B276CC9093C9AEA86C19D900081E097A7A821F2E0AD6101F113C390D`; zip `891F2ABA04E17D6BE74997D7201D071431A956A070F25E2ACEFEFE9142DF171C`.
  - This hash refresh records automated source/package validation only. Live gameplay, save-load, natural A11 route-click traversal, failure/death-path, clicked Ancient UI, and co-op verification remain pending.
  - Detailed pass history lives in `docs/review.md` and `docs/archive/**`.

## Runtime

- [x] BaseLib appears in Mod Settings.
- [x] BaseLib loads when enabled in a controlled smoke profile.
- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.
- [x] Spire Plus appears in a refreshed Mod Settings UI screenshot after the display-name refresh package is installed.
- [x] Historical pre-display-name-refresh Mod Settings evidence exists for the same technical manifest id.
- [ ] Fresh loader smoke for the current beta.27 ZIP hash is pending. The beta.19 smoke under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` remains historical startup evidence only.
- [x] Historical normal Steam-client startup/log verification under `.tools\runtime-evidence\beta17-loader-smoke-20260525-194311` remains beta.17 context for the same 30-field source family; beta.13 loader/startup evidence remains older historical context.
- [ ] Latest loader smoke for the current beta.27 package hash has not been recaptured yet.
- [x] Historical normal Steam helper startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with only BaseLib and Spire Plus, reported `Found 22 SavedSpireFields`, restored settings, 24 moved mod entries, and 2 current-run files, left 0 `SlayTheSpire2` processes, and audited clean. This is historical context now superseded for runtime binaries by the 30-field loader smoke.
- [x] Repeat helper-driven normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` reached main menu with only BaseLib and Spire Plus, reported `Found 16 SavedSpireFields`, restored settings plus 24 moved mod entries, and audited clean.
- [x] BaseLib-only plug-off normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` reached main menu with the Spire Plus technical folder temporarily isolated out of the mods folder, loaded `1 mods (1 total)`, initialized BaseLib only, did not initialize Spire Plus, restored settings plus 25 moved entries and the current-run save, and audited clean.
- [x] `godot.log` reviewed for controlled smoke-test initializer errors.
- [ ] `godot.log` reviewed after fresh beta.27 normal Steam-client isolated startup/log verification.
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
.\scripts\collect-release-evidence.ps1 -NoLaunch
.\scripts\verify-spire-plus-release-evidence.ps1 -WritePassMarker
```

The collector creates the verifier-readable manifest scaffold.
The verifier hashes the package under test and requires current package hash parity,
manifest/evidence dirs/files inside the declared evidence root, matching row kinds,
`command.txt` for every passed row, clicked-UI screenshots with foreground preflight,
clean `godot-log-audit.json` files, result notes, fresh loader rows, preview-tools rows,
save/load rows, Vakuu victory/failure rows, Rootblight/A11/disable-mod rows, and co-op disposition.
Row `RequiredFiles` may add files; they cannot remove the defaults.
Unknown or blank manifest rows are ignored but reported as warnings.
Deferred rows fail unless rerun with `-AllowDeferred` after an explicit owner-approved release note.
`-WritePassMarker` writes `release-evidence-verifier-pass.json` only after the verifier exits 0.

| Requirement | Current artifacts / evidence | Status |
| --- | --- | --- |
| Dedicated Vakuu combat loop | Source: `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuTrialMonster.cs`, `VakuuFightEncounter.cs`, `VakuuFightPatch.cs`, `VakuuFightService.CombatState.cs`, `VakuuFightRunHook.cs`; resources: `EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn`, `EZMicroBalance/images/encounters/`, `EZMicroBalance/images/monsters/`; tests: `VakuuTemptationGuardTests`, `VakuuLothaSaveRiskGuardTests`, `AncientUiReadinessGuardTests`. | Source/package guarded; live victory return, no-black-screen proof, active-fight/pre-finished save-load, active-fight save/load behavior, death/failure path, and co-op disposition are still blocking. |
| Ancient rewards visible to players | Source: `AncientRewardRelicService` plus Urda/Morvi/Lotha/Vakuu option relic flows; package README and handoff say selected Ancient rewards grant visible marker relics; tests cover option/relic text and package handoff. | Source/package guarded; user still needs to confirm relic-bar visibility and hover readability in live runs. |
| Player text and tooltip polish | Active EN/zhs localization under `EZMicroBalance/localization/**`; guards reject stale development terms, old Cook wording, raw tokens, mojibake fragments, and mismatched option/relic descriptions. | Static-validated; live UI fit, tooltip readability, and hover behavior remain manual rows. |
| UI and art resource routing | Event backgrounds, map/run-history icons, option/relic icons, card portraits, power art, and Vakuu encounter art have separate paths in source/export/manifest; art audit and UI readiness guards cover dimensions, hashes, and export paths. | Source/package guarded; clicked Ancient screenshots and combat-scene screenshots remain required before visual closure. |
| Automation and package parity | `dotnet build`, normal `dotnet test`, `dotnet format --verify-no-changes`, `git diff --check`, opt-in artifact tests, and `scripts/check-installed-spire-plus-package.ps1` passed after the current package hash refresh. The installed DLL/manifest/PCK/README and copied game-root ZIP match the handoff hashes. | Automated coverage is green for the installed/package artifacts; it does not replace gameplay/manual proof. |
| Documented publish blockers | `PROJECT_STATE.md`, `docs/issues.md`, this checklist, `docs/test-ready-development-goal.md`, and `docs/features/ancient-expansion-v2.2/manual-test-checklist.md` keep live/manual rows open. | Current package is a user-test-ready handoff; publish-proof requires the open manual rows to be completed with evidence. |

## Release Hygiene

- [x] Debug probes are removed from active behavior or gated behind an explicit debug flag.
- [x] No original Slay the Spire 2 assets are included in the active `EZMicroBalance` publish package.
- [x] Active `mod_image.png` is original generated art with no text, numbers, logos, or official game assets.
- [x] No large decompiled game code bodies are copied into the active source.
- [x] Author placeholder is replaced for this private beta; `EZMicroBalance.json` author is `wenhuorongbing-netizen`.
- [ ] Worktree is clean.
- [ ] Commit is created.
- [ ] Push to `origin` is performed after validation, packaging, and an intentional commit.

## Known Issues

- Fresh loader smoke for the current beta.27 package hash is pending. The beta.19 smoke under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` loaded exactly BaseLib and `EZMicroBalance`, registered config, reported `v0.1.0-private-beta.19`, `Found 30 SavedSpireFields`, reached startup completion, and had 0 release-blocking hits. The helper stopped the game and restored 24 isolated mod entries. This is loader/startup evidence, not gameplay proof; it remains historical context for beta.27.
  - The 2026-05-13 helper startup/log pass is historical 16-field evidence. BaseLib-only plug-off evidence loaded only BaseLib; the earlier settings-only disabled attempt is invalid because Spire Plus still initialized.
  - Refreshed normal Steam-client Mod Settings UI evidence at `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` shows `Spire Plus` in the Mods list. Earlier page-level Mod Settings evidence predates the display-name refresh.
- Manual feature results are pending; `docs/features/ancients-rework-v4/manual-verification-matrix.md`, `docs/features/ancient-expansion-urda/manual-test-checklist.md`, and `docs/features/ascension-11-20/manual-test-checklist.md` remain the current manual surfaces.
- A11 source now inserts a reachable optional route node in the new column and adds Act 1/2/3 route rows, while ordinary A11 route nodes no longer receive a dedicated marker or hover tooltip. The existing A11 live save has a saved-map graph proof from the post-load first-node coord to the boss; natural click-by-click traversal remains pending.
- Host multiplayer A20 development selection logs an explicit downgrade warning. This is not live co-op support for Branded Form; A20 co-op boss-path behavior remains pending manual verification.
- The misleading multiplayer "game version differs" popup can also mean the vanilla `ModelDb` hash check failed after the visible game version matched. The current package logs host/local version, ModelDb hash, and gameplay-relevant mod-list differences before vanilla disconnects; it does not bypass the hash check.
- Urda source behavior is packaged, but live selection, reward-screen timing, room-entry rewards, act-transition cleanup, save/load, UI, and co-op behavior remain pending.
- Latest source hardening also filters Urda/Morvi player loops to active hook players and recovers deck-mirrored state only from owned, non-removed cards. This source pass is build/publish/package-refreshed, and release-artifact tests pass; live verification remains pending.
- The 2026-05-13 A14 Rootblight art-hover probe found pre-fix missing Urda vanilla-derived asset paths before combat. The Urda custom Ancient asset-path fix is packaged and the installed PCK resolves the custom Urda scene/icon in headless Godot, but post-fix live Urda and Rootblight visual/gameplay checks remain pending.
- Forge Token no longer wraps special rest-site options; live A12 rest/Smith regression testing is still needed before closing that issue.
- Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools, no-pool/no-model-modification rewards, elites, bosses, and events; on every second standard reward every visible reward option becomes off-color. If the reward banner hint cannot be updated, `godot.log` should contain a `PrismaticGem reward-screen hint fallback` diagnostic and testers should use the relic hover count plus visible off-color cards as fallback evidence.

## Unsupported Cases

- Enabling legacy `EzDailyContent` and `EZMicroBalance` together is unsupported.
- Other mods that alter card rewards, card pools, rest-site options, Ancient rewards, maps, or Ascension selectors are not compatibility-tested.
- A11-A20 selection is default-on only for single-player standard lobbies. Host-multiplayer selection/gameplay fails closed by default after the 2026-05-25 co-op crash logs unless `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1` is deliberately set for two-client debugging. The selector patch touches standard lobby selection/start paths, temporarily raises the local single-player run-start max only while launching A11-A20, and skips A11-A20 preferred-progress writes. Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison; set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.
- A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available and gives enhanced treasure nodes an extra Uncommon relic reward.
- A19/A20 Boss map points now have dedicated ability / Branded Form hover text.
- A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss, adds Boss 2 Branded Form metadata/parameters, restores 25% missing HP after Boss 1, adds one Boss card reward before Boss 2, and updates the Boss 1 reward screen header/proceed wording for the inter-boss pause. A20 inserts a fixed courtyard event between Boss 1 rewards and Boss 2.
- A bespoke full-screen intermission remains unimplemented pending live verification needs.
- Ascension 21-30 and custom-character content are not included.
- Former root `art_pipeline/` and `asset/` generated art/calibration folders are archived under ignored `.tools/archive/local-art-and-calibration-20260515/` and are not part of the active publish package.
