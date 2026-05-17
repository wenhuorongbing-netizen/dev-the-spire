# Ancient Expansion Pack v2.2

Status: current Urda stabilization is source-backed but live-pending, default-on Morvi v2.2 is source-complete for direct private-beta testing, Lotha is default-on with a source-complete test slice, and Vakuu fight is hidden by default with a dedicated source enemy/scene pending live proof.

## Scope

Ancient Expansion Pack v2.2, "Sowing, Borrowing, and Judgment", is the roadmap for expanding Ancient choices beyond the current Spire Plus private-beta slice.

Current source-backed state to preserve:

- Urda is default-on for private-beta testing.
- Active Urda blessing ids are `urda_seedbed`, `urda_humus_pact`, `urda_molting`, `urda_moss_map`, `urda_trial_branch`, `urda_shallow_root_relic`, `urda_rooted_route`, `urda_after_rain`, `urda_root_sight`, and `urda_seed_bank`.
- Current Urda hooks cover all ten v2.2 blessings in source, including the documented narrower UI paths for Trial Branch, Shallow-Root Relic, Rooted Route, Root-Sight, and Seed Bank.
- Live gameplay and save/load verification for current Urda remains pending.
- Morvi is default-on for private-beta testing with all eight v2.2 blessing ids, custom event art, map/run-history icons, option art, English/zhs localization, disable gates, force-Ancient gates, and force-blessing gates. Live gameplay, save/load, and co-op verification remain pending.
- Lotha is default-on for private-beta testing with all eight v2.2 blessing ids, custom Ancient event art, map/run-history icons, option art, English/zhs localization, disable gates, force-Ancient gates, and force-blessing gates.
- Lotha live gameplay, save/load, co-op, and post-Lotha loader/package validation remain pending.
- Vakuu fight is hidden by default and appears only with `EZMB_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`, or a force-fight gate. It now uses a dedicated Vakuu monster and custom encounter scene in source. The source clears the parent event node before entering child combat to address the reported post-victory black screen risk, but live victory, save/load, failure/death, clicked UI, and co-op evidence remain pending.
- Morvi, Lotha, Urda, and Vakuu small art now use browser ChatGPT/GPTimage2 rebuilt `final_generated` assets for the private-beta art pass; live clicked-UI preview is still pending.

## Documents

| Document | Purpose |
| --- | --- |
| `source-design.md` | Full v2.2 planning design in clean structured form. |
| `milestone-roadmap.md` | Future implementation order and gates. |
| `implementation-plan.md` | Future task packets and required source-evidence flow. |
| `api-research.md` | Current evidence and future API questions. |
| `manual-test-checklist.md` | Manual validation rows retained for guard/reference support; not part of the default next-development reading path. |
| `risk-register.md` | Known design and implementation risks. |
| `card-and-power-safety-rules.md` | Mandatory rules for copy, replay, verdict, and Power-card handling. |
| `art-direction.md` | Approved Morvi/Lotha event-art direction, target paths, and asset hygiene rules. |
| `art-generation-prompts.md` | Canonical `GPTimage2` prompt pack for final Ancient event, option/relic, card, power, and icon art. |
| `art-asset-manifest.json` | Machine-readable art provenance, status, target path, dimensions, SHA256, and prompt tracking. |
| `work-log.md` | Chronological planning log. |

Archived prompt material:

- `docs/archive/prompts/2026-05/codex-ancient-expansion-v22-next-development-prompt.md` is historical only. Use `docs/test-ready-development-goal.md` for current implementation scope.
- `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/` contains the historical overnight audit matrices. Use only for old-finding traceability.

## Hard Boundary

This folder may guide later implementation. Morvi and Lotha are source-complete but not live-verified. Vakuu fight is source-dedicated but not live-verified: it remains opt-in until post-victory, save/load, death/failure, clicked UI, and co-op proof exists. Future work must first acknowledge current blockers, update the related issue row, inspect local `source code/src/Core/`, and add source guards plus manual checklist rows before claiming a feature is live-ready.
