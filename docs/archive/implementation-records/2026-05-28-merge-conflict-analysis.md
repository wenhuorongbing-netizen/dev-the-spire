# Merge Conflict Decision Record - 2026-05-28

## Summary

The merge conflict was resolved in favor of the implementation that keeps `Sts1EventHelpers` for shared StS1 event operations.

This was not chosen only for style. The helper-backed branch already implements several player-visible event effects that the direct-call branch still left as TODOs: random relic grants, card removal, card upgrades, transforms, and curse handling. Keeping the helper avoids shipping event options that display a reward but do not actually grant it.

The analysis file was archived here instead of kept at the repository root because root-level scratch reports are not part of the project's governed worktree batches.

## Decisions

| Area | Decision | Reason |
| --- | --- | --- |
| StS1 event model conflicts | Keep the `Sts1EventHelpers` implementation. | The direct `CardPileCmd` branch still had missing relic/removal/upgrade/transform behavior in several events. |
| `Sts1Nest` compile status | Keep it compiled. | With `GrantRandomRelic` and curse helpers, the event no longer needs to stay excluded only because of incomplete reward behavior. |
| `Sts1Duplicator` compile status | Keep it excluded. | Card duplication remains a fragile/unverified surface and should not be enabled during this merge. |
| `Sts1EventRegistrationService` compile status | Keep it excluded. | Unconditional StS1 event registration is still prototype-only and not part of the default Spire Plus runtime path. |
| Simplified Chinese StS1 localization | Keep the remote version. | The remote file contains the complete key set; the local file was only a smaller subset. |
| Patch inventory docs | Keep the generated count as source of truth. | `docs/patch-inventory.md` currently reports 141 total patches and 22 high-risk patches. |
| Unregistered Ancient patch migrations | Restore raw `[HarmonyPatch]` classes. | Those classes were converted to `IPatchMethod` by the remote branch but were not registered in `RitsuLibBootstrap`, so keeping the conversion would silently skip runtime patches. |

## RitsuLib Boundary

Only the patches already registered in `RitsuLibBootstrap.RegisterMigratedPatches` should stay on the `IPatchMethod` path. Unregistered Ancient reward patches must remain on raw Harmony until they are migrated as an explicit batch with bootstrap registration, source guards, and runtime evidence.

## Follow-Up Notes

The helper is intentionally narrow. It is not a normalizer that hides bad state; it is a shared wrapper around source-backed game command APIs for repeated StS1 event operations.

The current merge only proves source compilation. StS1 event live behavior, UI flow, save/load, and multiplayer behavior still need manual/runtime evidence before those events can be described as release-ready.
