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
        RegisterPolicy("Preview.CrystalSphere", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("Preview.Transform", MultiplayerFeatureCategory.LocalUiOnly, envOverride: null, isVerified: false);
        RegisterPolicy("Ancients.Urda", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.Morvi", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.Lotha", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ancients.VakuuFight", MultiplayerFeatureCategory.UnsafeInMultiplayer, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("Ascension.A11A20", MultiplayerFeatureCategory.SharedRunState, CoopGameplayOverride, isVerified: false);
        RegisterPolicy("CombatHooks.AncientExpansion", MultiplayerFeatureCategory.CombatCommandReplicated, CoopCombatOverride, isVerified: false);
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
        }
    }
}
