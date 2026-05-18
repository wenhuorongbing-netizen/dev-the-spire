using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static AscensionNodeMetadata GetOrCreateMetadata(MapPoint point)
    {
        return MetadataByPoint.GetValue(point, _ => new AscensionNodeMetadata());
    }

    private static void EnsureQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        if (point.Quests.Any(quest => quest is TMarker))
        {
            return;
        }

        point.AddQuest(ModelDb.GetById<TMarker>(ModelDb.GetId<TMarker>()));
    }
}
