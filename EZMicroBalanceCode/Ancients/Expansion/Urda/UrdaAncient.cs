using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseLib.Abstracts;
using BaseLib.Utils;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class EzmbUrda : CustomAncientModel
{
    private const int ExpectedInitialOptionCount = 4;

    public EzmbUrda()
        : base(autoAdd: false)
    {
    }

    protected override OptionPools MakeOptionPools => new(MakePool(Array.Empty<AncientOption>()));

    public override string? CustomScenePath => UrdaAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => UrdaAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => UrdaAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => UrdaAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => UrdaAssetPaths.RunHistoryIconOutline;

    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            SeedbedSelectionOption,
            HumusSelectionOption,
            MoltingSelectionOption,
            MossMapSelectionOption,
            TrialBranchSelectionOption,
            ShallowRootRelicSelectionOption,
            RootedRouteSelectionOption,
            AfterRainSelectionOption,
            RootSightSelectionOption,
            SeedBankSelectionOption
        ];

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(AncientDialogueLine.sfxFallbackPath),
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(),
            AgnosticDialogues = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
        };
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions.ToList();
        var forcedBlessing = UrdaFeatureGate.ForcedBlessing;
        if (string.IsNullOrWhiteSpace(forcedBlessing))
        {
            return TakeFallbackOptions(options);
        }

        var normalized = forcedBlessing.Trim().ToLowerInvariant();
        var forced = options.FirstOrDefault(option =>
        {
            var optionId = option.TextKey[(option.TextKey.LastIndexOf('.') + 1)..];
            return optionId.Equals(normalized, StringComparison.OrdinalIgnoreCase);
        });

        if (forced is not null)
        {
            return [forced];
        }

        MainFile.Logger.Warn($"[EZMicroBalance] Urda forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(List<EventOption> options)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Urda only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        return options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
    }

    private EventOption SeedbedSelectionOption =>
        OptionWithRelic<UrdaSeedbedOptionRelic>(
            UrdaBlessingIds.Seedbed,
            HoverTipFactory.FromCardWithCardHoverTips<UrdaSeedling>());

    private EventOption HumusSelectionOption =>
        OptionWithRelic<UrdaHumusPactOptionRelic>(UrdaBlessingIds.HumusPact);

    private EventOption MoltingSelectionOption =>
        OptionWithRelic<UrdaMoltingOptionRelic>(
            UrdaBlessingIds.Molting,
            HoverTipFactory.FromCardWithCardHoverTips<WitheredHusk>());

    private EventOption MossMapSelectionOption =>
        OptionWithRelic<UrdaMossMapOptionRelic>(UrdaBlessingIds.MossMap);

    private EventOption TrialBranchSelectionOption =>
        OptionWithRelic<UrdaTrialBranchOptionRelic>(UrdaBlessingIds.TrialBranch);

    private EventOption ShallowRootRelicSelectionOption =>
        OptionWithRelic<UrdaShallowRootRelicOptionRelic>(UrdaBlessingIds.ShallowRootRelic);

    private EventOption RootedRouteSelectionOption =>
        OptionWithRelic<UrdaRootedRouteOptionRelic>(UrdaBlessingIds.RootedRoute);

    private EventOption AfterRainSelectionOption =>
        OptionWithRelic<UrdaAfterRainOptionRelic>(UrdaBlessingIds.AfterRain);

    private EventOption RootSightSelectionOption =>
        OptionWithRelic<UrdaRootSightOptionRelic>(UrdaBlessingIds.RootSight);

    private EventOption SeedBankSelectionOption =>
        OptionWithRelic<UrdaSeedBankOptionRelic>(UrdaBlessingIds.SeedBank);

    private EventOption OptionWithRelic<T>(string blessingId, IEnumerable<IHoverTip>? hoverTips = null) where T : RelicModel
    {
        var option = new EventOption(this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId), hoverTips ?? []);
        return option.WithRelic<T>(Owner);
    }

    private async Task SelectBlessing<T>(string blessingId)
        where T : RelicModel
    {
        if (Owner != null)
        {
            UrdaBlessingService.SetSelectedBlessing(Owner, blessingId);
            await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId);
            if (blessingId == UrdaBlessingIds.Molting)
            {
                await UrdaBlessingService.ApplyMolting(Owner);
            }
            else if (blessingId == UrdaBlessingIds.TrialBranch)
            {
                await UrdaBlessingService.ApplyTrialBranch(Owner);
            }
            else if (blessingId == UrdaBlessingIds.ShallowRootRelic)
            {
                await UrdaBlessingService.ApplyShallowRootRelic(Owner);
            }
            else if (blessingId == UrdaBlessingIds.RootedRoute)
            {
                UrdaBlessingService.ApplyRootedRoute(Owner);
            }
            else if (blessingId == UrdaBlessingIds.RootSight)
            {
                await UrdaBlessingService.ApplyRootSight(Owner);
            }
        }

        MainFile.Logger.Info($"[EZMicroBalance] Urda blessing selected: {blessingId}.");
        Done();
    }
}

internal static class UrdaAct1AncientService
{
    public static void AddUrdaToAct1(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!UrdaFeatureGate.IsUrdaEnabled(unlockState))
        {
            return;
        }

        var urda = ModelDb.AncientEvent<EzmbUrda>();
        if (UrdaFeatureGate.ShouldForceUrda)
        {
            unlockedAncients = [urda];
            MainFile.Logger.Info("[EZMicroBalance] EZMB_FORCE_ANCIENT forced Urda as the Act 1 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == urda.Id))
        {
            list.Add(urda);
            MainFile.Logger.Info("[EZMicroBalance] Urda added to Act 1 unlocked ancients.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Overgrowth), nameof(Overgrowth.GetUnlockedAncients))]
internal static class UrdaOvergrowthPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        UrdaAct1AncientService.AddUrdaToAct1(unlockState, ref __result);
}

internal static class UrdaAssetPaths
{
    public static string MapIcon => $"{MainFile.ResPath}/images/ancients/urda/ezmb_urda_map_icon.png";

    public static string MapIconOutline => $"{MainFile.ResPath}/images/ancients/urda/ezmb_urda_map_icon_outline.png";

    public static string RunHistoryIcon => $"{MainFile.ResPath}/images/ancients/urda/ezmb_urda_run_history_icon.png";

    public static string RunHistoryIconOutline => $"{MainFile.ResPath}/images/ancients/urda/ezmb_urda_run_history_icon_outline.png";

    public static string SeedbedOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_seedbed.png";

    public static string HumusPactOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_humus_pact.png";

    public static string MoltingOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_molting.png";

    public static string MossMapOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_moss_map.png";

    public static string TrialBranchOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_trial_branch.png";

    public static string ShallowRootRelicOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_shallow_root_relic.png";

    public static string RootedRouteOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_rooted_route.png";

    public static string AfterRainOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_after_rain.png";

    public static string RootSightOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_root_sight.png";

    public static string SeedBankOptionIcon => $"{MainFile.ResPath}/images/ancients/urda/options/urda_seed_bank.png";

    public static string BackgroundScene => $"{MainFile.ResPath}/scenes/events/background_scenes/ezmb_urda.tscn";
}

[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]
internal static class UrdaUnderdocksPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        UrdaAct1AncientService.AddUrdaToAct1(unlockState, ref __result);
}
