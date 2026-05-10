using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class EzmbUrda : AncientEventModel, ICustomModel
{
    public override IEnumerable<EventOption> AllPossibleOptions =>
        [
            SeedbedSelectionOption,
            HumusSelectionOption,
            MoltingSelectionOption,
            MossMapSelectionOption
        ];

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = null,
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(),
            AgnosticDialogues = Array.Empty<AncientDialogue>()
        };
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var options = AllPossibleOptions.ToList();
        var forcedBlessing = UrdaFeatureGate.ForcedBlessing;
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

        return forced is null ? options : new[] { forced };
    }

    private EventOption SeedbedSelectionOption => new(
        this,
        () => SelectBlessing(UrdaBlessingIds.Seedbed),
        InitialOptionKey(UrdaBlessingIds.Seedbed));

    private EventOption HumusSelectionOption => new(
        this,
        () => SelectBlessing(UrdaBlessingIds.HumusPact),
        InitialOptionKey(UrdaBlessingIds.HumusPact));

    private EventOption MoltingSelectionOption => new(
        this,
        () => SelectBlessing(UrdaBlessingIds.Molting),
        InitialOptionKey(UrdaBlessingIds.Molting));

    private EventOption MossMapSelectionOption => new(
        this,
        () => SelectBlessing(UrdaBlessingIds.MossMap),
        InitialOptionKey(UrdaBlessingIds.MossMap));

    private Task SelectBlessing(string blessingId)
    {
        if (Owner != null)
        {
            AncientSavedStateFields.UrdaStateKey[Owner] = blessingId;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Urda blessing selected: {blessingId}.");
        Done();
        return Task.CompletedTask;
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

        var list = unlockedAncients.ToList();
        var urda = ModelDb.AncientEvent<EzmbUrda>();
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

[HarmonyPatch(typeof(Underdocks), nameof(Underdocks.GetUnlockedAncients))]
internal static class UrdaUnderdocksPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        UrdaAct1AncientService.AddUrdaToAct1(unlockState, ref __result);
}
