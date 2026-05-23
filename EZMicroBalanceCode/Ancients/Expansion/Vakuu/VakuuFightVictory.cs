using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuFightService
{
    private const string VictoryDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY.description";
    private const string VictoryFallbackDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description";
    private const string VictoryFallbackOptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE";

    private static readonly MethodInfo SetEventStateMethod =
        typeof(EventModel).GetMethod("SetEventState", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(nameof(EventModel), "SetEventState");

    public static Task ResumeAfterVictory(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu, CombatRoom combatRoom)
    {
        var options = CreateVictoryOptions(vakuu, combatRoom).ToList();
        var descriptionKey = GetVictoryDescriptionKey(options.Count, options[0].TextKey == VictoryFallbackOptionKey);
        SetEventStateMethod.Invoke(
            vakuu,
            [new LocString("ancients", descriptionKey), options]);
        ReleaseEvidenceLog.Log(
            "VakuuFight",
            options[0].TextKey == VictoryFallbackOptionKey ? "fallback_map_exit" : "parent_event_resume_success",
            runState: combatRoom.CombatState.RunState,
            data: new Dictionary<string, object?>
            {
                ["brokenLocks"] = GetEncounter(combatRoom).BrokenLocks,
                ["options"] = options.Count
            });
        MainFile.Logger.Info(
            options[0].TextKey == VictoryFallbackOptionKey
                ? "[EZMicroBalance] Vakuu fight victory had no unclaimed non-Vakuu Act 3 Ancient blessing options; using explicit fallback."
                : $"[EZMicroBalance] Vakuu fight victory resolved with {GetEncounter(combatRoom).BrokenLocks} broken locks, {options.Count} blessing choices, and {GetEncounter(combatRoom).VictoryGold} bonus Gold.");
        return Task.CompletedTask;
    }

    private static IEnumerable<EventOption> CreateVictoryOptions(
        MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu,
        CombatRoom combatRoom)
    {
        var owner = vakuu.Owner;
        if (owner is null)
        {
            MainFile.Logger.Info("[EZMicroBalance] Vakuu fight victory resume had no owner; using the explicit fallback path. Live restore for this path remains pending.");
            return [CreateVictoryFallbackOption(vakuu, combatRoom)];
        }

        var encounter = GetEncounter(combatRoom);
        var targetChoiceCount = encounter.VictoryChoiceCount;
        var options = GetNonVakuuAct3AncientRewardChoices(owner)
            .ToList()
            .UnstableShuffle(vakuu.Rng)
            .Take(targetChoiceCount)
            .Select((VictoryRelicChoice choice) =>
            {
                choice.Relic.Owner = owner;
                return EventOption.FromRelic(
                    choice.Relic,
                    vakuu,
                    async () =>
                    {
                        await choice.OnChosen();
                        await SettleVakuuRewards(owner, encounter);
                        vakuu.StartPreFinished();
                    },
                    $"EZMB_VAKUU_FIGHT.pages.VICTORY.options.{choice.Relic.Id.Entry}");
            })
            .ToList();

        return options.Count > 0 ? options : [CreateVictoryFallbackOption(vakuu, combatRoom)];
    }

    private static EventOption CreateVictoryFallbackOption(
        MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu,
        CombatRoom combatRoom)
    {
        return new EventOption(
            vakuu,
            async () =>
            {
                if (vakuu.Owner != null)
                {
                    await SettleVakuuRewards(vakuu.Owner, GetEncounter(combatRoom));
                }

                vakuu.StartPreFinished();
            },
            VictoryFallbackOptionKey);
    }

    private static EzmbVakuuTrialEncounter GetEncounter(CombatRoom combatRoom) =>
        (EzmbVakuuTrialEncounter)combatRoom.Encounter;

    private static string GetVictoryDescriptionKey(int optionCount, bool fallback) =>
        fallback
            ? VictoryFallbackDescriptionKey
            : optionCount switch
            {
                1 => "EZMB_VAKUU_FIGHT.pages.VICTORY_ONE.description",
                2 => "EZMB_VAKUU_FIGHT.pages.VICTORY_TWO.description",
                _ => VictoryDescriptionKey
            };

    private static async Task SettleVakuuRewards(Player owner, EzmbVakuuTrialEncounter encounter)
    {
        if (encounter.VictoryGold > 0)
        {
            await PlayerCmd.GainGold(encounter.VictoryGold, owner);
        }

        if (encounter.BloodDebtShortfall > 0 && owner.Creature.CurrentHp > 1)
        {
            var chunks = Math.Ceiling(encounter.BloodDebtShortfall / EzmbVakuuTrialEncounter.GoldCostPerBloodDebt);
            var hpLoss = Math.Min(
                owner.Creature.CurrentHp - 1,
                chunks * EzmbVakuuTrialEncounter.HpLossPerDebtShortfall);
            if (hpLoss > 0)
            {
                await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - hpLoss);
            }
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu rewards settled: locks={encounter.BrokenLocks}, lootGold={encounter.VictoryLootGold}, bloodDebt={encounter.BloodDebt}, paidGold={encounter.VictoryGold}, shortfall={encounter.BloodDebtShortfall}.");
    }

}
