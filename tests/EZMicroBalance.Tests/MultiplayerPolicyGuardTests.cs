using Xunit;

namespace EZMicroBalance.Tests;

public sealed class MultiplayerPolicyGuardTests
{
    [Fact]
    public void MultiplayerPolicyCentralizesAuthorityAndEvidenceLogging()
    {
        var policy = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerFeaturePolicy.cs");

        Assert.Contains("IsSingleplayer", policy, StringComparison.Ordinal);
        Assert.Contains("IsHost", policy, StringComparison.Ordinal);
        Assert.Contains("IsClient", policy, StringComparison.Ordinal);
        Assert.Contains("CanMutateSharedRunState", policy, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopFeature", policy, StringComparison.Ordinal);
        Assert.Contains("ReleaseEvidenceLog.Log", policy, StringComparison.Ordinal);
    }

    [Fact]
    public void A20CoopCannotRunUnverifiedSecondBossMutation()
    {
        var core = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var patches = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var map = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var events = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Events");

        Assert.Contains("ShouldDisableUnverifiedCoopFeature", core, StringComparison.Ordinal);
        Assert.Contains("A20KingBrand", core, StringComparison.Ordinal);
        Assert.Contains("second_boss_set", patches, StringComparison.Ordinal);
        Assert.Contains("second_boss_brand_gated", map, StringComparison.Ordinal);
        Assert.Contains("courtyard_entered", events, StringComparison.Ordinal);
    }

    [Fact]
    public void RootEyesSharedMapMutationRequiresPolicyGate()
    {
        var urda = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Urda");

        Assert.Contains("ShouldDisableUnverifiedCoopFeature", urda, StringComparison.Ordinal);
        Assert.Contains("UrdaRootEyes", urda, StringComparison.Ordinal);
        Assert.Contains("shared map preview mutation", urda, StringComparison.Ordinal);
        Assert.Contains("selection_opened", urda, StringComparison.Ordinal);
        Assert.Contains("node_selected", urda, StringComparison.Ordinal);
        Assert.Contains("preview_saved", urda, StringComparison.Ordinal);
    }
}
