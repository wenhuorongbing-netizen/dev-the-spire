using BaseLib.Abstracts;
using BaseLib.Utils.Attributes;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

[Pool(typeof(SharedRelicPool))]
internal sealed class VakuuFightOptionRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override string PackedIconPath => VakuuFightAssetPaths.OptionIcon;

    protected override string BigIconPath => VakuuFightAssetPaths.OptionIcon;

    protected override string PackedIconOutlinePath => VakuuFightAssetPaths.OptionIcon;

    public override bool IsAllowed(IRunState runState) => false;

    public override bool IsAllowedAtNeow(Player player) => false;

    public override bool IsAllowedInShops => false;
}
