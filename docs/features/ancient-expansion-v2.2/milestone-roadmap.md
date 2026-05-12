# Ancient Expansion v2.2 Milestone Roadmap

Status: Milestone 1 source stabilization is active for the current four Urda blessings. Milestone 2 has a default-off Morvi prototype with source guards. Lotha, extra Urda blessings, and Vakuu fight remain planning-only.

## Milestone 0: Planning Ingest

Goal: record the v2.2 design without expanding gameplay surface.

Exit:

- `source-design.md` stores the full roadmap.
- `docs/issues/ancient-expansion-v2.2.md` tracks compact issues.
- Current Urda docs distinguish the active four-blessing slice from the full ten-blessing roadmap.
- Morvi is default-off prototype content; Lotha and Vakuu fight remain planning-only.

## Milestone 1: Current Urda Verification First

Goal: source-harden, then finish live verification for the already source-backed Urda slice.

Scope:

- Seedbed.
- Humus Pact.
- Molting / Withered Husk.
- Moss Map.

Required before closure:

- Source-level Humus Pact reward timing and Seedbed accounting/cost behavior are guarded.
- Live gameplay checks.
- Save/load checks.
- Localization/UI spot checks.
- No Urda-related exception in Steam-client logs.

Do not add more Urda blessings before this milestone is acknowledged.

## Milestone 2: Safe Morvi Prototype

Default-off prototype scope:

| Ancient | Candidate Blessings |
| --- | --- |
| Morvi | Misprint Press, Open-Book Exam, Debt Settlement |

Gate:

- Card and Power safety rules must be guarded first.
- Extra-play recursion must be prevented in tests.
- Morvi must stay behind `EZMB_ENABLE_MORVI_V22=1` until live reward UI/save-load/co-op checks pass.
- Lotha remains planning-only until the missing event-art/background resource path is resolved; Death Reprieve must not start until death-interrupt source evidence is recorded.

## Milestone 3: Vakuu Fight Prototype

Goal: prove the optional fight flow before adding rewards.

Gate:

- Event option insertion source evidence.
- Failure/death path source evidence.
- Victory reward screen source evidence.
- Manual UI/softlock checklist.

## Milestone 4: Expansion Pool Hardening

Goal: only after individual prototypes are stable, decide which blessings enter the active private-beta pool.

Gate:

- Every active blessing has source guards and manual rows.
- Disable gates work.
- Save/load and multiplayer stance is documented.
- Release docs name only implemented and live-verified content.
