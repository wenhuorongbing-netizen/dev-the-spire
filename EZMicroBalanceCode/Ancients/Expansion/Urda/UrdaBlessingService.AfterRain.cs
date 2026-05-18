namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private const int AfterRainBlock = 15;
    private const int AfterRainDraw = 1;
    private const int AfterRainWounds = 2;
    private const int AfterRainMaxHpLoss = 3;
    private const int AfterRainCompensationHeal = 8;
    private const int AfterRainCompensationGold = 75;
    private const int AfterRainEliteGold = 20;
    private const int AfterRainEliteGoldLimit = 2;

    public static bool ShouldDieLate(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            player.RunState.CurrentActIndex != 0 ||
            GetSelectedBlessing(player) != UrdaBlessingIds.AfterRain)
        {
            return true;
        }

        return GetProgress(player).AfterRainSpent;
    }

    public static async Task AfterPreventingDeath(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            player.RunState.CurrentActIndex != 0 ||
            GetSelectedBlessing(player) != UrdaBlessingIds.AfterRain)
        {
            return;
        }

        var progress = GetProgress(player);
        if (progress.AfterRainSpent)
        {
            await CreatureCmd.SetCurrentHp(creature, 1m);
            return;
        }

        SetProgress(player, progress with { AfterRainSpent = true });
        await CreatureCmd.SetCurrentHp(creature, 1m);
        await CreatureCmd.GainBlock(creature, AfterRainBlock, ValueProp.Move, null, fast: true);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), AfterRainDraw, player);
        if (player.Creature.CombatState is { } combatState)
        {
            for (var i = 0; i < AfterRainWounds; i++)
            {
                var wound = combatState.CreateCard<Wound>(player);
                await CardPileCmd.AddGeneratedCardToCombat(wound, PileType.Discard, player);
            }
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, AfterRainMaxHpLoss, isFromCard: false);
        MainFile.Logger.Info("[EZMicroBalance] Urda After the Rain prevented lethal Act 1 damage and spent the blessing.");
    }

    private static async Task GrantAfterRainEliteGold(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainSpent || progress.AfterRainEliteGoldCount >= AfterRainEliteGoldLimit)
        {
            return;
        }

        SetProgress(player, progress with { AfterRainEliteGoldCount = progress.AfterRainEliteGoldCount + 1 });
        await PlayerCmd.GainGold(AfterRainEliteGold, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda After the Rain Elite bonus granted {AfterRainEliteGold} Gold ({progress.AfterRainEliteGoldCount + 1}/{AfterRainEliteGoldLimit}).");
    }

    private static async Task CompensateAfterRainAtActTwo(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainSpent || progress.AfterRainCompensated)
        {
            return;
        }

        SetProgress(player, progress with { AfterRainCompensated = true });
        await CreatureCmd.Heal(player.Creature, AfterRainCompensationHeal);
        await PlayerCmd.GainGold(AfterRainCompensationGold, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda After the Rain Act 2 compensation granted {AfterRainCompensationHeal} HP and {AfterRainCompensationGold} Gold.");
    }
}
