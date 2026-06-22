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

Current status note (2026-06-22): the active manifest is `v0.1.0-private-beta.114` after the Sovereign Blade hover ModPatcher migration and package refresh pass.
The beta.88 package/hash rows and direct `v0.107.1` AdditiveBatch1 smoke are previous-package context context only. The beta.90 rows are previous RitsuLib-only package context.
The beta.114 package/hash rows, runtime preflight, source-workspace
validation are current package evidence; clicked Ancient UI smoke is previous beta.108 evidence. The previous beta.108 smoke is
captured under `.tools\runtime-evidence\monkey-stability-beta108-20260622-172312`;
it covered Urda, Morvi, Lotha, and normal Vakuu with 4 / 4 iterations, clean
audits, exact package/game/Ritsu markers, and packet verification 1621 / 0. The
beta.99 settings proof is previous-package context captured under
`.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`;
it rendered the Spire Plus RitsuLib settings page, retained same-session
`godot.log`, audited clean, and passed StS1 Off runtime shape verification
21 / 0. The beta.99 Off proof is previous-package context captured under
`.tools\runtime-evidence\v01071-beta99-ritsulib0432-off-direct-20260621-234221`;
it reached main menu with exactly STS2-RitsuLib and Spire Plus loaded, clean
audit, 25/25 Spire Plus patches, StS1Events disabled with 0 registration lines,
Off verifier 21 / 0, and packet verifier 43 / 0. The beta.96 RitsuLib Mod
Settings clicked UI row and beta.96 RitsuLib-only Off loader smoke are
previous-package context after the beta.99 resource refresh. The beta.93
RitsuLib-only AdditiveBatch1 registration smoke is previous-package loader
evidence only.
Gameplay, save-load, current enabled-mode proof, co-op, and independent QA evidence are still required before any live-ready or release-ready claim.

