namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int MossMapMonsterGold = 25;
    private const int MossMapEventHeal = 5;
    private const int MossMapRestMaxHp = 3;

    private static async Task ApplyMossMapRoomReward(Player player, RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Monster:
                await PlayerCmd.GainGold(MossMapMonsterGold, player);
                break;
            case RoomType.Event:
                await CreatureCmd.Heal(player.Creature, MossMapEventHeal);
                break;
            case RoomType.Shop:
                await TryGivePotion(player);
                break;
            case RoomType.Elite:
                UpgradeRandomCard(player);
                break;
            case RoomType.RestSite:
                await CreatureCmd.GainMaxHp(player.Creature, MossMapRestMaxHp);
                break;
        }

        MainFile.Logger.Info($"[Spire Plus] Urda Moss Map applied: first Act 1 {roomType} room reward granted.");
    }

    private static async Task TryGivePotion(Player player)
    {
        if (!player.HasOpenPotionSlots)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Moss Map skipped shop potion: no open potion slot.");
            return;
        }

        var potion = PotionFactory.CreateRandomPotionOutOfCombat(player, player.PlayerRng.Rewards).ToMutable();
        await PotionCmd.TryToProcure(potion, player);
    }

    private static void UpgradeRandomCard(Player player)
    {
        var target = PileType.Deck.GetPile(player).Cards
            .Where(card => card.IsUpgradable)
            .ToList()
            .StableShuffle(player.PlayerRng.Rewards)
            .FirstOrDefault();
        if (target == null)
        {
            MainFile.Logger.Info("[Spire Plus] Urda Moss Map skipped elite upgrade: no upgradable card.");
            return;
        }

        CardCmd.Upgrade(target, CardPreviewStyle.HorizontalLayout);
    }
}
