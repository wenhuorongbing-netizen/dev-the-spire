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
- Observed installed game version: `v0.106.0` from `D:\Steam\steamapps\common\Slay the Spire 2\release_info.json` on 2026-05-22.
- Observed installed game metadata: commit `cb2fbf47`, branch `v0.106.0`, build date `2026-05-21T16:17:40-07:00`, and `main_assembly_hash` `1001788235`.
- Steam appmanifest evidence: public-beta `BetaKey`, build id `23353684`, installed locally on 2026-05-22.
- Local source snapshot: `source code/` was cleaned and recovered from the installed `SlayTheSpire2.pck` plus `data_sts2_windows_x86_64\sts2.dll` on 2026-05-22 using GDRE Tools `v2.5.0-beta.5`.
- Source recovery evidence: `source code\release_info.json` matches `v0.106.0`; `source code/src/Core` contains 3,393 C# files; `source code/sts2.sln` builds with 0 warnings and 0 errors.
- Source recovery log: `source code/gdre_export.log` decompiled 3,463 scripts, reported 18 failed scripts, converted 3,937 resources, and logged one debug-scene parse warning for `scenes/debug/back_confirm_example.tscn`.
- API-diff artifacts live under `.tools\source-refresh-v0.106.0-20260522\`; the canonical Core signature diff found 1,712 removed signatures and 1,835 added signatures compared with the previous snapshot.
- BaseLib `v3.1.4` compatibility with v0.106.0:
  - Historical 22-field loader evidence: `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with only BaseLib plus Spire Plus under technical id `EZMicroBalance`, BaseLib `177 patches successfully, 0 failed`, config registration, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Found 22 SavedSpireFields`, `Time to main menu: 13,539ms`, and no release-blocking log signatures.
  - Runtime files under `<GameRoot>\mods\BaseLib` now report `v3.1.4`; the project references `Alchyr.Sts2.BaseLib` `3.1.4`.
  - Historical beta.19 loader evidence: `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` reached startup completion with only BaseLib plus Spire Plus, registered config for `EZMicroBalance`, reported `v0.1.0-private-beta.19`, `Found 30 SavedSpireFields`, matched its beta.19 package hash, audited clean, stopped the game, and restored the isolated mod setup. The beta.31 package still needs fresh loader proof. The beta.17 loader evidence remains historical context only.
  - Current display-name list evidence shows `Spire Plus`; older page-level Mod Settings evidence predates the display-name refresh.
  - A0/A10/A20 combat smoke, limited A11 map/save-load evidence, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight hover/starter-notice checks exist. Full Ancient gameplay, full Rootblight combat behavior, natural A11 traversal, and co-op verification remain pending.
- Do not use pre-2026-05-22 `v0.105.x` source notes as the sole basis for current `v0.106.0` conclusions. Reinspect the refreshed source and confirm runtime behavior before making release claims.
- Test environment must be ONLY BaseLib + Spire Plus. The Spire Plus technical folder/id is `EZMicroBalance`. Earlier 17-mod logs including DamageMeter, RouteSuggest, AnimeWaifuSilent, etc. are invalid release evidence.


## Project mission
- Active deliverable: `Spire Plus` private beta (`EZMicroBalance` technical id).
- Scope: Ancient reward rebalance v4.3, single Spire Plus mod structure, preview tools, localization, build/publish, manual verification, and gated Ascension 11-20 implementation work when explicitly requested.
- Out of scope this cycle: Ascension 21-30 and custom character implementation.

## Mod
- Legacy scaffold mod name: historical `Easy Content`
- Legacy manifest id: historical `EzDailyContent`
- Target private beta mod name: `Spire Plus`
- Target private beta manifest id: `EZMicroBalance`
- Active solution path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.sln`
- Active project path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.csproj`
- Active manifest path: `D:\Game\FOTN\dev-the-spire\EZMicroBalance.json`
- Legacy project metadata path: `D:\Game\FOTN\dev-the-spire\docs\archive\legacy-planning\legacy-project-files\EzDailyContent\EzDailyContent.csproj.legacy.xml`
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
- BaseLib runtime version: `v3.1.4`
- BaseLib source release: `https://github.com/Alchyr/BaseLib-StS2/releases/tag/v3.1.4`
- BaseLib old root-level path still present: `D:\Steam\steamapps\common\Slay the Spire 2\BaseLib`
- BaseLib old root-level version: `v0.1.3`
- Project NuGet BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.4`
- BaseLib version consistency: OK. Runtime `v3.1.4` matches project package `3.1.4`.

## Last known commands
- Last attempted default build: `dotnet build EZMicroBalance.sln` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: succeeded with 0 warnings and 0 errors.
- Last successful build: `dotnet build EZMicroBalance.sln` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: succeeded with 0 warnings and 0 errors.
- Last successful normal test run: `dotnet test EZMicroBalance.sln --no-build` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: passed, 287 passed, 20 skipped release artifact/runtime evidence tests, 0 failed.
- Last successful Release test run: `dotnet test EZMicroBalance.sln -c Release` on 2026-05-13 after the BaseLib-only plug-off startup/log refresh. Result: passed, 81 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- Latest opt-in artifact-validation run: `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` on 2026-05-26 after the beta.31 Soul Tide timing refresh and installed-folder sync. Result: passed, 307 passed, 0 skipped, 0 failed. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- Latest full local CI-script run: `scripts\ci-full-validation.ps1` on 2026-05-20 with explicit `STS2_PATH` and `GODOT_PATH`. Result: passed. This is local no-game validation, not live loader or gameplay proof.
- Last formatting check: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: exit code 0.
- Last required diff check: `git diff --check` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: exit code 0 with CRLF/LF warnings only.
- Last attempted default publish: `dotnet publish EZMicroBalance.sln` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: blocked by a running `SlayTheSpire2.exe` process locking `mods\EZMicroBalance\EZMicroBalance.dll`.
- Last successful publish: `dotnet publish EZMicroBalance.sln -p:ModsPath=.tools\publish-game-root\mods\` on 2026-05-26 after the beta.31 Soul Tide timing refresh. Result: succeeded against an isolated temporary mods root.
- Publish/package note: package staging, the versioned package folder, `publish\SpirePlus-v0.1.0-private-beta.31.zip`, the isolated `.tools\publish-game-root\mods\EZMicroBalance` folder, `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and `D:\Steam\steamapps\common\Slay the Spire 2\SpirePlus-v0.1.0-private-beta.31.zip` were refreshed and hash-checked on 2026-05-26 for local manual testing. The zip uses the player-facing `SpirePlus` archive name while the install folder remains `EZMicroBalance`. The beta.19 loader row is historical startup/log proof only; beta.31 loader, live gameplay, save-load, failure/death-path, clicked Ancient UI, and co-op verification remain pending.
- Last PCK hash check: the 2026-05-26 local manual-test package refresh left the staging, versioned, and zip-entry PCK at SHA256 `8A267BA928320DCA3A99FD4EA168F7863CD0AA28BF2ECB17FAD2FF24A0C1B26E`.
- Last staging/versioned DLL hash check: SHA256 `C5BB994347596FA4EE10DE23D1CC2AE88DD4779D40970919510A413335CE7C9B`.
  Detailed pass history lives in `docs/review.md` and `docs/archive/**`.
