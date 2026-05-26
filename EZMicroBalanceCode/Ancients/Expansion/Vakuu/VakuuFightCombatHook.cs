using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using MegaCrit.Sts2.Core.Modding;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class VakuuFightCombatHook : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterCreatureAddedToCombat(Creature creature) =>
        VakuuFightService.AfterCreatureAddedToCombat(creature);

    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource) =>
        VakuuFightService.AfterDamageGiven(choiceContext, dealer, result, props, target, cardSource);

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) =>
        VakuuContractService.AfterPlayerTurnStart(choiceContext, player);
}
