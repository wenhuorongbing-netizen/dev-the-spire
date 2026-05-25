# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-24:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `D8C7BB29B45FDA669BC2345B133EF0277E97004B1B6B53992DFF4FAC55F466F4` |
| DLL | `F11BD12A9E60327B07BB5965B777C216684766A310C2CCB8B9D98147C37057A0` |
| PCK | `42D98CA1DBEDAE9874A7302FBBC8596BF4D971A4B01BDB55A18329699D9046BE` |
| Manifest | `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6` |
| README_INSTALL | `33263ACDEEE8F46DD89FFCF649A259B190805C992F743BC3DC07F716FD212FAA` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` P0 source/package-fixed / live-pending
- `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
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
