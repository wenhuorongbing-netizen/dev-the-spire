# Development Environment

## 2026-06-18 Current Override

- Local `Directory.Build.props` points `Sts2Path` at `E:\Steam\steamapps\common\Slay the Spire 2`.
- The installed game `release_info.json` currently reports Slay the Spire 2 `v0.107.0`, commit `23d60b98`, date `2026-06-04T18:40:52-04:00`.
- Installed `STS2-RitsuLib` `v0.4.16` has runtime variants `0.103.2`, `0.106.1`, and `0.107.0`; the prior `v0.3.10` install was backed up to `%TEMP%\codex-ritsulib-backup-20260610-090338`.
- The recovered local source snapshot and prior clean `v0.106.1` loader evidence remain historical. Current `v0.107.0` beta.86 AdditiveBatch1 direct smoke is clean under `.tools\runtime-evidence\v01070-beta86-additive-batch1-direct-20260618-031254`: main menu reached, BaseLib/RitsuLib loaded, RitsuLib selected compat branch `0.107.0`, Spire Plus applied 25/25 ModPatcher patches, 10 event types registered through 14 calls, and `godot-log-audit.json` is clean.
- Current refactor/runtime pass evidence: solution build and publish passed with 0 errors and 0 warnings, installed package checker passed for beta.86, runtime preflight passed 27 / 0, and the fresh loader smoke above closed the AdditiveBatch1 package/source-shape blocker. Earlier cross-thread aborts are runner-contamination evidence only.

Historical environment rows below are retained for setup history. Prefer this override and `PROJECT_STATE.md` for current status.

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
- Observed installed game version: `v0.106.1` from `D:\Steam\steamapps\common\Slay the Spire 2\release_info.json` on 2026-05-22.
- Observed installed game metadata: commit `cb2fbf47`, branch `v0.106.1`, build date `2026-05-21T16:17:40-07:00`, and `main_assembly_hash` `1001788235`.
- Steam appmanifest evidence: public-beta `BetaKey`, build id `23353684`, installed locally on 2026-05-22.
- Local source snapshot: `source code/` was cleaned and recovered from the installed `SlayTheSpire2.pck` plus `data_sts2_windows_x86_64\sts2.dll` on 2026-05-22 using GDRE Tools `v2.5.0-beta.5`.
- Source recovery evidence: `source code\release_info.json` matches `v0.106.1`; `source code/src/Core` contains 3,393 C# files; `source code/sts2.sln` builds with 0 warnings and 0 errors.
- Source recovery log: `source code/gdre_export.log` decompiled 3,463 scripts, reported 18 failed scripts, converted 3,937 resources, and logged one debug-scene parse warning for `scenes/debug/back_confirm_example.tscn`.
- API-diff artifacts live under `.tools\source-refresh-v0.106.1-20260522\`; the canonical Core signature diff found 1,712 removed signatures and 1,835 added signatures compared with the previous snapshot.
- Do not use pre-2026-05-22 `v0.105.x` source notes as the sole basis for current `v0.106.1` conclusions. Reinspect the refreshed source and confirm runtime behavior before making release claims.
- Test environment must be ONLY BaseLib + Spire Plus. The Spire Plus technical folder/id is `EZMicroBalance`. Earlier multi-mod logs including DamageMeter, RouteSuggest, AnimeWaifuSilent, and similar local mods are invalid release evidence.

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
- Legacy installed files under `D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\` are historical scaffold artifacts, not current deliverables.
- Target DLL path: `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll`
- Target PCK path: `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.pck`
- Target JSON path: `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.json`

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

## Runtime evidence summary
- BaseLib `v3.1.4` compatibility with `v0.106.1` is the historical validated target. Historical 22-field loader evidence: `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` loaded only BaseLib plus Spire Plus, registered config, reported `Found 22 SavedSpireFields`, reached main menu, and had no release-blocking signatures. This is historical evidence only.
- Current source defines 30 SavedSpireFields. Historical 22-field and 16-field startup rows remain useful records only; beta.86 AdditiveBatch1 loader parity has fresh live evidence, while gameplay proof still needs manual verification.
- Historical beta.19 loader evidence: `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` loaded only BaseLib plus Spire Plus, registered `EZMicroBalance`, reported `v0.1.0-private-beta.19` and `Found 30 SavedSpireFields`, reached startup completion, matched its beta.19 package hash, audited clean, stopped the game, and restored the isolated mod setup.
- Current display-name list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` shows `Spire Plus`; older page-level Mod Settings evidence predates the display-name refresh.
- Historical package resource evidence under `.tools\runtime-evidence\current-package-smoke-20260514-015901` verified an earlier 22-field package and headless installed-PCK loading for Urda/Morvi/Lotha scenes plus 43 Ancient textures. This is resource-load evidence, not clicked live Ancient UI proof.
- Helper evidence: `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` loaded BaseLib plus Spire Plus, copied/audited `godot.log`, and restored settings plus moved mods. This is loader/helper evidence only; gameplay/manual gates remain pending.
- Plug-off loader evidence: `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` loaded `Loaded 1 mods (1 total)` with BaseLib only after temporarily isolating `EZMicroBalance`; the earlier settings-only disabled attempt remains invalid. This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.
- `scripts/spire-plus-live-session.ps1` preserves test-created `current_run*` files before restoring the user's original current-run files; `live-helper-preserve-current-run-smoke-20260513-133431` and `window-preflight-smoke-20260513-135402` cover helper restore and foreground preflight behavior.
- Detailed 2026-05-05 through 2026-05-14 runtime attempt history was archived to `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`; historical RC1 live notes remain in `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`.
- Historical RitsuLib diagnostic Off, CanaryOnly, and AdditiveBatch1 loader gates have clean `v0.106.1` evidence with BaseLib, RitsuLib, and Spire Plus loaded and 25/25 Spire Plus ModPatcher patches applied. Beta.85 Off and CanaryOnly `v0.107.0` proof remains previous-package loader context. Current beta.86 AdditiveBatch1 proof passed retained verifiers with 10 event types / 14 registration calls and exact tuple parity. Live gameplay, save-load, failure/death-path, clicked Ancient UI, preview-tools, and co-op verification remain pending.

