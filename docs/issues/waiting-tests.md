# Manual Verification Queue

This is the compact live/manual evidence queue for source-complete Spire Plus work. Full historical issue text was archived at `../archive/issues/waiting-tests-pre-slim-20260518.md`.

Close a row only when the manual evidence includes the requested screenshots, `godot.log` markers, host/client logs where relevant, and the exact package/build being tested. Move regressions back to `../issues.md` only when they need source work again.

## Current Queue

| ID | Area | Status | Manual proof needed |
| --- | --- | --- | --- |
| ROOTBLIGHT-CARD-TEXT | A14/A15/A18 cards | partial live proof | Broader combat behavior remains open after EN/ZHS hover proof. |
| CARD-TEXT-STYLE-GUIDE | localization/style | source-complete | Keep guard coverage while new cards/text are added. |
| ROOTBLIGHT-NOTICE | Rootblight notices | source-patched | Combat-end notice, Blight Sprout notice timing, non-paused notice behavior, and co-op ownership. |
| ROOTBLIGHT-ART | Rootblight visuals | packaged | In-game visual check for generated Rootblight and Blight Sprout portraits. |
| ROOTBLIGHT-DIAGNOSTICS | Rootblight diagnostics | partial live proof | Combat-end notice and co-op/desync checks. |
| MP-A11-A20-HP0-NEOW | multiplayer A11-A20 | diagnostics patched | Host/client co-op run-start logs proving whether HP0/Neow blocker is gone. |
| MP-SAVE-QUIT | multiplayer save/quit | source-investigated | Host/client save-quit propagation proof. |
| MP-RUN-START-BLACK-SCREEN | multiplayer startup | live-pending | Fresh BaseLib+Spire Plus host/client logs for run-start black screen. |
| MP-A20-BLACK-SCREEN | A20 multiplayer | source-patched | Co-op A20 retest, optional-boss load path, and logs. |
| ASCENSION-PUBLIC-SELECTION | A11-A20 selection | source-patched | Steam-client selection and co-op selection proof. |
| A11-MAP-LENGTH | A11 map | partial live proof | Natural traversal, boss reachability beyond spot checks, and save-load coverage. |
| A11-MAP-ANIMATION | A11 map UI | partial live proof | Player-visible animation/UI feedback review. |
| A12-TOOLTIP-COLORS | Firemarked Elite tooltip | source-patched | EN/ZHS hover rich-text proof. |
| A13-FISSION-RARITY | Fission rewards | diagnostics patched | Reward-screen sampling with eligible count and applied count. |
| ROOTBUD-ROOTBLIGHT-REWORK | Rootblight system | partial live proof | Full Rootbud/Rootblight behavior, combat-end notices, co-op ownership, generated-art visual check. |
| MP-A11-A20-SELECTION | multiplayer Ascension | source-patched | Live co-op A11-A20 selection proof. |
| A20-MP-WARNING | A20 multiplayer warning | source-patched | Co-op warning log proof. |
| LIVE-COOP-A11-A20-MATRIX | co-op matrix | pending | Full live co-op traversal matrix. |
| RUNTIME-ENV-POLLUTION | test environment | source-complete | Clean BaseLib+Spire Plus runtime evidence and package hash clarity. |
| A12-A16-MARKER-VARIETY | map markers | source-patched | Multi-seed and save/load marker variety proof. |
| A12-A16-A19-MAP-PREVIEW | map hover previews | source-patched | Firemark, Banner, dedicated ability, and Branded Form hover rendering proof. |
| A13-FISSION-SAMPLING | Fission diagnostics | source-patched | 20 normal, 10 Banner, 10 Firemarked Elite, and boss reward samples. |
| MP-MAC-MODELDB-HASH | cross-platform co-op | investigated | Host/Mac logs, release info, loaded-mod list, ModelDb hashes, and BaseLib/EZMB hash parity. |

## Execution Notes

- Test with only BaseLib and Spire Plus unless a row explicitly asks for polluted-environment comparison.
- Record package hash or installed DLL/PCK/manifest hashes with every evidence folder.
- For multiplayer rows, capture logs from host and client from the same attempt.
- For hover/UI rows, capture screenshots in English and Simplified Chinese when possible.
- For save/load rows, capture before-save, after-load, and post-resolution evidence.

## Resolved / Player-Verified

Closed source-repaired items are tracked in `archive/issues-archive.md`. Do not duplicate those closed rows here.
