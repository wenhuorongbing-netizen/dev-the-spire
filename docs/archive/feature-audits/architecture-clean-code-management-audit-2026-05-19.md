# Project Architecture / Clean Code / Management Deep Audit Report

Date: 2026-05-19  
Repository: `D:\Game\FOTN\dev-the-spire`  
Product: `Spire Plus` private beta candidate, technical manifest id `EZMicroBalance`; preview tools are integrated into the same mod; legacy scaffold `EzDailyContent` is archived for traceability.
Audit mode: source, docs, tests, scripts, package configuration, and local git state. The game was not launched. No production code was changed by this audit.

2026-05-22 compatibility note: the active project has since been consolidated into one `Spire Plus / EZMicroBalance` mod. Any older `EZFuturePeek` separation recommendations below are historical context, not current implementation guidance.

## 1. Executive Summary

This repository is a serious local mod workspace with strong source-guard discipline, explicit release honesty, and a clear rule that `EZMicroBalance` remains the stable manifest id for `Spire Plus`. The project is technically testable on one machine, but it has not yet reached release-ready governance because live gameplay, save/load, clicked UI, death path, and co-op proof remain open. The strongest engineering asset is the source-backed test/documentation net around Early Access API risk. The largest architecture risk is the wide Harmony patch surface across map, room, event, reward, combat, and UI seams. The largest maintainability risk is the 197-entry dirty worktree, which makes review, rollback, and ownership difficult. The largest test risk is that many tests assert source snippets and documentation claims; these guards catch drift, but they cannot replace behavior-level simulation and live evidence. The largest delivery-process gap is the absence of an active CI workflow under `.github/workflows`. Documentation is rich and unusually honest, yet active and archived knowledge still needs stronger ownership, expiry, and decision-record structure. The three highest-priority fixes are: create an active CI quality gate, split the dirty worktree into reviewable changes by bounded context, and extract explicit simulation seams for high-risk gameplay systems.

## 2. Project Inventory

| Item | Finding |
| --- | --- |
| Project Type | Slay the Spire 2 mod workspace with one active integrated mod plus archived legacy scaffold history. |
| Tech Stack | C# / .NET 9, Godot .NET SDK 4.5.1, Slay the Spire 2 Core API, previous framework 3.1.4, Harmony, xUnit, PowerShell. |
| Runtime | Slay the Spire 2 public beta target `v0.106.0`, previous framework under game `mods\previous framework`. |
| Package Manager | NuGet through SDK-style `.csproj`. |
| Build Tool | `dotnet build`, `dotnet publish`, Godot export-pack through project publish targets. |
| Test Framework | xUnit, `Microsoft.NET.Test.Sdk`; release artifact tests gated by `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`. |
| Lint / Format | `dotnet format --verify-no-changes`; `git diff --check`. |
| CI/CD Setup | No active `.github/workflows` files. A website workflow is archived and ignored. |
| Deployment Model | Local game `mods\EZMicroBalance` plus `publish\SpirePlus-v0.1.0-private-beta.0.zip`. |
| Entry Points | `EZMicroBalanceCode\MainFile.cs`; legacy entry points are archived only. |
| Config Files | `Directory.Build.props.example`, local ignored `Directory.Build.props`, `EZMicroBalance.json`, `EZFuturePeek.json`, `export_presets.cfg`, project files. |
| Main Documentation | `PROJECT_STATE.md`, `AGENTS.md`, `docs\README.md`, `docs\issues.md`, `docs\test-ready-development-goal.md`, feature docs under `docs\features`. |
| Main Directories | `EZMicroBalanceCode`, `EZMicroBalance`, `EZFuturePeekCode`, `EZFuturePeek`, `tests`, `scripts`, `docs`, `source code`, `publish`, `.tools`. |
| Risky / Unknown Areas | 137 Harmony patch declarations, 25 `DebugOnlyGetState()` call sites, pending live/co-op proof, current source/package live loader parity, broad dirty worktree. |

Code and doc volume from local scan:

| Area | Files | Lines |
| --- | ---: | ---: |
| `EZMicroBalanceCode` | 311 C# files | 22,148 |
| `tests` | 40 files | 13,897 |
| `docs` | 146 Markdown/files | 23,843 |
| `scripts` | 14 PowerShell files | 3,224 |
| `EZFuturePeekCode` | 7 C# files | 543 |
| `EzDailyContentCode` | 9 C# files | 1,935 |

## 3. Current Architecture Assessment

Current Architecture Pattern: feature-first modular monolith with patch adapters. It is closer to a pragmatic modular mod workspace than to Clean Architecture or Hexagonal Architecture.

Evidence:

- `EZMicroBalanceCode\MainFile.cs` initializes Harmony, config, and all feature initializers in one bootstrap.
- `EZMicroBalanceCode\Ancients\Expansion\Urda`, `Morvi`, `Lotha`, and `Vakuu` are grouped by feature.
- `EZMicroBalanceCode\Ascension` is grouped by feature surfaces: `Core`, `Map`, `Combat`, `Rewards`, `Patches`, `Powers`, `Relics`, `Cards`, `Events`.
- Harmony patches are concentrated in feature-adjacent files, but they still target many Core internals.
- Domain rules and Core side effects often live in static partial services such as `UrdaBlessingService` and `AscensionCombatModifierService`.

Strengths:

- Feature folders are coherent and more maintainable than a single technical-layer layout.
- Manifest separation between `EZMicroBalance`, `EZFuturePeek`, and legacy `EzDailyContent` is correct.
- Feature gates are explicit and source-guarded.
- Release documents clearly distinguish source/test-ready status from live proof.
- Production files are generally small after recent partial-file splits.

Weaknesses:

- Patch adapters, domain policy, persistence, logging, and command side effects frequently share the same static service boundary.
- Harmony patch count is high enough that game API drift can break behavior without compiler errors.
- Tests heavily inspect source text, making refactors expensive even when behavior is unchanged.
- Active CI is absent, so local green checks are not enforced before integration.

Hidden Risks:

- Runtime ordering bugs around event/room/combat patches may only appear in save/load or co-op.
- `DebugOnlyGetState()` is pragmatic for mod integration, but it creates implicit global-state coupling.
- ConditionalWeakTable and static dictionaries need lifecycle proof for save/load, combat end, and multiplayer disconnect paths.

