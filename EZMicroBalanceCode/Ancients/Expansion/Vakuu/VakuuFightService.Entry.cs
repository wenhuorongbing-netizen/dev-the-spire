using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
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
        var option = EventOption.FromRelic(
            ModelDb.Relic<VakuuFightOptionRelic>().ToMutable(),
            vakuu,
            () => StartFight(vakuu),
            FightOptionKey);
        option.HoverTips = option.HoverTips
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuKnifeContract>())
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuTemptation>())
            .Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuShelterContract>())
            .Concat([HoverTipFactory.FromPower<VakuuStolenVaultPower>()])
            .Concat([HoverTipFactory.FromPower<VakuuBloodDebtPower>()]);
        return option;
    }

    private static async Task StartFight(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        if (vakuu.Owner is null)
        {
            return;
        }

        await AncientRewardRelicService.ObtainSelectionRelicIfMissing<VakuuFightOptionRelic>(
            vakuu.Owner,
            FightOptionKey);

        var encounter = ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable();
        var combatRoom = new CombatRoom(encounter, vakuu.Owner.RunState)
        {
            ShouldResumeParentEventAfterCombat = true
        };

        ClearEventNode(vakuu);
        MainFile.Logger.Info("[EZMicroBalance] Starting Vakuu fight encounter through the explicit parent-room stack transition.");
        await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true);
    }

    private static void ClearEventNode(EventModel eventModel) =>
        EventNodeBackingField.SetValue(eventModel, null);
}
