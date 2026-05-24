using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int MaxSeedbedChecks = 4;
    private const int SeedbedMaxHpCost = 2;
    private const int SeedbedCompletionMaxHpGain = 10;

    private static bool TryAddSeedbedAlternative(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.SeedbedTransformed ||
            progress.SeedbedAccepted >= MaxSeedbedChecks ||
            !CanPaySeedbedCost(player))
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_SEEDBED",
            () => AcceptSeedbed(player, cardReward),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private static async Task AcceptSeedbed(Player player, CardReward cardReward)
    {
        if (!CardRewardContexts.TryGetValue(cardReward, out var context) ||
            !context.IsNormalActOneCombatCardReward)
        {
            return;
        }

        var progress = GetProgress(player);
        if (progress.SeedbedTransformed ||
            progress.SeedbedAccepted >= MaxSeedbedChecks ||
            !CanPaySeedbedCost(player))
        {
            return;
        }

        if (context.SeedbedHandled)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Seedbed duplicate reward alternative click ignored.");
            return;
        }

        context.SeedbedHandled = true;

        var seedbed = player.RunState.CreateCard<UrdaSeedbed>(player);
        if (progress.SeedbedAccepted == 0 && seedbed.IsUpgradable)
        {
            CardCmd.Upgrade(seedbed);
        }

        var addResult = await CardPileCmd.Add(seedbed, PileType.Deck);
        if (addResult.success)
        {
            CardCmd.PreviewCardPileAdd(addResult, 2f);
        }
        else
        {
            AncientCardHelpers.RemoveUnpiledRunCard(seedbed);
            MainFile.Logger.Warn("[Spire Plus] Urda Seedbed was not added to deck; cost and progress were not applied.");
            return;
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), player.Creature, SeedbedMaxHpCost, isFromCard: false);
        progress = progress with
        {
            SeedbedChecks = progress.SeedbedChecks + 1,
            SeedbedAccepted = progress.SeedbedAccepted + 1
        };
        if (progress.SeedbedAccepted >= MaxSeedbedChecks)
        {
            progress = progress with { SeedbedTransformed = true };
            await CreatureCmd.SetMaxHp(player.Creature, player.Creature.MaxHp + SeedbedCompletionMaxHpGain);
        }

        SetProgress(player, progress);
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seedbed applied: accepted {progress.SeedbedAccepted}/{MaxSeedbedChecks}; transformed={progress.SeedbedTransformed}.");
    }

    private static bool CanPaySeedbedCost(Player player) =>
        player.Creature.MaxHp > SeedbedMaxHpCost;
}
