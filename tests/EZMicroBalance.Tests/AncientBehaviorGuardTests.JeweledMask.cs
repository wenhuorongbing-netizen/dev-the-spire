using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void JeweledMaskCustomEnchantmentIsPowerOnlyPersistentAndCombatStartScoped()
    {
        var enchantment = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var combatSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            enchantment,
            "CustomEnchantmentModel, ILocalizationProvider",
            "public override bool HasExtraCardText => true",
            "return cardType == CardType.Power",
            "Card.EnergyCost.SetCustomBaseCost(0)");

        AssertSourceContains(
            pickupSource,
            "card => card.Type == CardType.Power && card.Enchantment == null",
            "DraftGeneratedPowerForJeweledMask(owner)",
            "owner.RunState.RemoveCard(unselected)",
            "await CardPileCmd.Add(selected, PileType.Deck)",
            "CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m)");

        AssertSourceContains(
            combatSource,
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "combatState.RoundNumber > 1",
            "AncientCardHelpers.IsJeweledMaskPower",
            "await CardPileCmd.Add(markedPower, PileType.Hand)",
            "marked power already in hand",
            "no marked power in draw pile or hand");

        Assert.Contains("Jeweled Mask free power", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Permanent 0-cost enchantment survives save/load.", manualMatrix, StringComparison.Ordinal);
    }
}
