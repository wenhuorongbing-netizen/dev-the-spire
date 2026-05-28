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
  Diagnostics/                 (release evidence log, live test console cmd)
  Map/                         (SpirePlusMapPointHoverComposer)
  Modding/                     (ModInfoLocalizationPatches)
  Ancients/
    Common/                    (shared Ancient state, card helpers, reward service)
    Patches/                   (Ancient Harmony patches)
    Expansion/
      Urda/                    (Urda Ancient feature)
      Morvi/                   (Morvi Ancient feature)
      Lotha/                   (Lotha Ancient feature)
      Vakuu/                   (Vakuu fight slice)
  Ascension/
    Core/                      (Ascension core logic, feature gate, diagnostics)
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
      RitsuLib/                (NEW -- future RitsuLib bootstrap module)
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

| Action | Path | Risk | Notes |
| --- | --- | --- | --- |
| NEW directory | `Core/Integrations/RitsuLib/` | Low | Future RitsuLib bootstrap; no behavior change |
| NEW directory | `Ancients/Rebalance/` | Low | Shared rebalance helpers extracted from Common |
| NEW directory | `Ascension/Ui/` | Low | UI-related Ascension patches (if separated) |
| NEW directory | `Ascension/Save/` | Medium | Save/load-related Ascension code (if separated) |
| NO rename | Root `EZMicroBalance*` files | -- | Hard constraint: do not rename manifest id, project, DLL, PCK, install folder |
| NO move | High-risk patches | -- | 137 Harmony patches, 22 high-risk; do not move with behavior changes |
| KEEP | `Config/`, `Diagnostics/`, `Map/`, `Modding/` at root | -- | No structural benefit to moving; keep flat |

## Constraints

- This map is documentation only. Actual file moves happen in PR3 (move-only
  refactor, no behavior changes).
- High-risk patches (run, room, save, lobby, multiplayer, lifecycle) must not be
  moved in the same PR as behavior changes.
- RitsuLib patcher migration (PR6+) must not be mixed with large folder moves.
- The `EZMicroBalance` manifest id, project name, resource folder, code folder,
  DLL, PCK, and install folder must not be renamed.
