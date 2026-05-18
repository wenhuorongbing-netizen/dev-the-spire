using System.Threading.Tasks;

using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaRewardSelectionService
{
    public static async Task SelectBlessing<T>(Player owner, string blessingId)
        where T : RelicModel
    {
        UrdaBlessingService.SetSelectedBlessing(owner, blessingId);
        await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(owner, blessingId);

        if (blessingId == UrdaBlessingIds.Molting)
        {
            await UrdaBlessingService.ApplyMolting(owner);
        }
        else if (blessingId == UrdaBlessingIds.TrialBranch)
        {
            await UrdaBlessingService.ApplyTrialBranch(owner);
        }
        else if (blessingId == UrdaBlessingIds.ShallowRootRelic)
        {
            await UrdaBlessingService.ApplyShallowRootRelic(owner);
        }
        else if (blessingId == UrdaBlessingIds.RootedRoute)
        {
            UrdaBlessingService.ApplyRootedRoute(owner);
        }
        else if (blessingId == UrdaBlessingIds.RootSight)
        {
            await UrdaBlessingService.ApplyRootSight(owner);
        }
    }
}
