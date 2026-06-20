namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using MegaCrit.Sts2.Core.Models.RelicPools;

internal sealed class ForgeTokenRelic : ModRelicTemplate, ILocalizationProvider
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowedInShops => false;

    public override bool ShowCounter => true;

    public override int DisplayAmount => 1;

    public override string CustomIconPath => AscensionAssetPaths.ForgeTokenStatus;

    public override string CustomIconOutlinePath => AscensionAssetPaths.ForgeTokenStatus;

    public override string CustomBigIconPath => AscensionAssetPaths.ForgeTokenStatus;

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new RelicLoc(
            "铸令",
            "击败[gold]火印精英[/gold]后获得。下一个[gold]休息处[/gold]：若选择[gold]休息[/gold]，随机升级[blue]1[/blue]张可升级的[gold]普通[/gold]或[gold]罕见[/gold]牌；若选择[gold]锻造[/gold]，回复[blue]7[/blue]点生命。休息没有可升级目标时，改为回复[blue]5[/blue]点生命。只有[gold]休息[/gold]或[gold]锻造[/gold]会消耗铸令。最多持有[blue]1[/blue]枚。",
            "下一个[gold]休息处[/gold]可用[blue]1[/blue]次。",
            ("additionalRestSiteHealText", "[gold]铸令[/gold]：[gold]休息[/gold]会随机升级[blue]1[/blue]张可升级的[gold]普通[/gold]或[gold]罕见[/gold]牌；若没有目标，回复[blue]5[/blue]点生命。"))
        : new RelicLoc(
            "Forge Token",
            "Gained from a [gold]Firemarked Elite[/gold]. At your next [gold]Rest Site[/gold], [gold]Rest[/gold] randomly upgrades [blue]1[/blue] upgradable [gold]common[/gold] or [gold]uncommon[/gold] card; [gold]Smith[/gold] heals [blue]7[/blue] HP. If Rest has no target, heal [blue]5[/blue] HP instead. Only [gold]Rest[/gold] or [gold]Smith[/gold] spends this token. Max [blue]1[/blue].",
            "A brief command from a [gold]Firemark[/gold], good for [blue]1[/blue] use at the next fire.",
            ("additionalRestSiteHealText", "[gold]Forge Token[/gold]: [gold]Rest[/gold] randomly upgrades [blue]1[/blue] upgradable [gold]common[/gold] or [gold]uncommon[/gold] card. If there is no target, heal [blue]5[/blue] HP."));

    public LocString? GetAdditionalRestSiteHealText()
    {
        return AdditionalRestSiteHealText;
    }
}
