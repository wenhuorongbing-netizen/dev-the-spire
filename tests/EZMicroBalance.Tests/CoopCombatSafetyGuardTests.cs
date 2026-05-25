using Xunit;

namespace EZMicroBalance.Tests;

public sealed class CoopCombatSafetyGuardTests
{
    [Fact]
    public void UnverifiedGameplayMutationsFailClosedInMultiplayer()
    {
        var policy = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "MultiplayerFeaturePolicy.cs");
        var ascensionGates = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureGate.Systems.cs");
        var ascensionSelection = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionSelectionPatches.cs");
        var ascensionRewards = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "AscensionRewardService.cs");
        var urdaOffer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaAct1AncientService.cs");
        var morviOffer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAct2AncientService.cs");
        var lothaOffer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAct3AncientService.cs");
        var urdaSelection = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaRewardSelectionService.cs");
        var morviSelection = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.OptionRows.cs");
        var lothaSelection = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.OptionRows.cs");
        var urdaRewards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaBlessingService.CardRewards.cs");

        Assert.Contains("SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY", policy, StringComparison.Ordinal);
        Assert.Contains("EZMB_ALLOW_UNVERIFIED_COOP_GAMEPLAY", policy, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopGameplay", policy, StringComparison.Ordinal);
        Assert.Contains("coop_gameplay_disabled", policy, StringComparison.Ordinal);
        Assert.Contains("LoggedCoopGameplayGateKeys", policy, StringComparison.Ordinal);

        Assert.Contains("IsCoopAscensionGameplayAllowed", ascensionGates, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopGameplay", ascensionGates, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopGameplay", ascensionSelection, StringComparison.Ordinal);
        Assert.Contains("ShouldDisableUnverifiedCoopGameplay", ascensionRewards, StringComparison.Ordinal);

        foreach (var source in new[]
        {
            urdaOffer,
            morviOffer,
            lothaOffer,
            urdaSelection,
            morviSelection,
            lothaSelection,
            urdaRewards
        })
        {
            Assert.Contains("ShouldDisableUnverifiedCoopGameplay", source, StringComparison.Ordinal);
        }

        Assert.Contains("coop_gameplay_disabled", urdaSelection, StringComparison.Ordinal);
        Assert.Contains("coop_gameplay_disabled", morviSelection, StringComparison.Ordinal);
        Assert.Contains("coop_gameplay_disabled", lothaSelection, StringComparison.Ordinal);
    }

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
