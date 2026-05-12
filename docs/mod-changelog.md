# EZ Micro Balance Changelog

Append only short tester/player-facing entries here when behavior or release validation changes.

## 2026-05-12

- Morvi default-off prototype hardening: Misprint Press now cleans up generated copies if they cannot enter combat, and Debt Settlement keeps its final payoff pending until the upgraded-card reward is actually offered.
- Morvi Debt Settlement text now tells players that missing Gold is paid with nonlethal HP.
- Morvi/Lotha event art remains pending because no explicit local source PNGs were available; no placeholder art was added.
- Morvi, the Lender-Scribe now has a default-off Act 2 prototype for focused testing.
- Set `EZMB_ENABLE_MORVI_V22=1` to include Morvi in Act 2 Ancient selection.
- Set `EZMB_FORCE_MORVI_BLESSING=morvi_misprint_press`, `morvi_open_book_exam`, or `morvi_debt_settlement` to force one prototype blessing.
- Morvi prototype blessings: Misprint Press replays your first Attack or Skill each combat, Open-Book Exam upgrades one eligible Act 2 combat reward card, and Debt Settlement trades immediate gold for three later repayments.
- Lotha, Vakuu fight content, and the six future Urda blessings remain unimplemented.
- Live gameplay, save/load, and co-op verification for Morvi remain pending.

## 2026-05-11

- Urda, Loamweaver is now enabled by default for private-beta testing in Act 1.
- Set `EZMB_DISABLE_URDA=1` only when you want to hide Urda for comparison.
- Urda now has first-pass gameplay for Seedbed, Humus Pact, Molting, and Moss Map; live gameplay and save/load testing are still pending.
- Seedbed can trade max HP for Seedlings from normal Act 1 combat card rewards, counts accepted Seedbed choices only, and rewards four accepts with max HP without healing.
- Humus Pact uses a Compost Reward card-reward option for normal Act 1 combat rewards, then opens a small removal plus upgraded-card payoff after the third compost.
- Molting removes one Strike and one Defend, adds two Withered Husk cards, then clears those husks at Act 2.
- Moss Map grants one small Act 1 reward per first visited room type.

## 2026-05-08

- Multiplayer A20: fixed a black-screen run-start crash caused by hard references to optional Early Access Boss Seal types missing from the current installed game DLL.
- Compatibility: Debt patch paths avoid direct assumptions about game members that changed across local Early Access builds. Pumpkin Candle and Door Wedge EZMB overrides are removed from the active v0.105.0 package; Pumpkin Candle is vanilla and Doormaker/Door Wedge is replaced by Aeonglass +5 Strength.
- A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
- Set `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.
- Set `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.
- `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.
- A20 multiplayer selection is not full A20 co-op support. Dual King Brands / second-boss Brand gameplay remains disabled or downgraded in co-op pending live verification.
- Normal Steam-client Mod Settings has RC1 evidence for BaseLib and EZ Micro Balance; controlled smoke passed is not the same as live co-op or gameplay verification.

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
- Status: current-package controlled `--force-steam off` smoke passed with only BaseLib and EZ Micro Balance enabled and `Found 13 SavedSpireFields`. Normal Steam-client verification, live feature verification, save/load, and live co-op verification remain pending until actually executed.
