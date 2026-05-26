namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    internal static IReadOnlyList<CardModel> GetSeedBankStoredCards(Player player) =>
        GetSeedBankCardIds(GetProgress(player))
            .Select(TryGetStoredCard)
            .OfType<CardModel>()
            .ToList();

    internal static int GetSeedBankStoredCount(Player player) =>
        GetSeedBankCardIds(GetProgress(player)).Count;

    internal static bool IsSeedBankSettled(Player player) =>
        GetProgress(player).SeedBankSettled;

    private static List<string> GetSeedBankCardIds(UrdaProgress progress) =>
        SplitList(progress.SeedBankCardIds, ',').Take(SeedBankMaxSeeds).ToList();

    private static CardModel? TryGetStoredCard(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (id.Contains('.', StringComparison.Ordinal))
        {
            try
            {
                return ModelDb.GetByIdOrNull<CardModel>(ModelId.Deserialize(id));
            }
            catch
            {
                return null;
            }
        }

        return ModelDb.AllCards.FirstOrDefault(card => card.Id.Entry == id);
    }

    private static void RefreshSeedBankRelicStatus(Player player)
    {
        var relic = player.Relics.OfType<UrdaSeedBankOptionRelic>().FirstOrDefault();
        if (relic == null)
        {
            return;
        }

        var progress = GetProgress(player);
        var storedCount = GetSeedBankCardIds(progress).Count;
        relic.Status = progress.SeedBankSettled
            ? RelicStatus.Disabled
            : storedCount > 0
                ? RelicStatus.Active
                : RelicStatus.Normal;
        relic.RefreshStoredSeedDisplay();
    }
}
