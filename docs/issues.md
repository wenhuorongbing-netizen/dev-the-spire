# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-23:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `11CCD08698F72F4A27547E0FB0D4E7793323ED729DA7CFE3F548CC39F4C51120` |
| DLL | `C9030FCDC459E35256B00CE71925854AF44CFE97667DEB07F751CEA540C86522` |
| PCK | `68F6DCCC5564AE402B3FFB1DD9A65B92555B58CF72282E8EC64FF273EEC4E0F8` |
| Manifest | `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6` |
| README_INSTALL | `2441D012F12D0FB81BCAF7E1C99B1E60F18187937B9911D7D4FD54ACC47BCC6A` |
## Active blockers
- `URDA-PROTOTYPE` P0 open / live-pending: default-on with ten source-backed ids; live gameplay and save/load proof remain pending.
- `MORVI/LOTHA/VAKUU` P0/P1 live-pending: default-on `morvi_forbidden_loan`/`lotha_death_reprieve` need gameplay proof; hidden Vakuu fight needs victory, death/failure, save/load, and no-black-screen proof.
## User manual-test findings, 2026-05-22/23
- `HUSK-CARD-BEHAVIOR` P1 source-fixed / live-pending: Husk gains 3 Block from `AfterCardExhausted`; verify card text, tooltip, and timing in game.
- `ROOT-SIGHT-ENCOUNTER-POOL` P0 source-fixed / live-pending: Root Eyes previews the encounter/event queue head, not a random later pool; verify room entry.
- `FIREMARK-HEAL/TEXT` P1 source-fixed / live-pending: Constant-Heal threshold and Power hover show the current 12/24/48 line by act. Verify feel and clarity.
- `UNKNOWN-EVENT-PREVIEW-READABILITY` P1 source-fixed / live-pending: Event preview adds compact option text; verify hover fit/readability in EN/ZHS.
- `ROOTBLIGHT-STARTER-MISSING` P0 source-fixed / live-pending: Rootblight starter and Blight Sprout markers wait for a real deck card, then retry if none exists. Verify repair, Sprouts, growth, cap/split, and save-load in `rootblight-behavior-checklist.md`.
- `WATERFALL-BOSS-SEAL` P1 source-fixed / live-pending: explosion ignores Weak/attack-down and gives affected players 1/2 Vulnerable. Verify timing and death flow.
- `HOURGLASS-BOSS-SEAL-DESIGN` P1 source-fixed / live-pending: Time Sand now clears by energy, adds extra Wither to the next Increasing Intensity, and updates Branded Form Eye Laser intent. Verify timing/save-load.
- `QUEEN-BOSS-SEAL-WEAKNESS` P1 source-fixed / live-pending: Royal Decree marks one visible Bound card per active player. Wrong or missed Bound cards give team-capped Majesty. Verify Queen pacing and co-op display.
- `FIREMARK-OVERFLOW/FORGE-ARMOR` P1 source-fixed / live-pending: one Firemark Host gets the full mark; overflow affects at most 1 secondary enemy. Forge Armor starts on player turns.
- `BANNER-ROOM-PREVIEW` P1 source-fixed / live-pending: Banner hover now names every Banner kind; verify map UI before route commit.
- `ROOT-EYES-CONFLICTS-COOP` P1 source-fixed / live-pending: Root Eyes shares nodes with Firemark, Banner, and Deep Branch markers via stacked hover text plus a small Root Eyes badge. Co-op mutation remains gated.
- `PREVIEW-TOOLS-REWARD-HOOKS` P1 source-fixed / live-pending: Crystal Sphere is UI-mask-only. Transform preview uses forked RNG. Prismatic Gem tracks modifying reward hooks. Verify preview and reward hooks.
- `SEAL-BANNER-VISIBILITY` P1 source-fixed / live-pending: A19/A20 hovers use v4.1 dedicated abilities. Intent refresh covers attack powers; Royal Decree and Time Sand have visible saved-state anchors. Active-fight save/load proof remains pending.
- `V33-DESIGN-PASS` P0 source-fixed / live-pending: Vakuu, Closed Court, Mirror, Rain, Seedbed, and shorter Lotha text are implemented; Seedbed reward reentry is guarded. Verify in game.
## Strict source/BaseLib audit no-go findings, 2026-05-20
Verdict: NO-GO for release readiness. Current state remains a manual-test candidate; do not certify every relic/effect/display/image/monster/Ancient reward as bug-free.
- `STRICT-AUDIT-LIVE-EVIDENCE` P0 open: clicked UI, hover readability, relic-bar display, combat scenes, monster visuals, no-black-screen, preview tools, and co-op need live proof.
- `STRICT-AUDIT-VAKUU-FIGHT` P0 open: dedicated monster/scene/source exist; victory return, failure/death, active-fight save/load, and no-black-screen behavior still need live proof.
- `STRICT-AUDIT-VAKUU-CULTURE-SAVE` P1 source-fixed / live-pending: custom decimal state uses invariant culture; active-fight save/load still needs live proof.
- `STRICT-AUDIT-PATCH-SURFACE` P1 source-mapped / live-pending: high-risk lifecycle patches are mapped in `docs/architecture/patch-boundaries.md` to source evidence plus manual proof; live rows remain open.
- `STRICT-AUDIT-EVIDENCE-LOG` P2 source-advanced / live-pending: opt-in markers cover Ancient reward selection and high-risk paths; keep open until `godot.log` captures them from live manual runs.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 source-advanced / still open: worktree remains large. Current pathspecs are in `.tools/worktree-batches/current`; latest generated snapshot has 151 dirty entries and 0 unclassified paths. Stage only by intentional review batch.
- `GOV-CI-FIRST-RUN` P2 runner-pending; `DOC-CONFLICT-GOVERNANCE` and `PLATFORM-PACKAGE-CHECKS` P2 source-fixed: CI first run and cross-platform live logs remain pending.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use `scripts/collect-ancient-ui-evidence.ps1`.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the `a19-a20-dedicated-boss-abilities` verifier row with the per-Boss checklist, logs, and notes. This is separate from A11 route traversal and cannot be closed by source guards.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE`: prove save/load plus Vakuu victory return, failure/death, active/pre-finished save-load, and no-black-screen behavior. Use `scripts/collect-vakuu-fight-evidence.ps1`.
- `CO-OP`: verify multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, save/reconnect, and preview tools. Use `scripts/collect-coop-evidence.ps1`.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