## Last known commands
- Last attempted build: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` on 2026-06-18 during the beta.86 package/source alignment pass. Result: succeeded with 0 warnings and 0 errors.
- Last successful build: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` on 2026-06-18 during the beta.86 package/source alignment pass. Result: succeeded with 0 warnings and 0 errors.
- Last successful test-project run: `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName!~ReleaseEvidenceGateTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1` on 2026-06-18 during beta.86 post-doc/test reconciliation. Result: passed, 480 passed, 39 skipped release artifact/runtime/local-source tests, 0 failed, 519 total.
- Latest rebuilt split no-build test lanes on 2026-06-18 after the beta.86 doc/website guard refresh: isolated `ReleaseEvidenceGateTests` passed 9 passed, 0 skipped, 0 failed, 9 total; the complementary no-build test-project lane excluding `ReleaseEvidenceGateTests` passed 480 passed, 39 skipped release artifact/runtime/local-source tests, 0 failed, 519 total.
- Latest compactness/current-claim validation on 2026-06-18: focused documentation/governance guard lane passed 59 / 0 / 0 / 59, current-doc claims passed 956 / 0, runtime preflight passed 27 / 0, v19 gate ledger passed 534 / 0, static suite passed 15 / 0, and format/diff-check/patch-inventory/batch-classifier checks passed after the final doc/website recap edits.
- Last successful Release test run: `dotnet test EZMicroBalance.sln -c Release` on 2026-05-13 after the BaseLib-only plug-off startup/log refresh. Result: passed, 81 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- Latest opt-in artifact-validation run: `STS2_PATH=E:\Steam\steamapps\common\Slay the Spire 2` plus `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` passed on 2026-06-18 after the beta.86 package refresh, with 67 passed, 0 failed, 2 skipped, 69 total. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- Latest full local CI-script run: `scripts\ci-full-validation.ps1` on 2026-05-20 with explicit `STS2_PATH` and `GODOT_PATH`. Result: passed. This is local no-game validation, not live loader or gameplay proof. As of 2026-06-17, the script defaults to the split `ReleaseEvidenceGateTests` strategy, but that updated full script has not been rerun end-to-end with publish/package.
- Local game-source API shape tests are opt-in with `SPIREPLUS_RUN_LOCAL_SOURCE_GUARDS=1` because `source code/` is ignored local material. Normal test lanes skip those checks; use the opt-in lane only after refreshing `source code/src/Core/**` from the current local game version, or set `SPIREPLUS_LOCAL_GAME_SOURCE_ROOT` to the refreshed decompiled source root.
- Last formatting check: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` on 2026-06-17 during the validation-lane helper continuation. Result: exit code 0.
- Last required diff check: `git diff --check` on 2026-06-17 during the validation-lane helper continuation. Result: exit code 0 with no whitespace errors; PowerShell/Git reported existing CRLF normalization warnings for `docs/goals/refactor.md` and `scripts/check-sts1-event-current-doc-claims.ps1`.
- Last attempted default publish: `dotnet publish EZMicroBalance.sln -m:1` on 2026-06-18 after the beta.86 package/source alignment pass. Result: succeeded against the real installed mods root. The attempted `dotnet publish EZMicroBalance.sln -m:1 --no-incremental` command failed because solution-level publish does not accept `--no-incremental`; the successful rerun omitted that switch.
- Last successful isolated publish: `dotnet publish EZMicroBalance.sln -p:ModsPath=.tools\publish-game-root\mods\` on 2026-05-27 after the beta.84 Urda Seedbed Harmony patch bugfix. Result: succeeded against an isolated temporary mods root; the isolated root is tooling context only and is not the current package-parity source.
- Publish/package note: package staging, the versioned package folder, `publish\SpirePlus-v0.1.0-private-beta.86.zip`, `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and `E:\Steam\steamapps\common\Slay the Spire 2\SpirePlus-v0.1.0-private-beta.86.zip` were refreshed and hash-checked on 2026-06-18 for local manual testing.
  The zip uses the player-facing `SpirePlus` archive name while the install folder remains `EZMicroBalance`.
  The current `v0.107.0` beta.86 AdditiveBatch1 direct loader row is clean under `.tools\runtime-evidence\v01070-beta86-additive-batch1-direct-20260618-031254`, with 10 event types / 14 registration calls and exact tuple parity.
  The beta.85 Off and CanaryOnly loader rows remain previous-package context under `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510` and `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621`.
  Live gameplay, save-load, failure/death-path, clicked Ancient UI, and co-op verification remain pending.
