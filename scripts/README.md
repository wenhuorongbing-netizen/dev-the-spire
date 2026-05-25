# Scripts

Repository helper scripts live here. Keep scripts small, idempotent where possible, and documented in this file when added.

| Script | Purpose |
| --- | --- |
| `audit-godot-log.ps1` | Scan `godot.log` for known loader/API drift/runtime failure patterns and emit a JSON-style audit summary. |
| `audit-ancient-art-assets.ps1` | Audit `docs/features/ancient-expansion-v2.2/art-asset-manifest.json` for missing target files, SHA256 drift, temporary/final-art status, duplicate bytes, missing `export_presets.cfg` coverage, and final-generated assets that do not record `GPTimage2`. Use `-FailOnHashMismatch`, `-FailOnMissingFinal`, `-FailOnMissingExport`, or `-FailOnInvalidGenerationMode` when a pass should fail on those conditions. |
| `bootstrap-windows.ps1` | Bootstrap local Windows setup for this workspace. Use with care because local paths and installed tools vary by machine. |
| `check-installed-spire-plus-package.ps1` | Preferred Windows checker and source of truth for installed Spire Plus compatibility artifact hashes, the copied game-root `SpirePlus` ZIP, the installed PCK's Sere Talon imported textures, and the English/Simplified Chinese Sere Talon / Tanx Claws content split against the current handoff docs. Defaults are Windows-oriented; pass `-ModDirectory` or `-GameRootZipPath` explicitly for non-default paths, or `-SkipGameRootZipCheck` when checking only an installed mod folder. |
| `check-installed-spire-plus-package.sh` | Preferred macOS/Linux checker and source of truth for installed Spire Plus compatibility artifact hashes plus the installed PCK's Sere Talon imported textures and English/Simplified Chinese Sere Talon / Tanx Claws content split with `shasum -a 256` or `sha256sum`. Pass the installed compatibility mod directory as the first argument. |
| `check-installed-ezmb-package.ps1` | Compatibility alias for older local commands. It forwards to `check-installed-spire-plus-package.ps1`; new tester-facing docs should use the Spire Plus script. |
| `check-installed-ezmb-package.sh` | Compatibility alias for older local commands. It forwards to `check-installed-spire-plus-package.sh`; new tester-facing docs should use the Spire Plus script. |
| `check-spire-window-preflight.ps1` | Report the current foreground window, Slay the Spire 2 window state, and visible top-level windows before capturing live gameplay screenshots. Use `-RequireSpireForeground` to fail fast when another app is covering the game. |
| `ci-full-validation.ps1` | Self-hosted Windows CI entry point for full no-game validation. Requires `STS2_PATH`/`GODOT_PATH`, writes a temporary ignored `Directory.Build.props` when needed, checks StS2 DLLs and BaseLib, then runs hygiene, build, tests, format, diff-check, publish, package, and opt-in artifact tests for the single Spire Plus mod. |
| `collect-ancient-ui-evidence.ps1` | Prepare or restore a forced Ancient clicked-UI evidence session for Urda, Morvi, Lotha, or Vakuu. Prepare mode writes `ancient-ui-evidence-plan.json`, `manual-instructions.md`, `command.txt`, `environment.json`, `package-hashes.json`, and a pending `manual-rows-template.json`; it runs the preflight unless `-NoPreflight` is used, and launches only when `-Launch` is explicit. |
| `collect-coop-evidence.ps1` | Prepare pending two-client co-op evidence templates for host/client logs, A11-A20 selection, Ancient sync, Root Eyes, Rootblight, save/reconnect, and preview-tool disposition. It never proves co-op by itself and does not auto-launch a two-client session. |
| `collect-preview-tools-evidence.ps1` | Prepare live-proof templates for the integrated Spire Plus preview tools: Crystal Sphere peek and deterministic transform preview. Run without `-Launch` to create pending templates only. |
| `collect-future-peek-evidence.ps1` | Compatibility wrapper for older local commands. It forwards to `collect-preview-tools-evidence.ps1`; current work should use the preview-tools name because these tools now ship inside Spire Plus. |
| `collect-vakuu-fight-evidence.ps1` | Prepare focused pending evidence templates for the hidden Vakuu fight: dedicated scene/monster, Contract turns, locks/Blood Debt, victory return/no-black-screen, non-Vakuu rewards, failure/death, and active/prefinished save-load. Launches only with explicit `-Launch`. |
| `capture-spire-window.ps1` | Capture the visible Slay the Spire 2 window to a PNG. Use with `-RequireSpireForeground` after the preflight passes. |
| `generate-patch-inventory.ps1` | Scan active C# source for Harmony patches and refresh `docs/patch-inventory.md` with owner/risk labels. Use `-Check` to fail when the inventory is stale. |
| `invoke-ancient-art-gpt4free.ps1` | Build one Ancient art request from `art-generation-prompts.md` and `art-asset-manifest.json`, force `generation_mode`, `mode`, and `semantic_model` to `GPTimage2`, map the local g4f transport model to `gpt-image` by default, and POST it to `GPT4FREE_IMAGE_ENDPOINT`. Without an endpoint, it writes a dry-run request JSON only. |
| `package-spire-plus.ps1` | Build the player-facing `publish\SpirePlus-v...zip` from the installed compatibility artifacts. The visible package name is Spire Plus; the inner compatibility folder, manifest id, DLL, and PCK are still named `EZMicroBalance`. Run after `dotnet publish`; use `-NoRefreshFromInstalled` only to re-zip an already refreshed staging folder. The no-refresh path still requires the staged DLL, manifest, and PCK to exist. |
| `prepare-current-manual-test-handoff.ps1` | Rebuild the full no-launch manual-test handoff folder in one command. It creates release, Ancient UI, Vakuu fight, preview-tools, and co-op evidence templates, then confirms the pending release manifest fails closed unless `-SkipPendingVerifier` is used. |
| `report-worktree-batches.ps1` | Read `git status --short` and classify dirty paths into the review batches from `docs/month-plan/commit-boundaries.md`. Use `-FailOnUnclassified` before staging or release handoff; add `-PathspecDirectory .tools\worktree-batches\current` to write per-batch pathspec files plus manifest `git add --pathspec-from-file=...` commands without staging anything. |
| `send-spire-dev-console-command.ps1` | Bring Slay the Spire 2 to the foreground and send one simple DevConsole command with `SendKeys`. Use only for short ASCII commands such as `spireplus_test_ancient URDA confirm`. |
| `spire-plus-live-session.ps1` | Prepare and restore a restore-safe normal Steam live-test session with only BaseLib and Spire Plus enabled. Use `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch` to create evidence state and launch, or add `-DisableSpirePlus` with `-MoveOtherMods` to temporarily isolate the compatibility mod folder for BaseLib-only plug-off comparison evidence. Then run `-Mode Restore -EvidenceDir <dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore` after copying screenshots/log notes from any session that starts or continues a run. |
| `validate-repository-hygiene.ps1` | CI-safe repository checks for manifest identity, JSON validity, removed duplicate mod roots, ignored website clutter, and fresh patch inventory. |
| `verify-spire-plus-release-evidence.ps1` | Check a filled manual release evidence manifest before any release-ready claim. It hashes the package under test, rejects duplicate row ids, wrong row kinds, manifests/evidence dirs outside the evidence root, file/screenshot paths that escape their evidence dir, and empty evidence files, warns on unknown or blank manifest rows, keeps each row's default evidence files required even when extra files are listed, checks current package hash parity, requires `command.txt` for every passed row, clicked-UI screenshots with foreground preflight, valid non-empty PNG evidence at least 800x450 by default, clean `godot-log-audit.json` files, non-empty evidence notes that do not describe invalid/non-counting evidence, filled Ancient reward relic, player text QA, art/resource routing, Vakuu victory/failure/save-load, Rootblight behavior, preview-tools, co-op disposition, and A19/A20 boss-ability checklists for those rows, fresh loader, save-load, A11/disable-mod rows, or explicit owner-approved deferrals where allowed. |

