# Next Overnight Run - RitsuLib Migration

## Run Date

TBD (next available runtime/manual QA session)

## Objective

**Manual QA + Batch 4c Owner Review After Clean Loader Proof.**

The old hard blocker was missing or non-clean RitsuLib runtime smoke. That blocker is now closed for loader proof: historical Slay the Spire 2 `v0.106.1` Off, CanaryOnly, and AdditiveBatch1 diagnostic evidence is clean, and current `v0.107.0` beta.85 Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` reached main menu and audited clean after the Spire Plus API-target drift fix. The next run should upgrade this into manual gameplay proof and owner-reviewed Batch 4c planning. It is not approval to migrate more patches.

Coordination boundary: while the same-repository validation pause remains active, do not run this plan's `dotnet` validation, package/release, runtime/game smoke, staging, commit, or push steps. During the pause, use this plan only for read-only/static planning, source-only expected-shape output, and already-captured evidence review; run the pre-run checklist only after the pause is lifted and one controlled validation lane is assigned.

## Current State

- Current repository HEAD must be checked at run start. The latest observed pushed HEAD before this runtime-fix pass is `bdb51c39`.
- Current worktree is dirty and includes tracked and untracked source, script, and doc edits, including StS1 event governance files and the tracked `docs/features/ritsulib-migration/batch-4c-candidates.md`. Classify dirty files before any validation claim.
- Latest beta.85 no-game validation is summarized in `PROJECT_STATE.md`: 0 build errors and 0 warnings; split no-build lanes covered 475 passed / 0 failed / 21 skipped / 496 total; the opt-in package/artifact subset passed 67 / 0 failed / 0 skipped / 67 total. Revalidate current HEAD in a clean single validation lane again before package handoff if the worktree changes.
- Patch state remains 25 migrated `IPatchMethod` classes, 142 raw `[HarmonyPatch]` declarations, 167 tracked patch units total.
- Historical `v0.106.1` loader-gate runtime proof exists:
  - Off: `.tools\runtime-evidence\smoke-k1-off-20260602-145938`, 0 Sts1Events registrations, clean audit.
  - CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.
  - AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.
  - Recorded smokes loaded BaseLib, RitsuLib, and Spire Plus, applied 25/25 migrated patches, and observed 30 SavedSpireFields.
- Current `v0.107.0` beta.85 package runtime proof is clean for loader/patch application under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`; installed package parity passes.
- Current enabled-mode proof is still missing: before gameplay evidence, CanaryOnly must prove 4 event types / 6 registration calls and AdditiveBatch1 must prove 10 event types / 14 registration calls on beta.85 / `v0.107.0`, with retained `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` verifier reports.
- Dependency decision is recorded: keep the current dirty source and beta.85 package line at compile/manifest `0.3.2`; move both to `0.4.16` only in a future owner-approved versioned package pass for the `v0.107.0` runtime.
- Batch 4c is proposal-only. The current candidate list is `docs/features/ritsulib-migration/batch-4c-candidates.md`; migration requires explicit owner approval and fresh validation.
- Gameplay, Mod Settings UI page refresh, event screenshots, save-load, image/render, replacement functional proof, multiplayer fail-closed proof, independent QA, clean-worktree decision, and tester-package handoff remain pending.
- Release-ready and live-ready remain no.

## Pre-Run Checklist

1. Confirm branch, HEAD, and dirty state:

```powershell
git status --short --branch
git log -1 --oneline --decorate
```

2. Confirm or refresh no-game validation for the current HEAD:

```powershell
dotnet clean EZMicroBalance.sln
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

3. Confirm current runtime compatibility before any loader smoke:

```powershell
Get-Content "E:\Steam\steamapps\common\Slay the Spire 2\release_info.json"
Get-ChildItem "E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib\lib"
```

4. If validation or runtime compatibility fails, record the exact command, error, and affected dirty files before editing.
5. If owner approves a new `v0.107.0` tester package, move the repo package reference and manifest minimum from `0.3.2` to `0.4.16` in that same versioned package pass and follow the private-beta package-version and validation rules.

## Run Steps

### Step 1: Reconcile Loader Evidence

1. Verify the evidence folders listed above still exist or record that only documentation references remain.
2. Verify the current game version still has a matching RitsuLib variant and that the installed package under test matches the intended artifact; if either check fails, record the blocker instead of launching the game. The beta.85 package artifact currently matches and has clean current-runtime loader proof under `v0.107.0`.
3. Re-audit the recorded logs if needed with `scripts\audit-godot-log.ps1`.
4. Rerun loader smoke only if runtime compatibility is present and HEAD/package drift makes the old logs stale for the claim being made.
5. Before any StS1 event gameplay claim, capture fresh current CanaryOnly smoke and verify 4 event types / 6 registration calls with `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json` retained in the evidence folder.
6. Before any AdditiveBatch1 gameplay claim, capture fresh current AdditiveBatch1 smoke and verify 10 event types / 14 registration calls with the same retained verifier reports.
7. If current enabled-mode smoke cannot be captured, record the exact blocker instead of continuing into gameplay proof.
8. Keep AdditiveAllDraft and ReplaceUnknownEventsPrototype out of tester/release paths.

### Step 2: Manual Gameplay Evidence

For CanaryOnly events (`Sts1BigFish`, `Sts1GoldenIdol`, `Sts1TheLab`, `Sts1DivineFountain`), capture or explicitly block:

- spawn/route status;
- EN and ZHS rendering;
- options;
- reward/effect;
- no-softlock and exit;
- save/load;
- screenshot evidence.

### Step 3: UI And Multiplayer Evidence

1. Capture current Spire Plus Mod Settings UI evidence.
2. Capture or block co-op/fail-closed proof.
3. Confirm loader-gate proof is not being treated as gameplay proof.

### Step 4: Batch 4c Candidate Review

1. Review `docs/features/ritsulib-migration/batch-4c-candidates.md`.
2. Confirm the list is 5-10 low-risk patch classes only.
3. Reject any run lifecycle, save/load, map generation, multiplayer/lobby, death, A20 boss-flow, or reward-state candidate.
4. Record owner decision: accept all, accept subset, request changes, or block.
5. Do not migrate any candidate without explicit owner acceptance and fresh validation.

### Step 5: Blocker Reporting

If a gate cannot be completed, record:

- exact gate;
- blocker reason;
- evidence path;
- attempted actions;
- owner action needed;
- why continuing would create an unsupported claim.

## Success Criteria

- [x] Current STS2-RitsuLib installed locally (`v0.4.16`, `lib\0.107.0`).
- [x] Historical `v0.106.1` Off loader gate proves 0 Sts1Events registrations.
- [x] Historical `v0.106.1` CanaryOnly loader gate proves exactly 4 canary registrations in the old source shape; current proof still needs 4 event types / 6 registration calls.
- [x] Historical `v0.106.1` AdditiveBatch1 loader gate proves 10 event types through the old 11 registration calls; current proof still needs 10 event types / 14 registration calls.
- [x] Current `v0.107.0` game install has matching STS2-RitsuLib `v0.4.16` / `lib\0.107.0` runtime files.
- [x] Dependency bump decision recorded: defer compile/manifest `0.4.16` bump until an owner-approved versioned package pass.
- [x] Installed beta.85 package parity verified after the 2026-06-11 package refresh.
- [x] Clean current `v0.107.0` beta.85 Off loader smoke captured after the `v0.4.16` install.
- [ ] Current HEAD validation refreshed after the latest dirty worktree changes, or explicitly recorded as stale under the validation coordination pause.
- [ ] Current `v0.107.0` CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.
- [ ] Current `v0.107.0` AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports.
- [ ] Mod Settings UI evidence captured.
- [ ] CanaryOnly gameplay matrix completed or blocked with evidence.
- [ ] Save/load evidence captured or blocked with evidence.
- [ ] Co-op/fail-closed evidence captured or blocked with evidence.
- [ ] Batch 4c owner decision recorded.

## Hard Stops

- No release-ready, live-ready, or runtime-safe claim from loader logs alone.
- No Batch 4c migration without owner acceptance.
- No high-risk migration in this run.
- No package handoff while validation, gameplay proof, clean-worktree decision, or owner decision is missing.
