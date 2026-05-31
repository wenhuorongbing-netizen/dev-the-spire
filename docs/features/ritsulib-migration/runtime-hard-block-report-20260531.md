# Runtime Hard Block Report — 2026-05-31

## Decision

**Original runtime hard block resolved for loader gates; release remains blocked.** The missing-dependency blocker is cleared locally: official STS2-RitsuLib `v0.3.10` is installed in the active E-drive game root. After fixing RitsuLib target descriptors, Off and CanaryOnly diagnostic smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. Off proves 0 StS1 registration lines; CanaryOnly proves exactly 4 canary content registrations. Batch 4c, high-risk migration, live-ready, and release-ready remain blocked until independent QA reruns and gameplay/UI/save-load/co-op/package handoff gates are addressed.

## Evidence

| Check | Result |
| --- | --- |
| HEAD | `6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2` |
| Branch | `main...origin/main` |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Present (`v0.3.10`, includes `lib\0.106.1`) |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Present; copied evidence logs should be used for review |
| Off target-fix evidence | `.tools\runtime-evidence\ritsulib-off-after-target-fix-20260531-2325\godot.log.after-launch`; clean audit, 25/25 patches, Sts1Events Off |
| Canary target-fix evidence | `.tools\runtime-evidence\ritsulib-canary-after-target-fix-20260531-2327\godot.log.after-direct-launch`; clean audit, 25/25 patches, 4 canary registrations |
| Prior controlled loader evidence | `.tools\runtime-evidence\sts1-events-v15-loader-20260531-231135\godot.log.after-launch`; historical failed smoke before target fix |
| Direct-launch evidence | `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-direct-exe-steam-init-fail.log` |
| Steam-launch evidence | `.tools\runtime-evidence\ritsulib-runtime-proof-20260531-2304\godot-steam-applaunch.log` |
| Steam-launch audit | Not clean; 3 Godot ERROR lines |

## Required Runtime Proof

1. Keep STS2-RitsuLib v0.3.10 installed under the active game root at `<GameRoot>\mods\STS2-RitsuLib`.
2. Enable only BaseLib, STS2-RitsuLib, and Spire Plus.
3. Keep the clean Off and CanaryOnly target-fix evidence attached to any QA rerun.
4. Rerun independent QA/Red-Team against the new evidence.
5. Refresh a versioned tester package before any handoff if these code changes are shipped.
6. Continue to withhold live-ready/release-ready claims until gameplay, UI, save-load, and co-op proof exists.

## Next Action

Rerun independent QA/Red-Team against the target-fix evidence, then decide whether to cut a versioned tester package. Do not start Batch 4c before QA accepts the loader/gate proof and the owner accepts the worktree/package state.

Latest local prerequisite evidence: `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt`.
