namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int ShallowRootRelicChoices = 2;
    private const int ShallowRootInitialGold = 75;
    private const int ShallowRootEliteGold = 35;

    public static async Task ApplyShallowRootRelic(Player player)
    {
        var relics = new List<RelicModel>();
        for (var i = 0; i < ShallowRootRelicChoices; i++)
        {
            var relic = RelicFactory.PullNextRelicFromFront(
                player,
                RelicRarity.Common,
                candidate => relics.All(existing => existing.Id != candidate.Id)).ToMutable();
            relics.Add(relic);
        }

        var selected = await RelicSelectCmd.FromChooseARelicScreen(player, relics);
        if (selected == null)
        {
            return;
        }

        await RelicCmd.Obtain(selected, player);
        await PlayerCmd.GainGold(ShallowRootInitialGold, player);
        SetProgress(player, GetProgress(player) with
        {
            ShallowRelicPending = true,
            ShallowRelicRooted = false,
            ShallowRelicId = selected.Id.ToString()
        });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Shallow-Root Relic granted {selected.Id.Entry} and {ShallowRootInitialGold} Gold.");
    }

    private static async Task RootShallowRelicFromElite(Player player)
    {
        var progress = GetProgress(player);
        if (!progress.ShallowRelicPending || progress.ShallowRelicRooted)
        {
            return;
        }

        progress = progress with
        {
            ShallowRelicPending = false,
            ShallowRelicRooted = true
        };
        SetProgress(player, progress);
        await PlayerCmd.GainGold(ShallowRootEliteGold, player);
        MainFile.Logger.Info($"[Spire Plus] Urda Shallow-Root Relic rooted after Act 1 Elite; gained {ShallowRootEliteGold} Gold.");
    }

    private static async Task SettleUnrootedShallowRelicAtActTwo(Player player)
    {
        var progress = GetProgress(player);
        if (!progress.ShallowRelicPending || progress.ShallowRelicRooted)
        {
            return;
        }

        var relic = FindRelicById(player, progress.ShallowRelicId);
        if (relic != null)
        {
            await RelicCmd.Remove(relic);
        }

        await PlayerCmd.GainGold(ShallowRootInitialGold, player);
        SetProgress(player, progress with
        {
            ShallowRelicPending = false,
            ShallowRelicRooted = false
        });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Shallow-Root Relic Act 2 fallback settled: removed pending relic and refunded {ShallowRootInitialGold} Gold.");
    }

    private static RelicModel? FindRelicById(Player player, string id) =>
        player.Relics.FirstOrDefault(relic =>
            relic.Id.ToString().Equals(id, StringComparison.Ordinal) ||
            relic.Id.Entry.Equals(id, StringComparison.Ordinal));
}
