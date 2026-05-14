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

    public static readonly SavedSpireField<CardModel, string> UrdaDeckStateKey =
        new(() => string.Empty, "EZMicroBalanceUrdaDeckStateKey");

    public static readonly SavedSpireField<CardModel, bool> UrdaTrialPlantCard =
        new(() => false, "EZMicroBalanceUrdaTrialPlantCard");

    public static readonly SavedSpireField<Player, string> MorviStateKey =
        new(() => string.Empty, "EZMicroBalanceMorviStateKey");

    public static readonly SavedSpireField<CardModel, string> MorviDeckStateKey =
        new(() => string.Empty, "EZMicroBalanceMorviDeckStateKey");

    public static readonly SavedSpireField<CardModel, bool> MorviBorrowedAncientCard =
        new(() => false, "EZMicroBalanceMorviBorrowedAncientCard");

    public static readonly SavedSpireField<CardModel, bool> MorviOpenBookSealedCard =
        new(() => false, "EZMicroBalanceMorviOpenBookSealedCard");

    public static readonly SavedSpireField<Player, string> LothaStateKey =
        new(() => string.Empty, "EZMicroBalanceLothaStateKey");

    public static readonly SavedSpireField<CardModel, string> LothaDeckStateKey =
        new(() => string.Empty, "EZMicroBalanceLothaDeckStateKey");

    public static readonly SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard =
        new(() => false, "EZMicroBalanceLothaMirrorRebuttalCard");
}
