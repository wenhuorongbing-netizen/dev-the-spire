# Ancient Expansion Pack v2.2

Status: current Urda stabilization is source-backed but live-pending, and Morvi has a default-off Morvi prototype with the latest source guards for v2.2 testing. Lotha, Vakuu fight, and extra Urda blessings remain planning-only.

## Scope

Ancient Expansion Pack v2.2, "Sowing, Borrowing, and Judgment" (`播种、借阅与审判`), is a future roadmap for expanding Ancient choices beyond the current EZ Micro Balance private-beta slice.

Current source-backed state to preserve:

- Urda is default-on for private-beta testing.
- Active Urda blessing ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, and `urda_moss_map`.
- Current Urda hooks cover Seedbed, Humus Pact, Molting, and Moss Map in source.
- Current Urda stabilization has source-hardened Humus Pact reward timing and Seedbed accounting/cost/max-HP behavior.
- Live gameplay and save/load verification for current Urda remains pending.
- Morvi has a default-off v2.2 prototype behind `EZMB_ENABLE_MORVI_V22=1`; Misprint generated-copy cleanup and Debt payoff clearing are source-guarded, but live/save-load/co-op checks are pending.
- Lotha and Vakuu fight content are not active gameplay content.
- Morvi/Lotha event-art source files are still missing locally, so no Morvi/Lotha PNG, `.import`, or export-preset entry is claimed.

## Documents

| Document | Purpose |
| --- | --- |
| `source-design.md` | Full v2.2 planning design in clean structured form. |
| `milestone-roadmap.md` | Future implementation order and gates. |
| `implementation-plan.md` | Future task packets and required source-evidence flow. |
| `api-research.md` | Current evidence and future API questions. |
| `manual-test-checklist.md` | Manual validation rows for later implementation. |
| `risk-register.md` | Known design and implementation risks. |
| `card-and-power-safety-rules.md` | Mandatory rules for copy, replay, verdict, and Power-card handling. |
| `art-direction.md` | Approved Morvi/Lotha event-art direction, target paths, and asset hygiene rules. |
| `next-development-prompt.md` | Long-form prompt for the next implementation pass. |
| `work-log.md` | Chronological planning log. |

## Hard Boundary

This folder may guide later implementation, but it does not authorize turning prototypes into default-on content by itself. Future work must first acknowledge current RC blockers, update the related issue row, inspect local `source code/src/Core/`, and add source guards plus manual checklist rows before claiming a feature is playable.
