using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int RootedRouteMaxVisibleFloor = 7;
    private const int RootedRouteCardRewards = 3;
    private const int RootedRouteWitherHpLoss = 8;
    private const int RootedRouteWitherGold = 25;

    public static void ApplyRootedRoute(Player player)
    {
        var progress = GetProgress(player);
        var target = FindRootedRouteTarget(player);
        if (target == null)
        {
            SetProgress(player, progress with { RootedRouteWithered = true });
            MainFile.Logger.Warn("[Spire Plus] Urda Rooted Route could not find a source-safe reachable Act 1 normal combat target.");
            return;
        }

        EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>(target);
        SetProgress(player, progress with
        {
            RootedRouteCoord = FormatCoord(target.coord),
            RootedRouteResolved = false,
            RootedRouteWithered = false
        });
        MainFile.Logger.Info($"[Spire Plus] Urda Rooted Route marked reachable normal combat node {target.coord.col},{target.coord.row}.");
    }

    private static async Task CheckRootedRouteBeforeRoom(Player player)
    {
        var progress = GetProgress(player);
        if (progress.RootedRouteResolved ||
            progress.RootedRouteWithered ||
            string.IsNullOrWhiteSpace(progress.RootedRouteCoord))
        {
            return;
        }

        var runState = player.RunState;
        var current = runState.CurrentMapPoint;
        var target = FindPointByCoord(runState, progress.RootedRouteCoord);
        if (current == null || target == null)
        {
            await WitherRootedRoute(player, progress, "missing current or target map point");
            return;
        }

        if (SameCoord(current.coord, target.coord))
        {
            return;
        }

        var path = current.BFS_FindPath(target).ToList();
        if (target.coord.row < current.coord.row || path.Count == 0)
        {
            await WitherRootedRoute(player, progress, "target is no longer reachable from the current route");
        }
    }

    private static async Task WitherRootedRoute(Player player, Progress progress, string reason)
    {
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            RootedRouteWitherHpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
        await PlayerCmd.GainGold(RootedRouteWitherGold, player);
        SetProgress(player, progress with { RootedRouteWithered = true });
        MainFile.Logger.Info(
            $"[Spire Plus] Urda Rooted Route withered ({reason}); lost {RootedRouteWitherHpLoss} HP and gained {RootedRouteWitherGold} Gold.");
    }

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

    private static MapPoint? FindRootedRouteTarget(Player player)
    {
        var runState = player.RunState;
        var current = runState.CurrentMapPoint ?? runState.Map.StartingMapPoint;
        return EnumerateReachable(current)
            .Where(point =>
                point.coord.row > current.coord.row &&
                point.coord.row + 1 <= RootedRouteMaxVisibleFloor &&
                point.PointType == MapPointType.Monster)
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .FirstOrDefault();
    }

}
