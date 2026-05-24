namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionExpansionConfig
{
    public const string DisableAllEnvironmentVariable = "SPIREPLUS_ASCENSION_DISABLE_ALL_SYSTEMS";
    public const string EnableMapGeometryEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_MAP_GEOMETRY";
    public const string EnableRootblightEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_ROOTBLIGHT";
    public const string EnableBlightSproutEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_BLIGHT_SPROUT";
    public const string EnableFiremarkedElitesEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_FIRE_MARK_ELITES";
    public const string EnableForgeTokenEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_FORGE_TOKEN";
    public const string EnableFissionEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_FISSION";
    public const string EnableBannerRoomsEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_BANNER_ROOMS";
    public const string EnableDeepBranchesEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_DEEP_BRANCHES";
    public const string EnableBossSealsEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_BOSS_SEALS";
    public const string EnableBrandedFormEnvironmentVariable = "SPIREPLUS_ASCENSION_ENABLE_BRANDED_FORM";

    public const string LegacyDisableAllEnvironmentVariable = "EZMB_ASCENSION_DISABLE_ALL_SYSTEMS";
    public const string LegacyEnableMapGeometryEnvironmentVariable = "EZMB_ASCENSION_ENABLE_MAP_GEOMETRY";
    public const string LegacyEnableRootblightEnvironmentVariable = "EZMB_ASCENSION_ENABLE_ROOTBLIGHT";
    public const string LegacyEnableBlightSproutEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BLIGHT_SPROUT";
    public const string LegacyEnableFiremarkedElitesEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FIRE_MARK_ELITES";
    public const string LegacyEnableForgeTokenEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FORGE_TOKEN";
    public const string LegacyEnableFissionEnvironmentVariable = "EZMB_ASCENSION_ENABLE_FISSION";
    public const string LegacyEnableBannerRoomsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BANNER_ROOMS";
    public const string LegacyEnableDeepBranchesEnvironmentVariable = "EZMB_ASCENSION_ENABLE_DEEP_BRANCHES";
    public const string LegacyEnableBossSealsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BOSS_SEALS";
    public const string LegacyEnableBrandedFormEnvironmentVariable = "EZMB_ASCENSION_ENABLE_BRANDED_FORM";
    // Legacy local scripts used the original design name. Keep it as an alias,
    // while new code and logs use the player-facing Branded Form term.
    public const string EnableDualKingBrandsEnvironmentVariable = "EZMB_ASCENSION_ENABLE_DUAL_KING_BRANDS";

    public static AscensionExpansionConfig Current => new();

    public bool EnableMapGeometry => IsEnabled(EnableMapGeometryEnvironmentVariable, LegacyEnableMapGeometryEnvironmentVariable);
    public bool EnableRootblight => IsEnabled(EnableRootblightEnvironmentVariable, LegacyEnableRootblightEnvironmentVariable);
    public bool EnableBlightSprout => IsEnabled(EnableBlightSproutEnvironmentVariable, LegacyEnableBlightSproutEnvironmentVariable);
    public bool EnableFiremarkedElites => IsEnabled(EnableFiremarkedElitesEnvironmentVariable, LegacyEnableFiremarkedElitesEnvironmentVariable);
    public bool EnableForgeToken => IsEnabled(EnableForgeTokenEnvironmentVariable, LegacyEnableForgeTokenEnvironmentVariable);
    public bool EnableFission => IsEnabled(EnableFissionEnvironmentVariable, LegacyEnableFissionEnvironmentVariable);
    public bool EnableBannerRooms => IsEnabled(EnableBannerRoomsEnvironmentVariable, LegacyEnableBannerRoomsEnvironmentVariable);
    public bool EnableDeepBranches => IsEnabled(EnableDeepBranchesEnvironmentVariable, LegacyEnableDeepBranchesEnvironmentVariable);
    public bool EnableBossSeals => IsEnabled(EnableBossSealsEnvironmentVariable, LegacyEnableBossSealsEnvironmentVariable);
    public bool EnableBrandedForm => IsEnabled(EnableBrandedFormEnvironmentVariable, LegacyEnableBrandedFormEnvironmentVariable) && IsEnabled(EnableDualKingBrandsEnvironmentVariable);
    public bool EnableDualKingBrands => EnableBrandedForm;

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
        EnableBrandedForm;

    public string Summary =>
        $"map={EnableMapGeometry}; rootblight={EnableRootblight}; sprout={EnableBlightSprout}; firemarks={EnableFiremarkedElites}; forgeToken={EnableForgeToken}; fission={EnableFission}; banners={EnableBannerRooms}; deepBranches={EnableDeepBranches}; bossSeals={EnableBossSeals}; brandedForm={EnableBrandedForm}";

    private static bool IsEnabled(string environmentVariable, params string[] legacyEnvironmentVariables)
    {
        if (IsTruthyEnvironmentVariable(DisableAllEnvironmentVariable, LegacyDisableAllEnvironmentVariable))
        {
            return false;
        }

        var rawValue = FirstRawEnvironmentValue(new[] { environmentVariable }.Concat(legacyEnvironmentVariables).ToArray());
        return rawValue == null || IsTruthy(rawValue);
    }

    private static string? FirstRawEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsTruthyEnvironmentVariable(params string[] names) =>
        names.Any(name => IsTruthy(Environment.GetEnvironmentVariable(name)));

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
