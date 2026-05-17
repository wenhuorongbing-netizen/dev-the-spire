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
            $"[EZMicroBalance] Vakuu fight hooks registered but hidden by default; set {VakuuFightFeatureGate.EnableEnvironmentVariable}=1 or {VakuuFightFeatureGate.SpirePlusEnableEnvironmentVariable}=1 to opt in, or {VakuuFightFeatureGate.ForceFightEnvironmentVariable}=1 / {VakuuFightFeatureGate.SpirePlusForceFightEnvironmentVariable}=1 for focused debugging.");
    }

    private static IEnumerable<AbstractModel> CreateRunHookSubscribers(RunState runState) =>
        VakuuFightFeatureGate.IsFightEnabledForRun(runState)
            ? [ModelDb.GetById<VakuuFightRunHook>(ModelDb.GetId<VakuuFightRunHook>())]
            : [];
}

internal sealed class VakuuFightRunHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterCreatureAddedToCombat(Creature creature) =>
        VakuuFightService.AfterCreatureAddedToCombat(creature);

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        VakuuFightService.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) =>
        VakuuContractService.AfterPlayerTurnStart(choiceContext, player);
}

internal static class VakuuContractService
{
    private const int FirstContractTurn = 1;
    private const int ContractTurnCadence = 2;

    private static readonly Type[] ContractTypes =
    [
        typeof(VakuuKnifeContract),
        typeof(VakuuTemptation),
        typeof(VakuuShelterContract)
    ];

    private static readonly ConditionalWeakTable<ICombatState, CombatContractState> CombatStates = new();

    private sealed class CombatContractState
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
        if (round < FirstContractTurn ||
            (round - FirstContractTurn) % ContractTurnCadence != 0)
        {
            return;
        }

        var state = CombatStates.GetOrCreateValue(combatState);
        if (!state.InjectedRounds.Add(round))
        {
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Vakuu fight skipped a Contract on player turn {round} because the hand is full.");
            return;
        }

        var contractType = player.RunState.Rng.CombatCardSelection.NextItem(ContractTypes) ?? typeof(VakuuTemptation);
        var contract = combatState.CreateCard(ModelDb.GetById<CardModel>(ModelDb.GetId(contractType)), player);
        var result = await AncientCardHelpers.TryAddGeneratedCardToCombat(
            contract,
            PileType.Hand,
            player);

        if (result?.success == true)
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Vakuu fight added a Contract to hand on player turn {round}.");
        }
        else
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance] Vakuu fight could not add a Contract on player turn {round}; generated card was cleaned up.");
        }
    }

    private static bool IsVakuuTrialCombat(ICombatState combatState) =>
        combatState.Encounter is EzmbVakuuTrialEncounter;
}