## Ancient UI evidence helper

Use `collect-ancient-ui-evidence.ps1` for live-pending clicked Ancient UI evidence. It creates a timestamped `.tools\runtime-evidence\ancient-ui-click-<ancient>-<timestamp>` folder by default, records the force environment variables and expected option counts, writes the release-verifier template files, and prints the exact launch command when run without `-Launch`.

Example:

```powershell
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient URDA -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient VAKUU -ForceVakuuFight -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Restore -EvidenceDir <evidence-dir>
```

The helper does not click the game UI, capture screenshots, audit a fresh log, or prove clicked UI by itself. Follow the generated `manual-instructions.md` and keep the clicked UI rows pending until the screenshot and log evidence exists.

Preferred main-menu UI-smoke command after launch:

```text
spireplus_test_ancient URDA confirm
spireplus_test_ancient MORVI confirm
spireplus_test_ancient LOTHA confirm
spireplus_test_ancient VAKUU confirm
spireplus_test_ancient VAKUU confirm fight
```

The Spire Plus command starts an unsaved single-player test run and refuses to run over an existing run. It is still only UI smoke; natural map routing, gameplay, save/load, and co-op rows need their own evidence.

Vakuu shows three normal options by default. Use `-ForceVakuuFight` only for the unfinished one-option fight smoke, or set `SPIREPLUS_ENABLE_VAKUU_FIGHT=1` / legacy `EZMB_ENABLE_VAKUU_FIGHT=1` deliberately when you need the fourth unfinished fight option.

