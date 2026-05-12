using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class EzmbMorvi : AncientEventModel, ICustomModel
{
    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            MisprintPressSelectionOption,
            OpenBookExamSelectionOption,
            DebtSettlementSelectionOption
        ];

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = null,
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
            {
                [CharKey<Ironclad>()] = Array.Empty<AncientDialogue>(),
                [CharKey<Silent>()] = Array.Empty<AncientDialogue>(),
                [CharKey<Defect>()] = Array.Empty<AncientDialogue>(),
                [CharKey<Necrobinder>()] = Array.Empty<AncientDialogue>(),
                [CharKey<Regent>()] = Array.Empty<AncientDialogue>()
            },
            AgnosticDialogues = Array.Empty<AncientDialogue>()
        };
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions.ToList();
        var forcedBlessing = MorviFeatureGate.ForcedBlessing;
        if (string.IsNullOrWhiteSpace(forcedBlessing))
        {
            return options;
        }

        var normalized = forcedBlessing.Trim().ToLowerInvariant();
        var forced = options.FirstOrDefault(option =>
        {
            var optionId = option.TextKey[(option.TextKey.LastIndexOf('.') + 1)..];
            return optionId.Equals(normalized, StringComparison.OrdinalIgnoreCase);
        });

        return forced is null ? options : [forced];
    }

    private EventOption MisprintPressSelectionOption => new(
        this,
        () => SelectBlessing(MorviBlessingIds.MisprintPress),
        InitialOptionKey(MorviBlessingIds.MisprintPress));

    private EventOption OpenBookExamSelectionOption => new(
        this,
        () => SelectBlessing(MorviBlessingIds.OpenBookExam),
        InitialOptionKey(MorviBlessingIds.OpenBookExam));

    private EventOption DebtSettlementSelectionOption => new(
        this,
        () => SelectBlessing(MorviBlessingIds.DebtSettlement),
        InitialOptionKey(MorviBlessingIds.DebtSettlement));

    private async Task SelectBlessing(string blessingId)
    {
        if (Owner != null)
        {
            await MorviBlessingService.SetSelectedBlessing(Owner, blessingId);
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

        var list = unlockedAncients.ToList();
        var morvi = ModelDb.AncientEvent<EzmbMorvi>();
        if (!list.Any(ancient => ancient.Id == morvi.Id))
        {
            list.Add(morvi);
            MainFile.Logger.Info("[EZMicroBalance] Morvi added to Act 2 unlocked ancients behind default-off v2.2 gate.");
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

