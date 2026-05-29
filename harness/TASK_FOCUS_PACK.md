# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- PR 5 compile/manifest dependency added. Runtime verification pending. Ready for continued PR 6 work.

## Acceptance Criteria (PR 5 - Compile/manifest dependency added; runtime unverified)

- RitsuLib 0.3.2 NuGet package added to csproj
- Manifest dependency added for STS2-RitsuLib
- Build: 0 errors, 87 warnings (all Sts1Events nullable)
- Tests: 361 passed, 21 skipped, 0 failed (382 total)
- Format: clean (verified at Revision F replay)
- migration.md updated
- docs/integrations/ritsulib.md updated

## PR 6 Scope (Next)

- Bootstrap: CreateContentPack entry point
- Diagnostics: RitsuLib version/logging
- Optional settings page via RegisterModSettings
- No existing high-risk content changes

## Related Files Or Modules

- EZMicroBalance.csproj (RitsuLib PackageReference)
- EZMicroBalance.json (manifest dependency)
- docs/migration.md (PR tracking)
- docs/integrations/ritsulib.md (integration docs)
- EZMicroBalanceCode/Core/Integrations/RitsuLib/ (future bootstrap module)

## Risks

- RitsuLib 0.3.2 base package may have runtime API mismatches with v0.106.1
  (compile-time verified clean, runtime unverified — no loader smoke or godot.log evidence)
- Upgrade to compat package when available on NuGet
- Runtime ModPatcher behavior for 25 migrated patches unverified
