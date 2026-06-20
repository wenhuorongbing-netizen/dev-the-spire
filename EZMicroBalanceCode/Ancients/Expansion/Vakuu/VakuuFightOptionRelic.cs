using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuFightOptionRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override string CustomIconPath => VakuuFightAssetPaths.OptionIcon;

    public override string CustomBigIconPath => VakuuFightAssetPaths.OptionIcon;

    public override string CustomIconOutlinePath => VakuuFightAssetPaths.OptionIcon;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;
}
