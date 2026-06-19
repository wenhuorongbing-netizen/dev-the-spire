using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private static void AssertEnvironmentIncludesGitHandoffEvidence(string environmentPath)
    {
        using var environmentDocument = JsonDocument.Parse(File.ReadAllText(environmentPath));
        var root = environmentDocument.RootElement;

        Assert.True(root.TryGetProperty("GitHead", out var gitHead), $"Missing GitHead in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitStatusShort", out var gitStatusShort), $"Missing GitStatusShort in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitBranchStatus", out var gitBranchStatus), $"Missing GitBranchStatus in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitUpstream", out var gitUpstream), $"Missing GitUpstream in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitUpstreamHead", out var gitUpstreamHead), $"Missing GitUpstreamHead in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitPushedHead", out var gitPushedHead), $"Missing GitPushedHead in {environmentPath}.");
        Assert.True(root.TryGetProperty("GitHeadMatchesUpstream", out var gitHeadMatchesUpstream), $"Missing GitHeadMatchesUpstream in {environmentPath}.");
        Assert.True(root.TryGetProperty("Git", out var git), $"Missing nested Git evidence in {environmentPath}.");

        Assert.Equal(gitHead.GetString(), git.GetProperty("Head").GetString());
        Assert.Equal(gitStatusShort.GetString(), git.GetProperty("StatusShort").GetString());
        Assert.Equal(gitBranchStatus.GetString(), git.GetProperty("BranchStatus").GetString());
        Assert.Equal(gitUpstream.GetString(), git.GetProperty("Upstream").GetString());
        Assert.Equal(gitUpstreamHead.GetString(), git.GetProperty("UpstreamHead").GetString());
        Assert.Equal(gitPushedHead.GetString(), git.GetProperty("PushedHead").GetString());
        Assert.Equal(gitHeadMatchesUpstream.GetBoolean(), git.GetProperty("HeadMatchesUpstream").GetBoolean());
        Assert.True(git.TryGetProperty("LatestCommit", out _), $"Missing Git.LatestCommit in {environmentPath}.");
        Assert.True(git.TryGetProperty("Branch", out _), $"Missing Git.Branch in {environmentPath}.");
    }

    private static void AssertPackageHashesUseVersionedArtifacts(string packageHashesPath)
    {
        using var packageDocument = JsonDocument.Parse(File.ReadAllText(packageHashesPath));
        var files = packageDocument.RootElement
            .GetProperty("Files")
            .EnumerateArray()
            .ToArray();
        var paths = files
            .Select(file => file.GetProperty("Path").GetString())
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(CurrentPackageZipRelativePath(), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.dll"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.pck"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("EZMicroBalance.json"), paths);
        Assert.Contains(CurrentPackageArtifactRelativePath("README_INSTALL.txt"), paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.dll", paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.pck", paths);
        Assert.DoesNotContain("publish\\EZMicroBalance.json", paths);

        foreach (var file in files.Where(file => file.GetProperty("Path").GetString()?.StartsWith($"publish\\{CurrentPackageName()}", StringComparison.Ordinal) == true))
        {
            Assert.True(file.GetProperty("Exists").GetBoolean(), $"Package hash row points at a missing package artifact: {file.GetProperty("Path").GetString()}");
        }
    }
}
