using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseLib.Abstracts;
using BaseLib.Utils;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed partial class EzmbUrda
{
    private EventOption SeedbedSelectionOption =>
        OptionWithRelic<UrdaSeedbedOptionRelic>(
            UrdaBlessingIds.Seedbed,
            [HoverTipFactory.FromCard<UrdaSeedbed>()]);

    private EventOption HumusSelectionOption =>
        OptionWithRelic<UrdaHumusPactOptionRelic>(UrdaBlessingIds.HumusPact);

    private EventOption MoltingSelectionOption =>
        OptionWithRelic<UrdaMoltingOptionRelic>(
            UrdaBlessingIds.Molting,
            [HoverTipFactory.FromCard<WitheredHusk>()]);

    private EventOption MossMapSelectionOption =>
        OptionWithRelic<UrdaMossMapOptionRelic>(UrdaBlessingIds.MossMap);

    private EventOption TrialBranchSelectionOption =>
        OptionWithRelic<UrdaTrialBranchOptionRelic>(UrdaBlessingIds.TrialBranch);

    private EventOption ShallowRootRelicSelectionOption =>
        OptionWithRelic<UrdaShallowRootRelicOptionRelic>(UrdaBlessingIds.ShallowRootRelic);

    private EventOption EliteRootSelectionOption =>
        OptionWithRelic<UrdaEliteRootOptionRelic>(UrdaBlessingIds.EliteRoot);

    private EventOption RootedRouteSelectionOption =>
        OptionWithRelic<UrdaRootedRouteOptionRelic>(UrdaBlessingIds.RootedRoute);

    private EventOption AfterRainSelectionOption =>
        OptionWithRelic<UrdaAfterRainOptionRelic>(UrdaBlessingIds.AfterRain);

    private EventOption RootSightSelectionOption =>
        OptionWithRelic<UrdaRootSightOptionRelic>(UrdaBlessingIds.RootSight, RootSightHoverTips);

    private EventOption SeedBankSelectionOption =>
        OptionWithRelic<UrdaSeedBankOptionRelic>(UrdaBlessingIds.SeedBank);

    private static IEnumerable<IHoverTip> RootSightHoverTips =>
    [
        new HoverTip(
            new LocString("ancients", "EZMB_URDA.root_sight.hover.title"),
            new LocString("ancients", "EZMB_URDA.root_sight.hover.description"))
    ];

    private EventOption OptionWithRelic<T>(string blessingId, IEnumerable<IHoverTip>? hoverTips = null) where T : RelicModel
    {
        var relic = ModelDb.Relic<T>().ToMutable();
        if (Owner != null)
        {
            relic.Owner = Owner;
        }

        var option = EventOption.FromRelic(relic, this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId));
        option.HoverTips = option.HoverTips.Concat(hoverTips ?? []).ToList();
        return option;
    }

    private async Task SelectBlessing<T>(string blessingId)
        where T : RelicModel
    {
        if (Owner != null)
        {
            await UrdaRewardSelectionService.SelectBlessing<T>(Owner, blessingId);
            AncientSelectionEvidenceLog.LogBlessingSelected(
                Owner,
                "Urda",
                blessingId,
                typeof(T).Name,
                !string.IsNullOrWhiteSpace(UrdaFeatureGate.ForcedBlessing));
        }
        Done();
    }
}
