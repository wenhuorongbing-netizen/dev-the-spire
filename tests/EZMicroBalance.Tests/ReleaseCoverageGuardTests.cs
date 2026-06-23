using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseCoverageGuardTests
{
    [Fact]
    public void ImplementedAncientSystemsHaveSourceDocsAndLocalizationCoverage()
    {
        var allAncientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var englishRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var simplifiedChineseRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var englishRestSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var simplifiedChineseRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");

        foreach (var system in ImplementedAncientSystems)
        {
            Assert.Contains($"| {system.ManualRow} |", manualMatrix, StringComparison.Ordinal);
            AssertLocalizedKeys(system.RelicKeys, englishRelics, simplifiedChineseRelics, $"relic localization for {system.ManualRow}");
            AssertLocalizedKeys(system.CardKeys, englishCards, simplifiedChineseCards, $"card localization for {system.ManualRow}");
            AssertLocalizedKeys(system.RestSiteKeys, englishRestSite, simplifiedChineseRestSite, $"rest-site localization for {system.ManualRow}");

            foreach (var snippet in system.SourceSnippets)
            {
                Assert.Contains(snippet, allAncientSource, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void GatedRootFamilySystemsHaveSourceDocsAndLocalizationCoverage()
    {
        var allAscensionSource = ReadSourceTree("EZMicroBalanceCode", "Ascension");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var englishCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var simplifiedChineseCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        AssertSourceContains(
            allAscensionSource,
            "RootBeginsLevel = 14",
            "BossRootBudLevel = 15",
            "EliteRootBudLevel = 18",
            "DebugLevelEnvironmentVariable = \"SPIREPLUS_ASCENSION_DEBUG_LEVEL\"",
            "LegacyDebugLevelEnvironmentVariable = \"EZMB_ASCENSION_DEBUG_LEVEL\"",
            "return 0;",
            "RootFamilyCard",
            "RootBud",
            "RootblightIII",
            "RootblightLevel",
            "RootDeckService.AddRootblightI",
            "RootDeckService.ResolveCombatEndRootblight",
            "CardPileCmd.RemoveFromDeck(card, showPreview: false)");

        AssertLocalizedKeys(
            [
                "EZMB_ROOT.title",
                "EZMB_ROOT.description",
                "EZMB_DEEP_ROOT.title",
                "EZMB_DEEP_ROOT.description",
                "EZMB_ROOTBLIGHT_III.title",
                "EZMB_ROOTBLIGHT_III.description",
                "EZMB_ROOT_BUD.title",
                "EZMB_ROOT_BUD.description"
            ],
            englishCards,
            simplifiedChineseCards,
            "Root-family card localization");

        Assert.Contains("A14 Rootblight MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A15 Boss Blight Sprout MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A18 Elite Blight Sprout MVP", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("one visible Exhaust keyword, no duplicate `Play: Exhaust` body text", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight II has one visible Exhaust keyword", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Blight Sprout has one visible Exhaust keyword", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("localized `[gold]Rootblight[/gold] added.` / `[gold]\u6839\u8680[/gold]\u5df2\u52a0\u5165\u3002` notice", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Rootblight I/II/III and Blight Sprout are implemented for A14/A15/A18 after the current standard-lobby selector expansion.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("## Live Evidence Protocol", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns -Launch", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/check-spire-window-preflight.ps1 -OutFile <evidence-dir>\\window-preflight.json -RequireSpireForeground", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/audit-godot-log.ps1 -Path <evidence-dir>\\godot.log -OutFile <evidence-dir>\\godot-log-audit.json -FailOnHit", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("scripts/spire-plus-live-session.ps1 -Mode Restore -EvidenceDir <evidence-dir> -StopGameOnRestore -PreserveNewCurrentRunsOnRestore", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Covered desktop captures, wrong-surface captures, or sessions that never reach the target game surface do not satisfy Rootblight, Ascension, or gameplay rows.", manualChecklist, StringComparison.Ordinal);

        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1` to restore vanilla A1-A10 selection for comparison.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Set `SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` to disable only host-multiplayer A11-A20 selection.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("`EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is legacy-compatible and no longer required.", releaseChecklist, StringComparison.Ordinal);
    }

    [Fact]
    public void CardLocalizationStyleGuideIsIndexedAndCoversPreviewKeywordRules()
    {
        var guide = ReadRepoText("docs", "style", "card-localization-style-guide.md");
        var agents = ReadRepoText("AGENTS.md");
        var skill = ReadRepoText("docs", "skills", "sts2-godot-mod-development.md");
        var docsIndex = ReadRepoText("docs", "README.md");

        AssertSourceContains(
            guide,
            "CanonicalKeywords",
            "duplicate Exhaust",
            "[gold]",
            "card preview",
            "English and Simplified Chinese",
            "Rootblight = \u6839\u8680",
            "Blight Sprout / Root Bud = \u6839\u82bd",
            "HoverTipFactory.FromCard<T>()",
            "ModelDb",
            "Preview cards must");

        Assert.Contains("docs/style/card-localization-style-guide.md", agents, StringComparison.Ordinal);
        Assert.Contains("docs/style/card-localization-style-guide.md", skill, StringComparison.Ordinal);
        Assert.Contains("style/card-localization-style-guide.md", docsIndex, StringComparison.Ordinal);
    }

    [Fact]
    public void DocumentationOverhaulKeepsRepositoryRootDocsCanonicalAndArchiveMetadataVisible()
    {
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docsReadme = ReadRepoText("docs", "README.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var overduePrompt = ReadRepoText("docs", "archive", "prompts", "2026-05", "codex-repo-overhaul-refactor-prompt.md");
        var urdaOvernightPrompt = ReadRepoText("docs", "archive", "prompts", "2026-05", "codex-urda-overnight-prompt.md");
        var urdaAddendum = ReadRepoText("docs", "archive", "prompts", "2026-05", "issues-urda-overnight-addendum.md");

        Assert.Contains("`../PROJECT_STATE.md`", docsReadme, StringComparison.Ordinal);
        Assert.Contains("`docs/archive/`", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/issues/waiting-tests.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", overduePrompt, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", urdaOvernightPrompt, StringComparison.Ordinal);
        Assert.Contains("# Archived prompt (2026-05)", urdaAddendum, StringComparison.Ordinal);

        var rootDocs = Directory.GetFiles(RepoPath("docs"), "*.md", SearchOption.TopDirectoryOnly)
            .Select(path => Path.GetFileName(path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var movedPrompt in new[]
        {
            "codex-repo-overhaul-refactor-prompt.md",
            "codex-urda-overnight-prompt.md",
            "issues-urda-overnight-addendum.md",
            "issues-waiting-tests.md"
        })
        {
            Assert.DoesNotContain(movedPrompt, rootDocs);
        }

        AssertRepoDirectoryExists("docs", "issues");
        AssertRepoDirectoryExists("docs", "archive", "prompts", "2026-05");
        AssertRepoFileExists("docs", "issues", "waiting-tests.md");
        AssertRepoFileExists("docs", "archive", "issues", "waiting-tests-pre-slim-20260518.md");
        AssertRepoFileExists("docs", "archive", "feature-audits", "review-pre-slim-20260518.md");
        AssertRepoFileExists("docs", "archive", "prompts", "2026-05", "codex-urda-overnight-prompt.md");
        AssertRepoFileExists("docs", "archive", "prompts", "2026-05", "issues-urda-overnight-addendum.md");

        var waitingTests = ReadRepoText("docs", "issues", "waiting-tests.md");
        var review = ReadRepoText("docs", "review.md");
        Assert.Contains("Compact manual evidence queue", docsReadme, StringComparison.Ordinal);
        Assert.Contains("Full historical issue text was archived", waitingTests, StringComparison.Ordinal);
        Assert.Contains("MP-MAC-MODELDB-HASH", waitingTests, StringComparison.Ordinal);
        Assert.True(waitingTests.Split('\n').Length <= 80, "Keep the active waiting-tests queue compact; archive detailed historical rows.");
        Assert.Contains("review-pre-slim-20260518.md", review, StringComparison.Ordinal);
        Assert.True(review.Split('\n').Length <= 140, "Keep docs/review.md compact; archive detailed historical review logs.");
    }

    [Fact]
    public void GatedAscensionSlicesHaveSourceDocsAndManualCoverage()
    {
        var allAscensionSource = ReadSourceTree("EZMicroBalanceCode", "Ascension");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            allAscensionSource,
            "WiderLongerMapLevel = 11",
            "A11ExtraMapColumns = 1",
            "A11ActOneExtraMapRows = 1",
            "A11ActTwoExtraMapRows = 1",
            "A11ActThreeExtraMapRows = 2",
            "AscensionFeatureGate.IsMapGeometryEnabled(runState)",
            "DeepBranchesLevel = 17",
            "AscensionFeatureGate.IsDeepBranchesEnabled(runState)",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "TryInsertDeepBranch",
            "safe-route reconnect",
            "canBeModified: false",
            "DeepBranchNodeKind.EnhancedReward",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "Ascension A11 applied: expanded map width",
            "reachable optional route",
            "TryInsertA11WidthChoice(saved)",
            "new SavedActMap(saved)",
            "A17 gate active: Deep Branch already present or unsupported for safe insertion");
        Assert.Contains("A11 converts the generated map", apiResearch, StringComparison.Ordinal);
        Assert.Contains("No A11-specific marker", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("LONG_ROAD_NODE", allAscensionSource, StringComparison.Ordinal);
        Assert.DoesNotContain("LongRoad", allAscensionSource, StringComparison.Ordinal);
        Assert.Contains("A17 uses the same saved-map replacement path", apiResearch, StringComparison.Ordinal);

        foreach (var slice in ImplementedGatedAscensionSlices)
        {
            AssertSourceContains(allAscensionSource, slice.SourceSnippets);
            AssertSourceContains(apiResearch, slice.ApiSnippets);

            var manualSection = SliceBetween(manualChecklist, slice.ManualSectionStart, slice.ManualSectionEnd);
            AssertSourceContains(manualSection, slice.ManualSnippets);
            Assert.DoesNotContain("- [x]", manualSection, StringComparison.Ordinal);
            Assert.DoesNotContain("Release-ready", manualSection, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void ExportPresetTracksEveryActiveResourceAndExcludesNonReleaseFolders()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var exported = ParseExportFiles(exportPreset);
        var expected = Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".import", StringComparison.Ordinal))
            .Where(IsActiveExportResource)
            .Select(path => "res://" + ToRepoRelativePath(path))
            .Concat(["res://EZMicroBalance.json"])
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, exported.OrderBy(path => path, StringComparer.Ordinal));
        Assert.DoesNotContain("res://EZMicroBalance/mod_real.png", exported);

        Assert.Contains("export_filter=\"resources\"", exportPreset, StringComparison.Ordinal);
        Assert.Contains("include_filter=\"EZMicroBalance.json,EZMicroBalance/localization/*/*.json,EZMicroBalance/localization/*/*/*.json\"", exportPreset, StringComparison.Ordinal);
        AssertSourceContains(
            exportPreset,
            "EZMicroBalanceCode/*",
            "art_pipeline/*",
            "asset/*",
            "source code/*",
            "docs/*",
            "legacy/*");
    }

    [Fact]
    public void IssuesIndexIsCompactAndRoutesUrdaDetailsToFeatureIssueDocs()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var urdaIssueIndex = ReadRepoText("docs", "issues", "urda.md");
        var logAuditScript = ReadRepoText("scripts", "audit-godot-log.ps1");

        AssertSourceContains(
            logAuditScript,
            "Creature\\.get_ShowsInfiniteHp",
            "DependencyFramework\\.Patches\\.UI\\.HealthBarForecastPatch",
            "\\[ERROR\\]",
            "TypeLoadException",
            "MissingMethodException",
            "Spire Plus error/exception",
            "FailOnHit",
            "ConvertTo-Json");
        Assert.DoesNotContain("retired framework", logAuditScript, StringComparison.OrdinalIgnoreCase);

        var godotErrorPatternMatch = Regex.Match(logAuditScript, @"Name = 'Godot ERROR line'; Pattern = '([^']+)'");
        Assert.True(godotErrorPatternMatch.Success, "Missing Godot ERROR line signature pattern.");
        var godotErrorPattern = godotErrorPatternMatch.Groups[1].Value;
        Assert.Matches(godotErrorPattern, "[ERROR] Mod manifest bad");
        Assert.Matches(godotErrorPattern, "ERROR Mod manifest bad");
        Assert.Matches(godotErrorPattern, "[Godot] ERROR Mod manifest bad");
        Assert.DoesNotMatch(godotErrorPattern, "[INFO] [previous package] Applied 177 patches successfully, 0 failed");

        Assert.Contains("## Active blockers", issues, StringComparison.Ordinal);
        Assert.Contains("## Issue detail links", issues, StringComparison.Ordinal);
        Assert.Contains("docs/issues/urda.md", issues, StringComparison.Ordinal);
        Assert.Contains("docs/issues/waiting-tests.md", issues, StringComparison.Ordinal);
        Assert.Contains("Current package hashes, 2026-06-23 beta.131:", issues, StringComparison.Ordinal);
        Assert.Contains("`URDA-PROTOTYPE` P0 open", issues, StringComparison.Ordinal);
        Assert.DoesNotContain("Status: resolved", issues, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\uFFFD", issues, StringComparison.Ordinal);

        Assert.Contains("Urda is default-on", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("EZMB_DISABLE_URDA", urdaIssueIndex, StringComparison.Ordinal);
        Assert.Contains("prototype", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Lotha is now default-on in the active test slice", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", urdaIssueIndex, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProjectStateAndHandoffClaimsTrackCurrentHeadAndNoStaleBaselineRefs()
    {
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var handoff = ReadRepoText("docs", "private-beta-verification-handoff.md");
        var readme = ReadRepoText("docs", "README.md");

        Assert.Contains("Current reviewed state", projectState, StringComparison.Ordinal);
        Assert.Contains("Latest package target is beta.131", projectState, StringComparison.Ordinal);
        Assert.Contains("build, publish, package refresh, installed-package parity, runtime preflight, source-workspace validation, and local RitsuLib runtime install are current", projectState, StringComparison.Ordinal);
        Assert.Contains("Forced clicked Ancient UI smoke remains previous beta.128 package evidence", projectState, StringComparison.Ordinal);
        Assert.Contains("Previous beta.128 clicked Ancient UI smoke proof is", projectState, StringComparison.Ordinal);
        Assert.Contains("Previous beta.99 settings/off proof, beta.96 Off proof, and beta.93 AdditiveBatch1 proof are previous-package context", projectState, StringComparison.Ordinal);
        Assert.Contains("git log -1 --oneline --decorate", projectState, StringComparison.Ordinal);
        Assert.Contains("a2183ee", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("5be5c51", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Refresh beta35 package guards", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("f201508", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("git log -1 --oneline --decorate", handoff, StringComparison.Ordinal);
        Assert.Contains("git status --short --branch", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("f201508", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("b82023c", handoff, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Current git status before", handoff, StringComparison.Ordinal);
        Assert.DoesNotContain("A1.05.01", handoff, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("PROJECT_STATE.md", readme, StringComparison.Ordinal);
    }
}