Do not put downloaded binaries or generated tool output in this folder. Use ignored local folders such as `.tools/`, `publish/`, or `source code/` for machine-specific material.

## Release evidence verifier

`collect-release-evidence.ps1` writes pending manual rows with the same row ids and kinds required by `verify-spire-plus-release-evidence.ps1`, plus a verifier-readable `release-evidence-manifest.json`. Row folders include `*-checklist-template.md` references and directly editable `*-checklist.md` working files. The template is only a collection scaffold until live logs, screenshots, notes, and a verifier pass marker are added.

Use `prepare-current-manual-test-handoff.ps1` when you want the full current handoff folder for a manual test pass:

```powershell
.\scripts\prepare-current-manual-test-handoff.ps1 -EvidenceRoot .tools\runtime-evidence\manual-test-handoff-20260523-current
```

The generated handoff includes `TESTER_START_HERE.md`, `handoff-summary.json`, `release/`, `ancient-ui/`, `vakuu/`, `preview-tools/`, and `coop/`. `TESTER_START_HERE.md` repeats the package path and ZIP SHA256 from the generated manifest and prints the verifier command for the actual evidence root used in that run. `handoff-summary.json` records the expected no-launch verifier result: `PendingVerifierRequiredRowCount` is 20, `PendingVerifierFailureCount` is 20, and `PendingVerifierWarningCount` is 0. Release row folders include `*-checklist-template.md` references plus directly editable `*-checklist.md` working files. It does not launch the game and does not mark rows as passed.

Use `verify-spire-plus-release-evidence.ps1` after manual testing has produced evidence folders. You can either run `collect-release-evidence.ps1` to create the evidence folder, verifier-readable manifest, and one subfolder per required row, or use `-WriteTemplate` to create only the manifest. Fill every row with `Status`, `EvidenceDir`, screenshots, result notes, and any owner-approved deferrals:

```powershell
.\scripts\collect-release-evidence.ps1 -NoLaunch
.\scripts\verify-spire-plus-release-evidence.ps1 -WriteTemplate
.\scripts\verify-spire-plus-release-evidence.ps1 -WritePassMarker
```

By default, deferred rows fail. Use `-AllowDeferred` only when the project owner has explicitly accepted a release-note deferral and the row has `ExplicitOwnerDecision: true` plus a non-empty `ReleaseNote`. Pass rows also fail on duplicate row ids, wrong row kinds, manifests or evidence dirs outside the evidence root, required-file or screenshot paths that escape the row evidence dir, empty required files, missing `command.txt`, empty, invalid, or undersized clicked-UI PNG screenshots, empty required Markdown note files, or notes that say the evidence is invalid, covered, main-menu-only, loader-health-only, or otherwise not counted. Default evidence files cannot be removed from a row; `RequiredFiles` only adds extra files. Unknown or blank manifest rows are ignored but reported in `Warnings`.
The verifier also computes the SHA256 of `publish\SpirePlus-v0.1.0-private-beta.10.zip` by default; use `-PackagePath` only when intentionally auditing a different zip. `-WritePassMarker` writes `release-evidence-verifier-pass.json` only after all rows pass or are accepted deferrals.

## Full local CI lane

Use `ci-full-validation.ps1` directly on a machine with Slay the Spire 2, BaseLib, and Godot installed, or through `.github/workflows/full-local-validation.yml` on a self-hosted Windows runner.

```powershell
$env:STS2_PATH='D:\Steam\steamapps\common\Slay the Spire 2'
$env:GODOT_PATH='D:\Game\FOTN\dev-the-spire\.tools\godot-4.5.1-mono\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe'
.\scripts\ci-full-validation.ps1
```

This lane does not launch the game and does not satisfy live/manual rows. It only proves the local source, package, and artifact checks on a runner that has the required game dependencies.