## 4. Module Boundary & Coupling Review

| Module | Responsibility | Dependencies | Coupling Risk | Cohesion | Recommendation |
| --- | --- | --- | --- | ---: | --- |
| `MainFile` | Bootstrap, Harmony patching, config, feature initialization. | previous framework, Harmony, feature initializers. | Medium: one ordering point for all features. | 4 | Keep thin. Add an initializer manifest test that asserts explicit order and failure behavior. |
| `Ancients\Common` | Shared saved fields, helpers, option relic service, feature gates. | previous framework, Core card/relic/save APIs. | Medium: can become shared bucket. | 3 | Keep only true cross-Ancient primitives here; move blessing-specific helpers back to feature folders. |
| `Ancients\Patches` | Rebalanced vanilla Ancient reward patches. | Many Core model/relic/card types. | High: 40+ direct patch seams. | 3 | Add a patch inventory doc generated from source and group by Core seam risk. |
| `Urda` | Root rewards, Root Sight, Seed Bank, Seedbed, Rooted Route. | RunManager, ModelDb, card piles, map UI, saved state. | High: map/run/card/UI interactions. | 4 | Extract `RootSightPolicy`, `SeedBankPolicy`, `SeedbedPolicy` with pure inputs/outputs; keep Core commands in adapters. |
| `Morvi` | Debt, borrowed cards, proofread, open book, archive page systems. | Combat hooks, card generation, saved fields. | Medium-high. | 4 | Introduce a compact state-transition test harness for debt and borrowed-card cleanup. |
| `Lotha` | Verdict, death reprieve, extra plays, evidence/debuff logic. | Combat hooks, death hooks, power system, saved state. | High: lethal path and restore sensitivity. | 4 | Create characterization tests around death-reprieve phase transitions and power-card fallback. |
| `Vakuu` | Optional child combat, custom enemy/encounter, post-victory reward flow. | EventRoom, CombatRoom, RunManager, EventModel, EncounterModel. | Very high: black-screen/save-load risk. | 3 | Treat as a separate high-risk bounded context with a runtime-state diagram and targeted simulation fakes. |
| `Ascension\Core` | Gates, saved fields, diagnostics, multiplayer selection. | SaveManager, StartRunLobby, RunManager. | High for multiplayer. | 3 | Split diagnostic-only patches from gameplay patches; gate diagnostic code independently. |
| `Ascension\Map` | A11 geometry, A17 branches, map markers/metadata. | ActMap, SavedActMap, map UI. | High: route traversal and save persistence. | 4 | Keep map mutation pure where possible; add graph invariant tests independent of source snippets. |
| `Ascension\Combat` | Firemarks, banners, boss seals, Rootbud hooks. | CombatState, CreatureCmd, PowerCmd, CardCmd. | High: many turn timing hooks. | 4 | Extract act-value policy and per-banner/firemark state machines into behavior-testable classes. |
| `Ascension\Rewards` | Fission, Forge Token, Root deck, boss seal reward definitions. | CardFactory, reward creation, ModelDb. | Medium-high. | 4 | Add deterministic RNG contract tests using local Core source patterns. |
| `EZFuturePeek` | Independent preview-only mod. | NCrystalSphereScreen, NTransformPreview, RNG snapshots. | Medium: small but patch-based. | 4 | Keep independent. Add JSON parse tests using explicit UTF-8 and a publish artifact check. |
| `tests` | Source guards, docs guards, artifact guards. | File system, ZIP/PCK, docs, source text. | Medium-high: brittle refactor coupling. | 3 | Split source-shape tests from behavior simulations and label which tests are contractual. |
| `scripts` | Packaging, evidence collection, log audit, screenshots. | Local game paths, PowerShell, Windows UI. | Medium. | 3 | Add dry-run and CI-safe modes consistently; centralize path normalization. |

## 5. Domain / DDD Review

Candidate Bounded Contexts:

| Context | Core Concepts | Current Evidence |
| --- | --- | --- |
| Mod Identity & Packaging | manifest id, display name, install folder, PCK/DLL, package zip | `EZMicroBalance.json`, `EZFuturePeek.json`, `scripts\package-spire-plus.ps1` |
| Ancient Reward Rebalance | vanilla Ancient relic/card reward changes | `EZMicroBalanceCode\Ancients\Patches` |
| Ancient Expansion | Urda, Morvi, Lotha, Vakuu options and selected marker relics | `EZMicroBalanceCode\Ancients\Expansion` |
| Root Systems | Rootbud, Rootblight, Blight Sprout, Seedbed, Root Sight | `Ascension\Cards`, `Ascension\Rewards`, `Urda` |
| Ascension Progression | A11 geometry, A12 firemarks, A16 banners, A19/A20 boss seals | `EZMicroBalanceCode\Ascension` |
| Preview-Only Tools | Crystal Sphere peek, transform preview | `EZFuturePeekCode` |
| Release Evidence | package hashes, manual rows, log/screenshot evidence | `docs\release-checklist.md`, scripts, tests |

Ubiquitous Language Issues:

- `Royal Seal`, `King Brand`, `Boss Seal`, and `Royal Brand` are related terms that need a small glossary.
- `Firemark Host`, `Firemarked Elite`, and Chinese `火印精英` need consistent visible text and internal naming notes.
- `source-safe`, `source-guarded`, `live-pending` are useful engineering terms, but they should stay in docs, not player text.
- `Future Peek` is a separate mod; it should remain outside Spire Plus code and docs unless an integration matrix explicitly references it.

Suggested Domain Model:

| Model | Purpose | Port / Adapter Boundary |
| --- | --- | --- |
| `AncientSelectionPolicy` | Decide selectable blessings and marker relics. | Adapter reads Core event options and relic commands. |
| `BlessingProgress` value objects | Encode selected blessing progress without raw string manipulation spread across files. | Adapter serializes to existing `previous saved-state API` strings for compatibility. |
| `RootSightPredictionPolicy` | Given map point, room type odds, RNG seed, and player state, return preview/commit plan. | Adapter mutates RunManager queues and map UI. |
| `BannerCombatPolicy` | Given combat round/cards/enemies/act, return PowerCmd/CreatureCmd actions. | Adapter applies commands in Core hooks. |
| `FiremarkPolicy` | Encapsulate firemark windows and act-scaled values. | Adapter observes damage/block/heal hooks. |
| `VakuuFightFlow` | Explicit parent-event/child-combat/victory/failure states. | Adapter enters rooms and reconstructs parent events. |

