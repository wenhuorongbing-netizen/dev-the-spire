# Spire Plus Issues
Current target: test-ready manual build, not release-ready. Current package hashes, 2026-05-26:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `891F2ABA04E17D6BE74997D7201D071431A956A070F25E2ACEFEFE9142DF171C` |
| DLL | `B6F91BC3079CAA342C27690E7DCC056E958CF927D41147EE78207EF717553F04` |
| PCK | `C62CBC8B9DAC1C4522FA1A30B4BA35B95C53EDF4628FD6B8135F4B9BBD83281A` |
| Manifest | `E6E708FF4564A1F288391260D9C8FE53C28503A1B2F565CB24E176EC1D1556B7` |
| README_INSTALL | `DCAB3CE1B276CC9093C9AEA86C19D900081E097A7A821F2E0AD6101F113C390D` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` and `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `URDA-SEEDBED-PLANTING` P0 source/package-fixed / live-pending:
  - Seedbed explains Planting / 种下 as a non-play, non-discard, non-exhaust handling action.
  - Planted Blight Sprouts are handled and add no Rootblight I after combat.
  - Planted Rootblight pauses its combat-end check for this combat only and stays in the master deck at the same stage; it does not improve, worsen, split, downgrade, cleanse, or get removed.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending:
  - User log ended during multiplayer combat startup for `CUBEX_CONSTRUCT_NORMAL`, with no managed stack. Treat this as latest Spire Plus crash evidence even if the in-game manifest text still shows an older beta label.
  - Source fix: co-op fails closed before proof for A11-A20 selection/gameplay, Ascension map/rewards, Ancient offers/selections/run hooks, Urda reward alternatives, combat hooks, and preview tools.
  - Retest must use the newly packaged build and leave `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY`, `SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS`, and `SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS` unset unless deliberately debugging.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending:
  - User log `godot2026-05-25T08.54.22.log` is latest crash evidence despite the stale beta label. It shows co-op A20, A14 Rootblight, Urda/Seed Bank, A13 Fission, reconnect, and `Aroma of Chaos` transform sync.
  - Source fix: transform prediction, Crystal Sphere peek, and broader Spire Plus co-op gameplay mutation fail closed by default.
  - Retest needs two-client combat start, transform event, Crystal Sphere if available, reconnect/save-load, and both clients' logs.
- `ENEMY-DAMAGE-POLISH` P1 source-fixed / live-pending: Decimillipede, Terror Eel, and Phantasmal Gardener attack pressure is lightly reduced through damage getter patches so intent and real damage stay aligned; live elite-fight pacing proof remains pending.
- `TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending: Tanx Claws now creates upgraded Maul+ / 撕咬+; live pickup proof still pending.
- `URDA-PROTOTYPE` P0 open / live-pending: default-on with eleven source-backed ids, including Elite Root; live gameplay and save/load proof remain pending.
- `MORVI/LOTHA/VAKUU` P0/P1 live-pending: default-on morvi_forbidden_loan/lotha_death_reprieve need gameplay proof; hidden Vakuu fight needs victory, death/failure, save/load, and no-black-screen proof.
## User manual-test findings, 2026-05-22/24
- `HUSK-CARD-BEHAVIOR`, `ROOT-SIGHT-ENCOUNTER-POOL`, `FIREMARK-HEAL/TEXT` P1: check Husk block, Root Eyes previews, and Act Constant-Heal thresholds.
- `UNKNOWN-EVENT-PREVIEW-READABILITY`, `ROOTBLIGHT-STARTER-MISSING`, `WATERFALL-BOSS-SEAL` P1: check event hover, Rootblight starter deck, and Waterfall Boss.
- `HOURGLASS-BOSS-SEAL-DESIGN`, `QUEEN-BOSS-SEAL-WEAKNESS`, `FIREMARK-OVERFLOW/FORGE-ARMOR`, `BANNER-ROOM-PREVIEW` P1: check Time Sand, Royal Decree cards, Firemark, and Banners.
- `ROOT-EYES-CONFLICTS-COOP`, `PREVIEW-TOOLS-REWARD-HOOKS`, `SEAL-BANNER-VISIBILITY`, `V33-DESIGN-PASS` P1: check co-op Root Eyes, preview rewards, seal visibility, and v3.3 designs.
## Strict source/BaseLib audit no-go findings, 2026-05-20
- `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open; `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2 source-fixed / live-pending.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 source-fixed: current worktree is clean after intentional batches. `GOV-CI-FIRST-RUN`, `DOC-CONFLICT-GOVERNANCE`, and `PLATFORM-PACKAGE-CHECKS` P2 remain pending.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
