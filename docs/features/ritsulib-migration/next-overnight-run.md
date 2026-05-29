# Next Overnight Run — RitsuLib Migration

## Run Date

TBD (next available overnight session)

## Objective

Continue RitsuLib migration by executing Batch 4c (low-risk patches) and preparing evidence backlog reduction for high-risk patches (Batch 5).

## Pre-Run Checklist

1. Confirm `main` branch is clean: `git status` shows no uncommitted changes
2. Confirm full test suite passes: `dotnet test EZMicroBalance.sln --no-build` → 311+ passed, 0 failed
3. Confirm build is clean: `dotnet build EZMicroBalance.sln` → 0 errors
4. Confirm format is clean: `dotnet format --verify-no-changes`

## Run Steps

### Step 1: Identify Batch 4c Candidates

Read `docs/patch-inventory.md` and select 10-15 low-risk patches from the raw Harmony section. Prioritize:
- Patches with narrow, isolated blast radius
- Patches that don't touch run lifecycle, map, save/load, or multiplayer
- Patches in `Ancients/Rebalance/` (reward rebalance patches are good candidates)

Document selected patches in `docs/migration.md` under a new Batch 4c section.

### Step 2: Migrate Selected Patches

For each selected patch:
1. Create a new class implementing `IPatchMethod` in `EZMicroBalanceCode/Core/Integrations/RitsuLib/`
2. Move the patch logic from the raw Harmony class to the new `IPatchMethod` class
3. Register the patch in `RitsuLibBootstrap.RegisterMigratedPatches()`
4. Remove the `[HarmonyPatch]` attribute from the original class
5. Run `dotnet build` to verify compilation
6. Run `dotnet test EZMicroBalance.sln --no-build` to verify no regressions

### Step 3: Update Guard Tests

After migrating each batch:
1. Update `RitsuLibMigrationGuardTests.cs` with new patch counts
2. Add new guard tests for any new patterns introduced
3. Verify double-patch guard still passes (no class has both `IPatchMethod` and `[HarmonyPatch]`)

### Step 4: Update Documentation

1. Update `docs/migration.md` with Batch 4c status and patch count
2. Update `docs/patch-inventory.md` with migrated patches moved to the Migrated section
3. Update `docs/features/ritsulib-migration/monthly-dev-spec.md` with progress
4. Update `RitsuLibBootstrap.cs` comment with new patch count

### Step 5: Final Validation

1. `dotnet build EZMicroBalance.sln` → 0 errors
2. `dotnet test EZMicroBalance.sln --no-build` → all passed, 0 failed
3. `dotnet format --verify-no-changes` → clean
4. `git diff --check` → clean
5. Commit and push all changes

## Success Criteria

- [ ] 10-15 new patches migrated to RitsuLib `IPatchMethod`
- [ ] All guard tests updated and passing
- [ ] Documentation updated (migration.md, patch-inventory.md, monthly-dev-spec.md)
- [ ] Full test suite passes with 0 failures
- [ ] Build, format, and diff clean
- [ ] Changes committed and pushed

## Risk Mitigation

- **Migration breaks a patch**: Revert the specific patch migration, re-run tests
- **Guard test count mismatch**: Update guard test assertions to match new counts
- **Build errors**: Check for missing imports or namespace changes after migration
- **Test failures**: Run tests in isolation to identify which migration caused the failure

## Notes

- Do NOT attempt high-risk patches (Batch 5) in this run — those require runtime evidence first
- If a patch's behavior is unclear, skip it and document why in `docs/migration.md`
- If runtime smoke becomes available, pause migration and execute `runtime-smoke-checklist.md` first
