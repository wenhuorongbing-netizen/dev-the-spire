namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static BannerKind ResolveBannerForCombat(CombatState combatState, AscensionNodeMetadata metadata)
    {
        var banner = metadata.Banner!.Value;
        if (!RequiresMultiplePrimaryEnemies(banner) ||
            HasMultiplePrimaryEnemies(combatState))
        {
            return banner;
        }

        var fallback = BannerKind.BloodPrize;
        metadata.Banner = fallback;
        MainFile.Logger.Info(
            $"[Spire Plus] Ascension A16 converted {banner} banner to {fallback}: this combat has one primary enemy.");
        return fallback;
    }

    private static bool RequiresMultiplePrimaryEnemies(BannerKind banner) =>
        banner is BannerKind.Shieldwall or BannerKind.LastStand;

    private static Creature? PickBannerTarget(CombatState combatState)
    {
        var candidates = PrimaryAliveEnemies(combatState).ToList();

        if (candidates.Count == 0)
        {
            candidates = AliveEnemies(combatState).ToList();
        }

        return candidates
            .OrderByDescending(enemy => enemy.MaxHp)
            .ThenBy(enemy => combatState.Enemies.IndexOf(enemy))
            .FirstOrDefault();
    }
}
