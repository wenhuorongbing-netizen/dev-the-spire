# M5 Revision L Owner-Review Packet

Date: 2026-06-10
Baseline HEAD: `f32c6767 (HEAD -> main, origin/main, origin/HEAD)`
Status: owner-review packet prepared; not release-ready; not live-ready.

Revision M supersession note, 2026-06-11: this packet's beta.84 package/runtime boundary is historical. Current beta.85 has clean `v0.107.0` default-Off loader proof only; current CanaryOnly, AdditiveBatch1, gameplay, save-load, replacement, multiplayer, QA, and release-ready proof remain pending. Use `PROJECT_STATE.md` and the Revision M docs for current proof claims.

Current supersession, 2026-06-20: beta.91 has RitsuLib-only Off and AdditiveBatch1 loader/registration proof on Slay the Spire 2 `v0.107.1` with STS2-RitsuLib `0.4.28`; gameplay, UI, save-load, replacement, co-op, QA, and release-ready proof remain pending. Use `PROJECT_STATE.md` and `docs/test-ready-development-goal.md` for current claims.

## Current Truth

- Worktree is dirty and must not be treated as the beta.84 package source without owner approval, version bump, publish/package refresh, and handoff docs.
- Current source/build references started from the historical Slay the Spire 2 `v0.106.1` / previous framework `v3.1.4` target, while the installed game root is now `v0.107.0`; dirty source API fixes were adapted to the installed DLL surface, official RitsuLib `v0.4.16` with `lib\0.107.0` is installed, installed beta.84 package parity is restored, and the fresh beta.84 Off smoke failed clean audit on stale package API targets.
- The installed game DLL exposes the currently compiled API surface used by the dirty source changes: `ModifyPowerAmountGivenAdditive`, `Ectoplasm.ModifyGoldGained`, and `CookRestSiteOption.get_IsEnabled`.
- The checked-in `source code/` snapshot contains stale source names for some of those APIs; the build against the installed project references is the deciding no-game gate for this pass.
- Static EN/ZHS localization is aligned. Runtime EN/ZHS render proof is still pending.
- Final no-game validation is recorded in `docs/reviews/current-validation.md`; this thread did not start overlapping validation after the same-repo coordination pause.

## Validation Snapshot

| Gate | Current result |
|---|---|
| Project build | Superseded by solution build; earlier 70-warning project-build snapshot is historical |
| Full solution build | PASS: `dotnet build EZMicroBalance.sln -m:1 --no-incremental`, 0 errors, 0 warnings |
| Tests | PASS: test-project and exact solution-level no-build lanes both report 464 passed / 0 failed / 21 skipped / 485 total; the 21 skipped tests are documented in `docs/reviews/current-validation.md` as opt-in `[ReleaseArtifactFact]` package/install/runtime artifact checks |
| Format | PASS |
| Diff check | PASS; emitted only the existing CRLF normalization warning for `docs/patch-inventory.md` |
| Patch inventory | PASS; regenerated and fresh |
| Batch classifier | PASS: 62 dirty entries, 0 unclassified |
| Publish/package | Not run; no tester package handoff is being made from the dirty source |

## Runtime Boundary

- RitsuLib is installed at `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` as official `v0.4.16` with `lib\0.107.0\STS2-RitsuLib.dll`.
- The current installed game is Slay the Spire 2 `v0.107.0`; installed Spire Plus DLL hash now matches packaged beta.84 after the 2026-06-10 DLL restore, but the fresh beta.84 Off smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` failed clean audit on stale package API targets.
- Historical diagnostic runtime evidence exists for Off, CanaryOnly, and AdditiveBatch1 modes. Those logs reached the main menu with previous framework, RitsuLib, and Spire Plus loaded, clean audits, and 25/25 Spire Plus ModPatcher patches.
- No fresh game launch was produced for the current dirty source. No gameplay, event screenshot, save-load, image/render, replacement, multiplayer, or independent QA proof was produced.
- `publish/SpirePlus-v0.1.0-private-beta.84.zip` remains the last packaged artifact and must not be represented as a package of this dirty source state.

## Decisions For Owner

| Area | Recommendation | Reason |
|---|---|---|
| Source API fix slice | Accept for owner review | It restores build compatibility with installed game APIs but affects combat/reward/rest-site behavior, so it still needs owner-approved commit scope. |
| Sts1Events | Keep staging-only | Loader gates exist and current warning debt is cleared, but gameplay/render/save-load proof is still open. |
| Debug | Accept scaffold only | Superseded by Revision M: the unused `SpirePlusDebug.LogPreview` helper has been removed. Broad debug logging remains scaffold-level and must stay gated before any promotion. |
| RitsuLib migration | Treat the historical loader gate as validated, not release-ready | The old missing-runtime-folder blocker and historical `v0.106.1` loader smoke blocker are cleared, but current `v0.107.0` runtime proof and live feature proof are not. |
| Batch 4c | Proposal-only | Candidate list exists; no migration should start until owner accepts scope and current source/package boundary. |
| Manual handoff harness | Review as its own slice | It changes no-launch evidence scaffolding and PowerShell test execution, not gameplay behavior. |
| Commit/push | Do not commit or push in this pass | The requested debug pass asked for owner review first. |

## Open Blockers

- Owner approval of commit slices.
- Fresh loader smoke for any new tester package.
- Clicked Ancient UI, gameplay, save-load, route traversal, preview-tools, Vakuu, and co-op proof.
- Sts1Events event screenshots, EN/ZHS render proof, image/render decision, replacement functional proof, and multiplayer fail-closed proof.
- Versioned tester package handoff if the source slice is delivered.
