using System.Reflection;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Common;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
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
        if (!VakuuFightFeatureGate.ShouldForceVakuu)
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
            exitedRoom is not CombatRoom { Encounter: EzmbVakuuTrialEncounter } combatRoom)
        {
            return true;
        }

        __result = VakuuFightService.ResumeAfterVictory(vakuu, combatRoom);
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

[HarmonyPatch(typeof(EventRoom), nameof(EventRoom.EnterInternal))]
internal static class VakuuFightPreFinishedParentRestorePatch
{
    [HarmonyPrefix]
    private static void ArmVakuuPreFinishedParentRestoreHealSkip(
        EventRoom __instance,
        IRunState? runState,
        bool isRestoringRoomStackBase) =>
        VakuuFightService.ArmPrefinishedParentRestoreHealSkip(
            __instance,
            runState,
            isRestoringRoomStackBase);
}

[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]
internal static class VakuuFightPreFinishedParentRestoreHealPatch
{
    [HarmonyPrefix]
    private static bool SkipDuplicateVakuuRestoreHeal(
        AncientEventModel __instance,
        bool isPreFinished,
        ref Task __result)
    {
        if (!VakuuFightService.ShouldSkipPrefinishedParentRestoreHeal(__instance, isPreFinished))
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]
internal static class VakuuFightNoRewardRestorePatch
{
    [HarmonyPrefix]
    private static bool SkipVakuuLoadedTerminalRewards(CombatRoom __instance, ref Task __result)
    {
        if (__instance.Encounter is not EzmbVakuuTrialEncounter)
        {
            return true;
        }

        __result = VakuuFightService.ProceedFromNoRewardVictory(__instance);
        return false;
    }
}

internal static class VakuuFightService
{
    private const string FightOptionKey = "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight";
    private const string VictoryDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY.description";
    private const string VictoryFallbackDescriptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.description";
    private const string VictoryFallbackOptionKey = "EZMB_VAKUU_FIGHT.pages.VICTORY_FALLBACK.options.CONTINUE";

    private static int pendingVakuuPrefinishedParentRestoreHealSkips;

    private static readonly MethodInfo SetEventStateMethod =
        typeof(EventModel).GetMethod("SetEventState", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(nameof(EventModel), "SetEventState");

    private static readonly FieldInfo EventNodeBackingField =
        typeof(EventModel).GetField("<Node>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingFieldException(nameof(EventModel), "Node backing field");

    private sealed record VictoryRelicChoice(RelicModel Relic, Func<Task> OnChosen);

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

    public static Task ResumeAfterVictory(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu, CombatRoom combatRoom)
    {
        var options = CreateVictoryOptions(vakuu, combatRoom).ToList();
        var descriptionKey = GetVictoryDescriptionKey(options.Count, options[0].TextKey == VictoryFallbackOptionKey);
        SetEventStateMethod.Invoke(
            vakuu,
            [new LocString("ancients", descriptionKey), options]);
        MainFile.Logger.Info(
            options[0].TextKey == VictoryFallbackOptionKey
                ? "[EZMicroBalance] Vakuu fight victory had no unclaimed non-Vakuu Act 3 Ancient blessing options; using explicit fallback."
                : $"[EZMicroBalance] Vakuu fight victory resolved with {GetEncounter(combatRoom).BrokenLocks} broken locks, {options.Count} blessing choices, and {GetEncounter(combatRoom).VictoryGold} bonus Gold.");
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

        var encounter = ModelDb.Encounter<EzmbVakuuTrialEncounter>().ToMutable();
        var combatRoom = new CombatRoom(encounter, vakuu.Owner.RunState)
        {
            ShouldResumeParentEventAfterCombat = true
        };

        ClearEventNode(vakuu);
        MainFile.Logger.Info("[EZMicroBalance] Starting Vakuu fight encounter through the explicit parent-room stack transition.");
        await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true);
    }

    public static async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Monster is not EzmbVakuuTrialMonster ||
            creature.CombatState?.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        var remainingLocks = encounter.RemainingLocks;
        if (remainingLocks > 0)
        {
            await PowerCmd.Apply<VakuuStolenVaultPower>(
                new ThrowingPlayerChoiceContext(),
                creature,
                remainingLocks,
                null,
                null,
                silent: true);
        }
    }

    public static async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (result.UnblockedDamage <= 0 ||
            dealer?.IsPlayer != true ||
            target.Monster is not EzmbVakuuTrialMonster ||
            target.CombatState?.Encounter is not EzmbVakuuTrialEncounter encounter ||
            target.CombatState.CurrentSide != CombatSide.Player)
        {
            return;
        }

