# Next Overnight Run — RitsuLib Migration

## Run Date

TBD (next available overnight session)

## Objective

**Runtime Proof + Architecture Integration.** Runtime smoke remains the critical path blocker. Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes. Focus on runtime environment setup, not patch migration.

## Current State

- HEAD: `85a38dd1` on `main` before local refactor-governance edits
- Build: 2026-05-31 clean build passed with 0 errors, 89 warnings (Sts1Events nullable CS8602/CS8604/CS8625)
- Tests: 2026-05-31 full `dotnet test --no-build` passed with 452 passed, 0 failed, 21 skipped (473 total). See `docs/reviews/current-validation.md`.
- 25 patches migrated, 142 raw Harmony remaining
- Architecture canary integration complete (RewardPipeline diagnostics + CardPlayContext depth guard)
- DeathProtectionService + MultiplayerPolicy stubs created (diagnostics-only)
- Runtime smoke: **BLOCKED** — STS2-RitsuLib not installed. 2026-05-31 `Test-Path` checks returned `False` at both `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` and `D:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib`. Batch 4c, Off/CanaryOnly runtime smoke claims, and live-ready claims remain blocked until STS2-RitsuLib is installed and logs are captured.
- FeatureRegistry source hardening: unified ForceEnvKeys/DisableEnvKeys truthy override evaluation was added before bootstrap record creation; this is source-level governance only and does not close runtime smoke.

## Pre-Run Checklist

1. Confirm `main` branch is clean: `git status` shows no uncommitted changes
2. Confirm full test suite passes: `dotnet test EZMicroBalance.sln --no-build` → expected current target is 0 failed
3. Confirm build is clean: `dotnet build EZMicroBalance.sln` → 0 errors; warning count must be recaptured from the current source
4. Confirm format is clean: `dotnet format --verify-no-changes`

## Run Steps

### Step 1: Install STS2-RitsuLib

1. Download STS2-RitsuLib v0.3.2+ from the official source
2. Install to `<GameRoot>\mods\STS2-RitsuLib`
3. Verify BaseLib v3.1.4 is installed at `<GameRoot>\mods\BaseLib`
4. Verify Spire Plus package is installed at `<GameRoot>\mods\EZMicroBalance`
5. Remove all other mods from `<GameRoot>\mods\` for clean smoke

### Step 2: Execute Runtime Smoke

Run the full runtime smoke checklist per `runtime-smoke-checklist.md`:
1. Launch game via Steam
2. Check `godot.log` for RitsuLib bootstrap, ModPatcher, BaseLib, Spire Plus init
3. Verify 25 ModPatcher patches applied
4. Verify SavedSpireFields count (30)
5. Verify Mod Settings UI renders
6. Verify no MissingMethodException, TypeLoadException, or manifest dependency failure

### Step 3: If Runtime Smoke Passes

1. Update `runtime-smoke-checklist.md` with evidence
2. Update `docs/dev-environment.md` with runtime evidence
3. Consider Batch 4c migration (10-15 low-risk patches)

### Step 4: If Runtime Smoke Fails

1. Document exact failure in runtime-smoke-checklist.md
2. Create issue with error excerpts and next action
3. Do NOT proceed to Batch 4c
4. Focus on fixing the runtime blocker

## Success Criteria

- [ ] STS2-RitsuLib installed locally
- [ ] Runtime smoke checklist completed (at least loader smoke)
- [ ] godot.log captured and stored in `docs/evidence/`
- [ ] Batch 4c decision made (proceed or block)

## Risk Mitigation

- **RitsuLib version mismatch**: Check manifest dependency against installed version
- **MissingMethodException**: Check RitsuLib API changes against bootstrap code
- **Game crash on startup**: Check godot.log for exact error, revert to clean state
