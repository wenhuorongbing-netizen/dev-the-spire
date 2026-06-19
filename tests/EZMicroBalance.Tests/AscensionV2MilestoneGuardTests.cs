using System.IO.Compression;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionV2MilestoneGuardTests
{
    [Fact]
    public void CombatModifierEntryPointsShareNodeMetadataRefreshHelpers()
    {
        var combatModifiers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");

        AssertSourceContains(
            combatModifiers,
            "private static bool TryRefreshNodeMetadata(",
            "tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState)",
            "tracker.NodeMetadata = current",
            "private static bool TryRefreshActiveBossSealMetadata(",
            "TryRefreshNodeMetadata(combatState, tracker, out metadata) &&",
            "HasActiveBossSeal(combatState, metadata)");
        Assert.DoesNotContain(
            "var metadata = tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState);",
            combatModifiers,
            StringComparison.Ordinal);

        var metadataLookupCount = Regex.Matches(
            combatModifiers,
            @"tracker\.NodeMetadata \?\? AscensionMapService\.TryGetCurrentMetadata\(combatState\.RunState\)",
            RegexOptions.CultureInvariant).Count;
        Assert.Equal(1, metadataLookupCount);
    }

    [Fact]
    public void Milestone0FeatureFlagsAreIndependentAndAllOffIsANoOp()
    {
        var config = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionExpansionConfig.cs");
        var gates = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Core");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionInitializer.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var rootRunHook = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "RootRunHook.cs");

        AssertSourceContains(
            config,
            "DisableAllEnvironmentVariable = \"SPIREPLUS_ASCENSION_DISABLE_ALL_SYSTEMS\"",
            "LegacyDisableAllEnvironmentVariable = \"EZMB_ASCENSION_DISABLE_ALL_SYSTEMS\"",
            "EnableRootblightEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_ROOTBLIGHT\"",
            "LegacyEnableRootblightEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_ROOTBLIGHT\"",
            "EnableBlightSproutEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BLIGHT_SPROUT\"",
            "LegacyEnableBlightSproutEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT\"",
            "EnableFiremarkedElitesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FIRE_MARK_ELITES\"",
            "LegacyEnableFiremarkedElitesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES\"",
            "EnableForgeTokenEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FORGE_TOKEN\"",
            "LegacyEnableForgeTokenEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FORGE_TOKEN\"",
            "EnableFissionEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_FISSION\"",
            "LegacyEnableFissionEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_FISSION\"",
            "EnableBannerRoomsEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BANNER_ROOMS\"",
            "LegacyEnableBannerRoomsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BANNER_ROOMS\"",
            "EnableDeepBranchesEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "LegacyEnableDeepBranchesEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DEEP_BRANCHES\"",
            "EnableBossSealsEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BOSS_SEALS\"",
            "LegacyEnableBossSealsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BOSS_SEALS\"",
            "EnableBrandedFormEnvironmentVariable = \"SPIREPLUS_ASCENSION_ENABLE_BRANDED_FORM\"",
            "LegacyEnableBrandedFormEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_BRANDED_FORM\"",
            "EnableDualKingBrandsEnvironmentVariable = \"EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS\"",
            "EnableBrandedForm => IsEnabled(EnableBrandedFormEnvironmentVariable, LegacyEnableBrandedFormEnvironmentVariable) && IsEnabled(EnableDualKingBrandsEnvironmentVariable)",
            "return false;",
            "rootblight={EnableRootblight}",
            "brandedForm={EnableBrandedForm}");

        AssertSourceContains(
            gates,
            "if (!AscensionExpansionConfig.Current.AnyGameplaySystemEnabled)",
            "return false;",
            "IsMapGeometryEnabled",
            "IsFiremarkedEliteEnabled",
            "IsForgeTokenEnabled",
            "IsFissionEnabled",
            "IsRootblightEnabled",
            "IsBossBlightSproutEnabled",
            "IsBannerRoomEnabled",
            "IsDeepBranchesEnabled",
            "IsBossSealsEnabled",
            "IsBrandedFormEnabled",
            "IsBrandedFormSinglePlayerEnabled",
            "runState.Players.Count == 1");

        AssertSourceContains(
            initializer,
            "AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) ||",
            "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(combatState.RunState) &&",
            "!AscensionFeatureGate.IsDiagnosticsEnabled",
            "ShouldDisableUnverifiedCoopCombatHook");
        AssertSourceContains(mapService, "if (!AscensionFeatureGate.IsAnyImplementedSliceEnabled(runState) &&", "return map;");
        AssertSourceContains(rewardService, "AscensionFeatureGate.IsFiremarkedEliteEnabled", "AscensionFeatureGate.IsBossSealsEnabled", "AscensionFeatureGate.IsFissionEnabled");
        AssertSourceContains(rootRunHook, "AscensionFeatureGate.IsRootblightEnabled", "ForgeTokenService.SyncVisibleTokens");
    }

    [ReleaseArtifactFact]
    public void PackageContainsCurrentAscensionLocalization()
    {
        var package = CurrentPackageZipPath();
        Assert.True(File.Exists(package), $"Missing package zip: {package}");

        using var archive = ZipFile.OpenRead(package);
        var pck = ReadPckDirectory(ReadZipBytes(archive, "EZMicroBalance/EZMicroBalance.pck"));

        Assert.Contains("EZMicroBalance/localization/eng/ascension.json", pck);
        Assert.Contains("EZMicroBalance/localization/zhs/ascension.json", pck);
        Assert.Contains("EZMicroBalance/localization/eng/cards.json", pck);
        Assert.Contains("EZMicroBalance/localization/zhs/cards.json", pck);

    }

    [Fact]
    public void CurrentDocsDoNotClaimAscensionReadiness()
    {
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        Assert.Contains("Full live Ascension verification is pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("A11-A20 selection is default-on only for single-player standard lobbies", currentDocs, StringComparison.Ordinal);
        Assert.Contains("live Ascension gameplay not executed yet", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("private beta ready", currentDocs, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("release ready", currentDocs, StringComparison.OrdinalIgnoreCase);
    }


}
