# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-23:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `EFDF43EBAD1A8A6AD9263E971B5CC0366E823739BA2660A69BDD747DF3425686` |
| DLL | `BC79A2634F87314046C8C86120E2FD94030FDC650F5B432864E789AA6B888A4A` |
| PCK | `6F5080524B57EC07F750D6DCFB6D6B274C5F5780EA60DBABB0E6B254D26D01C5` |
| Manifest | `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6` |
| README_INSTALL | `C735AA228BBB5CD002BF618334A04483C0013328C82ECC33551C65B0A1165599` |
## Active blockers
- `URDA-PROTOTYPE` P0 open / live-pending: default-on with ten source-backed ids; live gameplay and save/load proof remain pending.
- `MORVI/LOTHA/VAKUU` P0/P1 live-pending: default-on `morvi_forbidden_loan`/`lotha_death_reprieve` need gameplay proof; hidden Vakuu fight needs victory, death/failure, save/load, and no-black-screen proof.
## User manual-test findings, 2026-05-22/23
- `HUSK-CARD-BEHAVIOR` P1 source-fixed / live-pending: Husk gains 3 Block from `AfterCardExhausted`; verify card text, tooltip, and timing in game.
- `ROOT-SIGHT-ENCOUNTER-POOL` P0 source-fixed / live-pending: Root Eyes previews the encounter/event queue head, not a random later pool; verify room entry.
- `FIREMARK-HEAL-THRESHOLD` P2 source-fixed / balance-pending: Constant-Heal interrupt threshold follows v3.3.1 at 12/24/48; verify feel and hover clarity.
- `FIREMARK-POWER-THRESHOLD-TEXT` P1 source-fixed / live-pending: Constant-Heal Power hover now adds the current interrupt damage as a dynamic value; verify it shows 12/24/48 by act in game.
- `UNKNOWN-EVENT-PREVIEW-READABILITY` P1 source-fixed / live-pending: Event preview adds compact option text; verify hover fit/readability in EN/ZHS.
- `ROOTBLIGHT-STARTER-MISSING` P0 source-fixed / live-pending: A14 starter Rootblight now retries before room entry and no longer treats first-map progress as applied; verify current run repair and new A14/A20 starts.
- `WATERFALL-BOSS-SEAL` P1 source-fixed / live-pending: explosion ignores Weak/attack-down and gives affected players 1/2 Vulnerable. Verify timing and death flow.
- `HOURGLASS-BOSS-SEAL-DESIGN` P1 source-fixed / live-pending: Time Sand now clears by energy, adds extra Wither to the next Increasing Intensity, and updates King Brand Eye Laser intent. Verify timing/save-load.
- `QUEEN-BOSS-SEAL-WEAKNESS` P1 source-fixed / live-pending: Royal Decree avoids punishment when played. Wrong or missed Bound cards give Queen Majesty. Verify Queen pacing.
- `FIREMARK-OVERFLOW/FORGE-ARMOR` P1 source-fixed / live-pending: one Firemark Host gets the full mark; overflow affects at most 1 secondary enemy. Forge Armor starts on player turns.
- `BANNER-ROOM-PREVIEW` P1 source-fixed / live-pending: Banner hover now names every Banner kind; verify map UI before route commit.
- `ROOT-EYES-CONFLICTS-COOP` P1 source-fixed / live-pending: Root Eyes shares nodes with Firemark, Banner, and Deep Branch markers via stacked hover text plus a small Root Eyes badge. Co-op mutation remains gated.
- `PREVIEW-TOOLS-REWARD-HOOKS` P1 source-fixed / live-pending: Crystal Sphere remains UI-mask-only, transform preview uses forked RNG, and Prismatic Gem tracks only modifying reward hooks; verify preview and reward-modifier interactions.
- `SEAL-BANNER-VISIBILITY` P1 source-fixed / live-pending: Boss Seal hovers are updated to A19/A20 v4.1 dedicated abilities. Verify hovers/icons.
- `V33-DESIGN-PASS` P0 source-fixed / live-pending: Vakuu, Closed Court, Mirror, Rain, Seedbed, and shorter Lotha text are implemented. Verify in game.
## Strict source/BaseLib audit no-go findings, 2026-05-20
Verdict: NO-GO for release readiness. Current state remains a manual-test candidate; do not certify every relic/effect/display/image/monster/Ancient reward as bug-free.
- `STRICT-AUDIT-LIVE-EVIDENCE` P0 open: clicked UI, hover readability, relic-bar display, combat scenes, monster visuals, no-black-screen, preview tools, and co-op need live proof.
- `STRICT-AUDIT-VAKUU-FIGHT` P0 open: dedicated monster/scene/source exist; victory return, failure/death, active-fight save/load, and no-black-screen behavior still need live proof.
- `STRICT-AUDIT-VAKUU-CULTURE-SAVE` P1 source-fixed / live-pending: custom decimal state uses invariant culture; active-fight save/load still needs live proof.
- `STRICT-AUDIT-PATCH-SURFACE` P1 open: high-risk patches touch `RunManager`, `CombatRoom`, `EventRoom`, save/load, map generation, start-run flow, and A20 reward routing.
- `STRICT-AUDIT-EVIDENCE-LOG` P2 source-advanced / live-pending: opt-in markers cover high-risk paths; keep open until `godot.log` captures them from live manual runs.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 open: worktree is too large for safe review/rollback. Follow `docs/month-plan/commit-boundaries.md` before commit or release handoff.
- `GOV-CI-FIRST-RUN` P2 runner-pending; `DOC-CONFLICT-GOVERNANCE` P2 source-fixed: CI first run is pending; doc authority order now marks old Urda/reference inputs as support evidence.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE`: prove save/load plus Vakuu victory return, failure/death, active/pre-finished save-load, and no-black-screen behavior.
- `CO-OP`: verify multiplayer Ancient/Ascension behavior, especially A12/A14-A20 combat markers, Rooted Route, Root Eyes, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
