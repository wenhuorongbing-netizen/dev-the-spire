using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
using Xunit;

namespace EZMicroBalance.Tests;

/// <summary>
/// Guard tests for architecture skeleton contracts.
/// Verifies RewardPipeline exists with correct shape.
/// </summary>
public sealed partial class ArchitectureSkeletonGuardTests
{
    // ── RewardPipeline ──────────────────────────────────────────────

    [Fact]
    public void RewardPipelineDefinesRewardPhaseEnum()
    {
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipelineDiagnosticsContracts.cs");

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
        var source = TestRepo.ReadRepoText("EZMicroBalanceCode", "Core", "Architecture", "RewardPipelineDiagnosticsContracts.cs");

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

}
