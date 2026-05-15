# Spire Plus Next Test-Ready Development Goal

This is the single active implementation directive for the next large Codex pass.

Goal: turn the current `Spire Plus` workspace into a release-candidate-quality test build, with the v2.2 Ancient design implemented as completely as source evidence allows, final-looking original art, readable highlighted text/tooltips, coherent package naming, and no stale release-ready claims.

This is not a release-ready claim. Live gameplay, save/load, and co-op status can only be closed with actual runtime evidence.

Latest static evidence, 2026-05-14: the source-audited text correction pass passed JSON parse checks, build, normal tests, format, diff check, publish, package refresh, post-hash rebuild/test, and opt-in artifact tests after correcting Rootblight, Marginal Note, Red Ink Overdraft, Seedbed, After the Rain, Forbidden Loan, and Debt Settlement text. It did not run live game/Steam, clicked Ancient UI, gameplay, save/load, death/failure path, co-op, or final package smoke for this package.

Latest feedback response, 2026-05-15: Ancient reward choices now obtain visible marker relics when selected, so players can inspect the chosen Urda/Morvi/Lotha/Vakuu reward from the relic bar after the event. The package manifest description and `README_INSTALL.txt` now use a short manual-test introduction instead of a long status dump. This still needs live clicked-Ancient UI and gameplay proof.

## Current Focus Todo

Keep the next pass focused on finishing the manual-test candidate, not re-implementing source-complete Ancient slices.

1. Bespoke art remains open only for event-background provenance and live preview.
   - Urda, Morvi, Lotha, and Vakuu have source-backed custom event/icon/option asset paths and browser GPTimage2 oil-repaint small-art assets; post-rebuild live clicked-UI verification is still pending.
   - Final original option/relic/card/power/fallback/Ascension small art is now generated and manifest-tracked. Do not invent additional Image API prompts, hashes, or provenance unless assets are actually generated or supplied.
2. Live/manual proof remains open.
   - Run the Ancient UI/gameplay matrix for Urda, Morvi, Lotha, and the single-player Vakuu fight.
   - No safe automated clicked-Ancient UI path exists yet; use the force-gate evidence protocol in `docs/features/ancient-expansion-v2.2/manual-test-checklist.md`.
   - Include save/load, failure/death path, co-op disposition, A11 traversal, Rootblight visual/save-load proof, and final package smoke before any release-ready claim.
3. Source-red-team follow-up remains open where it needs runtime design work.
   - Vakuu child combat uses a parent-event combat room shape that is not save/load-proven.
   - Lotha Death Reprieve, Morvi Red Ink/Open Book, and Player-field Ancient state still need live restore checks after the source hardening pass.
4. Maintain player-facing polish.
   - Keep English/zhs option, relic, card, and power text truthful to the source implementation.
   - Avoid development wording, stale "missing" claims, and unresolved dynamic expressions.
   - Current source text has been scrubbed for the 2026-05-14 player feedback list. Keep live UI fit and hover readability pending until clicked-screen evidence exists.
5. Fix image placement and aspect fit before treating the build as visually test-ready.
   - Current source scenes use cover-style fit for clicked Ancient backgrounds; final visual acceptance still needs live screenshots.
   - Large Ancient portraits belong in clicked Ancient/event screens; map/run-history thumbnails and option/relic icons must stay separate.
6. Keep reward visibility player-first.
   - Ancient choices may use event option previews, but the selected reward must also become a visible relic unless the design deliberately says otherwise.
   - If a reward is only a hidden saved-field or run hook, add a marker relic or status surface so testers can see what they chose.

## Player Text Scrub Notes, 2026-05-14

The next pass should rewrite the visible text as if it were written by a Slay-the-Spire player/designer, not by an implementation log.

Global rules:

- Use short, plain sentences. Prefer "Choose 1 of 4 cards" over "Choose 1 of 4 common/uncommon class cards" unless rarity/class restrictions are important to the player's decision.
- Use one term for one mechanic. For A12 Chinese text, prefer `火印精英`; avoid mixing `火印宿主` into player-facing level descriptions unless the combat UI specifically needs to explain the marked enemy.
- Keep numbers blue and gameplay nouns/actions gold where the existing rich-text policy supports it.
- Do not include design-analysis phrases such as "setup window", "burst window", "source-safe", "candidate", "host", "fallback", or "route graph".
- Tooltips should explain what happens, not why the implementation is shaped that way.
- Recheck every `EZMicroBalance/localization/eng/*.json` and `EZMicroBalance/localization/zhs/*.json` entry that is shown to the player: Ancient choices, option relics, powers, custom cards/statuses, card reward alternatives, Ascension levels, map hovers, boss seals/brands, and event dialogue.

