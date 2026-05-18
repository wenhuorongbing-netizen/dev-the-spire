using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static IEnumerable<ModelId> GetReservedRootSightModelIds(
        RunState runState,
        RoomType roomType,
        string currentCoord)
    {
        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
        {
            foreach (var preview in GetRootSightPreviews(GetProgress(player).RootSightPreviewRecords)
                .Where(preview =>
                    preview.ActIndex == runState.CurrentActIndex &&
                    preview.Coord != currentCoord &&
                    preview.RoomType == roomType))
            {
                if (TryDeserializeModelId(preview.ModelId, out var id) && id != null)
                {
                    yield return id;
                }
            }
        }
    }

    private static bool TryDeserializeModelId(string value, out ModelId? id)
    {
        try
        {
            id = ModelId.Deserialize(value);
            return true;
        }
        catch
        {
            id = null;
            return false;
        }
    }
}
