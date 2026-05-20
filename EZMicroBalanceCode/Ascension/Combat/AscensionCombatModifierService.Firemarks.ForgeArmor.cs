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
        tracker.FiremarkArmorBlockBaseline = 0m;
        tracker.FiremarkArmorRemainingThisTurn = 0m;
        if (tracker.FiremarkArmorSkippedNextTurn)
        {
            tracker.FiremarkArmorSkippedNextTurn = false;
            MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Forge Armor skipped after the armor was shattered.");
            return;
        }

        var block = GetForgeArmorBlock(combatState);
        tracker.FiremarkArmorGeneratedThisTurn = true;
        tracker.FiremarkArmorBlockBaseline = host.Block;
        tracker.FiremarkArmorRemainingThisTurn = block;
        await CreatureCmd.GainBlock(host, block, ValueProp.Move, null, fast: true);
    }

    private static void TrackForgeArmorBlockedDamage(AscensionCombatTracker tracker, Creature target, DamageResult result)
    {
        if (!tracker.FiremarkArmorGeneratedThisTurn ||
            target != tracker.FiremarkHost ||
            result.BlockedDamage <= 0)
        {
            return;
        }

        tracker.FiremarkArmorRemainingThisTurn = Math.Max(
            0m,
            tracker.FiremarkArmorRemainingThisTurn - result.BlockedDamage);
    }

    private static void ResolveForgeArmorShatter(AscensionCombatTracker tracker)
    {
        if (!tracker.FiremarkArmorGeneratedThisTurn ||
            tracker.FiremarkArmorBreaks >= MaxForgeArmorShatters ||
            tracker.FiremarkHost is not { IsAlive: true })
        {
            tracker.FiremarkArmorGeneratedThisTurn = false;
            tracker.FiremarkArmorBlockBaseline = 0m;
            tracker.FiremarkArmorRemainingThisTurn = 0m;
            return;
        }

        var armorWasShattered =
            tracker.FiremarkArmorRemainingThisTurn <= 0m ||
            tracker.FiremarkHost.Block <= tracker.FiremarkArmorBlockBaseline;
        if (!armorWasShattered)
        {
            tracker.FiremarkArmorGeneratedThisTurn = false;
            tracker.FiremarkArmorBlockBaseline = 0m;
            tracker.FiremarkArmorRemainingThisTurn = 0m;
            return;
        }

        tracker.FiremarkArmorGeneratedThisTurn = false;
        tracker.FiremarkArmorBlockBaseline = 0m;
        tracker.FiremarkArmorRemainingThisTurn = 0m;
        tracker.FiremarkArmorBreaks++;
        tracker.FiremarkArmorSkippedNextTurn = true;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A12 applied: Forge Armor shattered and will skip the next armor gain.");
    }
}
