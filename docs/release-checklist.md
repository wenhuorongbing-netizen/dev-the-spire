# Private Beta Release Checklist

Target mod: `EZ Micro Balance`
Target manifest id: `EZMicroBalance`

## Architecture

- [x] Existing `EzDailyContent` manifest id remains unchanged.
- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.
- [x] Enabling `EZMicroBalance` does not require enabling legacy `EzDailyContent`.
- [x] Custom-character work is not included in this private beta.
- [x] A11-A20 selection is now default-on in this private-beta multiplayer test candidate for single-player and host-multiplayer standard lobbies. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Host multiplayer A20 selection/start logs a downgrade warning because Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification. Full live Ascension and co-op verification is pending.

## Build And Publish

- [x] `dotnet build` succeeds.
- [x] `dotnet publish` succeeds.
- [x] Published `EZMicroBalance.json` exists.
- [x] Published `EZMicroBalance.dll` exists.
- [x] Published `EZMicroBalance.pck` exists.
- [x] Manifest declares structured `BaseLib` dependency with `min_version: v3.1.2`.
- [x] Manifest has `affects_gameplay: true`.
- [x] PCK audit excludes legacy `EzDailyContent`, C# source, docs, art, asset, and archive folders.
- [x] Normal source/localization/documentation guard tests do not require ignored publish/package artifacts.
- [x] Release artifact tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` after publish and package refresh.
- [x] Release artifact, installed DLL/PCK, package hash, and runtime-smoke evidence tests have been rerun for the current tree with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [x] `publish/EZMicroBalance-v0.1.0-private-beta.0.zip` is rebuilt from the current installed artifacts. Current zip SHA256 `BE05559B4EA1180FB88129235A980978B1E2498187F1CB665882EC7DCC1CD314`.

## Runtime

- [x] BaseLib appears in Mod Settings.
- [x] BaseLib loads when enabled in a controlled smoke profile.
- [x] EZ Micro Balance appears in Mod Settings.
- [x] EZ Micro Balance loads when enabled in a controlled smoke profile.
- [x] Game reaches main menu with only BaseLib and EZ Micro Balance enabled in the controlled smoke profile.
- [x] `godot.log` reviewed for controlled smoke-test initializer errors.
- [ ] `godot.log` reviewed after normal Steam-client manual verification.

## Content Verification

- [x] Every implemented Ancient reward change has a manual checklist row.
- [ ] Every implemented Ancient reward change has a completed manual runtime result.
- [ ] Save/load-sensitive behavior is tested.
- [x] Disable-mod loader behavior is tested.
- [ ] Disable-mod gameplay behavior is tested in a run.
- [x] English localization parses and matches implemented behavior by text review.
- [x] Simplified Chinese localization is valid UTF-8 JSON and matches implemented behavior by text review.

## Release Hygiene

- [x] Debug probes are removed from active release behavior or gated behind an explicit debug flag.
- [x] No original Slay the Spire 2 assets are included in the active `EZMicroBalance` publish package.
- [x] Active `mod_image.png` is original generated art with no text, numbers, logos, or official game assets.
- [x] No large decompiled game code bodies are copied into the active release source.
- [x] Stale setup-era docs are corrected for the active release target.
- [x] Historical research docs are archived if still useful.
- [x] Known issues and unsupported cases are documented.
- [ ] Author placeholder is replaced or explicitly accepted for this private beta.
- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.
- [ ] Worktree is clean.
- [ ] Commit is created.
- [ ] Push to `origin/main` is performed only after explicit user approval.

## Known Issues

- **Superseded compatibility warning (2026-05-08):** The earlier v0.105.0 live log was collected with 17 mods and BaseLib `v3.1.0`, and showed `Creature.get_ShowsInfiniteHp` / BaseLib patch failures. Current runtime dependency is BaseLib `v3.1.2`; the latest controlled BaseLib+EZMB-only smoke has no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures. Normal Steam-client Mod Settings now has RC1 evidence, normal Steam-client A0/A10/A20 DevConsole combat smoke passed, Act 1 A11 map/save-load passed, and Act 2/3 A11 map-surface observation passed; full manual feature verification is still pending.
- User-reported live baseline after the BaseLib update: single-player A0/A10/A20 and boss/basic combats pass. This is complemented by Codex-observed A0/A10/A20 combat smoke and A11 Act 1/2/3 map evidence, but does not replace the natural route-click Ancient reward, natural A11 traversal/boss-reachability check, full save/load, or co-op matrices.
- RC1 live gates still pending: Rootblight visual feedback, Rootblight card art, broader A11 geometry/traversal diagnostics beyond the Act 1/2/3 width/row spot checks, Ancient reward gameplay spot checks, multiplayer matrix, Steam-client Mod Settings rerun if package/mod state changes, and Ancient/co-op save/load verification.
- Earlier 17-mod logs are not valid release evidence.
- Current player-reported open issues are tracked in `docs/issues.md`; do not claim Ascension readiness until those entries are fixed or explicitly closed with runtime evidence.
- A11 source now inserts a reachable optional route node in the new column and adds Act 1/2/3 route rows, while ordinary A11 route nodes no longer receive a dedicated marker or hover tooltip. RC1 Act 1 normal Steam-client evidence selected A11 through the original UI, rendered the map without an A11 marker, logged `columns=8; rows=17`, saved after the first node, continued the run, and reopened the map after load. RC1 Act 2/3 normal Steam-client DevConsole observation rendered later-act maps without A11 markers and logged Act 2 `columns=8; rows=16` with 1 late row plus Act 3 `columns=8; rows=16` with 2 late rows. Natural traversal and boss reachability remain pending.
- Host multiplayer A20 development selection now logs an explicit downgrade warning. This is not live co-op support for Dual King Brands; A20 co-op boss-path behavior remains pending manual verification.
- Forge Token no longer wraps special rest-site options; the player-reported rest-site crash still needs live A12 rest/Smith regression testing before the issue can be closed.
- A12 tooltip/rich text needs polish: values should use native blue rich text and important game terms/reward nouns should use native gold rich text.
- Independent `EZMicroBalance` controlled runtime load has current `--force-steam off` evidence after the v0.105.0/BaseLib v3.1.2 package refresh: the bounded smoke temporarily enabled only `BaseLib` and `EZMicroBalance`, loaded exactly 2 mods, reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte.
- RC1 normal Steam-client isolated startup log reached main menu through Steam with only BaseLib + EZ Micro Balance discovered and loaded (`Loaded 2 mods (2 total)`), BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialized, `Found 12 SavedSpireFields`, 0 startup `ERROR` lines, and no `Creature.get_ShowsInfiniteHp`, `HealthBarForecastPatch`, `DamageMeter`, `RouteSuggest`, `TypeLoadException`, `MissingMethodException`, or EZMB error/exception signatures. Codex temporarily moved 23 unrelated local mod entries out of the game `mods` directory, copied `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log` at main menu, then restored the moved entries and `settings.save`.
- RC1 normal Steam-client Mod Settings verification passed after adding the no-op EZ Micro Balance BaseLib config page. Earlier screenshots `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-modconfig.png` and `.tools\runtime-evidence\rc1-modsettings-attempt-20260508-092717-mainmenu-loadedmods.png` show BaseLib and the loaded-mod footer. The recheck screenshots `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` show the EZ Micro Balance page as `微平衡` with `无可配置选项。`. The matching log snapshot `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- RC1 A11 Act 1 map/save-load spot check launched through normal Steam with only BaseLib + EZ Micro Balance. Evidence under `.tools\runtime-evidence\rc1-a11-map-save-20260508-110008` shows A11 selected through the original UI, the Act 1 map rendered with no A11-specific marker, `current_run.save` written after the first node, Continue loading the saved run, and the map reopening after load with `columns=8; rows=17`. The live log snapshot has 0 `ERROR` lines and 0 release-blocking signatures.
- RC1 A11 Act 2/3 map-surface observation launched through normal Steam with only BaseLib + EZ Micro Balance. Evidence under `.tools\runtime-evidence\rc1-a11-act23-map-20260508-113355` shows A11 selected through the original UI, Act 2 and Act 3 map surfaces rendered with no A11-specific marker, and `a11-act23-godot-live.log` recording Act 2 `columns=8; rows=16` plus Act 3 `columns=8; rows=16`; the log has 0 `ERROR` lines and 0 release-blocking signatures.
- The earlier Root/Fission/Black Star bugfix pass rebuilt the package from installed artifacts, restored installed/staging/versioned/zip hash parity for DLL/JSON/PCK, and added source guards for Root Bud draw tracking, Fission no-template/no-duplicate-text behavior, stricter Fission eligibility, and Black Star post-obtain compensation.
- Direct automated launches without `--force-steam off` failed before mod loading due Steamworks initialization (`No appID found`, then `ConnectToGlobalUser failed` with a temporary app id). A controlled `--force-steam off` smoke profile loads BaseLib and EZ Micro Balance successfully, and a later normal Steam-client Mod Settings pass covers the UI gate. Live gameplay/manual verification is still required.
- Manual feature results are pending; `docs/features/ancients-rework-v4/manual-verification-matrix.md` is prepared but not executed. The v4.3 rows include Velvet Choker's retained soft-limit counting plus card-library canonical-card regression, Distinguished Cape `lose 30% of current Max HP, at least 18` trade gate with same-pool Vakuu replacement when unaffordable, Prismatic Gem "Every second standard card reward contains only off-color cards" plus reward-screen hint fallback diagnostics, and zhs numeric formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate. A20 host multiplayer selection/start logs that multiplayer A20 selection is for development testing and that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification. A12/A13/A16/A19 now have distinct Firemark/Fission/Banner/Royal Seal indicators or text surfaces and stricter source guards, but Rootblight/Blight Sprout, Firemarked Elite/Forge Token, Fission reward pickup, Boss Royal Seals/Brands, Ancient save/load, card registration/visuals, natural A11 route traversal/boss reachability, and live co-op behavior are not fully verified.
- The private-beta zip contains only `EZMicroBalance.dll`, `EZMicroBalance.json`, `EZMicroBalance.pck`, and `README_INSTALL.txt` under `EZMicroBalance/`; installed, staging, and versioned package hashes match for DLL/JSON/PCK. `README_INSTALL.txt` now also states the controlled-smoke status, passed Mod Settings status, pending manual gameplay gates, and Ascension development status. Local `source code/` scratch material is ignored and excluded from export/package surfaces.
- The latest bounded smoke log includes unrelated local invalid-manifest errors for `RouteSuggestConfig.json` and `sts2-heybox-support`. Only `BaseLib` and `EZMicroBalance` initialized, `Loaded 2 mods (19 total)` was logged, `Found 12 SavedSpireFields` was reported, the game reached main menu in `13,628ms`, and no EZ Micro Balance startup exception or error was present. This remains a controlled smoke only; it does not complete live gameplay verification.
- Old local `D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent` artifacts may still exist from earlier setup. Disable or remove that legacy mod while testing `EZMicroBalance` to avoid duplicate Ancient patches.
- Godot headless export prints a non-fatal `sts2` assembly load exception while scanning C# scripts; publish still completes and the PCK audit passes.
- `AUTHOR_NAME_REPLACE_ME` remains the manifest author placeholder unless accepted for private beta or replaced before release.
- The old root-level BaseLib folder remains at `D:\Steam\steamapps\common\Slay the Spire 2\BaseLib`; the active runtime dependency is `mods\BaseLib` `v3.1.2`.

