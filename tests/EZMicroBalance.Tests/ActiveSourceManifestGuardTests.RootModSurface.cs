using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ActiveSourceManifestGuardTests
{
    [Fact]
    public void OnlySpirePlusIsAnActiveRootModSurface()
    {
        foreach (var removedRootSurface in new[]
        {
            "EzDailyContent",
            "EzDailyContentCode",
            "EzDailyContent.json",
            "EZFuturePeek",
            "EZFuturePeekCode",
            "EZFuturePeek.csproj",
            "EZFuturePeek.json",
            "EZFuturePeek.sln",
            Path.Combine("tests", "EZFuturePeek.Tests")
        })
        {
            Assert.False(Directory.Exists(RepoPath(removedRootSurface)), $"{removedRootSurface} should not remain as an active mod directory.");
            Assert.False(File.Exists(RepoPath(removedRootSurface)), $"{removedRootSurface} should not remain as an active mod file.");
        }
    }
}
