using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionFeatureGate
{
    public static bool IsMapGeometryEnabled(IRunState runState) =>
        IsLevelEnabled(runState, WiderLongerMapLevel) &&
        AscensionExpansionConfig.Current.EnableMapGeometry;

    public static bool IsFiremarkedEliteEnabled(IRunState runState) =>
        IsLevelEnabled(runState, FiremarkedEliteLevel) &&
        AscensionExpansionConfig.Current.EnableFiremarkedElites;

    public static bool IsForgeTokenEnabled(IRunState runState) =>
        IsFiremarkedEliteEnabled(runState) &&
        AscensionExpansionConfig.Current.EnableForgeToken;

    public static bool IsFissionEnabled(IRunState runState) =>
        IsLevelEnabled(runState, FissionLevel) &&
        AscensionExpansionConfig.Current.EnableFission;

    public static bool IsRootblightEnabled(IRunState runState) =>
        IsLevelEnabled(runState, RootBeginsLevel) &&
        AscensionExpansionConfig.Current.EnableRootblight;

    public static bool IsBossBlightSproutEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BossRootBudLevel) &&
        AscensionExpansionConfig.Current.EnableBlightSprout;

    public static bool IsBannerRoomEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BannerRoomLevel) &&
        AscensionExpansionConfig.Current.EnableBannerRooms;

    public static bool IsDeepBranchesEnabled(IRunState runState) =>
        IsLevelEnabled(runState, DeepBranchesLevel) &&
        AscensionExpansionConfig.Current.EnableDeepBranches;

    public static bool IsEliteBlightSproutEnabled(IRunState runState) =>
        IsLevelEnabled(runState, EliteRootBudLevel) &&
        AscensionExpansionConfig.Current.EnableBlightSprout;

    public static bool IsBossSealsEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BossSealsLevel) &&
        AscensionExpansionConfig.Current.EnableBossSeals;

    public static bool IsDualKingBrandsEnabled(IRunState runState) =>
        IsLevelEnabled(runState, DoubleRoyalBrandLevel) &&
        AscensionExpansionConfig.Current.EnableDualKingBrands;

    public static bool IsDualKingBrandsSinglePlayerEnabled(IRunState runState)
    {
        var hasVanillaSinglePlayerRunShape = runState.Players.Count == 1;
        return IsDualKingBrandsEnabled(runState) &&
            hasVanillaSinglePlayerRunShape &&
            !MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
                runState,
                "A20KingBrand",
                "dual King Brand and second boss routing are pending two-client proof");
    }

    public static bool IsAnyImplementedSliceEnabled(IRunState runState)
    {
        return IsMapGeometryEnabled(runState) ||
            IsFiremarkedEliteEnabled(runState) ||
            IsForgeTokenEnabled(runState) ||
            IsFissionEnabled(runState) ||
            IsRootblightEnabled(runState) ||
            IsBossBlightSproutEnabled(runState) ||
            IsBannerRoomEnabled(runState) ||
            IsDeepBranchesEnabled(runState) ||
            IsEliteBlightSproutEnabled(runState) ||
            IsBossSealsEnabled(runState) ||
            IsDualKingBrandsEnabled(runState);
    }
}
