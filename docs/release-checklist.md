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
- [x] Manifest depends on `BaseLib`.
- [x] Manifest has `affects_gameplay: true`.
- [x] PCK audit excludes legacy `EzDailyContent`, C# source, docs, art, asset, and archive folders.
- [x] Normal source/localization/documentation guard tests do not require ignored publish/package artifacts.
- [x] Release artifact tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` after publish and package refresh.
- [x] Release artifact, installed DLL/PCK, package hash, and runtime-smoke evidence tests have been rerun for the current tree with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- [x] `publish/EZMicroBalance-v0.1.0-private-beta.0.zip` is rebuilt from the current installed artifacts. Current zip SHA256 `C928B50616109FF198405F3990A1F4DA40FA9460E8CC6DFE69CC95784DBEEAE2`.

## Runtime

- [ ] BaseLib appears in Mod Settings.
- [x] BaseLib loads when enabled in a controlled smoke profile.
- [ ] EZ Micro Balance appears in Mod Settings.
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

- **Superseded compatibility warning (2026-05-08):** The earlier v0.105.0 live log was collected with 17 mods and BaseLib `v3.1.0`, and showed `Creature.get_ShowsInfiniteHp` / BaseLib patch failures. Current runtime dependency is BaseLib `v3.1.2`; the latest controlled BaseLib+EZMB-only smoke has no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures. This does not replace normal Steam-client Mod Settings verification or live combat/manual feature verification.
- User-reported live baseline after the BaseLib update: single-player A0/A10/A20 and boss/basic combats pass. This is not a clean Codex-collected normal Steam-client `godot.log`; keep the clean-log and Mod Settings gates open until a clean UI/manual pass is collected.
- RC1 live gates still pending: Rootblight visual feedback, Rootblight card art, A11 geometry diagnostics, clean normal Steam-client `godot.log`, multiplayer matrix, Steam-client Mod Settings, and save/load verification.
- Earlier 17-mod logs are not valid release evidence.
- Current player-reported open issues are tracked in `docs/issues.md`; do not claim Ascension readiness until those entries are fixed or explicitly closed with runtime evidence.
- A11 source now inserts a reachable optional route node in the new column and adds Act 1/2/3 route rows, while ordinary A11 route nodes no longer receive a dedicated marker or hover tooltip. The visual route choice still needs live Act 1/2/3 map verification.
- Host multiplayer A20 development selection now logs an explicit downgrade warning. This is not live co-op support for Dual King Brands; A20 co-op boss-path behavior remains pending manual verification.
- Forge Token no longer wraps special rest-site options; the player-reported rest-site crash still needs live A12 rest/Smith regression testing before the issue can be closed.
- A12 tooltip/rich text needs polish: values should use native blue rich text and important game terms/reward nouns should use native gold rich text.
- Independent `EZMicroBalance` controlled runtime load has current `--force-steam off` evidence after the v0.105.0/BaseLib v3.1.2 package refresh: the bounded smoke temporarily enabled only `BaseLib` and `EZMicroBalance`, loaded exactly 2 mods, reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte. Normal Steam-client Mod Settings verification is still pending.
- RC1 normal Steam-client launch/log probe reached main menu through Steam with `Loaded 2 mods (19 total)`, BaseLib `v3.1.2`, BaseLib `177 patches successfully, 0 failed`, EZ Micro Balance initialized, `Found 12 SavedSpireFields`, and no `Creature.get_ShowsInfiniteHp`, `HealthBarForecastPatch`, `TypeLoadException`, `MissingMethodException`, or EZMB error/exception signatures. It does not close the clean-log gate because the same log still has unrelated local invalid-manifest/dependency `ERROR` lines for discovered disabled mods (`RouteSuggestConfig.json`, `sts2-heybox-support`) and old-style dependency warnings. Mod Settings UI was not opened in this pass.
- normal Steam-client Mod Settings verification is still pending.
- The earlier Root/Fission/Black Star bugfix pass rebuilt the package from installed artifacts, restored installed/staging/versioned/zip hash parity for DLL/JSON/PCK, and added source guards for Root Bud draw tracking, Fission no-template/no-duplicate-text behavior, stricter Fission eligibility, and Black Star post-obtain compensation.
- Direct automated launches without `--force-steam off` failed before mod loading due Steamworks initialization (`No appID found`, then `ConnectToGlobalUser failed` with a temporary app id). A controlled `--force-steam off` smoke profile loads BaseLib and EZ Micro Balance successfully, but launch from the Steam client is still required for final manual verification.
- Manual feature results are pending; `docs/features/ancients-rework-v4/manual-verification-matrix.md` is prepared but not executed. The v4.3 rows include Velvet Choker's retained soft-limit counting plus card-library canonical-card regression, Distinguished Cape `lose 30% of current Max HP, at least 18` trade gate with same-pool Vakuu replacement when unaffordable, Prismatic Gem "Every second standard card reward contains only off-color cards" plus reward-screen hint fallback diagnostics, and zhs numeric formatting. v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate. A20 host multiplayer selection/start logs that multiplayer A20 selection is for development testing and that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification. A12/A13/A16/A19 now have distinct Firemark/Fission/Banner/Royal Seal indicators or text surfaces and stricter source guards, but Rootblight/Blight Sprout, Firemarked Elite/Forge Token, Fission reward pickup, Boss Royal Seals/Brands, save/load, card registration/visuals, map traversal, and live co-op behavior are not fully verified.
- The private-beta zip contains only `EZMicroBalance.dll`, `EZMicroBalance.json`, `EZMicroBalance.pck`, and `README_INSTALL.txt` under `EZMicroBalance/`; installed, staging, and versioned package hashes match for DLL/JSON/PCK. `README_INSTALL.txt` now also states the controlled-smoke status, pending manual gates, and Ascension development status. Local `source code/` scratch material is ignored and excluded from export/package surfaces.
- The latest bounded smoke log includes unrelated local invalid-manifest errors for `RouteSuggestConfig.json` and `sts2-heybox-support`. Only `BaseLib` and `EZMicroBalance` initialized, `Loaded 2 mods (19 total)` was logged, `Found 12 SavedSpireFields` was reported, the game reached main menu in `13,628ms`, and no EZ Micro Balance startup exception or error was present. This remains a controlled smoke only; it does not complete normal Steam-client Mod Settings or live gameplay verification.
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
