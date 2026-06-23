using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    private const string FightOptionKey = "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight";

    private static readonly FieldInfo EventNodeBackingField =
        typeof(EventModel).GetField("<Node>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingFieldException(nameof(EventModel), "Node backing field");

    public static EventOption CreateFightOption(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        var forcedOption = vakuu.Owner?.RunState is RunState runState &&
            VakuuFightFeatureGate.ShouldForceFightForRun(runState);
        if (vakuu.Owner?.RunState != null)
        {
            ReleaseEvidenceLog.Log("VakuuFight", "fight_option_shown", vakuu.Owner);
        }

        var option = EventOption.FromRelic(
            ModelDb.Relic<VakuuFightOptionRelic>().ToMutable(),
            vakuu,
            () => StartFight(vakuu, forcedOption),
            FightOptionKey);
        option.HoverTips = option.HoverTips
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuKnifeContract>())
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuTemptation>())
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuShelterContract>())
            .Concat([HoverTipFactory.FromPower<VakuuStolenVaultPower>()])
            .Concat([HoverTipFactory.FromPower<VakuuBloodDebtPower>()]);
        return option;
    }

    private static async Task StartFight(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu, bool forcedOption)
    {
        if (vakuu.Owner is null)
        {
            return;
        }

        AncientSelectionEvidenceLog.LogOptionSelected(
            vakuu.Owner,
            "Vakuu",
            FightOptionKey,
            nameof(VakuuFightOptionRelic),
            forcedOption);

        await AncientRewardRelicService.ObtainSelectionRelicIfMissing<VakuuFightOptionRelic>(
            vakuu.Owner,
            FightOptionKey);

        // Core's CombatRoom.ToSerializable rejects ParentEventId while combat
        // is still active. Keep the live fight as a plain child room and let
        // the prefinished-save postfix restore the parent marker only after
        // victory, when Core permits serializing a parent-linked combat room.
        var encounter = ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable();
        var combatRoom = new CombatRoom(encounter, vakuu.Owner.RunState)
        {
            ShouldResumeParentEventAfterCombat = true
        };

        // EnterRoomWithoutExitingCurrentRoom keeps the Vakuu event below the
        // trial combat on the room stack. Clearing the event node first avoids
        // leaving the old event scene attached while the child combat owns UI.
        ClearEventNode(vakuu);
        ReleaseEvidenceLog.Log("VakuuFight", "fight_started", vakuu.Owner);
        MainFile.Logger.Info("[Spire Plus] Starting Vakuu fight encounter through the explicit parent-room stack transition.");
        await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true);
        ReleaseEvidenceLog.Log("VakuuFight", "child_combat_room_entered", vakuu.Owner);
    }

    private static void ClearEventNode(EventModel eventModel) =>
        EventNodeBackingField.SetValue(eventModel, null);
}
