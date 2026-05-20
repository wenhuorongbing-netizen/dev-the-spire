# Spire Plus Issues
Current target: test-ready manual build, not release-ready.
Current package hashes, 2026-05-20:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `921EEB0468E9D9110405C4AADD958A64E6DED5D648A6DA8346CFD6B2A9956F91` |
| DLL | `CCF61C5E9B69C53C9DFF5E61F0C87913B54A8D1486F97000CBD6125D90903DE4` |
| PCK | `3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392` |
| Manifest | `A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF` |
| README_INSTALL | `BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906` |
## Active blockers
- `URDA-PROTOTYPE` P0 open: default-on with ten source-backed blessing ids and packaged custom Ancient asset paths. Live gameplay and save/load proof remain pending.
- `MORVI-DEFAULT-ON-TEST` P1 live-pending: default-on with all eight v2.2 ids, including `morvi_forbidden_loan`. Live gameplay and save/load proof remain pending.
- `LOTHA-DEFAULT-ON-TEST` P1 live-pending: default-on with all eight v2.2 ids, including `lotha_death_reprieve`. Live gameplay and save/load proof remain pending.
- `VAKUU-FIGHT-TEST` P0 hidden-by-default / live-pending: explicit gate only. Victory, death/failure, active-fight save/load, and no-black-screen proof remain pending.
## Strict source/BaseLib audit no-go findings, 2026-05-20
Verdict: NO-GO for release readiness. Current state remains a manual-test candidate. Do not certify every relic, effect, display, image, monster design, adjustment, or Ancient reward relic logic as bug-free.
- `STRICT-AUDIT-LIVE-EVIDENCE` P0 open: source tests and package checks do not prove clicked UI, hover readability, relic-bar display, combat scenes, monster visuals, or no-black-screen behavior.
- `STRICT-AUDIT-COOP` P0 open: Root Eyes, Vakuu fight, A20, and other high-risk paths remain gated or unproven in multiplayer. Do not advertise full co-op support.
- `STRICT-AUDIT-PREVIEW-TOOLS` P1 open: Crystal Sphere and transform preview are part of Spire Plus with `affects_gameplay=true`; live proof and RNG/reward mutation notes are still missing.
- `STRICT-AUDIT-VAKUU-FIGHT` P0 open: dedicated monster/scene/source exist; victory return, failure/death, active-fight save/load, and no-black-screen behavior still need live proof.
- `STRICT-AUDIT-VAKUU-CULTURE-SAVE` P1 source-fixed / live-pending: custom decimal state uses invariant culture; active-fight save/load still needs live proof.
- `STRICT-AUDIT-PATCH-SURFACE` P1 open: high-risk patches touch `RunManager`, `CombatRoom`, `EventRoom`, save/load, map generation, start-run flow, and A20 reward routing.
- `STRICT-AUDIT-EVIDENCE-LOG` P2 source-advanced / live-pending: opt-in markers now cover high-risk paths; keep open until `godot.log` captures them from live manual runs.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 open: worktree is still too large for safe review/rollback. Follow `docs/month-plan/commit-boundaries.md` before commit or release handoff.
- `GOV-CI-FIRST-RUN` P2 runner-pending: self-hosted `full-local-validation` lane exists; first CI run evidence is still pending.
- Fixed review rows: smoke parity, source-manifest coverage, patch inventory, CI lane, Firemark counters, Forge Armor tracking, co-op gate logs, evidence logs, Root Eyes, and Vakuu save scoping.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI`: capture clicked Ancient UI screenshots/logs for Urda, Morvi, Lotha, and gated Vakuu.
- `LIVE-GAMEPLAY`: run Ancient choices, A11-A20 routes, Rootblight, Root Eyes, Seed Bank, Morvi cards, Lotha rewards, and Vakuu gate/fight.
- `SAVE-LOAD`: prove live save/load for Urda, Morvi, Lotha, Vakuu, Root Sight, Seed Bank, Morvi state, Lotha Death Reprieve, and Rootblight.
- `VAKUU-FIGHT-LIVE`: prove victory return, failure/death path, active/pre-finished save-load behavior, and co-op behavior.
- `CO-OP`: verify multiplayer Ancient/Ascension behavior, especially A12/A14-A20 combat markers, Rooted Route, Root Eyes, and shared reward state.
## Issue detail links
- Current fixed items awaiting retest live in `docs/toreview.md`; latest review notes live in `docs/review.md`.
- Feature rows live in `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, and `docs/issues/waiting-tests.md`.
