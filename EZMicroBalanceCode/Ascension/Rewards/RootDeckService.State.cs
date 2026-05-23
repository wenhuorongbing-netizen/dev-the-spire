namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

internal static partial class RootDeckService
{
    public static int GetRootblightLevel(Player player)
    {
        return GetLevel(player);
    }

    private static int GetLevel(Player player)
    {
        var deckLevel = FindRootFamilyCards(player)
            .Select(card => card.RootblightLevel)
            .DefaultIfEmpty(0)
            .Max();
        var cachedLevel = Math.Clamp(AscensionSavedStateFields.RootblightLevel[player], 0, MaxRootblightLevel);
        return Math.Clamp(Math.Max(cachedLevel, deckLevel), 0, MaxRootblightLevel);
    }

    private static void SetLevel(Player player, int level)
    {
        AscensionSavedStateFields.RootblightLevel[player] = Math.Clamp(level, 0, MaxRootblightLevel);
    }

    private static void SetDiagnosticLevelFromDeck(Player player)
    {
        SetLevelFromCards(player, FindRootFamilyCards(player));
        if (FindRootFamilyCards(player).Count > 0)
        {
            ReleaseEvidenceLog.Log(
                "Rootblight",
                "state_hydrated_from_deck",
                player,
                new Dictionary<string, object?>
                {
                    ["level"] = GetLevel(player),
                    ["cards"] = FindRootFamilyCards(player).Count
                });
        }
    }

    private static void SetLevelFromCards(Player player, IEnumerable<RootFamilyCard> cards)
    {
        SetLevel(
            player,
            cards
                .Select(card => card.RootblightLevel)
                .DefaultIfEmpty(0)
                .Max());
    }

    private static bool HasRootBeginsApplied(Player player)
    {
        return AscensionSavedStateFields.RootBeginsApplied[player] ||
            FindRootFamilyCards(player).Count > 0;
    }

    private static void MarkRootBeginsApplied(Player player)
    {
        AscensionSavedStateFields.RootBeginsApplied[player] = true;
    }
}
