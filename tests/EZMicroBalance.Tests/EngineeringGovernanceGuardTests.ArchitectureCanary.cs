using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
{
    [Fact]
    public void FeatureRegistryEmitsRewardPipelineDiagnostics()
    {
        var registrySource = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");
        var canarySource = ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "ArchitectureCanaryBootstrap.cs");
        var registryFactory = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");

        AssertSourceContains(registrySource,
            "RewardPipeline.Diagnose(CreateBootstrapContext(module, gate, \"FeatureBootstrapGate\"))",
            "FeatureBootstrapInitialized",
            "FeatureBootstrapDisabled",
            "FeatureBootstrapFailed",
            "Feature = \"FeatureRegistry\"");

        AssertSourceContains(canarySource,
            "RewardPipeline.Register(new FeatureBootstrapRewardDiagnosticsHandler())",
            "canary_bootstrap_initialized",
            "feature_bootstrap_observed",
            "ascension_reward_surface_observed",
            "ReleaseEvidenceLog.Log",
            "RewardPipeline diagnostics observed",
            "string.Equals(context.Feature, \"FeatureRegistry\", StringComparison.Ordinal)");

        Assert.Contains("ArchitectureCanaryBootstrap.Initialize();", registryFactory, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyRegistersActiveFeatureRecords()
    {
        var canarySource = ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "ArchitectureCanaryBootstrap.cs");

        AssertSourceContains(canarySource,
            "RegisterPolicy(\"Preview.CrystalSphere\", MultiplayerFeatureCategory.LocalUiOnly",
            "RegisterPolicy(\"Preview.Transform\", MultiplayerFeatureCategory.LocalUiOnly",
            "RegisterPolicy(\"PreviewCrystalSphere\", MultiplayerFeatureCategory.LocalUiOnly",
            "RegisterPolicy(\"PreviewTransform\", MultiplayerFeatureCategory.LocalUiOnly",
            "RegisterPolicy(\"Ancients.Urda\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"Ancients.Morvi\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"Ancients.Lotha\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"Ancients.VakuuFight\", MultiplayerFeatureCategory.UnsafeInMultiplayer",
            "RegisterPolicy(\"Ascension.A11A20\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"AscensionRewards\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"AscensionCombatHooks\", MultiplayerFeatureCategory.CombatCommandReplicated",
            "RegisterPolicy(\"CombatHooks.AncientExpansion\", MultiplayerFeatureCategory.CombatCommandReplicated",
            "RegisterPolicy(\"Sts1Events\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaAncientOffer\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaAncientSelection\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaRunHooks\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaCombatHooks\", MultiplayerFeatureCategory.CombatCommandReplicated",
            "RegisterPolicy(\"UrdaRootEyes\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaSeedBank\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"UrdaCardRewardAlternatives\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"MorviAncientOffer\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"MorviAncientSelection\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"MorviRunHooks\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"MorviCombatHooks\", MultiplayerFeatureCategory.CombatCommandReplicated",
            "RegisterPolicy(\"LothaAncientOffer\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"LothaAncientSelection\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"LothaRunHooks\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"LothaCombatHooks\", MultiplayerFeatureCategory.CombatCommandReplicated",
            "RegisterPolicy(\"VakuuFightCombatHooks\", MultiplayerFeatureCategory.UnsafeInMultiplayer",
            "RegisterPolicy(\"AscensionA11A20Gameplay\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"AscensionMultiplayerSelection\", MultiplayerFeatureCategory.SharedRunState",
            "RegisterPolicy(\"A20BrandedForm\", MultiplayerFeatureCategory.SharedRunState",
            "SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY",
            "SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS");
    }

    [Fact]
    public void ArchitectureCanaryRegistersNoOpDeathProtectionProvider()
    {
        var canarySource = ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "ArchitectureCanaryBootstrap.cs");

        AssertSourceContains(canarySource,
            "DeathProtectionService.Register(new NoOpDeathProtectionProvider())",
            "class NoOpDeathProtectionProvider : IDeathProtectionProvider",
            "DeathProtectionPriority.LastStand",
            "CanProtect(DeathProtectionRequest request) => false");
    }

    [Fact]
    public void RewardPipelineDiagnosticsTouchAscensionRewardSurface()
    {
        var rewardSource = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "AscensionRewardService.cs");
        var canarySource = ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "ArchitectureCanaryBootstrap.cs");

        AssertSourceContains(rewardSource,
            "RewardPipeline.Diagnose(new RewardPipelineContext",
            "Feature = \"AscensionRewards\"",
            "card_options_surface_entered",
            "card_options_surface_completed",
            "room_rewards_surface_entered",
            "room_rewards_surface_completed");

        AssertSourceContains(canarySource,
            "RewardPipeline.Register(new AscensionRewardDiagnosticsHandler())",
            "string.Equals(context.Feature, \"AscensionRewards\", StringComparison.Ordinal)");
    }

    [Fact]
    public void CoopEvidenceIncludesMultiplayerPolicyMetadata()
    {
        var policySource = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerFeaturePolicy.cs");

        AssertSourceContains(policySource,
            "MultiplayerPolicyRegistry.Lookup(feature)",
            "policyRegistered",
            "policyFeatureId",
            "policyCategory",
            "policyEnvOverride",
            "policyVerified");
    }
}
