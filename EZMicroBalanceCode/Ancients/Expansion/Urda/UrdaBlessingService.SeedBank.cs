using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int SeedBankMaxSeeds = 3;
    private const int SeedBankMaxSettlementCards = 2;

    private static bool TryAddSeedBankAlternative(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.SeedBankSettled || GetSeedBankCardIds(progress).Count >= SeedBankMaxSeeds)
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_SEED_BANK_STORE",
            () => ChooseSeedBankStore(player, cardReward),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private static async Task ChooseSeedBankStore(Player player, CardReward reward)
    {
        if (!IsTrackedNormalActOneCombatReward(reward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.SeedBank)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.SeedBankHandled)
        {
            return;
        }

        context.SeedBankHandled = true;
        var progress = GetProgress(player);
        var seedIds = GetSeedBankCardIds(progress);
        if (progress.SeedBankSettled || seedIds.Count >= SeedBankMaxSeeds)
        {
            return;
        }

        var rewardCards = reward.Cards.ToList();
        if (rewardCards.Count == 0)
        {
            return;
        }

        var selected = rewardCards.Count == 1
            ? rewardCards[0]
            : (await CardSelectCmd.FromSimpleGrid(
                new BlockingPlayerChoiceContext(),
                rewardCards,
                player,
                new CardSelectorPrefs(UrdaLoc("urda_seed_bank.storeSelectionPrompt"), 1))).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        seedIds.Add(selected.Id.ToString());
        progress = progress with { SeedBankCardIds = string.Join(",", seedIds.Take(SeedBankMaxSeeds)) };
        SetProgress(player, progress);
        RefreshSeedBankRelicStatus(player);
        ReleaseEvidenceLog.Log(
            "UrdaSeedBank",
            "card_stored",
            player,
            new Dictionary<string, object?>
            {
                ["card"] = selected.Id.Entry,
                ["stored"] = seedIds.Count
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seed Bank stored {selected.Id.Entry}; stored {seedIds.Count}/{SeedBankMaxSeeds}. The source-safe slice consumes this card reward to store the Seed.");
    }
}
