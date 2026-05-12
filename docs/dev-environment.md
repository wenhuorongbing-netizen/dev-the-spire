# Development Environment

## Machine
- OS: Windows 11 Pro (`10.0.26200`, `64-bit`)
- Detected date: `2026-05-02`
- Working directory: `D:\Game\FOTN\dev-the-spire`
- Git toplevel: `D:\Game\FOTN`

## Tools
- dotnet SDK: `9.0.313`
- dotnet host/runtime: `9.0.15`
- git: `2.53.0.windows.1`
- Codex: Codex Desktop session (agent workspace mode)
- Godot/MegaDot: `D:\Game\FOTN\dev-the-spire\.tools\godot-4.5.1-mono\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe`
- Godot/MegaDot version: `4.5.1.stable.mono.official.f62fdbde1`

## Game
- Game root: `D:\Steam\steamapps\common\Slay the Spire 2`
- Mod folder: `D:\Steam\steamapps\common\Slay the Spire 2\mods`
- Current branch target: public beta
- Observed installed game version: `v0.105.1` from `D:\Steam\steamapps\common\Slay the Spire 2\release_info.json` on 2026-05-11. The local `source code/` reference snapshot remains `v0.105.0` and must be refreshed before source-backed conclusions about new `v0.105.1` API changes.
- Observed installed game date: `2026.05.07` upstream build date, installed locally on `2026-05-08`
- Live observed in-game version (player report, 2026-05-08): `v0.105.0, 2026.05.08`
- Local source snapshot: `source code/` was refreshed from `D:\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck` on 2026-05-08 using GDRE Tools `v2.5.0-beta.5`; `release_info.json` reports commit `e4579d36`, branch/version `v0.105.0`, and `main_assembly_hash` `359406027`.
- Source recovery log: `source code/gdre_export.log` detected Godot `4.5.1`, bytecode `4.5.0-stable (ebc36a7)`, extracted 15,551 files with no extraction errors, decompiled 3,448 scripts, reported 18 failed scripts, and successfully ran `dotnet build` for the recovered `sts2.sln`.
- BaseLib `v3.1.2` compatibility with v0.105.0: controlled main-menu smoke passed with only BaseLib + EZMicroBalance enabled. The RC1 normal Steam-client isolated startup log reached main menu with only BaseLib + EZMicroBalance discovered and loaded, BaseLib `177 patches successfully, 0 failed`, and no v0.105 removed-API, BaseLib patch-failure, non-EZMB mod, or EZMB exception signatures. The RC1 normal Steam-client Mod Settings recheck also shows the EZ Micro Balance page. The earlier `v3.1.0` / 17-mod live log (`godot2026-05-08T05.06.30.log`) is superseded for loader evidence. Codex-observed A0/A10/A20 combat smoke, the Act 1 A11 map/save-load spot check, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight English/ZHS hover/starter-notice checks have RC1 evidence; full Ancient gameplay, full Rootblight combat behavior, natural A11 traversal/boss reachability, and co-op verification remain pending.
- Do not use pre-2026-05-08 `v0.104.0` source notes as the sole basis for current `v0.105.0` conclusions. Reinspect the refreshed source and confirm runtime behavior before making release claims.
- Test environment must be ONLY BaseLib + EZMicroBalance. Earlier 17-mod logs including DamageMeter, RouteSuggest, AnimeWaifuSilent, etc. are invalid release evidence.


## Project mission
- Active deliverable: `EZ Micro Balance` private beta.
- Scope: Ancient reward rebalance v4.3, independent mod structure, localization, build/publish, manual verification, and gated Ascension 11-20 implementation work when explicitly requested.
- Out of scope this cycle: Ascension 21-30 and custom character implementation.

