# StS2 Godot Mod Development Skill

Use this repository-local skill/reference guide when working on EZ Micro Balance or future Slay the Spire 2 Godot/.NET mods in this workspace.

## Operating Rules

- Read `AGENTS.md` first and follow its scope, manifest, artifact, and validation rules.
- Treat `source code/src/Core/` as the primary implementation authority when it is present locally.
- Prefer local BaseLib, RitsuLib, template APIs, and package references before Harmony patches.
- Use the tutorial index only as secondary orientation: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html`.
- Do not copy official game assets into this repository.
- Do not copy large decompiled game code bodies; record only signatures, class names, field names, call paths, and conclusions.
- Do not mutate the legacy `EzDailyContent` manifest id.
- Keep `EZMicroBalance` independent from the legacy scaffold.
- For card text, visible keyword wording, rich text, dynamic variables, card previews, and bilingual terminology, follow `docs/style/card-localization-style-guide.md`.
- Keep Ascension 11-20 exposure aligned with the current release docs; for the current private-beta multiplayer test candidate A11-A20 selection is default-on, with documented emergency disable switches.
- Do not implement Ascension 21-30 in this cycle.
- Do not implement custom characters in this cycle.

## Source Evidence Workflow

For map, UI, reward, combat, save/load, progress, lobby, or hook changes:

1. Inspect the relevant local game source under `source code/src/Core/`.
2. Inspect local BaseLib/RitsuLib/template APIs or package references when available.
3. Record evidence and risk in `docs/features/ascension-11-20/api-research.md` or `docs/features/ascension-11-20/work-log.md`.
4. Prefer command APIs and BaseLib/template hooks over direct state mutation.
5. Use Harmony only after documenting why safer APIs are insufficient.
6. Add source guard tests for fragile patch points or invariants.
7. Add manual checklist rows for runtime behavior.
8. Do not claim live readiness without runtime proof.

Any canonical marker, hook, or model inheriting `AbstractModel` should be obtained from `ModelDb` where the game expects canonical models. Direct constructors are only acceptable for supported runtime/mutable instances such as created cards/relics when the game APIs prove that pattern.

## Testing Rules

- Source guards are not runtime verification.
- Keep clean-clone `dotnet test` from depending on ignored `publish/`, `.zip`, `.dll`, or `.pck` outputs unless the tests clearly skip or are explicitly release-gated.
- Release artifact parity tests should be opt-in or run after a documented publish/package refresh sequence.
- After code/config changes, run `dotnet build EZMicroBalance.sln`.
- After resource/localization/package changes, run `dotnet publish EZMicroBalance.sln` after build succeeds.
- Before release claims, run package/hash verification, controlled smoke, normal Steam-client Mod Settings verification, and the relevant manual matrices.

## Publish And Artifacts

- Do not commit `publish/`, local binaries, `.godot/`, `bin/`, `obj/`, local tooling, or Steam runtime files unless project policy explicitly changes.
- Package hash docs must match actual artifacts when release packaging is refreshed.
- A prior smoke log is stale after source changes that alter SavedSpireFields, hook registration, resources, or package contents.

## Multiplayer

- Treat selection gates, gameplay gates, progress writes, and live co-op verification as separate surfaces.
- Keep per-player systems keyed to `Player`/owner state and test host/client ownership.
- Do not assume source-shaped ownership prevents desync.
- Verify host lobby selection, client view, join/clamp behavior, run start, save/load, and `godot.log` warnings before claiming co-op support.
- If A20 or another high-risk slice remains single-player gated, add tester-visible warning/log/UI text before exposing confusing multiplayer selection.

## Art

- Use original assets only.
- If generated images are used, document prompt/source/hash and check that no official assets, visible text, logos, or unintended numbers are present.
- Keep release-art truth aligned across docs, tests, package, and active resources.
