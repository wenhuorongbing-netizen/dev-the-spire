namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RoyalDecreeEnchantment : CustomEnchantmentModel, ILocalizationProvider
{
    protected override string? CustomIconPath => AscensionAssetPaths.BossSealIndicator;

    public override bool HasExtraCardText => true;

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "王令",
            "本回合打出这张牌，可削弱[gold]女王[/gold]下一次给火炬头聚合体的[gold]力量[/gold]强化。",
            "打出后，女王下一次[gold]力量[/gold]强化-[blue]1[/blue]。")
        : new CardModifierLoc(
            "Royal Decree",
            "Play this this turn to weaken the Queen's next Strength gain for Torch Head Amalgam.",
            "Play to weaken the Queen's next buff.");

    public override bool CanEnchantCardType(CardType cardType)
    {
        return true;
    }
}