## Unsupported Cases

- Enabling legacy `EzDailyContent` and `EZMicroBalance` together is unsupported.
- Other mods that alter card rewards, card pools, rest-site options, or Ancient rewards are not compatibility-tested.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate. The selector patch touches only standard single-player and host-multiplayer lobby selection/start paths, temporarily raises the local single-player run-start max only while launching A11-A20, temporarily expands multiplayer lobby unlock caps only during max recomputation, and skips A11-A20 preferred-progress writes; it does not patch the global `CharacterStats` getter, `ProgressState`, `ProgressSaveManager`, `NAscensionPanel`, or `AscensionManager.maxAscensionAllowed`. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison; set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips. A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 for single-player runs when safe saved-map geometry is available and gives enhanced treasure nodes an extra Uncommon relic reward. A19/A20 Boss map points now have Royal Seal / King Brand hover text. A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss, adds Boss 2 Brand metadata/parameters, restores 25% missing HP after Boss 1, adds one Boss card reward before Boss 2, and updates the Boss 1 reward screen header/proceed wording for the inter-boss pause. A20 inserts a fixed courtyard event between Boss 1 rewards and Boss 2. A bespoke full-screen intermission remains unimplemented pending live verification needs.
- A12 targets about 3 route-exclusive Firemarked Elites per act when safe nodes exist, with a dedicated map indicator and visible Forge Token status relic. Forge Token spends on Rest/Smith only in this build; special rest-site payout is disabled until a safe runtime API is proven. This still needs live route/UI/save-load/rest verification.
- A13 Fission applies only to eligible Attack/Skill rewards, uses a dedicated icon, and excludes already-exhausting cards. This still needs live reward/reroll/pickup/save-load verification.
- Ascension 21-30 and custom-character content are not included.
- Root-family Ascension cards are registered through the BaseLib custom-card path and guarded against known generated-card paths, but random transform/reward behavior still requires live verification before any live support claim.
- Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools, no-pool/no-model-modification rewards, elites, bosses, and events; on every second standard reward every visible reward option becomes off-color. If the reward banner hint cannot be updated, `godot.log` should contain a `PrismaticGem reward-screen hint fallback` diagnostic and testers should use the relic hover count plus visible off-color cards as fallback evidence.
- Generated art and calibration folders under `art_pipeline/` and `asset/` are not part of the active publish package.
