# Ancient Expansion v2.2 Implementation Plan

Status: current Urda stabilization is source-backed but live-pending, and a default-off Morvi v2.2 prototype is source-implemented with generated-copy/debt-payoff guards for testing. Lotha, extra Urda blessings, and Vakuu fight work remain planning-only.

## 0. Evidence First

For every future implementation slice:

1. Read `PROJECT_STATE.md`, `AGENTS.md`, and this feature folder.
2. Inspect local `source code/src/Core/` for the exact game flow.
3. Inspect BaseLib/RitsuLib/template APIs and prefer supported APIs over Harmony patches.
4. Record source evidence in `api-research.md`.
5. Add source guard tests before or with implementation.
6. Add manual checklist rows before claiming the feature is playable.

## 1. Current Pass Boundary

Current hard boundaries:

- Do not add Lotha source files until the event-art/background blocker is resolved or a dedicated default-off Lotha slice updates docs and tests first.
- Do not add Vakuu fight source files.
- Do not add new Urda blessing code.
- Do not add Morvi/Lotha/Vakuu to any default pool.
- Change Ascension, Rootblight, Boss Seal, Fission, or multiplayer gameplay.
- Publish/package unless resource/localization/package inputs changed and the required build succeeds first.

Allowed current-pass Urda work:

- Fix source-level bugs in the four active Urda blessings.
- Add source/localization/docs guards for those four blessings.
- Keep all live gameplay and save/load verification rows open until actually tested.

Allowed current-pass Morvi work:

- Keep Morvi default-off behind `EZMB_ENABLE_MORVI_V22=1`.
- Keep the source-backed prototype limited to Misprint Press, Open-Book Exam, and Debt Settlement.
- Do not expose Morvi in default private-beta runs.
- Keep Misprint Press Attack/Skill-only, nonrecursive, and cleanup-safe on generated-copy insertion failure.
- Keep Debt Settlement payoff pending until the payoff reward resolver succeeds.

Lotha blocker for this pass:

- Local Core source supports Act 3 Ancient pool inspection (`Glory.GetUnlockedAncients`) and combat hooks, but local Ancient UI source loads background scenes for Ancient events. No explicit Morvi/Lotha art source file or custom Ancient background scene exists in the repo, so Lotha remains planning-only until the visual/resource path is supplied or intentionally designed.

## 2. Future Task Packet Template

Each blessing implementation packet should contain:

- User-facing rule text.
- Exact source hooks and API evidence.
- Data/state model.
- Disable gate.
- Localization keys.
- Save/load plan.
- Multiplayer ownership stance.
- Source guard test.
- Manual test rows.
- Rollback plan.

## 3. Recommended Future Order

1. Complete current Urda live/save-load verification.
2. Implement card and Power safety guards.
3. Source-guard and live-test Morvi Misprint Press using Attack/Skill-only extra play.
4. Source-guard and live-test Morvi Open-Book Exam as a low-risk reward decision.
5. Source-guard and live-test Morvi Debt Settlement with debt accounting and HP fallback.
6. Prototype Lotha Mirror Rebuttal and Single Sentence after the event-art/background resource path is resolved.
7. Research Lotha Death Reprieve; implement only after death-interrupt proof.
8. Research Vakuu fight option and failure path.

## 4. Acceptance Pattern

A future feature is not complete until all of these are true:

- `dotnet build EZMicroBalance.sln` passes.
- `dotnet test EZMicroBalance.sln --no-build` passes.
- Relevant localization/source guards pass.
- Manual checklist rows are updated truthfully.
- Runtime logs show no related exception.
- Release docs do not overclaim the feature.
