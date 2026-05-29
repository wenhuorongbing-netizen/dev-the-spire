using Xunit;

namespace EZMicroBalance.Tests;

/// <summary>
/// Guard tests for architecture skeleton contracts.
/// Verifies RewardPipeline and CardPlayContext exist with correct shape.
/// </summary>
public sealed class ArchitectureSkeletonGuardTests
{
    // ── RewardPipeline ──────────────────────────────────────────────

    [Fact]
    public void RewardPipelineDefinesRewardPhaseEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipeline.cs");

        Assert.Contains("enum RewardPhase", source, StringComparison.Ordinal);
        Assert.Contains("PreGeneration", source, StringComparison.Ordinal);
        Assert.Contains("CardOptions", source, StringComparison.Ordinal);
        Assert.Contains("RoomRewards", source, StringComparison.Ordinal);
        Assert.Contains("PostProcessing", source, StringComparison.Ordinal);
        Assert.Contains("Finalized", source, StringComparison.Ordinal);
    }

    [Fact]
    public void RewardPipelineDefinesIRewardHandlerInterface()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipeline.cs");

        Assert.Contains("interface IRewardHandler", source, StringComparison.Ordinal);
        Assert.Contains("RewardPhase Phase", source, StringComparison.Ordinal);
        Assert.Contains("int Priority", source, StringComparison.Ordinal);
        Assert.Contains("void Handle(RewardPipelineContext context)", source, StringComparison.Ordinal);
    }

    [Fact]
    public void RewardPipelineIsDiagnosticsOnly()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipeline.cs");

        Assert.Contains("static void Diagnose(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ModifyReward", source, StringComparison.Ordinal);
        Assert.DoesNotContain("InjectReward", source, StringComparison.Ordinal);
        Assert.DoesNotContain("RemoveReward", source, StringComparison.Ordinal);
    }

    [Fact]
    public void RewardPipelineSortsHandlersByPhaseThenPriority()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipeline.cs");

        Assert.Contains("a.Phase.CompareTo(b.Phase)", source, StringComparison.Ordinal);
        Assert.Contains("a.Priority.CompareTo(b.Priority)", source, StringComparison.Ordinal);
    }

    // ── CardPlayContext ─────────────────────────────────────────────

    [Fact]
    public void CardPlayContextDefinesExtraPlayPolicyEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "CardPlayContext.cs");

        Assert.Contains("enum ExtraPlayPolicy", source, StringComparison.Ordinal);
        Assert.Contains("Allow", source, StringComparison.Ordinal);
        Assert.Contains("Block", source, StringComparison.Ordinal);
        Assert.Contains("FallbackToPower", source, StringComparison.Ordinal);
    }

    [Fact]
    public void CardPlayContextEnforcesDepthGuard()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "CardPlayContext.cs");

        Assert.Contains("const int MaxDepth", source, StringComparison.Ordinal);
        Assert.Contains("bool TryIncrementDepth()", source, StringComparison.Ordinal);
        Assert.Contains("void DecrementDepth()", source, StringComparison.Ordinal);
        Assert.Contains("bool IsDepthExceeded", source, StringComparison.Ordinal);
    }

    [Fact]
    public void CardPlayContextTracksPowerFallback()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "CardPlayContext.cs");

        Assert.Contains("bool IsPowerFallback", source, StringComparison.Ordinal);
        Assert.Contains("FallbackToPower", source, StringComparison.Ordinal);
    }

    // ── DeathProtectionService spec ────────────────────────────────

    [Fact]
    public void DeathProtectionSpecDocumentsReprieveLifecycle()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("ShouldDie", spec, StringComparison.Ordinal);
        Assert.Contains("AfterPreventingDeath", spec, StringComparison.Ordinal);
        Assert.Contains("DeathReprieveActive", spec, StringComparison.Ordinal);
        Assert.Contains("DeathReprievePendingStart", spec, StringComparison.Ordinal);
        Assert.Contains("inReprieve", spec, StringComparison.Ordinal);
        Assert.Contains("forced unavoidable death", spec, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeathProtectionSpecDocumentsCoopOwnerAttribution()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("co-op", spec, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("owner", spec, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ShouldSkipCoopCombat", spec, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS", spec, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionSpecDefinesFutureInterface()
    {
        var spec = TestRepo.ReadRepoText("docs", "architecture", "death-protection-spec.md");

        Assert.Contains("IDeathProtectionProvider", spec, StringComparison.Ordinal);
        Assert.Contains("ShouldPreventDeath", spec, StringComparison.Ordinal);
        Assert.Contains("IsInReprieve", spec, StringComparison.Ordinal);
        Assert.Contains("CanBypassForcedDeath", spec, StringComparison.Ordinal);
    }

    // ── MultiplayerPolicy taxonomy ─────────────────────────────────

    [Fact]
    public void MultiplayerTaxonomyDefinesAllSixCategories()
    {
        var taxonomy = TestRepo.ReadRepoText("docs", "architecture", "multiplayer-policy-taxonomy.md");

        Assert.Contains("LocalUiOnly", taxonomy, StringComparison.Ordinal);
        Assert.Contains("LocalPlayerOnly", taxonomy, StringComparison.Ordinal);
        Assert.Contains("HostAuthoritative", taxonomy, StringComparison.Ordinal);
        Assert.Contains("SharedRunState", taxonomy, StringComparison.Ordinal);
        Assert.Contains("CombatCommandReplicated", taxonomy, StringComparison.Ordinal);
        Assert.Contains("UnsafeInMultiplayer", taxonomy, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerTaxonomyDocumentsEnvOverrides()
    {
        var taxonomy = TestRepo.ReadRepoText("docs", "architecture", "multiplayer-policy-taxonomy.md");

        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS", taxonomy, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY", taxonomy, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerTaxonomyMapsExistingPolicy()
    {
        var taxonomy = TestRepo.ReadRepoText("docs", "architecture", "multiplayer-policy-taxonomy.md");

        Assert.Contains("MultiplayerFeaturePolicy", taxonomy, StringComparison.Ordinal);
        Assert.Contains("IsSingleplayer", taxonomy, StringComparison.Ordinal);
        Assert.Contains("IsHost", taxonomy, StringComparison.Ordinal);
        Assert.Contains("IsClient", taxonomy, StringComparison.Ordinal);
        Assert.Contains("CanMutateSharedRunState", taxonomy, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopFeature", taxonomy, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopGameplay", taxonomy, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopCombatHook", taxonomy, StringComparison.Ordinal);
    }
}
