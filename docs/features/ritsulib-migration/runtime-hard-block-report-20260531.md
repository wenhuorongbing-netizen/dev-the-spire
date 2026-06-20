# Runtime Hard Block Report — 2026-05-31

## Decision

2026-06-21 current note: this report is historical `v0.106.1` loader-gate evidence. The current local game install is `v0.107.1`, and official STS2-RitsuLib `v0.4.29` is installed with `lib\0.107.1`. Installed beta.92 package parity passed, beta.92 default-Off proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-off-direct-20260621/` and beta.92 AdditiveBatch1 proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-additivebatch1-direct-20260621/` are the current RitsuLib-only loader/registration context. Retained beta.85/beta.87 `v0.107.0` proof and beta.88 BaseLib-backed proof remain previous-package or previous-dependency context. Current gameplay, save-load, replacement, multiplayer, QA, and release-ready proof remain pending or blocked.

**Original runtime hard block resolved for historical loader gates; release remains blocked.** The missing-dependency blocker was cleared locally for the historical `v0.106.1` lane: official STS2-RitsuLib `v0.3.10` was installed in the active E-drive game root. After fixing RitsuLib target descriptors, Off and CanaryOnly diagnostic smokes reached main menu with BaseLib, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches applied. Off proves 0 StS1 registration lines; CanaryOnly proves exactly 4 canary content registrations for that historical lane. Batch 4c, high-risk migration, live-ready, and release-ready remain blocked until current enabled-mode proof, independent QA reruns, and gameplay/UI/save-load/co-op/package handoff gates are addressed.

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

1. Keep the current official STS2-RitsuLib install under the active game root at `<GameRoot>\mods\STS2-RitsuLib`; as of 2026-06-20 this is `v0.4.29` with `lib\0.107.1`.
2. Enable only STS2-RitsuLib and Spire Plus for current RitsuLib-only proof.
3. Preserve the beta.92 default-Off loader proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-off-direct-20260621/` as current loader proof until the package/source/runtime target changes.
4. Preserve beta.92 AdditiveBatch1 proof under `.tools/runtime-evidence/v01071-beta92-ritsulib0429-additivebatch1-direct-20260621/` as current loader/registration proof only; beta.85/beta.87/beta.88 rows are historical context.
5. Rerun independent QA/Red-Team against the new current evidence.
6. Refresh a versioned tester package before any handoff if these code changes are shipped.
7. Continue to withhold live-ready/release-ready claims until gameplay, UI, save-load, and co-op proof exists.

## Next Action

After coordination clears, capture gameplay, save-load, render, replacement, multiplayer, and QA evidence or record the exact blocker. Do not start Batch 4c before runtime gameplay proof, QA acceptance, and owner acceptance of the worktree/package state.

Latest local prerequisite evidence: `.tools/runtime-evidence/refactor-overnight-20260531/runtime-prereq-paths.txt`.
