using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ArchitectureSkeletonGuardTests
{
    // MultiplayerPolicy taxonomy

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

    // MultiplayerPolicyRegistry stub

    [Fact]
    public void MultiplayerPolicyRegistryStubExists()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

        Assert.Contains("MultiplayerPolicyRegistry", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyDefinesFeatureCategoryEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicyDiagnosticsContracts.cs");

        Assert.Contains("enum MultiplayerFeatureCategory", source, StringComparison.Ordinal);
        Assert.Contains("LocalUiOnly", source, StringComparison.Ordinal);
        Assert.Contains("LocalPlayerOnly", source, StringComparison.Ordinal);
        Assert.Contains("HostAuthoritative", source, StringComparison.Ordinal);
        Assert.Contains("SharedRunState", source, StringComparison.Ordinal);
        Assert.Contains("CombatCommandReplicated", source, StringComparison.Ordinal);
        Assert.Contains("UnsafeInMultiplayer", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyDefinesPolicyRecord()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicyDiagnosticsContracts.cs");

        Assert.Contains("record MultiplayerPolicyRecord", source, StringComparison.Ordinal);
        Assert.Contains("string FeatureId", source, StringComparison.Ordinal);
        Assert.Contains("MultiplayerFeatureCategory Category", source, StringComparison.Ordinal);
        Assert.Contains("string? EnvOverride", source, StringComparison.Ordinal);
        Assert.Contains("bool IsVerified", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyRegistryProvidesLookup()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

        Assert.Contains("MultiplayerPolicyRecord? Lookup(string featureId)", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyRegistryIsDiagnosticsOnly()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

        Assert.Contains("Diagnostics-only", source, StringComparison.Ordinal);
        Assert.Contains("Not wired into game logic", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShouldDisable", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyRegistryProvidesCategoryQuery()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

        Assert.Contains("FeaturesInCategory", source, StringComparison.Ordinal);
        Assert.Contains("MultiplayerFeatureCategory category", source, StringComparison.Ordinal);
    }

    // MultiplayerPolicyRegistry canary (behavioral)

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_RegisterIncrementsPolicyCount()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            Assert.Equal(0, MultiplayerPolicyRegistry.PolicyCount);

            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("FeatureA", MultiplayerFeatureCategory.LocalUiOnly, null, true));
            Assert.Equal(1, MultiplayerPolicyRegistry.PolicyCount);

            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("FeatureB", MultiplayerFeatureCategory.LocalPlayerOnly, null, false));
            Assert.Equal(2, MultiplayerPolicyRegistry.PolicyCount);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_LookupReturnsRegisteredPolicy()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            var policy = new MultiplayerPolicyRecord("TestFeature", MultiplayerFeatureCategory.HostAuthoritative, "ENV_KEY", true);
            MultiplayerPolicyRegistry.Register(policy);

            var result = MultiplayerPolicyRegistry.Lookup("TestFeature");
            Assert.NotNull(result);
            Assert.Equal("TestFeature", result.FeatureId);
            Assert.Equal(MultiplayerFeatureCategory.HostAuthoritative, result.Category);
            Assert.Equal("ENV_KEY", result.EnvOverride);
            Assert.True(result.IsVerified);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_LookupReturnsNullForUnknownFeature()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("KnownFeature", MultiplayerFeatureCategory.LocalUiOnly, null, true));

            var result = MultiplayerPolicyRegistry.Lookup("UnknownFeature");
            Assert.Null(result);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_AllPoliciesReturnsAllRegistered()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("A", MultiplayerFeatureCategory.LocalUiOnly, null, true));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("B", MultiplayerFeatureCategory.SharedRunState, null, false));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("C", MultiplayerFeatureCategory.UnsafeInMultiplayer, null, false));

            var all = MultiplayerPolicyRegistry.AllPolicies;
            Assert.Equal(3, all.Count);
            Assert.Contains(all, p => p.FeatureId == "A");
            Assert.Contains(all, p => p.FeatureId == "B");
            Assert.Contains(all, p => p.FeatureId == "C");
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_FeaturesInCategoryFiltersCorrectly()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("UI1", MultiplayerFeatureCategory.LocalUiOnly, null, true));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("UI2", MultiplayerFeatureCategory.LocalUiOnly, null, true));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("Combat1", MultiplayerFeatureCategory.CombatCommandReplicated, null, false));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("Shared1", MultiplayerFeatureCategory.SharedRunState, null, false));

            var uiFeatures = MultiplayerPolicyRegistry.FeaturesInCategory(MultiplayerFeatureCategory.LocalUiOnly);
            Assert.Equal(2, uiFeatures.Count);
            Assert.Contains("UI1", uiFeatures);
            Assert.Contains("UI2", uiFeatures);

            var combatFeatures = MultiplayerPolicyRegistry.FeaturesInCategory(MultiplayerFeatureCategory.CombatCommandReplicated);
            Assert.Single(combatFeatures);
            Assert.Contains("Combat1", combatFeatures);

            var unsafeFeatures = MultiplayerPolicyRegistry.FeaturesInCategory(MultiplayerFeatureCategory.UnsafeInMultiplayer);
            Assert.Empty(unsafeFeatures);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_ClearPoliciesResetsState()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("A", MultiplayerFeatureCategory.LocalUiOnly, null, true));
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("B", MultiplayerFeatureCategory.LocalPlayerOnly, null, false));

            Assert.Equal(2, MultiplayerPolicyRegistry.PolicyCount);

            MultiplayerPolicyRegistry.ClearPolicies();
            Assert.Equal(0, MultiplayerPolicyRegistry.PolicyCount);
            Assert.Empty(MultiplayerPolicyRegistry.AllPolicies);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_LookupIsCaseSensitive()
    {
        MultiplayerPolicyRegistry.ClearPolicies();
        try
        {
            MultiplayerPolicyRegistry.Register(new MultiplayerPolicyRecord("FeatureA", MultiplayerFeatureCategory.LocalUiOnly, null, true));

            // Different case should not match (Ordinal comparison).
            var result = MultiplayerPolicyRegistry.Lookup("featurea");
            Assert.Null(result);
        }
        finally
        {
            MultiplayerPolicyRegistry.ClearPolicies();
        }
    }

    [Fact]
    public void MultiplayerPolicyRegistry_Canary_AllSixCategoriesAreDistinct()
    {
        var values = Enum.GetValues<MultiplayerFeatureCategory>();

        Assert.Equal(6, values.Length);
        Assert.Contains(MultiplayerFeatureCategory.LocalUiOnly, values);
        Assert.Contains(MultiplayerFeatureCategory.LocalPlayerOnly, values);
        Assert.Contains(MultiplayerFeatureCategory.HostAuthoritative, values);
        Assert.Contains(MultiplayerFeatureCategory.SharedRunState, values);
        Assert.Contains(MultiplayerFeatureCategory.CombatCommandReplicated, values);
        Assert.Contains(MultiplayerFeatureCategory.UnsafeInMultiplayer, values);
    }
}
