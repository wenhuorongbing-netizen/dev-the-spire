using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientHighRiskSourceGuardTests
{
    [Fact]
    public void ManualAncientRuntimeEvidenceRemainsExplicitlyPending()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var ancientMatrix = SliceBetween(
            manualMatrix,
            "## Ancient Reward Matrix",
            "## Simplified Chinese Localization Spot Checks");

        Assert.Contains("- [x] Every implemented Ancient reward change has a manual checklist row.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("- [ ] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("- [x] Every implemented Ancient reward change has a completed manual runtime result.", releaseChecklist, StringComparison.Ordinal);
        Assert.Contains("Manual feature results are pending", releaseChecklist, StringComparison.Ordinal);

        Assert.Contains("| Prismatic Gem |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Meat Cleaver |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("| Blood-Soaked Rose / Enthralled |", ancientMatrix, StringComparison.Ordinal);
        Assert.Contains("Pending", ancientMatrix, StringComparison.Ordinal);
        Assert.DoesNotContain("| Pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Result: pass", ancientMatrix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("manually verified", ancientMatrix, StringComparison.OrdinalIgnoreCase);
    }
}
