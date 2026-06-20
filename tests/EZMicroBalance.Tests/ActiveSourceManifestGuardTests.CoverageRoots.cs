namespace EZMicroBalance.Tests;

public sealed partial class ActiveSourceManifestGuardTests
{
    private static readonly SourceCoverageRoot[] IndependentCoverageRoots =
    [
        new("EZMicroBalanceCode/Ancients/Common/", "ReleaseCoverageGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\")"),
        new("EZMicroBalanceCode/Ancients/Patches/", "AncientHighRiskSourceGuardTests.RewardRelicSafety.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\", \"Patches\")"),
        new("EZMicroBalanceCode/Ancients/Expansion/Urda/", "UrdaReleaseCoverageGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\", \"Expansion\", \"Urda\")"),
        new("EZMicroBalanceCode/Ancients/Expansion/Morvi/", "MorviV22GuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\", \"Expansion\", \"Morvi\")"),
        new("EZMicroBalanceCode/Ancients/Expansion/Lotha/", "LothaPolishGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\", \"Expansion\", \"Lotha\")"),
        new("EZMicroBalanceCode/Ancients/Expansion/Vakuu/", "VakuuTemptationGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ancients\", \"Expansion\", \"Vakuu\")"),
        new("EZMicroBalanceCode/Ascension/", "AscensionFeatureGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\", \"Ascension\")"),
        new("EZMicroBalanceCode/Core/Architecture/", "ArchitectureSkeletonGuardTests.cs", "RewardPipelineDefinesRewardPhaseEnum"),
        new("EZMicroBalanceCode/Core/Features/", "EngineeringGovernanceGuardTests.cs", "var registry = SpirePlusFeatureRegistry.CreateDefault()"),
        new("EZMicroBalanceCode/Core/Integrations/RitsuLib/", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")"),
        new("EZMicroBalanceCode/Core/Localization/", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")"),
        new("EZMicroBalanceCode/Config/", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")"),
        new("EZMicroBalanceCode/Diagnostics/", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")"),
        new("EZMicroBalanceCode/Map/", "AncientHighRiskSourceGuardTests.RootSight.cs", "ReadRepoText(\"EZMicroBalanceCode\", \"Map\", \"SpirePlusMapPointHoverComposer.cs\")"),
        new("EZMicroBalanceCode/Modding/", "ModInfoLocalizationGuardTests.cs", "ReadRepoText(\"EZMicroBalanceCode\", \"Modding\", \"ModInfoLocalizationPatches.cs\")"),
        new("EZMicroBalanceCode/Preview/", "PreviewToolsGuardTests.cs", "CrystalSpherePatchOnlyTouchesTheMaskAndButton"),
        new("EZMicroBalanceCode/Sts1Events/", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")"),
        new("EZMicroBalanceCode/MainFile.cs", "ReleaseSafetyExpandedGuardTests.cs", "ReadSourceTree(\"EZMicroBalanceCode\")")
    ];

    private sealed record SourceCoverageRoot(string PathPrefix, string GuardTestFile, string GuardEvidenceSnippet)
    {
        public bool Covers(string activeSource)
        {
            return PathPrefix.EndsWith(".cs", StringComparison.Ordinal)
                ? activeSource.Equals(PathPrefix, StringComparison.Ordinal)
                : activeSource.StartsWith(PathPrefix, StringComparison.Ordinal);
        }
    }
}
