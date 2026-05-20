# Current Source Review

Date: 2026-05-20

Scope: current no-game source/resource review notes for taking `Spire Plus` to a user-test-ready build. Full historical review details are archived at `docs/archive/feature-audits/review-pre-slim-20260518.md`.

## Current Conclusion

No current static P0/P1 source blocker is known from the latest no-game review passes. This does not prove release readiness.

Live-only blockers remain:

- Vakuu victory return/no-black-screen, failure/death path, active-fight save-load, and co-op.
- Urda Root Eyes hover/click/entry/save-load, Seed Bank click extraction, and clicked Ancient UI.
- Morvi and Lotha live gameplay, card-play freeze reports, save-load, and co-op.
- A11 route traversal, A12/A16/A19/A20 combat behavior, Rootblight combat-end behavior, and fresh current-package loader proof.

## Latest Fixed Findings

- 2026-05-20 governance pass: added CI-safe repository hygiene workflow, issue/PR templates, ADR template, committed `.editorconfig`, generated `docs/patch-inventory.md`, added `docs/release-evidence-status.md`, and guarded these with `EngineeringGovernanceGuardTests`.
- 2026-05-20 governance pass: added self-hosted `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` for full no-game validation with explicit `STS2_PATH` and `GODOT_PATH`; the script passed locally, and first GitHub self-hosted workflow run evidence remains pending.
- 2026-05-20 release-planning pass: converted `docs/goal.md` into no-game baseline, release scope, website claim audit, traceability matrix, source-research, architecture-boundary, save-state, and commit-boundary docs. The pass keeps live/manual rows open.
- 2026-05-20 governance pass: updated `scripts/verify-spire-plus-release-evidence.ps1` and `ReleaseSafetyExpandedGuardTests` so the verifier default package hash matches the current `CE417F595E2CCE8435C0575D95A3A866CBDA8FD605DE3F40014639E9301EFF62` package.
- 2026-05-20 subagent review pass: current smoke-log parity now computes the expected `SavedSpireField` count from source and rejects historical 22-field logs as current package evidence.
- 2026-05-20 subagent review pass: tightened current smoke-log parity to count only static `SavedSpireField` declarations, so helper method generic references do not inflate the expected runtime loader count.
- 2026-05-20 subagent review pass: active-source coverage no longer lets `ActiveSourceManifestGuardTests.cs` satisfy itself; every active source file must map to an independent guard root.
- 2026-05-20 subagent review pass: patch-inventory freshness checks now ignore the generated date, fail if the inventory is missing, and CI whitespace checks inspect committed/PR changes rather than an empty working tree.
- 2026-05-20 subagent review pass: Forge Armor shatter now uses the host's pre-Molten-Armor Block baseline instead of subtracting shared `BlockedDamage`.
- 2026-05-20 subagent review pass: fixed low-risk lifetime/scope issues in the transform-preview RNG context, Urda Root Eyes transient selection state, Root Eyes failure logging, and Vakuu pre-finished parent restore heal skips.
- 2026-05-20 subagent review pass: Root Eyes now refunds previews that become unreachable after the player chooses another map branch, including marker restore and hover cleanup paths.
- 2026-05-20 goal guard pass: added completion-claim and save-state contract guard tests plus `docs/reviews/red-team-goal-implementation-pass-1.md`; this is not a release-ready claim and live loader, clicked UI, save-load, Vakuu, co-op, and preview proof remain pending.
- Seedbed now catches eligible cards that enter the hand through Urda's hand-change hook, not only through the RootBud combat hook.
- Lotha Death Reprieve save hydration now restores pending-start state from the saved phase instead of inferring it from the current power list.
- Urda Molting act-entry cleanup clears its active flag after removing generated husks.
- Firemark Giant's Molten Core window no longer counts the threshold-crossing hit as window damage.
- Banner and Forge Token target selection no longer consumes live run RNG for source-testable deterministic cases.
- Multi-enemy-only banner map previews now use generic banner text/icon until combat knows the enemy count.
- Preview tools are integrated under Spire Plus; Crystal Sphere preview restores/hides its UI after the minigame finishes, and transform preview remains preview-only.
- Morvi, Lotha, and Vakuu combat powers now use dedicated 64px/256px power art paths instead of option, card, or fallback art.
- `export_presets.cfg` was restored to UTF-8 without BOM after Godot rejected the export preset during publish.

## Package Under Test

`publish/SpirePlus-v0.1.0-private-beta.0.zip`

| Artifact | SHA256 |
| --- | --- |
| ZIP | `CE417F595E2CCE8435C0575D95A3A866CBDA8FD605DE3F40014639E9301EFF62` |
| DLL | `940F1FEA66B01CB54A1CCEC388D4F023693C947395C7B7F9922BF596A8586E1E` |
| PCK | `3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392` |
| Manifest | `A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF` |
| README_INSTALL | `BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906` |

## Latest Validation

No game was opened.

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build
```

Results:

- Spire Plus build: 0 warnings/errors.
- Spire Plus normal tests: 209 passed / 18 skipped.
- Format check: passed.
- `git diff --check`: passed with existing CRLF/LF warnings only.
- Spire Plus publish/package: passed.
- Artifact tests: 227 passed / 0 skipped.
- Local game-root zip copy/hash check: passed.
- New `scripts/ci-full-validation.ps1` lane: passed locally with explicit `STS2_PATH` and `GODOT_PATH`.

## Manual Retest Queue

Use `docs/toreview.md` as the current tester queue. Do not close those rows from source review alone. Close only after the matching live manual proof exists.

## Review Rules

- Keep source moves behavior-preserving unless the slice is explicitly a bug fix.
- Keep active docs compact and archive historical logs under `docs/archive/**`.
- Do not claim live gameplay, save-load, death/failure, co-op, or release readiness without direct game evidence.