Context Map:

`Bootstrap` -> `Feature Initializers` -> `Feature Domain Policies` -> `Core Adapter Patches` -> `Slay the Spire 2 Core`.

Dependency direction should point inward from Harmony/Core adapters to feature policies. Current code often lets policies reach directly into Core state and commands.

## 6. Directory Structure Review

Current Problems:

- Active work spans 197 git status rows, mixing Spire Plus, Future Peek, docs, tests, resources, and generated `.uid` files.
- `docs` is valuable but large; first-read discipline exists, yet historical knowledge still dominates searches.
- `tests\EZMicroBalance.Tests` has very large guard files, with the largest over 1,000 lines.
- `Ancients\Common` has real shared primitives plus risk of future convenience dumping.
- `export_presets.cfg` is long and easy to drift during resource additions.

Recommended Target Structure:

```text
EZMicroBalanceCode/
  Bootstrap/
  Shared/
    FeatureFlags/
    Persistence/
    CoreAdapters/
  Ancients/
    Rebalance/
    Expansion/
      Urda/
        Domain/
        Adapters/
        Models/
      Morvi/
      Lotha/
      Vakuu/
  Ascension/
    Domain/
      Firemarks/
      Banners/
      Rootblight/
      BossSeals/
    Adapters/
      CombatHooks/
      MapHooks/
      LobbyHooks/
      UiHooks/
    Models/
    Resources/
EZFuturePeekCode/
  Domain/
  Adapters/
  Config/
tests/
  EZMicroBalance.Tests/
    SourceGuards/
    BehaviorSimulations/
    ReleaseArtifacts/
    DocumentationGuards/
```

Migration Plan:

1. Keep namespace and manifest id stable.
2. Move only one bounded context per PR.
3. Before each move, convert file-name source guards to source-tree or behavior guards.
4. Preserve saved-field serialized formats.
5. Run build, default tests, format, diff check, publish if resources move.

Files to Archive or Keep Ignored:

- Keep `source code`, `.tools`, `publish`, and `Directory.Build.props` ignored and documented.
- Keep legacy `EzDailyContent` until project rules change.
- Keep website workflow archived unless the website becomes an actual product surface.

Naming Convention:

- Player-facing terms: short, consistent, localized.
- Internal policy classes: `{Feature}{Rule}Policy`.
- Core adapters: `{Feature}{CoreSurface}Patch` or `{Feature}{Hook}Adapter`.
- Tests: `{Feature}{Behavior}Tests` for behavior; `{Feature}{SourceGuardTests}` for source-shape contracts.

## 7. Clean Code Findings

### Issue #1: Dirty Worktree Is Too Large For Safe Review

- Severity: 4
- Confidence: High
- File / Module: repository state
- Code Evidence: `git status --short` reports 197 rows including source, tests, docs, resources, Future Peek, deleted partial files, and untracked assets.
- Observation: Multiple bounded contexts are changed simultaneously.
- Inference: Reviewers cannot reliably reason about behavior, rollback, or ownership.
- User / Business Impact: Private testers may receive a package whose source change set is hard to audit.
- Maintainability Impact: Regression blame becomes expensive.
- Readability Impact: High-volume diffs bury important changes.
- Extensibility Impact: Future feature work starts on unstable ground.
- Violated Principle: Small batches, WIP limit, Clean as You Code.
- Root Cause: Long-running local development loop without CI-backed integration checkpoints.
- Recommendation: Split into ordered commits/PRs: Future Peek, player text, Ancient option relics, Urda Root Sight/Seedbed, Ascension banners/firemarks, Vakuu, tests/docs, package evidence.
- Before / After Example: one 197-row diff -> eight bounded diffs with one validation note each.
- Acceptance Criteria: each batch builds/tests independently; no unrelated resource/doc churn in code PRs.
- Estimated Effort: M
- Priority: P0

### Issue #2: No Active CI Quality Gate

- Severity: 4
- Confidence: High
- File / Module: `.github\workflows`
- Code Evidence: `.github\workflows` has no active files; the only workflow found is archived and ignored.
- Observation: Build/test/format/release guards are manual.
- Inference: Local green status can drift from repository reality.
- User / Business Impact: A tester package can be created from a state never verified in a clean runner.
- Maintainability Impact: Contributors lack a shared definition of safe integration.
- Readability Impact: Reviewers must ask which commands were run.
- Extensibility Impact: New mods/features increase command matrix complexity.
- Violated Principle: Continuous Integration, Continuous Delivery, DORA change safety.
- Root Cause: Local-machine-first mod workflow.
- Recommendation: Add a CI workflow for `dotnet build`, default tests, format check, `git diff --check`, JSON validation, and optional artifact job when secrets/paths are available.
- Before / After Example: README command list only -> pull-request status checks.
- Acceptance Criteria: PR cannot merge when build/tests/format fail.
- Estimated Effort: S
- Priority: P0

### Issue #3: Harmony Patch Surface Is Broad And Runtime-Brittle

- Severity: 3
- Confidence: High
- File / Module: `EZMicroBalanceCode`, `EZFuturePeekCode`
- Code Evidence: `rg "\[HarmonyPatch"` returns 137 patch declarations. `VakuuFightPatch.cs` patches `Glory`, `Vakuu`, `EventModel`, `CombatRoom`, `EventRoom`, and `AncientEventModel`.
- Observation: Many critical flows rely on private or source-shaped Core behavior.
- Inference: Early Access API drift can break features without compile errors when method names/signatures remain but behavior shifts.
- User / Business Impact: Black screens, wrong rewards, or softlocks appear during manual testing.
- Maintainability Impact: Patch intent is scattered across many files.
- Readability Impact: It is difficult to know which Core seam owns a feature.
- Extensibility Impact: New patches may conflict with existing patches or other mods.
- Violated Principle: Stable boundaries, ports/adapters, minimize framework intrusion.
- Root Cause: Modding requires Harmony where previous framework lacks extension points.
- Recommendation: Create `docs/patch-inventory.md` generated from source: target type, method, feature owner, risk, source evidence file, test guard. Add source-signature tests for the highest-risk patches.
- Before / After Example: patch list embedded in tests -> explicit patch registry with owners and risk labels.
- Acceptance Criteria: every patch has owner, reason, source evidence, and test category.
- Estimated Effort: M
- Priority: P1

