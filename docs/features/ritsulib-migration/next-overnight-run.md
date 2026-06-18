# Next Overnight Run - RitsuLib Migration

## Run Date

TBD (next available runtime/manual QA session)

## Objective

**Manual QA + Batch 4c Owner Review After Clean Loader Proof.**

The old hard blocker was missing or non-clean RitsuLib runtime smoke. That blocker is now closed for loader/registration proof: historical Slay the Spire 2 `v0.106.1` Off, CanaryOnly, and AdditiveBatch1 diagnostic evidence is clean, current `v0.107.0` beta.85 Off smoke under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/` reached main menu and audited clean after the Spire Plus API-target drift fix, current beta.85 CanaryOnly smoke under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` passed retained log/packet verifiers with 4 event types / 6 registration lines as previous-package context, and current beta.87 AdditiveBatch1 direct smoke under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` passed retained log/packet verifiers with 10 event types / 14 registration lines. The beta.85 AdditiveBatch1 13/14 mismatch is root-cause history only. The next run should move to gameplay, render, save-load, replacement, multiplayer, QA, and handoff proof after the validation coordination pause clears. It is not approval to migrate more patches.

Coordination boundary: do not run overlapping validation, package/release, runtime/game smoke, staging, commit, or push steps. Assign one controlled lane, verify `git status --short --branch` first, and record the resulting HEAD/worktree status before using evidence for handoff.

## Current State

- Current repository HEAD must be checked at run start. The run-start pushed HEAD for the blank-path/log-growth runtime probe follow-up was `77d46f23`; any later commits or dirty edits must still be recaptured exactly before handoff.
- Any dirty files after the latest pushed HEAD are post-baseline follow-up scope. Classify them before any validation claim, package handoff, commit, or push.
- Latest beta.87 no-game validation is summarized in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`: 0 build errors and 0 warnings; split no-build lanes covered 139 passed / 0 failed / 15 skipped / 154 total for the migration-focused surface; opt-in artifact/package coverage passed; the latest focused follow-up recapture covered `RuntimeFailureAnalyzer` 20 / 0 / 0 / 20, `RuntimeMonkeyPacketChecker` 20 / 0 / 0 / 20, `AncientUiReadinessGuardTests` 13 / 0 / 0 / 13, and `DocumentationCompactnessGuardTests` 25 / 0 / 0 / 25 after rebuilding, with current-doc claims 1033 / 0, runtime preflight 27 / 0, v19 ledger 534 / 0, v20 overlay 29 / 0, static suite 15 / 0, and static-file hygiene 12 / 0. Revalidate current HEAD in a clean single validation lane again before package handoff if the worktree changes.
- Patch state remains 25 migrated `IPatchMethod` classes, 142 raw `[HarmonyPatch]` declarations, 167 tracked patch units total.
- Historical `v0.106.1` loader-gate runtime proof exists:
  - Off: `.tools\runtime-evidence\smoke-k1-off-20260602-145938`, 0 Sts1Events registrations, clean audit.
  - CanaryOnly: `.tools\runtime-evidence\smoke-k1-canary3-20260602-151104`, exactly 4 canary registrations in the old source shape, clean audit. Current source expects 4 event types through 6 registration calls.
  - AdditiveBatch1: `.tools\runtime-evidence\additive-batch1-20260602-150445`, 10 event types through the old 11 registration calls, clean audit. Current source expects 10 event types through 14 registration calls.
  - Recorded smokes loaded BaseLib, RitsuLib, and Spire Plus, applied 25/25 migrated patches, and observed 30 SavedSpireFields.
