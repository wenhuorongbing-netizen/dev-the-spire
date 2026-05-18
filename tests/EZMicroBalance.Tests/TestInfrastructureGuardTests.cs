using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class TestInfrastructureGuardTests
{
    [Fact]
    public void GuardTestsUseSharedRepositoryPathHelpers()
    {
        var duplicateHelperPattern = new Regex(
            @"\bprivate\s+static\s+(?:(?:[^(;\r\n]+)|(?:\([^)]*\)))\s+(ReadRepoText|ReadSharedText|ReadAllTextShared|RepoPath|GamePath|FindRepoRoot|ToRepoRelativePath|AssertRepoFileExists|AssertRepoDirectoryExists|AssertRepoPathDoesNotExist|AssertDirectoryContainsOnlyFiles|AssertSourceContains|AssertLocalizedKeys|AssertNoMojibake|JsonStringMap|JsonStringValues|JsonKeys|ManifestVersion|ReadCurrentFacingDocs|SliceFrom|SliceBetween|SourceSlice|AssertBefore|CountOccurrences|ReadSourceTree|ReadAllTestSource|ReadZipBytes|ReadZipText|ReadPckDirectory|Sha256|NormalizeJson|ReadPngBytes|ReadPngDimensions|ReadPngSize|AssertSmallUiPngHasAlpha|ReadBigEndianInt32|Unwrap|ReadAncientSource|ReadAscensionSource|ParseExportFiles|IsActiveExportResource|IsActiveReleaseResource)\s*\(",
            RegexOptions.CultureInvariant);

        var duplicateHelpers = Directory
            .GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path => !Path.GetFileName(path).Equals("TestRepo.cs", StringComparison.Ordinal))
            .SelectMany(path => duplicateHelperPattern
                .Matches(File.ReadAllText(path))
                .Select(match => $"{ToRepoRelativePath(path)}:{match.Groups[1].Value}"))
            .OrderBy(match => match, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            duplicateHelpers.Length == 0,
            "Guard tests should use TestRepo.cs instead of copying repository path helpers:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, duplicateHelpers));
    }

    [Fact]
    public void TestReadmeDocumentsSharedRepositoryHelpers()
    {
        var readme = ReadRepoText("tests", "EZMicroBalance.Tests", "README.md");

        Assert.Contains("TestRepo.cs", readme, StringComparison.Ordinal);
        Assert.Contains("instead of copying local `FindRepoRoot`, `RepoPath`, or `ReadRepoText` helpers", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared `ReadSharedText` helper for logs", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared repository path assertion helpers", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared `AssertSourceContains` helper for source-shape evidence checks", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared `AssertNoMojibake` helper", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared `AssertLocalizedKeys` helper", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared JSON/source-slicing helpers for common guard-test parsing", readme, StringComparison.Ordinal);
        Assert.Contains("`JsonKeys`", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared manifest, PNG byte/dimension, small-UI PNG alpha, JSON normalization, and exception-unwrapping helpers", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared export-preset parser", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared active release resource predicates", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void HistoricalFeatureInputsStayInCentralArchive()
    {
        var featureLocalArchiveDirectories = new[]
        {
            RepoPath("docs", "features", "ancients-rework-v4", "archive"),
            RepoPath("docs", "features", "ascension-11-20", "archive")
        };
        var existingFeatureLocalArchives = featureLocalArchiveDirectories
            .Where(Directory.Exists)
            .Select(ToRepoRelativePath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            existingFeatureLocalArchives.Length == 0,
            "Historical prompt/spec archives should live under docs/archive/feature-inputs, not inside active feature folders:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, existingFeatureLocalArchives));

        var centralArchiveReadme = ReadRepoText("docs", "archive", "feature-inputs", "README.md");
        Assert.Contains("docs/archive/feature-inputs", centralArchiveReadme, StringComparison.Ordinal);
        Assert.Contains("Current development should start from `docs/README.md`", centralArchiveReadme, StringComparison.Ordinal);
        Assert.DoesNotContain("feature's `archive/` subfolder", ReadRepoText("docs", "features", "README.md"), StringComparison.Ordinal);
        Assert.DoesNotContain("Historical prompt/spec material lives in `archive/`", ReadRepoText("docs", "features", "ascension-11-20", "README.md"), StringComparison.Ordinal);

        var staleFeatureArchiveReferencePattern = new Regex(
            @"(?:docs/)?features/(?:ancients-rework-v4|ascension-11-20)/archive/|(?:ancients-rework-v4|ascension-11-20)/archive/",
            RegexOptions.CultureInvariant);
        var currentDocumentationFiles = Directory
            .GetFiles(RepoPath("docs"), "*.md", SearchOption.AllDirectories)
            .Where(path =>
            {
                var relativePath = ToRepoRelativePath(path);
                return !relativePath.StartsWith("docs/archive/", StringComparison.Ordinal) &&
                       !relativePath.Equals("docs/doc-inventory.md", StringComparison.Ordinal);
            });
        var currentTestFiles = Directory.GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly);
        var staleReferences = currentDocumentationFiles
            .Concat(currentTestFiles)
            .SelectMany(path => staleFeatureArchiveReferencePattern
                .Matches(File.ReadAllText(path))
                .Select(match => $"{ToRepoRelativePath(path)}:{match.Value}"))
            .OrderBy(reference => reference, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            staleReferences.Length == 0,
            "Current docs/tests should reference docs/archive/feature-inputs instead of old feature-local archive paths:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, staleReferences));
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
            "Future Peek is a separate mod idea",
            "No live-game, save-load, death/failure, or co-op evidence may be claimed from these commands.");
        Assert.DoesNotContain("One-Shot Prompt", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## Subagent Plan", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P0:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P1:", goal, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("## P2:", goal, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WebsitePreviewDraftStaysOutOfReleaseCandidateDiff()
    {
        var gitIgnore = ReadRepoText(".gitignore");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");

        Assert.Contains("/website/", gitIgnore, StringComparison.Ordinal);
        Assert.Contains("/.github/workflows/spire-plus-site.yml", gitIgnore, StringComparison.Ordinal);
        Assert.Contains("Removed ignored local draft", projectMap, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-website-preview-20260516", projectMap, StringComparison.Ordinal);
        Assert.Contains("local-website-preview-20260516", docInventory, StringComparison.Ordinal);
        Assert.Contains("deleted after preserving snapshot", docInventory, StringComparison.Ordinal);
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
        Assert.Contains("Latest normal Steam-client startup/log evidence is historical for the pre-review Spire Plus package", projectState, StringComparison.Ordinal);
        Assert.Contains("Current manual-test package is not a release-readiness claim", projectState, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", projectState, StringComparison.Ordinal);
        Assert.Contains("git diff --check", projectState, StringComparison.Ordinal);

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
        Assert.Contains("toreview-pre-slim-20260518.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("toreview-pre-slim-20260518.md", archiveReadme, StringComparison.Ordinal);
    }

    [Fact]
    public void RootLocalClutterStaysArchivedOrIgnored()
    {
        var gitIgnore = ReadRepoText(".gitignore");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");
        var cleanupAudit = ReadRepoText("docs", "worktree-cleanup-audit.md");

        AssertRepoPathDoesNotExist("art_pipeline");
        AssertRepoPathDoesNotExist("asset");

        if (Directory.Exists(RepoPath(".tools", "archive")))
        {
            AssertRepoDirectoryExists(".tools", "archive", "local-art-and-calibration-20260515");
            AssertRepoDirectoryExists(".tools", "archive", "local-root-clutter-20260515");
        }

        Assert.Contains("/art_pipeline/", gitIgnore, StringComparison.Ordinal);
        Assert.Contains("/asset/", gitIgnore, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-art-and-calibration-20260515", projectMap, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-root-clutter-20260515", projectMap, StringComparison.Ordinal);
        Assert.Contains("local-art-and-calibration-20260515", docInventory, StringComparison.Ordinal);
        Assert.Contains("local-root-clutter-20260515", docInventory, StringComparison.Ordinal);
        Assert.Contains("Root local art/calibration folders", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("Root local clutter archives", cleanupAudit, StringComparison.Ordinal);
    }

    [Fact]
    public void OwnerSensitiveLocalMaterialStaysIgnoredAndDecisionTracked()
    {
        var gitIgnore = ReadRepoText(".gitignore");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var cleanupAudit = ReadRepoText("docs", "worktree-cleanup-audit.md");

        AssertSourceContains(
            gitIgnore,
            ".tools/",
            "publish/",
            "/source code/",
            "Directory.Build.props",
            "*.zip",
            "*.dll",
            "*.pck");

        AssertSourceContains(
            projectMap,
            "| `source code/` | Ignored local scratch |",
            "| `.tools/` | Ignored local tools |",
            "| `publish/` | Ignored release output |");

        AssertSourceContains(
            cleanupAudit,
            "| `source code/` | `docs/test-ready-development-goal.md` names `source code/src/Core/**` as primary source evidence",
            "| `publish/` | Package refresh scripts and opt-in release-artifact tests use `publish/SpirePlus-v0.1.0-private-beta.0.zip`",
            "| `.tools/` | Remaining subfolders are runtime evidence, generated art provenance, local archives, downloaded/decompiled game tooling, Godot, or ILSpy.",
            "Future targeted prune only for newly proven generated clutter.",
            "Unreferenced Edge browser profile/cache folders, stale redirected publish outputs, an old install backup, and generated Playwright/Godot cache folders were deleted",
            "stale redirected publish-output folders",
            "| `source code/` | Default keep because current tests/docs require it. |",
            "| `publish/` | Retained current package/staging/cover-source output; stale old-name package folder was deleted. Future prune should happen only after a new package rebuild/hash refresh. |",
            "| `.tools/` | Unreferenced Edge browser profile/cache folders, stale redirected publish outputs, an old install backup, and generated Playwright/Godot cache folders were deleted; remaining `.tools/` subfolders are retained as current evidence, art provenance, local archives, or local tool installations. Wholesale deletion is not recommended. |");
    }

    [Fact]
    public void CleanupAuditKeepsPromptCoverageAndOwnerDeletionDecisions()
    {
        var cleanupAudit = ReadRepoText("docs", "worktree-cleanup-audit.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");

        Assert.Contains("worktree-cleanup-audit.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("worktree-cleanup-audit.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/worktree-cleanup-audit.md", docInventory, StringComparison.Ordinal);

        AssertSourceContains(
            cleanupAudit,
            "# Worktree Cleanup Audit",
            "## Objective",
            "## Refactor Cleanup Completed",
            "## Completion Audit Against Cleanup Goal",
            "## Prompt-To-Artifact Checklist",
            "## Owner Deletion Decision Checklist",
            "Use this checklist before any permanent deletion.",
            "Confirm whether every uncertain area is useless before permanent deletion",
            "Complete: remaining retained areas have current evidence or hard-rule justification",
            "Remaining large ignored/local areas are retained intentionally",
            "`EzDailyContent/`, `EzDailyContentCode/`, `EzDailyContent.json`",
            "Former root `legacy/`",
            "`source code/`",
            "`publish/`",
            "`.tools/`",
            "`website/` and `.github/workflows/spire-plus-site.yml`");

        Assert.DoesNotContain("Status: Complete", cleanupAudit, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("| `source code/` | Default keep because current tests/docs require it. |", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("| `website/` and `.github/workflows/spire-plus-site.yml` | Deleted local draft after verifying `.tools/archive/local-website-preview-20260516/` snapshot; keep deleted unless deliberately promoted later. |", cleanupAudit, StringComparison.Ordinal);
    }
}