## Mod
- Legacy scaffold mod name: `Easy Content` in `EzDailyContent.json`
- Legacy manifest id: `EzDailyContent`
- Target private beta mod name: `EZ Micro Balance`
- Target private beta manifest id: `EZMicroBalance`
- Legacy manifest path: `D:\Game\FOTN\dev-the-spire\EzDailyContent.json`
- Active solution path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.sln`
- Active project path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.csproj`
- Active manifest path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.json`
- Legacy project metadata path: `D:\Game\FOTN\dev-the-spire\legacy\EzDailyContent\EzDailyContent.csproj.legacy.xml`
- Legacy DLL path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.dll`
- Legacy PCK path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.pck`
- Legacy JSON path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.json`
- Target DLL path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll`
- Target PCK path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.pck`
- Target JSON path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.json`

## Dependencies
- Template package: `Alchyr.Sts2.Templates` `2.3.9`
- Content template short name: `alchyrsts2contentmod`
- BaseLib runtime status: installed at expected runtime path.
- BaseLib runtime path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`
- BaseLib runtime files: `BaseLib.json`, `BaseLib.dll`, `BaseLib.pck`
- BaseLib runtime version: `v3.1.2`
- BaseLib source release: `https://github.com/Alchyr/BaseLib-StS2/releases/tag/v3.1.2`
- BaseLib old root-level path still present: `D:\Steam\steamapps\common\Slay the Spire 2\BaseLib`
- BaseLib old root-level version: `v0.1.3`
- Project NuGet BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.2`
- BaseLib version consistency: OK. Runtime `v3.1.2` matches project package `3.1.2`.

## Last known commands
- Last attempted default build: `dotnet build EZMicroBalance.sln` on 2026-05-12 after the Urda source gameplay slice. Default Debug builds no longer overwrite installed release artifacts; Release build/publish remains the installed-mod copy path. Result: succeeded with 0 warnings and 0 errors.
- Last successful build: `dotnet build EZMicroBalance.sln` on 2026-05-12 after the Urda source gameplay slice. Result: succeeded with 0 warnings and 0 errors.
- Last successful normal test run: `dotnet test EZMicroBalance.sln --no-build` on 2026-05-12. Result: passed, 73 passed, 16 skipped release artifact/runtime evidence tests, 0 failed.
- Latest opt-in artifact-validation run: `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` on 2026-05-12 after the Urda package refresh. Result: passed, 89 passed, 0 skipped, 0 failed.
- Last formatting check: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` on 2026-05-12 after the Urda package refresh. Result: exit code 0.
- Last required diff check: `git diff --check` on 2026-05-12 after the Urda package refresh. Result: exit code 0 with CRLF normalization warnings only.
- Last attempted publish: `dotnet publish EZMicroBalance.sln` on 2026-05-12 after the Urda localization/export update.
- Last successful publish: `dotnet publish EZMicroBalance.sln` on 2026-05-12. Result: command returned exit code 0, built `EZMicroBalance` in Release, copied `EZMicroBalance.dll` and `EZMicroBalance.json`, and exported the selected-resource PCK with Urda card-reward UI localization included.
- Last PCK audit: publish/package refresh left the installed, staging, versioned, and zip PCK at SHA256 `FCD38F1E5D940D4CDEB94623465FA24D71A75AABFF323586D1B9FBED856D4557`. The latest opt-in automated PCK-content audit matches this full artifact set and is current through the latest package refresh.
- Last installed DLL audit: installed, staging, versioned package, and extracted zip DLL SHA256 all match `EE6B9EE9F2D0D3F4962D6DA11B03E19E6E4806DF08930C1F342BF9530A36A6EF`.
- Last Harmony patch audit: standalone .NET 9 audit called `Harmony.PatchAll(...)` on `EZMicroBalance.dll` and returned `PatchAll OK`.
- Last private beta package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` was rebuilt from the current installed artifacts after the Urda source gameplay slice. The package `README_INSTALL.txt` records the resolved author/generated-art status, the multiplayer diagnostics note, and Urda live-verification limits. SHA256 `2A13A44EA643EA872A8A189883E4EEFFDE8D9DDB8A83A0F5838CE9B6FA8072AD`; entries are `EZMicroBalance/EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt`.
- Last release art audit: `EZMicroBalance/mod_image.png` and `publish\EZMicroBalance-cover-source.png` currently have SHA256 `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75` and were manually checked as original generated art with no visible text, letters, numbers, numerals, logos, or official game assets.
- Publish note: Headless Godot export requires `export_presets.cfg` to be UTF-8 without BOM and needs local runtime references available to the editor assembly scan. `EZMicroBalance.csproj` copies `sts2.dll`, `0Harmony.dll`, and BaseLib into the Godot temp build folders before export; the selected-resource PCK still contains only active mod resources.

## Manual game verification
- Manual game verification succeeded: legacy baseline only.
- Status: target Mod Settings verification passed; live gameplay verification pending.
- Verification surface: Slay the Spire 2 Settings -> Mod Settings.
- BaseLib appeared in Mod Settings: yes.
- BaseLib enabled: yes.
- Legacy EzDailyContent appeared in Mod Settings: yes.
- Legacy EzDailyContent enabled: yes.
- Target EZMicroBalance Mod Settings verification: passed for RC1. A no-op BaseLib `ModConfig` page is registered for EZ Micro Balance, and the normal Steam-client UI probe opened `妯＄粍閰嶇疆` after temporarily isolating non-BaseLib/EZMB mods. Evidence `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` shows the EZ Micro Balance page entry as `寰钩琛�? `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png` shows its `鏃犲彲閰嶇疆閫夐」銆�?page; `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log` shows `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, and 0 `ERROR` / release-blocking signature hits.
- Screenshot-observed in-game version: `v0.104.0`, date `2026.04.23`.
- Current branch target: public beta.

