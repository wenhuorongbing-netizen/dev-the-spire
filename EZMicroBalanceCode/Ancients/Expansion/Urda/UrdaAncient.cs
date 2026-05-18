using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

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
            HoverTipFactory.FromCardWithCardHoverTips<UrdaSeedbed>());

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
        }

        MainFile.Logger.Info($"[EZMicroBalance] Urda blessing selected: {blessingId}.");
        Done();
    }
}