Specific rewrite targets from user feedback:

- `Trial Branch / 试炼枝条`: shorten to the idea "Choose 1 of 4 cards. It is upgraded and added to your deck. If you do not prove it during the next 3 combats, it is removed." Match the actual source rule exactly; do not over-describe rarity/class filters.
- `Rooted Route / 扎根路线` and `Seed Bank / 种子库`: rewrite in simple player terms and avoid UI/backend wording.
- A12: shorten the level description. Use `火印精英` consistently and remove the confusing `火印宿主` phrasing from the Ascension description.
- A15: describe it as "Act 2 and Act 3 Boss combats bury two Blight Sprouts." Avoid long consequence chains in the level line.
- A16: mention that Banner Rooms have extra rewards.
- A17: describe it as one special route that is more dangerous and more rewarding; avoid "generate branch" wording.
- A18: remove the long "if seen and not played, may add Rootblight" detail from the level description; leave detailed mechanics to card/tooltips.
- A19: say each Boss gains a Royal Seal / 王印 that specially strengthens it.
- A20: say the final Act 3 Boss pressure upgrades Royal Seals into King Brands / 王烙印. Keep the wording understandable for players who do not know internal double-boss terms.
- Holy Daze / 圣昏: replace "turns the stun into a setup window rather than a burst window" with a concrete effect summary.
- Struggle Bait / 挣扎饵: explain exactly what the player sees and what pressure is added; avoid abstract "escape pressure" wording.
- Residual Sample / 残留样本: describe it as the second and third phases keeping part of the previous phase's strength.

Current source status:

- Applied in EN/zhs localization: Trial Branch, Rooted Route, Seed Bank, A12-A20, Firemarked Elite terminology, Blight Sprout timing, Banner Room rewards, Royal Seal/King Brand, Holy Daze, Struggle Bait, Residual Sample, Morvi Open-Book, and key Lotha option/relic hover lines.
- Static guards reject stale development phrases such as `common/uncommon`, `Firemark Host`, `火印宿主`, `setup window`, `burst window`, and `holding area` in player-facing text.
- Live clicked-screen readability remains pending; do not close text polish until Ancient option screens, hover tips, Ascension level text, and combat tooltips have been read in game.

## Documentation Hygiene

The current active reading path is intentionally small. Do not create another large prompt or audit folder unless it replaces this file.

- Active directive: this file.
- Active blockers/TODO: `docs/issues.md`.
- Active v2.2 feature state: `docs/features/ancient-expansion-v2.2/README.md`.
- Historical audit matrices: `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`.
- Completed implementation record: `docs/archive/implementation-records/2026-05-13-spire-plus-source-test-ready-pass.md`.

Manual checklist and completion-audit documents remain in place because automated guards read them, but they are support evidence, not default next-development reading material.

## Naming Rule

- Player-facing name: `Spire Plus`.
- Current stable technical id: `EZMicroBalance`.
- Do not mutate the existing `EZMicroBalance` manifest id in place.
- If the user requires the technical id/package/file names to become `SpirePlus`, implement it as a deliberate migration:
  - create/document the new `SpirePlus` technical identity before first build;
  - decide whether old `EZMicroBalance` saves/config are abandoned or migrated;
  - keep compatibility aliases for existing `EZMB_` env vars during this test cycle unless explicitly removed;
  - add tests for manifest id, package folder, DLL/PCK names, export paths, scripts, and saved-field namespace decisions.
- The safe immediate package-facing improvement is to make the downloadable archive name `SpirePlus-v...zip` while the inner installed folder can remain `EZMicroBalance` until the full migration is implemented.

## Required Reading

Read these before editing:

1. `PROJECT_STATE.md`
2. `AGENTS.md`
3. `docs/test-ready-development-goal.md`
4. `docs/README.md`
5. `docs/PROJECT_MAP.md`
6. `docs/issues.md`
7. `docs/issues/urda.md`
8. `docs/issues/ancient-expansion-v2.2.md`
9. `docs/features/ancient-expansion-v2.2/README.md`
10. `docs/features/ancient-expansion-v2.2/api-research.md`
11. `docs/features/ancient-expansion-v2.2/source-design.md`
12. `docs/features/ancient-expansion-v2.2/implementation-plan.md`
13. `docs/features/ancient-expansion-v2.2/card-and-power-safety-rules.md`
14. `docs/features/ancient-expansion-v2.2/art-direction.md`
15. `docs/features/ancient-expansion-v2.2/risk-register.md`
16. `docs/style/card-localization-style-guide.md`
17. `docs/skills/sts2-godot-mod-development.md`

Do not read archived prompt dumps or archived audit matrices unless a current document explicitly asks for one. `docs/archive/prompts/**` and `docs/archive/feature-audits/**` are historical context, not current scope.

Evidence priority:

1. Local game source: `source code/src/Core/**`.
2. Local BaseLib/RitsuLib/template source or package references.
3. Existing repository code and tests.
4. Tutorial index as secondary orientation only: <https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html>.

Do not copy official game assets or large decompiled code into the repo. Record only signatures, class names, field names, hook paths, and conclusions.

## Subagent Plan

Subagents are useful, but keep ownership disjoint and current-state aware:

- Source Red-Team Reviewer: attack save/load, reward UI reentry, death interruption, multiplayer authority, and accidental default leakage in the already implemented Ancient slices.
- Localization/UI Text Reviewer: keep English and Simplified Chinese text truthful, compact, highlighted, and free of development wording.
- Test Builder: add source, resource, localization, package, stale-claim, gate, recursion, and reward-reentry guards for concrete risks.
- Art Producer: create/integrate original map icons, run-history icons, event backgrounds, option/relic art, card art, and power art only when Image API access or user-supplied source art is actually available; document prompt/source/hash.
- Release Engineer: run build/test/format/publish/package validation only after source/resources are stable.
- Docs Curator: keep active docs compact and archive stale prompt/planning records instead of adding more required reading.

## Image Generation Contract

Current player feedback: the visible Lotha event screen and blessing/relic option art are placeholder-quality. The next pass must generate or integrate final original art before calling Lotha test-ready.

The single operational source for image generation is now `docs/features/ancient-expansion-v2.2/art-generation-prompts.md`. That file owns the templates, target paths, asset concepts, negative prompts, and inspection rules.

Generation mode is mandatory:

```text
generation_mode: GPTimage2
mode: GPTimage2
semantic_model: GPTimage2
```

If `GPTimage2` is unavailable, do not generate final art and do not fall back to a generic image model or older `art_pipeline/prompts/*.md` calibration prompts. Remote gpt4free image calls should go through `scripts/invoke-ancient-art-gpt4free.ps1` or an equivalent wrapper that reads the canonical prompt pack and sends the same `GPTimage2` mode fields. The current local g4f `OpenaiChat` provider exposes the API transport model as `gpt-image`; that transport-name mapping is acceptable only while the request still records `GPTimage2` as the generation mode.

Do not ask the image generator for generic "epic detailed fantasy". The required visual core is:

```text
Slay the Spire 2 inspired dark fantasy roguelike card-game art, hand-painted 2D illustration, rough gouache and oil brush texture, painterly flat colors, strong black silhouette, uneven ink outline, muted navy-purple shadows, small saturated highlights, grotesque but charming fantasy design, readable at small size, not realistic, not anime, not 3D, not overpolished.
```

Art acceptance rules stay short here and detailed in the prompt pack:

- Strong silhouette matters more than detail density.
- Low detail and high recognizability are preferred over polished concept-art rendering.
- Use deep navy, black-purple, gray-green, and dirty brown bases; reserve saturated color for eyes, candles, gems, hearts, seals, or runes.
- Lines and edges must feel hand-painted and imperfect, not clean mobile-game vector art.
- Relics should look like cursed tabletop tokens or card-game icons, not product renders.
- No text, logos, UI, official game assets, watermarks, or release numbers.
- Every generated image must record prompt, source path, target path, SHA256, and whether it is event background, map icon, run-history icon, option art, relic art, card art, or power art.