## Runtime smoke attempts
- 2026-05-05: direct `SlayTheSpire2.exe` smoke launch produced a fresh `godot.log` but failed before mod loading because Steamworks initialization reported `No appID found`.
- 2026-05-05: direct launch with temporary `steam_appid.txt` value `2868840` produced a fresh `godot.log` but still failed before mod loading because Steamworks initialization reported `ConnectToGlobalUser failed`; the temporary file was removed after the attempt.
- 2026-05-05: `D:\Steam\steam.exe -applaunch 2868840` did not start a detectable `SlayTheSpire2` process within the bounded smoke-test window.
- 2026-05-05: inspected local `NGame.InitializePlatform()` and found `--force-steam off` skips Steam initialization before startup. This is a local smoke-test path, not a substitute for final Steam-client manual verification.
- 2026-05-05: controlled default-profile smoke launch with `--force-steam off` and temporary settings enabling only `BaseLib` and `EZMicroBalance` found both manifests, loaded BaseLib, loaded `EZMicroBalance.dll` and `.pck`, finished `EZMicroBalance` initialization, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-05: first controlled smoke exposed invalid Harmony targets for `SealOfGoldMaxEnergyPatch` and `CrossbowOfferPatch`; both were retargeted to `AbstractModel` hooks and the final controlled smoke passed mod initialization.
- 2026-05-05: controlled disable smoke with BaseLib enabled and EZ Micro Balance disabled skipped `EZMicroBalance`, did not load its DLL, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-05: after correcting Release solution mapping, isolated controlled default-profile smoke with `--force-steam off` and temporary settings enabling only `BaseLib` and `EZMicroBalance` loaded exactly 2 mods, loaded the installed Release `EZMicroBalance.dll` and `.pck`, finished BaseLib and EZ Micro Balance initialization, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-06: bounded `--force-steam off` smoke after Ascension integration initially exposed a startup `MissingMethodException` for `RootBudCombatHook` because StS2 model database startup requires parameterless constructors for concrete `AbstractModel` types.
- 2026-05-06: final bounded `--force-steam off` smoke after the `source code/` ignore/export hardening and PCK refresh temporarily enabled only `BaseLib` and `EZMicroBalance`, loaded the then-current installed `EZMicroBalance.dll` and `.pck`, registered 8 SavedSpireFields, finished EZ Micro Balance initialization, and reached main menu in `12,755ms`. Temporary profile settings were restored byte-for-byte afterward. The log still includes unrelated local invalid-manifest errors for `RouteSuggestConfig.json` and `sts2-heybox-support`; no EZ Micro Balance startup exception or error was present. This smoke predates the Rootblight level-state migration.
- 2026-05-06: release-engineering bounded `--force-steam off` smoke temporarily enabled only `BaseLib` and `EZMicroBalance` in the default profile, loaded the then-installed `EZMicroBalance.dll` and `.pck`, registered 8 SavedSpireFields, finished EZ Micro Balance initialization, reached main menu in 14 seconds, found 0 EZ Micro Balance error lines, and restored both `default\1\settings.save` and `settings.save.backup` byte-for-byte. This smoke is superseded by the 2026-05-07 current-package smoke below.
- 2026-05-06: bounded `--force-steam off` smoke after the A11+ run-start black-screen fix loaded the refreshed installed DLL/PCK, reached main menu, found 0 EZ Micro Balance error lines, and confirmed the prior `DuplicateModelException` / direct `RootRunHook(RunState)` constructor path was absent from the new `godot.log`. Temporary default-profile settings were restored byte-for-byte.
- 2026-05-07: earlier bounded `--force-steam off` smoke for the A20 fixed-courtyard package temporarily enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, loaded exactly 2 mods, initialized BaseLib and EZ Micro Balance, reached main menu, found 0 EZ Micro Balance error/exception lines, and restored both `default\1\settings.save` and `settings.save.backup` to their original contents. This smoke is superseded by the 2026-05-07 current-package smoke below.
- 2026-05-09: current-package bounded `--force-steam off` smoke after the Rootblight event-room notice package refresh physically isolated unrelated mod entries and enabled only `BaseLib` and `EZMicroBalance`. Evidence under `.tools\runtime-evidence\rootblight-notice-package-smoke-clean-20260509-035904` loaded exactly 2 mods (`Loaded 2 mods (2 total)`), initialized BaseLib and EZ Micro Balance, loaded the current installed `EZMicroBalance.dll` and `.pck`, confirmed the current source defines 13 SavedSpireFields and runtime reported `Found 13 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored `default\1\settings.save`, `settings.save.backup`, and 22 moved mod entries.
- 2026-05-08: RC1 normal Steam-client launch/log probe used `D:\Steam\steam.exe -applaunch 2868840` and reached main menu in `14,444ms`. `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib `Version=3.1.2.0`, `[BaseLib] Applied 177 patches successfully, 0 failed`, BaseLib and EZ Micro Balance initialization, and `Found 13 SavedSpireFields`. Strict scan found 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits. This first probe still contains unrelated local invalid-manifest/dependency `ERROR` lines for discovered disabled mods (`RouteSuggestConfig.json`, `sts2-heybox-support`), so the isolated startup log below supersedes it for clean-log evidence.
- 2026-05-08: fresh RC1 normal Steam-client isolated startup log after the structured-dependency manifest refresh temporarily moved 23 non-BaseLib/EZMB entries out of the game `mods` directory, launched through `D:\Steam\steam.exe -applaunch 2868840`, reached main menu in `13,470ms`, saved startup snapshot `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`, then restored the moved entries and `settings.save`. Positive evidence: only `BaseLib\BaseLib.json` and `EZMicroBalance\EZMicroBalance.json` were discovered, `Loaded 2 mods (2 total)`, BaseLib applied `177 patches successfully, 0 failed`, BaseLib and EZ Micro Balance initialized, and BaseLib reported `Found 13 SavedSpireFields`. Strict scan found 0 `ERROR` lines in the startup snapshot, 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `DamageMeter`, 0 `RouteSuggest`, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 EZMB error/exception pattern hits. The snapshot still has normal startup warnings for D3D12 PSO caching, BaseLib/EZMB manifest metadata, and uncached assets.
- 2026-05-08: RC1 normal Steam-client Mod Settings recheck after adding the no-op EZ Micro Balance config page temporarily moved 23 non-BaseLib/EZMB entries out of the game `mods` directory, launched through `D:\Steam\steam.exe -applaunch 2868840`, opened `妯＄粍閰嶇疆`, captured `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png`, copied `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log`, then restored moved entries and minimized windows. The log has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- 2026-05-09: targeted normal Steam-client A14 Rootblight ZHS hover/notice pass temporarily moved 22 non-BaseLib/EZMB entries out of the game `mods` directory, selected A14 through the original single-player UI, and restored settings/saves/moved mods afterward. `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516` verifies Simplified Chinese Rootblight I/II/III and Blight Sprout hovers with one visible Exhaust keyword, no raw `[gold]` tags, and expected previews. `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455\07-run-start-06.png` verifies the A14 Neow starter Rootblight-added notice.
- 2026-05-09: targeted normal Steam-client A14 Rootblight English hover/notice pass under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` verifies the English Rootblight-added Neow notice with deck count 11 plus Rootblight I/II/III and Blight Sprout hovers with one visible Exhaust keyword, no raw `[gold]` tags, and expected previews. `restore-check.json` confirms settings/saves and 22 moved mod entries were restored. Combat-end notices, full Rootblight/Blight Sprout behavior, generated-art visual verification, and co-op ownership/desync remain pending.
- 2026-05-09: a separate normal Steam-client BaseLib+EZMB-only main-menu log under `.tools\runtime-evidence\rootblight-a14-notice-zhs-no-current-20260509-041615\godot-mainmenu.log` audited clean with 0 `ERROR` lines and 0 release-blocking signatures. Steam cloud rehydrated current-run files before startup, so this is clean startup evidence, not Rootblight notice evidence.
- Controlled smoke note: local unrelated mods `RouteSuggest-v1.9.0` and `sts2-heybox-support` still emit invalid-manifest scan errors from their own JSON files before disabled-mod filtering; these are not emitted by `EZMicroBalance`.
- Runtime verification still requires live gameplay/manual feature matrix checks; normal Steam-client Mod Settings, the Act 1 A11 map/save-load spot check, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks are now covered by RC1 evidence.
- refreshed runtime smoke remains pending in current logs until the next isolated smoke run log is retained as a fresh controlled-pass evidence file.

## Phase progress
- Completed setup baseline: build, publish, and legacy Mod Settings verification.
- Current release phase: EZ Micro Balance Ancient v4.3 private beta completion.
- Independent project structure: created.
- Build/publish status: complete for local artifacts.
- Automated release artifact/source guard tests: normal source/localization/docs tests pass; release artifact/runtime evidence tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- Ascension 11-20 implementation track: research complete; A11-A20 selection is now default-on in this private-beta multiplayer test candidate. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Full live Ascension verification is pending, and live co-op verification is pending. Host multiplayer A20 selection/start logs that multiplayer A20 is development testing and that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification. A11 now widens maps by 1 column and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2, without A11-specific map markers or hover tips. RC1 normal Steam-client evidence covers the A11 Act 1 map/save-load spot check with `columns=8; rows=17` and Act 2/3 map-surface observation with Act 2 `columns=8; rows=16` plus Act 3 `columns=8; rows=16`; natural route traversal remains pending. A17 now inserts one optional 3-4 node Deep Branch in Acts 2/3 when safe single-player map geometry is available. A20 now uses the vanilla double-boss path for final-act Boss 2 creation/reveal, Boss 2 Brand metadata/parameters, Boss 1 recovery, one Boss card reward, narrow Boss 1 reward-screen intermission wording, and a fixed courtyard event before Boss 2; a bespoke full custom intermission screen remains deferred. Controlled smoke passed is not the same as live co-op verification; normal Steam-client Mod Settings is covered by the separate RC1 evidence above.
- Remaining before private beta: manual feature matrix, save/load, multiplayer disposition, generated-art visual verification, clean commit, and user-approved push.

## TODO
1. Rerun the release artifact tests and manual/live gates for the latest author/art package before final release handoff.
2. Check `godot.log` during private beta verification.
3. Old root-level BaseLib folder remains present; leave it untouched unless explicitly cleaning up later.

