using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

[Pool(typeof(SharedRelicPool))]
internal sealed class UrdaRootSightOptionRelic : UrdaOptionRelic
{
    public override string PackedIconPath => UrdaAssetPaths.RootSightOptionIcon;

    public override bool IsUsedUp =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.GetRootSightEyes(Owner) <= 0;

    public override bool ShowCounter =>
        IsMutable &&
        Owner != null &&
        UrdaBlessingService.GetRootSightEyes(Owner) > 0;

    public override int DisplayAmount =>
        IsMutable && Owner != null
            ? UrdaBlessingService.GetRootSightEyes(Owner)
            : 0;

    public void RefreshRootSightDisplay() => InvokeDisplayAmountChanged();
}
