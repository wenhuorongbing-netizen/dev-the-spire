# Ancient Expansion v2.2 Milestone Roadmap

Status: Urda has ten source-backed/default-on v2.2 blessings. Morvi has eight source-backed/default-on v2.2 blessings. Lotha has eight source-backed/default-on v2.2 blessings. Vakuu fight is hidden by default as a dedicated opt-in slice with Temptation. This roadmap is now a historical milestone map; use `source-design.md`, `manual-test-checklist.md`, and `docs/issues.md` for current work.

## Milestone 0: Planning Ingest

Goal: record the v2.2 design without expanding gameplay surface.

Exit:

- `source-design.md` stores the full roadmap.
- `docs/issues/ancient-expansion-v2.2.md` tracks compact issues.
- Current Urda docs track the full ten-blessing source-backed slice.
- Morvi is default-on source-complete/live-pending content; Lotha is default-on source-complete/live-pending content; Vakuu fight is hidden-by-default source-dedicated/live-pending content.

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
- Morvi must keep `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`, force-Ancient, and force-blessing gates until live reward UI/save-load/co-op checks pass.
- Lotha remains live-pending until event UI, gameplay, save/load, co-op, and Death Reprieve lethal-path evidence is recorded.

## Milestone 3: Vakuu Fight Prototype

Goal: prove the optional fight flow in live testing before any broader activation.

Gate:

- Event option insertion source evidence exists.
- Victory parent-event resume and reward-choice source evidence exists.
- Failure/death path source evidence is partial and requires live proof.
- Manual UI/softlock, save/load, and co-op checklist rows remain open.

## Milestone 4: Expansion Pool Hardening

Goal: only after individual prototypes are stable, decide which blessings enter the active private-beta pool.

Gate:

- Every active blessing has source guards and manual rows.
- Disable gates work.
- Save/load and multiplayer stance is documented.
- Release docs name only implemented and live-verified content.

## Milestone 5: Player-Facing Release-Candidate Polish

Goal: convert the current source-complete/live-pending prototype into a build that is worth full manual testing.

Scope:

- Replace temporary crop art with final original Slay-the-Spire-2-style event, map, option/relic, card, status, and power art.
- Rewrite visible player text so each mechanic is understandable from the option, card, power, or hover tooltip.
- Add rich-text/highlight parity for English and Simplified Chinese according to `docs/style/card-localization-style-guide.md`.
- Live-verify Lotha's corrected source-safe v2.2 behavior, including Mirror Hall Echo, Presumption, Closed Court, and the documented Death Reprieve enemy-turn timing deviation.
- Decide whether Morvi becomes default-on for the next test build; if yes, implement the full v2.2 pool, not a partial hidden prototype.
- Implement the remaining Urda roadmap blessings only with source-backed hooks and explicit tests.
- Polish Vakuu fight readability, Temptation/status behavior, failure/death path, and victory reward clarity.

Exit:

- Build/test/format/publish pass.
- Package hashes are refreshed only after resources/code are stable.
- Live Ancient UI/gameplay, save/load, and co-op matrices are either completed or explicitly listed as release blockers.
- No release-ready claim is made without actual runtime evidence.
