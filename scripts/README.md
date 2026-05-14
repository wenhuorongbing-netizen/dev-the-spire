# Scripts

Repository helper scripts live here. Keep scripts small, idempotent where possible, and documented in this file when added.

| Script | Purpose |
| --- | --- |
| `audit-godot-log.ps1` | Scan `godot.log` for known loader/API drift/runtime failure patterns and emit a JSON-style audit summary. |
| `audit-ancient-art-assets.ps1` | Audit `docs/features/ancient-expansion-v2.2/art-asset-manifest.json` for missing target files, SHA256 drift, temporary/final-art status, duplicate bytes, missing `export_presets.cfg` coverage, and final-generated assets that do not record `GPTimage2`. Use `-FailOnHashMismatch`, `-FailOnMissingFinal`, `-FailOnMissingExport`, or `-FailOnInvalidGenerationMode` when a pass should fail on those conditions. |
| `bootstrap-windows.ps1` | Bootstrap local Windows setup for this workspace. Use with care because local paths and installed tools vary by machine. |
| `check-installed-ezmb-package.ps1` | Check installed `EZMicroBalance` artifact hashes against the current handoff docs. Defaults are Windows-oriented; pass `-InstallRoot` explicitly for non-default or macOS paths. |
| `check-spire-window-preflight.ps1` | Report the current foreground window, Slay the Spire 2 window state, and visible top-level windows before capturing live gameplay screenshots. Use `-RequireSpireForeground` to fail fast when another app is covering the game. |
| `collect-ancient-ui-evidence.ps1` | Prepare or restore a forced Ancient clicked-UI evidence session for Urda, Morvi, Lotha, or Vakuu. Prepare mode writes `ancient-ui-evidence-plan.json` and `manual-instructions.md`, runs the preflight unless `-NoPreflight` is used, and launches only when `-Launch` is explicit. |
| `invoke-ancient-art-gpt4free.ps1` | Build one Ancient art request from `art-generation-prompts.md` and `art-asset-manifest.json`, force `generation_mode`, `mode`, and `semantic_model` to `GPTimage2`, map the local g4f transport model to `gpt-image` by default, and POST it to `GPT4FREE_IMAGE_ENDPOINT`. Without an endpoint, it writes a dry-run request JSON only. |
| `package-spire-plus.ps1` | Build the player-facing `publish\SpirePlus-v...zip` from the installed `EZMicroBalance` artifacts while keeping the inner install folder, manifest id, DLL, and PCK named `EZMicroBalance`. Run after `dotnet publish`; use `-NoRefreshFromInstalled` only to re-zip an already refreshed staging folder. |
| `spire-plus-live-session.ps1` | Prepare and restore a restore-safe normal Steam live-test session with only BaseLib and Spire Plus / `EZMicroBalance` enabled. Use `-Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch` to create evidence state and launch, or add `-DisableSpirePlus` with `-MoveOtherMods` to temporarily isolate `EZMicroBalance` out of the mods folder for BaseLib-only plug-off comparison evidence. Then run `-Mode Restore -EvidenceDir <dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore` after copying screenshots/log notes from any session that starts or continues a run. |

## Ancient UI evidence helper

Use `collect-ancient-ui-evidence.ps1` for live-pending clicked Ancient UI evidence. It creates a timestamped `.tools\runtime-evidence\ancient-ui-click-<ancient>-<timestamp>` folder by default, records the force environment variables and expected option counts, and prints the exact launch command when run without `-Launch`.

Example:

```powershell
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient URDA -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Prepare -Ancient VAKUU -ForceVakuuFight -MoveOtherMods -MoveCurrentRuns
.\scripts\collect-ancient-ui-evidence.ps1 -Mode Restore -EvidenceDir <evidence-dir>
```

The helper does not click the game UI, capture screenshots, audit a fresh log, or prove clicked UI by itself. Follow the generated `manual-instructions.md` and keep the clicked UI rows pending until the screenshot and log evidence exists.

Do not put downloaded binaries or generated tool output in this folder. Use ignored local folders such as `.tools/`, `publish/`, or `source code/` for machine-specific material.
