namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int DeathReprieveCards = 10;
    private const int DeathReprieveEnergy = 10;

    public static bool ShouldDieLate(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return true;
        }

        if (creature.Player is not { } player)
        {
            return true;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
        {
            return false;
        }

        return GetProgress(player).DeathReprieveUsed;
    }

    public static bool ShouldDie(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return true;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        return !(combatState.DeathReprieveActive || combatState.DeathReprievePendingStart);
    }

    public static async Task AfterPreventingDeath(Creature creature)
    {
        if (!creature.IsPlayer)
        {
            return;
        }

        if (creature.Player is not { } player)
        {
            return;
        }

        if (GetSelectedBlessing(player) != LothaBlessingIds.DeathReprieve)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        HydrateDeathReprieveState(player, combatState);
        var progress = GetProgress(player);
        if (progress.DeathReprieveUsed)
        {
            if (combatState.DeathReprieveActive || combatState.DeathReprievePendingStart)
            {
                await CreatureCmd.SetCurrentHp(creature, 1m);
                MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve kept the player at 1 HP during the reprieve turn.");
            }

            return;
        }

        await CreatureCmd.SetCurrentHp(creature, 1m);

        if (creature.CombatState?.CurrentSide == CombatSide.Player &&
            CombatManager.Instance.IsPartOfPlayerTurn(player))
        {
            SetProgress(player, progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.Active
            });
            await StartDeathReprieveTurn(new ThrowingPlayerChoiceContext(), player, combatState, "current player turn after lethal damage");
        }
        else
        {
            SetProgress(player, progress with
            {
                DeathReprieveUsed = true,
                DeathReprievePhase = DeathReprievePhase.PendingStart
            });
            combatState.DeathReprievePendingStart = true;
            combatState.DeathReprieveActive = true;
            await EnsureDeathReprievePower(new ThrowingPlayerChoiceContext(), player);
            MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve prevented lethal damage; reprieve turn is pending at the next player turn.");
        }
    }

    private static async Task StartDeathReprieveTurn(
        PlayerChoiceContext choiceContext,
        Player player,
        LothaCombatState combatState,
        string source)
    {
        if (combatState.DeathReprieveStarted)
        {
            return;
        }

        combatState.DeathReprieveStarted = true;
        combatState.DeathReprieveActive = true;
        combatState.DeathReprievePendingStart = false;
        SetProgress(player, GetProgress(player) with
        {
            DeathReprieveUsed = true,
            DeathReprievePhase = DeathReprievePhase.Active
        });
        await CreatureCmd.SetCurrentHp(player.Creature, 1m);
        await EnsureDeathReprievePower(choiceContext, player);
        await CardPileCmd.Draw(choiceContext, DeathReprieveCards, player);
        await PlayerCmd.GainEnergy(DeathReprieveEnergy, player);
        MainFile.Logger.Info($"[EZMicroBalance] Lotha Death Reprieve started the reprieve turn from {source}: draw 10, Energy 10, all costs 0.");
    }

    private static async Task EnsureDeathReprievePower(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature.GetPower<LothaDeathReprievePower>() != null)
        {
            return;
        }

        await PowerCmd.Apply<LothaDeathReprievePower>(
            choiceContext,
            player.Creature,
            1,
            player.Creature,
            null);
    }

    private static async Task ResolveDeathReprieveTurnEnd(Player player, LothaCombatState combatState)
    {
        combatState.DeathReprieveActive = false;
        combatState.DeathReprievePendingStart = false;
        ResolveDeathReprieveProgress(player);
        await PowerCmd.Remove<LothaDeathReprievePower>(player.Creature);

        if (player.Creature.CombatState?.Enemies.Any(enemy => enemy.IsAlive) == true)
        {
            MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve ended with enemies alive; killing the player with force=true.");
            await CreatureCmd.Kill(player.Creature, force: true);
            return;
        }

        MainFile.Logger.Info("[EZMicroBalance] Lotha Death Reprieve ended after victory; the run continues.");
    }

    private static bool IsDeathReprieveCostFree(Player player, LothaCombatState combatState) =>
        GetSelectedBlessing(player) == LothaBlessingIds.DeathReprieve &&
        combatState.DeathReprieveActive;
}