- Last Harmony patch audit: standalone .NET 9 audit called `Harmony.PatchAll(...)` on `EZMicroBalance.dll` and returned `PatchAll OK`.
- Last private beta package: `publish\SpirePlus-v0.1.0-private-beta.31.zip` was rebuilt from isolated package staging, copied to the game root, and synced into the real installed mod folder on 2026-05-26. The staging, versioned, installed, game-root zip, and zip-entry artifacts match the hashes below.
  - Package note: `README_INSTALL.txt` is a short manual-test install note and says Ancient selections grant visible marker relics.
  - Zip SHA256: `E5299E778F78878C1A62934B999D94BC51F1682EA865A2C7996E54AEFB86B618`
  - DLL SHA256: `C5BB994347596FA4EE10DE23D1CC2AE88DD4779D40970919510A413335CE7C9B`
  - README SHA256: `45F0E46431421CEF3A4FF932E3D189469C5FFBA678DF414383BF3D274CDFA429`
  - Manifest SHA256: `A92F92402A30C459D98E65BD29FB3BC5AC70B14A587002AAA4E4E1CC0C7D4F23`
  - PCK SHA256: `8A267BA928320DCA3A99FD4EA168F7863CD0AA28BF2ECB17FAD2FF24A0C1B26E`
  - Entries: `EZMicroBalance/EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt`.
