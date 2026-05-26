using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static class VakuuContractService
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

    public static async Task OfferCashOutAfterLockBreak(
        PlayerChoiceContext choiceContext,
        ICombatState combatState,
        EzmbVakuuTrialEncounter encounter)
    {
        var player = combatState.Players.FirstOrDefault(player => player.IsActiveForHooks);
        if (player == null ||
            combatState.RunState.Players.Count != 1 ||
            encounter.BrokenLocks <= 0 ||
            encounter.CashOutOfferedLock >= encounter.BrokenLocks)
        {
            return;
        }

        var contract = combatState.CreateCard(
            ModelDb.GetById<CardModel>(ModelDb.GetId<VakuuCashOutContract>()),
            player);
        if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
        {
            await OfferImmediateCashOutChoice(choiceContext, player, combatState, encounter, contract);
            return;
        }

        var result = await AncientCardHelpers.TryAddGeneratedCardToCombat(contract, PileType.Hand, player);
        if (result?.success == true)
        {
            encounter.CashOutOfferedLock = encounter.BrokenLocks;
            MainFile.Logger.Info(
                $"[Spire Plus] Vakuu fight offered Cash Out after lock {encounter.BrokenLocks}.");
        }
    }

    private static async Task OfferImmediateCashOutChoice(
        PlayerChoiceContext choiceContext,
        Player player,
        ICombatState combatState,
        EzmbVakuuTrialEncounter encounter,
        CardModel contract)
    {
        encounter.CashOutOfferedLock = encounter.BrokenLocks;
        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [contract],
            player,
            new CardSelectorPrefs(new LocString("cards", "EZMB_VAKUU_CASH_OUT.selectionScreenPrompt"), 0, 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();

        if (selected == contract)
        {
            await VakuuFightService.CashOut(choiceContext, player, contract);
        }

        AncientCardHelpers.RemoveUnpiledCombatCard(contract, combatState);
    }

    private static async Task<CardModel?> ChooseContract(
        PlayerChoiceContext choiceContext,
        Player player,
        ICombatState combatState,
        bool includeCashOut)
    {
        var offerTypes = ContractTypes
            .ToList()
            .UnstableShuffle(player.RunState.Rng.CombatCardSelection)
            .Take(ContractOfferCount)
            .ToList();
        if (includeCashOut)
        {
            offerTypes.Insert(0, typeof(VakuuCashOutContract));
        }

        var offers = offerTypes
            .Select(type => combatState.CreateCard(GetContractModel(type), player))
            .ToList();
        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            offers,
            player,
            new CardSelectorPrefs(new LocString("cards", "EZMB_VAKUU_CONTRACT.selectionScreenPrompt"), 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();

        foreach (var offer in offers.Where(offer => offer != selected))
        {
            AncientCardHelpers.RemoveUnpiledCombatCard(offer, combatState);
        }

        return selected;
    }

    private static CardModel GetContractModel(Type type)
    {
        if (type == typeof(VakuuKnifeContract))
        {
            return ModelDb.Card<VakuuKnifeContract>();
        }

        if (type == typeof(VakuuTemptation))
        {
            return ModelDb.Card<VakuuTemptation>();
        }

        if (type == typeof(VakuuShelterContract))
        {
            return ModelDb.Card<VakuuShelterContract>();
        }

        if (type == typeof(VakuuTrickContract))
        {
            return ModelDb.Card<VakuuTrickContract>();
        }

        return ModelDb.Card<VakuuCashOutContract>();
    }

    private static bool IsVakuuTrialCombat(ICombatState combatState) =>
        combatState.Encounter is EzmbVakuuTrialEncounter;
}
