using EZMicroBalance.EZMicroBalanceCode.Ancients;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private sealed class SeedBankExtractionState
    {
        public bool InProgress { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, SeedBankExtractionState> SeedBankExtractionInProgress = new();

    public static async Task TryExtractSeedBankFromRelicClick(Player player)
    {
        var extractionState = SeedBankExtractionInProgress.GetOrCreateValue(player);
        if (extractionState.InProgress)
        {
            MainFile.Logger.Info("[EZMicroBalance] Urda Seed Bank extraction ignored: a Seed Bank selection is already open.");
            return;
        }

        extractionState.InProgress = true;
        try
        {
            await TryExtractSeedBankFromRelicClickOnce(player);
        }
        finally
        {
            extractionState.InProgress = false;
        }
    }

    private static async Task TryExtractSeedBankFromRelicClickOnce(Player player)
    {
        if (player.RunState.Players.Count > 1)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Seed Bank relic extraction is single-player only until host-authoritative reward selection sync is implemented.");
            return;
        }

        var progress = GetProgress(player);
        var seedIds = GetSeedBankCardIds(progress);
        if (progress.SeedBankSettled || seedIds.Count == 0)
        {
            return;
        }

        var cards = seedIds
            .Select(TryGetStoredCard)
            .OfType<CardModel>()
            .Select(card => player.RunState.CreateCard(card, player))
            .ToList();
        if (cards.Count == 0)
        {
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

                MainFile.Logger.Info("[EZMicroBalance] Urda Seed Bank extraction was canceled; stored Seeds remain available.");
                return;
            }

            foreach (var unchosen in cards.Where(card => !selected.Contains(card)))
            {
                AncientCardHelpers.RemoveUnpiledRunCard(unchosen);
            }

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
                    CardCmd.PreviewCardPileAdd(addResult, 2f);
                }
                else
                {
                    failedSelectedIds.Add(card.Id.ToString());
                    AncientCardHelpers.RemoveUnpiledRunCard(card);
                }
            }

            if (failedSelectedIds.Count > 0)
            {
                SetProgress(player, addedCount > 0
                    ? progress with { SeedBankCardIds = string.Join(",", failedSelectedIds.Take(SeedBankMaxSeeds)) }
                    : progress);
                RefreshSeedBankRelicStatus(player);
                MainFile.Logger.Warn(
                    $"[EZMicroBalance] Urda Seed Bank extraction preserved {failedSelectedIds.Count} Seed card(s) because they could not be added to the deck.");
                return;
            }

            SetProgress(player, progress with
            {
                SeedBankCardIds = string.Empty,
                SeedBankSettled = true
            });
            RefreshSeedBankRelicStatus(player);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Urda Seed Bank extracted by relic click: added {addedCount} Seed card(s).");
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
