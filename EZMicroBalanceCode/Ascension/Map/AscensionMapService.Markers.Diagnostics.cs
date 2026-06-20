using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionMapService
{
    private static void LogMapAssignment<TEnum>(int actIndex, MapCoord coord, string markerFamily, TEnum kind)
        where TEnum : struct, Enum
    {
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension map assignment: actIndex={actIndex}; coord=({coord.col},{coord.row}); markerFamily={markerFamily}; kind={kind}.");
    }

    private static void LogMapDistributionSummary(IRunState runState, ActMap map, int actIndex)
    {
        if (!AscensionFeatureGate.IsDiagnosticsEnabled)
        {
            return;
        }

        var assigned = map.GetAllMapPoints()
            .Select(point => (Point: point, Metadata: TryGetMetadata(point)))
            .Where(entry => entry.Metadata != null)
            .ToList();

        var firemarks = assigned
            .Where(entry => entry.Metadata!.Firemark.HasValue)
            .Select(entry => $"({entry.Point.coord.col},{entry.Point.coord.row})={entry.Metadata!.Firemark!.Value}");
        var banners = assigned
            .Where(entry => entry.Metadata!.Banner.HasValue)
            .Select(entry => $"({entry.Point.coord.col},{entry.Point.coord.row})={entry.Metadata!.Banner!.Value}");
        var bossSeals = new[] { map.BossMapPoint, map.SecondBossMapPoint }
            .Where(point => point != null)
            .Select(point => (Point: point!, Metadata: TryGetMetadata(point)))
            .Where(entry => entry.Metadata?.BossSeal != null)
            .Select(entry =>
                $"({entry.Point.coord.col},{entry.Point.coord.row})={entry.Metadata!.BossSeal!.Name}/{entry.Metadata.BossSeal.Id}" +
                (entry.Metadata.IsBossBrand ? "/brand" : "/seal"));

        MainFile.Logger.Info(
            $"[Spire Plus] Ascension diagnostics: map marker distribution; actIndex={actIndex}; seed={runState.Rng.StringSeed}; firemarkKinds=[{string.Join(", ", firemarks)}]; bannerKinds=[{string.Join(", ", banners)}]; bossSeals=[{string.Join(", ", bossSeals)}].");
    }
}
