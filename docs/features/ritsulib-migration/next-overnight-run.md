# Next Overnight Run - RitsuLib Migration

## Run Date

TBD (next available runtime/manual QA session)

## Objective

**Loader-Proven Manual QA + Batch 4c Owner Review.**

The old hard blocker was missing or non-clean RitsuLib runtime smoke. That blocker is cleared only for the historical Slay the Spire 2 `v0.106.1` loader-gate level: Off, CanaryOnly, and AdditiveBatch1 have clean diagnostic evidence there. The current local game install is `v0.107.0`; official `STS2-RitsuLib` `v0.4.16` is now installed with `lib\0.107.0`, and installed beta.84 package parity passes after the 2026-06-10 DLL restore. The fresh beta.84 `v0.107.0` Off smoke under `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` reached main menu but failed clean audit because the package still targets stale game APIs. The next run must decide whether to build an owner-approved current-source tester package before trying to upgrade evidence into manual gameplay proof and owner-reviewed Batch 4c planning. It is not approval to migrate more patches.

## Current State

- Current repository HEAD must be checked at run start. The latest observed HEAD in this pass is `f32c6767`.
- Current worktree is dirty and includes unrelated source/test/doc edits plus an untracked `docs/features/ritsulib-migration/batch-4c-candidates.md`. Classify dirty files before any validation claim.
- Latest recorded no-game validation is in `docs/reviews/current-validation.md`: the current dirty-worktree pass has 0 build errors and 0 warnings, and both the test-project lane and exact solution-level `dotnet test EZMicroBalance.sln --no-build` lane passed with 464 passed / 0 failed / 21 skipped / 485 total after overlapping validation processes were absent. Revalidate current HEAD in a clean single validation lane again before package handoff if the worktree changes.
- Patch state remains 25 migrated `IPatchMethod` classes, 142 raw `[HarmonyPatch]` declarations, 167 tracked patch units total.
- Historical `v0.106.1` loader-gate runtime proof exists:
  - Off: `.tools\runtime-evidence\smoke-k1-off-20260602-145938`, 0 Sts1Events registrations, clean audit.
  - CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations, clean audit.
  - AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through 11 registration calls, clean audit.
  - Recorded smokes loaded BaseLib, RitsuLib, and Spire Plus, applied 25/25 migrated patches, and observed 30 SavedSpireFields.
- Current `v0.107.0` package runtime proof is blocked by the non-clean beta.84 smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/`; installed package parity itself now passes after the DLL restore.
- Dependency decision is recorded: keep the current dirty source and beta.84 package line at compile/manifest `0.3.2`; move both to `0.4.16` only in a future owner-approved versioned package pass for the `v0.107.0` runtime.
- Batch 4c is proposal-only. The current candidate list is `docs/features/ritsulib-migration/batch-4c-candidates.md`; migration requires explicit owner approval and fresh validation.
- Gameplay, Mod Settings UI, event screenshots, save-load, image/render, replacement functional proof, multiplayer fail-closed proof, independent QA, clean-worktree decision, and versioned tester-package handoff remain pending.
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
2. Verify the current game version still has a matching RitsuLib variant and that the installed package under test matches the intended artifact; if either check fails, record the blocker instead of launching the game. The beta.84 package artifact now matches but fails clean current-runtime smoke under `v0.107.0`.
3. Re-audit the recorded logs if needed with `scripts\audit-godot-log.ps1`.
4. Rerun loader smoke only if runtime compatibility is present and HEAD/package drift makes the old logs stale for the claim being made.
5. Keep AdditiveAllDraft and ReplaceUnknownEventsPrototype out of tester/release paths.

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
- [x] Historical `v0.106.1` CanaryOnly loader gate proves exactly 4 canary registrations.
- [x] Historical `v0.106.1` AdditiveBatch1 loader gate proves 10 event types through 11 registration calls.
- [x] Current `v0.107.0` game install has matching STS2-RitsuLib `v0.4.16` / `lib\0.107.0` runtime files.
- [x] Dependency bump decision recorded: defer compile/manifest `0.4.16` bump until an owner-approved versioned package pass.
- [x] Installed beta.84 package parity restored and verified after the 2026-06-10 DLL restore.
- [ ] Clean current `v0.107.0` loader smoke captured after the `v0.4.16` install. The beta.84 package-parity smoke was captured but failed clean audit.
- [ ] Current HEAD validation refreshed or explicitly recorded as stale.
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
