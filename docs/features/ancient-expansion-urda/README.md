# Spire Plus Ancient Expansion - Urda, Loamweaver

This folder tracks the Urda Ancient vertical slice for private beta.

Status: Urda is default-on for private-beta testing with eleven source-backed blessing ids: Seedbed, Humus Pact, Molting, Moss Map, Trial Branch, Shallow-Root Relic, Elite Root, Rooted Route, After the Rain, Root-Sight, and Seed Bank. Historical previous package resource evidence under `.tools/runtime-evidence/urda-pck-resource-load-20260513-123345` verified that the installed PCK resolved the custom scene/icon with 0 `ERROR` / `WARNING` lines; current beta.93 loader proof is RitsuLib-only and does not by itself prove clicked Urda UI. Live gameplay, save/load, co-op, and current UI verification are still pending and tracked through live Urda issues in `docs/issues.md`.

Ancient Expansion v2.2 is the current source of truth for the combined Urda/Morvi/Lotha/Vakuu Ancient expansion in `../ancient-expansion-v2.2/README.md`. This folder remains the Urda-specific evidence and checklist folder.

## Feature Map

- `source-design.md` defines Urda scope, active blessing set, state model, and safety boundaries.
- `api-research.md` records local source/API evidence, command-path assumptions, and unresolved evidence questions.
- `implementation-plan.md` breaks the work into a guarded, testable sequence.
- `manual-test-checklist.md` contains the validation matrix and environment controls.
- `work-log.md` tracks changes, commands, and evidence.

Current active focus from `docs/issues.md`:

- One default-on Act 1 Ancient: `Urda, Loamweaver`.
- Eleven blessing ids exposed for source/manual testing, including Elite Root.
- Emergency gate: `SPIREPLUS_DISABLE_URDA=1`; force gate: `SPIREPLUS_FORCE_ANCIENT=URDA`; blessing force gate: `SPIREPLUS_FORCE_URDA_BLESSING`. Legacy `EZMB_*` aliases still work.
- Live gameplay, save/load, co-op, Rootblight interaction, and final-art verification remain pending.

Out of scope for this feature pass:

- Ascension 11-20.
- Any custom character changes.
- Non-Urda ancient families.

See `docs/PROJECT_MAP.md` for code ownership conventions and `docs/AGENTS.md` for repository hard rules.
