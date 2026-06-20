using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionSavedStateFields
{
    public static readonly SavedAttachedState<Player, bool> RootBeginsApplied =
        new("EZMicroBalanceAscensionRootBeginsApplied", () => false);

    public static readonly SavedAttachedState<Player, int> RootblightLevel =
        new("EZMicroBalanceAscensionRootblightLevel", () => 0);

    public static readonly SavedAttachedState<Player, string> RootblightPendingCombatDowngrades =
        new("EZMicroBalanceAscensionRootblightPendingCombatDowngrades", () => string.Empty);

    public static readonly SavedAttachedState<RootFamilyCard, bool> RootblightWasPresentAtCombatStart =
        new("EZMicroBalanceAscensionRootblightWasPresentAtCombatStart", () => false);

    public static readonly SavedAttachedState<RootFamilyCard, bool> RootblightHasSplit =
        new("EZMicroBalanceAscensionRootblightHasSplit", () => false);

    public static readonly SavedAttachedState<RootFamilyCard, bool> RootblightPlantedInSeedbed =
        new("EZMicroBalanceAscensionRootblightPlantedInSeedbed", () => false);

    public static readonly SavedAttachedState<Player, bool> ForgeTokenHeld =
        new("EZMicroBalanceAscensionForgeTokenHeld", () => false);

    public static readonly SavedAttachedState<CardModel, bool> StruggleBaitGeneratedEscape =
        new("EZMicroBalanceAscensionStruggleBaitGeneratedEscape", () => false);

    public static readonly SavedAttachedState<CardModel, bool> RoyalDecreeMarkedCard =
        new("EZMicroBalanceAscensionRoyalDecreeMarkedCard", () => false);

    public static readonly SavedAttachedState<CardModel, bool> RoyalDecreePlayedCard =
        new("EZMicroBalanceAscensionRoyalDecreePlayedCard", () => false);

    public static readonly SavedAttachedState<CardModel, bool> RoyalDecreePlayedBoundCard =
        new("EZMicroBalanceAscensionRoyalDecreePlayedBoundCard", () => false);

    public static readonly SavedAttachedState<RootBud, bool> RootBudEnteredHand =
        new("EZMicroBalanceAscensionRootBudEnteredHand", () => false);

    public static readonly SavedAttachedState<RootBud, bool> RootBudPlayed =
        new("EZMicroBalanceAscensionRootBudPlayed", () => false);

    public static readonly SavedAttachedState<RootBud, bool> RootBudSprouted =
        new("EZMicroBalanceAscensionRootBudSprouted", () => false);

    public static readonly SavedAttachedState<RootBud, bool> RootBudPlantedInSeedbed =
        new("EZMicroBalanceAscensionRootBudPlantedInSeedbed", () => false);

    public static readonly SavedAttachedState<RootBud, int> RootBudSproutRound =
        new("EZMicroBalanceAscensionRootBudSproutRound", () => RootBud.DefaultSproutRound);
}
