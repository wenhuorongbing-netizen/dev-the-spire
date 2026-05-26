namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal abstract class UrdaOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    protected override string BigIconPath => PackedIconPath;

    protected override string PackedIconOutlinePath => PackedIconPath;
}
