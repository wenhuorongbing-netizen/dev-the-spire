using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ActiveSourceManifestGuardTests
{
    [Fact]
    public void ActiveSourceFilesAreCoveredByTheGuardManifest()
    {
        var activeSources = Directory.GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
            .Select(path => ToRepoRelativePath(path))
            .Where(path => path != "EZMicroBalanceCode/Ancients/GlobalUsings.cs")
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ExpectedActiveSourceFiles.OrderBy(path => path, StringComparer.Ordinal), activeSources);

        var coverageRoots = IndependentCoverageRoots;
        var uncoveredSources = activeSources
            .Where(activeSource => !coverageRoots.Any(root => root.Covers(activeSource)))
            .ToArray();

        Assert.Empty(uncoveredSources);

        foreach (var root in coverageRoots)
        {
            Assert.Contains(activeSources, activeSource => root.Covers(activeSource));
            Assert.NotEqual($"{nameof(ActiveSourceManifestGuardTests)}.cs", root.GuardTestFile);

            var guardSource = ReadRepoText("tests", "EZMicroBalance.Tests", root.GuardTestFile);
            Assert.Contains(root.GuardEvidenceSnippet, guardSource, StringComparison.Ordinal);
        }
    }
}
