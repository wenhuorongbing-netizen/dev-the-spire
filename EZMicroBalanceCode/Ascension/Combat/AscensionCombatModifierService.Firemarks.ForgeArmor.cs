namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private const int MaxForgeArmorShatters = 2;

    private static async Task ApplyForgeArmorFiremarkCombatStart(CombatState combatState, Creature host)
    {
        var block = GetForgeArmorBlock(combatState);
        await PowerCmd.Apply<ForgeArmorMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, block, host, null);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension A12 applied: Forge Armor firemark host {host.Name} will gain {block} Block after its turn.");
    }

    private static async Task ApplyForgeArmorGain(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        var host = tracker.FiremarkHost;
        if (host == null || !host.IsAlive)
        {
            return;
        }

        tracker.FiremarkArmorGeneratedThisTurn = false;
        if (tracker.FiremarkArmorSkippedNextTurn)
        {
            tracker.FiremarkArmorSkippedNextTurn = false;
            MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Forge Armor skipped after the armor was shattered.");
            return;
        }

        var block = GetForgeArmorBlock(combatState);
        tracker.FiremarkArmorGeneratedThisTurn = true;
        await CreatureCmd.GainBlock(host, block, ValueProp.Move, null, fast: true);
    }

    private static void ResolveForgeArmorShatter(AscensionCombatTracker tracker)
    {
        if (!tracker.FiremarkArmorGeneratedThisTurn ||
            tracker.FiremarkArmorBreaks >= MaxForgeArmorShatters ||
            tracker.FiremarkHost is not { IsAlive: true } host ||
            host.Block > 0)
        {
            return;
        }

        tracker.FiremarkArmorGeneratedThisTurn = false;
        tracker.FiremarkArmorBreaks++;
        tracker.FiremarkArmorSkippedNextTurn = true;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Forge Armor shattered and will skip the next armor gain.");
    }
}