- [x] `dotnet build` succeeds.
- [x] Latest source build check passed with `dotnet build` after the beta.114 Sovereign Blade hover ModPatcher migration and package refresh pass.
- [x] Publish/package/hash refresh has been rerun for the latest source/text/resource slices with a rebuilt `SpirePlus` private-beta zip.
- [x] `dotnet publish` succeeds.
- [x] Published `EZMicroBalance.json` exists.
- [x] Published `EZMicroBalance.dll` exists.
- [x] Published `EZMicroBalance.pck` exists.
- [x] Manifest declares structured `STS2-RitsuLib` dependency with `min_version: 0.4.34`.
- [x] Manifest has `affects_gameplay: true`.
- [x] PCK audit packages only `EZMicroBalance` installable resources and excludes C# source, docs, art, asset, and archive folders.
- [x] Normal source/localization/documentation guard tests do not require ignored publish/package artifacts.
- [x] Release artifact tests are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` after publish and package refresh. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- [x] Release artifact, installed DLL/PCK, package hash, and runtime-evidence guard tests pass after the latest package refresh with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [x] `publish/SpirePlus-v0.1.0-private-beta.114.zip` is rebuilt from the beta.114 Sovereign Blade hover ModPatcher migration and package refresh pass and copied to the local game root for manual testing.
- Current package hashes: DLL `F5176518FA25ED456D48C4FD3E5BC82C314D25793AA018D8E35F035FB5F32294`; manifest `C9AEB95E89C41E3CF99FB1C5FE5BA7B83B50E156BCA733680A671AA7225BD2DE`; PCK `6A0E4B94F23F4C3AF72B82ADF56113F519FFA14F944FCA6474C42A55D8315569`; ZIP-entry `README_INSTALL.txt` `CBEC19723F1A168B68882CE2072B00B9F264F9CF61FBB1D8036BDF31BB2C5F00`; zip `5CF6DA713066D91BF84D1AE019F30047D159CD3FA7F22F35AE42F9EAD9B86003`.
  - This hash refresh records automated source/package validation plus smoke-level clicked Ancient UI proof. Live gameplay, save-load, natural A11 route-click traversal, failure/death-path, gated Vakuu fight-option UI, and co-op verification remain pending.
  - Detailed pass history lives in `docs/review.md` and `docs/archive/**`.

## Runtime

- [x] STS2-RitsuLib appears in Mod Settings for the beta.99 RitsuLib-only package; this is previous-package context after beta.114 because settings code/resources did not change in the beta.108 pass.
- [x] STS2-RitsuLib loads when enabled in a controlled Off smoke profile for the beta.108 RitsuLib-only package.
- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.
- [x] Historical refreshed Mod Settings UI list screenshot shows `Spire Plus` after the display-name refresh package is installed.
- [x] Previous beta.99 Mod Settings list plus Spire Plus config page screenshots are captured under release-evidence row `mod-settings-current-display`.
  - Evidence root: `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`.
  - It shows `RitsuLib` and `Spire Plus` in the RitsuLib Mods tree, renders Migration Status, `STS2-RitsuLib >= 0.4.34`, evidence-boundary, technical-id, and Preview Tools controls, retains same-session `godot.log`, audits clean, and passes StS1 Off runtime shape verification 21 / 0.
- [x] Historical pre-display-name-refresh Mod Settings evidence exists for the same technical manifest id.
- [x] Previous beta.108 RitsuLib-only clicked Ancient UI smoke is retained as previous-package smoke-level UI proof after the latest RitsuLib package refresh.
  - Evidence root: `.tools\runtime-evidence\monkey-stability-beta108-20260622-172312`.
  - It proves smoke-level UI navigation/Ancient commands only; gameplay and save-load remain pending.
  - Previous beta.99 Off proof remains previous-package context at `.tools\runtime-evidence\v01071-beta99-ritsulib0432-off-direct-20260621-234221`.
  - Previous beta.96 proof remains previous-package context at `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056`.
  - Historical beta.87 proof remains previous-game-version context only.
- [x] Previous RitsuLib-only AdditiveBatch1 registration smoke for the beta.93 ZIP hash is captured.
  - Evidence root: `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`.
  - It registered 10 event types through 14 calls, audited clean, passed verifier 31 / 0, and passed packet 61 / 0.
- [x] Historical RitsuLib diagnostic loader gates exist for Off, CanaryOnly, and AdditiveBatch1 modes with clean audits and 25/34 Spire Plus ModPatcher patches; beta.85 Off/CanaryOnly and beta.86/beta.87 AdditiveBatch1 are previous-package/game-version loader proof, while beta.88 AdditiveBatch1 proof belongs to the previous-package context.
- [x] Historical normal Steam-client startup/log verification under `.tools\runtime-evidence\beta17-loader-smoke-20260525-194311` remains beta.17 context for the same 30-field source family; beta.13 loader/startup evidence remains older historical context.
- [x] Latest retained clicked Ancient UI smoke is beta.108 previous-package smoke-level UI proof on Slay the Spire 2 `v0.107.1`; retained beta.87/beta.88/beta.90/beta.96/beta.99 loader evidence is historical or previous-package context only.
- [x] Historical normal Steam helper startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with only previous package and Spire Plus, reported `Found 22 previous saved-state registrations`, restored settings, 24 moved mod entries, and 2 current-run files, left 0 `SlayTheSpire2` processes, and audited clean. This is historical context; beta.108 clicked UI smoke supersedes it for current startup/default-Off smoke shape.
- [x] Historical repeat helper-driven normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` reached main menu with only previous package and Spire Plus, reported `Found 16 previous saved-state registrations`, restored settings plus 24 moved mod entries, and audited clean. This is previous-package context only.
- [x] Historical previous package-only plug-off normal Steam startup/log verification under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` reached main menu with the Spire Plus technical folder temporarily isolated out of the mods folder, loaded `1 mods (1 total)`, initialized previous package only, did not initialize Spire Plus, restored settings plus 25 moved entries and the current-run save, and audited clean. Current startup shape is covered by beta.108 clicked UI smoke; actual disable-mod gameplay remains pending.
- [x] `godot.log` reviewed for controlled smoke-test initializer errors.
- [x] `godot.log` reviewed after fresh beta.99 RitsuLib-only Off isolated startup/log verification.
- [x] `godot.log` reviewed after previous beta.96 RitsuLib-only Off isolated startup/log verification and previous beta.93 AdditiveBatch1 registration verification.
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
Mod Settings current-display rows, save/load rows, Vakuu victory/failure rows, Rootblight/A11/disable-mod rows, and co-op disposition.
Row `RequiredFiles` may add files; they cannot remove the defaults.
Unknown or blank manifest rows are ignored but reported as warnings.
Deferred rows fail unless rerun with `-AllowDeferred` after an explicit owner-approved release note.
`-WritePassMarker` writes `release-evidence-verifier-pass.json` only after the verifier exits 0.

| Requirement | Current artifacts / evidence | Status |
| --- | --- | --- |
| Dedicated Vakuu combat loop | Source: `EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuTrialMonster.cs`, `VakuuFightEncounter.cs`, `VakuuFightPatch.cs`, `VakuuFightService.StolenVault.cs`, `VakuuFightService.LockBreaks.cs`, `VakuuFightService.Contracts.cs`, `VakuuFightService.BloodDebt.cs`, `VakuuFightRunHook.cs`, `VakuuFightCombatHook.cs`, `VakuuContractService.cs`; resources: `EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn`, `EZMicroBalance/images/encounters/`, `EZMicroBalance/images/monsters/`; tests: `VakuuTemptationGuardTests`, `VakuuLothaSaveRiskGuardTests`, `AncientUiReadinessGuardTests`. | Source/package guarded; live victory return, no-black-screen proof, active-fight/pre-finished save-load, active-fight save/load behavior, death/failure path, and co-op disposition are still blocking. |
| Ancient rewards visible to players | Source: `AncientRewardRelicService` plus Urda/Morvi/Lotha/Vakuu option relic flows; package README and handoff say selected Ancient rewards grant visible marker relics; tests cover option/relic text and package handoff. | Source/package guarded; user still needs to confirm relic-bar visibility and hover readability in live runs. |
| Player text and tooltip polish | Active EN/zhs localization under `EZMicroBalance/localization/**`; guards reject stale development terms, old Cook wording, raw tokens, mojibake fragments, and mismatched option/relic descriptions. | Static-validated; live UI fit, tooltip readability, and hover behavior remain manual rows. |
| UI and art resource routing | Event backgrounds, map/run-history icons, option/relic icons, card portraits, power art, and Vakuu encounter art have separate paths in source/export/manifest; art audit and UI readiness guards cover dimensions, hashes, and export paths. | Source/package guarded; clicked Ancient screenshots and combat-scene screenshots remain required before visual closure. |
| Automation and package parity | `dotnet build`, normal `dotnet test`, `dotnet format --verify-no-changes`, `git diff --check`, opt-in artifact tests, and `scripts/check-installed-spire-plus-package.ps1` passed after the current package hash refresh. The installed DLL/manifest/PCK/README and copied game-root ZIP match the handoff hashes. | Automated coverage passed for the installed/package artifacts; it does not replace gameplay/manual proof. |
| Documented publish blockers | `PROJECT_STATE.md`, `docs/issues.md`, this checklist, `docs/test-ready-development-goal.md`, and `docs/features/ancient-expansion-v2.2/manual-test-checklist.md` keep live/manual rows open. | Current package is a user-test-ready handoff; publish-proof requires the open manual rows to be completed with evidence. |

## Release Hygiene

- [x] Debug probes are removed from active behavior or gated behind an explicit debug flag: broad info diagnostics require `SPIREPLUS_ENABLE_DEBUG_LOGS=1` or legacy `EZMB_ENABLE_DEBUG_LOGS=1`, while preview diagnostics use the localized preview diagnostics setting.
- [x] No original Slay the Spire 2 assets are included in the active `EZMicroBalance` publish package.
- [x] Active `mod_image.png` is original generated art with no text, numbers, logos, or official game assets.
- [x] No large decompiled game code bodies are copied into the active source.
- [x] Author placeholder is replaced for this private beta; `EZMicroBalance.json` author is `wenhuorongbing-netizen`.
- [ ] Worktree is clean.
- [ ] Commit is created.
- [ ] Push to `origin` is performed after validation, packaging, and an intentional commit.

## Known Issues

- Previous beta.96 RitsuLib-only Off proof has been recaptured under `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056` and closed the package-hash loader smoke for that earlier Off-mode surface.
  - The previous proof under `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` loaded previous package, RitsuLib, and `EZMicroBalance`, reported `v0.1.0-private-beta.88`, registered AdditiveBatch1 as 10 event types / 14 calls, reached main menu, audited clean, and passed packet verification with 0 mismatches.
  - Previous beta.93 AdditiveBatch1 registration proof has been recaptured under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`.
  - This is loader/registration evidence, not gameplay proof.
  - The beta.88 row is previous-package context loader/startup evidence, and beta.90 is previous RitsuLib-only package context; neither is beta.93 gameplay proof.
  - The 2026-05-13 helper startup/log pass is historical 16-field evidence. The old previous package-only plug-off evidence is previous package loader-isolation context; beta.96 startup/log shape is previous-package context after beta.99. The earlier settings-only disabled attempt is invalid because Spire Plus still initialized.
  - Previous beta.99 RitsuLib Mod Settings UI proof is captured under `.tools\runtime-evidence\mod-settings-beta99-ritsulib-click-20260621-223210`. It opened Settings -> `Mod Settings (RitsuLib)` in a normal Steam-client session with only `STS2-RitsuLib` and `EZMicroBalance` enabled, showed only `RitsuLib` and `Spire Plus` in the RitsuLib Mods tree, rendered the Spire Plus Migration Status, `STS2-RitsuLib >= 0.4.34` runtime dependency, evidence-boundary, technical-id, and Preview Tools controls, retained same-session `godot.log`, audited clean, and passed StS1 Off runtime shape verification 21 / 0. Treat it as previous-package settings-page context after beta.114; the older `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342\02-mod-config-list.png` screenshot is historical list context only.
- Manual feature results are pending; `docs/features/ancients-rework-v4/manual-verification-matrix.md`, `docs/features/ancient-expansion-urda/manual-test-checklist.md`, and `docs/features/ascension-11-20/manual-test-checklist.md` remain the current manual surfaces.
- A11 source now inserts a reachable optional route node in the new column and adds Act 1/2/3 route rows, while ordinary A11 route nodes no longer receive a dedicated marker or hover tooltip. The existing A11 live save has a saved-map graph proof from the post-load first-node coord to the boss; natural click-by-click traversal remains pending.
- Host multiplayer A20 development selection logs an explicit downgrade warning. This is not live co-op support for Branded Form; A20 co-op boss-path behavior remains pending manual verification.
- The misleading multiplayer "game version differs" popup can also mean the vanilla `ModelDb` hash check failed after the visible game version matched. The current package logs host/local version, ModelDb hash, and gameplay-relevant mod-list differences before vanilla disconnects; it does not bypass the hash check.
- Urda source behavior is packaged, but live selection, reward-screen timing, room-entry rewards, act-transition cleanup, save/load, UI, and co-op behavior remain pending.
- StS1 event prototype remains default-Off for normal users.
Previous beta.96 RitsuLib-only Off proof and previous beta.93 AdditiveBatch1 registration proof have been recaptured.
Off evidence root: `.tools\runtime-evidence\v01071-beta96-ritsulib0431-off-direct-20260621-185056`.
AdditiveBatch1 evidence root: `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`.
Beta.88 proof is previous-package context only.
Before any simple-batch gameplay or handoff claim, AdditiveBatch1 gameplay screenshots/logs must be captured separately; before any canary gameplay claim, the retained CanaryOnly proof must stay tied to the current package/source shape and gameplay screenshots/logs must be captured separately.
  Use `docs/features/sts1-events/v19-gate-evidence-map.md` and `docs/features/sts1-events/v19-gate-ledger.csv`, guarded by `scripts/check-sts1-v19-gate-ledger.ps1`, for the current O0-O76 gate split, plus `docs/features/sts1-events/v20-final-gate-overlay.csv`, guarded by `scripts/check-sts1-v20-final-gate-overlay.ps1`, for the O76-O84 final documentation/handoff overlay and `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md` for the current v20 hard-stop/next-run start point before any event handoff claim.
  A 2026-06-11 static scan also found 33 source-referenced StS1 result-page localization keys missing from both EN and ZHS; see `docs/features/sts1-events/localization-source-gap-scan-20260611.md` and the static closure order in `docs/features/sts1-events/localization-gap-closure-plan.md`.
Closing only the direct Golden Idol missing key remains a localization unblocker, not gameplay proof or a replacement for verifier reports.
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
