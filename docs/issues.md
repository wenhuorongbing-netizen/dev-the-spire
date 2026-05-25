# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-25:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `4E838C7EE1A790E65C045A3D72CE572450E66CA54E31616F578D342AEF9F5DC6` |
| DLL | `9D98CD31EEBE27E89299F24EF9AA846C9EAFD5B5ABC876FFB346A136DB45D928` |
| PCK | `E3261B48D134FE57B52D043511093CC4170C699E80E08EDB00D96999CE197BC7` |
| Manifest | `0A537E6EB76B903D2D7A52312F97B332F7421EB1E69C059E946051B7445092CD` |
| README_INSTALL | `40E564BC5AD0D72E2C2E3E5EBD0EE1A2A8188DEB982B29F23527C9249E71C825` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` P0 source/package-fixed / live-pending
- `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending:
  - User log ended during multiplayer combat startup for `CUBEX_CONSTRUCT_NORMAL`, with no managed stack.
  - Source fix: multiplayer combat disables unverified Ascension, Morvi, Lotha, Urda, and Vakuu combat hooks by default, and logs one warning per feature.
  - Debug opt-in: `SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS=1`, legacy `EZMB_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS=1`.
  - The provided log declared `v0.1.0-private-beta.2`; retest must use the current package.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending:
  - User log `godot2026-05-25T08.54.22.log` used `v0.1.0-private-beta.2`; Root Eyes was only offered, while the suspicious path is co-op `Aroma of Chaos` transform plus reconnect/choice-sync.
  - Source/package fix: transform prediction and Crystal Sphere peek fail closed in co-op by default; opt-in env vars exist only for debugging. Retest needs two-client transform, Crystal Sphere, reconnect/save-load, and both logs.
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
