namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using MegaCrit.Sts2.Core.Models.RelicPools;

[Pool(typeof(SharedRelicPool))]
internal sealed class ForgeTokenRelic : CustomRelicModel, ILocalizationProvider
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowedInShops => false;

    public override bool ShowCounter => true;

    public override int DisplayAmount => 1;

    public override string PackedIconPath => AscensionAssetPaths.ForgeTokenStatus;

    protected override string PackedIconOutlinePath => AscensionAssetPaths.ForgeTokenStatus;

    protected override string BigIconPath => AscensionAssetPaths.ForgeTokenStatus;

    public override List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new RelicLoc(
            "铸令",
            "击败[gold]火印精英[/gold]后获得。下一个[gold]休息点[/gold]：[gold]休息[/gold]会随机升级[blue]1[/blue]张可升级的[gold]普通[/gold]或[gold]罕见[/gold]牌；[gold]锻造[/gold]会回复[blue]7[/blue]点生命。若休息时没有目标，改为回复[blue]5[/blue]点生命。最多持有[blue]1[/blue]枚。",
            "[gold]火印[/gold]留下的短暂号令，只能在下一个营火使用[blue]1[/blue]次。",
            ("additionalRestSiteHealText", "[gold]铸令[/gold]：[gold]休息[/gold]会随机升级[blue]1[/blue]张可升级的[gold]普通[/gold]或[gold]罕见[/gold]牌；若没有目标，回复[blue]5[/blue]点生命。"))
        : new RelicLoc(
            "Forge Token",
            "Gained from a [gold]Firemarked Elite[/gold]. At your next [gold]Rest Site[/gold], [gold]Rest[/gold] randomly upgrades [blue]1[/blue] upgradable [gold]common[/gold] or [gold]uncommon[/gold] card; [gold]Smith[/gold] heals [blue]7[/blue] HP. If Rest has no target, heal [blue]5[/blue] HP instead. Max [blue]1[/blue].",
            "A brief command from a [gold]Firemark[/gold], good for [blue]1[/blue] use at the next fire.",
            ("additionalRestSiteHealText", "[gold]Forge Token[/gold]: [gold]Rest[/gold] randomly upgrades [blue]1[/blue] upgradable [gold]common[/gold] or [gold]uncommon[/gold] card. If there is no target, heal [blue]5[/blue] HP."));

    public LocString? GetAdditionalRestSiteHealText()
    {
        return AdditionalRestSiteHealText;
    }
}
