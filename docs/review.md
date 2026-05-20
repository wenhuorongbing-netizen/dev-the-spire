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
- 2026-05-20 governance pass: updated `scripts/verify-spire-plus-release-evidence.ps1` and `ReleaseSafetyExpandedGuardTests` so the verifier default package hash matches the current `B19620D8D8A15D5B96208D3DE8C3B372BCA0874E076DD2DEBEDE09422FF28BD2` package.
- 2026-05-20 subagent review pass: current smoke-log parity now computes the expected `SavedSpireField` count from source and rejects historical 22-field logs as current package evidence.
- 2026-05-20 subagent review pass: tightened current smoke-log parity to count only static `SavedSpireField` declarations, so helper method generic references do not inflate the expected runtime loader count.
- 2026-05-20 subagent review pass: active-source coverage no longer lets `ActiveSourceManifestGuardTests.cs` satisfy itself; every active source file must map to an independent guard root.
- 2026-05-20 subagent review pass: patch-inventory freshness checks now ignore the generated date, fail if the inventory is missing, and CI whitespace checks inspect committed/PR changes rather than an empty working tree.
- 2026-05-20 subagent review pass: Forge Armor shatter now uses the host's pre-Molten-Armor Block baseline instead of subtracting shared `BlockedDamage`.
- 2026-05-20 subagent review pass: fixed low-risk lifetime/scope issues in Future Peek transform RNG context, Urda Root Eyes transient selection state, Root Eyes failure logging, and Vakuu pre-finished parent restore heal skips.
- 2026-05-20 subagent review pass: Root Eyes now refunds previews that become unreachable after the player chooses another map branch, including marker restore and hover cleanup paths.
- Seedbed now catches eligible cards that enter the hand through Urda's hand-change hook, not only through the RootBud combat hook.
- Lotha Death Reprieve save hydration now restores pending-start state from the saved phase instead of inferring it from the current power list.
- Urda Molting act-entry cleanup clears its active flag after removing generated husks.
- Firemark Giant's Molten Core window no longer counts the threshold-crossing hit as window damage.
- Banner and Forge Token target selection no longer consumes live run RNG for source-testable deterministic cases.
- Multi-enemy-only banner map previews now use generic banner text/icon until combat knows the enemy count.
- Future Peek stays outside Spire Plus folders; Crystal Sphere preview restores/hides its UI after the minigame finishes, and transform preview remains preview-only.
- Morvi, Lotha, and Vakuu combat powers now use dedicated 64px/256px power art paths instead of option, card, or fallback art.
- `export_presets.cfg` was restored to UTF-8 without BOM after Godot rejected the export preset during publish.

## Package Under Test

`publish/SpirePlus-v0.1.0-private-beta.0.zip`

| Artifact | SHA256 |
| --- | --- |
| ZIP | `B19620D8D8A15D5B96208D3DE8C3B372BCA0874E076DD2DEBEDE09422FF28BD2` |
| DLL | `A1D86D01E57E0F58617ACA23EA8094B1AF35F525E3254007DE3675A1289B8159` |
| PCK | `073CAF976C91D9E6CEA39FA90FB5A6417E66CD5E12DED5EDD8169C892A0F0538` |
| Manifest | `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2` |
| README_INSTALL | `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4` |

## Latest Validation

No game was opened.

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet build EZFuturePeek.sln
dotnet test EZFuturePeek.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
dotnet format EZFuturePeek.sln --verify-no-changes --no-restore
git diff --check
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
dotnet publish EZFuturePeek.sln
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build
```

Results:

- Spire Plus build: 0 warnings/errors.
- Spire Plus normal tests: 202 passed / 18 skipped.
- Future Peek build: 0 warnings/errors.
- Future Peek tests: 8 passed.
- Format checks: passed for both solutions.
- `git diff --check`: passed with existing CRLF/LF warnings only.
- Spire Plus publish/package: passed.
- Future Peek publish: passed.
- Artifact tests: 220 passed / 0 skipped.
- Local game-root zip copy/hash check: passed.
- New `scripts/ci-full-validation.ps1 -IncludeFuturePeek` lane: passed locally with explicit `STS2_PATH` and `GODOT_PATH`.

## Manual Retest Queue

Use `docs/toreview.md` as the current tester queue. Do not close those rows from source review alone. Close only after the matching live manual proof exists.

## Review Rules

- Keep source moves behavior-preserving unless the slice is explicitly a bug fix.
- Keep active docs compact and archive historical logs under `docs/archive/**`.
- Do not claim live gameplay, save-load, death/failure, co-op, or release readiness without direct game evidence.