### Issue #4: Static Partial Services Mix Policy, IO, Commands, Logging, And Persistence

- Severity: 3
- Confidence: High
- File / Module: `UrdaBlessingService.*`, `AscensionCombatModifierService.*`
- Code Evidence: `UrdaBlessingService.RootSightRouting.cs` reads `RunManager`, validates previews, commits queues, consumes state, uses `ModelDb`, and swallows exceptions in one method. `AscensionCombatModifierService.CombatLifecycle.cs` orchestrates multiple systems directly from hook callbacks.
- Observation: Core side effects and business decisions are tightly co-located.
- Inference: Behavior tests require heavy source guards because there is no clean pure-policy seam.
- User / Business Impact: Complex features take longer to stabilize.
- Maintainability Impact: Small rule changes can touch run state, localization, commands, and tests.
- Readability Impact: Main intent is mixed with Core API mechanics.
- Extensibility Impact: New similar blessings copy adapter logic.
- Violated Principle: Single Responsibility, Dependency Inversion.
- Root Cause: Static service pattern grew from rapid patch integration.
- Recommendation: Extract pure policy classes that return command plans, then adapters apply Core commands.
- Before / After Example: `TryGetRootSightModelForCurrentPoint` both decides and commits -> `RootSightPolicy.ResolveEntry()` returns `PreviewCommitPlan`.
- Acceptance Criteria: policy tests run without `RunManager` or `ModelDb` where possible.
- Estimated Effort: L
- Priority: P1

### Issue #5: Tests Over-Index On Source Text And Documentation Assertions

- Severity: 3
- Confidence: High
- File / Module: `tests\EZMicroBalance.Tests`
- Code Evidence: 2,080 uses of `Assert.Contains`, `AssertSourceContains`, `ReadSourceTree`, or `ReadRepoText`; largest guard files exceed 900-1,000 lines.
- Observation: The suite is strong at detecting drift, but many checks lock implementation shape.
- Inference: Safe refactors can fail tests while behavioral regressions still need live/manual evidence.
- User / Business Impact: Development velocity suffers, and green tests may overstate behavior readiness.
- Maintainability Impact: Tests become harder to update than production code.
- Readability Impact: Long guard files hide the few checks that matter most.
- Extensibility Impact: New features copy source-guard style instead of building behavior seams.
- Violated Principle: Test pyramid balance, characterization before refactoring.
- Root Cause: Game runtime is hard to instantiate headlessly, so source guards filled the gap.
- Recommendation: Keep source guards for patch-surface contracts; add behavior simulation tests for policies and package JSON/PCK validation for resources.
- Before / After Example: assert method contains snippet -> test policy result for A16 Shieldwall turn-end block plan.
- Acceptance Criteria: each high-risk system has at least one behavior-simulation test independent of source text.
- Estimated Effort: M
- Priority: P1

### Issue #6: Release Evidence Is Documented But Still Manual And Pending

- Severity: 3
- Confidence: High
- File / Module: `docs\release-checklist.md`, `docs\issues.md`, `scripts\verify-spire-plus-release-evidence.ps1`
- Code Evidence: release checklist still has open rows for gameplay runtime results, save/load, disable-mod run behavior, multiplayer disposition, Vakuu victory/failure.
- Observation: The project accurately records pending gates.
- Inference: The next release risk is evidence execution, not source ignorance.
- User / Business Impact: Users can test a candidate, but release claims must stay conservative.
- Maintainability Impact: Manual proof may lag source changes.
- Readability Impact: Status is scattered across several docs.
- Extensibility Impact: New systems add more manual rows.
- Violated Principle: Definition of Done, release readiness discipline.
- Root Cause: Game UI and co-op require manual/live validation.
- Recommendation: Convert release evidence manifest into a single active dashboard and gate release-ready claims through the verifier.
- Before / After Example: checklist rows across docs -> one `release-evidence.json` plus generated Markdown summary.
- Acceptance Criteria: every release row has pass/deferred/fail, evidence dir, owner, and date.
- Estimated Effort: M
- Priority: P1

### Issue #7: Debug State Access Needs Clearer Ownership

- Severity: 2
- Confidence: High
- File / Module: `DebugOnlyGetState()` call sites
- Code Evidence: 25 source call sites across diagnostics, Ascension patches, Urda Root Sight, Morvi/Lotha hooks.
- Observation: Debug state access is used in both diagnostics and active gameplay integration.
- Inference: Future maintainers may not know which call sites are safe during host/client, restore, or UI lifecycle.
- User / Business Impact: Desync or null-state paths can appear in multiplayer.
- Maintainability Impact: Global access obscures ownership.
- Readability Impact: The data source is implicit.
- Extensibility Impact: New features may repeat the pattern without guardrails.
- Violated Principle: Explicit dependencies.
- Root Cause: Core APIs expose needed state through debug-only access points.
- Recommendation: Wrap calls in named adapters: `RunStateAccess.ForUiHover`, `ForRoomEntry`, `ForDiagnostics`, each with null/co-op policy.
- Before / After Example: direct `RunManager.Instance.DebugOnlyGetState()` -> `RunStateAccess.TryGetForMapHover(out state)`.
- Acceptance Criteria: active feature code has no direct `DebugOnlyGetState()` outside adapters and diagnostics.
- Estimated Effort: M
- Priority: P2

### Issue #8: Error Handling Sometimes Hides Useful Failure Context

