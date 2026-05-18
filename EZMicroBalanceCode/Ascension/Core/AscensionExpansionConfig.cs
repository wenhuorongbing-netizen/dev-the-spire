namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionExpansionConfig
{
    public const string DisableAllEnvironmentVariable = "EZMB_ASCENSION_DISABLE_ALL_SYSTEMS";
    public const string EnableMapGeometryEnvironmentVariable = "EZMB_ASCENSION_ENABLE_MAP_GEOMETRY";
    public const string EnableRootblightEnvironmentVariable = "EZMB_ASCENSION_ENABLE_ROOTBLIGHT";
    public const string EnableBlightSproutEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT";
    public const string EnableFiremarkedElitesEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES";
    public const string EnableForgeTokenEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FORGE_TOKEN";
    public const string EnableFissionEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FISSION";
    public const string EnableBannerRoomsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BANNER_ROOMS";
    public const string EnableDeepBranchesEnvironmentVariable = "EZMB_ASCENSION_ENABLE_DEEP_BRANCHES";
    public const string EnableBossSealsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BOSS_SEALS";
    public const string EnableDualKingBrandsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS";

    public static AscensionExpansionConfig Current => new();

    public bool EnableMapGeometry => IsEnabled(EnableMapGeometryEnvironmentVariable);
    public bool EnableRootblight => IsEnabled(EnableRootblightEnvironmentVariable);
    public bool EnableBlightSprout => IsEnabled(EnableBlightSproutEnvironmentVariable);
    public bool EnableFiremarkedElites => IsEnabled(EnableFiremarkedElitesEnvironmentVariable);
    public bool EnableForgeToken => IsEnabled(EnableForgeTokenEnvironmentVariable);
    public bool EnableFission => IsEnabled(EnableFissionEnvironmentVariable);
    public bool EnableBannerRooms => IsEnabled(EnableBannerRoomsEnvironmentVariable);
    public bool EnableDeepBranches => IsEnabled(EnableDeepBranchesEnvironmentVariable);
    public bool EnableBossSeals => IsEnabled(EnableBossSealsEnvironmentVariable);
    public bool EnableDualKingBrands => IsEnabled(EnableDualKingBrandsEnvironmentVariable);

    public bool AnyGameplaySystemEnabled =>
        EnableMapGeometry ||
        EnableRootblight ||
        EnableBlightSprout ||
        EnableFiremarkedElites ||
        EnableForgeToken ||
        EnableFission ||
        EnableBannerRooms ||
        EnableDeepBranches ||
        EnableBossSeals ||
        EnableDualKingBrands;

    public string Summary =>
        $"map={EnableMapGeometry}; rootblight={EnableRootblight}; sprout={EnableBlightSprout}; firemarks={EnableFiremarkedElites}; forgeToken={EnableForgeToken}; fission={EnableFission}; banners={EnableBannerRooms}; deepBranches={EnableDeepBranches}; bossSeals={EnableBossSeals}; dualBrands={EnableDualKingBrands}";

    private static bool IsEnabled(string environmentVariable)
    {
        if (IsTruthy(Environment.GetEnvironmentVariable(DisableAllEnvironmentVariable)))
        {
            return false;
        }

        var rawValue = Environment.GetEnvironmentVariable(environmentVariable);
        return rawValue == null || IsTruthy(rawValue);
    }

    public static bool IsTruthy(string? value)
    {
        var candidate = value?.Trim();
        return !string.IsNullOrWhiteSpace(candidate) &&
            (candidate.Equals("1", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("true", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
             candidate.Equals("on", StringComparison.OrdinalIgnoreCase));
    }
}
