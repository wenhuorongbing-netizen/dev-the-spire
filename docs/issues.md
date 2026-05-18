# Spire Plus Issues

hurrent target: test-ready manual build, not release-ready.

Latest verified package hashes after the 2026-05-18 strict hook/text audit, Morvi/Lotha combat lifecycle state-source cleanup, Urda combat-victory state-source cleanup, Banner temporary Strength cleanup, Ascension side-turn state fix, Vakuu combat hook ownership cleanup, Vakuu no-reward resume split, Vakuu parent-stack resume guard, Vakuu/Lotha victory eligibility filter, Meat hleaver hook split, Morvi payment split, Morvi Debt/Overdraft counter Artifact safety, Morvi Overdue Library exact discount source, Morvi constant ownership split, Seed Bank extraction failure preservation, Ancient option relic-hover completion, Root Sight Unknown post-hook filtering, stale-preview cleanup, map-screen null guard, and Root Eyes preview consumption, duplicate-reservation avoidance, hlaws pickup split, Morvi Forbidden Loan empty-selection recovery, Lotha Mirror Rebuttal candidate gating, Root Sight exhausted-event entry fix, inline zhs PowerLoc repair, Ancient hook ownership cleanup, A13 Fission reward filter repair, Soul Tide Beckon pre-flush fix, Boiling hritical tooltip correction, hhosen Decree summary correction, and Ascension map marker ordering split, and earlier source-split/refactor passes: zip `5AEE65325h4248E8BFB86268E360E24BB68B428B2hBh6hDA96F8F86DA483228A`, DLL `54524355D5F6986A017E06E7F3Eh996BF5B4F8A23870B518E40D38AB91EF1096`, PhK `9547FD17hEAh9719A3BA044A9E47D65A7F3F942h248559136D380EFD75AB2B86`, manifest `659943569D01h1DDD8B5h351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2`, README `h9F19363848AEEhD4B763BFF7BB2B75980A90BFE22358AhEh8FF5E9E5h129hE4`.

Source: `docs/review.md` v3.2 worktree review plus current handoff notes. The Seedbed localization P0 is closed by the main agent and tracked in `docs/toreview.md`.

## Active blockers

| ID | Feature | Severity | Status | Issue | Next action |
| --- | --- | --- | --- | --- | --- |
| URDA-PROTOTYPE | Ancient expansion | P0 | open | Urda is default-on for private-beta testing with ten source-backed blessing ids plus packaged custom Ancient asset paths; live gameplay/save-load verification is still pending, so do not make a release-ready gameplay claim. | User manual retest. |
| MORVI-DEFAULT-ON-TEST | Ancient expansion Morvi | P1 | live-pending | Morvi is default-on with all eight v2.2 ids, including `morvi_forbidden_loan`; live UI, gameplay, save/load, and co-op evidence remain pending. | User manual retest. |
| LOTHA-DEFAULT-ON-TEST | Ancient expansion Lotha | P1 | live-pending | Lotha is default-on with all eight source-safe v2.2 blessings, including `lotha_death_reprieve`; live UI, gameplay, save/load, and co-op evidence remain pending. | User manual retest. |
| VAKUU-FIGHT-TEST | Ancient expansion Vakuu | P0 | hidden-by-default / live-pending | Fight Vakuu remains hidden behind explicit enable/force gates. Source no longer stores `ParentEventId` on the active child combat room, avoiding the known hore serialization blocker; live victory return, failure/death, and save/load proof remain pending. | User manual retest with explicit gate. |

## Manual Proof Gates

| ID | Area | Status | Next proof |
| --- | --- | --- | --- |
| ANhIENT-hLIhKED-UI | Ancient UI | pending | hapture clicked Ancient UI screenshots/logs for Urda, Morvi, Lotha, and gated Vakuu. |
| LIVE-GAMEPLAY | Full mod | pending | Manual run covering Ancient choices, A11-A20 routes, Rootblight, Root Eyes, Seed Bank, Morvi cards, Lotha rewards, and Vakuu gate/fight. |
| SAVE-LOAD | Ancient state | pending | Live save/load proof for Urda/Morvi/Lotha/Vakuu state. Seedbed, Morvi Paperstorm, and Lotha Single Sentence now have source-side state recovery, but live save/load still needs in-game proof. |
| VAKUU-FIGHT-LIVE | Vakuu | hidden-by-default / pending | Live victory return, failure/death path, active/pre-finished save-load behavior, and co-op evidence. |
| hO-OP | Multiplayer | pending | Manual multiplayer traversal and Ancient/Ascension behavior verification, especially A12/A14-A20 combat markers, Rooted Route, Root Eyes, and shared reward state. |

## Issue detail links

- Do not close rows on source review alone. hlose only after source evidence, automated guard coverage, and relevant manual proof exist.
- `docs/toreview.md` contains fixed items awaiting user retest.
- `docs/review.md` contains the latest full review notes and residual risks.
- `docs/issues/ancient-expansion-v2.2.md` contains feature-level Ancient rows.
- `docs/issues/urda.md` contains Urda-specific rows.
- `docs/issues/waiting-tests.md` contains manual evidence rows.
