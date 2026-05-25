# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-25:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `BD412625FF6BB72B7B493EDAD6D20F793512B6600C1B1B46CCD961AA65B30971` |
| DLL | `29B7A4D505AD1ED68998C5AF9E2FB3E1AEBDEC15FDFE79C9B385DF74DDB8F26C` |
| PCK | `456378240576DB6EF8FF1669A4E2E8B747BB64AC269470635CED1BC0A90ADEFF` |
| Manifest | `5E76B8441D999119110ED6AEC2074F812E80E0F68AF5F675019098EA7174AE99` |
| README_INSTALL | `CCBD00A154028A897BDD71A15B7057FC9DF3E17930DB48312E552D0253121336` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` P0 source/package-fixed / live-pending
- `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
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
- `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open: clicked UI, relic-bar display, combat visuals, preview tools, co-op, and Vakuu fight need live proof.
- `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2: check invariant culture, patch mapping, and evidence logging.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0: worktree has 320 dirty entries, 0 unclassified paths. Stage only by intentional batch.
- `GOV-CI-FIRST-RUN` P2 runner-pending; `DOC-CONFLICT-GOVERNANCE` and `PLATFORM-PACKAGE-CHECKS` P2: CI first run and cross-platform checks.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE`: prove save/load plus Vakuu victory return, failure/death, active/pre-finished save-load, and no-black-screen behavior.
- `CO-OP`: verify multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, save/reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