- Last release art audit: `EZMicroBalance/mod_image.png` and `publish\EZMicroBalance-cover-source.png` currently have SHA256 `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75` and were manually checked as original generated art with no visible text, letters, numbers, numerals, logos, or official game assets.
- Publish note: Headless Godot export requires `export_presets.cfg` to be UTF-8 without BOM and needs local runtime references available to the editor assembly scan. `EZMicroBalance.csproj` copies `sts2.dll`, `0Harmony.dll`, and BaseLib into the Godot temp build folders before export; the selected-resource PCK still contains only active mod resources.

## Manual game verification
- Manual game verification succeeded: legacy baseline only.
- Status: latest normal Steam startup/log verification covers the historical beta.19 package hash and loaded only BaseLib plus Spire Plus with a clean audit. Refreshed Mod Settings UI list screenshot shows `Spire Plus` under the display-name package; beta.31 loader and live gameplay verification remain pending.
- Verification surface: Slay the Spire 2 Settings -> Mod Settings.
- BaseLib appeared in Mod Settings: yes.
- BaseLib enabled: yes.
- Legacy EzDailyContent appeared in Mod Settings: yes.
- Legacy EzDailyContent enabled: yes.
- Target EZMicroBalance Mod Settings verification:
  - Current UI list pass under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` shows `Spire Plus` in the Mods list with only BaseLib and `EZMicroBalance` enabled.
  - Historical startup/log evidence under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` logged `v0.1.0-private-beta.19`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 30 SavedSpireFields`, startup completion, beta.19 package hash parity, and 0 release-blocking hits.
  - Earlier package startup/log evidence under `.tools\runtime-evidence\current-package-smoke-20260514-015901` is historical 22-field loader/resource evidence.
  - Historical page-level UI evidence remains under `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-*` with the old display name.
- Screenshot-observed in-game version: `v0.104.0`, date `2026.04.23`.
- Current branch target: public beta.

## Runtime smoke attempts
- 2026-05-05: direct `SlayTheSpire2.exe` smoke launch produced a fresh `godot.log` but failed before mod loading because Steamworks initialization reported `No appID found`.
- 2026-05-05: direct launch with temporary `steam_appid.txt` value `2868840` produced a fresh `godot.log` but still failed before mod loading because Steamworks initialization reported `ConnectToGlobalUser failed`; the temporary file was removed after the attempt.
- 2026-05-05: `D:\Steam\steam.exe -applaunch 2868840` did not start a detectable `SlayTheSpire2` process within the bounded smoke-test window.
- 2026-05-05: inspected local `NGame.InitializePlatform()` and found `--force-steam off` skips Steam initialization before startup. This is a local smoke-test path, not a substitute for final Steam-client manual verification.
- 2026-05-05: controlled default-profile smoke launch with `--force-steam off` and temporary settings enabling only `BaseLib` and `EZMicroBalance` found both manifests, loaded BaseLib, loaded `EZMicroBalance.dll` and `.pck`, finished `EZMicroBalance` initialization, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-05: first controlled smoke exposed invalid Harmony targets for `SealOfGoldMaxEnergyPatch` and `CrossbowOfferPatch`; both were retargeted to `AbstractModel` hooks and the final controlled smoke passed mod initialization.
- 2026-05-05: controlled disable smoke with BaseLib enabled and Spire Plus disabled skipped `EZMicroBalance`, did not load its DLL, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-05: after correcting Release solution mapping, isolated controlled default-profile smoke with `--force-steam off` and temporary settings enabling only `BaseLib` and `EZMicroBalance` loaded exactly 2 mods, loaded the installed Release `EZMicroBalance.dll` and `.pck`, finished BaseLib and Spire Plus initialization, and reached main menu. Original default-profile settings were restored after the test.
- 2026-05-06: bounded `--force-steam off` smoke after Ascension integration initially exposed a startup `MissingMethodException` for `RootBudCombatHook` because StS2 model database startup requires parameterless constructors for concrete `AbstractModel` types.
- 2026-05-06: final bounded `--force-steam off` smoke after the `source code/` ignore/export hardening and PCK refresh temporarily enabled only `BaseLib` and `EZMicroBalance`, loaded the then-current installed `EZMicroBalance.dll` and `.pck`, registered 8 SavedSpireFields, finished Spire Plus initialization, and reached main menu in `12,755ms`. Temporary profile settings were restored byte-for-byte afterward. The log still includes unrelated local invalid-manifest errors for `RouteSuggestConfig.json` and `sts2-heybox-support`; no Spire Plus startup exception or error was present. This smoke predates the Rootblight level-state migration.
- 2026-05-06: release-engineering bounded `--force-steam off` smoke temporarily enabled only `BaseLib` and `EZMicroBalance` in the default profile, loaded the then-installed `EZMicroBalance.dll` and `.pck`, registered 8 SavedSpireFields, finished Spire Plus initialization, reached main menu in 14 seconds, found 0 Spire Plus error lines, and restored both `default\1\settings.save` and `settings.save.backup` byte-for-byte. This smoke is superseded by the 2026-05-07 current-package smoke below.
- 2026-05-06: bounded `--force-steam off` smoke after the A11+ run-start black-screen fix loaded the refreshed installed DLL/PCK, reached main menu, found 0 Spire Plus error lines, and confirmed the prior `DuplicateModelException` / direct `RootRunHook(RunState)` constructor path was absent from the new `godot.log`. Temporary default-profile settings were restored byte-for-byte.
- 2026-05-07: earlier bounded `--force-steam off` smoke for the A20 fixed-courtyard package temporarily enabled only `BaseLib` and `EZMicroBalance`, explicitly disabled other discovered local mods, loaded exactly 2 mods, initialized BaseLib and Spire Plus, reached main menu, found 0 Spire Plus error/exception lines, and restored both `default\1\settings.save` and `settings.save.backup` to their original contents. This smoke is superseded by the 2026-05-07 current-package smoke below.
- 2026-05-09: older-package bounded `--force-steam off` smoke after the Rootblight event-room notice package refresh physically isolated unrelated mod entries and enabled only `BaseLib` and `EZMicroBalance`. Evidence under `.tools\runtime-evidence\rootblight-notice-package-smoke-clean-20260509-035904` loaded exactly 2 mods (`Loaded 2 mods (2 total)`), initialized BaseLib and Spire Plus, loaded the installed `EZMicroBalance.dll` and `.pck` from that package state, confirmed that package source defined 13 SavedSpireFields and runtime reported `Found 13 SavedSpireFields`; this smoke is superseded by the 2026-05-13 current-package 16-field smoke below.
- 2026-05-08: RC1 normal Steam-client launch/log probe used `D:\Steam\steam.exe -applaunch 2868840` and reached main menu in `14,444ms`. `godot.log` showed `Loaded 2 mods (19 total)`, BaseLib `Version=3.1.2.0`, `[BaseLib] Applied 177 patches successfully, 0 failed`, BaseLib and Spire Plus initialization, and `Found 13 SavedSpireFields`. Strict scan found 0 `Creature.get_ShowsInfiniteHp`, 0 `BaseLib.Patches.UI.HealthBarForecastPatch`, 0 BaseLib undefined-target patch failures, 0 `TypeLoadException`, 0 `MissingMethodException`, and 0 Spire Plus error/exception pattern hits. This first probe still contains unrelated local invalid-manifest/dependency `ERROR` lines for discovered disabled mods (`RouteSuggestConfig.json`, `sts2-heybox-support`), so the isolated startup log below supersedes it for clean-log evidence.
- 2026-05-08: fresh RC1 normal Steam-client isolated startup log after the structured-dependency manifest refresh:
  - Evidence: temporarily moved 23 non-BaseLib/Spire Plus mod entries, launched through `D:\Steam\steam.exe -applaunch 2868840`, reached main menu in `13,470ms`, saved `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log`, then restored moved entries and `settings.save`.
  - Positive signals: only `BaseLib\BaseLib.json` and `EZMicroBalance\EZMicroBalance.json` were discovered, `Loaded 2 mods (2 total)`, BaseLib applied `177 patches successfully, 0 failed`, Spire Plus initialized, and BaseLib reported `Found 13 SavedSpireFields`.
  - Strict scan found 0 `ERROR`, removed-healthbar API, BaseLib undefined-target patch, disabled-mod, `TypeLoadException`, `MissingMethodException`, and EZMB error/exception hits. Normal D3D12, manifest metadata, and uncached-asset warnings remain.
- 2026-05-08: RC1 normal Steam-client Mod Settings recheck after adding the no-op Spire Plus config page temporarily moved 23 non-BaseLib/Spire Plus entries out of the game `mods` directory, launched through `D:\Steam\steam.exe -applaunch 2868840`, opened the Mod Settings list and the Spire Plus config page, captured `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-modconfig-list.png` and `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-ezmb-page.png`, copied `.tools\runtime-evidence\rc1-normal-steam-modsettings-page-godot-20260508-095137.log`, then restored moved entries and minimized windows. The log has config registration, `Loaded 2 mods (2 total)`, 0 `ERROR` lines, and 0 release-blocking signatures.
- 2026-05-09: targeted normal Steam-client A14 Rootblight ZHS hover/notice pass temporarily moved 22 non-BaseLib/Spire Plus entries out of the game `mods` directory, selected A14 through the original single-player UI, and restored settings/saves/moved mods afterward. `.tools\runtime-evidence\rootblight-a14-ui-eng-20260509-033516` verifies Simplified Chinese Rootblight I/II/III and Blight Sprout hovers with one visible Exhaust keyword, no raw `[gold]` tags, and expected previews. `.tools\runtime-evidence\rootblight-a14-notice-zhs-step-20260509-040455\07-run-start-06.png` verifies the A14 Neow starter Rootblight-added notice.
- 2026-05-09: targeted normal Steam-client A14 Rootblight English hover/notice pass under `.tools\runtime-evidence\rootblight-a14-hover-eng-20260509-044010` verifies the English Rootblight-added Neow notice with deck count 11 plus Rootblight I/II/III and Blight Sprout hovers with one visible Exhaust keyword, no raw `[gold]` tags, and expected previews. `restore-check.json` confirms settings/saves and 22 moved mod entries were restored. Combat-end notices, full Rootblight/Blight Sprout behavior, generated-art visual verification, and co-op ownership/desync remain pending.
- 2026-05-09: a separate normal Steam-client BaseLib + Spire Plus-only main-menu log under `.tools\runtime-evidence\rootblight-a14-notice-zhs-no-current-20260509-041615\godot-mainmenu.log` audited clean with 0 `ERROR` lines and 0 release-blocking signatures. Steam cloud rehydrated current-run files before startup, so this is clean startup evidence, not Rootblight notice evidence.
- Controlled smoke note: local unrelated mods `RouteSuggest-v1.9.0` and `sts2-heybox-support` still emit invalid-manifest scan errors from their own JSON files before disabled-mod filtering; these are not emitted by `EZMicroBalance`.
- Runtime verification still requires live gameplay/manual feature matrix checks; current normal Steam-client startup/log, historical Mod Settings UI, the Act 1 A11 map/save-load spot check, saved-map boss-reachability graph proof, Act 2/3 A11 map-surface observation, and targeted A14 Rootblight English/ZHS hover/starter-notice spot checks are now covered by evidence.
- 2026-05-13: current-package bounded `--force-steam off` smoke under `.tools\runtime-evidence\current-package-smoke-20260513-044306` temporarily enabled BaseLib and Spire Plus, loaded exactly 2 mods (`Loaded 2 mods (22 total)`), initialized BaseLib and Spire Plus, reported `Found 16 SavedSpireFields`, reached main menu in `13,884ms`, found 0 Spire Plus error signatures, found no `Creature.get_ShowsInfiniteHp`, BaseLib patch-failure, or removed-API signatures, and restored `settings.save` plus `settings.save.backup` byte-for-byte. The audit is not fully clean because unrelated disabled local-mod manifest/name noise from RouteSuggest, sts2-heybox-support, and DamageMeter remains in this developer mods folder.
- 2026-05-13: previous normal Steam-client isolated startup/log verification under `.tools\runtime-evidence\current-spire-plus-normal-steam-20260513-054241` temporarily moved 24 non-BaseLib/Spire Plus entries out of `D:\Steam\steamapps\common\Slay the Spire 2\mods`, enabled only BaseLib and `EZMicroBalance` in Steam user settings, launched through `D:\Steam\steam.exe -applaunch 2868840`, reached main menu in `12,790ms`, copied `godot.log`, and restored settings plus moved mod entries. The log lists `0: BaseLib (BaseLib)` and `1: Spire Plus (EZMicroBalance)`, has `Registered config for mod EZMicroBalance`, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, and audits clean with 0 `ERROR` / release-blocking signature hits.
- 2026-05-13: A14 Rootblight generated-art hover probe under `.tools\runtime-evidence\current-rootblight-art-hover-20260513-114103`:
  - Negative evidence: the run entered the default-on Urda Ancient event before combat and `godot-live.log` exposed missing vanilla-derived Urda icon/run-history/background scene asset paths.
  - Follow-up fix: Urda now uses BaseLib `CustomAncientModel` custom icon/background-scene paths and packages the Urda background scene.
  - Headless installed-PCK resource-load check `.tools\runtime-evidence\urda-pck-resource-load-20260513-123345` resolved the custom Urda scene/icon with 0 `ERROR` / `WARNING` lines. Post-fix live Urda and Rootblight visual/gameplay verification remains pending.
- 2026-05-13: `scripts/spire-plus-live-session.ps1` was added for repeatable normal Steam-client local live-test prepare/restore sessions. No-launch smoke checks created `.tools\runtime-evidence\live-spire-plus-session-*` evidence state, restored Steam `settings.save` byte-for-byte, restored 24 temporarily moved non-BaseLib/EZMicroBalance mod entries, and confirmed current-run isolation is a clean no-op when no current-run files exist. Restore now supports `-PreserveNewCurrentRunsOnRestore` for sessions that start or continue a run; it moves test-created `current_run*` files into the evidence folder before restoring the user's original current-run files.
- 2026-05-13: `-PreserveNewCurrentRunsOnRestore` no-launch smoke passed under `.tools\runtime-evidence\live-helper-preserve-current-run-smoke-20260513-133431`: the helper moved the original current run aside, a dummy test-created `current_run.save` was preserved into the evidence folder during restore, the original current run was restored, and Steam settings restored to the pre-session hash. This is tooling safety evidence only, not gameplay evidence.
- 2026-05-13: `scripts/check-spire-window-preflight.ps1` was added after invalid live screenshot attempts showed another foreground application covering Slay the Spire 2 during desktop capture. The preflight reports foreground window state and can fail with `-RequireSpireForeground` before screenshot evidence is collected.
- 2026-05-13: window-preflight smoke under `.tools\runtime-evidence\window-preflight-smoke-20260513-135402` succeeded without changing window state and reported `VampireSurvivors` as the foreground process with Slay the Spire 2 not running. This confirms the preflight would have rejected screenshot capture instead of producing misleading gameplay evidence.
- 2026-05-13: helper-driven normal Steam startup/log validation under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` prepared a restore-safe session, moved 24 non-BaseLib/Spire Plus entries aside, enabled only BaseLib and `EZMicroBalance`, launched through Steam, logged `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 16 SavedSpireFields`, and `Time to main menu: 13,849ms`, then stopped the game and restored settings plus moved entries. `godot-log-audit.json` is clean with 0 release-blocking signatures. This is loader/helper evidence only; gameplay/manual gates remain pending.
- 2026-05-13: the first `-DisableSpirePlus` normal Steam attempt under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-142835` was invalid for plug-off evidence because settings-only `is_enabled=false` still loaded Spire Plus. The helper was tightened so `-DisableSpirePlus` requires `-MoveOtherMods` and temporarily isolates `EZMicroBalance` out of the mods folder.
- 2026-05-13: BaseLib-only plug-off normal Steam startup/log validation under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` prepared a restore-safe session, moved 25 entries including `EZMicroBalance`, launched through Steam, logged `Loaded 1 mods (1 total)` and `Finished mod initialization for 'BaseLib' (BaseLib)`, did not initialize Spire Plus under technical id `EZMicroBalance`, audited clean, then stopped the game and restored settings, the current-run save, and all moved mod entries. This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.
- 2026-05-14: historical package smoke/log/resource verification under `.tools\runtime-evidence\current-package-smoke-20260514-015901`:
  - Scope: earlier 22-field package, installed/staging/versioned/zip artifact parity, installed README sync, and headless installed-PCK loading for Urda/Morvi/Lotha scenes plus 43 Ancient textures.
  - Log signals: BaseLib `177 patches successfully, 0 failed`, config registered for `EZMicroBalance`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, `Time to main menu: 14,045ms`, and 0 release-blocking scan hits.
  - Boundary: Current source defines 30 SavedSpireFields after the 2026-05-17 static fixes, so this smoke is historical loader/log/resource evidence only. Gameplay/manual gates remain pending.
  - Restore stopped the game, restored settings to the original hash, restored moved mod entries/current-run files, preserved Steam-rehydrated test current-run files under evidence, and left 0 `SlayTheSpire2` processes.
