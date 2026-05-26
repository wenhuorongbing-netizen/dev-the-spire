namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task ApplyBannerCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        switch (banner)
        {
            case BannerKind.Vanguard:
                await ApplyVanguardCombatStart(combatState);
                break;
            case BannerKind.Shieldwall:
                await ApplyShieldwallCombatStart(combatState, tracker);
                break;
            case BannerKind.BloodPrize:
                await ApplyBloodPrizeCombatStart(combatState, tracker);
                break;
            case BannerKind.PressingLine:
                tracker.PressingLineRound = combatState.RoundNumber;
                tracker.PressingLineCardsPlayed.Clear();
                tracker.PressingLineLayers.Clear();
                MainFile.Logger.Info("[Spire Plus] Ascension A16 applied: Pressing Line banner is tracking card play this combat.");
                break;
            case BannerKind.LastStand:
                ApplyLastStandCombatStart(combatState);
                break;
        }
    }

    private static async Task ApplyBannerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        switch (banner)
        {
            case BannerKind.Vanguard:
                if (combatState.RoundNumber >= VanguardRemovalRound && !tracker.VanguardStrengthRemoved)
                {
                    tracker.VanguardStrengthRemoved = true;
                    await RemoveVanguardStrength(combatState);
                    MainFile.Logger.Info("[Spire Plus] Ascension A16 applied: Vanguard banner temporary Strength expired.");
                }

                break;
            case BannerKind.BloodPrize:
                await ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: false);
                break;
            case BannerKind.PressingLine:
                if (tracker.PressingLineRound != combatState.RoundNumber)
                {
                    tracker.PressingLineRound = combatState.RoundNumber;
                    tracker.PressingLineCardsPlayed.Clear();
                    tracker.PressingLineLayers.Clear();
                }

                break;
        }
    }

    private static async Task AfterBannerEnemyHpChanged(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner,
        Creature creature)
    {
        switch (banner)
        {
            case BannerKind.Shieldwall:
                await ApplyShieldwallDeathBlock(combatState, tracker, creature);
                break;
            case BannerKind.BloodPrize:
                TrackBloodPrizeKill(combatState, tracker, creature);
                break;
        }
    }
}
