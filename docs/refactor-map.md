# Move-Only Folder Refactor Map

This document records the intended directory restructuring for
`EZMicroBalanceCode/`. No files are moved in this phase -- this is a planning
artifact only.

## Current Structure

```text
EZMicroBalanceCode/
  MainFile.cs                  (entry point)
  Config/                      (SpirePlusModConfig)
  Core/
    Features/                  (feature gates, registry, startup sequence)
      IFeatureModule.cs        (feature module interface)
      FeatureRegistry.cs       (registry that sorts and executes modules)
      FeatureGateResult.cs     (gate result record)
      FeatureOrders.cs         (named init-order constants)
      SpirePlusFeatureRegistry.cs (registers named feature modules)
  Diagnostics/                 (release evidence log, live test console cmd)
  Map/                         (SpirePlusMapPointHoverComposer)
  Modding/                     (ModInfoLocalizationPatches)
  Ancients/
    Common/                    (shared Ancient state, card helpers, reward service)
    Patches/                   (Ancient Harmony patches)
    Expansion/
      Urda/                    (Urda Ancient feature + UrdaFeatureModule)
      Morvi/                   (Morvi Ancient feature + MorviFeatureModule)
      Lotha/                   (Lotha Ancient feature + LothaFeatureModule)
      Vakuu/                   (Vakuu fight slice + VakuuFightFeatureModule)
  Ascension/
    Core/                      (Ascension core logic, feature gate, diagnostics + AscensionFeatureModule)
    Map/                       (Ascension map service, markers, deep branches)
    Combat/                    (Ascension combat modifiers, trackers, RootBud hooks)
    Rewards/                   (Ascension rewards, boss seals, forge tokens, RootDeck)
    Cards/                     (Ascension cards: boss seals, RootBud, Root family)
    Powers/                    (Ascension powers: banners, boss seals, firemarks)
    Relics/                    (Ascension relics: forge token)
    Events/                    (Ascension events: A20 courtyard)
    Enchantments/              (Ascension enchantments: fission, royal decree)
    Patches/                   (Ascension Harmony patches)
  Preview/                     (Crystal Sphere peek, transform preview)
```

## Target Structure (When Move Phase Executes)

```text
EZMicroBalanceCode/
  MainFile.cs
  Config/                      (keep at root -- no move needed)
  Core/
    Features/
    Integrations/
      RitsuLib/                (current RitsuLib bootstrap, patcher, and content registration)
    Logging/                   (NEW -- if logging helpers are extracted)
    Multiplayer/               (NEW -- if multiplayer code is consolidated)
  Diagnostics/                 (keep at root -- no move needed)
  Map/                         (keep at root -- no move needed)
  Modding/                     (keep at root -- no move needed)
  Ancients/
    Common/
    Rebalance/                 (NEW -- shared Ancient rebalance logic)
    Patches/
    Expansion/
      Urda/
      Morvi/
      Lotha/
      Vakuu/
  Ascension/
    Core/
    Map/
    Combat/
    Rewards/
    Cards/
    Powers/
    Relics/
    Events/
    Enchantments/
    Ui/                        (NEW -- Ascension UI patches)
    Save/                      (NEW -- Ascension save/load)
    Patches/
  Preview/
```

## Changes Summary

### Completed (2026-05-28)

| Action | Path | Risk | Notes |
| --- | --- | --- | --- |
| NEW file | `Core/Features/FeatureOrders.cs` | Low | Named constants replacing magic numbers 100/200/300/400/500 |
| NEW file | `Ancients/Expansion/Lotha/LothaFeatureModule.cs` | Low | Named IFeatureModule replacing DelegateFeatureModule lambda |
| NEW file | `Ancients/Expansion/Morvi/MorviFeatureModule.cs` | Low | Named IFeatureModule replacing DelegateFeatureModule lambda |
| NEW file | `Ancients/Expansion/Urda/UrdaFeatureModule.cs` | Low | Named IFeatureModule replacing DelegateFeatureModule lambda |
| NEW file | `Ancients/Expansion/Vakuu/VakuuFightFeatureModule.cs` | Low | Named IFeatureModule replacing DelegateFeatureModule lambda |
| NEW file | `Ascension/Core/AscensionFeatureModule.cs` | Low | Named IFeatureModule replacing DelegateFeatureModule lambda |
| REFACTORED | `Core/Features/SpirePlusFeatureRegistry.cs` | Low | Now registers named modules instead of inline lambdas |
| UPDATED | `Ascension/Core/AscensionInitializer.cs` | Low | Added comment clarifying [ModInitializer] is compatibility fallback |
| UPDATED | `docs/architecture/patch-boundaries.md` | Low | Fixed patch count drift; now references patch-inventory.md |
| UPDATED | `docs/refactor-map.md` | Low | Updated structure and patch count references |
| UPDATED | `.github/pull_request_template.md` | Low | Added high-risk patch seam and source-only/live-proof checklist items |

### Planned (Future)

| Action | Path | Risk | Notes |
| --- | --- | --- | --- |
| NEW directory | `Ancients/Rebalance/` | Low | Shared rebalance helpers extracted from Common |
| NEW directory | `Ascension/Ui/` | Low | UI-related Ascension patches (if separated) |
| NEW directory | `Ascension/Save/` | Medium | Save/load-related Ascension code (if separated) |
| NO rename | Root `EZMicroBalance*` files | -- | Hard constraint: do not rename manifest id, project, DLL, PCK, install folder |
| NO move | High-risk patches | -- | See `docs/patch-inventory.md` for current count (157 total, 22 high-risk as of 2026-05-28); do not move with behavior changes |
| KEEP | `Config/`, `Diagnostics/`, `Map/`, `Modding/` at root | -- | No structural benefit to moving; keep flat |

## Constraints

- This map is documentation only. Actual file moves happen in PR3 (move-only
  refactor, no behavior changes).
- High-risk patches (run, room, save, lobby, multiplayer, lifecycle) must not be
  moved in the same PR as behavior changes.
- RitsuLib patcher migration is already active; future large folder moves must not
  be mixed with behavior changes in that integration layer.
- The `EZMicroBalance` manifest id, project name, resource folder, code folder,
  DLL, PCK, and install folder must not be renamed.
