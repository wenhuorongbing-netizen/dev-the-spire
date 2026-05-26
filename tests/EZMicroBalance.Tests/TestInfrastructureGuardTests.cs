using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class TestInfrastructureGuardTests
{
    [Fact]
    public void GuardTestsUseSharedRepositoryPathHelpers()
    {
        var duplicateHelperPattern = new Regex(
            @"\bprivate\s+static\s+(?:(?:[^(;\r\n]+)|(?:\([^)]*\)))\s+(ReadRepoText|ReadSharedText|ReadAllTextShared|RepoPath|GamePath|FindRepoRoot|ToRepoRelativePath|AssertRepoFileExists|AssertRepoDirectoryExists|AssertRepoPathDoesNotExist|AssertDirectoryContainsOnlyFiles|AssertSourceContains|AssertLocalizedKeys|AssertNoMojibake|JsonStringMap|JsonStringValues|JsonKeys|ManifestVersion|CurrentPackageName|CurrentPackageZipRelativePath|CurrentPackageZipPath|CurrentPackageZipSha256|CurrentPackageArtifactRelativePath|CurrentPackageHashesFromIssues|ReadCurrentFacingDocs|SliceFrom|SliceBetween|SourceSlice|AssertBefore|CountOccurrences|ReadSourceTree|ReadAllTestSource|ReadZipBytes|ReadZipText|ReadPckDirectory|Sha256|NormalizeJson|ReadPngBytes|ReadPngDimensions|ReadPngSize|AssertSmallUiPngHasAlpha|ReadBigEndianInt32|Unwrap|ReadAncientSource|ReadAscensionSource|ParseExportFiles|IsActiveExportResource|IsActiveReleaseResource)\s*\(",
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
    public void GuardTestsUseSharedCurrentReleaseState()
    {
        var testFiles = Directory
            .GetFiles(RepoPath("tests", "EZMicroBalance.Tests"), "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path =>
            {
                var fileName = Path.GetFileName(path);
                return !fileName.Equals("TestRepo.cs", StringComparison.Ordinal) &&
                       !fileName.Equals("TestInfrastructureGuardTests.cs", StringComparison.Ordinal);
            })
            .ToArray();

        var localCurrentFacingDocs = testFiles
            .Where(path => Regex.IsMatch(
                File.ReadAllText(path),
                @"\bprivate\s+static\s+readonly\s+string\[\]\s+CurrentFacingDocs\s*=",
                RegexOptions.CultureInvariant))
            .Select(ToRepoRelativePath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            localCurrentFacingDocs.Length == 0,
            "Guard tests should use TestRepo.CurrentFacingDocs instead of copying the current-facing doc list:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, localCurrentFacingDocs));

        var hardcodedCurrentPackageZipPaths = testFiles
            .SelectMany(path => Regex
                .Matches(
                    File.ReadAllText(path),
                    @"RepoPath\(\s*""publish""\s*,\s*\$""SpirePlus-\{ManifestVersion\(\)\}\.zip""\s*\)",
                    RegexOptions.CultureInvariant)
                .Select(match => $"{ToRepoRelativePath(path)}:{match.Value}"))
            .OrderBy(match => match, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            hardcodedCurrentPackageZipPaths.Length == 0,
            "Guard tests should use CurrentPackageZipPath or CurrentPackageZipSha256 instead of rebuilding the current package ZIP path:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, hardcodedCurrentPackageZipPaths));
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
        Assert.Contains("Use the shared current package helpers", readme, StringComparison.Ordinal);
        Assert.Contains("Use the shared `CurrentFacingDocs` list", readme, StringComparison.Ordinal);
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
    public void WebsiteSurfaceIsPromotedAndGeneratedForumOutputStaysIgnored()
    {
        var gitIgnore = ReadRepoText(".gitignore");
        var gitIgnoreLines = gitIgnore.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var docInventory = ReadRepoText("docs", "doc-inventory.md");

        Assert.DoesNotContain("/website/", gitIgnoreLines);
        Assert.DoesNotContain("/.github/workflows/spire-plus-site.yml", gitIgnoreLines);
        Assert.Contains("/website/forum/", gitIgnoreLines);
        Assert.Contains("/website/**/*.import", gitIgnoreLines);
        AssertRepoFileExists("website", "README.md");
        AssertRepoFileExists("website", "content-data.js");
        AssertRepoFileExists(".github", "workflows", "spire-plus-site.yml");
        AssertRepoFileExists("docs", "intro.zh.md");
        AssertRepoFileExists("docs", "archive", "implementation-records", "forum-public-integration-qa-20260526.md");
        AssertRepoFileExists("docs", "archive", "implementation-records", "website-localization-qa-20260522.md");
        AssertRepoPathDoesNotExist("docs", "介绍.md");
        AssertRepoPathDoesNotExist("web_issue.md");
        AssertRepoPathDoesNotExist("website", "localization_qa.md");
        Assert.Contains("Promoted website source is tracked", projectMap, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-website-preview-20260516", projectMap, StringComparison.Ordinal);
        Assert.Contains("docs/intro.zh.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("forum-public-integration-qa-20260526.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("website-localization-qa-20260522.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("current public site source is tracked", docInventory, StringComparison.Ordinal);
        Assert.Contains("docs/intro.zh.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("docs/介绍.md` -> `docs/intro.zh.md", docInventory, StringComparison.Ordinal);
        Assert.Contains("root `web_issue.md` -> `docs/archive/implementation-records/forum-public-integration-qa-20260526.md`", docInventory, StringComparison.Ordinal);
        Assert.Contains("website/localization_qa.md` -> `docs/archive/implementation-records/website-localization-qa-20260522.md`", docInventory, StringComparison.Ordinal);
        Assert.Contains("ignored `forum/node_modules/` dependency cache deleted", docInventory, StringComparison.Ordinal);
        Assert.Contains("local-website-preview-20260516", docInventory, StringComparison.Ordinal);
        Assert.Contains("generated `website/forum/` output is ignored", docInventory, StringComparison.Ordinal);
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
        Assert.Contains("/output/playwright/", gitIgnore, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-art-and-calibration-20260515", projectMap, StringComparison.Ordinal);
        Assert.Contains(".tools/archive/local-root-clutter-20260515", projectMap, StringComparison.Ordinal);
        Assert.Contains("local-art-and-calibration-20260515", docInventory, StringComparison.Ordinal);
        Assert.Contains("local-root-clutter-20260515", docInventory, StringComparison.Ordinal);
        Assert.Contains("Root local art/calibration folders", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("Root local clutter archives", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("Local Playwright screenshots/logs", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("new browser screenshots, HARs, logs, and PID files", cleanupAudit, StringComparison.Ordinal);
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
            "/output/playwright/",
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
            $"| `publish/` | Package refresh scripts and opt-in release-artifact tests use `{CurrentPackageZipRelativePath().Replace('\\', '/')}`",
            "| `.tools/` | Remaining subfolders are runtime evidence, generated art provenance, local archives, downloaded/decompiled game tooling, Godot, or ILSpy.",
            "duplicate root mod surfaces were removed after the owner rule change",
            "Future targeted prune only for newly proven generated clutter.",
            "Unreferenced Edge browser profile/cache folders, stale redirected publish outputs, an old install backup, and generated Playwright/Godot cache folders were deleted",
            "stale redirected publish-output folders",
            "| `source code/` | Default keep because current tests/docs require it. |",
            "| `publish/` | Retained current beta.52 package/staging/cover-source output; stale beta.0-beta.51 ZIPs and expanded folders are removed by the guarded prune after confirming current-package hash/path parity. Future prune should happen only after a new package rebuild/hash refresh. |",
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
            "Removed duplicate root mod surfaces",
            "Former root `legacy/`",
            "`source code/`",
            "`publish/`",
            "`.tools/`",
            "`website/` and `.github/workflows/spire-plus-site.yml`");

        Assert.DoesNotContain("Status: Complete", cleanupAudit, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("| `source code/` | Default keep because current tests/docs require it. |", cleanupAudit, StringComparison.Ordinal);
        Assert.Contains("Promoted and tracked as current public site source and Pages workflow. Generated `website/forum/` output and `website/**/*.import` metadata remain ignored", cleanupAudit, StringComparison.Ordinal);
    }
}
