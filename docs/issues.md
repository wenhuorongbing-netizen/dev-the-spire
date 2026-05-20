# Spire Plus Issues

Current target: test-ready manual build, not release-ready.

Current package hashes, 2026-05-20:

| Artifact | SHA256 |
| --- | --- |
| ZIP | `CE417F595E2CCE8435C0575D95A3A866CBDA8FD605DE3F40014639E9301EFF62` |
| DLL | `940F1FEA66B01CB54A1CCEC388D4F023693C947395C7B7F9922BF596A8586E1E` |
| PCK | `3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392` |
| Manifest | `A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF` |
| README_INSTALL | `BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906` |
## Active blockers

- `URDA-PROTOTYPE` P0 open: Urda is default-on with ten source-backed blessing ids and packaged custom Ancient asset paths. Live gameplay and save/load proof remain pending. Next: user manual retest.
- `MORVI-DEFAULT-ON-TEST` P1 live-pending: Morvi is default-on with all eight v2.2 ids, including `morvi_forbidden_loan`. Live UI, gameplay, save/load, and co-op proof remain pending. Next: user manual retest.
- `LOTHA-DEFAULT-ON-TEST` P1 live-pending: Lotha is default-on with all eight source-safe v2.2 blessings, including `lotha_death_reprieve`. Live UI, gameplay, save/load, and co-op proof remain pending. Next: user manual retest.
- `VAKUU-FIGHT-TEST` P0 hidden-by-default / live-pending: Vakuu fight is enabled only by explicit gate; active child combat no longer stores `ParentEventId`. Victory, death/failure, and save/load proof remain pending.

## Engineering governance blockers

- `GOV-WIP-SPLIT` P0 open: current worktree is still too large for safe review/rollback. Next: follow `docs/month-plan/commit-boundaries.md` before any commit or release handoff.
- `GOV-CI-FIRST-RUN` P2 runner-pending: self-hosted `full-local-validation` lane exists with explicit `STS2_PATH`/`GODOT_PATH`; first CI run evidence is still pending.
- 2026-05-20 fixed review rows: smoke parity, source-manifest coverage, patch inventory, full local CI lane, Forge Armor, preview RNG, Root Eyes, and Vakuu restore scoping.

## Manual Proof Gates

- `ANCIENT-CLICKED-UI`: capture clicked Ancient UI screenshots/logs for Urda, Morvi, Lotha, and gated Vakuu.
- `LIVE-GAMEPLAY`: run Ancient choices, A11-A20 routes, Rootblight, Root Eyes, Seed Bank, Morvi cards, Lotha rewards, and Vakuu gate/fight.
- `SAVE-LOAD`: prove live save/load for Urda, Morvi, Lotha, Vakuu, Root Sight, Seed Bank, Morvi state, Lotha Death Reprieve, and Rootblight.
- `VAKUU-FIGHT-LIVE`: prove victory return, failure/death path, active/pre-finished save-load behavior, and co-op behavior.
- `CO-OP`: verify multiplayer Ancient/Ascension behavior, especially A12/A14-A20 combat markers, Rooted Route, Root Eyes, and shared reward state.

## Issue detail links

- Do not close rows on source review alone. Close only after source evidence, automated guard coverage, and relevant manual proof exist.
- `docs/toreview.md` contains fixed items awaiting user retest.
- `docs/review.md` contains the latest full review notes and residual risks.
- `docs/issues/ancient-expansion-v2.2.md` contains feature-level Ancient rows.
- `docs/issues/urda.md` contains Urda-specific rows.
- `docs/issues/waiting-tests.md` contains manual evidence rows.
