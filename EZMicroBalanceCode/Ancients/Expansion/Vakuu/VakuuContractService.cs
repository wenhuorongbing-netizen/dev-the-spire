using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuContractService
{
    private const int FirstContractTurn = 1;
    private const int ContractTurnCadence = 2;
    private const int LastContractOfferTurn = 5;
    private const int ContractOfferCount = 3;

    private static readonly Type[] ContractTypes =
    [
        typeof(VakuuKnifeContract),
        typeof(VakuuTemptation),
        typeof(VakuuShelterContract),
        typeof(VakuuTrickContract)
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
            round > LastContractOfferTurn ||
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
                $"[Spire Plus] Vakuu fight skipped a Contract on player turn {round} because the hand is full.");
            return;
        }

        var selected = await ChooseContract(choiceContext, player, combatState, includeCashOut: false);
        if (selected == null)
        {
            return;
        }

        var result = await AncientCardHelpers.TryAddGeneratedCardToCombat(selected, PileType.Hand, player);
        if (result?.success == true)
        {
            MainFile.Logger.Info(
                $"[Spire Plus] Vakuu fight added a chosen Contract to hand on player turn {round}.");
        }
        else
        {
            MainFile.Logger.Warn(
                $"[Spire Plus] Vakuu fight could not add a Contract on player turn {round}; generated card was cleaned up.");
        }
    }

    private static bool IsVakuuTrialCombat(ICombatState combatState) =>
        combatState.Encounter is EzmbVakuuTrialEncounter;
}
