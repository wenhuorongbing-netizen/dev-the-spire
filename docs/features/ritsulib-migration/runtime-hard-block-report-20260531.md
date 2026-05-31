# Runtime Hard Block Report — 2026-05-31

## Decision

**Hard Block Stop.** Runtime smoke cannot run because STS2-RitsuLib is not installed in the active game root. Batch 4c, high-risk migration, Off/CanaryOnly runtime claims, live-ready, and release-ready remain blocked.

## Evidence

| Check | Result |
| --- | --- |
| HEAD | `24d4fe9a (HEAD -> main, origin/main, origin/HEAD) ci: regenerate patch inventory consistently` |
| Branch | `main...origin/main` |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Missing |
| `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` | Missing |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib` | Present |
| `E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | Present |
| `E:\Steam\steam.exe` | Present |
| `C:\Users\zihao\AppData\Roaming\SlayTheSpire2\logs\godot.log` | Missing |

## Required Runtime Proof

1. Install STS2-RitsuLib v0.3.2+ under the active game root at `<GameRoot>\mods\STS2-RitsuLib`.
2. Enable only BaseLib, STS2-RitsuLib, and Spire Plus.
3. Launch through Steam and capture fresh `godot.log`.
4. Verify RitsuLib bootstrap starts and ModPatcher applies 25 patches.
5. Verify BaseLib and Spire Plus initialize.
6. Verify no `MissingMethodException`, `TypeLoadException`, or manifest dependency failure.
7. Run Sts1Events Off mode and prove 0 registrations.
8. Run Sts1Events CanaryOnly mode and prove exactly 4 canary registrations.

## Next Action

Install STS2-RitsuLib in the active E-drive game root, rerun runtime smoke from `docs/features/ritsulib-migration/runtime-smoke-checklist.md`, and update `docs/reviews/current-validation.md` with the captured `godot.log` evidence. Do not start Batch 4c before this proof exists.
