using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private const string FiremarkMarkerFamily = "firemark";
    private const string BannerMarkerFamily = "banner";
    private const int ActOneFiremarkedEliteTargetCount = 2;
    private const int LaterActFiremarkedEliteTargetCount = 3;
    private const int MinimumFiremarkedEliteFallbackCount = 2;
    private const int VanillaMapColumns = 7;
    private const int A11InsertedColumn = 4;
    private const int DeepBranchMinLength = 3;
    private const int DeepBranchMaxLength = 4;

    private static readonly ConditionalWeakTable<MapPoint, AscensionNodeMetadata> MetadataByPoint = new();
    private static readonly ConditionalWeakTable<ActMap, AppliedMapMarker> AppliedMaps = new();

    public static ActMap Apply(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&
            !AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return map;
        }

        var marker = AppliedMaps.GetValue(map, _ => new AppliedMapMarker());
        if (marker.Applied)
        {
            return map;
        }

        marker.Applied = true;

        map = ApplyMapGeometry(runState, map, actIndex);
        MarkDeepBranch(runState, map, actIndex);
        MarkFiremarkedElite(runState, map, actIndex);
        MarkBannerRooms(runState, map, actIndex);
        MarkBossSeals(runState, map, actIndex);
        LogMapDistributionSummary(runState, map, actIndex);

        AppliedMaps.GetValue(map, _ => new AppliedMapMarker()).Applied = true;

        return map;
    }

    public static AscensionNodeMetadata? TryGetMetadata(MapPoint? point)
    {
        if (point == null)
        {
            return null;
        }

        return MetadataByPoint.TryGetValue(point, out var metadata) && metadata.HasAny
            ? metadata
            : null;
    }

    public static AscensionNodeMetadata? TryGetCurrentMetadata(IRunState runState)
    {
        var appliedMap = Apply(runState, runState.Map, runState.CurrentActIndex);
        if (!ReferenceEquals(appliedMap, runState.Map))
        {
            runState.Map = appliedMap;
        }

        return TryGetMetadata(runState.CurrentMapPoint);
    }

    private sealed class AppliedMapMarker
    {
        public bool Applied { get; set; }
    }
}
