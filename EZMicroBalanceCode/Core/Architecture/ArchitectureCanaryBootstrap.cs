using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Architecture;

internal static class ArchitectureCanaryBootstrap
{
    private const string CoopGameplayOverride = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY";
    private const string CoopCombatOverride = "SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS";
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        RewardPipeline.Register(new FeatureBootstrapRewardDiagnosticsHandler());
        RewardPipeline.Register(new AscensionRewardDiagnosticsHandler());
        DeathProtectionService.Register(new NoOpDeathProtectionProvider());
        RegisterPolicy("Preview.CrystalSphere", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("Preview.Transform", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("PreviewCrystalSphere", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("PreviewTransform", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("Ancients.Urda", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.Morvi", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.Lotha", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.VakuuFight", MultiplayerFeatureCategory.UnsafeInMultiplayer, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ascension.A11A20", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("AscensionRewards", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("AscensionCombatHooks", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
        RegisterPolicy("CombatHooks.AncientExpansion", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
        RegisterPolicy("Sts1Events", MultiplayerFeatureCategory.SharedRunState, "SPIREPLUS_STS1_EVENT_MODE", isVerified: false);
        RegisterPolicy("UrdaAncientOffer", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("UrdaAncientSelection", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("UrdaRunHooks", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("UrdaCombatHooks", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
        RegisterPolicy("UrdaRootEyes", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("UrdaSeedBank", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("UrdaCardRewardAlternatives", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("MorviAncientOffer", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("MorviAncientSelection", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("MorviRunHooks", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("MorviCombatHooks", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
        RegisterPolicy("LothaAncientOffer", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("LothaAncientSelection", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("LothaRunHooks", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("LothaCombatHooks", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
        RegisterPolicy("VakuuFightCombatHooks", MultiplayerFeatureCategory.UnsafeInMultiplayer, CoopCombatOverride, isVerified: false);
        RegisterPolicy("AscensionA11A20Gameplay", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("AscensionMultiplayerSelection", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("A20BrandedForm", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);

        ReleaseEvidenceLog.Log(
            "ArchitectureCanary",
            "canary_bootstrap_initialized",
            data: new Dictionary<string, object?>
            {
                ["rewardHandlers"] = RewardPipeline.HandlerCount,
                ["deathProtectionProviders"] = DeathProtectionService.ProviderCount,
                ["multiplayerPolicies"] = MultiplayerPolicyRegistry.PolicyCount
            });
    }

    private static void RegisterPolicy(
        string featureId,
        MultiplayerFeatureCategory category,
        string? envOverride,
        bool isVerified)
    {
        if (MultiplayerPolicyRegistry.Lookup(featureId) != null)
        {
            return;
        }

        MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord(featureId, category, envOverride, isVerified));
    }

    private sealed class FeatureBootstrapRewardDiagnosticsHandler : IRewardHandler
    {
        public RewardPhase Phase => RewardPhase.PreGeneration;

        public int Priority => -100;

        public void Handle(RewardPipelineContext context)
        {
            if (!string.Equals(context.Feature, "FeatureRegistry", StringComparison.Ordinal))
            {
                return;
            }

            var id = context.Data.TryGetValue("id", out var value) ? value : "unknown";
            MainFile.Logger.Info($"[Spire Plus] RewardPipeline diagnostics observed {context.EventName} for {id}.");
            ReleaseEvidenceLog.Log(
                "ArchitectureCanary",
                "feature_bootstrap_observed",
                data: new Dictionary<string, object?>(context.Data)
                {
                    ["pipelineEvent"] = context.EventName
                });
        }
    }

    private sealed class AscensionRewardDiagnosticsHandler : IRewardHandler
    {
        public RewardPhase Phase => RewardPhase.CardOptions;

        public int Priority => -90;

        public void Handle(RewardPipelineContext context)
        {
            if (!string.Equals(context.Feature, "AscensionRewards", StringComparison.Ordinal))
            {
                return;
            }

            MainFile.Logger.Info($"[Spire Plus] RewardPipeline diagnostics observed {context.EventName} for AscensionRewards ({context.NetMode}).");
            ReleaseEvidenceLog.Log(
                "ArchitectureCanary",
                "ascension_reward_surface_observed",
                data: new Dictionary<string, object?>(context.Data)
                {
                    ["pipelineEvent"] = context.EventName,
                    ["netMode"] = context.NetMode
                });
        }
    }

    private sealed class NoOpDeathProtectionProvider : IDeathProtectionProvider
    {
        public DeathProtectionPriority Priority => DeathProtectionPriority.LastStand;

        public bool CanProtect(DeathProtectionRequest request) => false;
    }
}