- 2026-05-25: Steam-client loader smoke under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` prepared a restore-safe session, moved 24 non-BaseLib/Spire Plus entries aside, enabled only BaseLib and `EZMicroBalance`, deleted the stale pre-launch `godot.log`, launched through Steam, logged `v0.1.0-private-beta.19`, `Loaded 2 mods (2 total)`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Registered config for mod EZMicroBalance`, `Found 30 SavedSpireFields`, and startup completion, then stopped the game and restored settings plus moved entries. `godot-log-audit.json` is clean with 0 release-blocking signatures. This is startup/log evidence for the beta.19 package; gameplay/manual gates remain pending.

## Phase progress
- Completed setup baseline: build, publish, and legacy Mod Settings verification.
- Current release phase: Spire Plus (`EZMicroBalance`) Ancient v4.3 private beta completion.
- Independent project structure: created.
- Build/publish status: complete for local artifacts.
- Automated release artifact/source guard tests: normal source/localization/docs tests pass; release artifact/runtime evidence tests are opt-in with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1`. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- Ascension 11-20 implementation track:
  - Research complete. A11-A20 selection is default-on for single-player; host-multiplayer A11-A20 selection/gameplay fails closed by default unless explicitly enabled for two-client debugging with `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY=1`.
  - Gates: `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, and legacy-compatible `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`.
  - Live Ascension and live co-op verification remain pending. Host A20 multiplayer logs describe development-test status and downgraded A20 Branded Form / second-boss enhanced dedicated ability behavior.
  - A11 widens maps by 1 column and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2. It has no A11-specific map markers or hover tips.
  - Historical RC1 evidence covers the A11 Act 1 map/save-load spot check, `columns=8; rows=17`, saved-map boss-reachability from `(3,1)` to `(3,17)`, and Act 2/3 map-surface observation with `columns=8; rows=16`. Natural click-by-click traversal remains pending.
  - A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 when safe single-player map geometry is available.
  - A20 uses the vanilla double-boss path for Boss 2 creation/reveal, Boss 2 Branded Form metadata, Boss 1 recovery, one Boss card reward, narrow Boss 1 reward-screen intermission wording, and a fixed courtyard event before Boss 2. A full custom intermission screen remains deferred.
  - Startup/log checks do not satisfy live co-op verification. Historical normal Steam-client Mod Settings UI is covered by separate RC1 evidence.
- Remaining before private beta: manual feature matrix, save/load, multiplayer disposition, generated-art visual verification, clean commit, and pushed branch after validation.

## Pending manual checks
1. Run the manual/live gates for the latest package before final release handoff.
2. Check `godot.log` during private beta verification.
3. Old root-level BaseLib folder remains present; leave it untouched unless explicitly cleaning up later.
