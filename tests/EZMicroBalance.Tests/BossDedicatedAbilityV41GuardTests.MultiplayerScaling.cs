using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class BossDedicatedAbilityV41GuardTests
{
    [Fact]
    public void MultiplayerScalingRulesAreEncodedForV41BossAbilities()
    {
        var martyr = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MartyrOath.cs");
        var inkReturn = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.InkReturn.cs");
        var startledShell = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.StartledShell.cs");
        var soulTide = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.SoulTide.cs");
        var boiling = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.BoilingCritical.cs");
        var marginalNote = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MarginalNote.cs");
        var struggleBait = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.StruggleBait.cs");
        var chosenDecree = ReadChosenDecreeCombatSources();
        var turnFlow = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.TurnFlow.cs");
        var turnLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.TurnLifecycle.cs");
        var cardEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.CardEvents.cs");
        var combatEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.CombatEvents.cs");
        var rootBudEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEvents.cs");
        var aeonglass = ReadAeonglassHourglassCombatSources();
        var residualSample = ReadResidualSampleCombatSources();
        var marginalNotePowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MarginalNotePowers.cs");
        var tracker = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatTracker.BossSeals.cs");
        var chosenDecreeAssignStart = SliceBetween(
            chosenDecree,
            "private static void TryAssignChosenDecreeInHandForPlayer",
            "var boundCards = player.Piles");

        AssertSourceContains(
            martyr,
            "const int triggerCap = 2;",
            "creature.Monster is not KinFollower",
            "metadata.IsBossBrand ? 4m : 3m",
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(priest, 1, priest, null)");
        Assert.DoesNotContain("triggerCap = metadata.IsBossBrand ? 3 : 2", martyr, StringComparison.Ordinal);

        AssertSourceContains(
            inkReturn,
            "var ratio = isBossBrand ? 0.35m : 0.25m;",
            "var minimum = isBossBrand ? 5 : 3;",
            "var maximum = isBossBrand ? 18 : 12;",
            "ApplyPowerWithFinalDisplayedGain<SlipperyPower>");

        AssertSourceContains(
            startledShell,
            "TrackStartledShellDamageStart",
            "StartledShellWakeByPlayerDamagePending",
            "result.UnblockedDamage <= 0m",
            "PowerCmd.Apply<PlatingPower>",
            "metadata.IsBossBrand ? 6 : 4",
            "metadata.IsBossBrand ? 10 : 8",
            "var divisor = metadata.IsBossBrand ? 3m : 2m");
        AssertSourceContains(
            combatEvents,
            "BeforeDamageReceived(",
            "metadata.BossSeal?.Id != BossSealId.StartledShell",
            "TrackStartledShellDamageStart(tracker, target)");
        AssertSourceContains(
            rootBudEvents,
            "public override async Task BeforeDamageReceived",
            "AscensionCombatModifierService.BeforeDamageReceived");

        AssertSourceContains(
            soulTide,
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(soulFysh, 1, soulFysh, null)",
            "TrackSoulTideBeckonsBeforePlayerTurnEnd",
            "Count it before Core runs turn-end in-hand effects",
            "metadata.IsBossBrand ? 3m : 2m",
            "combatState.Players.Count(player => player.IsActiveForHooks)",
            "return playerCount <= 1 ? 12 : playerCount == 2 ? 16 : 20;",
            "return playerCount <= 1 ? 8 : playerCount == 2 ? 12 : 16;");
        AssertSourceContains(
            rootBudEvents,
            "public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)",
            "AscensionCombatModifierService.BeforeTurnEnd(state, GetTracker(state), side, participants)");
        AssertSourceContains(
            turnLifecycle,
            "if (side == CombatSide.Player)",
            "metadata.BossSeal?.Id == BossSealId.SoulTide",
            "next player turn starts",
            "await ApplySoulTidePendingBlock(combatState, tracker, metadata);");
        var bossSealPlayerTurnStart = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealPlayerTurnStart(",
            "private static async Task ApplyBossSealSideTurnStart(");
        var bossSealEnemyTurnStart = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealSideTurnStart(",
            "private static async Task ApplyBossSealTurnEnd(");
        var bossSealTurnEnd = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealTurnEnd(",
            "// The Branded Form double-follower bonus");
        Assert.Contains("await ApplySoulTidePendingBlock(combatState, tracker, metadata);", bossSealPlayerTurnStart, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealEnemyTurnStart, StringComparison.Ordinal);
        Assert.Contains("case BossSealId.SoulTide:", bossSealTurnEnd, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealTurnEnd, StringComparison.Ordinal);

        AssertSourceContains(
            boiling,
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>",
            "artifactBefore",
            "tracker.BoilingExplosionArtifactAdded",
            "giant.GetPower<WeakPower>()",
            "PowerCmd.Remove(weak)",
            "strength is { Amount: < 0 }",
            "tracker.BoilingExplosionVulnerabilityRound == combatState.RoundNumber",
            "var vulnerable = metadata.IsBossBrand ? 2m : 1m",
            "PowerCmd.Apply<VulnerablePower>",
            "ClearBoilingExplosionFortification",
            "tracker.BoilingExplosionFortified = false",
            "Math.Min(artifact.Amount, artifactToRemove)");
        AssertSourceContains(
            tracker,
            "public bool BoilingExplosionFortified { get; set; }",
            "public int BoilingExplosionArtifactAdded { get; set; }");
        Assert.DoesNotContain("GetTypeForAmount(power.Amount) == PowerType.Debuff", boiling, StringComparison.Ordinal);

        AssertSourceContains(
            marginalNote,
            "var roundRoom = Math.Max(0, 2 - tracker.MarginalDeepThoughtAddedThisRound);",
            "ClampPowerAmount<DeepThoughtPower>(demon, metadata.IsBossBrand ? 3 : 2, demon, null)");
        var deepThought = SliceBetween(
            marginalNotePowers,
            "internal sealed class DeepThoughtPower",
            "internal sealed class DeepThoughtCostTaxPower");
        AssertSourceContains(
            deepThought,
            "private decimal GetSideCostLayers(Player player)",
            "metadata is { IsBossBrand: true, BossSeal.Id: BossSealId.MarginalNote }",
            "Math.Min(layers, 1m)",
            "PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, sideCostLayers",
            "PlayerCmd.LoseEnergy(sideCostLayers, player)");
        Assert.DoesNotContain("PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, 1m", deepThought, StringComparison.Ordinal);
        Assert.DoesNotContain("PlayerCmd.LoseEnergy(1m, player)", deepThought, StringComparison.Ordinal);

        AssertSourceContains(
            struggleBait,
            "targetPlayers.Take(1)",
            "tracker.StruggleBaitVigorGainRound == combatState.RoundNumber",
            "metadata.IsBossBrand ? 3m : 2m",
            "PowerCmd.Apply<VigorPower>");

        AssertSourceContains(
            chosenDecreeAssignStart,
            "HydrateChosenDecreeFromVisibleCards(combatState, tracker);",
            "if (metadata.BossSeal?.Id != BossSealId.ChosenDecree");
        AssertSourceContains(
            chosenDecree,
            "tracker.ChosenDecreeCardsByPlayer.Remove(player)",
            "tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player)",
            "Bound is applied as cards are drawn",
            "of always marking the first Bound card that entered hand",
            "private static bool CanMarkChosenDecree(CardModel card)",
            "card.Type is CardType.Attack or CardType.Skill or CardType.Power",
            "!card.Keywords.Contains(CardKeyword.Unplayable)",
            "ModelDb.Enchantment<RoyalDecreeEnchantment>().CanEnchant(card)",
            "catch (InvalidOperationException ex)",
            "skipped Royal Decree mark for un-enchantable Bound card",
            ".Where(CanMarkChosenDecree)",
            "var affectedPlayers = tracker.ChosenDecreeCardsByPlayer.Keys",
            "foreach (var player in affectedPlayers)",
            "ClearChosenDecreeSavedMarkers(player)",
            "tracker.ChosenDecreePlayersWhoPlayedDecree.Contains(player)",
            "tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player)",
            "tracker.ChosenDecreeMajestyGainedThisRound >= 2",
            "tracker.ChosenDecreeAmalgamStrengthThisRound < 2",
            "tracker.ChosenDecreeRoundCapRound == roundNumber",
            "ClampPowerAmount<RoyalMajestyPower>(queen, metadata.IsBossBrand ? 3 : 2, queen, null)");
        AssertSourceContains(
            turnFlow,
            "ResetChosenDecreeRoundCaps(tracker, combatState.RoundNumber)",
            "TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, player)",
            "await ClearBoilingExplosionFortification(combatState, tracker)",
            "poison, thorns, or delayed damage cannot",
            "ResetMartyrOathTurnCounters(tracker);");
        AssertSourceContains(
            cardEvents,
            "if (card.Owner is { } owner)",
            "TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, owner);");
        Assert.DoesNotContain("TryAssignChosenDecree(combatState, tracker, metadata, card);", cardEvents, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAssignChosenDecreeInHands(combatState, tracker, metadata);", chosenDecree, StringComparison.Ordinal);

        AssertSourceContains(
            aeonglass,
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2;",
            "await ArmAeonglassLaserEchoPreviewIfEligible(combatState, tracker, metadata);",
            "tracker.AeonglassTimeSand -= spent;",
            "if (tracker.AeonglassTimeSand <= 0)",
            "PowerCmd.Remove(timeSand)",
            "PowerCmd.Remove(aeonglass.GetPower<AeonglassLaserEchoPower>())",
            "The extra hit changes damage",
            "tracker.AeonglassLaserEchoesUsed < 2",
            "var pendingWither = tracker.AeonglassTimeSand",
            "tracker.AeonglassExtraWitherFromSands += pendingWither",
            "PowerCmd.Apply<AeonglassPendingWitherPower>",
            "PowerCmd.Apply<AeonglassLaserEchoUseCounterPower>",
            "CardPileCmd.AddToCombatAndPreview<Wither>");

        AssertSourceContains(
            residualSample,
            "PlayResidualSampleNotice(subject, samples);",
            "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE",
            "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason",
            "TalkCmd.Play(line, subject, VfxColor.Purple, VfxDuration.Long);");
    }
}
