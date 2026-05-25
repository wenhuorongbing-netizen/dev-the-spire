using System.Threading.Tasks;

using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static class UrdaRewardSelectionService
{
    public static async Task<bool> SelectBlessing<T>(Player owner, string blessingId)
        where T : RelicModel
    {
        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                owner.RunState,
                "UrdaAncientSelection",
                "Urda Ancient rewards can open local card/relic/map choices and are disabled in co-op until host-authoritative sync is proven."))
        {
            AncientSelectionEvidenceLog.LogBlessingSelectionFailed(
                owner,
                "Urda",
                blessingId,
                "coop_gameplay_disabled",
                !string.IsNullOrWhiteSpace(UrdaFeatureGate.ForcedBlessing));
            return false;
        }

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

        return true;
    }
}
