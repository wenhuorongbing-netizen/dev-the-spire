using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static async Task CommitSeedBankSelectedCards(
        Player player,
        UrdaProgress progress,
        IReadOnlyList<CardModel> selected)
    {
        var addedCount = 0;
        var failedSelectedIds = new List<string>();
        for (var i = 0; i < selected.Count && i < SeedBankMaxSettlementCards; i++)
        {
            var card = selected[i];
            if (i == 0 && card.IsUpgradable)
            {
                CardCmd.Upgrade(card, CardPreviewStyle.None);
            }

            var addResult = await CardPileCmd.Add(card, PileType.Deck);
            if (addResult.success)
            {
                addedCount++;
                SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedBankOptionRelic>(), 2f);
                ReleaseEvidenceLog.Log(
                    "UrdaSeedBank",
                    "deck_add_success",
                    player,
                    new Dictionary<string, object?>
                    {
                        ["card"] = card.Id.Entry
                    });
            }
            else
            {
                failedSelectedIds.Add(card.Id.ToString());
                AncientCardHelpers.RemoveUnpiledRunCard(card);
                ReleaseEvidenceLog.Log(
                    "UrdaSeedBank",
                    "deck_add_failed",
                    player,
                    new Dictionary<string, object?>
                    {
                        ["card"] = card.Id.Entry
                    });
            }
        }

        if (failedSelectedIds.Count > 0)
        {
            SetProgress(player, addedCount > 0
                ? progress with { SeedBankCardIds = string.Join(",", failedSelectedIds.Take(SeedBankMaxSeeds)) }
                : progress);
            RefreshSeedBankRelicStatus(player);
            MainFile.Logger.Warn(
                $"[Spire Plus] Urda Seed Bank extraction preserved {failedSelectedIds.Count} Seed card(s) because they could not be added to the deck.");
            return;
        }

        SetProgress(player, progress with
        {
            SeedBankCardIds = string.Empty,
            SeedBankSettled = true
        });
        RefreshSeedBankRelicStatus(player);
        ReleaseEvidenceLog.Log(
            "UrdaSeedBank",
            "storage_cleared",
            player,
            new Dictionary<string, object?>
            {
                ["added"] = addedCount
            });
        ReleaseEvidenceLog.Log(
            "UrdaSeedBank",
            "extracted_by_relic_click",
            player,
            new Dictionary<string, object?>
            {
                ["added"] = addedCount
            });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Seed Bank extracted by relic click: added {addedCount} Seed card(s).");
    }
}
