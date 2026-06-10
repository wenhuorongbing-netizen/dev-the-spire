# M5 Revision L Runtime Hard-Blocker

Date: 2026-06-10

## Verdict

The previous "missing RitsuLib folder" blocker is closed for local owner-review purposes: `STS2-RitsuLib` `v0.4.16`, BaseLib `v3.1.4`, and the `EZMicroBalance` install folder are present on the E-drive game root.

Fresh runtime proof is still blocked. The installed game root is now Slay the Spire 2 `v0.107.0`, and the installed RitsuLib package includes `lib\0.107.0`. The installed Spire Plus DLL now matches the packaged beta.84 DLL hash, but the current package-parity Off smoke is non-clean: beta.84 still fails Spire Plus initialization against `v0.107.0` because it contains stale patch targets from before the dirty-source installed-game API fixes. Do not use the historical `v0.106.1` loader smokes or the red beta.84 smoke as current runtime proof.

## Evidence Boundary

| Evidence | Status |
|---|---|
| Installed game version | `v0.107.0` from `E:\Steam\steamapps\common\Slay the Spire 2\release_info.json` |
| RitsuLib folder | Present: `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` |
| RitsuLib variant DLL | Present: `lib\0.107.0\STS2-RitsuLib.dll` from installed `STS2-RitsuLib` `v0.4.16` |
| Installed Spire Plus DLL | Present, SHA256 `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766` |
| Packaged beta.84 DLL | Present, SHA256 `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766` |
| Historical Off diagnostic smoke | PASS: main menu, clean audit, Sts1Events disabled with 0 registrations |
| Historical CanaryOnly diagnostic smoke | PASS: main menu, clean audit, exactly 4 canary registrations |
| Historical AdditiveBatch1 diagnostic smoke | PASS: main menu, clean audit, 10 event types through 11 registration calls |
| Current `v0.107.0` package-parity Off launch | FAIL: `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`; audit `Clean=false`, 11 Godot ERROR lines, 1 Spire Plus error/exception |
| Current dirty-source gameplay proof | Not run |

## Still Blocking Release Claims

- Current `v0.107.0` beta.84 loader evidence is red, not clean.
- Spire Plus beta.84 fails initialization with `EctoplasmGoldGatePatch::Prefix(...)` undefined target method.
- The dirty source has the installed-game API compatibility fix, but no new versioned package has been cut from it.
- Current live `godot.log` cannot be used for clean proof until the environment is isolated and recaptured.
- No versioned package handoff from this dirty source.
- No event encounter screenshots.
- No save-load proof.
- No image/render proof.
- No replacement functional proof.
- No multiplayer fail-closed proof.
- No independent QA rerun.

## Decision

Record RitsuLib as runtime-loader validated only for the historical `v0.106.1` diagnostic evidence. Keep `Spire Plus` as not live-ready and not release-ready until a fixed versioned package has clean `v0.107.0` loader smoke and the manual/runtime proof rows are completed.
