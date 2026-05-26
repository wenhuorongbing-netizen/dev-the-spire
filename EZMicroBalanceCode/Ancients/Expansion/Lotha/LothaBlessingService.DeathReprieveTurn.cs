using EZMicroBalance.EZMicroBalanceCode.Diagnostics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int DeathReprieveCards = 10;
    private const int DeathReprieveEnergy = 10;

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
        var activeProgress = GetProgress(player) with
        {
            DeathReprieveUsed = true,
            DeathReprievePhase = DeathReprievePhase.Active
        };
        SetProgress(player, activeProgress);
        await CreatureCmd.SetCurrentHp(player.Creature, 1m);
        await EnsureDeathReprievePower(choiceContext, player);
        await CardPileCmd.Draw(choiceContext, DeathReprieveCards, player);
        await PlayerCmd.GainEnergy(DeathReprieveEnergy, player);
        ReleaseEvidenceLog.Log(
            "LothaDeathReprieve",
            "lethal_prevented",
            player,
            DeathReprieveDiagnostics(player, combatState, activeProgress, source));
        MainFile.Logger.Info($"[Spire Plus] Lotha Death Reprieve started the reprieve turn from {source}: draw 10, Energy 10, all costs 0.");
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
            ReleaseEvidenceLog.Log(
                "LothaDeathReprieve",
                "resolved",
                player,
                DeathReprieveDiagnostics(player, combatState, GetProgress(player), "turn end with enemies alive", forcedDeath: true));
            MainFile.Logger.Info("[Spire Plus] Lotha Death Reprieve ended with enemies alive; killing the player with force=true.");
            await CreatureCmd.Kill(player.Creature, force: true);
            return;
        }

        ReleaseEvidenceLog.Log(
            "LothaDeathReprieve",
            "resolved",
            player,
            DeathReprieveDiagnostics(player, combatState, GetProgress(player), "turn end after victory"));
        MainFile.Logger.Info("[Spire Plus] Lotha Death Reprieve ended after victory; the run continues.");
    }

    private static bool IsDeathReprieveCostFree(Player player, LothaCombatState combatState) =>
        GetSelectedBlessing(player) == LothaBlessingIds.DeathReprieve &&
        combatState.DeathReprieveActive;
}
