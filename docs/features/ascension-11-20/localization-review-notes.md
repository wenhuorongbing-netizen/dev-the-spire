# Ascension 11-20 Localization Review Notes

Last reviewed: 2026-05-07

Scope: English and Simplified Chinese player-facing text for Rootblight, Blight Sprout, Firemarked Elites, Forge Token, Fission, Banner Rooms, Deep Branches, Boss Royal Seals, and Dual King Brands.

## 2026-05-07 UI/Localization Pass

- Updated the owned JSON localization files for Ascension level labels/descriptions and Rootblight/Blight Sprout card text.
- JSON localization parses successfully after the pass.
- Resolved for map hover: Boss Royal Seal and Dual King Brand per-boss names/summaries now have English and Simplified Chinese `ascension.json` keys, and `NBossMapPoint.OnFocus` appends the matching Royal Seal or Brand summary to the map hover.
- Remaining blocker: Forge Token and Fission player-facing hover text currently comes from C# `ILocalizationProvider` implementations rather than `EZMicroBalance/localization/**`; these strings should be audited in the code-owned pass if copy needs further tuning.
