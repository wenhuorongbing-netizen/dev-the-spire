namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static partial class MorviBlessingService
{
    private const int OverdueLibraryPageCount = 3;

    private static readonly Type[] ArchivePageTypes =
    [
        typeof(MorviArchiveDrawPage),
        typeof(MorviArchiveVeilPage),
        typeof(MorviArchiveBurnPage),
        typeof(MorviArchiveDiscountPage),
        typeof(MorviArchiveBraveryPage),
        typeof(MorviArchiveDexterityPage)
    ];

    public static void ArmOverdueLibraryDiscount(Player player, CardModel sourceCard)
    {
        var combatState = CombatStates.GetOrCreateValue(player);
        combatState.OverdueLibraryDiscountArmed = true;
        combatState.OverdueLibraryDiscountSourceCard = sourceCard;
        MainFile.Logger.Info("[EZMicroBalance] Morvi Overdue Library armed the next-card cost-0 page.");
    }

    private static async Task AddArchivePages(Player player)
    {
        if (player.Creature.CombatState == null)
        {
            return;
        }

        var pages = new List<CardModel>();
        for (var index = 0; index < OverdueLibraryPageCount; index++)
        {
            var pageType = player.RunState.Rng.CombatCardSelection.NextItem(ArchivePageTypes) ?? typeof(MorviArchiveDrawPage);
            var canonical = ModelDb.GetById<CardModel>(ModelDb.GetId(pageType));
            pages.Add(player.Creature.CombatState.CreateCard(canonical, player));
        }

        foreach (var page in pages)
        {
            await AncientCardHelpers.TryAddGeneratedCardToCombat(page, PileType.Hand, player);
        }

        MainFile.Logger.Info("[EZMicroBalance] Morvi Overdue Library added 3 random Archive Pages to hand.");
    }

    private static void TryConsumeOverdueLibraryDiscount(CardModel card, MorviCombatState combatState)
    {
        if (!combatState.OverdueLibraryDiscountArmed ||
            ReferenceEquals(combatState.OverdueLibraryDiscountSourceCard, card))
        {
            return;
        }

        combatState.OverdueLibraryDiscountArmed = false;
        combatState.OverdueLibraryDiscountSourceCard = null;
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Overdue Library consumed next-card cost-0 discount on {card.Id.Entry}.");
    }
}
