using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Odds;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static IEnumerable<(RoomType RoomType, float Odds)> GetRootSightUnknownRoomOdds(IRunState runState)
    {
        var odds = runState.Odds.UnknownMapPoint;
        yield return (RoomType.Monster, odds.MonsterOdds);
        yield return (RoomType.Elite, odds.EliteOdds);
        yield return (RoomType.Treasure, odds.TreasureOdds);
        yield return (RoomType.Shop, odds.ShopOdds);
    }

    private static IEnumerable<(RoomType RoomType, float Odds)> GetRootSightUnknownRoomBaseOdds(RunState runState)
    {
        var field = AccessTools.Field(typeof(UnknownMapPointOdds), "_baseOdds");
        if (field?.GetValue(runState.Odds.UnknownMapPoint) is Dictionary<RoomType, float> baseOdds)
        {
            foreach (var pair in baseOdds)
            {
                yield return (pair.Key, pair.Value);
            }
        }
    }

    private static float GetRootSightUnknownRoomOdds(RunState runState, RoomType roomType) =>
        roomType switch
        {
            RoomType.Monster => runState.Odds.UnknownMapPoint.MonsterOdds,
            RoomType.Elite => runState.Odds.UnknownMapPoint.EliteOdds,
            RoomType.Treasure => runState.Odds.UnknownMapPoint.TreasureOdds,
            RoomType.Shop => runState.Odds.UnknownMapPoint.ShopOdds,
            _ => 0f
        };

    private static void SetRootSightUnknownRoomOdds(RunState runState, RoomType roomType, float value)
    {
        switch (roomType)
        {
            case RoomType.Monster:
                runState.Odds.UnknownMapPoint.MonsterOdds = value;
                break;
            case RoomType.Elite:
                runState.Odds.UnknownMapPoint.EliteOdds = value;
                break;
            case RoomType.Treasure:
                runState.Odds.UnknownMapPoint.TreasureOdds = value;
                break;
            case RoomType.Shop:
                runState.Odds.UnknownMapPoint.ShopOdds = value;
                break;
        }
    }
}
