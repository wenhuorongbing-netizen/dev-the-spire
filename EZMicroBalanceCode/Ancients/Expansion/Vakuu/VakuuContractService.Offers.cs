using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal static partial class VakuuContractService
{
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
}
