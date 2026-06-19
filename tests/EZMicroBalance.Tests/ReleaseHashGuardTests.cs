using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseHashGuardTests
{
    [Fact]
    public void CurrentStatusDocsUseLatestPackageHashes()
    {
        var currentHashes = CurrentPackageHashesFromIssues();
        var currentZipHash = CurrentPackageZipSha256();
        Assert.Equal(currentZipHash, currentHashes["ZIP"]);
        var currentDllHash = currentHashes["DLL"];
        var currentPckHash = currentHashes["PCK"];
        var currentManifestHash = currentHashes["Manifest"];
        var currentReadmeHash = currentHashes["README_INSTALL"];

        var currentStatusDocs = new[]
        {
                ReadRepoText("PROJECT_STATE.md"),
                ReadRepoText("docs", "issues.md"),
                ReadRepoText("docs", "dev-environment.md"),
                ReadRepoText("docs", "private-beta-verification-handoff.md"),
                ReadRepoText("docs", "private-beta-release-completion-audit.md"),
                ReadRepoText("docs", "release-checklist.md"),
                ReadRepoText("docs", "test-ready-completion-audit.md")
            };

        foreach (var doc in currentStatusDocs)
        {
            AssertDoesNotContainKnownStalePackageHashes(doc);
        }

        AssertSourceContains(
            ReadRepoText("docs", "issues.md"),
            currentZipHash,
            currentDllHash,
            currentPckHash,
            currentManifestHash,
            currentReadmeHash);
    }
}
