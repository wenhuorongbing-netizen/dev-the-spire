using EZMicroBalance.EZMicroBalanceCode.Core.Architecture;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ArchitectureSkeletonGuardTests
{
    // CardPlayContext

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

    // CardPlayContext canary (behavioral)

    [Fact]
    public void CardPlayContext_Canary_DepthGuardBlocksAtMaxDepth()
    {
        var ctx = new CardPlayContext();

        // Increment exactly MaxDepth times - all should succeed.
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
}
