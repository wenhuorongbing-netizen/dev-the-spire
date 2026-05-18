using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private const decimal AeonglassStrengthAmount = 5m;
    private static readonly ModelId AeonglassMonsterId = new("MONSTER", "AEONGLASS");

    private static async Task ApplyBossSealCombatStart(CombatState combatState, AscensionNodeMetadata metadata)
    {
        var definition = metadata.BossSeal;
        if (definition == null)
        {
            return;
        }

        var mode = metadata.IsBossBrand ? "A20 Brand" : "A19 Royal Seal";
        var brandText = metadata.IsBossBrand
            ? $" brand={definition.BrandSummary}"
            : string.Empty;
        MainFile.Logger.Info(
            $"[EZMicroBalance] Ascension {mode} armed: {definition.Name} ({definition.Id}) is active for this boss. evidence={definition.RuntimeEvidence}{brandText}");

        if (definition.Id == BossSealId.AeonglassStrength)
        {
            var boss = AliveEnemies(combatState)
                .FirstOrDefault(enemy => enemy.ModelId == AeonglassMonsterId);
            if (boss != null)
            {
                await PowerCmd.Apply<StrengthPower>(
                    new BlockingPlayerChoiceContext(),
                    boss,
                    AeonglassStrengthAmount,
                    boss,
                    null);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension AeonglassStrength: applied +5 Strength to {boss.ModelId.Entry}.");
                return;
            }

            MainFile.Logger.Warn("[EZMicroBalance] Ascension AeonglassStrength skipped: AEONGLASS monster was not found in combat.");
        }
    }
}
