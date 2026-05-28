# TASK_FOCUS_PACK - Current Task Focus

## Current Task

- Implement restructure.md integration prep phase

## Acceptance Criteria

- docs/codex-harness/ templates created from uploaded zip
- harness/ status files created for current task tracking
- docs/integrations/ritsulib.md documents runtime install, variant pack, version mismatch
- Move-only refactor map produced (no actual file moves)
- PROJECT_STATE, docs/PROJECT_MAP, docs/README updated with index entries
- Build, test, format, diff-check all pass

## Related Files Or Modules

- docs/restructure.md (the plan being implemented)
- docs/codex-harness/ (template directory)
- harness/ (live status)
- docs/integrations/ritsulib.md (RitsuLib staging docs)

## Out Of Scope For Now

- Actual source folder moves (PR3 scope)
- RitsuLib hard dependency (PR5 scope)
- Behavior changes of any kind

## Known Facts

- Branch: refactor/integrate-harness-ritsulib-cleanup
- Baseline: build passes, batch script updated to classify docs/restructure.md
- RitsuLib variant pack has 0.103.2, 0.105.1, 0.106.1 -- no 0.106.0

## Unknowns

- Whether RitsuLib 0.106.0 variant exists or 0.106.1 is forward-compatible

## Risks

- None for this docs-only phase

## Verification Method

- dotnet build, dotnet test, dotnet format --verify-no-changes, git diff --check
