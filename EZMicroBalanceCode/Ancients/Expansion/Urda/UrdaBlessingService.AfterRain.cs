namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int AfterRainGoldPayoff = 75;
    private const int AfterRainRecoveryHeal = 8;
    private const int AfterRainCleanActOneThreshold = 3;

    public static async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!target.IsPlayer ||
            target.Player is not { } player ||
            !player.IsActiveForHooks ||
            player.RunState.CurrentActIndex != 0 ||
            GetSelectedBlessing(player) != UrdaBlessingIds.AfterRain ||
            !IsAfterRainTriggerDamage(result, props, dealer, cardSource))
        {
            return;
        }

        var progress = GetProgress(player);
        if (progress.AfterRainTriggeredThisCombat)
        {
            return;
        }

        SetProgress(
            player,
            progress with
            {
                AfterRainTriggeredThisCombat = true,
                AfterRainTriggerCount = progress.AfterRainTriggerCount + 1
            });

        if (player.Creature.CombatState is { } combatState)
        {
            var rainBreath = combatState.CreateCard<UrdaRainBreath>(player);
            await CardPileCmd.AddGeneratedCardToCombat(rainBreath, PileType.Hand, player);
        }

        MainFile.Logger.Info("[Spire Plus] Urda After the Rain triggered: added Rain Breath after first unblocked enemy attack damage this combat.");
    }

    private static void ResetAfterRainCombatTrigger(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainTriggeredThisCombat)
        {
            SetProgress(player, progress with { AfterRainTriggeredThisCombat = false });
        }
    }

    private static async Task CompensateAfterRainAtActTwo(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainCompensated)
        {
            return;
        }

        SetProgress(player, progress with { AfterRainCompensated = true, AfterRainTriggeredThisCombat = false });
        if (progress.AfterRainTriggerCount < AfterRainCleanActOneThreshold)
        {
            await PlayerCmd.GainGold(AfterRainGoldPayoff, player);
            MainFile.Logger.Info(
                $"[Spire Plus] Urda After the Rain Act 2 payoff granted {AfterRainGoldPayoff} Gold after {progress.AfterRainTriggerCount} Act 1 trigger(s).");
            return;
        }

        await CreatureCmd.Heal(player.Creature, AfterRainRecoveryHeal);
        var selected = (await CardSelectCmd.FromDeckForUpgrade(
            player,
            new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 0, 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();
        if (selected != null)
        {
            CardCmd.Upgrade(selected);
        }

        MainFile.Logger.Info(
            $"[Spire Plus] Urda After the Rain Act 2 payoff healed {AfterRainRecoveryHeal} HP and upgraded 1 card after {progress.AfterRainTriggerCount} Act 1 trigger(s).");
    }

    private static bool IsAfterRainTriggerDamage(
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) =>
        result.UnblockedDamage > 0 &&
        !result.WasFullyBlocked &&
        dealer is { IsEnemy: true } &&
        cardSource == null &&
        props.HasFlag(ValueProp.Move);
}
