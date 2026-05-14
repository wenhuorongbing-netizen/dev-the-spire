using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]
internal static class VakuuForceAncientPatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result)
    {
        if (!VakuuFightFeatureGate.IsFightEnabled(unlockState) || !VakuuFightFeatureGate.ShouldForceVakuu)
        {
            return;
        }

        __result = [ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>()];
        MainFile.Logger.Info("[EZMicroBalance] Force Ancient gate selected Vakuu as the Act 3 Ancient.");
    }
}

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), "GenerateInitialOptions")]
internal static class VakuuFightOptionPatch
{
    [HarmonyPostfix]
    private static void AddFightOption(
        MegaCrit.Sts2.Core.Models.Events.Vakuu __instance,
        ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner?.RunState is not RunState runState ||
            !VakuuFightFeatureGate.IsFightEnabledForRun(runState))
        {
            return;
        }

        var fightOption = VakuuFightService.CreateFightOption(__instance);
        if (VakuuFightFeatureGate.ShouldForceFight)
        {
            __result = [fightOption];
            return;
        }

        __result = __result.Concat([fightOption]).ToList();
    }
}

[HarmonyPatch(typeof(EventModel), nameof(EventModel.Resume))]
internal static class VakuuFightResumePatch
{
    [HarmonyPrefix]
    private static bool ResumeVakuuFightVictory(EventModel __instance, AbstractRoom exitedRoom, ref Task __result)
    {
        if (__instance is not MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu ||
            exitedRoom is not CombatRoom { Encounter: EzmbVakuuTrialEncounter })
        {
            return true;
        }

        __result = VakuuFightService.ResumeAfterVictory(vakuu);
        return false;
    }
}

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.ToSerializable))]
internal static class VakuuFightPreFinishedSavePatch
{
    [HarmonyPostfix]
    private static void PreserveVakuuParentForPreFinishedSave(
        CombatRoom __instance,
        SerializableRoom __result) =>
        VakuuFightService.PreserveParentEventForPreFinishedSave(__instance, __result);
}

internal static class VakuuFightService
{
    private const string FightOptionKey = "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight";
    private const string VictoryDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY.description";
    private const string VictoryFallbackDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description";
    private const string VictoryFallbackOptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE";

    private static readonly MethodInfo SetEventStateMethod =
        typeof(EventModel).GetMethod("SetEventState", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(nameof(EventModel), "SetEventState");

    public static EventOption CreateFightOption(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        var option = EventOption.FromRelic(
                ModelDb.Relic<VakuuFightOptionRelic>().ToMutable(),
                vakuu,
                () => StartFight(vakuu),
                FightOptionKey)
            .ThatWillKillPlayerIf(_ => true);
        option.HoverTips = option.HoverTips.Concat(HoverTipFactory.FromCardWithCardHoverTips<VakuuTemptation>());
        return option;
    }

    public static Task ResumeAfterVictory(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        var options = CreateVictoryOptions(vakuu).ToList();
        var descriptionKey =
            options.Count == 1 && options[0].TextKey == VictoryFallbackOptionKey
                ? VictoryFallbackDescriptionKey
                : VictoryDescriptionKey;
        SetEventStateMethod.Invoke(
            vakuu,
            [new LocString("ancients", descriptionKey), options]);
        MainFile.Logger.Info(
            options[0].TextKey == VictoryFallbackOptionKey
                ? "[EZMicroBalance] Vakuu fight victory had no unclaimed non-Vakuu Act 3 Ancient blessing options; using explicit fallback."
                : "[EZMicroBalance] Vakuu fight victory resolved into three non-Vakuu Act 3 Ancient blessing options.");
        return Task.CompletedTask;
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

        var combatRoom = new CombatRoom(ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable(), vakuu.Owner.RunState);

        MainFile.Logger.Info("[EZMicroBalance] Starting Vakuu fight encounter with parent resume handled by the active room stack.");
        await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true);
    }

    public static void PreserveParentEventForPreFinishedSave(
        CombatRoom combatRoom,
        SerializableRoom serializableRoom)
    {
        if (combatRoom.Encounter is not EzmbVakuuTrialEncounter ||
            !combatRoom.IsPreFinished ||
            !combatRoom.ShouldResumeParentEventAfterCombat ||
            serializableRoom.ParentEventId is not null)
        {
            return;
        }

        serializableRoom.ParentEventId =
            ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>().Id;
        serializableRoom.ShouldResumeParentEvent = true;
        MainFile.Logger.Info("[EZMicroBalance] Vakuu pre-finished fight save records Vakuu as the resume parent event.");
    }

    private static IEnumerable<EventOption> CreateVictoryOptions(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        var owner = vakuu.Owner;
        if (owner is null)
        {
            MainFile.Logger.Info("[EZMicroBalance] Vakuu fight victory resume had no owner; using the explicit fallback path. Live restore for this path remains pending.");
            return [CreateVictoryFallbackOption(vakuu)];
        }

        var options = GetNonVakuuAct3AncientRelics(owner)
            .ToList()
            .UnstableShuffle(vakuu.Rng)
            .Take(3)
            .Select((RelicModel relic) => EventOption.FromRelic(
                relic,
                vakuu,
                async () =>
                {
                    await RelicCmd.Obtain(relic, owner);
                    vakuu.StartPreFinished();
                },
                $"EZMB_VAKUU_FIGHT.pages.VICTORY.options.{relic.Id.Entry}"))
            .ToList();

        return options.Count == 3 ? options : [CreateVictoryFallbackOption(vakuu)];
    }

    private static EventOption CreateVictoryFallbackOption(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu)
    {
        return new EventOption(
            vakuu,
            () =>
            {
                vakuu.StartPreFinished();
                return Task.CompletedTask;
            },
            VictoryFallbackOptionKey);
    }

    private static IEnumerable<RelicModel> GetNonVakuuAct3AncientRelics(Player owner)
    {
        var sourceAncients = new AncientEventModel[]
        {
            ModelDb.AncientEvent<Nonupeipe>(),
            ModelDb.AncientEvent<Tanx>()
        };

        return sourceAncients
            .SelectMany(ancient => ancient.AllPossibleOptions)
            .Select(option => option.Relic?.CanonicalInstance)
            .OfType<RelicModel>()
            .Where(relic => owner.GetRelicById(relic.Id) is null)
            .Select(relic => relic.ToMutable());
    }
}

internal static class VakuuFightAssetPaths
{
    public static string OptionIcon => $"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png";
}
