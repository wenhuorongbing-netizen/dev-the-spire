using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static Creature? FindCeremonialBeast(CombatState combatState) =>
        AliveEnemies(combatState)
            .FirstOrDefault(enemy => enemy.Monster is CeremonialBeast);

    private static async Task TryApplyHolyDaze(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.HolyDaze || tracker.HolyDazeTriggered)
        {
            return;
        }

        var beast = FindCeremonialBeast(combatState);
        if (beast == null ||
            beast.HasPower<PlowPower>() ||
            beast.Monster?.NextMove.StateId != "STUN_MOVE")
        {
            return;
        }

        tracker.HolyDazeTriggered = true;
        var strengthAfterDaze = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<HolyDazePower>(new BlockingPlayerChoiceContext(), beast, strengthAfterDaze, beast, null);
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Holy Daze capped Ceremonial Beast's first stun damage window.");
    }

    private static async Task EndHolyDaze(CombatState combatState, AscensionCombatTracker tracker)
    {
        var beast = FindCeremonialBeast(combatState);
        var daze = beast?.GetPower<HolyDazePower>();
        if (beast == null || daze == null)
        {
            return;
        }

        await PowerCmd.Remove(daze);
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), beast, daze.Amount, beast, null);
        MainFile.Logger.Info("[Spire Plus] Ascension A19 applied: Holy Daze ended and granted Strength.");
    }
}