- Severity: 2
- Confidence: Medium
- File / Module: `UrdaBlessingService.RootSightRouting.cs`, `RootSightHover.cs`, `A20Courtyard.cs`
- Code Evidence: several `catch` blocks return false or reset marker without logging.
- Observation: Some failures are intentionally nonfatal.
- Inference: Silent fallback can make player-visible "nothing happened" bugs hard to diagnose.
- User / Business Impact: Testers cannot provide useful logs for failed previews or UI hovers.
- Maintainability Impact: Root-cause analysis depends on reproducing locally.
- Readability Impact: Expected versus unexpected failure is unclear.
- Extensibility Impact: More preview systems may copy silent catches.
- Violated Principle: Observable failure boundaries.
- Root Cause: Avoiding noisy logs during UI hover/entry.
- Recommendation: Add rate-limited debug logs with feature, coord, room type, model id, and exception type.
- Before / After Example: `catch { return false; }` -> `catch (Exception ex) { LogDebugOnce(...); return false; }`.
- Acceptance Criteria: diagnostics can identify stale preview, missing model, invalid id, and unsupported co-op separately.
- Estimated Effort: S
- Priority: P2

### Issue #9: Documentation Is Rich But Needs Stronger Governance

- Severity: 2
- Confidence: High
- File / Module: `docs`
- Code Evidence: 146 docs files and 23,843 lines; `docs\README.md` has active path guidance and archive policy.
- Observation: The project already reduced first-read clutter but still has many status surfaces.
- Inference: Future contributors can miss the current source of truth.
- User / Business Impact: Misread status can lead to false release claims.
- Maintainability Impact: Docs need constant guard tests to prevent drift.
- Readability Impact: Search results include historical material.
- Extensibility Impact: Each new feature can add another doc cluster.
- Violated Principle: Knowledge management, single source of truth.
- Root Cause: Long-running multi-agent implementation history.
- Recommendation: Add doc ownership/expiry fields to active feature docs and generate a one-page status dashboard.
- Before / After Example: multiple current docs describing pending gates -> one generated summary linked from all entry points.
- Acceptance Criteria: every active doc has owner, last verified date, next action, archive rule.
- Estimated Effort: S
- Priority: P2

### Issue #10: Future Peek Has Correct Independence But Needs Its Own Release Discipline

- Severity: 2
- Confidence: High
- File / Module: `EZFuturePeekCode`, `EZFuturePeek`, `tests\EZFuturePeek.Tests`
- Code Evidence: independent `EZFuturePeek.sln`, `EZFuturePeek.json`, separate code/resource folders, preview-only manifest with `affects_gameplay=false`.
- Observation: The separation is architecturally correct.
- Inference: It can still drift because it lacks the same release checklist maturity as Spire Plus.
- User / Business Impact: Preview mod may be tested beside Spire Plus without clear compatibility evidence.
- Maintainability Impact: Smaller test suite may miss resource/package issues.
- Readability Impact: Current docs mention implementation plan but need release rows.
- Extensibility Impact: More preview features can accidentally cross into gameplay.
- Violated Principle: Independent deployability with independent checks.
- Root Cause: New mod created inside an existing release-heavy workspace.
- Recommendation: Add `docs\features\future-peek\release-checklist.md` and CI job for `EZFuturePeek.sln`.
- Before / After Example: only guard tests -> build/test/publish/package/evidence checklist.
- Acceptance Criteria: Future Peek package can be validated without touching `EZMicroBalance`.
- Estimated Effort: S
- Priority: P2

## 8. Testing & Testability Review

Current Test Strategy:

- Normal `dotnet test EZMicroBalance.sln --no-build` covers source shape, docs, localization, package rules, and many safety guards.
- Release artifact tests run only with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- `EZFuturePeek` has a small source/package guard suite.
- Live gameplay is intentionally manual and documented through release evidence scripts.

Test Pyramid Assessment:

| Layer | Current State | Risk |
| --- | --- | --- |
| Unit / policy tests | Limited; many rules are embedded in static services. | Medium-high |
| Integration-style static guards | Very strong. | Medium, because they lock implementation shape. |
| Package/resource tests | Strong for Spire Plus; smaller for Future Peek. | Medium |
| E2E/live tests | Manual and pending. | High |
| Co-op/save-load tests | Documented; not completed. | High |

Critical Untested Areas:

- Vakuu child-combat victory, failure/death, active save/load, prefinished restore.
- A11 natural route click traversal and map UI proof.
- A16 banner timing in live combat, especially Shieldwall turn-end block.
- Root Sight target selection and reveal behavior in live UI.
- Seed Bank click extraction and boss transition stability.
- Co-op ownership/desync across A11-A20 and Ancient reward state.

Recommended Test Seams:

- Pure `RootSightPolicy`.
- Pure `BannerPolicy`.
- Pure `FiremarkWindowPolicy`.
- Pure `VakuuFightFlowStateMachine`.
- `SavedStateRoundTrip` fixtures for encoded `Progress` values.
- `PackageResourceManifest` tests for export preset and PCK entries.

Specific Test Cases:

| Test Name | Target Module | Given / When / Then | Why It Matters | Priority |
| --- | --- | --- | --- | --- |
| `RootSight_UnknownMonster_CommitsOnePreviewOnly` | Urda Root Sight | Given eligible unknown node; when preview is committed; then room type/model is fixed and eye count decreases once. | Prevents fake preview and repeat commit. | P0 |
| `SeedBank_ClickExtract_RelicBecomesInactive` | Urda Seed Bank | Given stored seed cards; when relic is clicked; then cards are offered/restored once and relic status changes. | Addresses visible player interaction and boss crash risk. | P0 |
| `VakuuVictory_ReturnsToParentEventState` | Vakuu | Given trial combat won; when rewards resolve; then parent event has non-Vakuu options or fallback. | Directly targets reported black screen. | P0 |
| `VakuuActiveCombat_SaveLoad_DoesNotSerializeForbiddenParent` | Vakuu | Given active trial combat; when room serializes; then no invalid active parent exception path. | Protects restore path. | P0 |
| `Shieldwall_AppliesBlockAfterEnemyTurn` | A16 Banner | Given multi-enemy Shieldwall; when enemy side turn ends; then other enemies carry block into player turn. | Matches player feedback. | P1 |
| `ForgeArmor_BreakWindow_SuppressesNextMoltenArmor` | A12 Firemark | Given generated armor broken; when next turn starts; then molten armor skip is clear and tooltip explains state. | Prevents confusing "broken but reapplied" behavior. | P1 |
| `FissionReward_UsesRewardRngOnce` | A13 Fission | Given deterministic RNG and eligible options; when reward mutates; then exactly one option can be enchanted. | Guards deterministic reward behavior. | P1 |
| `DeathReprieve_PhaseRoundTrip` | Lotha | Given pending/active/recovered phases; when saved/loaded; then phase resumes according to policy. | Highest-risk Lotha save path. | P1 |
| `FuturePeek_TransformPrediction_DoesNotAdvanceSourceRng` | Future Peek | Given snapshot RNG; when preview is shown; then source seed/counter remain unchanged. | Supports `affects_gameplay=false`. | P1 |
| `Localization_ZhsJson_ParsesUtf8NoBom` | All resources | Given zhs JSON; when read using explicit UTF-8; then parse succeeds and mojibake fragments are absent. | Prevents tooling false positives and player text corruption. | P2 |

