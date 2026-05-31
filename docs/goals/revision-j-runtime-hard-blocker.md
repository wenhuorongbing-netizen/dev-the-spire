# Revision J Runtime Hard Blocker

Date: 2026-05-31
HEAD: `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2`

## Blocker

Runtime smoke remains blocked because the fresh controlled loader log is not clean enough for current-package runtime proof.

This is not the old dependency-path blocker. The E-drive runtime dependency paths now exist, including official `STS2-RitsuLib` `v0.3.10` and its `0.106.1` variant. A fresh controlled loader log at `.tools/runtime-evidence/sts1-events-v15-loader-20260531-231135/godot.log.after-launch` reaches main menu with BaseLib, RitsuLib, and Spire Plus loaded, but the audit reports 11 `Godot ERROR` hits, including `ritsulib-variants.json` manifest parsing and 8 optional Spire Plus ModPatcher failures.

## Path Evidence

| Path | Exists | Decision |
|---|---:|---|
| `E:\Steam\steamapps\common\Slay the Spire 2` | True | Active game root |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | True | Required dependency present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | True | Required dependency present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\STS2-RitsuLib.dll` | True | Runtime loader present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\lib\0.106.1\STS2-RitsuLib.dll` | True | Current target variant present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | True | Spire Plus install present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll` | True | Spire Plus runtime DLL present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | True | Fresh loader proof exists, but audit is not clean |

## Why The Worktree Cannot Resolve It

- A clean `godot.log` is produced by launching the game and resolving loader/runtime errors, not by editing documentation.
- Source/build/test validation can prove compile and guard behavior, but cannot prove runtime loader order, RitsuLib variant selection, ModPatcher application, or Sts1Events registration counts.
- Current live proof has loader errors and only 17 of 25 Spire Plus ModPatcher patches applied, so Off=0 closure, CanaryOnly=4, runtime safety, live-ready, and release-ready remain unproven.

## Exact Owner Action Required

Resolve or explicitly disposition the 11 loader error hits, then rerun a controlled live loader smoke on the E-drive install with BaseLib, STS2-RitsuLib, and Spire Plus enabled. Preserve a clean `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log`.

Required proof:

1. Off mode: unset or empty `SPIREPLUS_STS1_EVENT_MODE`; prove 0 StS1 event registrations.
2. CanaryOnly mode: `SPIREPLUS_STS1_EVENT_MODE=CanaryOnly`; prove exactly Big Fish, Golden Idol, The Lab, and Divine Fountain register.
3. Loader log: prove BaseLib, STS2-RitsuLib, Spire Plus, RitsuLib bootstrap, 25 migrated patches, remaining raw Harmony patches, and 30 SavedSpireFields initialize without release-blocking errors.

## Owner Decision Options

| Option | Meaning | Risk |
|---|---|---|
| Run live smoke now | Close the blocker with fresh `godot.log` evidence | Requires external Steam/game session |
| Accept owner-review packet | Keep runtime blocked but document current truth precisely | No runtime-ready claim allowed |
| Reject local runtime-install docs | Revert docs that record local `STS2-RitsuLib` install state | Future agents may chase a solved dependency prerequisite |
| Reject helper-script change | Do not keep `STS2-RitsuLib` in live-session allowed mod ids | `-MoveOtherMods` can break runtime smoke by moving RitsuLib out |

## Stop Decision

Revision J may be owner-review complete with this blocker documented, but it is not runtime complete. Batch 4c, Batch 5, PR7, Sts1Events formalization, debug expansion, longhaul audit, runtime-ready, live-ready, and release-ready remain blocked.
