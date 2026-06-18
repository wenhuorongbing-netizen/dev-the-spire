using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void VelvetChokerSoftLimitCountsOnlyManualFirstFromHandPlaysAndResetsEachTurn()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Equal("Gain 1 Energy. Each turn, the 7th and later cards played from your hand cost 1 more.", relics["VELVET_CHOKER.description"]);

        AssertSourceContains(
            source,
            "new DynamicVar[] { new CardsVar(7), new EnergyVar(1) }",
            "VelvetChokerShouldPlayPatch",
            "__result = true",
            "CardEnergyCost.GetWithModifiers",
            "modifiers.HasFlag(CostModifiers.Global)",
            "__result += VelvetChokerSoftLimitTracker.ExtraEnergyCost",
            "PlayerCombatState.HasEnoughResourcesFor",
            "UnplayableReason.EnergyCostTooHigh",
            "CardModel.SpendResources",
            "CapturedXValue = Math.Max(0, result.Item1 - VelvetChokerSoftLimitTracker.ExtraEnergyCost)",
            "!cardPlay.IsAutoPlay",
            "cardPlay.IsFirstInSeries",
            "!cardPlay.Card.IsClone",
            "cardPlay.Card.Owner == __instance.Owner",
            "card.IsClone",
            "card.Pile?.Type != PileType.Hand",
            "HandPlayedThisTurn(choker) >= FreeHandPlaysPerTurn",
            "BeforeSideTurnStart",
            "side == __instance.Owner.Creature.Side",
            "AfterRoomEntered",
            "AfterCombatEnd");

        var shouldTax = SliceBetween(
            source,
            "public static bool ShouldTax(CardModel card)",
            "private static Player? TryGetOwner");
        var tryGetOwner = SliceBetween(
            source,
            "private static Player? TryGetOwner",
            "public static void Increment");
        AssertSourceContains(
            shouldTax,
            "if (!CombatManager.Instance.IsInProgress",
            "SuppressedCostCards.Contains(card)",
            "card.Pile?.Type != PileType.Hand",
            "var owner = TryGetOwner(card)",
            "owner?.GetRelic<VelvetChoker>()");
        Assert.True(
            shouldTax.IndexOf("CombatManager.Instance.IsInProgress", StringComparison.Ordinal) <
            shouldTax.IndexOf("TryGetOwner(card)", StringComparison.Ordinal),
            "Velvet Choker cost checks must reject non-combat/card-library contexts before reading CardModel.Owner.");
        AssertSourceContains(
            tryGetOwner,
            "return card.Owner",
            "catch (MegaCrit.Sts2.Core.Models.Exceptions.CanonicalModelException)",
            "return null");
        Assert.DoesNotContain("card.Owner?.GetRelic<VelvetChoker>()", shouldTax, StringComparison.Ordinal);

        AssertSourceContains(
            turnSource,
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay)",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card))",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources)");

        Assert.Contains("| Velvet Choker |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("copied, autoplayed, or repeated plays do not advance the counter", manualMatrix, StringComparison.Ordinal);
    }
}