- Current `v0.107.0` beta.85 package runtime proof is clean for loader/patch application under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`; installed package parity passed for that package line.
- Current CanaryOnly enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`: 4 event types / 6 registration lines, retained `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`, both 0 mismatches.
- Current AdditiveBatch1 enabled-mode proof is clean under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`: 10 event types / 14 registration lines, retained `enabled-mode-log-check.json` and `runtime-evidence-packet-check.json`, both 0 mismatches. The beta.85 AdditiveBatch1 13/14 verifier mismatch remains root-cause history only.
- Dependency decision is recorded: beta.87 intentionally aligns the compile package and manifest minimum on `STS2.RitsuLib` / `STS2-RitsuLib` `0.4.24`, with BaseLib `v3.2.1` and package version `v0.1.0-private-beta.87`.
- Batch 4c is proposal-only. The current candidate list is `docs/features/ritsulib-migration/batch-4c-candidates.md`; migration requires explicit owner approval and fresh validation.
- Gameplay, Mod Settings UI page refresh, event screenshots, save-load, image/render, replacement functional proof, multiplayer fail-closed proof, independent QA, and tester-package handoff remain pending. Future handoff must recapture HEAD and worktree status after any later edits.
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
5. If owner approves another dependency-floor change, move the repo package reference and manifest minimum in that same versioned package pass and follow the private-beta package-version and validation rules.

## Run Steps

### Step 1: Reconcile Loader Evidence

1. Verify the evidence folders listed above still exist or record that only documentation references remain.
2. Verify the current game version still has a matching RitsuLib variant and that the installed package under test matches the intended artifact; if either check fails, record the blocker instead of launching the game. The beta.87 package artifact currently matches and has clean current-runtime AdditiveBatch1 loader/registration proof under `v0.107.0`.
3. Re-audit the recorded logs if needed with `scripts\audit-godot-log.ps1`.
4. Rerun loader smoke only if runtime compatibility is present and HEAD/package drift makes the old logs stale for the claim being made.
5. Before any StS1 canary gameplay claim, cite the fresh current CanaryOnly packet at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`.
6. Before any AdditiveBatch1 gameplay claim, cite the current beta.87 loader/registration packet at `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` and recapture it if package/source shape changes.
7. If current AdditiveBatch1 loader/registration proof goes stale or cannot be recaptured, record the exact blocker instead of continuing into AdditiveBatch1 gameplay proof.
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

1. Capture current Spire Plus Mod Settings UI evidence. Start with `scripts\collect-mod-settings-evidence.ps1 -NoLaunch`, then rerun the same evidence dir with `-Capture List -RequireSpireForeground` and `-Capture Page -RequireSpireForeground` after manually opening the Mods list and Spire Plus page.
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

- [x] Current STS2-RitsuLib installed locally (`v0.4.24`, `lib\0.107.0`).
- [x] Historical `v0.106.1` Off loader gate proves 0 Sts1Events registrations.
- [x] Historical `v0.106.1` CanaryOnly loader gate proves exactly 4 canary registrations in the old source shape.
- [x] Historical `v0.106.1` AdditiveBatch1 loader gate proves 10 event types through the old 11 registration calls.
- [x] Current `v0.107.0` game install has matching STS2-RitsuLib `v0.4.24` / `lib\0.107.0` runtime files.
- [x] Dependency bump decision recorded: beta.87 compile/manifest floor is `0.4.24`.
- [x] Installed beta.85 package parity verified after the 2026-06-11 package refresh.
- [x] Installed beta.86 package parity verified after the 2026-06-18 package/source alignment pass.
- [x] Installed beta.87 package parity verified after the 2026-06-18 dependency-floor package refresh.
- [x] Clean current `v0.107.0` beta.85 Off loader smoke captured after the prior `v0.4.16` install; retain as previous-package context.
- [x] Current HEAD validation refreshed after the beta.87 dependency-floor pass and runtime packet-checker hardening; recheck again before handoff if any later edits appear.
- [x] Current `v0.107.0` beta.85 CanaryOnly smoke proves 4 event types / 6 registration calls with retained verifier reports.
- [x] Current `v0.107.0` beta.87 AdditiveBatch1 smoke proves 10 event types / 14 registration calls with retained verifier reports; beta.85 13/14 attempt remains root-cause history only.
- [ ] Mod Settings UI evidence captured.
- [ ] CanaryOnly gameplay matrix completed or blocked with evidence.
- [ ] Save/load evidence captured or blocked with evidence.
- [ ] Co-op/fail-closed evidence captured or blocked with evidence.
- [ ] Batch 4c owner decision recorded.

## Hard Stops

- No release-ready, live-ready, or runtime-safe claim from loader logs alone.
- No Batch 4c migration without owner acceptance.
- No high-risk migration in this run.
- No package handoff while validation recapture, gameplay proof, handoff status recapture, or owner decision is missing.
