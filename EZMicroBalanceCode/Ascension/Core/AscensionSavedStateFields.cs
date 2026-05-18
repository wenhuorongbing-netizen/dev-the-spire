using MegaCrit.Sts2.Core.Entities.Players;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionSavedStateFields
{
    public static readonly SavedSpireField<Player, bool> RootBeginsApplied =
        new(() => false, "EZMicroBalanceAscensionRootBeginsApplied");

    public static readonly SavedSpireField<Player, int> RootblightLevel =
        new(() => 0, "EZMicroBalanceAscensionRootblightLevel");

    public static readonly SavedSpireField<Player, string> RootblightPendingCombatDowngrades =
        new(() => string.Empty, "EZMicroBalanceAscensionRootblightPendingCombatDowngrades");

    public static readonly SavedSpireField<RootFamilyCard, bool> RootblightWasPresentAtCombatStart =
        new(() => false, "EZMicroBalanceAscensionRootblightWasPresentAtCombatStart");

    public static readonly SavedSpireField<RootFamilyCard, bool> RootblightHasSplit =
        new(() => false, "EZMicroBalanceAscensionRootblightHasSplit");

    public static readonly SavedSpireField<RootFamilyCard, bool> RootblightPlantedInSeedbed =
        new(() => false, "EZMicroBalanceAscensionRootblightPlantedInSeedbed");

    public static readonly SavedSpireField<Player, bool> ForgeTokenHeld =
        new(() => false, "EZMicroBalanceAscensionForgeTokenHeld");

    public static readonly SavedSpireField<RootBud, bool> RootBudEnteredHand =
        new(() => false, "EZMicroBalanceAscensionRootBudEnteredHand");

    public static readonly SavedSpireField<RootBud, bool> RootBudPlayed =
        new(() => false, "EZMicroBalanceAscensionRootBudPlayed");

    public static readonly SavedSpireField<RootBud, bool> RootBudSprouted =
        new(() => false, "EZMicroBalanceAscensionRootBudSprouted");

    public static readonly SavedSpireField<RootBud, bool> RootBudPlantedInSeedbed =
        new(() => false, "EZMicroBalanceAscensionRootBudPlantedInSeedbed");

    public static readonly SavedSpireField<RootBud, int> RootBudSproutRound =
        new(() => RootBud.DefaultSproutRound, "EZMicroBalanceAscensionRootBudSproutRound");
}
