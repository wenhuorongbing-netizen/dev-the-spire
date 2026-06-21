# Historical archive.

This prompt is superseded by `docs/test-ready-development-goal.md`. Do not use it as current implementation truth.

# Next Development Prompt: Ancient Expansion v2.2 Implementation Push

Use this prompt for the next Codex development pass.

```text
You are in:

D:\Game\FOTN\dev-the-spire

Goal: continue Spire Plus (`EZMicroBalance` manifest id) Ancient Expansion v2.2 until the next source-complete, test-backed development slice is ready for player testing. This is a code/docs/test/package pass, not a release-ready claim. Do not mark live verification complete unless you actually run the game and record evidence.

Hard constraints:
- Read PROJECT_STATE.md first, then AGENTS.md.
- Read current docs before archived docs.
- Do not change the EzDailyContent manifest id.
- Do not copy official Slay the Spire 2 assets or large decompiled code into the repo.
- Do not implement A21-A30 or a custom character.
- Keep EZMicroBalance independent.
- Preserve `Spire Plus` as the player-facing display name and `EZMicroBalance` as the stable manifest id/package id.
- Prefer previous framework/template APIs before Harmony; use Harmony only where local source proves there is no safer API.
- Any AbstractModel canonical marker/hook/model must come from ModelDb where appropriate.
- Keep experimental Ancient expansion content independently disableable.
- Do not expose Lotha/Vakuu/default-off Morvi content in default private-beta runs unless docs, issue gates, tests, and user approval explicitly say to do so.
- Do not claim release-ready or live-ready without runtime evidence.

Primary references to read:
1. PROJECT_STATE.md
2. AGENTS.md
3. README.md
4. docs/README.md
5. docs/test-ready-development-goal.md
6. docs/PROJECT_MAP.md
7. docs/issues.md
8. docs/issues/ancient-expansion-v2.2.md
9. docs/issues/urda.md
10. docs/features/ancient-expansion-v2.2/README.md
11. docs/features/ancient-expansion-v2.2/source-design.md
12. docs/features/ancient-expansion-v2.2/implementation-plan.md
13. docs/features/ancient-expansion-v2.2/milestone-roadmap.md
14. docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md
15. docs/features/ancient-expansion-v2.2/art-direction.md
16. docs/features/ancient-expansion-v2.2/manual-test-checklist.md
17. docs/features/ancient-expansion-v2.2/api-research.md
18. docs/features/ancient-expansion-v2.2/risk-register.md
19. docs/features/ancient-expansion-v2.2/work-log.md
20. docs/features/ancient-expansion-urda/README.md
21. docs/features/ancient-expansion-urda/source-design.md
22. docs/features/ancient-expansion-urda/implementation-plan.md
23. docs/features/ancient-expansion-urda/manual-test-checklist.md
24. docs/style/card-localization-style-guide.md
25. docs/skills/sts2-godot-mod-development.md
26. source code/src/Core/** as primary local game-source evidence
27. Local previous framework/RitsuLib/template references if present
28. Tutorial index only as secondary reference: https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html

Current source-backed state:
- Urda is default-on private-beta prototype content.
- Active Urda ids: urda_seedbed, urda_humus_pact, urda_molting, urda_moss_map.
- Urda live gameplay/save-load/co-op verification remains pending.
- Morvi has a default-off prototype behind EZMB_ENABLE_MORVI_V22=1.
- Current Morvi prototype ids: morvi_misprint_press, morvi_open_book_exam, morvi_debt_settlement.
- Lotha and Vakuu fight content are planning-only right now.
- Current source defines 16 previous saved-state registrations after Morvi state registration and Urda/Morvi deck mirror state. Current controlled `--force-steam off` smoke reports `Found 16 previous saved-state registrations`; older 13-field smoke evidence is historical only.

Approved art direction:
- Morvi art target: EZMicroBalance/images/events/ezmb_morvi.png. Visual reference: blue-lit lender-scribe court, sealed contract, skeletal hands, ledger/typewriter, central blue eye.
- Lotha art target: EZMicroBalance/images/events/ezmb_lotha.png. Visual reference: dark mirror tribunal, crystal evidence panes, central judge figure, mirror/heart judgment motif.
- Do not copy unconfirmed temp files. If the user has provided explicit local image file paths, copy those exact files to the target paths. If the images only exist as chat previews and no local source file can be identified, document this as pending and continue non-art code work without adding fake placeholder art.

Implementation objective:
1. Art integration, if source files are available:
   - Copy Morvi and Lotha event art into EZMicroBalance/images/events/.
   - Generate/refresh .import files through publish/Godot import.
   - Add both resources to export_presets.cfg.
   - Add tests that fail if active event-art PNGs are missing from export resources.
   - Bind Morvi event portrait to ezmb_morvi.png using source-backed EventModel portrait APIs.
   - If Lotha remains source-disabled, document ezmb_lotha.png as staged art only and do not claim Lotha is playable.

2. Morvi hardening:
   - Audit MorviRunHook against local source code/src/Core/ card-play and reward flows.
   - Confirm Misprint Press cannot recurse, cannot replay Power cards, cannot replay generated clones, and cannot softlock if AddGeneratedCardToCombat fails.
   - Confirm Open-Book Exam only upgrades Attack/Skill reward cards in normal Act 2 combat rewards.
   - Confirm Debt Settlement cannot softlock, cannot pay lethal HP, cannot duplicate payoff reward, and keeps payoff pending until resolved.
   - Add or tighten source guard tests for all of the above.
   - Keep Morvi default-off behind EZMB_ENABLE_MORVI_V22=1.

3. Lotha first default-off prototype:
   - Add a new independent Lotha gate, e.g. EZMB_ENABLE_LOTHA_V22=1.
   - Add forced blessing selection env only for testing, e.g. EZMB_FORCE_LOTHA_BLESSING.
   - Add Lotha to the appropriate act only when the gate is enabled; inspect local act Ancient unlock source first.
   - Implement a conservative first slice only if source evidence supports it:
     - lotha_mirror_rebuttal: source-safe visible rebuttal effect with no Power-card copy/replay.
     - lotha_single_sentence: a clear one-card-turn or one-card-judgment rule that cannot softlock.
     - optionally lotha_public_evidence if debuff/evidence source APIs are clear.
   - Do not implement lotha_death_reprieve until local lethal-damage/death-interrupt source evidence is strong enough.
   - Do not implement broad hidden enemy-action rewrites.
   - Do not implement effects that read and counter the player's deck composition.
   - Use Player-owned saved state if any state is needed.
   - Add English and zhs localization for all visible strings.
   - Add source guard tests and manual checklist rows.

4. Urda backlog:
   - Do not add future six Urda blessings in this pass unless Morvi/Lotha work is fully green and the new scope is explicitly documented.
   - You may fix source-level bugs in current Urda if tests or source review expose them.
   - Keep live/save-load verification pending unless actually tested.

5. Vakuu fight:
   - Keep Vakuu fight planning-only unless every higher-priority item is green and you have explicit source evidence for event option, combat entry, victory reward, failure/death path, and reward ownership.
   - If not implementing, update docs with precise blockers and required source evidence.

6. Documentation:
   - Update PROJECT_STATE.md.
   - Update docs/issues.md and docs/issues/ancient-expansion-v2.2.md.
   - Update docs/features/ancient-expansion-v2.2/api-research.md with source evidence.
   - Update docs/features/ancient-expansion-v2.2/implementation-plan.md and milestone-roadmap.md with actual status.
   - Update docs/features/ancient-expansion-v2.2/manual-test-checklist.md with truthful rows.
   - Update docs/features/ancient-expansion-v2.2/risk-register.md for new risks.
   - Update docs/features/ancient-expansion-v2.2/work-log.md.
   - Update docs/mod-changelog.md with short tester-facing entries.
   - Do not claim private beta ready.

7. Tests and validation:
   - Add source guards for new files, gates, localization, event art resources, no-default exposure, Power-card safety, recursion safety, payoff/reward UI safety, and stale runtime-smoke honesty.
   - Run:
     - git status --short --branch
     - git log -1 --oneline --decorate
     - dotnet build EZMicroBalance.sln
     - dotnet test EZMicroBalance.sln --no-build
     - dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
     - dotnet publish EZMicroBalance.sln if resources/localization/package inputs changed
     - dotnet test EZMicroBalance.sln --no-build again after publish
     - git diff --check
   - If you change release artifact logic, run:
     - $env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'; dotnet test EZMicroBalance.sln --no-build

Success criteria:
- Build passes with 0 warnings/errors.
- Default tests pass.
- Format verification passes.
- Publish passes if resources/localization changed.
- diff-check passes.
- Morvi remains default-off and source-guarded.
- Lotha, if implemented, is default-off and source-guarded.
- Vakuu remains planning-only unless explicitly implemented with source evidence.
- Event art resources are either correctly integrated and exported, or clearly documented as missing source files.
- Docs accurately state what is source-complete, what is default-on, what is default-off, and what still needs live testing.
- Final response lists changed files, commands run, and explicitly says whether live game/save-load/co-op testing was or was not performed.
```
