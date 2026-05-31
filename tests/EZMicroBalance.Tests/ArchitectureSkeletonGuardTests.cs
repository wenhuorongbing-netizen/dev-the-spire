using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
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

    // ── RewardPipeline canary (behavioral) ─────────────────────────

    [Fact]
    public void RewardPipeline_Canary_DiagnosticsHandlerCapturesPhaseFlow()
    {
        // Arrange: register a capturing handler, run Diagnose through a phase flow,
        // verify the handler recorded all invocations without mutating reward state.
        RewardPipeline.ClearHandlers();
        try
        {
            var captured = new List<(RewardPhase Phase, string EventName)>();
            var handler = new CapturingRewardHandler(RewardPhase.PreGeneration, 0, captured);
            RewardPipeline.Register(handler);

            // Simulate a reward phase flow: PreGeneration → CardOptions → RoomRewards
            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "CanaryTest",
                EventName = "RewardFlowStart",
                Data = new Dictionary<string, object?> { ["phase"] = "PreGeneration" }
            });

            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "CanaryTest",
                EventName = "CardOptionsReady",
                Data = new Dictionary<string, object?> { ["phase"] = "CardOptions" }
            });

            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "CanaryTest",
                EventName = "RoomRewardsReady",
                Data = new Dictionary<string, object?> { ["phase"] = "RoomRewards" }
            });

            Assert.Equal(3, captured.Count);
            Assert.Equal(RewardPhase.PreGeneration, captured[0].Phase);
            Assert.Equal("RewardFlowStart", captured[0].EventName);
            Assert.Equal(RewardPhase.PreGeneration, captured[1].Phase);
            Assert.Equal("CardOptionsReady", captured[1].EventName);
            Assert.Equal(RewardPhase.PreGeneration, captured[2].Phase);
            Assert.Equal("RoomRewardsReady", captured[2].EventName);
        }
        finally
        {
            RewardPipeline.ClearHandlers();
        }
    }

    [Fact]
    public void RewardPipeline_Canary_SortsHandlersByPhaseThenPriority()
    {
        // Register handlers out of order, verify Diagnose invokes them sorted.
        RewardPipeline.ClearHandlers();
        try
        {
            var order = new List<string>();
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.RoomRewards, 20, order, "room-high"));
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.PreGeneration, 0, order, "pre-first"));
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.CardOptions, 10, order, "card-mid"));
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.PreGeneration, 5, order, "pre-second"));

            RewardPipeline.Diagnose(new RewardPipelineContext { Feature = "CanaryTest", EventName = "SortTest" });

            Assert.Equal(4, order.Count);
            Assert.Equal("pre-first", order[0]);    // PreGeneration, Priority=0
            Assert.Equal("pre-second", order[1]);   // PreGeneration, Priority=5
            Assert.Equal("card-mid", order[2]);     // CardOptions, Priority=10
            Assert.Equal("room-high", order[3]);    // RoomRewards, Priority=20
        }
        finally
        {
            RewardPipeline.ClearHandlers();
        }
    }

    [Fact]
    public void RewardPipeline_Canary_ClearHandlersIsolatesState()
    {
        RewardPipeline.ClearHandlers();
        try
        {
            Assert.Equal(0, RewardPipeline.HandlerCount);

            RewardPipeline.Register(new CapturingRewardHandler(RewardPhase.Finalized, 0, []));
            Assert.Equal(1, RewardPipeline.HandlerCount);

            RewardPipeline.ClearHandlers();
            Assert.Equal(0, RewardPipeline.HandlerCount);
            Assert.Empty(RewardPipeline.RegisteredPhases);
        }
        finally
        {
            RewardPipeline.ClearHandlers();
        }
    }

    [Fact]
    public void RewardPipeline_Canary_DiagnoseWithEmptyPipelineIsNoOp()
    {
        // Diagnose on an empty pipeline should complete without exception.
        RewardPipeline.ClearHandlers();
        try
        {
            RewardPipeline.Diagnose(new RewardPipelineContext
            {
                Feature = "CanaryTest",
                EventName = "EmptyPipeline"
            });

            Assert.Equal(0, RewardPipeline.HandlerCount);
        }
        finally
        {
            RewardPipeline.ClearHandlers();
        }
    }

    [Fact]
    public void RewardPipeline_Canary_RegisteredPhasesReflectsHandlerPhases()
    {
        RewardPipeline.ClearHandlers();
        try
        {
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.CardOptions, 0, [], "a"));
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.RoomRewards, 0, [], "b"));
            RewardPipeline.Register(new OrderTrackingHandler(RewardPhase.CardOptions, 5, [], "c"));

            var phases = RewardPipeline.RegisteredPhases;
            Assert.Equal(3, phases.Count);
            // Sorted: CardOptions(0), CardOptions(5), RoomRewards(0)
            Assert.Equal(RewardPhase.CardOptions, phases[0]);
            Assert.Equal(RewardPhase.CardOptions, phases[1]);
            Assert.Equal(RewardPhase.RoomRewards, phases[2]);
        }
        finally
        {
            RewardPipeline.ClearHandlers();
        }
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

    [Fact]
    public void CardPlayContextCanaryAdapterIsWiredIntoLothaExtraPlay()
    {
        var contextSource = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "CardPlayContext.cs");
        var diagnosticsSource = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "CardPlayContextCanary.Diagnostics.cs");
        var lothaSource = TestRepo.ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardPlayCount.cs");

        Assert.Contains("static partial class CardPlayContextCanary", contextSource, StringComparison.Ordinal);
        Assert.Contains("EvaluateSingleExtraPlay", contextSource, StringComparison.Ordinal);
        Assert.Contains("context.TryIncrementDepth()", contextSource, StringComparison.Ordinal);
        Assert.Contains("LogSingleExtraPlay", contextSource, StringComparison.Ordinal);
        Assert.Contains("ReleaseEvidenceLog.Log", diagnosticsSource, StringComparison.Ordinal);
        Assert.Contains("single_extra_play_evaluated", diagnosticsSource, StringComparison.Ordinal);

        Assert.Contains("CardPlayContextCanary.EvaluateSingleExtraPlay(\"Lotha\", \"mirror_rebuttal\")", lothaSource, StringComparison.Ordinal);
        Assert.Contains("CardPlayContextCanary.EvaluateSingleExtraPlay(\"Lotha\", \"mirror_hall_echo\")", lothaSource, StringComparison.Ordinal);
        Assert.Contains("CardPlayContextCanary.EvaluateSingleExtraPlay(\"Lotha\", \"deferred_verdict\")", lothaSource, StringComparison.Ordinal);
        Assert.Contains("CardPlayContextCanary.EvaluateSingleExtraPlay(\"Lotha\", \"single_sentence\")", lothaSource, StringComparison.Ordinal);
    }

    // ── CardPlayContext canary (behavioral) ────────────────────────

    [Fact]
    public void CardPlayContext_Canary_DepthGuardBlocksAtMaxDepth()
    {
        var ctx = new CardPlayContext();

        // Increment exactly MaxDepth times — all should succeed.
        for (var i = 0; i < CardPlayContext.MaxDepth; i++)
        {
            Assert.True(ctx.TryIncrementDepth(), $"Increment {i} should succeed (MaxDepth={CardPlayContext.MaxDepth}).");
        }

        Assert.Equal(CardPlayContext.MaxDepth, ctx.CurrentDepth);

        // At exactly MaxDepth, the guard blocks further increments but
        // IsDepthExceeded is false (it requires _currentDepth > MaxDepth).
        Assert.False(ctx.TryIncrementDepth(), "Increment at MaxDepth should be blocked.");
        Assert.False(ctx.IsDepthExceeded, "IsDepthExceeded is false when _currentDepth == MaxDepth (boundary, not exceeded).");
    }

    [Fact]
    public void CardPlayContext_Canary_ExtraPlayPolicyDefaultIsAllow()
    {
        var ctx = new CardPlayContext();

        Assert.Equal(ExtraPlayPolicy.Allow, ctx.Policy);
    }

    [Fact]
    public void CardPlayContext_Canary_ExtraPlayPolicyAllValuesAreDistinct()
    {
        // Verify all three policy enum values are distinct and cover the expected semantics.
        var values = Enum.GetValues<ExtraPlayPolicy>();

        Assert.Contains(ExtraPlayPolicy.Allow, values);
        Assert.Contains(ExtraPlayPolicy.Block, values);
        Assert.Contains(ExtraPlayPolicy.FallbackToPower, values);
        Assert.Equal(3, values.Length);
    }

    [Fact]
    public void CardPlayContext_Canary_PowerFallbackTracking()
    {
        var ctx = new CardPlayContext();

        Assert.False(ctx.IsPowerFallback);

        ctx.IsPowerFallback = true;
        Assert.True(ctx.IsPowerFallback);

        ctx.IsPowerFallback = false;
        Assert.False(ctx.IsPowerFallback);
    }

    [Fact]
    public void CardPlayContext_Canary_ResetClearsAllState()
    {
        var ctx = new CardPlayContext();

        // Mutate all state.
        ctx.TryIncrementDepth();
        ctx.TryIncrementDepth();
        ctx.Policy = ExtraPlayPolicy.FallbackToPower;
        ctx.IsPowerFallback = true;

        Assert.Equal(2, ctx.CurrentDepth);
        Assert.Equal(ExtraPlayPolicy.FallbackToPower, ctx.Policy);
        Assert.True(ctx.IsPowerFallback);

        // Reset should restore defaults.
        ctx.Reset();

        Assert.Equal(0, ctx.CurrentDepth);
        Assert.Equal(ExtraPlayPolicy.Allow, ctx.Policy);
        Assert.False(ctx.IsPowerFallback);
    }

    [Fact]
    public void CardPlayContext_Canary_DecrementDepthDoesNotGoBelowZero()
    {
        var ctx = new CardPlayContext();

        Assert.Equal(0, ctx.CurrentDepth);

        // Decrementing at zero should be a no-op (clamped).
        ctx.DecrementDepth();
        Assert.Equal(0, ctx.CurrentDepth);
    }

    [Fact]
    public void CardPlayContext_Canary_IncrementDecrementRoundTrips()
    {
        var ctx = new CardPlayContext();

        Assert.True(ctx.TryIncrementDepth());
        Assert.True(ctx.TryIncrementDepth());
        Assert.Equal(2, ctx.CurrentDepth);

        ctx.DecrementDepth();
        Assert.Equal(1, ctx.CurrentDepth);

        ctx.DecrementDepth();
        Assert.Equal(0, ctx.CurrentDepth);

        // Can increment again after decrementing back to zero.
        Assert.True(ctx.TryIncrementDepth());
        Assert.Equal(1, ctx.CurrentDepth);
    }

    [Fact]
    public void CardPlayContext_Canary_IsDepthExceededFalseAtMaxDepth()
    {
        // At exactly MaxDepth, IsDepthExceeded should be false (not exceeded, only beyond).
        var ctx = new CardPlayContext();

        for (var i = 0; i < CardPlayContext.MaxDepth; i++)
        {
            ctx.TryIncrementDepth();
        }

        Assert.Equal(CardPlayContext.MaxDepth, ctx.CurrentDepth);
        // _currentDepth > MaxDepth is false when _currentDepth == MaxDepth
        Assert.False(ctx.IsDepthExceeded);
    }

    [Fact]
    public void CardPlayContext_Canary_MaxDepthConstantIsTen()
    {
        // Guard: MaxDepth must be exactly 10 for the depth guard contract.
        Assert.Equal(10, CardPlayContext.MaxDepth);
    }

    [Fact]
    public void CardPlayContext_Canary_AdapterAllowsSingleExtraPlay()
    {
        var policy = CardPlayContextCanary.EvaluateSingleExtraPlay("CanaryTest", "unit_test");

        Assert.Equal(ExtraPlayPolicy.Allow, policy);
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

    // ── DeathProtectionService stub ──────────────────────────────────

    [Fact]
    public void DeathProtectionServiceStubExists()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("DeathProtectionService", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesRequestRecord()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("record DeathProtectionRequest", source, StringComparison.Ordinal);
        Assert.Contains("string Player", source, StringComparison.Ordinal);
        Assert.Contains("string Source", source, StringComparison.Ordinal);
        Assert.Contains("int Damage", source, StringComparison.Ordinal);
        Assert.Contains("bool IsUnavoidable", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesResultEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("enum DeathProtectionResult", source, StringComparison.Ordinal);
        Assert.Contains("Protected", source, StringComparison.Ordinal);
        Assert.Contains("NotProtected", source, StringComparison.Ordinal);
        Assert.Contains("ForcedDeath", source, StringComparison.Ordinal);
        Assert.Contains("record DeathProtectionCheck", source, StringComparison.Ordinal);
        Assert.Contains("IDeathProtectionProvider? Provider", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesPriorityEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("enum DeathProtectionPriority", source, StringComparison.Ordinal);
        Assert.Contains("Reprieve = 100", source, StringComparison.Ordinal);
        Assert.Contains("Sacrifice = 200", source, StringComparison.Ordinal);
        Assert.Contains("LastStand = 300", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceDefinesProviderInterface()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("interface IDeathProtectionProvider", source, StringComparison.Ordinal);
        Assert.Contains("bool CanProtect(DeathProtectionRequest", source, StringComparison.Ordinal);
        Assert.Contains("DeathProtectionPriority Priority", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceIsDiagnosticsOnly()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("Diagnostics-only", source, StringComparison.Ordinal);
        Assert.Contains("Not wired into game logic", source, StringComparison.Ordinal);
        Assert.Contains("CheckProtection", source, StringComparison.Ordinal);
        Assert.Contains("CheckProtectionDetailed", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceSortsProvidersByPriority()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("a.Priority.CompareTo(b.Priority)", source, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathProtectionServiceHandlesUnavoidableDeath()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "DeathProtectionService.cs");

        Assert.Contains("request.IsUnavoidable", source, StringComparison.Ordinal);
        Assert.Contains("DeathProtectionResult.ForcedDeath", source, StringComparison.Ordinal);
    }

    // ── MultiplayerPolicyRegistry stub ───────────────────────────────

    [Fact]
    public void MultiplayerPolicyRegistryStubExists()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

        Assert.Contains("MultiplayerPolicyRegistry", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MultiplayerPolicyDefinesFeatureCategoryEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

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
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "MultiplayerPolicy.cs");

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

    // ── DeathProtectionService canary (behavioral) ─────────────────

    [Fact]
    public void DeathProtectionService_Canary_RegisterIncrementsProviderCount()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            Assert.Equal(0, DeathProtectionService.ProviderCount);

            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            Assert.Equal(1, DeathProtectionService.ProviderCount);

            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: false));
            Assert.Equal(2, DeathProtectionService.ProviderCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_ProvidersSortedByPriority()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register out of priority order: LastStand first, then Reprieve, then Sacrifice.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.LastStand, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true));

            var priorities = DeathProtectionService.RegisteredPriorities;
            Assert.Equal(3, priorities.Count);
            Assert.Equal(DeathProtectionPriority.Reprieve, priorities[0]);   // 100
            Assert.Equal(DeathProtectionPriority.Sacrifice, priorities[1]);  // 200
            Assert.Equal(DeathProtectionPriority.LastStand, priorities[2]);  // 300
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_UnavoidableDeathReturnsForcedDeath()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register a provider that says it can protect — but unavoidable death should still force death.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));

            var request = new DeathProtectionRequest("Player1", "TestSource", 999, IsUnavoidable: true);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.ForcedDeath, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_UnavoidableDeathDoesNotQueryProviders()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var provider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true);
            DeathProtectionService.Register(provider);

            var request = new DeathProtectionRequest("Player1", "TestSource", 999, IsUnavoidable: true);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.ForcedDeath, result.Result);
            Assert.Null(result.Provider);
            Assert.Equal(0, provider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_NoProvidersReturnsNotProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.NotProtected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_MatchingProviderReturnsProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.Protected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_FirstMatchingProviderWins()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Register two providers: Reprieve (priority 100) and Sacrifice (priority 200).
            // Both can protect — but Reprieve is checked first due to lower priority.
            var reprieveProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true);
            var sacrificeProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true);

            DeathProtectionService.Register(sacrificeProvider);  // Register higher priority first
            DeathProtectionService.Register(reprieveProvider);   // Register lower priority second

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.Protected, result.Result);
            Assert.Same(reprieveProvider, result.Provider);
            Assert.Equal(1, reprieveProvider.CallCount);
            Assert.Equal(0, sacrificeProvider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_DetailedCheckReturnsMatchingProvider()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            var nonMatchingProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: false);
            var matchingProvider = new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: true);
            DeathProtectionService.Register(matchingProvider);
            DeathProtectionService.Register(nonMatchingProvider);

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtectionDetailed(request);

            Assert.Equal(DeathProtectionResult.Protected, result.Result);
            Assert.Same(matchingProvider, result.Provider);
            Assert.Equal(1, nonMatchingProvider.CallCount);
            Assert.Equal(1, matchingProvider.CallCount);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_ClearProvidersResetsState()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: true));
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Sacrifice, canProtect: false));

            Assert.Equal(2, DeathProtectionService.ProviderCount);

            DeathProtectionService.ClearProviders();
            Assert.Equal(0, DeathProtectionService.ProviderCount);
            Assert.Empty(DeathProtectionService.RegisteredPriorities);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    [Fact]
    public void DeathProtectionService_Canary_CheckProtectionWithNonMatchingProviderReturnsNotProtected()
    {
        DeathProtectionService.ClearProviders();
        try
        {
            // Provider says it cannot protect — should return NotProtected.
            DeathProtectionService.Register(new StubDeathProtectionProvider(DeathProtectionPriority.Reprieve, canProtect: false));

            var request = new DeathProtectionRequest("Player1", "TestSource", 10, IsUnavoidable: false);
            var result = DeathProtectionService.CheckProtection(request);

            Assert.Equal(DeathProtectionResult.NotProtected, result);
        }
        finally
        {
            DeathProtectionService.ClearProviders();
        }
    }

    // ── MultiplayerPolicyRegistry canary (behavioral) ─────────────

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

    // ── Test helpers for RewardPipeline canary tests ──────────────────

    /// <summary>
    /// Test handler that records every invocation for later assertion.
    /// </summary>
    private sealed class CapturingRewardHandler : IRewardHandler
    {
        private readonly List<(RewardPhase Phase, string EventName)> _captured;

        public RewardPhase Phase { get; }
        public int Priority { get; }

        public CapturingRewardHandler(RewardPhase phase, int priority, List<(RewardPhase, string)> captured)
        {
            Phase = phase;
            Priority = priority;
            _captured = captured;
        }

        public void Handle(RewardPipelineContext context)
        {
            _captured.Add((Phase, context.EventName));
        }
    }

    /// <summary>
    /// Test handler that appends its label to an order list for sort verification.
    /// </summary>
    private sealed class OrderTrackingHandler : IRewardHandler
    {
        private readonly List<string> _order;
        private readonly string _label;

        public RewardPhase Phase { get; }
        public int Priority { get; }

        public OrderTrackingHandler(RewardPhase phase, int priority, List<string> order, string label)
        {
            Phase = phase;
            Priority = priority;
            _order = order;
            _label = label;
        }

        public void Handle(RewardPipelineContext context)
        {
            _order.Add(_label);
        }
    }

    /// <summary>
    /// Test provider for DeathProtectionService behavioral canary tests.
    /// </summary>
    private sealed class StubDeathProtectionProvider : IDeathProtectionProvider
    {
        private readonly bool _canProtect;

        public DeathProtectionPriority Priority { get; }
        public int CallCount { get; private set; }

        public StubDeathProtectionProvider(DeathProtectionPriority priority, bool canProtect)
        {
            Priority = priority;
            _canProtect = canProtect;
        }

        public bool CanProtect(DeathProtectionRequest request)
        {
            CallCount++;
            return _canProtect;
        }
    }
}
