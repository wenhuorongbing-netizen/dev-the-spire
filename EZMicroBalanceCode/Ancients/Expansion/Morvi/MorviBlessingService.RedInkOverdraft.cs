using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.Gold;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int RedInkOverdraftDraw = 2;
    private const int RedInkOverdraftEnergy = 1;
    private const int RedInkOverdraftGoldPerDebt = 12;
    private const int RedInkOverdraftHpPerUnpaidDebt = 3;

    public static bool CanUseRedInkOverdraft(Player player)
    {
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != MorviBlessingIds.RedInkOverdraft ||
            player.PlayerCombatState?.Energy != 0)
        {
            return false;
        }

        return !CombatStates.GetOrCreateValue(player).RedInkUsedThisTurn;
    }

    public static async Task UseRedInkOverdraft(PlayerChoiceContext choiceContext, Player player)
    {
        if (!CanUseRedInkOverdraft(player))
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        combatState.RedInkUsedThisTurn = true;
        combatState.RedInkDebtsThisCombat++;
        await CardPileCmd.Draw(choiceContext, RedInkOverdraftDraw, player);
        await PlayerCmd.GainEnergy(RedInkOverdraftEnergy, player);
        await SetCounterPower<MorviOverdraftPower>(choiceContext, player, combatState.RedInkDebtsThisCombat);
        MainFile.Logger.Info($"[Spire Plus] Morvi Red Ink Overdraft recorded debt {combatState.RedInkDebtsThisCombat} this combat.");
    }

    private static async Task AddRedInkOverdraftCard(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        var hand = PileType.Hand.GetPile(player);
        if (hand.Cards.Count >= CardPile.MaxCardsInHand)
        {
            MainFile.Logger.Info("[Spire Plus] Morvi Red Ink Overdraft skipped this turn because the hand is full.");
            return;
        }

        var card = player.Creature.CombatState.CreateCard<MorviRedInkOverdraftCard>(player);
        var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(card, PileType.Hand, player);
        if (addResult is not { success: true } result)
        {
            return;
        }

        if (result.cardAdded.Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.RemoveFromCombat(result.cardAdded, skipVisuals: true);
            MainFile.Logger.Warn("[Spire Plus] Morvi Red Ink Overdraft generated card did not land in hand and was removed to avoid combat-pile flooding.");
        }
    }

    private static async Task PayRedInkOverdraftDebts(Player player, MorviCombatState combatState)
    {
        var visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0;
        var debtCount = Math.Max(combatState.RedInkDebtsThisCombat, visibleDebtCount);
        if (debtCount <= 0)
        {
            return;
        }

        for (var index = 0; index < debtCount; index++)
        {
            if (player.Gold >= RedInkOverdraftGoldPerDebt)
            {
                await PlayerCmd.LoseGold(RedInkOverdraftGoldPerDebt, player, GoldLossType.Spent);
                continue;
            }

            await DamagePlayerNonlethal(player, RedInkOverdraftHpPerUnpaidDebt);
        }

        MainFile.Logger.Info($"[Spire Plus] Morvi Red Ink Overdraft settled {debtCount} combat debt(s).");
        combatState.RedInkDebtsThisCombat = 0;
        await SetCounterPower<MorviOverdraftPower>(new ThrowingPlayerChoiceContext(), player, 0);
    }
}
