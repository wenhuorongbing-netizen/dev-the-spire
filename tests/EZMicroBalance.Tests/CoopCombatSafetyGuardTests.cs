using Xunit;

namespace EZMicroBalance.Tests;

public sealed class CoopCombatSafetyGuardTests
{
    [Fact]
    public void UnverifiedCombatHooksFailClosedInMultiplayer()
    {
        var policy = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerFeaturePolicy.cs");
        var ascensionInitializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        var morviInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var lothaInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaInitializer.cs");
        var urdaInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaInitializer.cs");
        var vakuuInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightRunHook.cs");

        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS", policy, StringComparison.Ordinal);
        Assert.Contains("EZMB_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS", policy, StringComparison.Ordinal);
        Assert.Contains("coop_combat_hook_disabled", policy, StringComparison.Ordinal);
        Assert.Contains("MainFile.Logger.Warn(message)", policy, StringComparison.Ordinal);
        Assert.Contains("LoggedCoopCombatGateKeys", policy, StringComparison.Ordinal);

        foreach (var source in new[]
        {
            ascensionInitializer,
            morviInitializer,
            lothaInitializer,
            urdaInitializer,
            vakuuInitializer
        })
        {
            Assert.Contains("ShouldDisableUnverifiedCoopCombatHook", source, StringComparison.Ordinal);
        }

        Assert.Contains("return Array.Empty<AbstractModel>()", ascensionInitializer, StringComparison.Ordinal);
        Assert.Contains("return []", morviInitializer, StringComparison.Ordinal);
        Assert.Contains("return []", lothaInitializer, StringComparison.Ordinal);
        Assert.Contains("return []", urdaInitializer, StringComparison.Ordinal);
        Assert.Contains("return []", vakuuInitializer, StringComparison.Ordinal);
    }

    [Fact]
    public void AncientRunHooksDoNotBypassTheCoopCombatGate()
    {
        var morviHooks = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviHooks.cs");
        var lothaHooks = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaHooks.cs");
        var urdaHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRunHook.cs");

        Assert.Contains("ShouldSkipCoopCombat(CurrentRunState())", morviHooks, StringComparison.Ordinal);
        Assert.Contains("ShouldSkipCoopCombat(room.CombatState?.RunState)", morviHooks, StringComparison.Ordinal);
        Assert.Contains("card.Owner?.Creature.CombatState != null && ShouldSkipCoopCombat(card.Owner.RunState)", morviHooks, StringComparison.Ordinal);

        Assert.Contains("ShouldSkipCoopCombat(CurrentRunState())", lothaHooks, StringComparison.Ordinal);
        Assert.Contains("ShouldSkipCoopCombat(creature.CombatState?.RunState)", lothaHooks, StringComparison.Ordinal);
        Assert.Contains("ShouldSkipCoopCombat(target.CombatState?.RunState)", lothaHooks, StringComparison.Ordinal);

        Assert.Contains("card.Owner?.Creature.CombatState != null && ShouldSkipCoopCombat(card.Owner.RunState)", urdaHook, StringComparison.Ordinal);
        Assert.Contains("ShouldSkipCoopCombat(room.CombatState?.RunState)", urdaHook, StringComparison.Ordinal);
        Assert.Contains("UrdaRunHook.ShouldSkipCoopCombat(cardPlay.Card.Owner?.RunState)", urdaHook, StringComparison.Ordinal);
    }
}
