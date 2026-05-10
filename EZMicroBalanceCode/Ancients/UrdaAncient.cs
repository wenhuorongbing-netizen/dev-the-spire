using System;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class EzmbUrda : AncientEventModel
{
    public const string SeedbedBlessing = "urda_seedbed";
    public const string HumusBlessing = "urda_humus_pact";
    public const string MoltingBlessing = "urda_molting";
    public const string MossMapBlessing = "urda_moss_map";

    public override IEnumerable<EventOption> AllPossibleOptions =>
        new[]
        {
            SeedbedSelectionOption,
            HumusSelectionOption,
            MoltingSelectionOption,
            MossMapSelectionOption
        };

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
            var optionBlessingId = option.TextKey[(option.TextKey.LastIndexOf('.') + 1)..];
            return optionBlessingId.Equals(normalized, StringComparison.OrdinalIgnoreCase);
        });

        if (forced != null)
        {
            return new[] { forced };
        }

        MainFile.Logger.Warn($"[EZMicroBalance] Urda blessing force ignored; unknown value '{forcedBlessing}'. Showing full pool.");
        return options;
    }

    private EventOption SeedbedSelectionOption => new(
        this,
        () => SelectBlessing(SeedbedBlessing),
        InitialOptionKey(SeedbedBlessing));

    private EventOption HumusSelectionOption => new(
        this,
        () => SelectBlessing(HumusBlessing),
        InitialOptionKey(HumusBlessing));

    private EventOption MoltingSelectionOption => new(
        this,
        () => SelectBlessing(MoltingBlessing),
        InitialOptionKey(MoltingBlessing));

    private EventOption MossMapSelectionOption => new(
        this,
        () => SelectBlessing(MossMapBlessing),
        InitialOptionKey(MossMapBlessing));

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

internal static class UrdaFeatureGate
{
    public const string ForceAncientEnvironmentVariable = "EZMB_FORCE_ANCIENT";
    public const string ForceBlessingEnvironmentVariable = "EZMB_FORCE_URDA_BLESSING";

    public static string? ForcedBlessing => Environment.GetEnvironmentVariable(ForceBlessingEnvironmentVariable)?.Trim();

    public static bool IsUrdaEnabled(UnlockState unlockState) =>
        unlockState.IsEpochRevealed<NeowEpoch>() ||
        string.Equals(
            Environment.GetEnvironmentVariable(ForceAncientEnvironmentVariable)?.Trim(),
            "URDA",
            StringComparison.OrdinalIgnoreCase);
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
