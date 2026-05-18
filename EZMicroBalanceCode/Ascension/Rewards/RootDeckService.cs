using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class RootDeckService
{
    public const int MaxRootblightLevel = 3;
    public const int MaxRootblightCards = 4;

    public static IReadOnlyList<RootFamilyCard> FindRootFamilyCards(Player player)
    {
        return EnumerateRootFamilyCards(player)
            .Select(entry => entry.Card)
            .ToList();
    }
}
