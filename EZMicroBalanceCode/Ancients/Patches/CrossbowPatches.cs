using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class CrossbowOfferPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "crossbow-offer";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Crossbow to offer temporary attack cards on side turn start";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))];
    [HarmonyPrefix]
    private static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState, ref Task __result)
    {
        if (__instance is not Crossbow crossbow)
        {
            return true;
        }

        if (side != crossbow.Owner.Creature.Side)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = OfferTemporaryAttack(crossbow, choiceContext, combatState);
        return false;
    }

    private static async Task OfferTemporaryAttack(Crossbow crossbow, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        var owner = crossbow.Owner;
        var attackPool = owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Type == CardType.Attack && card.CanBeGeneratedInCombat)
            .ToList();
        var generated = CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generated == null)
        {
            MainFile.Logger.Warn("[Spire Plus] Crossbow skipped: no eligible attack generated.");
            return;
        }

        AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1);
        AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { generated }, owner, canSkip: true);
        if (selected == generated)
        {
            crossbow.Flash();
            var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner);
            if (addResult is { success: true })
            {
                MainFile.Logger.Info($"[Spire Plus] Crossbow applied: accepted temporary attack {generated.Id.Entry}.");
                return;
            }

            MainFile.Logger.Warn($"[Spire Plus] Crossbow skipped: accepted temporary attack {generated.Id.Entry} could not be added to combat.");
            return;
        }

        AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState);
        MainFile.Logger.Info($"[Spire Plus] Crossbow applied: skipped temporary attack {generated.Id.Entry}.");
    }
}

internal sealed class CrossbowVanillaAfterTurnPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "crossbow-vanilla-after-turn";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Suppress vanilla Crossbow after-side-turn-start behavior";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart))];
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}
