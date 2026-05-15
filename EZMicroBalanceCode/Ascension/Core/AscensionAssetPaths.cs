namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionAssetPaths
{
    public static string FiremarkedEliteIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemarked_elite_indicator.png");

    public static string FiremarkMightIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemark_might_indicator.png");

    public static string FiremarkGiantIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemark_giant_indicator.png");

    public static string FiremarkForgeArmorIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemark_forge_armor_indicator.png");

    public static string FiremarkConstantHealIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemark_constant_heal_indicator.png");

    public static string BannerRoomIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "banner_room_indicator.png");

    public static string BannerVanguardIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "banner_vanguard_indicator.png");

    public static string BannerShieldFormationIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "banner_shield_formation_indicator.png");

    public static string BannerBountyIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "banner_bounty_indicator.png");

    public static string BossSealIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "boss_seal_indicator.png");

    public static string DeepBranchEntryIndicator => BossSealIndicator;

    public static string ForgeTokenStatus =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "forge_token_status.png");

    public static string FissionEnchantmentIcon =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "fission_enchantment_icon.png");

    public static string GetFiremarkIndicator(FiremarkKind firemark) => firemark switch
    {
        FiremarkKind.Might => FiremarkMightIndicator,
        FiremarkKind.Giant => FiremarkGiantIndicator,
        FiremarkKind.ForgeArmor => FiremarkForgeArmorIndicator,
        FiremarkKind.ConstantHeal => FiremarkConstantHealIndicator,
        _ => FiremarkedEliteIndicator
    };

    public static string GetBannerIndicator(BannerKind banner) => banner switch
    {
        BannerKind.Vanguard => BannerVanguardIndicator,
        BannerKind.ShieldFormation => BannerShieldFormationIndicator,
        BannerKind.Bounty => BannerBountyIndicator,
        _ => BannerRoomIndicator
    };
}