        var round = target.CombatState.RoundNumber;
        if (encounter.DamageRound != round)
        {
            encounter.DamageRound = round;
            encounter.DamageThisRound = 0m;
        }

        encounter.DamageThisRound += result.UnblockedDamage;
        if (encounter.DamageLockRound == round ||
            encounter.DamageThisRound < EzmbVakuuTrialEncounter.DamageLockThreshold)
        {
            return;
        }

        encounter.DamageLockRound = round;
        await BreakLock(choiceContext, target.CombatState, "damage threshold");
    }

    public static Creature? FindVakuuCreature(ICombatState? combatState) =>
        combatState?.Enemies.FirstOrDefault(enemy => enemy.Monster is EzmbVakuuTrialMonster);

    public static async Task SignContract(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        decimal hpLoss)
    {
        if (player.Creature.CombatState is not { } combatState ||
            combatState.Encounter is not EzmbVakuuTrialEncounter encounter)
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            player.Creature,
            hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            null,
            cardSource);

        encounter.BloodDebt++;
        var vakuu = FindVakuuCreature(combatState);
        if (vakuu != null)
        {
            await PowerCmd.Apply<VakuuBloodDebtPower>(
                choiceContext,
                vakuu,
                1m,
                player.Creature,
                cardSource);
        }

