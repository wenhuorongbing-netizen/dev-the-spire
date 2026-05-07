# EZ Micro Balance Completion Audit

Last updated: 2026-05-07

## Objective

Concrete deliverables for the current goal:

- Produce an independent `EZMicroBalance` private-beta package without mutating the legacy `EzDailyContent` manifest id.
- Bring Ancient reward rebalance implementation, localization, art, docs, package, and automated tests into agreement with the v4.3 source design. v4.3 is current.
- Keep Ancient v4.3 behavior stable while Ascension 11-20 develops separately. A11-A20 single-player and host-multiplayer selection patch is implemented but private-beta default-disabled, with full live Ascension and co-op verification pending.
- Rebuild and hash-check the final package from installed artifacts.
- Pass every automated gate.
- Mark every non-automatable gate pending unless it was actually executed, with enough evidence for the next tester to reproduce it.

## Prompt-To-Artifact Checklist

| Requirement | Evidence | Status |
| --- | --- | --- |
| Preserve existing `EzDailyContent` manifest id | `EzDailyContent.json` still uses id `EzDailyContent`; automated test `ActiveManifestHasStableReleaseIdentity` checks this. | Pass |
| Independent mod structure for plug-in / plug-off behavior | `EZMicroBalance.csproj`, `EZMicroBalance.json`, `EZMicroBalance/`, and `EZMicroBalanceCode/`; `docs/architecture-ez-micro-balance.md`. Controlled smoke loads with legacy `EzDailyContent` disabled. | Pass |
| Stable new manifest id documented | `EZMicroBalance.json` uses id `EZMicroBalance`; architecture docs record the decision. | Pass |
| Required input docs exist and were audited | `AGENTS.md`, `README.md`, `docs/architecture-ez-micro-balance.md`, `docs/dev-environment.md`, `docs/test-plan.md`, `docs/release-checklist.md`, Ancient v4 source/plan/matrix/audit, and Ascension 11-20 source/research/plan/checklist/work-log all exist and were inspected during the final audit. | Pass |
| v4.3 adjustment plan archived | `C:\Users\Jack\Downloads\sts2_ancients_rework_v4_3_adjustment_plan.md` was copied into `docs/features/ancients-rework-v4/sts2_ancients_rework_v4_3_adjustment_plan.md` for repository-local traceability, then annotated to clarify current implementation status and pending runtime/gameplay verification. | Pass |
| v4.2 next plan preserved as historical | `docs/features/ancients-rework-v4/sts2_ancients_rework_v4_2_next_plan.md` remains in the repository for history. v4.2 is historical; v4.2 rightmost-slot Prismatic Gem is historical only. v4.2 Distinguished Cape 40% min15 is historical only. | Pass |
| P0 art drift repaired | Active `EZMicroBalance/mod_real.png` is absent. The text/numbered source image is archived at ignored/source-only `art_pipeline/marketing/EZMicroBalance-mod-real-text-numbered-source.png`. Active `EZMicroBalance/mod_image.png` remains the exported cover. | Pass |
| Export/package/tests agree on art policy | `export_presets.cfg` exports `res://EZMicroBalance/mod_image.png` and not `mod_real`; package/PCK guard tests reject active or packaged `mod_real`; active art hash is recorded below. | Pass |
| Parallel subagent plan executed | Spec Auditor, Ancient Builder, Ascension Builder, Art/UX Producer, Test Builder, Reviewer, and Release Engineer all returned scoped findings; P1 reviewer finding on Ascension map metadata was fixed before final validation. | Pass |
| Active project isolated from legacy sources | Automated test `ActiveProjectDoesNotCompileOrPackageLegacySources` verifies the active project compiles `EZMicroBalanceCode`, solution excludes `EzDailyContent.csproj`, and export preset excludes legacy/source/docs/archive paths. | Pass |
| Ancient reward rebalance implemented or deferred | Active patches under `EZMicroBalanceCode/Ancients/`; implementation notes in `api-discovery.md` and `work-log.md`. No source-design item currently marked API-blocked for v4.3. | Pass pending live behavior verification |
| Full Ancient manual matrix is represented | `manual-verification-matrix.md` covers Pael's Horn, Black Star, War Hammer, Jewelry Box, Preserved Fog/Folly, Claws, Choices Paradox, Jeweled Mask, Prismatic Gem, Pael's Tooth, Sovereign Blade/Forge, Seal of Gold/Debt, Sozu, Ectoplasm, Fiddle, Iron Club, Brilliant Scarf, Beautiful Bracelet, Music Box, Crossbow, Toasty Mittens, Whispering Earring, Pumpkin Candle, Meat Cleaver, Blood-Soaked Rose/Enthralled, Distinguished Cape, Velvet Choker, and zhs numeric formatting. | Pass pending live gameplay results |
| Jewelry Box adds actual non-Innate Apotheosis | `VakuRewardPatches.cs` marks only Jewelry Box-created `Apotheosis`, filters `Innate` only for marked instances, and writes/restores the marker through `SerializableCard.Props`. `JewelryBoxApotheosisNonInnateMarkerIsInstanceScopedAndSerializable` guards source/docs coverage. | Pass pending manual deck/opening-hand/save-load verification |
| Prismatic Gem v4.3 all-off-color reward replacement | `PrismaticGemPatches.cs` uses `CardReward.Populate()` context plus `ConditionalWeakTable<CardReward, RewardScreenState>`; the saved counter increments once per standard reward screen, rerolls reuse the same trigger decision, and trigger screens replace every visible reward option with off-color cards. `api-discovery.md` records `CardReward.Reroll()` evidence; automated tests guard source, installed `_banner` API shape, hover/banner hints, detached-banner rejection, `UI/Banner` fallback diagnostics, fallback evidence, and manual coverage. | Pass pending manual gameplay verification |
| Existing Prismatic Gem exclusions preserved | `PrismaticGemPatches.cs` filters normal encounter rewards only and skips custom pools, filters, colorless-only pools, no-pool/no-model-modification, and non-screen contexts. | Pass pending manual elite/boss/event verification |
| Velvet Choker v4.2 soft limit implemented | `VakuRewardPatches.cs` no-ops the hard cap, counts only non-autoplay first manual card-play series from the owner's hand, resets each player turn, and applies +1 energy to the 7th+ from-hand plays through `CardEnergyCost.GetWithModifiers(...)` after other cost changes. X-cost handling requires the extra energy without increasing captured X. | Pass pending manual card-play/runtime verification |
| Distinguished Cape v4.3 max-HP math implemented | `VakuRewardPatches.cs` computes `max(ceil(currentMaxHp * 0.30), 18)`, replaces an unaffordable Vakuu Cape roll with a payable Pool 2 option instead of shrinking choices, clamps current HP with `CreatureCmd.SetCurrentHp(...)` when needed, then calls `CreatureCmd.LoseMaxHp(...)` and adds three `Apparition` cards. The cost is not routed through damage. | Pass pending manual pickup verification |
| Resolve legacy no-Ascension scope conflict | The 2026-05-06 overnight sprint goal opens Ascension 11-20 as implementation work. A11-A20 single-player and host-multiplayer selection patch is implemented but private-beta default-disabled; Ascension 21-30 and custom character work remain out of scope. Rootblight/Blight Sprout implementation is isolated under `EZMicroBalanceCode/Ascension/` and guarded by tests. | Pass for development build |
| Ascension safe slices implemented | `EZMicroBalanceCode/Ascension/` implements A11-A20 single-player and host-multiplayer selector expansion plus A11 +1 map column with an inserted-column optional route and extra route rows by act (Act 1 +1, Act 2 +1, Act 3 +2) without A11-specific map markers, A12 Firemarked Elite/Forge Token, A13 Fission, A14 Root, A15 Boss Blight Sprout, A16 Banner Rooms, A17 optional Act 2/3 Deep Branches, A18 Elite Blight Sprout, A19 Boss Seals/fourth reward, and A20 vanilla double-boss metadata/Brand-parameter/recovery/reward-screen/courtyard-event hooks. A bespoke A20 full custom intermission remains deferred with blockers in Ascension docs. | Pass for development build |
| A11-A20 selector is constrained | Automated source guards require the selector patch to stay on standard single-player and host-multiplayer lobby paths and avoid global `CharacterStats` getter, `ProgressState`, `ProgressSaveManager`, `NAscensionPanel`, or `AscensionManager.maxAscensionAllowed` patches. | Pass |
| Ascension 21-30 and custom character excluded | `release-checklist.md`, `README.md`, and source guards document/exclude A21-A30 and custom-character work. | Pass |
| Debug probes removed or gated | Active project compiles `EZMicroBalanceCode/**/*.cs`; legacy `AncientRewardNoopProbe` is gated behind `EZ_MICRO_BALANCE_DEBUG_PROBES`. | Pass |
| `.cs.uid` tracking policy consistent | Every Godot-imported C# source under `EZMicroBalanceCode/` and `EzDailyContentCode/` has a `.cs.uid` companion. Test project C# files are outside Godot import scope and intentionally do not use `.cs.uid`. | Pass |
| No original StS2 assets copied into active package | PCK audit test excludes source/docs/art/archive folders; active package contains selected template mod resources, localization, and original generated `mod_image.png` art. | Pass |
| No large decompiled game code bodies copied | Active code is small Harmony patches/helpers; local API evidence is summarized in docs rather than copied as game method bodies. Local `source code/` decompile/reference scratch material is ignored by `.gitignore` and excluded from export so it is not commit-eligible or packaged. | Pass by active-source/package review |
| No accidental build/package artifacts in active release set | The final untracked-file audit is broad because the migration is still uncommitted, but the release package/PCK guards exclude source, docs, art pipeline, archives, build outputs, local tooling, and `source code/`. Active `EZMicroBalance/` resources contain only images, `.import` metadata, and localization JSON. | Pass for active package; worktree still dirty |
| Historical planning/research docs preserved | All 13 deleted tracked planning docs have archive copies under `docs/archive/legacy-planning/`; normalized line-content comparison against `HEAD` passed for every archived copy. | Pass |
| Current-facing docs do not describe legacy `EzDailyContent` as the active release | `BETA_COMPATIBILITY.md`, `REMOTE_DEVELOPMENT_SETUP.md`, and `manual-test-checklist.md` now reference `EZMicroBalance`; `SETUP_SPEC.md` is explicitly marked historical. Manual checklist also requires legacy `EzDailyContent` to be disabled or absent. | Pass |
| English localization valid and current | `LocalizationJsonIsValidUtf8AndKeyCompatible` parses active localization and checks key parity where applicable. | Pass pending in-game text spot check |
| Simplified Chinese localization valid UTF-8 and current | Automated tests cover `zhs` JSON/key parity, banned English leftovers, the Beautiful Bracelet `杩呴€?` wording, Jeweled Mask custom enchantment zhs text, and no-space player-facing number formatting. | Pass pending in-game text spot check |
| Build succeeds | Latest default Debug solution build passed with 0 warnings and 0 errors. Default `dotnet build` no longer overwrites installed release artifacts; `dotnet publish` remains the release install/copy path. | Pass |
| Tests pass | Latest solution-level no-build run of the expanded automated guard suite is refreshed in the evidence section below. The suite now covers source manifest drift, package hash parity, release art audit wording, source-declared localization keys, stale current-facing docs, Ascension selector constraints, unsupported-system completion claims, handoff evidence, Velvet Choker counting, Distinguished Cape v4.3 max-HP math/selectability, Prismatic Gem v4.3 all-off-color rewards, and zhs no-space numeric formatting. | Pass after validation refresh |
| Publish succeeds | Latest `dotnet publish EZMicroBalance.sln` passed, built `EZMicroBalance` in Release, copied DLL/manifest, and skipped publishing tests. The installed PCK is current from the selected-resource export. | Pass |
| Published artifacts exist | Installed `mods/EZMicroBalance` contains `.json`, `.dll`, and `.pck`; automated tests check PCK contents, installed manifest parity, and DLL parity. | Pass |
| Private-beta package created | Current rebuilt zip `publish\EZMicroBalance-v0.1.0-private-beta.0.zip` has SHA256 `6A5273519B2FD8F4D0256EA755D1E07525E7D185BEF9D0A607EEF261F4F81427` and was rebuilt from installed artifacts with matching staging/versioned/extracted zip DLL/JSON/PCK hashes. | Pass |
| Harmony patch targets resolve | Automated test `HarmonyPatchesResolveAgainstInstalledGameApi` calls `Harmony.PatchAll` on installed DLL against local game/BaseLib assemblies. | Pass |
| Game loads the mod | Prior bounded `--force-steam off` smoke loaded only BaseLib and EZ Micro Balance, registered 9 SavedSpireFields, finished EZ Micro Balance initialization, and reached main menu. That smoke predates the current 12 SavedSpireField source/package and must be refreshed before any private-beta readiness claim. | Prior controlled pass; current-package and normal Steam-client passes pending |
| BaseLib loads | Controlled smoke and release checklist record BaseLib initialization. | Controlled pass; Mod Settings pass pending |
| EZ Micro Balance appears and can be enabled | Not verified through normal Steam-client Mod Settings UI. | Pending |
| Every implemented Ancient reward change has manual checklist/result | Checklist exists in `manual-verification-matrix.md`; results remain pending. | Pending |
| Save/load-sensitive behavior verified | Matrix rows exist for Prismatic Gem, Pael's Tooth, Jeweled Mask, Debt, and Folly. | Pending |
| Multiplayer behavior verified | Ancient and Ascension manual checklists include ownership/desync checks, but no multiplayer runtime pass was executed. | Pending |
| Disable-mod behavior verified | Controlled loader-disable smoke passed; gameplay disable check remains pending. | Partial |
| Final required command sequence executed | Build, no-build tests, format verify, publish, package/hash verification, post-publish no-build tests, and `git diff --check` were rerun against the current installed/package artifacts. Current-package runtime smoke and normal Steam-client verification remain pending. | Pass for automated gates; runtime blockers documented |
| Worktree clean | `git status -sb` remains dirty with intended migration/docs/test changes. | Pending |
| Commit created | No commit created in this pass. | Pending |
| Push to `origin/main` only after approval | No push attempted. | Pending user approval |

