using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal static partial class LothaBlessingService
{
    private const int PublicEvidenceEnlightenmentGain = 1;
    private const int PublicEvidenceConsumeLimit = 3;
    private const int PublicEvidenceBlockPerEnlightenment = 4;
    private const int PublicEvidenceCardsPerEnlightenment = 1;

    public static decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target)
    {
        if (amount == 0m ||
            target is not { IsEnemy: true } ||
            !giver.IsPlayer ||
            giver.Player is not { } player ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence ||
            !IsPublicEvidenceDebuffApplication(power, amount))
        {
            return 0m;
        }

        MainFile.Logger.Info($"[Spire Plus] Lotha Public Evidence doubled player-applied debuff {power.Id.Entry}.");
        return amount;
    }

    public static bool TryModifyPowerAmountReceived(
        PowerModel canonicalPower,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (amount == 0m ||
            !target.IsPlayer ||
            target.Player is not { } player ||
            !player.IsActiveForHooks ||
            applier is not { IsEnemy: true } ||
            GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence ||
            !IsPublicEvidenceDebuffApplication(canonicalPower, amount))
        {
            return false;
        }

        modifiedAmount = amount * 2m;
        MainFile.Logger.Info($"[Spire Plus] Lotha Public Evidence doubled enemy-applied debuff {canonicalPower.Id.Entry}.");
        return true;
    }

    public static async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount == 0m || !IsPublicEvidenceDebuffApplication(power, amount))
        {
            return;
        }

        if (applier is { IsPlayer: true, Player: { } applyingPlayer } &&
            applyingPlayer.IsActiveForHooks &&
            power.Owner.IsEnemy &&
            GetSelectedBlessing(applyingPlayer) == LothaBlessingIds.PublicEvidence)
        {
            await PowerCmd.Apply<LothaEnlightenmentPower>(
                choiceContext,
                applyingPlayer.Creature,
                PublicEvidenceEnlightenmentGain,
                applyingPlayer.Creature,
                cardSource);
            MainFile.Logger.Info("[Spire Plus] Lotha Public Evidence granted Enlightenment after a player-applied debuff.");
            return;
        }

        if (power.Owner is { IsPlayer: true, Player: { } targetPlayer } &&
            targetPlayer.IsActiveForHooks &&
            applier is { IsEnemy: true } &&
            GetSelectedBlessing(targetPlayer) == LothaBlessingIds.PublicEvidence)
        {
            await RemoveOnePublicEvidenceEnlightenment(choiceContext, targetPlayer);
            MainFile.Logger.Info("[Spire Plus] Lotha Public Evidence removed Enlightenment after an enemy-applied debuff.");
        }
    }

    private static async Task ConsumePublicEvidenceEnlightenmentAtTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var enlightenment = player.Creature.GetPower<LothaEnlightenmentPower>();
        if (enlightenment is not { Amount: > 0 })
        {
            return;
        }

        var consumed = Math.Min(PublicEvidenceConsumeLimit, enlightenment.Amount);
        await PowerCmd.ModifyAmount(choiceContext, enlightenment, -consumed, player.Creature, null);
        for (var i = 0; i < consumed; i++)
        {
            await CardPileCmd.Draw(choiceContext, PublicEvidenceCardsPerEnlightenment, player);
            await CreatureCmd.GainBlock(player.Creature, PublicEvidenceBlockPerEnlightenment, ValueProp.Move, null, fast: true);
        }

        MainFile.Logger.Info($"[Spire Plus] Lotha Public Evidence consumed {consumed} Enlightenment at turn start.");
    }

    private static async Task RemoveOnePublicEvidenceEnlightenment(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var enlightenment = player.Creature.GetPower<LothaEnlightenmentPower>();
        if (enlightenment is not { Amount: > 0 })
        {
            return;
        }

        await PowerCmd.Decrement(enlightenment);
    }

    private static bool IsPublicEvidenceDebuffApplication(PowerModel power, decimal amount) =>
        power.GetTypeForAmount(amount) == PowerType.Debuff &&
        !IsPublicEvidenceExcludedDamageDebuff(power);

    private static bool IsPublicEvidenceExcludedDamageDebuff(PowerModel power)
    {
        // Core v0.106.1 models these as Debuffs, but their source resolves damage, kill, or poison ticks.
        return power is PoisonPower
            or ConstrictPower
            or DemisePower
            or DisintegrationPower
            or DoomPower
            or MagicBombPower
            or StranglePower
            or TheGambitPower;
    }
}
