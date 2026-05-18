using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Characters;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed class EzmbLotha : CustomAncientModel
{
    private const int ExpectedInitialOptionCount = 3;

    public EzmbLotha()
        : base(autoAdd: false)
    {
    }

    protected override OptionPools MakeOptionPools => new(MakePool(Array.Empty<AncientOption>()));

    public override string? CustomScenePath => LothaAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => LothaAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => LothaAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => LothaAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => LothaAssetPaths.RunHistoryIconOutline;

    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            MirrorRebuttalSelectionOption,
            MirrorHallEchoSelectionOption,
            PresumptionSelectionOption,
            ClosedCourtSelectionOption,
            DeferredVerdictSelectionOption,
            DeathReprieveSelectionOption,
            SingleSentenceSelectionOption,
            PublicEvidenceSelectionOption
        ];

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(AncientDialogueLine.sfxFallbackPath),
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
            {
                [CharKey<Ironclad>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Silent>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Defect>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Necrobinder>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Regent>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
            },
            AgnosticDialogues = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
        };
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions
            .Where(IsCurrentlyAvailableOption)
            .ToList();
        var forcedBlessing = LothaFeatureGate.ForcedBlessing;
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

        MainFile.Logger.Warn($"[EZMicroBalance] Lotha forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(List<EventOption> options)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Lotha only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        return options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
    }

    private bool IsCurrentlyAvailableOption(EventOption option)
    {
        if (Owner == null)
        {
            return true;
        }

        return !option.TextKey.EndsWith($".{LothaBlessingIds.MirrorRebuttal}", StringComparison.OrdinalIgnoreCase) ||
            LothaBlessingService.HasMirrorRebuttalCandidates(Owner);
    }

    private EventOption MirrorRebuttalSelectionOption =>
        OptionWithRelic<LothaMirrorRebuttalOptionRelic>(
            LothaBlessingIds.MirrorRebuttal,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption MirrorHallEchoSelectionOption =>
        OptionWithRelic<LothaMirrorHallEchoOptionRelic>(
            LothaBlessingIds.MirrorHallEcho,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PresumptionSelectionOption =>
        OptionWithRelic<LothaPresumptionOptionRelic>(
            LothaBlessingIds.Presumption,
            [
                HoverTipFactory.FromPower<LothaPresumptionPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption ClosedCourtSelectionOption =>
        OptionWithRelic<LothaClosedCourtOptionRelic>(
            LothaBlessingIds.ClosedCourt,
            [HoverTipFactory.Static(StaticHoverTip.Energy)]);

    private EventOption DeferredVerdictSelectionOption =>
        OptionWithRelic<LothaDeferredVerdictOptionRelic>(
            LothaBlessingIds.DeferredVerdict,
            [
                HoverTipFactory.FromPower<LothaVerdictPower>(),
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption DeathReprieveSelectionOption =>
        OptionWithRelic<LothaDeathReprieveOptionRelic>(
            LothaBlessingIds.DeathReprieve,
            [HoverTipFactory.FromPower<LothaDeathReprievePower>()]);

    private EventOption SingleSentenceSelectionOption =>
        OptionWithRelic<LothaSingleSentenceOptionRelic>(
            LothaBlessingIds.SingleSentence,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PublicEvidenceSelectionOption =>
        OptionWithRelic<LothaPublicEvidenceOptionRelic>(
            LothaBlessingIds.PublicEvidence,
            [
                HoverTipFactory.FromPower<LothaEnlightenmentPower>(),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

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
            await LothaRewardSelectionService.SelectBlessing<T>(Owner, blessingId);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha blessing selected: {blessingId}.");
        Done();
    }
}
