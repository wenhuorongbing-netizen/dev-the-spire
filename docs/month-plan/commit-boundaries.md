# Commit Boundaries

Purpose: turn `GOV-WIP-SPLIT` into a reviewable split plan before any release handoff.

Current status: `GOV-WIP-SPLIT` remains open. GOV-WIP-SPLIT remains open until these batches are separated or owner-accepted. This file is a plan, not a closure.

## Proposed Order

| Batch | Scope | Files | Validation |
| --- | --- | --- | --- |
| 1 | Release planning docs | `docs/goal.md`, `docs/month-plan/**`, `docs/specs/**`, `docs/source-research/**`, `docs/architecture/**` | Docs guards, build/test not required unless tests change. |
| 2 | Governance guards | `.github/**`, `scripts/**`, `tests/*Governance*`, `docs/README.md`, `docs/PROJECT_MAP.md` | `dotnet test EZMicroBalance.sln --no-build --filter EngineeringGovernanceGuardTests`. |
| 3 | Preview tools | `EZMicroBalanceCode/Preview/**`, related localization/tests/docs | Spire Plus build/test/format. |
| 4 | Urda and Ancient reward fixes | `EZMicroBalanceCode/Ancients/Expansion/Urda/**`, `Ancients/Patches/**`, Urda docs/tests | Spire Plus build/test plus focused Ancient/Urda guards. |
| 5 | Morvi/Lotha/Vakuu | `Ancients/Expansion/Morvi/**`, `Lotha/**`, `Vakuu/**`, related tests/docs | Spire Plus build/test plus save-risk/Vakuu guards. |
| 6 | Ascension and Rootdeck | `EZMicroBalanceCode/Ascension/**`, related localization/tests/docs | Spire Plus build/test plus Ascension and Rootdeck guards. |
| 7 | Package/release evidence | `publish/` generated output, release docs, handoff docs | Publish, package, artifact tests, release evidence verifier. |

## Rules

- Keep preview-tool changes reviewable as their own Spire Plus batch.
- Do not mix docs-only planning with gameplay fixes unless the doc is the acceptance record for the same fix.
- Do not close live/manual rows in a commit that has no live evidence folder.
- Regenerate patch inventory in the same batch that adds, deletes, or moves patches.
- Keep `EZMicroBalance` manifest id unchanged.

## Current Next Action

Keep this plan linked from `docs/issues.md` until work is split or the project owner accepts a larger handoff risk.
