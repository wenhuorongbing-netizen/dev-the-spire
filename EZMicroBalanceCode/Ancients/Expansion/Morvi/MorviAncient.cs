using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class EzmbMorvi : CustomAncientModel
{
    private const int ExpectedInitialOptionCount = 3;

    public EzmbMorvi()
        : base(autoAdd: false)
    {
    }

    protected override OptionPools MakeOptionPools => new(MakePool(Array.Empty<AncientOption>()));

    public override string? CustomScenePath => MorviAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => MorviAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => MorviAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => MorviAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => MorviAssetPaths.RunHistoryIconOutline;

    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            ForbiddenLoanSelectionOption,
            MisprintPressSelectionOption,
            RedInkOverdraftSelectionOption,
            OverdueLibrarySelectionOption,
            OpenBookExamSelectionOption,
            PaperstormSelectionOption,
            BlueprintProofSelectionOption,
            DebtSettlementSelectionOption
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
        var forcedBlessing = MorviFeatureGate.ForcedBlessing;
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

        MainFile.Logger.Warn($"[EZMicroBalance] Morvi forced blessing '{forcedBlessing}' did not match any option; showing fallback options.");
        return TakeFallbackOptions(options);
    }

    private IReadOnlyList<EventOption> TakeFallbackOptions(List<EventOption> options)
    {
        if (options.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Morvi has no source-backed Ancient options to show; the event will finish instead of presenting a blank Ancient screen.");
            return [];
        }

        if (options.Count < ExpectedInitialOptionCount)
        {
            MainFile.Logger.Warn($"[EZMicroBalance] Morvi only has {options.Count} source-backed option(s), expected {ExpectedInitialOptionCount}; showing all available options.");
        }

        return options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList();
    }

    private EventOption ForbiddenLoanSelectionOption =>
        OptionWithRelic<MorviForbiddenLoanOptionRelic>(
            MorviBlessingIds.ForbiddenLoan);

    private EventOption MisprintPressSelectionOption =>
        OptionWithRelic<MorviMisprintPressOptionRelic>(
            MorviBlessingIds.MisprintPress,
            [
                HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption RedInkOverdraftSelectionOption =>
        OptionWithRelic<MorviRedInkOverdraftOptionRelic>(
            MorviBlessingIds.RedInkOverdraft,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviRedInkOverdraftCard>(),
                HoverTipFactory.FromPower<MorviOverdraftPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption OverdueLibrarySelectionOption =>
        OptionWithRelic<MorviOverdueLibraryOptionRelic>(
            MorviBlessingIds.OverdueLibrary,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDrawPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveVeilPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBurnPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDiscountPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveBraveryPage>(),
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviArchiveDexterityPage>(),
                HoverTipFactory.Static(StaticHoverTip.Energy),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption OpenBookExamSelectionOption =>
        OptionWithRelic<MorviOpenBookExamOptionRelic>(
            MorviBlessingIds.OpenBookExam,
            [
                HoverTipFactory.FromPower<MorviOpenBookPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption PaperstormSelectionOption =>
        OptionWithRelic<MorviPaperstormOptionRelic>(
            MorviBlessingIds.Paperstorm,
            [
                .. HoverTipFactory.FromCardWithCardHoverTips<MorviWastePaper>(),
                HoverTipFactory.FromPower<MorviPaperstormPower>(),
                HoverTipFactory.Static(StaticHoverTip.Energy)
            ]);

    private EventOption BlueprintProofSelectionOption =>
        OptionWithRelic<MorviBlueprintProofOptionRelic>(
            MorviBlessingIds.BlueprintProof,
            [
                HoverTipFactory.FromPower<MorviProofreadPower>(),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ]);

    private EventOption DebtSettlementSelectionOption =>
        OptionWithRelic<MorviDebtSettlementOptionRelic>(
            MorviBlessingIds.DebtSettlement,
            [HoverTipFactory.FromPower<MorviDebtPower>()]);

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
            await MorviBlessingService.SetSelectedBlessing(Owner, blessingId);
            await AncientRewardRelicService.ObtainSelectionRelicIfMissing<T>(Owner, blessingId);
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi blessing selected: {blessingId}.");
        Done();
    }
}

internal static class MorviAct2AncientService
{
    public static void AddMorviToAct2(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!MorviFeatureGate.IsMorviEnabled(unlockState))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedAncient) &&
            !MorviFeatureGate.ShouldForceMorvi)
        {
            return;
        }

        var morvi = ModelDb.AncientEvent<EzmbMorvi>();
        if (MorviFeatureGate.ShouldForceMorvi)
        {
            unlockedAncients = [morvi];
            MainFile.Logger.Info("[EZMicroBalance] Force Ancient gate selected Morvi as the Act 2 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == morvi.Id))
        {
            list.Add(morvi);
            MainFile.Logger.Info("[EZMicroBalance] Morvi added to Act 2 unlocked ancients for private-beta testing.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))]
internal static class MorviHivePatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        MorviAct2AncientService.AddMorviToAct2(unlockState, ref __result);
}

internal static class MorviAssetPaths
{
    public static string MapIcon => $"{MainFile.ResPath}/images/ancients/morvi/ezmb_morvi_map_icon.png";

    public static string MapIconOutline => $"{MainFile.ResPath}/images/ancients/morvi/ezmb_morvi_map_icon_outline.png";

    public static string RunHistoryIcon => $"{MainFile.ResPath}/images/ancients/morvi/ezmb_morvi_run_history_icon.png";

    public static string RunHistoryIconOutline => $"{MainFile.ResPath}/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png";

    public static string ForbiddenLoanOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_forbidden_loan.png";

    public static string MisprintPressOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_misprint_press.png";

    public static string RedInkOverdraftOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_red_ink_overdraft.png";

    public static string OverdueLibraryOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_overdue_library.png";

    public static string OpenBookExamOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_open_book_exam.png";

    public static string PaperstormOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_paperstorm.png";

    public static string BlueprintProofOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_blueprint_proof.png";

    public static string DebtSettlementOptionIcon => $"{MainFile.ResPath}/images/ancients/morvi/options/morvi_debt_settlement.png";

    public static string DebtPowerIcon => DebtSettlementOptionIcon;

    public static string ProofreadPowerIcon => BlueprintProofOptionIcon;

    public static string OpenBookPowerIcon => OpenBookExamOptionIcon;

    public static string OverdraftPowerIcon => RedInkOverdraftOptionIcon;

    public static string PaperstormPowerIcon => PaperstormOptionIcon;

    public static string BackgroundScene => $"{MainFile.ResPath}/scenes/events/background_scenes/ezmb_morvi.tscn";
}