## Current Automated Evidence

- `git status --short --branch`: dirty `main...origin/main`, with intended migration/docs/test/art/package changes still uncommitted.
- `Get-Process SlayTheSpire2 -ErrorAction SilentlyContinue`: no process before publish.
- `dotnet build EZMicroBalance.sln`: pass, 0 warnings, 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: pass, 75 passed, 0 failed, 0 skipped after the A11-A20 v2.0 source/test/package refresh.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: pass.
- `dotnet publish EZMicroBalance.sln`: pass, Release DLL/manifest installed and the selected-resource PCK remained current.
- Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 75 passed, 0 failed, 0 skipped after rebuilding the private-beta zip and refreshing hash docs.
- Package verification: installed, staging, versioned, and extracted zip DLL/JSON/PCK hashes match; staging/versioned/extracted `README_INSTALL.txt` hashes match. Installed runtime folder intentionally contains only DLL/JSON/PCK. Package-facing `README_INSTALL.txt` documents controlled-smoke status, pending manual gates, and current Ascension development limits.
- Controlled smoke with `--force-steam off`: prior A20 fixed-courtyard package pass. Temporary default-profile settings enabled only BaseLib and EZ Micro Balance, explicitly disabled other discovered local mods, then restored `default\1\settings.save` and `settings.save.backup` to their original contents. The log showed `Loaded 2 mods (19 total)`, BaseLib DLL load/init, EZ Micro Balance DLL/PCK load/init, 9 SavedSpireFields, main menu reached in `4,076ms`, and 0 EZ Micro Balance error/exception lines. That smoke predates the current 12 SavedSpireField source/package; current-package runtime smoke and normal Steam-client verification remain pending.
- `git diff --check`: exit code 0 with the documented CRLF normalization warnings for `EzDailyContent.json` and `docs/dev-environment.md`.
- Active release art hash: `320112CC087B38C7FA1E1C92C67455A894B2435E3BB0A6B399D05576A3CFDE75`.
- Installed/staging/versioned/extracted zip DLL hash: `B8303AC917540479B131FF6501E2643114220BFA05B6E63D63F1ECE41E0F54BA`. Current installed/staging/versioned/extracted PCK hash is `1B89120EA299F4334CDC4D22D3ABBC704899894FF7AAF258AD04A6743BF98717`; manifest hash is `D09ACE04E532B7205D4938A03A3DFCF5BA60D0F5B9DBAC9310EBA5B0A9970758`.
- Private-beta package: `publish\EZMicroBalance-v0.1.0-private-beta.0.zip`, SHA256 `6A5273519B2FD8F4D0256EA755D1E07525E7D185BEF9D0A607EEF261F4F81427`.

## Remaining Gates

1. Launch through the normal Steam client and verify BaseLib plus EZ Micro Balance in Mod Settings.
2. Review `godot.log` after normal Steam-client launch.
3. Execute `manual-verification-matrix.md`, especially Prismatic Gem reroll, elite/boss/event exclusions, save/load, and disable gameplay checks.
4. Execute Ascension 11-20 manual checks under the documented internal gates if any gated slice will be promoted beyond source-guard/internal testing.
5. Execute multiplayer ownership/desync checks or explicitly release-note the feature as single-player verified only.
6. Decide whether `AUTHOR_NAME_REPLACE_ME` is acceptable for private beta.
7. Commit a clean release changeset.
8. Push only after explicit user approval.

