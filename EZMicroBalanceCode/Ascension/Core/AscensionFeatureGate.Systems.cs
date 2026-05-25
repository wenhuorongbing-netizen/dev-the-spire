using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionFeatureGate
{
    public static bool IsMapGeometryEnabled(IRunState runState) =>
        IsLevelEnabled(runState, WiderLongerMapLevel) &&
        AscensionExpansionConfig.Current.EnableMapGeometry &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsFiremarkedEliteEnabled(IRunState runState) =>
        IsLevelEnabled(runState, FiremarkedEliteLevel) &&
        AscensionExpansionConfig.Current.EnableFiremarkedElites &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsForgeTokenEnabled(IRunState runState) =>
        IsFiremarkedEliteEnabled(runState) &&
        AscensionExpansionConfig.Current.EnableForgeToken;

    public static bool IsFissionEnabled(IRunState runState) =>
        IsLevelEnabled(runState, FissionLevel) &&
        AscensionExpansionConfig.Current.EnableFission &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsRootblightEnabled(IRunState runState) =>
        IsLevelEnabled(runState, RootBeginsLevel) &&
        AscensionExpansionConfig.Current.EnableRootblight &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsBossBlightSproutEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BossRootBudLevel) &&
        AscensionExpansionConfig.Current.EnableBlightSprout &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsBannerRoomEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BannerRoomLevel) &&
        AscensionExpansionConfig.Current.EnableBannerRooms &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsDeepBranchesEnabled(IRunState runState) =>
        IsLevelEnabled(runState, DeepBranchesLevel) &&
        AscensionExpansionConfig.Current.EnableDeepBranches &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsEliteBlightSproutEnabled(IRunState runState) =>
        IsLevelEnabled(runState, EliteRootBudLevel) &&
        AscensionExpansionConfig.Current.EnableBlightSprout &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsBossSealsEnabled(IRunState runState) =>
        IsLevelEnabled(runState, BossSealsLevel) &&
        AscensionExpansionConfig.Current.EnableBossSeals &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsBrandedFormEnabled(IRunState runState) =>
        IsLevelEnabled(runState, DoubleRoyalBrandLevel) &&
        AscensionExpansionConfig.Current.EnableBrandedForm &&
        IsCoopAscensionGameplayAllowed(runState);

    public static bool IsBrandedFormSinglePlayerEnabled(IRunState runState)
    {
        if (!IsBrandedFormEnabled(runState))
        {
            return false;
        }

        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopFeature(
                runState,
                "A20BrandedForm",
                "A20 Branded Form and second boss routing are pending two-client proof"))
        {
            return false;
        }

        return runState.Players.Count == 1;
    }

    public static bool IsDualKingBrandsEnabled(IRunState runState) => IsBrandedFormEnabled(runState);

    public static bool IsDualKingBrandsSinglePlayerEnabled(IRunState runState) => IsBrandedFormSinglePlayerEnabled(runState);

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
            IsBrandedFormEnabled(runState);
    }

    private static bool IsCoopAscensionGameplayAllowed(IRunState runState) =>
        !MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
            runState,
            "AscensionA11A20Gameplay",
            "A11-A20 map, reward, Rootblight, Firemark, Banner, and Boss ability mutations are disabled in co-op until two-client proof exists.");
}
