using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRootSightOptionRelic : UrdaOptionRelic
{
    public override string CustomIconPath => UrdaAssetPaths.RootSightOptionIcon;

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