- Last PCK hash check: the 2026-06-18 local package-hash refresh found the staging, versioned, and zip-entry PCK at SHA256 `C5619646CEB02FC1D611554EC689CD2F9C81518BED9B6D5CB4CDCE90AED63F75`.
- Last staging/versioned DLL hash check: SHA256 `B89D89A502BB98950EEAE3E101559FA3E5BA74BFF264BA5D59D43A70A4268EAD`.
  Detailed pass history lives in `docs/review.md` and `docs/archive/**`.
- Last Harmony patch audit: standalone .NET 9 audit called `Harmony.PatchAll(...)` on `EZMicroBalance.dll` and returned `PatchAll OK`.
- Last private beta package: `publish\SpirePlus-v0.1.0-private-beta.86.zip` was rebuilt from the real installed mod artifacts, copied to the game root, and synced into the real installed mod folder on 2026-06-18. The staging, versioned, installed, game-root zip, and zip-entry artifacts match the hashes below.
  - Package note: `README_INSTALL.txt` is a short manual-test install note and says Ancient selections grant visible marker relics.
  - Zip SHA256: `3EDA50CCF8E2ECD49DCF1F6B4CEE7B7E3DE604793E8059253179914834781FFE`
  - DLL SHA256: `B89D89A502BB98950EEAE3E101559FA3E5BA74BFF264BA5D59D43A70A4268EAD`
  - README SHA256: `65293B1557BEBEE42E4DE1BBF162B23414CC436E6BBF748682D74299C356265D`
  - Manifest SHA256: `ABD6AEAFCF73F7CF74E31D01D4EBD17C667F36B4969724666DFDFC42997AD17E`
  - PCK SHA256: `C5619646CEB02FC1D611554EC689CD2F9C81518BED9B6D5CB4CDCE90AED63F75`
  - Entries: `EZMicroBalance/EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt`.
