namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionAssetPaths
{
    public static string FiremarkedEliteIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "firemarked_elite_indicator.png");

    public static string BannerRoomIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "banner_room_indicator.png");

    public static string BossSealIndicator =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "boss_seal_indicator.png");

    public static string DeepBranchEntryIndicator => BossSealIndicator;

    public static string ForgeTokenStatus =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "forge_token_status.png");

    public static string FissionEnchantmentIcon =>
        System.IO.Path.Join(MainFile.ResPath, "images", "ascension", "fission_enchantment_icon.png");
}
