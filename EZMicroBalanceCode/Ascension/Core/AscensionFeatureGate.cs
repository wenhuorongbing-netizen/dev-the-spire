using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionFeatureGate
{
    public const string DebugLevelEnvironmentVariable = "EZMB_ASCENSION_DEBUG_LEVEL";
    public const string PublicGateEnvironmentVariable = "EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION";
    public const string DisablePublicSelectionEnvironmentVariable = "EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION";
    public const string DisableMultiplayerSelectionEnvironmentVariable = "EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION";
    public const string DiagnosticsEnvironmentVariable = "EZMB_ASCENSION_DIAGNOSTICS";
    public const string MultiplayerDiagnosticsEnvironmentVariable = "EZMB_ASCENSION_MULTIPLAYER_DIAGNOSTICS";

    public const int MaxSupportedAscensionLevel = 20;

    public const int WiderLongerMapLevel = 11;
    public const int FiremarkedEliteLevel = 12;
    public const int FissionLevel = 13;
    public const int RootBeginsLevel = 14;
    public const int BossRootBudLevel = 15;
    public const int BannerRoomLevel = 16;
    public const int DeepBranchesLevel = 17;
    public const int EliteRootBudLevel = 18;
    public const int BossSealsLevel = 19;
    public const int DoubleRoyalBrandLevel = 20;

    public const int A11ExtraMapColumns = 1;
    public const int A11ActOneExtraMapRows = 1;
    public const int A11ActTwoExtraMapRows = 1;
    public const int A11ActThreeExtraMapRows = 2;

    public static bool IsEnabledFor(IRunState runState, int requiredAscensionLevel)
    {
        if (!AscensionExpansionConfig.Current.AnyGameplaySystemEnabled)
        {
            return false;
        }

        if (DebugLevel >= requiredAscensionLevel)
        {
            return true;
        }

        return IsPublicSelectionEnabled && runState.AscensionLevel >= requiredAscensionLevel;
    }

    private static bool IsLevelEnabled(IRunState runState, int requiredAscensionLevel)
    {
        if (DebugLevel >= requiredAscensionLevel)
        {
            return true;
        }

        return IsPublicSelectionEnabled && runState.AscensionLevel >= requiredAscensionLevel;
    }

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

    public static bool IsDualKingBrandsSinglePlayerEnabled(IRunState runState) =>
        IsDualKingBrandsEnabled(runState) &&
        runState.Players.Count == 1;

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

    public static int DebugLevel
    {
        get
        {
            var rawValue = Environment.GetEnvironmentVariable(DebugLevelEnvironmentVariable)?.Trim();
            if (!int.TryParse(rawValue, out var level))
            {
                return 0;
            }

            return Math.Clamp(level, 0, MaxSupportedAscensionLevel);
        }
    }

    public static bool IsPublicSelectionEnabled =>
        !IsTruthy(Environment.GetEnvironmentVariable(DisablePublicSelectionEnvironmentVariable));

    public static bool IsPublicGateEnabled =>
        IsPublicSelectionEnabled;

    public static bool IsMultiplayerSelectionDisabled =>
        IsTruthy(Environment.GetEnvironmentVariable(DisableMultiplayerSelectionEnvironmentVariable));

    public static bool IsDiagnosticsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(DiagnosticsEnvironmentVariable));

    public static bool IsMultiplayerDiagnosticsEnabled =>
        IsTruthy(Environment.GetEnvironmentVariable(MultiplayerDiagnosticsEnvironmentVariable));

    private static bool IsTruthy(string? value)
    {
        return AscensionExpansionConfig.IsTruthy(value);
    }
}
