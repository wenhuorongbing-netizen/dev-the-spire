namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RoyalDecreeEnchantment : CustomEnchantmentModel, ILocalizationProvider
{
    protected override string? CustomIconPath => AscensionAssetPaths.BossSealIndicator;

    public override bool HasExtraCardText => true;

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "王令",
            "本回合打出这张牌，可削弱女王对火炬头聚合体的下一次力量强化。",
            "打出以削弱女王的下一次强化。")
        : new CardModifierLoc(
            "Royal Decree",
            "Play this this turn to weaken the Queen's next Strength gain for Torch Head Amalgam.",
            "Play to weaken the Queen's next buff.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}
