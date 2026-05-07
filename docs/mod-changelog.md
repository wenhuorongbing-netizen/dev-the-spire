# EZ Micro Balance Changelog

Append only short tester/player-facing entries here when behavior or release validation changes.

## 2026-05-07

- Ancient rewards: completed the active v4.3 rebalance and major boundary fixes.
- Package structure: `EZ Micro Balance` remains an independent mod with manifest id `EZMicroBalance`.
- Release art: active cover art is original, with no visible text, numbers, logos, or official game assets.
- A11: Wide Tower, Long Road now changes vanilla-looking map geometry only: width +1, Act 1 +1 route row, Act 2 +1 route row, and Act 3 +2 route rows.
- A11: removed the dedicated Long Road marker/icon/hover tooltip; ordinary route nodes still look vanilla.
- A12: Firemarked Elites use dedicated route indicators, a visible Firemark Host, Forge Token rewards, and Heal/Smith token payouts.
- A12: Firemark, Forge Token, and Banner text now uses native rich-text color markup for important values and terms.
- A13: Fission reward appearance rates were raised for development testing, with stricter card eligibility and icon support.
- A14/A15/A18: Rootblight and Blight Sprout were migrated to the v2.0 card-state design and still need live verification.
- A16: Banner Rooms are visible enhanced normal combats with public rule text.
- A17: Deep Branches are optional Act 2/3 side branches in single-player only until route-voting behavior is proven.
- A19/A20: Boss Royal Seal / King Brand map hover text remains available; A20 Dual King Brands gameplay remains single-player gated.
- Multiplayer A20: host selection is enabled only for development testing when the public Ascension gate is on; logs now warn that Dual King Brands / second-boss Brand gameplay is disabled or downgraded in co-op pending live verification.
- Tests: release artifact/package/runtime-smoke checks are opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`; normal developer tests no longer require ignored `publish/` package artifacts.
- Status: current-package controlled `--force-steam off` smoke passed with only BaseLib and EZ Micro Balance enabled and `Found 12 SavedSpireFields`. Normal Steam-client verification, live feature verification, save/load, and live co-op verification remain pending until actually executed.
