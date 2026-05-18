namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private const int BountyDeadlineRound = 3;

    private static async Task ApplyBloodPrizeCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        tracker.BloodPrizeTarget = PickBannerTarget(combatState);
        if (tracker.BloodPrizeTarget != null)
        {
            await PowerCmd.Apply<BloodPrizeBannerTargetPower>(new BlockingPlayerChoiceContext(), tracker.BloodPrizeTarget, 1m, tracker.BloodPrizeTarget, null);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Ascension A16 applied: Blood Prize target set to {tracker.BloodPrizeTarget.Name}.");
        }
        else
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A16 gate active: Blood Prize had no living enemy target.");
        }
    }

    private static void TrackBloodPrizeKill(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature creature)
    {
        if (creature == tracker.BloodPrizeTarget &&
            creature.IsDead &&
            !tracker.BloodPrizeExpired &&
            combatState.RoundNumber <= BountyDeadlineRound)
        {
            tracker.BloodPrizeKilledEarly = true;
            MainFile.Logger.Info("[EZMicroBalance] Ascension A16 tracked: Blood Prize target killed before the deadline.");
        }
    }

    private static async Task ApplyBloodPrizePenaltyIfExpired(
        CombatState combatState,
        AscensionCombatTracker tracker,
        bool includeCurrentRound)
    {
        if (tracker.BloodPrizeExpired ||
            tracker.BloodPrizeKilledEarly ||
            combatState.RoundNumber < BountyDeadlineRound ||
            (!includeCurrentRound && combatState.RoundNumber <= BountyDeadlineRound))
        {
            return;
        }

        tracker.BloodPrizeExpired = true;
        var target = tracker.BloodPrizeTarget;
        if (target == null || !target.IsAlive)
        {
            return;
        }

        var strength = GetBloodPrizeRetaliationStrength(combatState);
        var artifact = GetBloodPrizeRetaliationArtifact(combatState);
        if (IsLikelyAttacker(target))
        {
            await ApplyStrengthAndArtifact(target, strength, artifact);
            await PowerCmd.Apply<BloodPrizeRetaliationPower>(new BlockingPlayerChoiceContext(), target, strength, target, null);
        }
        else
        {
            await ApplyStrengthToEnemies(
                PrimaryAliveEnemies(combatState),
                Math.Ceiling(strength / 2m));
        }

        await PowerCmd.Remove(target.GetPower<BloodPrizeBannerTargetPower>());
        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Blood Prize target survived the deadline and retaliated.");
    }

    private static void TryAddBountyReward(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        if (banner != BannerKind.BloodPrize ||
            tracker.BloodPrizeRewardAdded ||
            tracker.BloodPrizeExpired)
        {
            return;
        }

        if (!tracker.BloodPrizeKilledEarly &&
            tracker.BloodPrizeTarget is { IsDead: true } &&
            combatState.RoundNumber <= BountyDeadlineRound)
        {
            tracker.BloodPrizeKilledEarly = true;
        }

        if (!tracker.BloodPrizeKilledEarly ||
            combatState.RunState.CurrentRoom is not CombatRoom room)
        {
            return;
        }

        tracker.BloodPrizeRewardAdded = true;
        var reward = GetBloodPrizeGoldReward(combatState);
        var activePlayers = combatState.Players.Where(player => player.IsActiveForHooks).ToList();
        var playerReward = activePlayers.Count > 1
            ? (int)Math.Ceiling(reward * 0.5m)
            : reward;
        foreach (var player in activePlayers)
        {
            room.AddExtraReward(player, new GoldReward(playerReward, player));
        }

        MainFile.Logger.Info($"[EZMicroBalance] Ascension A16 applied: Blood Prize reward added {playerReward} Gold per active player.");
    }
}
