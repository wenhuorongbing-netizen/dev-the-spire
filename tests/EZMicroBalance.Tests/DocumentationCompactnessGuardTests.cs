using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class DocumentationCompactnessGuardTests
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
            "Last attempted default publish: `dotnet publish EZMicroBalance.sln` on 2026-05-26 after the beta.47 Root Sight selection commit split package sync. Result: succeeded against the real installed mods root.",
            "Last successful isolated publish: `dotnet publish EZMicroBalance.sln -p:ModsPath=.tools\\publish-game-root\\mods\\` on 2026-05-26 after the beta.47 Root Sight selection commit split package sync. Result: succeeded against an isolated temporary mods root; the isolated root is tooling context only and is not the current package-parity source.",
            "is not the current package-parity source",
            "`D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\EZMicroBalance`",
            "staging, versioned, installed, game-root zip, and zip-entry artifacts match");
        Assert.DoesNotContain("failed 5 installed-folder parity tests", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Sovereign Blade jade boon refresh", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("current `.2` manual-test package", devEnvironment, StringComparison.Ordinal);
    }

    [Fact]
    public void AncientV4DocsUseCurrentRuntimeTargetAndArchiveOldBaseline()
    {
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        AssertSourceContains(
            apiDiscovery,
            "Runtime target in `docs/dev-environment.md`: public beta `v0.106.0`, source-refreshed locally on `2026-05-22`",
            "Current authoritative source is the refreshed local public beta `v0.106.0` assembly/source noted above.",
            "The original Batch 2 inspection was performed against `v0.104.0` (`2026.04.23`)",
            "historical context only",
            "revalidate against `v0.106.0`");
        AssertSourceContains(
            manualChecklist,
            "- Target game version: public beta `v0.106.0`, refreshed locally on `2026-05-22` per `docs/dev-environment.md`",
            "- Legacy baseline: `v0.104.0` (`2026.04.23`) is historical only and is not the target for this checklist.");

        Assert.DoesNotContain("Evidence source remains local `sts2.dll` from public beta `v0.104.0`", apiDiscovery, StringComparison.Ordinal);
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
            "\u6c34\u6676\u7403\u9884\u89c1\u548c\u53d8\u6362\u9884\u89c8\u5de5\u5177\uff0c\u5df2\u7ecf\u5e76\u5165\u540c\u4e00\u4e2a `Spire Plus` \u6a21\u7ec4",
            "\u989d\u5916\u5b89\u88c5\u7684\u9884\u89c1\u5de5\u5177\u6a21\u7ec4\uff1b\u6c34\u6676\u7403\u9884\u89c1\u548c\u53d8\u6362\u9884\u89c8\u5df2\u7ecf\u5e76\u5165 `Spire Plus`",
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
    public void PlayerFacingNameStaysSpirePlusWhileTechnicalIdRemainsStable()
    {
        using var manifest = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        Assert.Equal("EZMicroBalance", manifest.RootElement.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", manifest.RootElement.GetProperty("name").GetString());

        Assert.Equal(
            "Spire Plus",
            JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json")["EZMICROBALANCE.mod_title"]);
        Assert.Equal(
            "Spire Plus",
            JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json")["EZMICROBALANCE.mod_title"]);

        var godotProject = ReadRepoText("project.godot");
        Assert.Contains("config/name=\"Spire Plus\"", godotProject, StringComparison.Ordinal);
        Assert.Contains("project/assembly_name=\"EZMicroBalance\"", godotProject, StringComparison.Ordinal);
        Assert.DoesNotContain("config/name=\"EZMicroBalance\"", godotProject, StringComparison.Ordinal);

        var projectFile = ReadRepoText("EZMicroBalance.csproj");
        Assert.Contains("Copying Spire Plus compatibility DLL and manifest", projectFile, StringComparison.Ordinal);
        Assert.Contains("Exporting Spire Plus compatibility Godot .pck", projectFile, StringComparison.Ordinal);
        Assert.DoesNotContain("Copying EZMicroBalance Release", projectFile, StringComparison.Ordinal);
        Assert.DoesNotContain("Exporting EZMicroBalance Godot", projectFile, StringComparison.Ordinal);

        var currentMarkdownFiles = Directory
            .GetFiles(Root, "*.md", SearchOption.AllDirectories)
            .Select(ToRepoRelativePath)
            .Where(path =>
                !path.StartsWith("docs/archive/", StringComparison.Ordinal) &&
                !path.StartsWith(".tools/", StringComparison.Ordinal) &&
                !path.StartsWith("publish/", StringComparison.Ordinal) &&
                !path.StartsWith("source code/", StringComparison.Ordinal) &&
                !path.Contains("/bin/", StringComparison.Ordinal) &&
                !path.Contains("/obj/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(currentMarkdownFiles);
        var offenders = currentMarkdownFiles
            .Select(path => new { Path = path, Text = ReadRepoText(path.Split('/')) })
            .Where(file =>
                file.Text.Contains("EZ Micro Balance", StringComparison.Ordinal) ||
                file.Text.Contains("EZ Microbalance", StringComparison.Ordinal) ||
                file.Text.Contains("EZmicrobalance", StringComparison.Ordinal))
            .Select(file => file.Path)
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Current player/tester-facing markdown must use Spire Plus, not the old display name. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, offenders));

        var legacyShorthandOffenders = currentMarkdownFiles
            .Select(path => new { Path = path, Text = ReadRepoText(path.Split('/')) })
            .SelectMany(file => new[]
                {
                    "EZMB-only",
                    "BaseLib+EZMB",
                    "non-BaseLib/EZMB",
                    "BaseLib/EZMB",
                    "no-op EZMB config",
                    "BaseLib + EZMicroBalance",
                    "Spire Plus / `EZMicroBalance`",
                    "Spire Plus / EZMicroBalance"
                }
                .Where(fragment => file.Text.Contains(fragment, StringComparison.Ordinal))
                .Select(fragment => $"{file.Path}:{fragment}"))
            .ToArray();

        Assert.True(
            legacyShorthandOffenders.Length == 0,
            "Current markdown should say Spire Plus for player/tester-facing setup shorthand; keep EZMicroBalance only for exact technical ids, paths, artifacts, and legacy env-var aliases. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, legacyShorthandOffenders));

        var remoteSetup = ReadRepoText("docs", "REMOTE_DEVELOPMENT_SETUP.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");

        AssertSourceContains(
            remoteSetup,
            "Active mod: `Spire Plus`",
            "Technical project, manifest id, and install folder: `EZMicroBalance`");
        Assert.DoesNotContain("Active project: `EZMicroBalance`", remoteSetup, StringComparison.Ordinal);
        Assert.Contains(
            "Why `Spire Plus` keeps the stable `EZMicroBalance` technical id.",
            docsIndex,
            StringComparison.Ordinal);
        Assert.Contains(
            "Independent `Spire Plus` project created on the stable `EZMicroBalance` technical id",
            projectMap,
            StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerVisibleLocalizationValuesAvoidTechnicalAndLegacyModNames()
    {
        var forbiddenFragments = new[]
        {
            "EZMicroBalance",
            "EZMB",
            "EZ Micro Balance",
            "EZ Microbalance",
            "EZmicrobalance",
            "Easy Content",
            "EzDailyContent",
            "Future Peek",
            "EZFuturePeek"
        };
        var localizationRoots = new[]
            {
                RepoPath("EZMicroBalance", "localization", "eng"),
                RepoPath("EZMicroBalance", "localization", "zhs"),
                RepoPath("website", "assets", "localization", "eng"),
                RepoPath("website", "assets", "localization", "zhs")
            }
            .Where(Directory.Exists)
            .ToArray();

        Assert.NotEmpty(localizationRoots);

        var offenders = localizationRoots
            .SelectMany(root => Directory.GetFiles(root, "*.json", SearchOption.TopDirectoryOnly))
            .OrderBy(path => path, StringComparer.Ordinal)
            .SelectMany(file => JsonStringMap(file)
                .SelectMany(entry => forbiddenFragments
                    .Where(fragment => entry.Value.Contains(fragment, StringComparison.Ordinal))
                    .Select(fragment => $"{ToRepoRelativePath(file)}:{entry.Key}:{fragment}")))
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "Player-visible localization values must say Spire Plus. Technical ids may remain in keys, paths, manifest id, saved fields, and legacy env-var aliases only. Offenders:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, offenders));
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
    public void ProjectStateStaysCurrentAndHistoricalPassLogIsArchived()
    {
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var archive = ReadRepoText("docs", "archive", "project-state-history-20260516.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");

        Assert.True(
            projectState.Split('\n').Length <= 100,
            "PROJECT_STATE.md should remain a compact first-read current-state file; archive historical pass logs instead.");
        Assert.Contains("docs/archive/project-state-history-20260516.md", projectState, StringComparison.Ordinal);
        Assert.Contains("Archive note: this is the pre-cleanup `PROJECT_STATE.md` snapshot", archive, StringComparison.Ordinal);
        Assert.Contains("beta.47 Root Sight selection commit split package sync", projectState, StringComparison.Ordinal);
        Assert.Contains("2026-05-24 after the Sere Talon `NRelic` fallback package refresh", projectState, StringComparison.Ordinal);
        Assert.Contains("focused Sere Talon/release-evidence/documentation/website guards", projectState, StringComparison.Ordinal);
        Assert.Contains("beta19-loader-smoke-20260525-213336", projectState, StringComparison.Ordinal);
        Assert.Contains("beta.19 Steam-client loader smoke", projectState, StringComparison.Ordinal);
        Assert.Contains("Current manual-test package is not a release-readiness claim", projectState, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", projectState, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.Ordinal);
        Assert.DoesNotContain("2026-05-18 package was not live-loader-smoked", projectState, StringComparison.Ordinal);
        Assert.DoesNotContain("235 passed / 18 skipped", projectState, StringComparison.Ordinal);

        foreach (var stalePassMarker in new[]
        {
            "Latest Vakuu/text/reward polish package refresh",
            "Latest event-background live UI correction",
            "Latest browser GPTimage2 oil-repaint art rebuild",
            "Latest Ancient art promotion",
            "Historical source/package refresh",
            "Current-package smoke/log/resource verification"
        })
        {
            Assert.DoesNotContain(stalePassMarker, projectState, StringComparison.Ordinal);
            Assert.Contains(stalePassMarker, archive, StringComparison.Ordinal);
        }

        Assert.Contains("archive/project-state-history-20260516.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("docs/archive/project-state-history-20260516.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/archive/project-state-history-20260516.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("project-state-history-20260516.md", archiveReadme, StringComparison.Ordinal);
    }

    [Fact]
    public void ToReviewQueueStaysCompactAndHistoricalRowsAreArchived()
    {
        var toReview = ReadRepoText("docs", "toreview.md");
        var archive = ReadRepoText("docs", "archive", "feature-audits", "toreview-pre-slim-20260518.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var archiveReadme = ReadRepoText("docs", "archive", "README.md");

        Assert.True(
            toReview.Split('\n').Length <= 60,
            "docs/toreview.md should stay a compact current manual retest queue; archive historical implementation rows.");
        Assert.Contains("docs/archive/feature-audits/toreview-pre-slim-20260518.md", toReview, StringComparison.Ordinal);
        Assert.Contains("BANNER-TEMP-STRENGTH-CLEANUP-20260518", archive, StringComparison.Ordinal);
        Assert.DoesNotContain("BANNER-TEMP-STRENGTH-CLEANUP-20260518", toReview, StringComparison.Ordinal);
        Assert.Contains("URDA-ROOT-EYES", toReview, StringComparison.Ordinal);
        Assert.Contains("VAKUU-FIGHT", toReview, StringComparison.Ordinal);
        Assert.DoesNotContain("candidate gating", toReview, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("toreview-pre-slim-20260518.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", archiveReadme, StringComparison.Ordinal);
    }

    [Fact]
    public void IssuesQueueStaysCompactAndDoesNotBecomeAReleaseJournal()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var lineCount = issues.Split('\n').Length;
        var longestLine = issues
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(lineCount <= 45, $"docs/issues.md should stay a compact blocker queue; current line count is {lineCount}.");
        Assert.True(longestLine <= 260, $"docs/issues.md should avoid release-journal lines; longest line is {longestLine} characters.");
        AssertSourceContains(
            issues,
            "Current target: test-ready manual build, not release-ready.",
            "Current package hashes, 2026-05-26:",
            "| ZIP |",
            "| DLL |",
            "## Active blockers",
            "`SERE-TALON/TANX-CLAWS-ROUTING`",
            "`TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending",
            "`SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending",
            "`GOV-WIP-SPLIT` P0 source-fixed",
            "`DOC-CONFLICT-GOVERNANCE` P2 source-fixed",
            "`PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending",
            "current committed worktree is clean after the beta.47 batch commit",
            "0 dirty entries / 0 unclassified entries",
            "## Manual Proof Gates");
        Assert.DoesNotContain("Latest verified package hashes after", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("source-split/refactor passes", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SERE-TALON/CLAWS-ROUTING", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("SERE-TALON-VISUAL-IDENTITYT P0 source-fixed / package/live-pending", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("current worktree is clean after intentional batches", issues, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentGovernanceDocsDoNotCarryStaleCleanupState()
    {
        var docsByPath = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["PROJECT_STATE.md"] = ReadRepoText("PROJECT_STATE.md"),
            ["docs/issues.md"] = ReadRepoText("docs", "issues.md"),
            ["docs/worktree-cleanup-audit.md"] = ReadRepoText("docs", "worktree-cleanup-audit.md"),
            ["docs/private-beta-release-completion-audit.md"] = ReadRepoText("docs", "private-beta-release-completion-audit.md"),
            ["docs/features/ancients-rework-v4/completion-audit.md"] = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md")
        };

        AssertSourceContains(
            docsByPath["PROJECT_STATE.md"],
            "beta.47 startup remains pending",
            "beta.47 loader proof still needs a fresh run");
        AssertSourceContains(
            docsByPath["docs/worktree-cleanup-audit.md"],
            "Current beta.47 evidence should be read from the latest validated HEAD");
        AssertSourceContains(
            docsByPath["docs/worktree-cleanup-audit.md"],
            "current package evidence derived from the manifest/versioned artifacts");
        AssertSourceContains(
            docsByPath["docs/issues.md"],
            "`DOC-CONFLICT-GOVERNANCE` P2 source-fixed",
            "`PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending");
        AssertSourceContains(
            docsByPath["docs/private-beta-release-completion-audit.md"],
            "Earlier dirty implementation batches have been split, validated, committed, and pushed on `main`",
            "Final release handoff capture: current `git status --short --branch`, validated HEAD, and pushed branch after the last validation pass.");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "Beta.38 loader startup and gameplay/manual rows pending");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "Legacy root surfaces including `EzDailyContent.json` are absent from the active root",
            "OnlySpirePlusIsAnActiveRootModSurface");
        AssertSourceContains(
            docsByPath["docs/features/ancients-rework-v4/completion-audit.md"],
            "avoid copying stale beta37 commit labels into beta39 handoff notes");

        foreach (var staleFragment in new[]
                 {
                     "beta.26 startup remains pending",
                     "beta.26 loader proof still needs a fresh run",
                     "current worktree is clean after intentional batches",
                     "319 dirty entries",
                     "47 dirty entries",
                     "9 dirty entries",
                     "10 dirty entries",
                     "14 dirty entries",
                     "21 dirty entries",
                     "45 dirty entries",
                     "80 dirty entries",
                     "current dirty worktree is intentionally batch-classified",
                     "remains dirty with many source/resource/docs/tests changes",
                     "remains dirty with many pending source/docs/test/resource changes",
                     "no commit or push has been performed",
                     "Clean intentional commit state and pushed branch after validation",
                     "Beta.35 loader startup",
                     "`EzDailyContent.json` still uses id `EzDailyContent`",
                     "efb3dc4",
                     "Refresh beta37 package evidence"
                 })
        {
            foreach (var (path, text) in docsByPath)
            {
                Assert.DoesNotContain(staleFragment, text, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void SourceFixedLivePendingIssuesHaveManualRetestRows()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var toReview = ReadRepoText("docs", "toreview.md");

        var mappings = new (string IssueId, string[] RetestEvidence)[]
        {
            ("SERE-TALON/TANX-CLAWS-ROUTING", ["MANUAL-20260519-COUNTERS-PEEK-TAGS", "MANUAL-20260524-SERE-TALON-ART"]),
            ("HUSK-CARD-BEHAVIOR", ["MANUAL-20260519-MOLTING"]),
            ("SERE-TALON-VISUAL-IDENTITY", ["MANUAL-20260524-SERE-TALON-ART", "MANUAL-20260524-SERE-TALON-TANX-CLAWS-REPORT"]),
            ("ROOT-SIGHT-ENCOUNTER-POOL", ["URDA-ROOT-EYES"]),
            ("FIREMARK-HEAL/TEXT", ["MANUAL-20260519-BANNER-FIREMARK"]),
            ("UNKNOWN-EVENT-PREVIEW-READABILITY", ["URDA-ROOT-EYES"]),
            ("ROOTBLIGHT-STARTER-MISSING", ["ASCENSION-A11-A20"]),
            ("WATERFALL-BOSS-SEAL", ["MANUAL-20260522-BOSS-SEALS"]),
            ("HOURGLASS-BOSS-SEAL-DESIGN", ["MANUAL-20260522-BOSS-SEALS", "MANUAL-20260522-SEAL-INDICATORS"]),
            ("QUEEN-BOSS-SEAL-WEAKNESS", ["MANUAL-20260522-BOSS-SEALS"]),
            ("FIREMARK-OVERFLOW/FORGE-ARMOR", ["MANUAL-20260519-BANNER-FIREMARK"]),
            ("BANNER-ROOM-PREVIEW", ["MANUAL-20260519-ACT-VALUES"]),
            ("ROOT-EYES-CONFLICTS-COOP", ["MANUAL-20260522-ROOT-EYES-CONFLICTS"]),
            ("PREVIEW-TOOLS-REWARD-HOOKS", ["MANUAL-20260522-PREVIEW-TOOLS"]),
            ("SEAL-BANNER-VISIBILITY", ["MANUAL-20260522-SEAL-INDICATORS"]),
            ("V33-DESIGN-PASS", ["MANUAL-20260522-V33-DESIGN"]),
            ("STRICT-AUDIT-VAKUU-CULTURE-SAVE", ["VAKUU-FIGHT"]),
            ("STRICT-AUDIT-PATCH-SURFACE", ["VAKUU-FIGHT", "ASCENSION-A11-A20", "MANUAL-20260520-EVIDENCE-LOG"]),
            ("STRICT-AUDIT-EVIDENCE-LOG", ["MANUAL-20260520-EVIDENCE-LOG"])
        };

        var failures = new List<string>();
        foreach (var mapping in mappings)
        {
            if (!issues.Contains($"`{mapping.IssueId}`", StringComparison.Ordinal))
            {
                failures.Add($"docs/issues.md no longer lists `{mapping.IssueId}`.");
                continue;
            }

            foreach (var retestEvidence in mapping.RetestEvidence)
            {
                if (!toReview.Contains(retestEvidence, StringComparison.Ordinal))
                {
                    failures.Add($"`{mapping.IssueId}` lacks manual retest evidence `{retestEvidence}` in docs/toreview.md.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void CurrentIssueDetailDocsAvoidReadableTypoArtifacts()
    {
        var issueDetails = ReadCurrentFacingDocs(
            "docs/issues/ancient-expansion-v2.2.md",
            "docs/issues/urda.md");

        AssertSourceContains(
            issueDetails,
            "SPIREPLUS_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_LOTHA",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "ISSUE-2026-05-13-LOTHA-FULL-TEST-IMPLEMENTATION",
            "ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES",
            "SavedSpireField<Player,string>",
            "UrdaDeckStateKey");
        var cardPowerSafetyIssue = SliceBetween(
            issueDetails,
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES",
            "## ISSUE-2026-05-12-MORVI-V22-PLANNING");
        AssertSourceContains(
            cardPowerSafetyIssue,
            "Status: source-fixed / live-pending",
            "Power-card replacement rewards",
            "Runtime closure still requires live gameplay");
        Assert.DoesNotContain("Status: open", cardPowerSafetyIssue, StringComparison.Ordinal);

        foreach (var typoArtifact in new[]
        {
            "SPIREPiUS",
            "DISABiE",
            "ENABiE",
            "BiESSING",
            "iOTHA",
            "FUii",
            "IMPiEMENTATION",
            "PiANNING",
            "AiIGNMENT",
            "MIiESTONE",
            "TECHNICAi",
            "iive",
            "iocal",
            "iegacy",
            "iinkedRewardSet",
            "SHEiTER",
            "Onokipped",
            "oavedopireField",
            "UrdaDeckotateKey",
            "otatus",
            "oource gameplay"
        })
        {
            Assert.DoesNotContain(typoArtifact, issueDetails, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveQueuesKeepSereTalonAndTanxClawsChineseReadable()
    {
        var activeQueues = ReadCurrentFacingDocs(
            "docs/issues.md",
            "docs/toreview.md",
            "docs/review.md");

        AssertSourceContains(
            activeQueues,
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "Maul+",
            "\u6495\u54ac+");

        foreach (var staleTanxClawsTuning in new[]
        {
            "Numeric Maul tuning is pending",
            "Maul tuning is pending",
            "design-pending",
            "[blue]1[/blue] more damage"
        })
        {
            Assert.DoesNotContain(staleTanxClawsTuning, activeQueues, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void CurrentAncientManualDocsKeepTanxClawsUpgradedMaulDesignCurrent()
    {
        var manualDocs = ReadCurrentFacingDocs(
            "docs/features/ancients-rework-v4/manual-test-checklist.md",
            "docs/features/ancients-rework-v4/manual-verification-matrix.md",
            "docs/features/ancients-rework-v4/source-design.md",
            "docs/features/ancients-rework-v4/implementation-plan.md");

        AssertSourceContains(
            manualDocs,
            "Vakuu's Sere Talon",
            "Tanx Claws",
            "upgraded Maul",
            "Maul+",
            "\u6495\u54ac+");

        foreach (var staleTanxClawsTuning in new[]
        {
            "Numeric Maul tuning is pending",
            "Maul tuning is pending",
            "design-pending",
            "[blue]1[/blue] more damage"
        })
        {
            Assert.DoesNotContain(staleTanxClawsTuning, manualDocs, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PrivateBetaHandoffAvoidsPinnedDirtyStatusSnapshots()
    {
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var longestLine = handoff
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 800, $"docs/private-beta-verification-handoff.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            handoff,
            "Current source/package highlights:",
            "Current automated snapshot:",
            "SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1",
            "Historical detailed command logs are preserved",
            "Automated evidence does not close clicked UI, live gameplay, save-load, death/failure, route traversal, preview-tools, or co-op rows.");
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Pre-commit local cleanup status summary", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Latest package note, 2026-05-18: the package hashes below include", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("V22 text/art-fit recheck", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Source-audited text correction recheck", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Source-guard follow-up, 2026-05-14", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("RC1 normal Steam-client isolated startup log started", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("Previous controlled `--force-steam off` smoke evidence", handoff, StringComparison.Ordinal);
    }

    [Fact]
    public void PrivateBetaReleaseAuditAvoidsPassHistoryDumpRows()
    {
        var audit = ReadRepoText("docs", "private-beta-release-completion-audit.md");
        var longestLine = audit
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 1100, $"docs/private-beta-release-completion-audit.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            audit,
            "Latest source/package refresh",
            "detailed pass history lives in `docs/review.md` and `docs/archive/**`",
            "gameplay verification pending",
            "## Missing Or Weakly Verified Items");
        Assert.DoesNotContain("includes this mod/resource state plus the browser GPTimage2 small-art rebuild", audit, StringComparison.Ordinal);
        Assert.DoesNotContain("and release-evidence verifier hardening.", audit, StringComparison.Ordinal);
    }

    [Fact]
    public void DevEnvironmentAvoidsReleaseJournalLines()
    {
        var devEnvironment = ReadRepoText("docs", "dev-environment.md");
        var archivedRuntimeHistory = ReadRepoText(
            "docs",
            "archive",
            "implementation-records",
            "dev-environment-runtime-smoke-history-20260526.md");
        var longestLine = devEnvironment
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Max(line => line.Length);

        Assert.True(longestLine <= 750, $"docs/dev-environment.md has a release-journal line of {longestLine} characters.");
        AssertSourceContains(
            devEnvironment,
            "Historical 22-field loader evidence:",
            "Current source defines 30 SavedSpireFields",
            "Historical beta.19 loader evidence:",
            "dev-environment-runtime-smoke-history-20260526.md",
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Last private beta package:",
            "Zip SHA256:",
            "DLL SHA256:",
            "## Pending manual checks",
            "Manual game verification");
        AssertSourceContains(
            archivedRuntimeHistory,
            "Historical archive.",
            "direct `SlayTheSpire2.exe` launch",
            "ConnectToGlobalUser failed",
            "RootBudCombatHook",
            "rc1-normal-steam-clean-godot-20260508-090122",
            "live-spire-plus-disabled-session-20260513-142835",
            "current-package-smoke-20260514-015901");
        Assert.DoesNotContain("## Runtime smoke attempts", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("direct `SlayTheSpire2.exe` smoke launch", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("ConnectToGlobalUser failed", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("RC1 normal Steam-client launch/log probe", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("VampireSurvivors", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("## TODO", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("strict hook/text audit", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Banner temporary Strength cleanup", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Ascension side-turn state fix", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Root Eyes stale-preview cleanup", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("duplicate-reservation avoidance", devEnvironment, StringComparison.Ordinal);
        Assert.DoesNotContain("Soul Tide Beckon pre-flush fix", devEnvironment, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentReleaseDocsAvoidReleaseJournalLines()
    {
        var docsToCheck = new[]
        {
            "docs/release-checklist.md",
            "docs/test-plan.md",
            "docs/test-ready-completion-audit.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/features/ancients-rework-v4/completion-audit.md"
        };
        var knownPassHistoryFragments = new[]
        {
            "strict hook/text audit",
            "Banner temporary Strength cleanup",
            "Ascension side-turn state fix",
            "Root Eyes stale-preview cleanup",
            "duplicate-reservation avoidance",
            "Soul Tide Beckon pre-flush fix"
        };
        var failures = new List<string>();

        foreach (var path in docsToCheck)
        {
            var text = ReadRepoText(path.Split('/'));
            var longestLine = text
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Max(line => line.Length);

            if (longestLine > 850)
            {
                failures.Add($"{path} has a release-journal line of {longestLine} characters.");
            }

            foreach (var fragment in knownPassHistoryFragments)
            {
                if (text.Contains(fragment, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale pass-history fragment `{fragment}`.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));

        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var ancientCompletionAudit = ReadRepoText("docs", "features", "ancients-rework-v4", "completion-audit.md");

        AssertSourceContains(
            releaseChecklist,
            "Current package hashes:",
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Fresh loader smoke for the current beta.47 package hash is pending",
            "Manual feature results are pending");
        AssertSourceContains(
            testPlan,
            "Current automated suite count and command results are recorded",
            "Historical Steam-client loader evidence",
            "manual feature matrix has runtime gameplay",
            "A20 multiplayer selection is not full A20 co-op support");
        AssertSourceContains(
            ancientCompletionAudit,
            "Detailed pass history lives in `docs/review.md` and `docs/archive/**`.",
            "Historical beta.19 package smoke",
            "gameplay/manual rows pending");
    }

    [Fact]
    public void CurrentDocsAvoidStaleNormalValidationCounts()
    {
        var docsToCheck = new[]
        {
            "README.md",
            "docs/BETA_COMPATIBILITY.md",
            "docs/dev-environment.md",
            "docs/toreview.md",
            "docs/test-ready-completion-audit.md",
            "docs/private-beta-verification-handoff.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/features/ancients-rework-v4/completion-audit.md",
            "docs/review.md"
        };
        var staleCounts = new[]
        {
            "202 passed / 18 skipped",
            "220 passed / 0 skipped",
            "235 passed / 18 skipped",
            "235 passed, 18 skipped",
            "244 passed / 18 skipped",
            "245 passed / 18 skipped",
            "246 passed / 18 skipped",
            "247 passed / 18 skipped",
            "248 passed / 18 skipped",
            "249 passed / 18 skipped",
            "250 passed / 18 skipped",
            "251 passed / 18 skipped",
            "252 passed / 18 skipped",
            "252 passed, 18 skipped",
            "253 passed / 18 skipped",
            "253 passed, 18 skipped",
            "253 passed / 0 skipped",
            "253 passed, 0 skipped",
            "257 passed / 18 skipped",
            "257 passed, 18 skipped",
            "258 passed / 18 skipped",
            "258 passed, 18 skipped",
            "262 passed / 18 skipped",
            "262 passed, 18 skipped",
            "263 passed / 18 skipped",
            "263 passed, 18 skipped",
            "264 passed / 20 skipped",
            "264 passed, 20 skipped",
            "266 passed / 20 skipped",
            "266 passed, 20 skipped",
            "267 passed / 20 skipped",
            "267 passed, 20 skipped",
            "268 passed / 20 skipped",
            "268 passed, 20 skipped",
            "269 passed / 20 skipped",
            "269 passed, 20 skipped",
            "280 passed / 0 skipped",
            "280 passed, 0 skipped",
            "281 passed / 0 skipped",
            "281 passed, 0 skipped",
            "286 passed / 0 skipped",
            "286 passed, 0 skipped",
            "287 passed / 0 skipped",
            "287 passed, 0 skipped",
            "289 passed / 0 skipped",
            "289 passed, 0 skipped",
            "275 passed / 0 skipped",
            "275 passed, 0 skipped",
            "264 passed / 0 skipped",
            "270 passed / 0 skipped",
            "270 passed, 0 skipped",
            "288 passed / 20 skipped",
            "288 passed, 20 skipped",
            "308 passed / 0 skipped",
            "308 passed, 0 skipped"
        };
        var staleSavedFieldCounts = new[]
        {
            "current source defines 26 SavedSpireFields",
            "Current source defines 26 SavedSpireFields",
            "current source now defines 26 SavedSpireFields",
            "Current source now defines 26 SavedSpireFields"
        };
        var failures = new List<string>();

        foreach (var path in docsToCheck)
        {
            var text = ReadRepoText(path.Split('/'));

            foreach (var staleCount in staleCounts)
            {
                if (text.Contains(staleCount, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale validation count `{staleCount}`.");
                }
            }

            foreach (var staleSavedFieldCount in staleSavedFieldCounts)
            {
                if (text.Contains(staleSavedFieldCount, StringComparison.Ordinal))
                {
                    failures.Add($"{path} contains stale saved-field count `{staleSavedFieldCount}`.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));

        var currentDocs = string.Join(
            Environment.NewLine,
            docsToCheck.Select(path => ReadRepoText(path.Split('/'))));
        Assert.Contains("295 passed / 20 skipped", currentDocs, StringComparison.Ordinal);
        Assert.Contains("315 passed / 0 skipped", currentDocs, StringComparison.Ordinal);
    }

    [Fact]
    public void CurrentChangelogUsesSpirePlusEnvironmentNamesFirst()
    {
        var changelog = ReadRepoText("docs", "mod-changelog.md");
        var scriptsReadme = ReadRepoText("scripts", "README.md");

        AssertSourceContains(
            changelog,
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "SPIREPLUS_DISABLE_MORVI=1",
            "preferred `SPIREPLUS_DISABLE_MORVI` plus legacy `EZMB_DISABLE_MORVI`",
            "preferred `SPIREPLUS_DISABLE_LOTHA` plus legacy `EZMB_DISABLE_LOTHA`",
            "SPIREPLUS_FORCE_MORVI_BLESSING=morvi_misprint_press",
            "SPIREPLUS_DISABLE_URDA=1",
            "Legacy `EZMB_ENABLE_VAKUU_FIGHT=1` still works",
            "Legacy `EZMB_FORCE_MORVI_BLESSING` still works",
            "Legacy `EZMB_DISABLE_URDA=1` still works");
        AssertSourceContains(
            scriptsReadme,
            "SPIREPLUS_ENABLE_VAKUU_FIGHT=1",
            "legacy `EZMB_ENABLE_VAKUU_FIGHT=1`");
        Assert.DoesNotContain("behind `EZMB_ENABLE_VAKUU_FIGHT=1`, `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_DISABLE_LOTHA` / `SPIREPLUS_DISABLE_LOTHA`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("Set TEZMB_FORCE_MORVI_BLESSING=", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("Set `EZMB_DISABLE_URDA=1`", changelog, StringComparison.Ordinal);
        Assert.DoesNotContain("`EZMB_ENABLE_VAKUU_FIGHT=1` / `SPIREPLUS_ENABLE_VAKUU_FIGHT=1`", scriptsReadme, StringComparison.Ordinal);
    }
}
