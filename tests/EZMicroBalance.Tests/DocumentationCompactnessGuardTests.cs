using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class DocumentationCompactnessGuardTests
{
    [Fact]
    public void ActiveGoalGuardStaysCompactAndReadable()
    {
        var goal = ReadRepoText("docs", "goal.md");
        var archiveReadme = ReadRepoText("docs", "archive", "feature-inputs", "README.md");
        var lineCount = goal.Split('\n').Length;

        AssertRepoPathDoesNotExist("docs", "implement.md");
        Assert.True(lineCount <= 40, $"docs/goal.md should stay a compact guardrail; current line count is {lineCount}.");
        AssertSourceContains(
            goal,
            "# Spire Plus Goal Guard",
            "Current target:",
            "Closure rules:",
            "Live proof required",
            "Source review may close only source-level issues",
            "Runtime rows need game logs, screenshots, manual notes, or two-client evidence",
            "Crystal Sphere and transform-preview live proof inside Spire Plus",
            "Seedbed / Planting clarity",
            "No source-only pass may mark this goal complete");
        foreach (var archivedGoalInput in new[]
        {
            "goal-md-mojibake-intake-20260523.md",
            "goal-coop-preview-plan-20260525.md",
            "goal-preview-plan-intake-20260526.md",
            "goal-architecture-refactor-mojibake-intake-20260526.md"
        })
        {
            Assert.Contains(archivedGoalInput, goal, StringComparison.Ordinal);
            Assert.Contains(archivedGoalInput, archiveReadme, StringComparison.Ordinal);
            AssertRepoFileExists("docs", "archive", "feature-inputs", archivedGoalInput);
        }

        Assert.Contains("implement-a19-a20-boss-ability-mojibake-intake-20260526.md", archiveReadme, StringComparison.Ordinal);
        AssertRepoFileExists(
            "docs",
            "archive",
            "feature-inputs",
            "implement-a19-a20-boss-ability-mojibake-intake-20260526.md");
        Assert.Contains("ritsulib-migration-mojibake-intake-20260526.md", archiveReadme, StringComparison.Ordinal);
        AssertRepoFileExists(
            "docs",
            "archive",
            "feature-inputs",
            "ritsulib-migration-mojibake-intake-20260526.md");

        Assert.DoesNotContain("A19 / A20", goal, StringComparison.OrdinalIgnoreCase);

        foreach (var stalePromptMarker in new[] { "## P0", "## P1", "One-Shot Prompt", "Phase 1", "sourcecodeonlyaianalysis", "## 结论", "FeatureRegistry" })
        {
            Assert.DoesNotContain(stalePromptMarker, goal, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void DebugGoalStaysCompactAndArchivedPromptDumpStaysOutOfActivePath()
    {
        var debug = ReadRepoText("docs", "goals", "debug.md");
        var archiveReadme = ReadRepoText("docs", "archive", "feature-inputs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var lineCount = debug.Split('\n').Length;

        Assert.True(lineCount <= 45, $"docs/goals/debug.md should stay a compact governance note; current line count is {lineCount}.");
        AssertRepoFileExists(
            "docs",
            "archive",
            "feature-inputs",
            "debug-goal-mojibake-intake-20260620.md");
        AssertSourceContains(
            debug,
            "# Debug Governance",
            "Current beta.93 loader truth is RitsuLib-only",
            "Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context",
            "Debug scaffold status: accept scaffold, do not expand.",
            "Keep StS1Events staging-only");
        AssertSourceContains(
            archiveReadme,
            "debug-goal-mojibake-intake-20260620.md",
            "Current debug governance is the compact active `docs/goals/debug.md`.");
        Assert.Contains("debug-goal-mojibake-intake-20260620.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("debug-goal-mojibake-intake-20260620.md", docInventory, StringComparison.Ordinal);

        foreach (var stalePromptMarker in new[]
                 {
                     "OwnerCommitAgent",
                     "M5 Revision N: beta.88 Evidence Governance",
                     "One-Shot Prompt",
                     "涓",
                     "锛",
                     "銆",
                     "歚",
                     "鐨",
                     "乧"
                 })
        {
            Assert.DoesNotContain(stalePromptMarker, debug, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Sts1V5MonthlySpecStaysCompactAndArchivedPromptDumpStaysOutOfActivePath()
    {
        const string archivedFileName = "sts1-event-port-strict-audit-monthly-spec-v5-overnight-subagents-20260620.md";
        var activeBoundary = ReadRepoText("docs", "goals", "sts1_event_port_strict_audit_monthly_spec_v5_overnight_subagents.md");
        var archiveReadme = ReadRepoText("docs", "archive", "feature-inputs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var lineCount = activeBoundary.Split('\n').Length;

        Assert.True(lineCount <= 35, $"StS1 v5 monthly spec boundary should stay compact; current line count is {lineCount}.");
        AssertRepoFileExists("docs", "archive", "feature-inputs", archivedFileName);
        AssertSourceContains(
            activeBoundary,
            "# StS1 Event Port v5 Historical Boundary",
            archivedFileName,
            "This v5 audit/spec is historical planning context only.",
            "Do not use its O0-O12 overnight gates, old registration assumptions, or old task",
            "Current StS1 event work routes through `docs/goals/event.md`",
            "current beta.93 proves `v0.107.1` RitsuLib-only Off plus",
            "AdditiveBatch1 loader/registration with STS2-RitsuLib `0.4.31`",
            "This loader evidence is not gameplay, save-load, replacement, multiplayer, QA");
        Assert.Contains(archivedFileName, archiveReadme, StringComparison.Ordinal);
        Assert.Contains(archivedFileName, projectMap, StringComparison.Ordinal);
        Assert.Contains(archivedFileName, docInventory, StringComparison.Ordinal);

        foreach (var stalePromptMarker in new[]
                 {
                     "必须启动 subagents",
                     "Overnight Exit Gates O0-O12 必须全绿",
                     "current `v0.107.1` loader proof needs recapture",
                     "他现在声称",
                     "Big Fish 必须严格实现"
                 })
        {
            Assert.DoesNotContain(stalePromptMarker, activeBoundary, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void UrdaSupportSourceDesignStaysReadableAndCurrent()
    {
        var urdaSourceDesign = ReadRepoText("docs", "features", "ancient-expansion-urda", "source-design.md");

        AssertSourceContains(
            urdaSourceDesign,
            "# Urda Ancient Source Design v1",
            "`EZMB_URDA` registration and visibility path",
            "The active v2.2 source pool contains eleven Urda blessings",
            "Humus Pact (`urda_humus_pact`, 腐殖约定)",
            "Moss Map (`urda_moss_map`, 苔痕地图)",
            "Elite Root (`urda_elite_root`, 精英根须)",
            "All eleven remain disableable through the Urda feature gate",
            "`Withered Husk` is a 0-cost Temporary Curse with Ethereal and Exhaust",
            "A planted Blight Sprout is treated as handled for that combat",
            "A planted Rootblight freezes that combat's end check only",
            "After the Rain (`urda_after_rain`)",
            "AncientSavedStateFields.UrdaStateKey",
            "All active Urda text must include EN + ZHS entries");

        foreach (var mojibakeOrDamagedTerm in new[]
        {
            "lncient",
            "lct ",
            "lfter",
            "lll ten",
            "URDl",
            "?",
            "?",
            "?",
            "?",
            "lPI"
        })
        {
            Assert.DoesNotContain(mojibakeOrDamagedTerm, urdaSourceDesign, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void UrdaSupportDocsKeepElevenBlessingCountCurrent()
    {
        var activeUrdaDocs = ReadCurrentFacingDocs(
            "docs/issues/urda.md",
            "docs/features/ancient-expansion-urda/README.md",
            "docs/features/ancient-expansion-urda/implementation-plan.md",
            "docs/features/ancient-expansion-urda/manual-test-checklist.md",
            "docs/features/ancient-expansion-urda/source-design.md",
            "docs/features/ancient-expansion-v2.2/README.md",
            "docs/features/ancient-expansion-v2.2/implementation-plan.md",
            "docs/specs/website-claim-audit.md",
            "docs/features/ancient-expansion-v2.2/source-design.md");

        AssertSourceContains(
            activeUrdaDocs,
            "eleven-blessing source candidate",
            "eleven active blessings",
            "all eleven Urda blessings",
            "any of the eleven Urda blessing rows",
            "eleven blessing ids",
            "those eleven blessings",
            "Current Urda hooks cover all eleven v2.2 blessings",
            "Urda eleven blessings",
            "Exactly the eleven source-backed Urda blessing ids");

        foreach (var staleCount in new[]
        {
            "ten active blessings",
            "all ten Urda blessings",
            "ten Urda blessing rows",
            "any of the ten Urda blessing rows",
            "ten blessing ids",
            "those ten blessings",
            "Urda ten blessings",
            "ten-blessing Urda pool",
            "all ten v2.2 blessings"
        })
        {
            Assert.DoesNotContain(staleCount, activeUrdaDocs, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void AncientExpansionV22ReadmeUsesReadableCurrentSourcePaths()
    {
        var readme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");

        AssertSourceContains(
            readme,
            "Current source-backed state to preserve:",
            "`UrdaRunHook.cs` now stays focused on run/reward/map dispatch, and `UrdaCombatHook.cs` owns the combat-only hook wrapper.",
            "UrdaBlessingService.SeedbedCombat.cs",
            "UrdaBlessingService.CardRewards.cs",
            "LothaBlessingService.CardPlayCount.cs",
            "LothaBlessingService.CardPlayDispatch.cs",
            "LothaBlessingService.CardRules.cs",
            "LothaBlessingService.CostRules.cs",
            "LothaBlessingService.CombatState.cs",
            "LothaBlessingService.CombatStart.cs",
            "LothaBlessingService.PlayerTurnStart.cs",
            "LothaBlessingService.TurnEnd.cs",
            "LothaBlessingService.CombatEnd.cs",
            "LothaBlessingService.CombatStateReset.cs",
            "browser ChatGPT/GPTimage2",
            "source code/src/Core/");

        foreach (var typoArtifact in new[]
        {
            "uurrent",
            "Seedbeduombat",
            "uardRewards",
            "uardRules",
            "uostRules",
            "uombatState",
            "uombatLifecycle",
            "uombatStateReset",
            "uhatGPT",
            "uanonical",
            "uhronological",
            "source code/src/uore/"
        })
        {
            Assert.DoesNotContain(typoArtifact, readme, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void DevEnvironmentDoesNotCarryStalePackageRefreshState()
    {
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");

        AssertSourceContains(
            devEnvironment,
            "Last attempted default publish: `dotnet publish EZMicroBalance.sln -m:1` on 2026-06-18 after the beta.86 package/source alignment pass. Result: succeeded against the real installed mods root.",
            "Last successful isolated publish: `dotnet publish EZMicroBalance.sln -p:ModsPath=.tools\\publish-game-root\\mods\\` on 2026-05-27 after the beta.84 Urda Seedbed Harmony patch bugfix. Result: succeeded against an isolated temporary mods root; the isolated root is tooling context only and is not the current package-parity source.",
            "is not the current package-parity source",
            "`E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\EZMicroBalance`",
            "staging, versioned, installed, game-root zip, and zip-entry artifacts match");
        Assert.DoesNotContain("failed 5 installed-folder parity tests", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Sovereign Blade jade boon refresh", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("current `.2` manual-test package", devEnvironment, StringComparison.Ordinal);
    }

    [Fact]
    public void AncientV4DocsUseCurrentRuntimeTargetAndArchiveOldBaseline()
    {
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var implementationPlan = ReadRepoText("docs", "features", "ancients-rework-v4", "implementation-plan.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        AssertSourceContains(
            apiDiscovery,
            "2026-06-20 dependency supersession: the May discovery notes below recorded the then-active BaseLib project shape.",
            "Current Spire Plus now compiles against `STS2.RitsuLib` `0.4.31`",
            "Historical runtime target in `docs/dev-environment.md`: public beta `v0.106.1`, source-refreshed locally on `2026-05-22`",
            "Historical local project package at the time: `Alchyr.Sts2.BaseLib` `3.1.4`; current local project package: `STS2.RitsuLib` `0.4.31`",
            "Current authoritative source is the refreshed local public beta `v0.107.1` assembly/source recorded in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`.",
            "The original Batch 2 inspection was performed against `v0.104.0` (`2026.04.23`)",
            "historical context only",
            "revalidate against the current `v0.107.1` source snapshot");
        AssertSourceContains(
            implementationPlan,
            "Historical scaffold-era wording referenced supported game/BaseLib/template APIs.",
            "Current release work must instead use native game command APIs, RitsuLib APIs, and template-supported APIs",
            "do not reintroduce BaseLib without owner-approved dependency documentation.");
        AssertSourceContains(
            manualChecklist,
            "- Target game version: public beta `v0.107.1`, source snapshot refreshed locally on `2026-06-20` per `docs/dev-environment.md` and `PROJECT_STATE.md`",
            "- Runtime framework: `STS2-RitsuLib` `v0.4.31` with `lib\\0.107.1`",
            "- Legacy baselines: `v0.104.0` (`2026.04.23`) and the later `v0.106.1` / BaseLib validation lane are historical only and are not the target for this checklist.");

        Assert.DoesNotContain("Evidence source remains local `sts2.dll` from public beta `v0.104.0`", apiDiscovery, StringComparison.Ordinal);
        Assert.DoesNotContain("through supported game/BaseLib/template APIs", implementationPlan, StringComparison.Ordinal);
        Assert.DoesNotContain("Verified baseline target: `v0.104.0`, `2026.04.23`", manualChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void ChineseIntroKeepsPreviewToolsInsideSpirePlus()
    {
        var intro = ReadRepoText("docs", "intro.zh.md");
        var packageScript = ReadRepoText("scripts", "package-spire-plus.ps1");

        AssertSourceContains(
            intro,
            "# Spire Plus \u7b80\u4ecb",
            "`Spire Plus` \u662f\u4e00\u4e2a Slay the Spire 2 \u79c1\u6d4b\u6a21\u7ec4",
            "\u73a9\u5bb6\u53ea\u9700\u8981\u542f\u7528\u8fd9\u4e00\u4e2a\u6a21\u7ec4",
            "\u6c34\u6676\u7403\u9884\u89c1\u548c\u53d8\u5316\u9884\u89c8\u5de5\u5177\uff0c\u5df2\u7ecf\u5e76\u5165\u540c\u4e00\u4e2a `Spire Plus` \u6a21\u7ec4",
            "\u989d\u5916\u5b89\u88c5\u7684\u9884\u89c1\u5de5\u5177\u6a21\u7ec4\uff1b\u6c34\u6676\u7403\u9884\u89c1\u548c\u53d8\u5316\u9884\u89c8\u5df2\u7ecf\u5e76\u5165 `Spire Plus`",
            "\u5f53\u524d\u76ee\u6807\u662f test-ready \u624b\u52a8\u6d4b\u8bd5\u5305");
        Assert.DoesNotContain("Future Peek", intro, StringComparison.Ordinal);
        Assert.DoesNotContain("EZFuturePeek", intro, StringComparison.Ordinal);
        Assert.Contains(
            "Crystal Sphere peek and transform preview are part of this Spire Plus package.",
            packageScript,
            StringComparison.Ordinal);
        Assert.DoesNotContain("Future Peek", packageScript, StringComparison.Ordinal);
        foreach (var mojibakeFragment in new[] { "\u93c4", "\u7ec9\u4f5e\u7974", "\u941c", "\u59d8\u5b58\u6ae0", "\u68f0\u6fe7", "\u5a34\u8bca", "\u59af\uff27\u7ca8" })
        {
            Assert.DoesNotContain(mojibakeFragment, intro, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void TestReadyDevelopmentGoalStaysCompactAndCurrent()
    {
        var goal = ReadRepoText("docs", "test-ready-development-goal.md");
        var lineCount = goal.Split('\n').Length;

        Assert.True(lineCount <= 120, $"docs/test-ready-development-goal.md should stay compact; current line count is {lineCount}.");
        AssertSourceContains(
            goal,
            "Goal: keep the current `Spire Plus` workspace at a user-test-ready manual test build",
            "Current stop line: Codex should not chase release-ready evidence in this pass.",
            "`source code/src/Core/**` is the primary source evidence",
            "Preview tools are now part of the single `Spire Plus` mod.",
            "$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'",
            "No live-game, save-load, death/failure, or co-op evidence may be claimed from these commands.",
            "Trial Branch /",
            "A12 uses",
            "dedicated ability /",
            "Branded Form /");
        Assert.DoesNotContain("$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'", goal, StringComparison.Ordinal);
        Assert.DoesNotContain("One-Shot Prompt", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## Subagent Plan", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P0:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P1:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P2:", goal, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RestructureDocStaysCurrentReadableAndRitsuLibOnly()
    {
        var restructure = ReadRepoText("docs", "restructure.md");
        var lineCount = restructure.Split('\n').Length;

        Assert.True(lineCount <= 90, $"docs/restructure.md should stay a compact current boundary; current line count is {lineCount}.");
        AssertSourceContains(
            restructure,
            "# Restructure Boundary",
            "Current package/runtime target is Spire Plus `v0.1.0-private-beta.93`",
            "`STS2-RitsuLib` `0.4.31`",
            "`lib\\0.107.1`",
            "BaseLib is previous-package or other-mod local context only",
            "`scripts\\check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot`",
            "Use `docs/goals/event.md`",
            "Do not combine behavior changes, package version bumps, broad file moves, and",
            "runtime dependency changes in one slice.");

        foreach (var staleCurrentTarget in new[]
                 {
                     "Slay the Spire 2 v0.106.1 + BaseLib v3.1.4",
                     "STS2-RitsuLib >= 0.3.2",
                     "RitsuLib `v0.3.10` variant pack",
                     "PackageReference Include=\"STS2.RitsuLib\" Version=\"0.3.2\"",
                     "register BaseLib config",
                     "BaseLib config"
                 })
        {
            Assert.DoesNotContain(staleCurrentTarget, restructure, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void RepoLocalSts2SkillKeepsRitsuLibOnlyCurrentGuidance()
    {
        var skill = ReadRepoText("docs", "skills", "sts2-godot-mod-development.md");

        AssertSourceContains(
            skill,
            "Prefer local game command APIs, RitsuLib APIs, template APIs, and package references before Harmony patches.",
            "BaseLib is historical or other-mod context only for current Spire Plus work",
            "Inspect local RitsuLib/template APIs or package references when available",
            "Prefer command APIs and RitsuLib/template hooks over direct state mutation.");

        foreach (var staleInstruction in new[]
                 {
                     "Prefer local BaseLib, RitsuLib, template APIs",
                     "Inspect local BaseLib/RitsuLib/template APIs",
                     "Prefer command APIs and BaseLib/template hooks"
                 })
        {
            Assert.DoesNotContain(staleInstruction, skill, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void RitsuLibIntegrationDocsReflectCurrentActiveDependency()
    {
        var docsReadme = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docRestructureSpec = ReadRepoText("docs", "doc-restructure-spec.md");
        var integration = ReadRepoText("docs", "integrations", "ritsulib.md");

        AssertSourceContains(
            docsReadme,
            "`integrations/ritsulib.md`",
            "Current RitsuLib integration record: compile package, manifest dependency, installed runtime variant, loader evidence, and remaining proof gates.");
        AssertSourceContains(
            projectMap,
            "`docs/integrations/`",
            "Runtime integration records for active dependencies such as RitsuLib.");
        AssertSourceContains(
            docRestructureSpec,
            "**RitsuLib integration** (PR5/PR6+)",
            "Current beta.93 RitsuLib-only compile/manifest/loader target active; later patch migrations gated");
        AssertSourceContains(
            integration,
            "# RitsuLib Integration - Current Record",
            "Compile and manifest dependency are active.",
            "`EZMicroBalance.csproj` references `STS2.RitsuLib` only",
            "`EZMicroBalance.json` declares only `STS2-RitsuLib`",
            "Current BaseLib target: none for Spire Plus",
            "2026-06-21 web recheck",
            "The current public Slay the Spire 2 update target remains Major Update #2",
            "Current compile dependency:",
            "Current manifest dependency:");

        foreach (var staleInstruction in new[]
                 {
                     "RitsuLib runtime staging record",
                     "version mismatch blocker",
                     "future migration plan",
                     "RitsuLib Integration - Staging Record",
                     "Historical upgrade path, now superseded by beta.93",
                     "Current-highest runtime manifest dependency",
                     "Nexus lists RitsuLib file version",
                     "| Just started |"
                 })
        {
            Assert.DoesNotContain(staleInstruction, docsReadme, StringComparison.Ordinal);
            Assert.DoesNotContain(staleInstruction, projectMap, StringComparison.Ordinal);
            Assert.DoesNotContain(staleInstruction, docRestructureSpec, StringComparison.Ordinal);
            Assert.DoesNotContain(staleInstruction, integration, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void HistoricalSts1ReviewsStayCompactAndArchivedOutOfActivePath()
    {
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var activeFiles = new[]
        {
            "overnight-run-20260529.md",
            "refactor-qa-20260602.md",
            "refactor-qa-20260602-round2.md"
        };
        var archivedFiles = new[]
        {
            "overnight-run-20260529.md",
            "refactor-qa-20260602.md",
            "refactor-qa-20260602-round2.md"
        };

        foreach (var activeFile in activeFiles)
        {
            var activeBoundary = ReadRepoText("docs", "reviews", activeFile);
            var lineCount = activeBoundary.Split('\n').Length;
            Assert.True(lineCount <= 20, $"{activeFile} should stay a compact historical QA stub; current line count is {lineCount}.");
            AssertSourceContains(
                activeBoundary,
                "Status: historical",
                "Full archived record:",
                "Current StS1 event work routes through `docs/goals/event.md`",
                "Do not use its");
            Assert.DoesNotContain("## Claim-by-Claim Verification", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Independent Verification Results", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Pack Completion Status", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("### 2.3 Runtime Smoke Evidence", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("### 1.6 CanaryOnly Runtime Smoke", activeBoundary, StringComparison.Ordinal);
        }

        foreach (var archivedFile in archivedFiles)
        {
            AssertRepoFileExists("docs", "archive", "feature-audits", archivedFile);
            Assert.Contains(archivedFile, archiveReadme, StringComparison.Ordinal);
            Assert.Contains(archivedFile, docInventory, StringComparison.Ordinal);
        }

        Assert.Contains("docs/archive/feature-audits/overnight-run-20260529.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/reviews/overnight-run-20260529.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/archive/feature-audits/refactor-qa-20260602*.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/reviews/refactor-qa-20260602*.md", projectMap, StringComparison.Ordinal);
    }

    [Fact]
    public void M5RevisionNDocsStayCompactAndArchivedOutOfActivePath()
    {
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var activeFiles = new[]
        {
            "m5-revision-n-final-report.md",
            "m5-revision-n-owner-commit-packet.md",
            "m5-revision-n-validation-replay.md",
            "m5-revision-n-runtime-evidence-plan.md"
        };
        var archivedFiles = new[]
        {
            "m5-revision-n-final-report-20260619.md",
            "m5-revision-n-owner-commit-packet-20260619.md",
            "m5-revision-n-validation-replay-20260619.md",
            "m5-revision-n-runtime-evidence-plan-20260619.md"
        };

        foreach (var activeFile in activeFiles)
        {
            var activeBoundary = ReadRepoText("docs", "goals", activeFile);
            var lineCount = activeBoundary.Split('\n').Length;
            Assert.True(lineCount <= 25, $"{activeFile} should stay a compact historical-boundary stub; current line count is {lineCount}.");
            AssertSourceContains(
                activeBoundary,
                "Status: archived",
                "beta.93",
                "RitsuLib-only");
            Assert.DoesNotContain("## Replay Commands", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Commit Slice Sketch", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Next Runtime Rows", activeBoundary, StringComparison.Ordinal);
        }

        foreach (var archivedFile in archivedFiles)
        {
            AssertRepoFileExists("docs", "archive", "legacy-planning", archivedFile);
            Assert.Contains(archivedFile, archiveReadme, StringComparison.Ordinal);
            Assert.Contains(archivedFile, docInventory, StringComparison.Ordinal);
        }

        Assert.Contains("docs/archive/legacy-planning/m5-revision-n-*-20260619.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/goals/m5-revision-n-*.md", projectMap, StringComparison.Ordinal);
    }

    [Fact]
    public void M5RevisionLDocsStayCompactAndArchivedOutOfActivePath()
    {
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var activeFiles = new[]
        {
            "m5-revision-l-runtime-hard-blocker.md",
            "m5-revision-l-runtime-smoke-plan.md",
            "m5-revision-l-final-report.md",
            "m5-revision-l-owner-review-packet.md",
            "m5-revision-l-dirty-ledger.md",
            "m5-revision-l-commit-slices.md",
            "m5-revision-l-warning-ledger.md"
        };
        var archivedFiles = new[]
        {
            "m5-revision-l-runtime-hard-blocker-20260610.md",
            "m5-revision-l-runtime-smoke-plan-20260610.md",
            "m5-revision-l-final-report-20260610.md",
            "m5-revision-l-owner-review-packet-20260610.md",
            "m5-revision-l-dirty-ledger-20260610.md",
            "m5-revision-l-commit-slices-20260610.md",
            "m5-revision-l-warning-ledger-20260610.md"
        };

        foreach (var activeFile in activeFiles)
        {
            var activeBoundary = ReadRepoText("docs", "goals", activeFile);
            var lineCount = activeBoundary.Split('\n').Length;
            Assert.True(lineCount <= 25, $"{activeFile} should stay a compact historical-boundary stub; current line count is {lineCount}.");
            AssertSourceContains(
                activeBoundary,
                "Status:",
                "archived");
            Assert.DoesNotContain("## Existing Historical Evidence", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Dirty Slices", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Recommended Order", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Current Count", activeBoundary, StringComparison.Ordinal);
        }

        foreach (var archivedFile in archivedFiles)
        {
            AssertRepoFileExists("docs", "archive", "legacy-planning", archivedFile);
            Assert.Contains(archivedFile, archiveReadme, StringComparison.Ordinal);
            Assert.Contains(archivedFile, docInventory, StringComparison.Ordinal);
        }

        Assert.Contains("docs/archive/legacy-planning/m5-revision-l-*-20260610.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/goals/m5-revision-l-*.md", projectMap, StringComparison.Ordinal);
    }

    [Fact]
    public void M5RevisionMDocsStayCompactAndArchivedOutOfActivePath()
    {
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var activeFiles = new[]
        {
            "m5-revision-m-final-report.md",
            "m5-revision-m-owner-review-packet.md",
            "m5-revision-m-runtime-drift-report.md",
            "m5-revision-m-patch-failure-ledger.md",
            "m5-revision-m-version-decision.md",
            "m5-revision-m-commit-slices.md"
        };
        var archivedFiles = new[]
        {
            "m5-revision-m-final-report-20260611.md",
            "m5-revision-m-owner-review-packet-20260611.md",
            "m5-revision-m-runtime-drift-report-20260618.md",
            "m5-revision-m-patch-failure-ledger-20260611.md",
            "m5-revision-m-version-decision-20260611.md",
            "m5-revision-m-commit-slices-20260611.md"
        };

        foreach (var activeFile in activeFiles)
        {
            var activeBoundary = ReadRepoText("docs", "goals", activeFile);
            var lineCount = activeBoundary.Split('\n').Length;
            Assert.True(lineCount <= 25, $"{activeFile} should stay a compact historical-boundary stub; current line count is {lineCount}.");
            AssertSourceContains(
                activeBoundary,
                "Status:",
                "archived");
            Assert.DoesNotContain("## Slice 1", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Required Next Lane", activeBoundary, StringComparison.Ordinal);
            Assert.DoesNotContain("## Red beta.84 Off Smoke Failures", activeBoundary, StringComparison.Ordinal);
        }

        foreach (var archivedFile in archivedFiles)
        {
            AssertRepoFileExists("docs", "archive", "legacy-planning", archivedFile);
            Assert.Contains(archivedFile, archiveReadme, StringComparison.Ordinal);
            Assert.Contains(archivedFile, docInventory, StringComparison.Ordinal);
        }

        Assert.Contains("docs/archive/legacy-planning/m5-revision-m-*-20260611.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/goals/m5-revision-m-*.md", projectMap, StringComparison.Ordinal);
    }

}
