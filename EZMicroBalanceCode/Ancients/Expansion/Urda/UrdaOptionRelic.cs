namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal abstract class UrdaOptionRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;

    public override string CustomBigIconPath => CustomIconPath!;

    public override string CustomIconOutlinePath => CustomIconPath!;
}
