using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionFeatureGuardTests
{
    [Fact]
    public void A11AndA17MapGeometryStayGatedOptionalAndRouteSafe()
    {
        var featureGate = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var expansionConfig = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var mapProof = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "A11MapGeometryProof.cs");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");
        var mapGenerationPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionMapGenerationPatches.cs");
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");

        AssertSourceContains(
            featureGate,
            "A11ExtraMapColumns = 1",
            "A11ActOneExtraMapRows = 1",
            "A11ActTwoExtraMapRows = 1",
            "A11ActThreeExtraMapRows = 2",
            "IsMapGeometryEnabled(IRunState runState)",
            "IsDeepBranchesEnabled(IRunState runState)",
            "AscensionExpansionConfig.Current.EnableMapGeometry",
            "AscensionExpansionConfig.Current.EnableDeepBranches");

        AssertSourceContains(
            expansionConfig,
            "EnableMapGeometryEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_MAP_GEOMETRY\"",
            "LegacyEnableMapGeometryEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_MAP_GEOMETRY\"",
            "EnableDeepBranchesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "LegacyEnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"");

        AssertSourceContains(
            mapService,
            "if (AscensionFeatureGate.IsMapGeometryEnabled(runState))",
            "if (AscensionFeatureGate.IsDeepBranchesEnabled(runState))",
            "VanillaMapColumns = 7",
            "A11InsertedColumn = 4",
            "TryInsertA11WidthChoice(saved)",
            "HasA11InsertedColumnRouteChoice",
            "ApplyA11MapGeometryAtCreateMapBoundary",
            "ActModel.CreateMap",
            "Ascension A11 source-boundary check",
            "insertedColumnRoute={evidence.HasInsertedColumnRouteChoice}",
            "originalRoutePreserved={evidence.HasStartToBossRouteAvoidingInsertedColumn}",
            "insertedColumnRouteChoices={evidence.InsertedColumnRouteChoiceCount}",
            "TryGetA11GeometryEvidence(map, out var evidence)",
            "catch (Exception ex)",
            "A11 map geometry diagnostic failed",
            "A11 map geometry diagnostic failed closed",
            "GetA11TargetRowCount(runState, actIndex)",
            "HasA11OriginalRoutePreserved(saved)",
            "A11MapGeometryProof.Analyze",
            "HasSerializablePath(saved.StartingPoint",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "EnumerateDeepBranchColumns(map)",
            "TryMatchExistingDeepBranch",
            "IsDeepBranchRouteSafe(saved, plan)",
            "HasSerializablePathAvoiding",
            "runState.Players.Count > 1",
            "IsDeepBranchAct(actIndex)",
            "canBeModified: false",
            "MapPointType.Elite",
            "MapPointType.Treasure",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "safe-route reconnect");

        AssertSourceContains(
            mapProof,
            "A11MapGeometryGraph",
            "A11MapGeometryEvidence",
            "HasInsertedColumnRouteChoice",
            "HasStartToBossRouteAvoidingInsertedColumn",
            "InsertedColumnRouteChoiceCount");

        AssertSourceContains(
            rootRunHook,
            "public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)",
            "return AscensionMapService.Apply(runState, map, actIndex);",
            "public override ActMap ModifyGeneratedMapLate(IRunState runState, ActMap map, int actIndex)");

        AssertSourceContains(
            mapGenerationPatch,
            "HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))",
            "Postfix(RunState runState, ref ActMap __result)",
            "AscensionMapService.ApplyA11MapGeometryAtCreateMapBoundary",
            "runState.CurrentActIndex");

        AssertSourceContains(
            metadata,
            "DeepBranchNodeKind",
            "EnhancedReward",
            "DeepBranch.HasValue",
            "IsDeepBranchEntry");

        Assert.Contains("A11 converts the generated map", apiResearch, StringComparison.Ordinal);
        Assert.Contains("reachable optional route", apiResearch, StringComparison.Ordinal);
        Assert.Contains("A17 uses the same saved-map replacement path", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Multiplayer Deep Branch insertion is intentionally skipped", apiResearch, StringComparison.Ordinal);
        Assert.Contains("Map width increases from 7 to 8 columns.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Act 1 visible route rows increase by 1, Act 2 visible route rows increase by 1, and Act 3 visible route rows increase by 2.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("At least one reachable optional node appears in the inserted width column.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("No A11-specific marker, icon, or hover tooltip appears", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A safer parallel route from the branch parent to reconnect remains available", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Deep Branch insertion now searches for an empty branch column", apiResearch, StringComparison.Ordinal);
        Assert.Contains("start-to-boss route that skips branch nodes remains", apiResearch, StringComparison.Ordinal);
        Assert.DoesNotContain("MarkLongRoad", mapService, StringComparison.Ordinal);
        Assert.DoesNotContain("LongRoad", metadata, StringComparison.Ordinal);
        Assert.DoesNotContain("LONG_ROAD_NODE", mapService, StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void CoreMapGenerationAndMapScreenStillUseExpectedMapGeometryHooks()
    {
        var coreRunManager = ReadLocalCoreText("Runs", "RunManager.cs");
        var coreMapScreen = ReadLocalCoreText("Nodes", "Screens", "Map", "NMapScreen.cs");

        AssertSourceContains(
            coreRunManager,
            "ActMap map2 = State.Act.CreateMap(State, replaceTreasureWithElites: false)",
            "map = Hook.ModifyGeneratedMap(State, map2, State.CurrentActIndex)",
            "State.Map = map",
            "NMapScreen.Instance?.SetMap(map, State.Rng.Seed, clearDrawings: true)");

        AssertSourceContains(
            coreMapScreen,
            "int rowCount = map.GetRowCount()",
            "int columnCount = map.GetColumnCount()",
            "_distY = 2325f / (float)(rowCount - 1)",
            "_distX = 1050f / (float)columnCount",
            "foreach (MapPoint allMapPoint in map.GetAllMapPoints())");
    }
}