## Remove From Current Scope

Do not spend time redoing completed work unless it blocks validation:

- Ancient reward rebalance v4.3 baseline.
- Ascension 21-30 and custom characters.
- New Ancient families or new blessing sets beyond Urda/Morvi/Lotha/Vakuu v2.2.
- Re-implementing source-complete Urda, Morvi, Lotha, Vakuu, or Temptation behavior without a concrete source bug.
- Old prompt/audit migration work already archived under `docs/archive/prompts/2026-05/` and `docs/archive/feature-audits/ancient-expansion-v2.2/2026-05-13/`.

## P0: Manual And Runtime Evidence

Current source state:

- Urda is default-on with ten source-backed v2.2 blessing ids and `EZMB_*` plus `SPIREPLUS_*` force/disable aliases.
- Morvi is default-on with eight source-backed v2.2 blessing ids.
- Lotha is default-on with eight source-backed v2.2 blessing ids.
- Vakuu fight is default-on for single-player testing with Temptation injected on turns 1/3/5+.

Required before any release-ready claim:

- Open each Ancient screen live and verify background art, option art, option text, hover tips, expected option count, and no black screen. Current expected counts are Urda four, Morvi three, Lotha three, and the Vakuu fight option only where single-player gates allow it.
- Run the Urda/Morvi/Lotha/Vakuu gameplay matrix in `manual-test-checklist.md`.
- Run save/load tests across player state, deck mirror recovery, Open-Book held cards, Red Ink debt, Lotha Death Reprieve, Vakuu child combat, Temptation draw/exhaust, and failure/death paths.
- Run co-op disposition tests or keep the feature explicitly single-player/live-pending.
- Refresh package smoke only after source/resource changes are stable.

## P1: Source-Red-Team Follow-Up

Keep fixes narrow and evidence-backed:

- Vakuu child combat no longer assigns the known Core-rejected unfinished `ParentEventId` active-combat shape, but live active-fight and prefinished parent-resume restore behavior remains a blocker until tested.
- Lotha Death Reprieve persists pending/active/resolved phase through the deck mirror, but exact active-turn restore is still not source-proven. Do not claim save/load-safe until live restore proves the current shape.
- Morvi Red Ink and Open Book have been source-hardened to recover from visible power/card markers; Red Ink also skips full hands, verifies hand placement, and uses nonlethal unpaid-debt HP fallback. They still require live restore testing.
- Player-level `SavedSpireField<Player,string>` persistence remains unproven; deck-card mirrors are the fallback, and live save/load proof remains required.
- Do not hide bad upstream state with normalizers. Fix the carrier or document the unsupported limitation.

## P1: Art, Text, And Package Polish

- Keep the browser GPTimage2 rebuilt Urda/Morvi/Lotha/Vakuu option/icon/card/power art unless live preview exposes readability or composition defects; do not reintroduce simplified deterministic icons or opaque-background crops.
- Keep event/background art separate from map icons and run-history icons.
- Keep option relic art separate from map thumbnails.
- Clicked Ancient background scenes now use cover-style TextureRect fit; verify title/home/Ancient-screen aspect ratios with live screenshots before calling the art pass complete.
- When the user supplies the next GPT Image prompt, use it as the governing style prompt for new final art and record it in the art manifest. Do not reuse misrouted temporary crops as final relic/card/power art.
- Record prompt/source path/target path/SHA256/asset role for every generated or supplied final asset.
- Keep localization bilingual, short enough for UI, and truthful to source behavior. Prefer player language over implementation language.
- Maintain package/export coverage if resources move.

## P2: SpirePlus Technical Identity Migration

User intent: players should not download something named `EZMicroBalance`.

Implement in stages:

1. Safe package naming:
   - generated zip name becomes `SpirePlus-v0.1.0-private-beta.0.zip`;
   - package README says `SpirePlus`;
   - document that installed technical id is still `EZMicroBalance` until migration.
2. Full technical migration, only if approved by current docs and tests:
   - new manifest id `SpirePlus`;
   - new project/resource/code/package surfaces or a clearly documented migration path;
   - saved-field compatibility decision;
   - env var aliases from `EZMB_` to `SPIREPLUS_`;
   - script/test/package updates;
   - fresh build/publish/runtime evidence.