- Last release art audit: `EZMicroBalance/mod_image.png` and `publish\EZMicroBalance-cover-source.png` currently have SHA256 `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75` and were manually checked as original generated art with no visible text, letters, numbers, numerals, logos, or official game assets.
- Publish note: Headless Godot export requires `export_presets.cfg` to be UTF-8 without BOM and needs local runtime references available to the editor assembly scan. `EZMicroBalance.csproj` copies `sts2.dll`, `0Harmony.dll`, and BaseLib into the Godot temp build folders before export; the selected-resource PCK still contains only active mod resources.
- Local tools note: Godot 4.5.1 mono was restored under `.tools\godot-4.5.1-mono` after the cleanup pass so `dotnet publish` remains usable. Raw local `.tools` runtime-evidence folders were pruned; historical evidence entries in this document are records of prior runs, not proof that those ignored raw folders are currently present.

## Manual game verification
- Manual game verification succeeded: legacy baseline only.
- Current verification surface: Slay the Spire 2 Settings -> Mod Settings.
- Current target Mod Settings verification: list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` shows `Spire Plus` in the Mods list with only BaseLib and `EZMicroBalance` enabled.
- Historical startup/log evidence under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` logged `v0.1.0-private-beta.19`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 30 SavedSpireFields`, startup completion, beta.19 package hash parity, and 0 release-blocking hits.
- Earlier package startup/log evidence under `.tools\runtime-evidence\current-package-smoke-20260514-015901` is historical 22-field loader/resource evidence.
- Historical page-level UI evidence remains under `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-*` with the old display name.
- Legacy `EzDailyContent` Mod Settings evidence on `v0.104.0` is historical scaffold evidence and not current Spire Plus readiness proof.
- Beta.43 loader and live gameplay verification remain pending.

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
  - Historical RC1 evidence covers the A11 Act 1 map/save-load spot check, `columns=8; rows=17`, saved-map boss-reachability from `(3,1)` to boss `(3,17)`, and Act 2/3 map-surface observation with `columns=8; rows=16`. Natural click-by-click traversal remains pending.
  - A17 inserts one optional 3-4 node Deep Branch in Acts 2/3 when safe single-player map geometry is available.
  - A20 uses the vanilla double-boss path for Boss 2 creation/reveal, Boss 2 Branded Form metadata, Boss 1 recovery, one Boss card reward, narrow Boss 1 reward-screen intermission wording, and a fixed courtyard event before Boss 2. A full custom intermission screen remains deferred.
  - Startup/log checks do not satisfy live co-op verification. Historical normal Steam-client Mod Settings UI is covered by separate RC1 evidence.
- Remaining before private beta: manual feature matrix, save/load, multiplayer disposition, generated-art visual verification, clean commit, and pushed branch after validation.

## Pending manual checks
1. Run the manual/live gates for the latest package before final release handoff.
2. Check `godot.log` during private beta verification.
3. Old root-level BaseLib folder remains present; leave it untouched unless explicitly cleaning up later.
