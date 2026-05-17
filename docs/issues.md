# Spire Plus Issues

Current target: test-ready manual build, not release-ready.

Latest source pass, 2026-05-17:

- The ten live-test feedback items from `.tools/runtime-evidence/user-feedback-openlogs-20260517-022541/godot.log` have source fixes or source research recorded in `docs/toreview.md`.
- `docs/review.md` keeps the full review notes. The older bulky issue index was archived to `docs/archive/implementation-records/2026-05-17-issues-before-test-ready-review-loop.md`.
- Remaining blockers are manual proof gates, not known source implementation gaps.
- Current package hashes from the latest verified package snapshot: zip `EA0EC3611DC21FD33C9B87E592326A9000ECE593512554D720843D7490CC589C`, DLL `5A9573E2BF3982D9B6F2D4296D2F52345968118FE0D6D17595E499B4A21CE707`, PCK `E6D7E3AA888824C50EAC7A380303D179F4D6AAE6E8BA36E7FD49CBC2C3A10A15`, manifest `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2`, README `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4`.

## Active blockers

| ID | Feature | Severity | Status | Blocker |
| --- | --- | --- | --- | --- |
| URDA-PROTOTYPE | Ancient expansion | P0 | open | Urda is default-on for private-beta testing with ten source-backed blessing ids plus packaged custom Ancient asset paths; live gameplay/save-load verification is still pending, so do not make a release-ready gameplay claim. |
| ANCIENT-CLICKED-UI | Ancient UI | P0 | open | Urda, Morvi, and Lotha clicked screens still need live screenshots/logs. |
| MORVI-DEFAULT-ON-TEST | Ancient expansion Morvi | P1 | live-pending | Morvi is default-on with all eight v2.2 ids, including `morvi_forbidden_loan`; live UI, gameplay, save/load, and co-op evidence remain pending. |
| LOTHA-DEFAULT-ON-TEST | Ancient expansion Lotha | P1 | live-pending | Lotha is default-on with all eight source-safe v2.2 blessings, including `lotha_death_reprieve`; live UI, gameplay, save/load, and co-op evidence remain pending. |
| VAKUU-FIGHT-TEST | Ancient expansion Vakuu | P0 | hidden-by-default / live-pending | Fight Vakuu remains hidden behind explicit enable/force gates. Source no longer stores `ParentEventId` on the active child combat room, avoiding the known Core serialization blocker; live victory return, failure/death, and save/load proof remain pending. |

## Manual Proof Gates

| ID | Area | Status | Next proof |
| --- | --- | --- | --- |
| ANCIENT-CLICKED-UI | Ancient UI | pending | Capture clicked Ancient UI screenshots/logs for Urda, Morvi, Lotha, and Vakuu. `scripts/collect-ancient-ui-evidence.ps1` creates `ancient-ui-evidence-plan.json`, `manual-instructions.md`, and commands such as `spireplus_test_ancient URDA confirm`. This helper and command prepare UI evidence. Keep this section pending until Urda, Morvi, Lotha, and Vakuu clicked-screen screenshots/logs are captured. |
| LIVE-GAMEPLAY | Full mod | pending | User manual run: Ancient choices, A11-A20 routes, Rootblight, Root Eyes, Seed Bank, Morvi cards, Lotha rewards, Vakuu gate/fight. No current live gameplay proof has been collected in this source-only loop. |
| SAVE-LOAD | Ancient state | pending | Urda/Morvi/Lotha state mirrors are source-guarded, but live save/load remains pending. Do not mark live save/load as verified or ready until the user proves it in-game. |
| VAKUU-FIGHT-LIVE | Vakuu | hidden-by-default / pending | Fight Vakuu remains behind explicit enable/force gates and single-player only. Live victory return, no-black-screen path, failure/death path, active-fight/pre-finished save-load behavior, and co-op evidence remain pending. |
| CLAWS-TERM-MAP | Vakuu / Claws wording | source-mapped / manual confirm | The user phrase `原初之爪` / `Primal Claw` has no unique source object. The closest implemented object is base Ancient relic `Claws` / `枯爪`: choose 1 of 4 Curses, then add 2 Wish and 1 upgraded Wish+. If the user meant a different Vakuu reward, a new concrete object name is needed before code changes. |
| CO-OP | Multiplayer | pending | Rooted Route research says shared map movement is host/vote driven; Root Eyes writeback is currently single-player only until host-authoritative preview sync exists. Co-op behavior remains a manual-test and design gate. |

## Art And Text Gates

- Final browser GPTimage2 small art generated this pass.
- No `generic_temporary` or `final_required_before_release` art blockers remain.
- Event backgrounds are active middle-draft resources.
- Live clicked-UI review remains unresolved.
- Current player-facing localization is source-scrubbed, but live UI fit and hover readability still need manual review.

## Issue detail links

- `docs/toreview.md` contains fixed items awaiting user retest.
- `docs/review.md` contains the latest source review.
- `docs/issues/ancient-expansion-v2.2.md` contains feature-level Ancient rows.
- `docs/issues/urda.md` contains Urda-specific rows.
- `docs/issues/waiting-tests.md` contains manual evidence rows.
- `docs/features/ancient-expansion-v2.2/manual-test-checklist.md` is the current manual checklist.

## Closing Rule

Close a row only after source evidence, automated guard coverage, and the relevant manual proof exist. This project can be test-ready without being release-ready.
