using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void FullLocalCiDoesNotAddRuntimeMonkeyAsDefaultLiveLane()
    {
        var ci = ReadRepoText("scripts", "ci-full-validation.ps1");

        Assert.DoesNotContain("run-spire-plus-monkey-stability.ps1", ci, StringComparison.Ordinal);
        Assert.DoesNotContain("check-spire-plus-runtime-monkey-packet.ps1", ci, StringComparison.Ordinal);
        Assert.DoesNotContain("monkey-stability", ci, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorktreeBatchClassifierIncludesRuntimeMonkeySurfaces()
    {
        var classifier = ReadRepoText("scripts", "report-worktree-batches.ps1");
        var batch5Classifier = SliceBetween(
            classifier,
            "if ($p.StartsWith('scripts/'",
            "return 5");

        Assert.Contains("RuntimeMonkey", batch5Classifier, StringComparison.Ordinal);
        Assert.Contains("$p -eq 'docs/testing'", batch5Classifier, StringComparison.Ordinal);
        Assert.Contains("$p.StartsWith('docs/testing/'", batch5Classifier, StringComparison.Ordinal);
        Assert.DoesNotContain("$p.StartsWith('docs/', [System.StringComparison]", classifier, StringComparison.Ordinal);
    }
}
