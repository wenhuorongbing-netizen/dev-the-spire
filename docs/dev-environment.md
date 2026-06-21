# Development Environment

## 2026-06-21 Current Override

- Local `Directory.Build.props` points `Sts2Path` at `E:\Steam\steamapps\common\Slay the Spire 2`.
- The installed game `release_info.json` currently reports Slay the Spire 2 `v0.107.1`, commit `59260271`, date `2026-06-18T15:43:56-07:00`.
- Installed `STS2-RitsuLib` `v0.4.31` has runtime variant `0.107.1`; previous package `v3.3.0` remains installed only as previous-package/other-mod local context after backing up the previous previous package `v3.2.1` install to `%TEMP%\codex-sts2-previous-package-backup-20260619-102852`.
- The recovered local source snapshot has been refreshed from the installed `v0.107.1` game PCK and `sts2.dll` using GDRE Tools `v2.5.0`. `scripts\check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot -ExpectedPackageVersion v0.1.0-private-beta.96 -ExpectedRitsuLibVersion 0.4.31 -ExpectedRitsuCompatBranch 0.107.1 -FailOnMismatch` passed 59 checks / 0 mismatches; retained warnings are the GDRE 18 failed scripts and one debug-scene parse warning, not source identity mismatches. Retained `v0.107.0` beta.85/beta.86/beta.87 direct smokes remain previous-package/game-version loader context.
  The first `v0.107.1` beta.87 recapture at `.tools\runtime-evidence\v01071-beta87-additive-batch1-direct-20260619-102309` reached main menu and matched AdditiveBatch1 registration shape, but failed clean-loader proof because previous package `v3.2.1` logged 2 patch failures against `v0.107.1`.
  The beta.88 previous package `v3.3.0` recapture at `.tools\runtime-evidence\v01071-beta88-previous-package330-additive-batch1-direct-cleanlog-20260619-103937` reached main menu on `v0.107.1`, loaded RitsuLib/Spire Plus, selected RitsuLib compat branch `0.107.0`, applied 25/25 Spire Plus patches, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verification with 0 mismatches.
- Current RitsuLib-only migration evidence: solution build and publish passed during the beta.96 package pass, installed package checker targets beta.96 after hash-doc refresh, runtime preflight passed for STS2-RitsuLib `v0.4.31`, source-workspace checker passed for beta.96, and the retained beta.88/beta.90/beta.93 smokes are previous-package contexts only. The direct-smoke verifier now accepts both append-style current-log slices and Godot-rewritten logs only when `godot.log.current-iteration` matches the retained after-launch log. Earlier cross-thread aborts are runner-contamination evidence only.

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
- Game root: `E:\Steam\steamapps\common\Slay the Spire 2`
- Mod folder: `E:\Steam\steamapps\common\Slay the Spire 2\mods`
- Current branch target: public beta
- Observed installed game version: `v0.107.1` from `E:\Steam\steamapps\common\Slay the Spire 2\release_info.json` on 2026-06-20.
- Observed installed game metadata: commit `59260271`, branch `v0.107.1`, build date `2026-06-18T15:43:56-07:00`, and `main_assembly_hash` `-1555940892`.
- Historical Steam appmanifest evidence: public-beta `BetaKey`, build id `23353684`, installed locally on 2026-05-22 for the older `v0.106.1` setup.
- Local source snapshot: `source code/` was refreshed from the installed `SlayTheSpire2.pck` plus `data_sts2_windows_x86_64\sts2.dll` on 2026-06-20 using GDRE Tools `v2.5.0`.
- Source recovery evidence: the retained ignored `source code/` recovery is current for the installed `v0.107.1` local game. It includes `source code\release_info.json` matching installed commit `59260271`, branch `v0.107.1`, and main assembly hash `-1555940892`; `source code\gdre_export.log` records GDRE Tools `v2.5.0` recovery from `E:\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck` plus `data_sts2_windows_x86_64\sts2.dll`.
- Source recovery log: `source code/gdre_export.log` extracted 15,658 files, decompiled 3,473 scripts, reported 18 failed scripts, converted 3,949 resources, and logged one debug-scene parse warning for `scenes/debug/back_confirm_example.tscn`; `source code/sts2.sln` built with 0 warnings and 0 errors during recovery.
- Historical API-diff artifacts live under `.tools\source-refresh-v0.106.1-20260522\`; the canonical Core signature diff found 1,712 removed signatures and 1,835 added signatures compared with the previous snapshot.
- Do not use pre-2026-06-20 `v0.106.1` or older source notes as the sole basis for current `v0.107.1` conclusions. Reinspect the refreshed source and confirm runtime behavior before making release claims.
- Test environment must be ONLY STS2-RitsuLib + Spire Plus for current RitsuLib-only validation. The Spire Plus technical folder/id is `EZMicroBalance`. Earlier multi-mod logs including DamageMeter, RouteSuggest, AnimeWaifuSilent, and similar local mods are invalid release evidence.

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
- Current Spire Plus runtime framework dependency: `STS2-RitsuLib` `v0.4.31` under `<GameRoot>\mods\STS2-RitsuLib`.
- Current Spire Plus project package dependency: no `previous package` package reference.
- previous package local runtime status: still installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\previous package` for historical/other-mod context only; it is no longer a Spire Plus project, manifest, or current package dependency.
- previous package old root-level path still present: `D:\Steam\steamapps\common\Slay the Spire 2\previous package`
- previous package old root-level version: `v0.1.3`

