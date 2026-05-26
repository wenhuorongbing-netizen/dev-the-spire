using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int RootedRouteCardRewards = 3;

    private static async Task TryResolveRootedRouteReward(Player player)
    {
        var progress = GetProgress(player);
        if (progress.RootedRouteResolved ||
            progress.RootedRouteWithered ||
            string.IsNullOrWhiteSpace(progress.RootedRouteCoord) ||
            player.RunState.CurrentMapPoint is not { } current ||
            !SameCoordString(current.coord, progress.RootedRouteCoord))
        {
            return;
        }

        var cards = CreateRootedRouteRewardCards(player);
        if (cards.Count > 0)
        {
            await new RewardsSet(player)
                .WithCustomRewards(cards.Select<CardModel, Reward>(card => new SpecialCardReward(card, player)).ToList())
                .WithSkippingDisallowed()
                .Offer();
        }

        await TryGivePotion(player);
        SetProgress(player, progress with { RootedRouteResolved = true });
        RemoveQuestMarker<UrdaRootedRouteMapQuestMarker>(current);
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Rooted Route resolved at {current.coord.col},{current.coord.row}; offered {cards.Count} source-safe single-card reward(s).");
    }

    private static List<CardModel> CreateRootedRouteRewardCards(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        var cards = CardFactory.CreateForReward(player, RootedRouteCardRewards, options)
            .Select(result => result.Card)
            .ToList();
        if (cards.FirstOrDefault() is { IsUpgradable: true } first)
        {
            CardCmd.Upgrade(first, CardPreviewStyle.None);
        }

        return cards;
    }
}