        await BreakLock(choiceContext, combatState, "contract");
        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu contract signed: Blood Debt {encounter.BloodDebt}, broken locks {encounter.BrokenLocks}.");
    }

    public static async Task ProceedFromNoRewardVictory(CombatRoom combatRoom)
    {
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu fight has no normal combat rewards; resuming the Vakuu event reward choice.");

        if (!combatRoom.IsPreFinished ||
            !combatRoom.ShouldResumeParentEventAfterCombat ||
            combatRoom.CombatState.RunState.CurrentRoomCount <= 1)
        {
            return;
        }

        await Cmd.Wait(1f);
        await RunManager.Instance.ProceedFromTerminalRewardsScreen();
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

    public static void ArmPrefinishedParentRestoreHealSkip(
        EventRoom eventRoom,
        IRunState? runState,
        bool isRestoringRoomStackBase)
    {
        if (!isRestoringRoomStackBase ||
            eventRoom.CanonicalEvent is not MegaCrit.Sts2.Core.Models.Events.Vakuu)
        {
            return;
        }

        pendingVakuuPrefinishedParentRestoreHealSkips = Math.Max(
            pendingVakuuPrefinishedParentRestoreHealSkips,
            Math.Max(1, runState?.Players.Count ?? 1));
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore armed duplicate Ancient heal skip for the parent event.");
    }

    public static bool ShouldSkipPrefinishedParentRestoreHeal(
        AncientEventModel ancient,
        bool isPreFinished)
    {
        if (isPreFinished ||
            pendingVakuuPrefinishedParentRestoreHealSkips <= 0 ||
            ancient is not MegaCrit.Sts2.Core.Models.Events.Vakuu)
        {
            return false;
        }

        pendingVakuuPrefinishedParentRestoreHealSkips--;
        MainFile.Logger.Info(
            "[EZMicroBalance] Vakuu pre-finished fight restore skipped duplicate Ancient heal on the reconstructed parent event.");
        return true;
    }

    private static IEnumerable<EventOption> CreateVictoryOptions(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu, CombatRoom combatRoom)
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
            .Select((VictoryRelicChoice choice) => EventOption.FromRelic(
                choice.Relic,
                vakuu,
                async () =>
                {
                    await choice.OnChosen();
                    if (encounter.VictoryGold > 0)
                    {
                        await PlayerCmd.GainGold(encounter.VictoryGold, owner);
                    }
                    vakuu.StartPreFinished();
                },
                $"EZMB_VAKUU_FIGHT.pages.VICTORY.options.{choice.Relic.Id.Entry}"))
            .ToList();

        return options.Count > 0 ? options : [CreateVictoryFallbackOption(vakuu, combatRoom)];
    }

    private static EventOption CreateVictoryFallbackOption(MegaCrit.Sts2.Core.Models.Events.Vakuu vakuu, CombatRoom combatRoom)
    {
        return new EventOption(
            vakuu,
            async () =>
            {
                if (vakuu.Owner != null)
                {
                    var gold = GetEncounter(combatRoom).VictoryGold;
                    if (gold > 0)
                    {
                        await PlayerCmd.GainGold(gold, vakuu.Owner);
                    }
                }

                vakuu.StartPreFinished();
            },
            VictoryFallbackOptionKey);
    }

    private static async Task BreakLock(PlayerChoiceContext choiceContext, ICombatState combatState, string source)
    {
        if (combatState.Encounter is not EzmbVakuuTrialEncounter encounter ||
            encounter.RemainingLocks <= 0)
        {
            return;
        }

        encounter.BrokenLocks++;
        var vakuu = FindVakuuCreature(combatState);
        var vault = vakuu?.GetPower<VakuuStolenVaultPower>();
        if (vault != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, vault, -1m, null, null);
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu Stolen Vault lock broken by {source}: {encounter.BrokenLocks}/{EzmbVakuuTrialEncounter.MaxLocks}.");
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

    private static void ClearEventNode(EventModel eventModel) =>
        EventNodeBackingField.SetValue(eventModel, null);

    private static IEnumerable<VictoryRelicChoice> GetNonVakuuAct3AncientRewardChoices(Player owner)
    {
        return GetSourceAct3AncientRelicChoices(owner)
            .Concat(GetLothaAct3AncientRelicChoices(owner));
    }

    private static IEnumerable<VictoryRelicChoice> GetSourceAct3AncientRelicChoices(Player owner)
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
            .Where(relic => IsEligibleSourceAncientReward(owner, relic))
            .Where(relic => owner.GetRelicById(relic.Id) is null)
            .Select(relic =>
            {
                var mutableRelic = relic.ToMutable();
                return new VictoryRelicChoice(
                    mutableRelic,
                    async () => await RelicCmd.Obtain(mutableRelic, owner));
            });
    }

    private static bool IsEligibleSourceAncientReward(Player owner, RelicModel relic)
    {
        if (relic is BeautifulBracelet)
        {
            return owner.Deck.Cards.Count(ModelDb.Enchantment<Swift>().CanEnchant) >= 4;
        }

        if (relic is TriBoomerang)
        {
            return owner.Deck.Cards.Count(card => ModelDb.Enchantment<Instinct>().CanEnchant(card)) >= 3;
        }

        return true;
    }

    private static IEnumerable<VictoryRelicChoice> GetLothaAct3AncientRelicChoices(Player owner)
    {
        if (!LothaFeatureGate.IsLothaEnabled(owner.RunState.UnlockState) ||
            !string.IsNullOrWhiteSpace(LothaBlessingService.GetSelectedBlessing(owner)))
        {
            return [];
        }

        return
        [
            .. LothaChoice<LothaMirrorRebuttalOptionRelic>(owner, LothaBlessingIds.MirrorRebuttal),
            .. LothaChoice<LothaMirrorHallEchoOptionRelic>(owner, LothaBlessingIds.MirrorHallEcho),
            .. LothaChoice<LothaPresumptionOptionRelic>(owner, LothaBlessingIds.Presumption),
            .. LothaChoice<LothaClosedCourtOptionRelic>(owner, LothaBlessingIds.ClosedCourt),
            .. LothaChoice<LothaDeferredVerdictOptionRelic>(owner, LothaBlessingIds.DeferredVerdict),
            .. LothaChoice<LothaDeathReprieveOptionRelic>(owner, LothaBlessingIds.DeathReprieve),
            .. LothaChoice<LothaSingleSentenceOptionRelic>(owner, LothaBlessingIds.SingleSentence),
            .. LothaChoice<LothaPublicEvidenceOptionRelic>(owner, LothaBlessingIds.PublicEvidence)
        ];
    }

    private static IEnumerable<VictoryRelicChoice> LothaChoice<T>(Player owner, string blessingId)
        where T : RelicModel
    {
        if (owner.GetRelic<T>() is not null)
        {
            return [];
        }

        var relic = ModelDb.Relic<T>().ToMutable();
        relic.Owner = owner;
        return
        [
            new VictoryRelicChoice(
                relic,
                () => LothaRewardSelectionService.SelectBlessing<T>(owner, blessingId))
        ];
    }
}

internal static class VakuuFightAssetPaths
{
    public static string OptionIcon => $"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png";

    public static string MonsterVisual => $"{MainFile.ResPath}/images/monsters/vakuu_trial.png";

    public static string EncounterScene => $"{MainFile.ResPath}/scenes/encounters/ezmb_vakuu_trial.tscn";

    public static string PowerIcon => OptionIcon;
}
