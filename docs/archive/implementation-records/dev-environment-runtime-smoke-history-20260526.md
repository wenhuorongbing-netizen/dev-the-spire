# Dev Environment Runtime Smoke History

Historical archive.

Archived on 2026-05-26 from `docs/dev-environment.md` so the active environment file can stay focused on current versions, latest package hashes, and pending manual gates.

## Scope

This record preserves older local smoke-test and runtime-helper notes. These rows are useful for debugging launch setup, helper-script behavior, and historical API drift, but they are not current beta.38 release or gameplay proof.

## 2026-05-05 Direct And Force-Steam-Off Attempts

- direct `SlayTheSpire2.exe` launch produced a fresh `godot.log` but failed before mod loading because Steamworks initialization reported `No appID found`.
- Direct launch with a temporary `steam_appid.txt` value `2868840` still failed before mod loading because Steamworks initialization reported `ConnectToGlobalUser failed`; the temporary file was removed after the attempt.
- `D:\Steam\steam.exe -applaunch 2868840` did not start a detectable `SlayTheSpire2` process within the bounded smoke-test window.
- Local source review found `--force-steam off` skips Steam initialization before startup. That path is useful for local smoke testing, but not a substitute for final Steam-client verification.
- Controlled `--force-steam off` runs with only `BaseLib` and `EZMicroBalance` enabled found both manifests, loaded BaseLib and the mod DLL/PCK, finished mod initialization, reached main menu, and restored the original profile settings.
- The first controlled smoke exposed invalid Harmony targets for `SealOfGoldMaxEnergyPatch` and `CrossbowOfferPatch`; both were later retargeted to `AbstractModel` hooks.
- Controlled disable smoke with BaseLib enabled and Spire Plus disabled skipped `EZMicroBalance`, did not load its DLL, reached main menu, and restored profile settings.

## 2026-05-06 To 2026-05-07 Early Ascension Startup Fixes

- A bounded `--force-steam off` smoke after Ascension integration initially exposed a startup `MissingMethodException` for `RootBudCombatHook`; the root cause was StS2 model database startup requiring parameterless constructors for concrete `AbstractModel` types.
- Follow-up controlled smokes temporarily enabled only `BaseLib` and `EZMicroBalance`, loaded then-current installed artifacts, registered 8 SavedSpireFields, reached main menu, and restored profile settings.
- Some logs still included unrelated local invalid-manifest errors from disabled local mods such as RouteSuggest and `sts2-heybox-support`; no Spire Plus startup exception was present in the relevant final controlled smoke.
- A later bounded smoke confirmed the prior `DuplicateModelException` / direct `RootRunHook(RunState)` constructor path was absent from the new `godot.log`.
- The A20 fixed-courtyard package smoke loaded exactly 2 mods, initialized BaseLib and Spire Plus, reached main menu, found no Spire Plus error/exception lines, and restored settings.

## 2026-05-08 To 2026-05-09 RC1 Evidence

- The first RC1 normal Steam-client probe reached main menu but included unrelated local invalid-manifest/dependency errors for discovered disabled mods; the isolated startup log superseded it for clean-log evidence.
- The clean RC1 isolated startup log at `.tools\runtime-evidence\rc1-normal-steam-clean-godot-20260508-090122.log` temporarily moved 23 non-BaseLib/Spire Plus mod entries, launched through Steam, reached main menu, discovered only BaseLib and `EZMicroBalance`, loaded 2 mods, applied BaseLib patches, initialized Spire Plus, reported `Found 13 SavedSpireFields`, and restored moved entries/settings.
- The RC1 Mod Settings recheck launched through Steam, opened the Mod Settings list and Spire Plus config page, captured screenshots, copied a `godot.log`, restored moved entries, and had 0 release-blocking signatures.
- The targeted A14 Rootblight Simplified Chinese hover/notice pass isolated 22 non-BaseLib/Spire Plus mods, selected A14, captured Rootblight I/II/III and Blight Sprout hovers, and verified the starter Rootblight-added notice. Combat-end and co-op behavior remained pending.
- The targeted A14 English hover/notice pass verified the English Rootblight notice and hover text, with settings/saves and moved mod entries restored.
- A separate BaseLib plus Spire Plus-only main-menu log audited clean, but Steam cloud rehydrated current-run files before startup, so it was clean startup evidence only.

## 2026-05-13 Runtime Helper And Plug-Off Evidence

- Bounded `--force-steam off` smoke under `.tools\runtime-evidence\current-package-smoke-20260513-044306` loaded BaseLib and Spire Plus, reported `Found 16 SavedSpireFields`, reached main menu, found no Spire Plus error signatures, and restored settings. The audit still had unrelated disabled local-mod manifest/name noise.
- Normal Steam-client isolated startup/log verification under `.tools\runtime-evidence\current-spire-plus-normal-steam-20260513-054241` moved 24 non-BaseLib/Spire Plus entries, enabled only BaseLib and `EZMicroBalance`, reached main menu, copied `godot.log`, restored settings and moved entries, and audited clean.
- A14 Rootblight generated-art hover probing exposed missing vanilla-derived Urda icon/run-history/background scene asset paths. Follow-up source/resource fixes switched Urda to BaseLib `CustomAncientModel` custom icon/background-scene paths and packaged the Urda background scene.
- `scripts/spire-plus-live-session.ps1` was added for repeatable normal Steam-client test sessions. No-launch helper checks verified settings restore, moved-mod restore, current-run isolation, and `-PreserveNewCurrentRunsOnRestore`.
- `scripts/check-spire-window-preflight.ps1` was added after invalid live screenshot attempts showed another foreground application covering Slay the Spire 2. The preflight can fail before screenshot evidence is collected.
- Helper-driven normal Steam startup/log validation under `.tools\runtime-evidence\live-spire-plus-session-20260513-125206` loaded only BaseLib plus Spire Plus, copied and audited `godot.log`, then stopped the game and restored settings/moved entries. This is loader/helper evidence only.
- The first `-DisableSpirePlus` normal Steam attempt under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-142835` was invalid because settings-only `is_enabled=false` still loaded Spire Plus. The helper was tightened so `-DisableSpirePlus` requires `-MoveOtherMods`.
- BaseLib-only plug-off normal Steam startup/log validation under `.tools\runtime-evidence\live-spire-plus-disabled-session-20260513-143020` moved 25 entries including `EZMicroBalance`, loaded 1 mod, did not initialize Spire Plus, audited clean, then restored settings, current-run saves, and moved mod entries. Disable-mod gameplay still remained pending.

## 2026-05-14 Historical Package Smoke

- Historical package smoke/log/resource verification under `.tools\runtime-evidence\current-package-smoke-20260514-015901` covered an earlier 22-field package, installed/staging/versioned/zip artifact parity, installed README sync, headless installed-PCK loading for Urda/Morvi/Lotha scenes, and 43 Ancient textures.
- Log signals included BaseLib patch success, config registered for `EZMicroBalance`, `Loaded 2 mods (2 total)`, `Found 22 SavedSpireFields`, `Time to main menu: 14,045ms`, and 0 release-blocking scan hits.
- Boundary: current source now defines 30 SavedSpireFields, so this is historical loader/log/resource evidence only. It does not prove current beta.38 loader parity or gameplay behavior.
