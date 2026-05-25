# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-25:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `067C58E7FB6782DC69BFCE003117706EAE55CE741A0093DE057C3CC4DD9CC62E` |
| DLL | `4DAAA7CDEF83F6BCF5FA5B3A096677EE1C8D316325ED73719E3EC9E03F133564` |
| PCK | `850A8538EB3FD7183A9D884E70C1A33BD97AE88AD46BAEBC5B644D265F73A6D1` |
| Manifest | `DEBD899E29D049A9C3D9F0CCA7E9B9B3931FE5E41BCEC557D728115F6AD01B82` |
| README_INSTALL | `674A1AA0255D61B1E740A0ADA71DC7912E72CE8F3D53A17C22BD2FA697BB8B10` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` P0 source/package-fixed / live-pending
- `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
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
