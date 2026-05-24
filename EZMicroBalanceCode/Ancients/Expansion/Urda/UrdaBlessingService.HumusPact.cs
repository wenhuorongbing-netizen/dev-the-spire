using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int HumusGoldPerSkip = 15;
    private const int HumusRequiredSkips = 3;

    public static async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (reward is not CardReward ||
            GetSelectedBlessing(player) != UrdaBlessingIds.HumusPact)
        {
            return;
        }

        var progress = GetProgress(player);
        if (!progress.HumusCompletionPending)
        {
            return;
        }

        var resolved = await ResolveHumusCompletion(player);
        if (!resolved)
        {
            return;
        }

        progress = GetProgress(player) with { HumusCompletionPending = false };
        SetProgress(player, progress);
    }

    private static bool TryAddHumusPactAlternative(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.HumusCompleted || progress.HumusCompletionPending)
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_HUMUS_PACT",
            () => ChooseHumusPact(player, cardReward),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private static async Task ChooseHumusPact(Player player, CardReward reward)
    {
        if (!IsTrackedNormalActOneCombatReward(reward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.HumusPact)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.HumusPactHandled)
        {
            return;
        }

        context.HumusPactHandled = true;
        var progress = GetProgress(player);
        if (progress.HumusCompleted || progress.HumusCompletionPending)
        {
            return;
        }

        progress = progress with { HumusSkips = progress.HumusSkips + 1 };
        if (progress.HumusSkips >= HumusRequiredSkips)
        {
            progress = progress with { HumusCompleted = true, HumusCompletionPending = true };
        }

        SetProgress(player, progress);
        await PlayerCmd.GainGold(HumusGoldPerSkip, player);
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Humus Pact applied: composted normal combat card reward {progress.HumusSkips}/{HumusRequiredSkips}; gained {HumusGoldPerSkip} gold.");
    }

    private static async Task<bool> ResolveHumusCompletion(Player player)
    {
        var rewardCard = CreateRandomRewardCard(player);
        if (rewardCard == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Humus Pact deferred upgraded card reward: no valid reward card could be generated.");
            return false;
        }

        var removalPrefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, 2)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };

        var selectedRemovals = (await CardSelectCmd.FromDeckForRemoval(player, removalPrefs)).ToList();
        foreach (var card in selectedRemovals)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        if (rewardCard.IsUpgradable)
        {
            CardCmd.Upgrade(rewardCard);
        }

        await new RewardsSet(player)
            .WithCustomRewards([new SpecialCardReward(rewardCard, player)])
            .WithSkippingDisallowed()
            .Offer();
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Humus Pact completed: removed {selectedRemovals.Count} card(s) and offered upgraded {rewardCard.Id.Entry}.");
        return true;
    }

    private static CardModel? CreateRandomRewardCard(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, 1, options).FirstOrDefault()?.Card;
    }
}
