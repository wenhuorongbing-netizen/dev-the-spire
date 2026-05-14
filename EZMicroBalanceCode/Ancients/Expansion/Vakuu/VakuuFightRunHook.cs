using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuFightInitializer
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        ModHelper.SubscribeForRunStateHooks(
            $"{MainFile.ModId}.VakuuFight.RunHooks",
            CreateRunHookSubscribers);

        MainFile.Logger.Info(
            $"[EZMicroBalance] Vakuu fight hooks registered default-on for single-player; set {VakuuFightFeatureGate.DisableEnvironmentVariable}=1 or {VakuuFightFeatureGate.SpirePlusDisableEnvironmentVariable}=1 to disable.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        VakuuFightFeatureGate.IsFightEnabledForRun(runState)
            ? [ModelDb.GetById<VakuuFightRunHook>(ModelDb.GetId<VakuuFightRunHook>())]
            : [];
}

internal sealed class VakuuFightRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) =>
        VakuuTemptationService.AfterPlayerTurnStart(choiceContext, player);
}

internal static class VakuuTemptationService
{
    private const int FirstTemptationTurn = 1;
    private const int TemptationTurnCadence = 2;

    private static readonly ConditionalWeakTable<ICombatState, CombatTemptationState> CombatStates = new();

    private sealed class CombatTemptationState
    {
        public HashSet<int> InjectedRounds { get; } = [];
    }

    public static async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (!player.IsActiveForHooks ||
            player.Creature.CombatState is not { } combatState ||
            !IsVakuuTrialCombat(combatState) ||
            combatState.RunState.Players.Count != 1)
        {
            return;
        }

        var round = combatState.RoundNumber;
        if (round < FirstTemptationTurn ||
            (round - FirstTemptationTurn) % TemptationTurnCadence != 0)
        {
            return;
        }

        var state = CombatStates.GetOrCreateValue(combatState);
        if (!state.InjectedRounds.Add(round))
        {
            return;
        }

        var temptation = combatState.CreateCard<VakuuTemptation>(player);
        var result = await AncientCardHelpers.TryAddGeneratedCardToCombat(
            temptation,
            PileType.Draw,
            player,
            CardPilePosition.Top);

        if (result?.success == true)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Vakuu fight added Temptation to the top of the draw pile on player turn {round}.");
        }
        else
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance] Vakuu fight could not add Temptation on player turn {round}; generated card was cleaned up.");
        }
    }

    private static bool IsVakuuTrialCombat(ICombatState combatState) =>
        combatState.Encounter is EzmbVakuuTrialEncounter;
}
