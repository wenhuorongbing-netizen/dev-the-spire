using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Unlocks;

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
        var options = AllPossibleOptions.ToList();
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
        var option = new EventOption(this, () => SelectBlessing<T>(blessingId), InitialOptionKey(blessingId), hoverTips ?? []);
        return option.WithRelic<T>(Owner);
    }

    private async Task SelectBlessing<T>(string blessingId)
        where T : RelicModel
    {
        if (Owner != null)
        {
            LothaBlessingService.SetSelectedBlessing(Owner, blessingId);
            await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId);
            if (blessingId == LothaBlessingIds.MirrorRebuttal)
            {
                await SelectMirrorRebuttalCard(Owner);
            }
        }

        MainFile.Logger.Info($"[EZMicroBalance] Lotha blessing selected: {blessingId}.");
        Done();
    }

    private static async Task SelectMirrorRebuttalCard(Player owner)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("ancients", "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt"),
            1)
        {
            RequireManualConfirmation = true
        };

        var selected = (await CardSelectCmd.FromDeckGeneric(
                owner,
                prefs,
                LothaBlessingService.IsMirrorRebuttalDeckCardCandidate))
            .FirstOrDefault();
        if (selected == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Lotha Mirror Rebuttal selection skipped: no eligible non-Curse, non-Status deck card.");
            return;
        }

        LothaBlessingService.MarkMirrorRebuttalCard(owner, selected);
    }
}

internal static class LothaAct3AncientService
{
    public static void AddLothaToAct3(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!LothaFeatureGate.IsLothaEnabled(unlockState))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(LothaFeatureGate.ForcedAncient) &&
            !LothaFeatureGate.ShouldForceLotha)
        {
            return;
        }

        var lotha = ModelDb.AncientEvent<EzmbLotha>();
        if (LothaFeatureGate.ShouldForceLotha)
        {
            unlockedAncients = [lotha];
            MainFile.Logger.Info("[EZMicroBalance] Force Ancient gate selected Lotha as the Act 3 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == lotha.Id))
        {
            list.Add(lotha);
            MainFile.Logger.Info("[EZMicroBalance] Lotha added to Act 3 unlocked ancients for private-beta testing.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]
internal static class LothaGloryPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        LothaAct3AncientService.AddLothaToAct3(unlockState, ref __result);
}

internal static class LothaAssetPaths
{
    public static string MapIcon => $"{MainFile.ResPath}/images/ancients/lotha/ezmb_lotha_map_icon.png";

    public static string MapIconOutline => $"{MainFile.ResPath}/images/ancients/lotha/ezmb_lotha_map_icon_outline.png";

    public static string RunHistoryIcon => $"{MainFile.ResPath}/images/ancients/lotha/ezmb_lotha_run_history_icon.png";

    public static string RunHistoryIconOutline => $"{MainFile.ResPath}/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png";

    public static string MirrorRebuttalOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_mirror_rebuttal.png";

    public static string MirrorHallEchoOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_mirror_hall_echo.png";

    public static string PresumptionOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_presumption.png";

    public static string ClosedCourtOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_closed_court.png";

    public static string DeferredVerdictOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_deferred_verdict.png";

    public static string DeathReprieveOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_death_reprieve.png";

    public static string SingleSentenceOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_single_sentence.png";

    public static string PublicEvidenceOptionIcon => $"{MainFile.ResPath}/images/ancients/lotha/options/lotha_public_evidence.png";

    public static string VerdictPowerIcon => $"{MainFile.ResPath}/images/powers/lotha_verdict.png";

    public static string PresumptionPowerIcon => PresumptionOptionIcon;

    public static string DeathReprievePowerIcon => DeathReprieveOptionIcon;

    public static string EnlightenmentPowerIcon => PublicEvidenceOptionIcon;

    public static string BackgroundScene => $"{MainFile.ResPath}/scenes/events/background_scenes/ezmb_lotha.tscn";
}
