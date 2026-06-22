namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal static class AncientSavedStateFields
{
    public static readonly SavedAttachedState<PrismaticGem, int> PrismaticGemNormalRewardCounter =
        new("EZMicroBalanceNormalRewardCounter", () => 0);

    public static readonly SavedAttachedState<PaelsTooth, int> PaelsToothNonBossCombatCounter =
        new("EZMicroBalanceNonBossCombatCounter", () => 0);

    public static readonly SavedAttachedState<CardModel, bool> JewelryBoxNonInnateApotheosis =
        new("EZMicroBalanceJewelryBoxNonInnateApotheosis", () => false);

    public static readonly SavedAttachedState<Player, string> UrdaStateKey =
        new("EZMicroBalanceUrdaStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, string> UrdaDeckStateKey =
        new("EZMicroBalanceUrdaDeckStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, bool> UrdaTrialPlantCard =
        new("EZMicroBalanceUrdaTrialPlantCard", () => false);

    public static readonly SavedAttachedState<Player, string> MorviStateKey =
        new("EZMicroBalanceMorviStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, string> MorviDeckStateKey =
        new("EZMicroBalanceMorviDeckStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, bool> MorviBorrowedAncientCard =
        new("EZMicroBalanceMorviBorrowedAncientCard", () => false);

    public static readonly SavedAttachedState<CardModel, bool> MorviOpenBookSealedCard =
        new("EZMicroBalanceMorviOpenBookSealedCard", () => false);

    public static readonly SavedAttachedState<Player, string> LothaStateKey =
        new("EZMicroBalanceLothaStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, string> LothaDeckStateKey =
        new("EZMicroBalanceLothaDeckStateKey", () => string.Empty);

    public static readonly SavedAttachedState<CardModel, bool> LothaMirrorRebuttalCard =
        new("EZMicroBalanceLothaMirrorRebuttalCard", () => false);

    public static readonly SavedAttachedState<Player, string> AncientInitialOptionRerollStateKey =
        new("EZMicroBalanceAncientInitialOptionRerollStateKey", () => string.Empty);

    public static void EnsureRegistered()
    {
        _ = PrismaticGemNormalRewardCounter;
        _ = PaelsToothNonBossCombatCounter;
        _ = JewelryBoxNonInnateApotheosis;
        _ = UrdaStateKey;
        _ = UrdaDeckStateKey;
        _ = UrdaTrialPlantCard;
        _ = MorviStateKey;
        _ = MorviDeckStateKey;
        _ = MorviBorrowedAncientCard;
        _ = MorviOpenBookSealedCard;
        _ = LothaStateKey;
        _ = LothaDeckStateKey;
        _ = LothaMirrorRebuttalCard;
        _ = AncientInitialOptionRerollStateKey;
    }
}