## Runtime evidence summary
- previous package `v3.2.1` compatibility with `v0.106.1` is the historical validated target. Historical 22-field loader evidence: `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` loaded only previous package plus Spire Plus, registered config, reported `Found 22 previous saved-state registrations`, reached main menu, and had no release-blocking signatures. This is historical evidence only.
- Current source defines 30 SavedAttachedState fields. Historical 22-field and 16-field startup rows remain useful records only. Beta.85/beta.86/beta.87 `v0.107.0`, beta.88 previous package `v0.107.1`, beta.90 RitsuLib-only loader rows, and beta.93 RitsuLib-only loader rows are previous-context evidence; beta.96 package parity is current RitsuLib-only evidence, and gameplay proof still needs manual verification.
- Historical beta.19 loader evidence: `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` loaded only previous package plus Spire Plus, registered `EZMicroBalance`, reported `v0.1.0-private-beta.19` and `Found 30 previous saved-state registrations`, reached startup completion, matched its beta.19 package hash, audited clean, stopped the game, and restored the isolated mod setup.
- Historical display-name list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` shows `Spire Plus`; older page-level Mod Settings evidence predates the display-name refresh. Fresh beta.96 RitsuLib Mod Settings page proof is captured under `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`.
- Historical package resource evidence under `.tools\runtime-evidence\current-package-smoke-20260514-015901` verified an earlier 22-field package and headless installed-PCK loading for Urda/Morvi/Lotha scenes plus 43 Ancient textures. This is resource-load evidence, not clicked live Ancient UI proof.
- Historical helper evidence: `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` loaded previous package plus Spire Plus, copied/audited `godot.log`, and restored settings plus moved mods. This is loader/helper evidence only; gameplay/manual gates remain pending.
- Historical previous package-only plug-off loader evidence: `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` loaded `Loaded 1 mods (1 total)` with previous package only after temporarily isolating `EZMicroBalance`; the earlier settings-only disabled attempt remains invalid. This is plug-off loader evidence only; disable-mod gameplay in an actual run remains pending.
- `scripts/spire-plus-live-session.ps1` preserves test-created `current_run*` files before restoring the user's original current-run files; `live-helper-preserve-current-run-smoke-20260513-133431` and `window-preflight-smoke-20260513-135402` cover helper restore and foreground preflight behavior.
- Detailed 2026-05-05 through 2026-05-14 runtime attempt history was archived to `docs/archive/implementation-records/dev-environment-runtime-smoke-history-20260526.md`; historical RC1 live notes remain in `docs/archive/implementation-records/rc1-live-validation-log-20260508-20260513.md`.
- Historical RitsuLib diagnostic Off, CanaryOnly, and AdditiveBatch1 loader gates have clean `v0.106.1` evidence with previous package, RitsuLib, and Spire Plus loaded and 25/25 Spire Plus ModPatcher patches applied. Retained beta.85/beta.86/beta.87 `v0.107.0` proof remains previous-package/game-version context, beta.88 `v0.107.1` AdditiveBatch1 proof is previous-package context, beta.90 is previous RitsuLib-only context, and beta.93 is previous-package RitsuLib-only context. Live gameplay, save-load, failure/death-path, clicked Ancient UI, preview-tools, and co-op verification remain pending.

## Last known commands
- Last attempted build: `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false` on 2026-06-21 during the RitsuLib integration-doc cleanup pass. Result: succeeded with 0 warnings and 0 errors.
- Last successful build: `dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false` on 2026-06-21 during the RitsuLib integration-doc cleanup pass. Result: succeeded with 0 warnings and 0 errors.
- Last successful test-project run: `dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~DocumentationCompactnessGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1` on 2026-06-21 during the RitsuLib integration-doc cleanup pass. Result: passed, 34 passed, 0 skipped, 0 failed, 34 total.
- Latest rebuilt split no-build test lanes on 2026-06-18 after the beta.86 doc/website guard refresh: isolated `ReleaseEvidenceGateTests` passed 9 passed, 0 skipped, 0 failed, 9 total; the complementary no-build test-project lane excluding `ReleaseEvidenceGateTests` passed 480 passed, 39 skipped release artifact/runtime/local-source tests, 0 failed, 519 total.
- Latest compactness/current-claim validation on 2026-06-21: focused documentation/source/release guard lane passed 74 passed / 0 failed / 4 skipped / 78 total, current-doc claims passed 1331 / 0, local source-workspace check passed 59 / 0 with two retained GDRE warnings, static-file hygiene passed 12 / 0, format/diff-check passed, and worktree batch classification reported 0 unclassified dirty entries before commit. This was documentation/source-truth alignment only and did not run runtime smoke, gameplay, QA, or handoff validation.
- Last successful Release test run: `dotnet test EZMicroBalance.sln -c Release` on 2026-05-13 after the previous package-only plug-off startup/log refresh. Result: passed, 81 passed, 18 skipped release artifact/runtime evidence tests, 0 failed.
- Latest opt-in artifact-validation run: `STS2_PATH=E:\Steam\steamapps\common\Slay the Spire 2` plus `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` passed on 2026-06-18 after the beta.87 package refresh, with 67 passed, 0 failed, 2 skipped, 69 total. Legacy `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` remains accepted.
- Latest full local CI-script run: `scripts\ci-full-validation.ps1` on 2026-05-20 with explicit `STS2_PATH` and `GODOT_PATH`. Result: passed. This is local no-game validation, not live loader or gameplay proof. As of 2026-06-17, the script defaults to the split `ReleaseEvidenceGateTests` strategy, but that updated full script has not been rerun end-to-end with publish/package.
- Local game-source API shape tests are opt-in with `SPIREPLUS_RUN_LOCAL_SOURCE_GUARDS=1` because `source code/` is ignored local material. Normal test lanes skip those checks; use the opt-in lane only after refreshing `source code/src/Core/**` from the current local game version, or set `SPIREPLUS_LOCAL_GAME_SOURCE_ROOT` to the refreshed decompiled source root.
- Last formatting check: `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore` on 2026-06-21 during the RitsuLib integration-doc cleanup pass. Result: exit code 0.
- Last required diff check: `git diff --check` on 2026-06-20 during the RitsuLib/source-snapshot documentation alignment pass. Result: exit code 0 with no whitespace errors; Git reported the existing CRLF normalization warning for `scripts/check-sts1-event-current-doc-claims.ps1`.
- Last attempted default publish: `dotnet publish EZMicroBalance.sln -m:1` on 2026-06-18 after the beta.86 package/source alignment pass. Result: succeeded against the real installed mods root. The attempted `dotnet publish EZMicroBalance.sln -m:1 --no-incremental` command failed because solution-level publish does not accept `--no-incremental`; the successful rerun omitted that switch.
- Last successful isolated publish: `dotnet publish EZMicroBalance.sln -p:ModsPath=.tools\publish-game-root\mods\` on 2026-05-27 after the beta.84 Urda Seedbed Harmony patch bugfix. Result: succeeded against an isolated temporary mods root; the isolated root is tooling context only and is not the current package-parity source.
- Publish/package note: package staging, the versioned package folder, `publish\SpirePlus-v0.1.0-private-beta.96.zip`, `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance`, and `E:\Steam\steamapps\common\Slay the Spire 2\SpirePlus-v0.1.0-private-beta.96.zip` were refreshed and hash-checked on 2026-06-21 for local manual testing.
  The zip uses the player-facing `SpirePlus` archive name while the install folder remains `EZMicroBalance`.
  Current loader context:
  - beta.96 package parity is current package context.
  - beta.93 RitsuLib-only Off proof is clean previous-package context under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-off-direct-20260621`.
  - The beta.93 proof loaded exactly `STS2-RitsuLib` and `EZMicroBalance`, audited clean, and passed Off packet verifier 43 / 0.
  - beta.93 RitsuLib-only AdditiveBatch1 proof is clean previous-package context under `.tools\runtime-evidence\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`, with 10 event types / 14 registration calls, enabled-mode verifier 31 / 0, and packet verifier 61 / 0.
  - Retained beta.87 direct AdditiveBatch1 proof is clean historical `v0.107.0` context with 10 event types / 14 calls.
  - beta.85/beta.86 Off and CanaryOnly rows remain previous-package loader context.
  - beta.88 AdditiveBatch1 is previous package loader/registration evidence only.
  The beta.85 Off and CanaryOnly loader rows remain previous-package context under `.tools\runtime-evidence\v01070-beta85-current-package-runtime-fix-20260611-0510` and `.tools\runtime-evidence\v01070-beta85-canary-20260617-233621`.
  Live gameplay, save-load, failure/death-path, clicked Ancient UI, and co-op verification remain pending.
- Last PCK hash check: the 2026-06-21 local package-hash refresh found the staging, versioned, and zip-entry PCK at SHA256 `549FD8B2A90B2AF74F8D6C591107F423588EFD868A61D1C901585E6FE188D20C`.
- Last staging/versioned DLL hash check: SHA256 `28D9AA1632B783CE34BC4D4174C5A84DEB26FD74947529656A71757BF660309F`.
  Detailed pass history lives in `docs/review.md` and `docs/archive/**`.
- Last Harmony patch audit: standalone .NET 9 audit called `Harmony.PatchAll(...)` on `EZMicroBalance.dll` and returned `PatchAll OK`.
- Last private beta package: `publish\SpirePlus-v0.1.0-private-beta.96.zip` was rebuilt from the real installed mod artifacts, copied to the game root, and synced into the real installed mod folder on 2026-06-21. The staging, versioned, installed, game-root zip, and zip-entry artifacts match the hashes below.
  - Package note: `README_INSTALL.txt` is a short manual-test install note and says Ancient selections grant visible marker relics.
  - Zip SHA256: `6E313D383E49B750E3C5809E92D7795CC5E196B5A7511707D2AB4357E24D4265`
  - DLL SHA256: `28D9AA1632B783CE34BC4D4174C5A84DEB26FD74947529656A71757BF660309F`
  - README SHA256: `C8171889B7B03E177CAC6428E4CCA3873BEEDB054180A10A7E6703DEBC72BDFE`
  - Manifest SHA256: `A752A38EFF068FDB75B629D4A0DC92153D115EFD76D369B406E3D7077E4E4593`
  - PCK SHA256: `549FD8B2A90B2AF74F8D6C591107F423588EFD868A61D1C901585E6FE188D20C`
  - Entries: `EZMicroBalance/EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt`.
- Last release art audit: `EZMicroBalance/mod_image.png` and `publish\EZMicroBalance-cover-source.png` currently have SHA256 `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75` and were manually checked as original generated art with no visible text, letters, numbers, numerals, logos, or official game assets.
- Publish note: Headless Godot export requires `export_presets.cfg` to be UTF-8 without BOM and needs local runtime references available to the editor assembly scan. `EZMicroBalance.csproj` copies `sts2.dll`, `0Harmony.dll`, and STS2-RitsuLib into the Godot temp build folders before export; the selected-resource PCK still contains only active mod resources.
- Local tools note: Godot 4.5.1 mono was restored under `.tools\godot-4.5.1-mono` after the cleanup pass so `dotnet publish` remains usable. Raw local `.tools` runtime-evidence folders were pruned; historical evidence entries in this document are records of prior runs, not proof that those ignored raw folders are currently present.

## Manual game verification
- Manual game verification succeeded: legacy baseline only.
- Current verification surface: Slay the Spire 2 Settings -> Mod Settings.
- Current target Mod Settings verification: beta.96 screenshot/page proof captured with only STS2-RitsuLib and `EZMicroBalance` enabled at `.tools\runtime-evidence\beta96-ritsulib-mod-settings-clicked-ui-20260621-160701`. Historical list evidence under `.tools\runtime-evidence\current-spire-plus-modsettings-20260513-111342` showed `Spire Plus` with previous package plus `EZMicroBalance` in an older package context.
- Historical startup/log evidence under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` logged `v0.1.0-private-beta.19`, `Finished mod initialization for 'Spire Plus' (EZMicroBalance)`, `Loaded 2 mods (2 total)`, `Found 30 previous saved-state registrations`, startup completion, beta.19 package hash parity, and 0 release-blocking hits.
- Earlier package startup/log evidence under `.tools\runtime-evidence\current-package-smoke-20260514-015901` is historical 22-field loader/resource evidence.
- Historical page-level UI evidence remains under `.tools\runtime-evidence\rc1-modsettings-page-20260508-095137-*` with the old display name.
- Legacy `EzDailyContent` Mod Settings evidence on `v0.104.0` is historical scaffold evidence and not current Spire Plus readiness proof.
- Beta.96 live gameplay verification remains pending.

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
3. Old root-level previous package folder remains present; leave it untouched unless explicitly cleaning up later.
