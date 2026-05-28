# Spire Plus Code

This is the active C# code for the single `Spire Plus` mod. The folder name remains
`EZMicroBalanceCode` because the stable technical manifest id is still `EZMicroBalance`.

## Module Map

| Area | Responsibility |
| --- | --- |
| `MainFile.cs` | Mod entry point for Harmony patching, config registration, and feature-registry bootstrap. |
| `Config/` | Mod configuration (SpirePlusModConfig). |
| `Core/Features/` | Small module registry that keeps startup order and feature ownership out of `MainFile.cs`. |
| `Core/Integrations/RitsuLib/` | Reserved for future RitsuLib bootstrap module (blocked on version mismatch). |
| `Diagnostics/` | Release evidence log and live test console command. |
| `Map/` | Spire Plus map point hover composer. |
| `Modding/` | Mod info localization patches. |
| `Ancients/` | Ancient reward rebalance implementation. |
| `Ancients/Common/` | Shared saved fields, card helpers, selection relic service, feature-gate helpers, and small model/enchantment helpers. |
| `Ancients/Rebalance/` | Reserved for shared Ancient rebalance helpers. |
| `Ancients/Patches/` | Harmony patches grouped by reward surface or relic family. |
| `Ancients/Expansion/Urda/` | Urda Ancient expansion prototype, blessing ids, and activation gate. |
| `Ancients/Expansion/Morvi/` | Morvi Ancient expansion implementation, cards, powers, blessing ids, and activation gate. |
| `Ancients/Expansion/Lotha/` | Lotha Ancient expansion implementation, powers, blessing ids, and activation gate. |
| `Ancients/Expansion/Vakuu/` | Hidden-by-default Vakuu fight slice, encounter, monster, Temptation card, and fight gate. |
| `Ascension/` | Ascension 11-20 development systems and guarded prototype slices. |
| `Ascension/Core/` | Gates, initialization, diagnostics, asset paths, and saved fields. |
| `Ascension/Map/` | Map generation/mutation services and map markers. |
| `Ascension/Combat/` | Combat trackers, combat modifiers, and combat hooks. |
| `Ascension/Rewards/` | Reward mutation helpers, Forge Token service, Root deck service, and boss dedicated ability definitions. |
| `Ascension/Enchantments/` | Ascension-specific card enchantments. |
| `Ascension/Patches/` | Harmony patches for lobby, map UI, run hooks, and A20 flow. |
| `Ascension/Cards/` | Rootblight, boss dedicated ability, and related card models. |
| `Ascension/Powers/` | Firemark, Banner, and boss dedicated ability powers. |
| `Ascension/Relics/` | Ascension-specific relic models. |
| `Ascension/Events/` | Ascension-specific event models. |
| `Ascension/Save/` | Reserved for Ascension save/load code. |
| `Ascension/Ui/` | Reserved for Ascension UI patches. |
| `Preview/` | Crystal Sphere peek and transform-preview helpers. |

## Extension Rules

- Add behavior beside the feature it belongs to; avoid cross-feature utility files unless there is real shared logic.
- Put saved run fields in the feature's `*SavedStateFields.cs` file and update source guards.
- Prefer BaseLib/template-supported APIs before adding Harmony patches.
- When Harmony is needed, keep patch targets narrow and source-guarded by tests.
- For canonical `AbstractModel` markers/hooks/models, use `ModelDb`; do not construct canonical model instances directly.
- Keep Early Access API references conservative. Avoid hard references to optional boss/power types when a stable `ModelId` check is enough.
- Update localization in `EZMicroBalance/localization/eng/` and `EZMicroBalance/localization/zhs/` with matching keys/placeholders.
- Update feature docs and tests in the same pass as behavior changes.

## Validation

Run `dotnet build` after code changes. Run `dotnet test EZMicroBalance.sln --no-build` after source, localization, docs, or guard changes. Run `dotnet publish` after resource, localization, packaging, or manifest changes.