## 9. CI/CD & DevOps Review

Current CI/CD Maturity: 25/100.

Missing Automation:

- Active GitHub Actions workflow.
- Automated JSON validation for all localization files.
- Automated source/docs inventory summary.
- Automated patch inventory check.
- Automated package creation in a clean environment.
- Secret scanning and dependency vulnerability checks.
- Artifact retention for package/hash/log evidence.

Recommended Pipeline:

1. `checkout`
2. setup .NET 9
3. restore
4. `dotnet build EZMicroBalance.sln --no-restore`
5. `dotnet test EZMicroBalance.sln --no-build`
6. `dotnet build EZFuturePeek.sln --no-restore`
7. `dotnet test EZFuturePeek.sln --no-build`
8. `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
9. `dotnet format EZFuturePeek.sln --verify-no-changes --no-restore`
10. JSON parse all `localization/**/*.json`
11. `git diff --check`
12. Generate patch inventory and compare with committed inventory.

Quality Gates:

- Build green.
- Tests green.
- Format green.
- No invalid JSON.
- No changed manifest id.
- No original game assets under active resource folders.
- No release-ready claim unless evidence manifest passes.

DORA Metrics To Track:

| Metric | Suggested Definition |
| --- | --- |
| Deployment Frequency | Number of tester packages produced per week. |
| Lead Time for Changes | Time from issue entry in `docs\issues.md` to packaged test build. |
| Change Failure Rate | Percentage of tester builds with P0/P1 manual regressions. |
| Mean Time To Restore | Time from user-reported blocker to installed fixed package. |
| Reliability | Number of clean live sessions per required evidence row. |

## 10. Project Management & Collaboration Review

Current Project Management Maturity Score: 58/100.

Top Process Risks:

- Very large WIP visible in git status.
- No enforced CI gate.
- Multiple docs carry overlapping status.
- Manual live proof is acknowledged but not yet executed for key flows.
- No clear PR template or review checklist in repository root.
- No explicit module owners.

Recommended Workflow:

- Use Kanban with WIP limit 2 active implementation issues.
- Keep issue size to 1-3 days.
- Use labels: `P0-blocker`, `P1-core`, `P2-quality`, `live-pending`, `source-guard`, `package`, `docs`, `future-peek`.
- Create one review batch per bounded context.
- For each batch: source change, focused tests, doc delta, validation note, package impact.

Recommended Definition of Done:

- Build passes for affected solution.
- Tests pass for affected solution.
- Format and diff check pass.
- Localization JSON parses in English and zhs.
- Source/docs updated for behavior change.
- Feature flag or disable path documented when needed.
- Save/load/co-op risk stated.
- Player-facing text reviewed in both languages when visible.
- Release evidence row updated when live proof is needed.

Recommended PR Checklist:

- Scope is one bounded context.
- Manifest id unchanged.
- No copied game assets or large decompiled code.
- New Harmony patch has source evidence and owner.
- New saved field has migration/save-load note.
- Player text avoids implementation jargon.
- Tests include at least one behavior or package check where possible.
- Manual rows updated for live-pending behavior.

Recommended Issue Template:

```markdown
## Problem
## Player Impact
## Source Evidence
## Proposed Fix
## Risk: save/load, co-op, UI, localization, package
## Acceptance Criteria
## Validation Commands
## Manual Evidence Needed
## Owner / Priority
```

Recommended ADR Template:

```markdown
# ADR-YYYYMMDD-title
Status:
Context:
Decision:
Alternatives Considered:
Consequences:
Validation:
Rollback:
```

Recommended 2-Week Improvement Plan:

- Week 1: CI, worktree split, patch inventory, JSON parser check, Future Peek release checklist.
- Week 2: behavior seams for Root Sight, Banner, Vakuu; convert top source guards into policy tests; update evidence dashboard.

## 11. Documentation Review

Documentation Score: 78/100.

What Works:

- `docs\README.md` gives a clear reading path.
- `AGENTS.md` preserves hard rules.
- `PROJECT_STATE.md` is compact enough to start.
- Release docs are honest about pending live proof.
- Feature docs preserve source evidence and risk registers.

Missing Docs:

- Active ADR folder and decision index.
- Patch inventory generated from source.
- Module owner map.
- PR/review checklist.
- One-page release evidence dashboard.
- Future Peek release checklist.
- Architecture diagram showing adapter/domain boundaries.

Recommended README Structure:

1. What this repo is.
2. Active products and manifest ids.
3. Setup.
4. Build/test/publish.
5. Current status.
6. Documentation entry points.
7. Release policy.
8. Troubleshooting.

Recommended Architecture Overview:

- Feature module map.
- Patch adapter map.
- Dependency direction.
- Save/load and co-op risk surfaces.
- Manifest/package boundaries.

Recommended Onboarding Guide:

- 30-minute read path.
- How to run build/tests.
- How to add a small localization change.
- How to add a Harmony patch responsibly.
- How to package and verify a manual-test build.

## 12. Security & Reliability Review

| Risk | Evidence | Severity | Recommendation | Quick Fix | Long-term Fix |
| --- | --- | ---: | --- | --- | --- |
| Hardcoded local paths in scripts/docs | `scripts\package-spire-plus.ps1` defaults to Steam path when `STS2_PATH` empty. | 1 | Keep default for local user, require explicit path in CI. | Add `-RequireGameRoot` for CI mode. | Central script config module. |
| Secret handling around image generation docs | Docs mention `GPT4FREE_API_KEY`, `OPENAI_API_KEY`; no secret value found in active files. | 1 | Continue documenting env vars without values. | Add secret scan to CI. | Use secure CI secrets if image jobs are ever automated. |
| Save/load reliability | Release checklist keeps save/load rows open. | 3 | Treat as release blocker for affected features. | Keep manual-test labeling. | Add save-state simulation and live evidence protocol. |
| Multiplayer desync | Docs repeatedly mark co-op pending. | 3 | Keep co-op unsupported/unverified until evidence exists. | Surface warning in release notes. | Two-client runbook evidence and deterministic ownership tests. |
| UI softlock/black screen | Vakuu live victory/failure pending. | 4 | Keep hidden by default and run targeted manual proof. | Maintain hidden gate. | Isolate child-combat flow into explicit state machine. |
| Runtime API drift | 137 Harmony patches against Early Access Core. | 3 | Patch inventory plus source signature guards. | Generate inventory. | Reduce patch surface when previous framework APIs become available. |

## 13. Scorecard

Total Score: 60 / 100.

| Dimension | Score | Rationale | Evidence | Improvement Direction |
| --- | ---: | --- | --- | --- |
| Architecture clarity | 8 / 12 | Feature layout is clear, adapter/domain boundaries are mixed. | `EZMicroBalanceCode\README.md`, module folders. | Add target architecture and patch inventory. |
| Module decoupling & boundaries | 6 / 12 | Feature grouping is good; static services and Core patches create coupling. | `UrdaBlessingService.*`, `AscensionCombatModifierService.*`. | Extract policies and adapters. |
| Domain modeling | 6 / 10 | Strong terminology in docs, limited value objects/policies in code. | Blessing ids, progress records, raw saved strings. | Introduce domain policy/value-object layer. |
| Code readability | 7 / 10 | Production files mostly small; hook flow requires Core knowledge. | largest prod files around 185 lines. | Add flow diagrams and adapter naming. |
| Code maintainability | 7 / 12 | Tests/docs help, but WIP and patch breadth hurt. | 197 git rows, 137 patches. | Split batches, reduce patch surface. |
| Extensibility | 6 / 10 | New features fit feature folders but often require new patches. | Ascension/Ancient structure. | Stable extension policies. |
| Testability & coverage | 7 / 10 | Many guards; behavior/live coverage gaps remain. | 196+ tests, release artifact gates. | Add simulation seams and live evidence dashboard. |
| CI/CD automation | 2 / 8 | No active workflow. | empty `.github\workflows`. | Add CI immediately. |
| PM & collaboration | 5 / 8 | Docs and priorities exist; WIP/PR discipline absent. | `docs\issues.md`, dirty worktree. | Kanban WIP and PR templates. |
| Docs & knowledge | 4 / 5 | Strong docs with some sprawl. | 146 docs files. | Ownership and generated status dashboard. |
| Security/stability | 2 / 3 | Secrets not found, but runtime stability proof pending. | release checklist, risk docs. | Live/save/co-op evidence. |

## 14. Top 10 Highest Impact Issues

| Priority | Issue | Area | Evidence | Impact | Effort | Recommendation |
| --- | --- | --- | --- | --- | --- | --- |
| P0 | Add active CI | DevOps | empty `.github\workflows` | Prevents unverified integration. | S | GitHub Actions for both solutions. |
| P0 | Split dirty worktree | Process | 197 git status rows | Enables review and rollback. | M | Batch by bounded context. |
| P0 | Prove Vakuu live flow | Runtime | release checklist open rows | Prevents black screen/softlock. | M | Manual evidence plus state-machine tests. |
| P1 | Patch inventory | Architecture | 137 Harmony patches | Controls API drift. | M | Generate owner/risk/source-evidence registry. |
| P1 | Behavior seams | Testability | source-heavy tests | Enables real refactors. | L | Extract RootSight/Banner/Firemark/Vakuu policies. |
| P1 | Co-op desync matrix | Reliability | docs mark co-op pending | Avoids multiplayer regressions. | M | Execute runbook and document support stance. |
| P1 | Save/load evidence | Reliability | release checklist open rows | Avoids corrupt run states. | M | Add save-state tests and live rows. |
| P2 | Docs governance | Knowledge | 23,843 doc lines | Reduces stale status. | S | Owner/date/expiry dashboard. |
| P2 | Future Peek release discipline | Separate mod | new independent project | Keeps preview-only promise. | S | Add checklist and CI job. |
| P2 | Observability for silent fallbacks | Debuggability | silent catches in preview/entry paths | Speeds manual test triage. | S | Rate-limited diagnostics. |

## 15. Quick Wins: 24-48 Hours

| Action | File / Module | Expected Impact | Effort | Owner |
| --- | --- | --- | --- | --- |
| Add CI workflow for build/test/format/diff | `.github\workflows\ci.yml` | Immediate quality gate. | S | DevOps |
| Add JSON parse script/test for all localization | `tests` or `scripts` | Catches encoding/resource errors. | S | Test owner |
| Generate patch inventory from `rg "\[HarmonyPatch"` | `scripts\generate-patch-inventory.ps1` | Makes patch risk visible. | S | Architecture owner |
| Split Future Peek changes into separate commit | `EZFuturePeek*` | Keeps independent mod isolated. | S | Feature owner |
| Add module owner table | `docs\PROJECT_MAP.md` | Clarifies responsibility. | S | TPM |
| Add PR checklist | `.github\pull_request_template.md` | Standardizes reviews. | S | Engineering manager |
| Create one-page release evidence dashboard | `docs\release-evidence-status.md` | Reduces status ambiguity. | S | Release owner |
| Add `RunStateAccess` adapter skeleton | `EZMicroBalanceCode\Shared` | Reduces global-state spread. | M | Architect |

## 16. 1-2 Week Refactoring Plan

| Phase | Work Item | Risk | Dependency | Acceptance Criteria | Success Metric |
| --- | --- | --- | --- | --- | --- |
| Days 1-2 | CI + PR/issue templates | Low | none | CI green on main; PR template committed. | 100% PRs gated. |
| Days 2-3 | Worktree split by context | Medium | current diff review | Each batch has build/test note. | No batch over 25 changed files unless resource-only. |
| Days 3-4 | Patch inventory | Low | source scan script | Inventory covers all 137 patch declarations. | Unknown-owner patches = 0. |
| Days 4-6 | Root Sight policy seam | Medium | current Urda tests | Deterministic tests for preview/commit. | Fewer source-snippet assertions for Root Sight. |
| Days 6-8 | Banner/Firemark policy seams | Medium | Ascension combat tests | Behavior tests for Shieldwall, Pressing Line, Forge Armor, Constant Heal. | A16/A12 simulation tests pass. |
| Days 8-10 | Vakuu state-machine documentation and tests | High | source review | Flow diagram plus simulation for victory/failure/save marker. | Manual-test script has exact expected states. |
| Days 10-12 | Release evidence dashboard | Low | checklist rows | One active dashboard links all live-pending rows. | No duplicated release status conflict. |
| Days 12-14 | Manual evidence prep package | Medium | latest package | Testers receive package, checklist, known limitations. | Manual run feedback maps to issue ids. |

## 17. 30-60-90 Day Engineering Improvement Roadmap

30 Days:

- Establish CI, PR templates, issue templates, ADR folder.
- Split and commit current WIP.
- Create patch inventory and module ownership map.
- Add behavior seams for Root Sight, Banner, Firemark, and Vakuu.
- Complete tester-facing manual evidence dashboard.

60 Days:

- Reduce source-snippet tests for the highest-churn modules.
- Add save/load characterization tests for encoded state.
- Execute co-op runbook or document unsupported scope clearly.
- Add package artifact generation to CI where local game assets are not required.
- Convert repeated feature gates into a shared flag registry.

90 Days:

- Formalize target modular-monolith architecture.
- Move Core adapters and domain policies into clear subfolders.
- Add lightweight architecture tests for dependency direction.
- Track DORA metrics for tester packages.
- Use ADRs for every new patch family, saved-field format, and release-scope decision.

## 18. Recommended Target Architecture

Target Architecture Pattern: feature-first modular monolith with ports/adapters at the Slay the Spire 2 Core boundary.

Module Map:

```text
Bootstrap -> Feature Initializers
Feature Initializers -> Domain Policies
Core Adapter Patches -> Domain Policies
Domain Policies -> Value Objects
Core Adapter Patches -> Core Commands / ModelDb / UI Nodes
```

Dependency Direction:

- Domain policies do not call `RunManager`, `CombatManager`, `ModelDb`, `CardCmd`, `CreatureCmd`, or `PowerCmd`.
- Adapter patches translate Core objects into small domain inputs.
- Adapters apply command plans returned by policies.
- Persistence adapters serialize existing formats to protect saves.

Suggested Interfaces / Ports:

| Port | Purpose |
| --- | --- |
| `IRunStateReader` | Read act, players, current room, map coord through a named lifecycle context. |
| `ICombatCommandSink` | Apply block, power, card, damage, heal actions generated by policies. |
| `IRngFork` | Deterministic RNG fork for preview-only predictions. |
| `ISavedProgressCodec<T>` | Encode/decode progress state with compatibility tests. |
| `IMapPreviewStore` | Store, commit, and consume previewed map outcomes. |

Suggested Events:

- `AncientBlessingSelected`
- `RootSightPreviewCommitted`
- `BannerRoomStarted`
- `FiremarkWindowOpened`
- `VakuuTrialStateChanged`
- `ReleaseEvidenceRowUpdated`

Suggested Testing Strategy:

- Unit tests for policies.
- Characterization tests for current saved-state codecs.
- Source guards only for patch target presence and forbidden Core calls.
- Artifact tests for package/PCK/export/localization.
- Manual/live evidence for UI, save/load, co-op, and true gameplay.

## 19. Final Prioritized Backlog

| Priority | Issue | Area | Recommendation | Impact | Effort | Owner | Metric | Acceptance Criteria |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| P0 | No CI | DevOps | Add workflow for both solutions. | High | S | DevOps | CI pass rate | Build/test/format/diff required. |
| P0 | Large dirty worktree | Process | Split into bounded-context batches. | High | M | Tech Lead | files per PR | Each batch reviewable and validated. |
| P0 | Vakuu runtime proof | Gameplay | Keep gated and execute no-black-screen/manual rows. | High | M | Feature owner | passed evidence rows | Victory/failure/save-load documented. |
| P1 | Patch inventory missing | Architecture | Generate and commit patch registry. | High | M | Architect | patches with owner | 100% patch owner/risk/source evidence. |
| P1 | Root Sight behavior seam | Testability | Extract policy and add deterministic tests. | High | M | Urda owner | behavior tests | Preview/commit/consume tested. |
| P1 | Banner/firemark behavior seam | Testability | Extract policies and add simulations. | High | M | Ascension owner | behavior tests | A12/A16 windows tested. |
| P1 | Save/load pending | Reliability | Add codec roundtrip tests and live rows. | High | M | Release owner | evidence rows | State survives documented restore scenarios. |
| P1 | Co-op pending | Reliability | Execute runbook or mark unsupported for release. | High | M | Multiplayer owner | co-op rows | Host/client logs clean or scope documented. |
| P2 | Docs ownership | Knowledge | Add owner/date/expiry fields. | Medium | S | TPM | stale docs count | Active docs have owner and next action. |
| P2 | Future Peek checklist | Release | Add independent release checklist and CI job. | Medium | S | Future Peek owner | checklist completeness | Future Peek validated separately. |
| P2 | Silent fallback diagnostics | Observability | Add rate-limited logs for preview/entry failures. | Medium | S | Feature owners | actionable log rate | Failures identify feature/coord/model. |
| P3 | Naming/glossary polish | Docs/localization | Add glossary for Firemark/Seal/Brand terms. | Medium | S | Localization owner | term drift | Player and docs terms match. |

Final conclusion: this project currently最应该优先解决的是 CI + dirty-worktree batch governance，因为它直接影响 every later bug fix, review, packaging, rollback, and tester trust.
