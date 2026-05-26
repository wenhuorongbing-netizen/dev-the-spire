namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RoyalDecreeEnchantment : CustomEnchantmentModel, ILocalizationProvider
{
    protected override string? CustomIconPath => AscensionAssetPaths.GetBossSealIndicator(BossSealId.ChosenDecree);

    public override bool HasExtraCardText => true;

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "御令",
            "本回合打出这张牌，可避免御令惩罚。",
            "打出后避免御令惩罚。")
        : new CardModifierLoc(
            "Royal Decree",
            "Play this card this turn to avoid the decree penalty.",
            "Play to avoid the decree penalty.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}
