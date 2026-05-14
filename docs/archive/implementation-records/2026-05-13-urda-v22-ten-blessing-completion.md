# 2026-05-13 Urda v2.2 Ten-Blessing Completion

Scope:

- Promoted Urda to ten default-on source-backed blessing ids while preserving the `EZMicroBalance` manifest id and `EZMB_DISABLE_URDA=1` disable gate.
- Added Trial Branch, Shallow-Root Relic, Rooted Route, After the Rain, Root-Sight, and Seed Bank source slices, localization, option relics/icons, manual rows, and guard tests.
- Kept the original four Urda blessings intact.

Documented source-safe deviations:

- Trial Branch uses a simple 4-card picker.
- Shallow-Root Relic uses Act 2 removal/refund when not rooted instead of the unproven `lose 6 Max HP` choice UI.
- Rooted Route auto-marks a reachable normal-combat node and does not mutate the map graph.
- Root-Sight auto-marks reachable non-Boss rooms instead of adding a map button.
- Seed Bank stores by consuming the card reward.

Validation:

- `dotnet build EZMicroBalance.sln`: passed.
- `dotnet test EZMicroBalance.sln --no-build`: passed after fixing one stale art-direction guard string.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF warnings only.
- `dotnet publish EZMicroBalance.sln`: passed with known Godot warnings.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed.

No live game, save-load, or co-op testing was run.
