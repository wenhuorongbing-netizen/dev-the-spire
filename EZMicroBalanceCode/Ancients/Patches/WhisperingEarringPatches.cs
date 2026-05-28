using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class WhisperingEarringPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "w-h-i-s-p-e-r-i-n-g-e-a-r-r-i-n-g-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))];
{
    [HarmonyPrefix]
    private static bool Prefix(WhisperingEarring __instance, PlayerChoiceContext choiceContext, Player player, ref Task __result)
    {
        if (player != __instance.Owner)
        {
            return true;
        }

        __result = AutoPlayOneHighestCostCard(__instance, choiceContext, player);
        return false;
    }

    private static async Task AutoPlayOneHighestCostCard(WhisperingEarring earring, PlayerChoiceContext choiceContext, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (combatState.RoundNumber > 3)
        {
            return;
        }

        var card = PileType.Hand.GetPile(player).Cards
            .Select((card, index) => new { Card = card, Index = index })
            .Where(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay))
            .OrderByDescending(item => VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card)))
            .ThenBy(item => item.Index)
            .Select(item => item.Card)
            .FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var target = AncientCardHelpers.GetPreferredTarget(card, combatState, player);
        if (card.TargetType is TargetType.AnyEnemy or TargetType.AnyAlly && !card.CanPlayTargeting(target))
        {
            return;
        }

        earring.Flash();
        await VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources);
        await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
        MainFile.Logger.Info($"[Spire Plus] WhisperingEarring applied: auto-played {card.Id.Entry} on round {combatState.RoundNumber}.");
    }
}


