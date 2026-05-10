namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class AncientSavedStateFields
{
    public static readonly SavedSpireField<PrismaticGem, int> PrismaticGemNormalRewardCounter =
        new(() => 0, "EZMicroBalanceNormalRewardCounter");

    public static readonly SavedSpireField<PaelsTooth, int> PaelsToothNonBossCombatCounter =
        new(() => 0, "EZMicroBalanceNonBossCombatCounter");

    public static readonly SavedSpireField<CardModel, bool> JewelryBoxNonInnateApotheosis =
        new(() => false, "EZMicroBalanceJewelryBoxNonInnateApotheosis");

    public static readonly SavedSpireField<Player, string> UrdaStateKey =
        new(() => string.Empty, "EZMicroBalanceUrdaStateKey");
}
