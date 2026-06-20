using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static async Task TryExtractSeedBankFromRelicClickOnce(Player player)
    {
        var hasMultiplayerRunState = player.RunState.Players.Count > 1;
        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
            player.RunState,
            "UrdaSeedBank",
            "Seed Bank extraction mutates deck from a local relic click") ||
            hasMultiplayerRunState)
        {
            MainFile.Logger.Warn("[Spire Plus] Urda Seed Bank relic extraction is single-player only until host-authoritative reward selection sync is implemented.");
            return;
        }

        var progress = GetProgress(player);
        var seedIds = GetSeedBankCardIds(progress);
        if (progress.SeedBankSettled || seedIds.Count == 0)
        {
            ReleaseEvidenceLog.Log(
                "UrdaSeedBank",
                "settlement_empty",
                player,
                new Dictionary<string, object?>
                {
                    ["stored"] = seedIds.Count,
                    ["settled"] = progress.SeedBankSettled
                });
            return;
        }

        ReleaseEvidenceLog.Log(
            "UrdaSeedBank",
            "extract_opened",
            player,
            new Dictionary<string, object?>
            {
                ["stored"] = seedIds.Count,
                ["settled"] = progress.SeedBankSettled
            });

        var cards = seedIds
            .Select(TryGetStoredCard)
            .OfType<CardModel>()
            .Select(card => player.RunState.CreateCard(card, player))
            .ToList();
        if (cards.Count == 0)
        {
            ReleaseEvidenceLog.Log(
                "UrdaSeedBank",
                "settlement_empty",
                player,
                new Dictionary<string, object?>
                {
                    ["stored"] = seedIds.Count,
                    ["resolvedCards"] = 0
                });
            SetProgress(player, progress with
            {
                SeedBankCardIds = string.Empty,
                SeedBankSettled = true
            });
            RefreshSeedBankRelicStatus(player);
            return;
        }

        try
        {
            var selected = (await CardSelectCmd.FromSimpleGrid(
                new BlockingPlayerChoiceContext(),
                cards,
                player,
                new CardSelectorPrefs(UrdaLoc("urda_seed_bank.settlementSelectionPrompt"), 0, Math.Min(SeedBankMaxSettlementCards, cards.Count))
                {
                    Cancelable = true,
                    RequireManualConfirmation = true
                })).ToList();

            if (selected.Count == 0)
            {
                foreach (var card in cards)
                {
                    AncientCardHelpers.RemoveUnpiledRunCard(card);
                }

                ReleaseEvidenceLog.Log(
                    "UrdaSeedBank",
                    "selection_cancelled",
                    player,
                    new Dictionary<string, object?>
                    {
                        ["stored"] = seedIds.Count
                    });
                MainFile.Logger.Info("[Spire Plus] Urda Seed Bank extraction was canceled; stored Seeds remain available.");
                return;
            }

            ReleaseEvidenceLog.Log(
                "UrdaSeedBank",
                "cards_selected",
                player,
                new Dictionary<string, object?>
                {
                    ["selected"] = selected.Count
                });

            foreach (var unchosen in cards.Where(card => !selected.Contains(card)))
            {
                AncientCardHelpers.RemoveUnpiledRunCard(unchosen);
            }

            await CommitSeedBankSelectedCards(player, progress, selected);
        }
        finally
        {
            foreach (var card in cards)
            {
                AncientCardHelpers.RemoveUnpiledRunCard(card);
            }
        }
    }
}