Do not silently edit `EZMicroBalance.json` id in place.

## P2: Documentation Cleanup

Keep active docs small:

- `docs/test-ready-development-goal.md` is the one active long prompt.
- Feature docs should hold source evidence, implementation plan, manual checklist, and work log, not repeated prompt dumps.
- Archive duplicate prompts under `docs/archive/prompts/2026-05/` with `Historical archive.` at the top.
- `docs/issues.md` stays a compact blocker index.
- Do not delete source-evidence docs; move historical prompts instead.

## Validation Commands

Always run after code/config changes:

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Run after resource/localization/manifest/export changes:

```powershell
dotnet publish EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

Run only after package artifacts are refreshed:

```powershell
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS
```

Runtime verification, if performed, must record:

- exact command/script used;
- enabled mods list;
- relevant `godot.log` path;
- screenshot/evidence paths;
- whether the test covered loader-only, UI, gameplay, save/load, or co-op.

## Final Response Requirements For The Developer

The developer must report:

- subagents used and their findings;
- files changed;
- which features are source-complete;
- which features are default-on for testing;
- which env vars disable or force them;
- build/test/format/publish/package results;
- whether live game testing was performed;
- whether save/load testing was performed;
- whether co-op testing was performed;
- exact remaining blockers.

## One-Shot Prompt

Use this exact prompt for the next implementation pass:

```text
You are in D:\Game\FOTN\dev-the-spire.

Your job is to move Spire Plus from source-complete Ancient expansion toward a manual-test candidate without adding a new feature set. Use subagents where useful. Read PROJECT_STATE.md, AGENTS.md, docs/test-ready-development-goal.md, docs/issues.md, docs/features/ancient-expansion-v2.2/source-design.md, implementation-plan.md, manual-test-checklist.md, risk-register.md, docs/style/card-localization-style-guide.md, and docs/skills/sts2-godot-mod-development.md before editing.

Current source state to preserve unless a concrete source bug is found: Urda has ten default-on v2.2 blessings; Morvi has eight default-on v2.2 blessings; Lotha has eight default-on v2.2 blessings; Vakuu fight is a default-on single-player test option with Temptation on turns 1/3/5+; the technical manifest id remains EZMicroBalance.

Use local source code/src/Core/** as primary evidence, BaseLib/RitsuLib/template references second, and the tutorial only as secondary orientation. Do not run live game/Steam unless explicitly asked and safe. Do not claim release-ready, live-ready, save/load-ready, or co-op-ready without runtime evidence.

Priority work: source-red-team hardening, a full player-facing EN/zhs text scrub, focused guard tests, final-art integration only if real generated/user-supplied assets exist, title/home/Ancient-screen image aspect-fit repair, and compact docs that point to remaining manual work. Known remaining blockers include bespoke art, live Ancient UI/gameplay matrix, save/load, death path, co-op disposition, A11 traversal, Rootblight visual/save-load proof, and final package smoke.

Before editing localization, apply the 2026-05-14 player text feedback in this goal file. Rewrite visible text to be short and understandable for players: simplify Trial Branch, Rooted Route, Seed Bank, A12/A15/A16/A17/A18/A19/A20 level descriptions, Firemarked Elite wording, Holy Daze, Struggle Bait, and Residual Sample. Use one term per mechanic, keep numbers blue and important gameplay nouns/actions gold, and remove implementation/design-analysis wording from tooltips.

Special caution: Vakuu child combat save/load is not proven because Core rejects serialization of unfinished parent-linked combat rooms. Lotha Death Reprieve pending/active state also needs restore proof or a persisted carrier. Morvi Red Ink/Open Book and Player-field Ancient state need live restore validation even after source guards.

Keep docs/issues.md compact. Do not create another giant active prompt or audit folder. Update PROJECT_STATE.md, docs/issues.md, relevant feature docs, and docs/mod-changelog.md only with evidence actually produced.

Run build, tests, format, diff-check, publish after resources/localization/package changes, post-publish tests, and opt-in artifact tests only after package refresh. Final answer must state changed files, fixes, tests/results, skipped live/runtime checks, and remaining blockers.
```
