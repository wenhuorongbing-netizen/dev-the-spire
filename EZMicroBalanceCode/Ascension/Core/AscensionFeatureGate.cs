using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionFeatureGate
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

}
