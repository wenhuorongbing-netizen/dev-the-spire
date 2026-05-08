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
- Observed installed game version: `v0.105.0`
- Observed installed game date: `2026.05.07` upstream build date, installed locally on `2026-05-08`
- Live observed in-game version (player report, 2026-05-08): `v0.105.0, 2026.05.08`
- Local source snapshot: `source code/` was refreshed from `D:\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck` on 2026-05-08 using GDRE Tools `v2.5.0-beta.5`; `release_info.json` reports commit `e4579d36`, branch/version `v0.105.0`, and `main_assembly_hash` `359406027`.
- Source recovery log: `source code/gdre_export.log` detected Godot `4.5.1`, bytecode `4.5.0-stable (ebc36a7)`, extracted 15,551 files with no extraction errors, decompiled 3,448 scripts, reported 18 failed scripts, and successfully ran `dotnet build` for the recovered `sts2.sln`.
- BaseLib `v3.1.2` compatibility with v0.105.0: controlled main-menu smoke passed with only BaseLib + EZMicroBalance enabled. The earlier `v3.1.0` / 17-mod live log (`godot2026-05-08T05.06.30.log`) is superseded for loader evidence, but normal Steam-client Mod Settings and live gameplay/combat verification are still pending.
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
- Last attempted default build: `dotnet build EZMicroBalance.sln` on 2026-05-08 after the v0.105.0/BaseLib v3.1.2 source/API refresh. Default Debug builds no longer overwrite installed release artifacts; Release build/publish remains the installed-mod copy path. Result: succeeded with 0 warnings and 0 errors.
- Last successful build: `dotnet build EZMicroBalance.sln` on 2026-05-08 after the v0.105.0/BaseLib v3.1.2 source/API refresh. Result: succeeded with 0 warnings and 0 errors.
- Last successful normal test run: `dotnet test EZMicroBalance.sln --no-build` on 2026-05-08 after the final v0.105.0 package/hash/smoke refresh. Result: passed, 65 passed, 16 skipped release artifact/runtime evidence tests, 0 failed.
- Last attempted test run: `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` on 2026-05-08 after the final v0.105.0 package/hash/smoke refresh. Result: passed, 81 passed, 0 skipped, 0 failed.
- Last formatting check: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` on 2026-05-08 after the final v0.105.0 package/smoke/doc refresh. Result: exit code 0.
- Last required diff check: `git diff --check` on 2026-05-08 after the final v0.105.0 package/smoke/doc refresh. Result: exit code 0 with CRLF normalization warnings for touched files.
- Last attempted publish: `dotnet publish EZMicroBalance.sln` on 2026-05-08 after the Brightest Flame zhs placeholder parity fix.
- Last successful publish: `dotnet publish EZMicroBalance.sln` on 2026-05-08. Result: command returned exit code 0, built `EZMicroBalance` in Release, copied `EZMicroBalance.dll` and `EZMicroBalance.json`, exported the selected-resource `EZMicroBalance.pck`, and did not publish the test project.
- Last PCK audit: automated release tests parsed `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.pck` after publish/package refresh. Result: 0 entries from `EzDailyContent`, `EzDailyContentCode`, `EZMicroBalanceCode`, `docs`, `art_pipeline`, `asset`, `source code`, or `legacy`; SHA256 `89D87BEB637EDE00A62A57491563A2254BBABBC471859C5B32F74C11F6D89A7F`.
- Last installed DLL audit: installed, staging, versioned package, and extracted zip DLL SHA256 all match `215A4621019CA93ABB0157BBFEA094FE4C8DBDEA247ECA02222709298784CF5C`.
- Last Harmony patch audit: standalone .NET 9 audit called `Harmony.PatchAll(...)` on `EZMicroBalance.dll` and returned `PatchAll OK`.
- Last private beta package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` was rebuilt after the 2026-05-08 v0.105.0/BaseLib v3.1.2 refresh. SHA256 `6C3A9CE64D7227BBC5204D1EC1215EA6877818E24E4400910DCE8BF9199BC090`; entries are `EZMicroBalance/EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt`.
- Last release art audit: `EZMicroBalance/mod_image.png` and `publish\EZMicroBalance-cover-source.png` currently have SHA256 `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75` and were manually checked as original generated art with no visible text, letters, numbers, numerals, logos, or official game assets.
- Publish note: Godot still prints a non-fatal script-scan `sts2` assembly load exception during headless export because the project assembly references runtime game assemblies that are not loaded by the editor process.

## Manual game verification
- Manual game verification succeeded: legacy baseline only.
- Status: target runtime verification pending.
- Verification surface: Slay the Spire 2 Settings -> Mod Settings.
- BaseLib appeared in Mod Settings: yes.
- BaseLib enabled: yes.
- Legacy EzDailyContent appeared in Mod Settings: yes.
- Legacy EzDailyContent enabled: yes.
- Target EZMicroBalance Mod Settings verification: pending runtime game pass.
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
- 2026-05-08: current-package bounded `--force-steam off` smoke after the v0.105.0/BaseLib v3.1.2 publish/package refresh temporarily enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, loaded exactly 2 mods (`Loaded 2 mods (19 total)`), initialized BaseLib and EZ Micro Balance, loaded the current installed `EZMicroBalance.dll` and `.pck`, confirmed the current source defines 12 SavedSpireFields and runtime reported `Found 12 SavedSpireFields`, logged the default-on Ascension initializer wording with 0 old `Default-off gate` lines, reached main menu in `13,628ms`, found 0 EZ Micro Balance error/exception lines, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or DamageMeter removed-API signatures, and restored both `default\1\settings.save` and `settings.save.backup` byte-for-byte.
- Controlled smoke note: local unrelated mods `RouteSuggest-v1.9.0` and `sts2-heybox-support` still emit invalid-manifest scan errors from their own JSON files before disabled-mod filtering; these are not emitted by `EZMicroBalance`.
- Runtime verification still requires launching from the normal Steam client path and checking Mod Settings plus the manual feature matrix.

## Phase progress
- Completed setup baseline: build, publish, and legacy Mod Settings verification.
- Current release phase: EZ Micro Balance Ancient v4.3 private beta completion.
- Independent project structure: created.
- Build/publish status: complete for local artifacts.
- Automated release artifact/source guard tests: normal source/localization/docs tests pass; release artifact/runtime evidence tests are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- Ascension 11-20 implementation track: research complete; A11-A20 selection is now default-on in this private-beta multiplayer test candidate. Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison. Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection. `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required. Full live Ascension verification is pending, and live co-op verification is pending. Host multiplayer A20 selection/start logs that multiplayer A20 is development testing and that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification. A11 now widens maps by 1 column and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2, without A11-specific map markers or hover tips. A17 now inserts one optional 3-4 node Deep Branch in Acts 2/3 when safe single-player map geometry is available. A20 now uses the vanilla double-boss path for final-act Boss 2 creation/reveal, Boss 2 Brand metadata/parameters, Boss 1 recovery, one Boss card reward, narrow Boss 1 reward-screen intermission wording, and a fixed courtyard event before Boss 2; a bespoke full custom intermission screen remains deferred. Controlled smoke passed is not the same as normal Steam-client Mod Settings or live co-op verification.
- Remaining before private beta: normal Steam-client Mod Settings verification, manual feature matrix, save/load, multiplayer disposition, author decision, clean commit, and user-approved push.

## TODO
1. Decide whether private beta can ship with `AUTHOR_NAME_REPLACE_ME` or ask the user for the desired author name before final release.
2. Check `godot.log` during private beta verification.
3. Old root-level BaseLib folder remains present; leave it untouched unless explicitly cleaning up later.
