using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static class AscensionCombatModifierService
{
    private const decimal VanguardStrength = 2m;
    private const int VanguardRemovalRound = 3;
    private const decimal ShieldFormationTurnBlock = 5m;
    private const decimal ShieldFormationDeathBlock = 8m;
    private const decimal BountyPenaltyBlock = 8m;
    private const decimal BountyPenaltyArtifact = 1m;
    private const int BountyDeadlineRound = 3;
    private const int BountyGoldReward = 15;
    private const decimal AeonglassStrengthAmount = 5m;
    private static readonly ModelId AeonglassMonsterId = new("MONSTER", "AEONGLASS");

    public static async Task BeforeCombatStart(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (tracker.CombatModifiersInitialized)
        {
            return;
        }

        tracker.CombatModifiersInitialized = true;
        tracker.NodeMetadata = AscensionMapService.TryGetCurrentMetadata(combatState.RunState);

        var metadata = tracker.NodeMetadata;
        if (metadata == null)
        {
            return;
        }

        if (HasActiveFiremark(combatState, metadata))
        {
            await ApplyFiremarkCombatStart(combatState, tracker, metadata.Firemark!.Value);
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await ApplyBannerCombatStart(combatState, tracker, metadata.Banner!.Value);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealCombatStart(combatState, metadata);
        }
    }

    public static async Task AfterPlayerTurnStart(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealPlayerTurnStart(combatState, tracker, metadata);
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await ApplyBannerTurnStart(combatState, tracker, metadata.Banner!.Value);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }
    }

    public static async Task AfterCurrentHpChanged(CombatState combatState, AscensionCombatTracker tracker, Creature creature, decimal delta)
    {
        if (!combatState.Enemies.Contains(creature))
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await AfterBossSealHpChanged(combatState, tracker, metadata, creature, delta);
        }

        if (delta >= 0m)
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            await AfterBannerEnemyHpChanged(combatState, tracker, metadata.Banner!.Value, creature);
        }

        if (creature.IsDead ||
            creature.GetHpPercentRemaining() > 0.5d ||
            tracker.ThresholdShieldedEnemies.Contains(creature))
        {
            return;
        }

        await TryApplyHolyDaze(combatState, tracker, metadata);
    }

    public static async Task AfterShuffle(CombatState combatState, AscensionCombatTracker tracker, Player shuffler)
    {
        if (tracker.ChaosApplied)
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }
    }

    public static async Task AfterSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }

        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata) &&
            metadata.Banner == BannerKind.ShieldFormation)
        {
            await ApplyShieldFormationTurnBlock(combatState, tracker);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealSideTurnStart(combatState, tracker, metadata, side);
        }
    }

    public static async Task AfterTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CombatSide side)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyBossSealTurnEnd(combatState, tracker, metadata, side);
        }
    }

    public static async Task AfterCombatEnd(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (!TryRefreshNodeMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        if (HasActiveBanner(combatState, metadata))
        {
            TryAddBountyReward(combatState, tracker, metadata.Banner!.Value);
        }

        if (!tracker.ForgeTokenAwarded && HasActiveFiremark(combatState, metadata))
        {
            tracker.ForgeTokenAwarded = true;
            await ForgeTokenService.GrantAfterFiremarkedElite(combatState);
        }

        if (HasActiveBossSeal(combatState, metadata))
        {
            await ApplyA20CourtyardRecovery(combatState, tracker, metadata);
        }
    }

    public static async Task AfterDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature target,
        DamageResult result,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        await AfterBossSealDamageReceived(combatState, tracker, metadata, target, result, dealer, cardSource);
    }

    public static async Task AfterDeath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        Creature creature,
        bool wasRemovalPrevented)
    {
        if (wasRemovalPrevented)
        {
            return;
        }

        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        await AfterBossSealDeath(combatState, tracker, metadata, creature);
    }

    public static async Task AfterCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardPlay cardPlay)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return;
        }

        await AfterBossSealCardPlayed(combatState, tracker, metadata, cardPlay);
    }

    public static Task AfterCardEnteredHand(
        CombatState combatState,
        AscensionCombatTracker tracker,
        CardModel card)
    {
        if (!TryRefreshActiveBossSealMetadata(combatState, tracker, out var metadata))
        {
            return Task.CompletedTask;
        }

        TryAssignChosenDecree(combatState, tracker, metadata, card);
        return Task.CompletedTask;
    }

    private static bool TryRefreshNodeMetadata(
        CombatState combatState,
        AscensionCombatTracker tracker,
        out AscensionNodeMetadata metadata)
    {
        var current = tracker.NodeMetadata ?? AscensionMapService.TryGetCurrentMetadata(combatState.RunState);
        if (current == null)
        {
            metadata = null!;
            return false;
        }

        tracker.NodeMetadata = current;
        metadata = current;
        return true;
    }

    private static bool TryRefreshActiveBossSealMetadata(
        CombatState combatState,
        AscensionCombatTracker tracker,
        out AscensionNodeMetadata metadata) =>
        TryRefreshNodeMetadata(combatState, tracker, out metadata) &&
        HasActiveBossSeal(combatState, metadata);

    private static async Task ApplyFiremarkCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        FiremarkKind firemark)
    {
        var host = FindFiremarkHost(combatState);
        if (host == null)
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A12 gate active: no living enemy was available for Firemark Host selection.");
            return;
        }

        tracker.FiremarkHost = host;
        var actIndex = Math.Clamp(combatState.RunState.CurrentActIndex, 0, 2);
        switch (firemark)
        {
            case FiremarkKind.Might:
                var strength = 2m + actIndex;
                await PowerCmd.Apply<MightMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, strength, host, null);
                await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), host, strength, host, null);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A12 applied: Might firemark host {host.Name} gained {strength} Strength.");
                break;
            case FiremarkKind.Giant:
                await PowerCmd.Apply<GiantMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, 30m, host, null);
                var giantMaxHp = Math.Ceiling(host.MaxHp * 1.3m);
                await CreatureCmd.SetMaxAndCurrentHp(host, giantMaxHp);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A12 applied: Giant firemark host {host.Name} max HP increased to {host.MaxHp}.");
                break;
            case FiremarkKind.ForgeArmor:
                var block = 8m + (5m * actIndex);
                await PowerCmd.Apply<ForgeArmorMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, block, host, null);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A12 applied: Forge Armor firemark host {host.Name} will gain {block} Block at end of turn.");
                break;
            case FiremarkKind.ConstantHeal:
                var heal = 6m + (4m * actIndex);
                await PowerCmd.Apply<ConstantHealMarkFiremarkPower>(new BlockingPlayerChoiceContext(), host, heal, host, null);
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Ascension A12 applied: Constant Heal firemark host {host.Name} will heal {heal} HP at end of turn.");
                break;
        }
    }

    private static Creature? FindFiremarkHost(CombatState combatState)
    {
        return AliveEnemies(combatState)
            .OrderByDescending(enemy => enemy.MaxHp)
            .ThenBy(enemy => combatState.Enemies.IndexOf(enemy))
            .FirstOrDefault();
    }

    private static async Task ApplyBannerCombatStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        switch (banner)
        {
            case BannerKind.Vanguard:
                foreach (var enemy in AliveEnemies(combatState))
                {
                    await PowerCmd.Apply<VanguardBannerPower>(new BlockingPlayerChoiceContext(), enemy, VanguardStrength, enemy, null);
                }

                MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Vanguard banner granted enemies temporary Strength.");
                break;
            case BannerKind.ShieldFormation:
                tracker.ShieldFormationBearer = PickBannerTarget(combatState);
                if (tracker.ShieldFormationBearer != null)
                {
                    await PowerCmd.Apply<ShieldFormationBannerbearerPower>(new BlockingPlayerChoiceContext(), tracker.ShieldFormationBearer, 1m, tracker.ShieldFormationBearer, null);
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension A16 applied: Shield Formation bannerbearer set to {tracker.ShieldFormationBearer.Name}.");
                }
                else
                {
                    MainFile.Logger.Info("[EZMicroBalance] Ascension A16 gate active: Shield Formation had no living enemy target.");
                }

                break;
            case BannerKind.Bounty:
                tracker.BountyTarget = PickBannerTarget(combatState);
                if (tracker.BountyTarget != null)
                {
                    await PowerCmd.Apply<BountyBannerTargetPower>(new BlockingPlayerChoiceContext(), tracker.BountyTarget, 1m, tracker.BountyTarget, null);
                    MainFile.Logger.Info(
                        $"[EZMicroBalance] Ascension A16 applied: Bounty banner target set to {tracker.BountyTarget.Name}.");
                }
                else
                {
                    MainFile.Logger.Info("[EZMicroBalance] Ascension A16 gate active: Bounty had no living enemy target.");
                }

                break;
        }
    }

    private static async Task ApplyBannerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        switch (banner)
        {
            case BannerKind.Vanguard:
                if (combatState.RoundNumber >= VanguardRemovalRound && !tracker.VanguardStrengthRemoved)
                {
                    tracker.VanguardStrengthRemoved = true;
                    await RemoveVanguardStrength(combatState);
                    MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Vanguard banner temporary Strength expired.");
                }

                break;
            case BannerKind.ShieldFormation:
                await ApplyShieldFormationTurnBlock(combatState, tracker);
                break;
            case BannerKind.Bounty:
                await ApplyBountyPenaltyIfExpired(combatState, tracker);
                break;
        }
    }

    private static async Task AfterBannerEnemyHpChanged(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner,
        Creature creature)
    {
        switch (banner)
        {
            case BannerKind.ShieldFormation:
                if (creature == tracker.ShieldFormationBearer &&
                    creature.IsDead &&
                    !tracker.ShieldFormationDeathBlockApplied)
                {
                    tracker.ShieldFormationDeathBlockApplied = true;
                    await ApplyBlockToEnemies(
                        AliveEnemies(combatState).Where(enemy => enemy != creature),
                        ShieldFormationDeathBlock);
                    MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Shield Formation bannerbearer death granted final Block.");
                }

                break;
            case BannerKind.Bounty:
                if (creature == tracker.BountyTarget &&
                    creature.IsDead &&
                    !tracker.BountyExpired &&
                    combatState.RoundNumber <= BountyDeadlineRound)
                {
                    tracker.BountyKilledEarly = true;
                    MainFile.Logger.Info("[EZMicroBalance] Ascension A16 tracked: Bounty target killed before the deadline.");
                }

                break;
        }
    }

    private static async Task RemoveVanguardStrength(CombatState combatState)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            var power = enemy.GetPower<VanguardBannerPower>();
            if (power == null)
            {
                continue;
            }

            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), enemy, -power.Amount, enemy, null, silent: true);
            await PowerCmd.Remove(power);
        }
    }

    private static async Task ApplyShieldFormationTurnBlock(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        var bearer = tracker.ShieldFormationBearer;
        if (bearer == null ||
            !bearer.IsAlive ||
            tracker.ShieldFormationLastBlockRound == combatState.RoundNumber)
        {
            return;
        }

        tracker.ShieldFormationLastBlockRound = combatState.RoundNumber;
        await ApplyBlockToEnemies(
            AliveEnemies(combatState).Where(enemy => enemy != bearer),
            ShieldFormationTurnBlock);
    }

    private static async Task ApplyBountyPenaltyIfExpired(
        CombatState combatState,
        AscensionCombatTracker tracker)
    {
        if (tracker.BountyExpired ||
            tracker.BountyKilledEarly ||
            combatState.RoundNumber <= BountyDeadlineRound)
        {
            return;
        }

        tracker.BountyExpired = true;
        var target = tracker.BountyTarget;
        if (target == null || !target.IsAlive)
        {
            return;
        }

        await ApplyBlockAndArtifact(target, BountyPenaltyBlock, BountyPenaltyArtifact);
        await PowerCmd.Remove(target.GetPower<BountyBannerTargetPower>());
        MainFile.Logger.Info("[EZMicroBalance] Ascension A16 applied: Bounty target survived the deadline and gained protection.");
    }

    private static void TryAddBountyReward(
        CombatState combatState,
        AscensionCombatTracker tracker,
        BannerKind banner)
    {
        if (banner != BannerKind.Bounty ||
            tracker.BountyRewardAdded ||
            tracker.BountyExpired)
        {
            return;
        }

        if (!tracker.BountyKilledEarly &&
            tracker.BountyTarget is { IsDead: true } &&
            combatState.RoundNumber <= BountyDeadlineRound)
        {
            tracker.BountyKilledEarly = true;
        }

        if (!tracker.BountyKilledEarly ||
            combatState.RunState.CurrentRoom is not CombatRoom room)
        {
            return;
        }

        tracker.BountyRewardAdded = true;
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            room.AddExtraReward(player, new GoldReward(BountyGoldReward, player));
        }

        MainFile.Logger.Info($"[EZMicroBalance] Ascension A16 applied: Bounty reward added {BountyGoldReward} Gold.");
    }

    private static Creature? PickBannerTarget(CombatState combatState)
    {
        var candidates = AliveEnemies(combatState)
            .Where(enemy => !enemy.HasPower<MinionPower>())
            .ToList();

        if (candidates.Count == 0)
        {
            candidates = AliveEnemies(combatState).ToList();
        }

        return combatState.RunState.Rng.Niche.NextItem(candidates);
    }

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

    private static async Task ApplyBossSealPlayerTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        tracker.MisalignedShellBlockedTargetsThisTurn.Clear();

        if (metadata.BossSeal?.Id == BossSealId.HolyDaze)
        {
            await TryApplyHolyDaze(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.InkReturn)
        {
            TrackInkReturnIfSlipperySpent(combatState, tracker);
        }

        if (metadata.BossSeal?.Id == BossSealId.StartledShell)
        {
            await TryApplyStartledShellFromWake(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.SoulTide)
        {
            await TrackSoulTideIntangible(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.BoilingCritical)
        {
            await TrackBoilingCriticalSteam(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.StruggleBait)
        {
            await TrackStruggleBaitObservations(combatState, tracker, metadata);
        }

        if (metadata.BossSeal?.Id == BossSealId.ResidualSample)
        {
            await TryApplyResidualSamples(combatState, tracker, metadata);
        }
    }

    private static async Task ApplyBossSealSideTurnStart(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CombatSide side)
    {
        if (side != CombatSide.Enemy || metadata.BossSeal == null)
        {
            return;
        }

        switch (metadata.BossSeal.Id)
        {
            case BossSealId.InkReturn:
                TrackInkReturnIfSlipperySpent(combatState, tracker);
                await ApplyInkReturnIfPending(combatState, tracker, metadata);
                break;
            case BossSealId.StartledShell:
                TrackStartledShellEnemyMove(combatState, tracker);
                break;
            case BossSealId.SoulTide:
                await ApplySoulTidePendingBlock(combatState, tracker);
                break;
            case BossSealId.BoilingCritical:
                await TrackBoilingCriticalSteam(combatState, tracker, metadata);
                await ApplyBoilingExplosionBlock(combatState, tracker, metadata);
                break;
            case BossSealId.MarginalNote:
                TrackKnowledgeDemonEnemyMove(combatState, tracker);
                break;
            case BossSealId.ResidualSample:
                await TryApplyResidualSamples(combatState, tracker, metadata);
                break;
        }
    }

    private static async Task ApplyBossSealTurnEnd(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CombatSide side)
    {
        if (metadata.BossSeal == null)
        {
            return;
        }

        if (side == CombatSide.Player)
        {
            await EndHolyDaze(combatState, tracker);
            await SettleSoulTideBeckons(combatState, tracker, metadata);
            await SettleMisalignedShellClawDeaths(combatState, tracker, metadata);
            await SettleMarginalNotes(combatState, metadata);
            await SettleStruggleBaitBrandEscapes(combatState, tracker, metadata);
            await SettleChosenDecree(combatState, tracker, metadata);
        }
        else if (side == CombatSide.Enemy)
        {
            switch (metadata.BossSeal.Id)
            {
                case BossSealId.StartledShell:
                    await TryApplyStartledShellFromWake(combatState, tracker, metadata);
                    await SettleStartledShellSoulSiphon(combatState, tracker, metadata);
                    break;
                case BossSealId.SoulTide:
                    await TrackSoulTideIntangible(combatState, tracker, metadata);
                    break;
                case BossSealId.BoilingCritical:
                    await TrackBoilingCriticalSteam(combatState, tracker, metadata);
                    break;
                case BossSealId.StruggleBait:
                    await TrackStruggleBaitObservations(combatState, tracker, metadata);
                    break;
                case BossSealId.MarginalNote:
                    if (tracker.KnowledgeDemonCurseMoveActive)
                    {
                        tracker.KnowledgeDemonCurseMoveActive = false;
                        await AddMarginalNotes(combatState, metadata);
                    }

                    break;
            }
        }
    }

    private static async Task AfterBossSealHpChanged(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature,
        decimal delta)
    {
        if (metadata.BossSeal?.Id == BossSealId.StruggleBait &&
            creature.Monster is TheInsatiable &&
            delta > 0m)
        {
            await AddStruggleBaitEscape(combatState, tracker, metadata);
        }
    }

    private static async Task AfterBossSealDamageReceived(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target,
        DamageResult result,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (result.UnblockedDamage <= 0m && result.TotalDamage <= 0m)
        {
            return;
        }

        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.HolyDaze:
                await TryApplyHolyDaze(combatState, tracker, metadata);
                break;
            case BossSealId.InkReturn:
                TrackInkReturnFromDamage(tracker, target);
                break;
            case BossSealId.StartledShell:
                await TryApplyStartledShellFromDamage(tracker, metadata, target);
                break;
            case BossSealId.MisalignedShell:
                await TryApplyMisalignedBackAttackBlock(tracker, metadata, target, dealer);
                break;
        }
    }

    private static async Task AfterBossSealDeath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.MartyrOath:
                await ApplyMartyrOath(combatState, tracker, metadata, creature);
                break;
            case BossSealId.MisalignedShell:
                TrackMisalignedShellClawDeath(tracker, creature);
                break;
            case BossSealId.ResidualSample:
                await TrackResidualSamplePhase(combatState, tracker, metadata, creature);
                break;
        }
    }

    private static Task AfterBossSealCardPlayed(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardPlay cardPlay)
    {
        switch (metadata.BossSeal?.Id)
        {
            case BossSealId.StruggleBait:
                if (cardPlay.Card is FranticEscape)
                {
                    tracker.FranticEscapesPlayed++;
                    tracker.StruggleBaitBrandEscapeAges.Remove(cardPlay.Card);
                }

                break;
            case BossSealId.ChosenDecree:
                TrackChosenDecreePlayed(tracker, cardPlay.Card);
                break;
        }

        if (cardPlay.Card is MarginalNote)
        {
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 tracked: Marginal Note was played.");
        }

        return Task.CompletedTask;
    }

    private static async Task TryApplyHolyDaze(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.HolyDaze || tracker.HolyDazeTriggered)
        {
            return;
        }

        var beast = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is CeremonialBeast);
        if (beast == null ||
            beast.HasPower<PlowPower>() ||
            beast.Monster?.NextMove.StateId != "STUN_MOVE")
        {
            return;
        }

        tracker.HolyDazeTriggered = true;
        var strengthAfterDaze = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<HolyDazePower>(new BlockingPlayerChoiceContext(), beast, strengthAfterDaze, beast, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Holy Daze capped Ceremonial Beast's first stun damage window.");
    }

    private static async Task EndHolyDaze(CombatState combatState, AscensionCombatTracker tracker)
    {
        var beast = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is CeremonialBeast);
        var daze = beast?.GetPower<HolyDazePower>();
        if (beast == null || daze == null)
        {
            return;
        }

        await PowerCmd.Remove(daze);
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), beast, daze.Amount, beast, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Holy Daze ended and granted Strength.");
    }

    private static async Task ApplyMartyrOath(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        var triggerCap = metadata.IsBossBrand ? 3 : 2;
        if (creature.Monster is not KinFollower || tracker.MartyrOathTriggers >= triggerCap)
        {
            return;
        }

        var priest = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KinPriest);
        if (priest == null)
        {
            return;
        }

        tracker.MartyrOathTriggers++;
        var block = metadata.IsBossBrand ? 14m : 12m;
        await CreatureCmd.GainBlock(priest, block, ValueProp.Move, null, fast: true);
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), priest, 1m, priest, null);
        if (priest.GetHpPercentRemaining() <= 0.5d)
        {
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), priest, 1m, priest, null);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Martyr Oath strengthened Kin Priest after a follower death.");
    }

    private static void TrackInkReturnFromDamage(AscensionCombatTracker tracker, Creature target)
    {
        if (tracker.InkReturnTriggered ||
            target.Monster is not Vantom ||
            target.GetPower<SlipperyPower>() is { Amount: > 0 })
        {
            return;
        }

        tracker.InkReturnTriggered = true;
        tracker.InkReturnPending = true;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 tracked: Ink Return will restore Slippery next enemy turn.");
    }

    private static void TrackInkReturnIfSlipperySpent(CombatState combatState, AscensionCombatTracker tracker)
    {
        var vantom = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom);
        if (vantom != null)
        {
            TrackInkReturnFromDamage(tracker, vantom);
        }
    }

    private static async Task ApplyInkReturnIfPending(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.InkReturnPending)
        {
            return;
        }

        var vantom = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Vantom);
        if (vantom == null)
        {
            return;
        }

        tracker.InkReturnPending = false;
        var slippery = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<SlipperyPower>(new BlockingPlayerChoiceContext(), vantom, slippery, vantom, null);
        if (metadata.IsBossBrand)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), vantom, 1m, vantom, null);
        }

        var resultText = metadata.IsBossBrand
            ? "restored extra Slippery and granted Strength"
            : "restored Slippery";
        MainFile.Logger.Info($"[EZMicroBalance] Ascension A19 applied: Ink Return {resultText}.");
    }

    private static async Task TryApplyStartledShellFromDamage(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target)
    {
        if (tracker.StartledShellApplied ||
            target.Monster is not LagavulinMatriarch ||
            target.HasPower<AsleepPower>())
        {
            return;
        }

        tracker.StartledShellApplied = true;
        var plating = metadata.IsBossBrand ? 6m : 4m;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), target, plating, target, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell added early-wake Plating.");
    }

    private static async Task TryApplyStartledShellFromWake(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var matriarch = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is LagavulinMatriarch);
        if (tracker.StartledShellApplied ||
            matriarch == null ||
            matriarch.HasPower<AsleepPower>())
        {
            return;
        }

        tracker.StartledShellApplied = true;
        var platingAmount = metadata.IsBossBrand ? 10m : 8m;
        await PowerCmd.Apply<PlatingPower>(new BlockingPlayerChoiceContext(), matriarch, platingAmount, matriarch, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell added wake Plating.");
    }

    private static void TrackStartledShellEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        tracker.StartledShellSoulSiphonTurn = AliveEnemies(combatState)
            .Any(enemy => enemy.Monster is LagavulinMatriarch &&
                enemy.Monster.NextMove.StateId == "SOUL_SIPHON_MOVE");
    }

    private static async Task SettleStartledShellSoulSiphon(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (!tracker.StartledShellSoulSiphonTurn || tracker.SoulSiphonShellReduced)
        {
            tracker.StartledShellSoulSiphonTurn = false;
            return;
        }

        tracker.StartledShellSoulSiphonTurn = false;
        var matriarch = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is LagavulinMatriarch);
        var plating = matriarch?.GetPower<PlatingPower>();
        if (plating == null || plating.Amount <= 1)
        {
            return;
        }

        tracker.SoulSiphonShellReduced = true;
        var divisor = metadata.IsBossBrand ? 3m : 2m;
        await PowerCmd.ModifyAmount(
            new BlockingPlayerChoiceContext(),
            plating,
            -Math.Floor(plating.Amount / divisor),
            matriarch,
            null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Startled Shell reduced Plating after Soul Siphon.");
    }

    private static async Task TrackSoulTideIntangible(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var soulFysh = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is SoulFysh);
        var intangibleAmount = soulFysh?.GetPower<IntangiblePower>()?.Amount ?? 0m;
        if (soulFysh == null || intangibleAmount <= 0m)
        {
            tracker.LastSoulFyshIntangibleAmount = 0;
            return;
        }

        if (intangibleAmount <= tracker.LastSoulFyshIntangibleAmount)
        {
            tracker.LastSoulFyshIntangibleAmount = (int)intangibleAmount;
            return;
        }

        tracker.LastSoulFyshIntangibleAmount = (int)intangibleAmount;
        var artifact = metadata.IsBossBrand ? 2m : 1m;
        await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), soulFysh, artifact, soulFysh, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Soul Tide added Artifact on Intangible entry.");
    }

    private static Task SettleSoulTideBeckons(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.SoulTide)
        {
            return Task.CompletedTask;
        }

        var beckonsInHand = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Count(card => card is Beckon);

        var cap = metadata.IsBossBrand ? 16m : 12m;
        tracker.PendingSoulTideBlock = Math.Min(cap, beckonsInHand * 2m);
        return Task.CompletedTask;
    }

    private static async Task ApplySoulTidePendingBlock(CombatState combatState, AscensionCombatTracker tracker)
    {
        if (tracker.PendingSoulTideBlock <= 0m)
        {
            return;
        }

        var soulFysh = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is SoulFysh);
        if (soulFysh == null)
        {
            return;
        }

        var block = tracker.PendingSoulTideBlock;
        tracker.PendingSoulTideBlock = 0m;
        await CreatureCmd.GainBlock(soulFysh, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Soul Tide converted Beckon hand pressure into Block.");
    }

    private static async Task TrackBoilingCriticalSteam(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var giant = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is WaterfallGiant);
        var steam = giant?.GetPower<SteamEruptionPower>();
        if (giant == null || steam == null || steam.Amount <= 0m)
        {
            return;
        }

        var steamThreshold = metadata.IsBossBrand ? 10m : 12m;
        var milestone = (int)Math.Floor(steam.Amount / steamThreshold);
        if (milestone <= tracker.LastSteamEruptionMilestone)
        {
            return;
        }

        var gained = milestone - tracker.LastSteamEruptionMilestone;
        tracker.LastSteamEruptionMilestone = milestone;
        await PowerCmd.Apply<BoilingCriticalPower>(new BlockingPlayerChoiceContext(), giant, gained, giant, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Boiling Critical gained a Boiling stack.");
    }

    private static async Task ApplyBoilingExplosionBlock(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (tracker.BoilingExplosionBlockGranted)
        {
            return;
        }

        var giant = AliveEnemies(combatState).FirstOrDefault(enemy =>
            enemy.Monster is WaterfallGiant &&
            enemy.Monster.NextMove.StateId == "EXPLODE_MOVE");
        var boiling = giant?.GetPower<BoilingCriticalPower>();
        if (giant == null || boiling == null || boiling.Amount <= 0)
        {
            return;
        }

        tracker.BoilingExplosionBlockGranted = true;
        var blockPerStack = metadata.IsBossBrand ? 1m : 2m;
        var block = boiling.Amount * blockPerStack;
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            await CreatureCmd.GainBlock(player.Creature, block, ValueProp.Move, null, fast: true);
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Boiling Critical telegraphed the explosion with player Block.");
    }

    private static async Task TryApplyMisalignedBackAttackBlock(
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature target,
        Creature? dealer)
    {
        if (dealer?.Player == null ||
            target.Monster is not Crusher and not Rocket ||
            !target.HasPower<BackAttackLeftPower>() && !target.HasPower<BackAttackRightPower>() ||
            !tracker.MisalignedShellBlockedTargetsThisTurn.Add(target))
        {
            return;
        }

        var block = metadata.IsBossBrand ? 8m : 6m;
        await CreatureCmd.GainBlock(target, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Misaligned Shell blocked the first back attack hit this turn.");
    }

    private static void TrackMisalignedShellClawDeath(AscensionCombatTracker tracker, Creature creature)
    {
        if (creature.Monster is Crusher or Rocket)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Add(creature);
        }
    }

    private static async Task SettleMisalignedShellClawDeaths(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (tracker.MisalignedShellArtifactApplied ||
            tracker.MisalignedShellClawsDiedThisTurn.Count != 1)
        {
            tracker.MisalignedShellClawsDiedThisTurn.Clear();
            return;
        }

        var otherClaw = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Crusher or Rocket);
        if (otherClaw != null)
        {
            tracker.MisalignedShellArtifactApplied = true;
            var artifact = metadata.IsBossBrand ? 2m : 1m;
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), otherClaw, artifact, otherClaw, null);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Misaligned Shell gave Artifact to the surviving claw.");
        }

        tracker.MisalignedShellClawsDiedThisTurn.Clear();
    }

    private static async Task AddMarginalNotes(CombatState combatState, AscensionNodeMetadata metadata)
    {
        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var noteCount = metadata.IsBossBrand ? 2 : 1;
            for (var index = 0; index < noteCount; index++)
            {
                var note = combatState.CreateCard<MarginalNote>(player);
                await CardPileCmd.AddGeneratedCardToCombat(note, PileType.Discard, player, CardPilePosition.Bottom);
            }
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Marginal Note pressure was shuffled into discard after Curse of Knowledge.");
    }

    private static void TrackKnowledgeDemonEnemyMove(CombatState combatState, AscensionCombatTracker tracker)
    {
        tracker.KnowledgeDemonCurseMoveActive = AliveEnemies(combatState)
            .Any(enemy => enemy.Monster is KnowledgeDemon &&
                enemy.Monster.NextMove.StateId == "CURSE_OF_KNOWLEDGE_MOVE");
    }

    private static async Task SettleMarginalNotes(CombatState combatState, AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.MarginalNote)
        {
            return;
        }

        var demon = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is KnowledgeDemon);
        if (demon == null)
        {
            return;
        }

        var notesInHand = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .Where(pile => pile.Type == PileType.Hand)
            .SelectMany(pile => pile.Cards)
            .Count(card => card is MarginalNote);

        if (notesInHand > 0)
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), demon, notesInHand, demon, null);
            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: unplayed Marginal Note granted Knowledge Demon Strength.");
        }
    }

    private static async Task TrackStruggleBaitObservations(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        var strength = insatiable.GetPowerAmount<StrengthPower>();
        var sandpit = insatiable.Powers
            .OfType<SandpitPower>()
            .Sum(power => power.Amount);

        if (!tracker.StruggleBaitBaselineCaptured)
        {
            tracker.StruggleBaitBaselineCaptured = true;
            tracker.LastInsatiableStrengthAmount = strength;
            tracker.LastInsatiableSandpitAmount = sandpit;
            return;
        }

        var shouldAddEscape = !tracker.SuppressStruggleBaitStrengthTrigger &&
            (strength > tracker.LastInsatiableStrengthAmount ||
                sandpit > tracker.LastInsatiableSandpitAmount);

        tracker.LastInsatiableStrengthAmount = strength;
        tracker.LastInsatiableSandpitAmount = sandpit;

        if (shouldAddEscape)
        {
            await AddStruggleBaitEscape(combatState, tracker, metadata);
        }
    }

    private static async Task AddStruggleBaitEscape(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var escape = combatState.CreateCard<FranticEscape>(player);
            await CardPileCmd.AddGeneratedCardToCombat(escape, PileType.Discard, player, CardPilePosition.Bottom);
            if (metadata.IsBossBrand)
            {
                tracker.StruggleBaitBrandEscapeAges[escape] = 0;
            }
        }

        if (tracker.FranticEscapesPlayed >= 3)
        {
            tracker.SuppressStruggleBaitStrengthTrigger = true;
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), insatiable, 1m, insatiable, null);
            tracker.SuppressStruggleBaitStrengthTrigger = false;
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Struggle Bait added Frantic Escape pressure.");
    }

    private static async Task SettleStruggleBaitBrandEscapes(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.StruggleBait ||
            !metadata.IsBossBrand ||
            tracker.StruggleBaitBrandEscapeAges.Count == 0)
        {
            return;
        }

        var combatCards = combatState.Players
            .Where(player => player.IsActiveForHooks)
            .SelectMany(player => player.Piles)
            .SelectMany(pile => pile.Cards)
            .ToHashSet();

        var maturedEscapes = new List<CardModel>();
        foreach (var card in tracker.StruggleBaitBrandEscapeAges.Keys.ToArray())
        {
            if (!combatCards.Contains(card))
            {
                tracker.StruggleBaitBrandEscapeAges.Remove(card);
                continue;
            }

            var age = tracker.StruggleBaitBrandEscapeAges[card] + 1;
            if (age >= 2)
            {
                maturedEscapes.Add(card);
            }
            else
            {
                tracker.StruggleBaitBrandEscapeAges[card] = age;
            }
        }

        if (maturedEscapes.Count == 0)
        {
            return;
        }

        foreach (var card in maturedEscapes)
        {
            tracker.StruggleBaitBrandEscapeAges.Remove(card);
        }

        var insatiable = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TheInsatiable);
        if (insatiable == null)
        {
            return;
        }

        var block = maturedEscapes.Count * 5m;
        await CreatureCmd.GainBlock(insatiable, block, ValueProp.Move, null, fast: true);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A20 applied: Struggle Bait Brand converted unplayed Frantic Escape pressure into Block.");
    }

    private static void TryAssignChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        CardModel card)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree ||
            tracker.ChosenDecreeCard != null ||
            card.Affliction is not Bound ||
            card.Enchantment != null)
        {
            return;
        }

        CardCmd.Enchant<RoyalDecreeEnchantment>(card, 1m);
        tracker.ChosenDecreeCard = card;
        tracker.ChosenDecreePlayed = false;
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Chosen Decree marked one Bound card.");
    }

    private static void TryAssignChosenDecreeInHands(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        foreach (var card in combatState.Players
                     .Where(player => player.IsActiveForHooks)
                     .SelectMany(player => player.Piles)
                     .Where(pile => pile.Type == PileType.Hand)
                     .SelectMany(pile => pile.Cards))
        {
            TryAssignChosenDecree(combatState, tracker, metadata, card);
            if (tracker.ChosenDecreeCard != null)
            {
                return;
            }
        }
    }

    private static void TrackChosenDecreePlayed(AscensionCombatTracker tracker, CardModel card)
    {
        if (tracker.ChosenDecreeCard == card ||
            card.Enchantment is RoyalDecreeEnchantment)
        {
            tracker.ChosenDecreePlayed = true;
        }
    }

    private static async Task SettleChosenDecree(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ChosenDecree)
        {
            return;
        }

        TryAssignChosenDecreeInHands(combatState, tracker, metadata);
        if (tracker.ChosenDecreeCard == null)
        {
            return;
        }

        var queen = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is Queen);
        var amalgam = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TorchHeadAmalgam);
        if (tracker.ChosenDecreePlayed)
        {
            if (amalgam != null)
            {
                await PowerCmd.Apply<ChosenDecreeReductionPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
            }

            if (metadata.IsBossBrand)
            {
                foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
                {
                    await CreatureCmd.GainBlock(player.Creature, 5m, ValueProp.Move, null, fast: true);
                }
            }

            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Chosen Decree was obeyed.");
        }
        else
        {
            if (queen != null)
            {
                await CreatureCmd.GainBlock(queen, metadata.IsBossBrand ? 14m : 10m, ValueProp.Move, null, fast: true);
            }

            if (amalgam != null)
            {
                await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), amalgam, 1m, queen, null);
            }

            MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: missed Chosen Decree strengthened the Queen's side.");
        }

        if (tracker.ChosenDecreeCard.Enchantment is RoyalDecreeEnchantment)
        {
            CardCmd.ClearEnchantment(tracker.ChosenDecreeCard);
        }

        tracker.ChosenDecreeCard = null;
        tracker.ChosenDecreePlayed = false;
    }

    private static async Task TrackResidualSamplePhase(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        if (creature.Monster is not TestSubject ||
            !creature.HasPower<AdaptablePower>())
        {
            return;
        }

        tracker.TestSubjectPhaseChanges++;
        var sampleCount = metadata.IsBossBrand && tracker.TestSubjectPhaseChanges == 1 ? 2m : 1m;
        await PowerCmd.Apply<ResidualSamplePower>(new BlockingPlayerChoiceContext(), creature, sampleCount, creature, null);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Residual Sample retained a weakened sample for the next phase.");
    }

    private static async Task TryApplyResidualSamples(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ResidualSample)
        {
            return;
        }

        var subject = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TestSubject);
        var sample = subject?.GetPower<ResidualSamplePower>();
        if (subject == null || sample == null || subject.IsDead)
        {
            return;
        }

        var amount = sample.Amount;
        await CreatureCmd.GainBlock(subject, 8m * amount, ValueProp.Move, null, fast: true);
        await PowerCmd.Remove(sample);
        MainFile.Logger.Info("[EZMicroBalance] Ascension A19 applied: Residual Sample resolved as weakened phase Block.");
    }

    private static async Task ApplyA20CourtyardRecovery(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.IsBossBrand ||
            !AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(combatState.RunState) ||
            combatState.RunState.Map.SecondBossMapPoint == null ||
            combatState.RunState.CurrentMapCoord != combatState.RunState.Map.BossMapPoint.coord)
        {
            return;
        }

        foreach (var player in combatState.Players.Where(player => player.IsActiveForHooks))
        {
            var missingHp = Math.Max(0m, player.Creature.MaxHp - player.Creature.CurrentHp);
            var heal = Math.Ceiling(missingHp * 0.25m);
            if (heal > 0m)
            {
                await CreatureCmd.Heal(player.Creature, heal);
            }
        }

        MainFile.Logger.Info("[EZMicroBalance] Ascension A20 applied: courtyard recovery restored 25% of missing HP and Boss 2 Brand remains armed on the map.");
    }

    private static bool HasActiveFiremark(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Firemark.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Elite &&
            AscensionFeatureGate.IsFiremarkedEliteEnabled(combatState.RunState);
    }

    private static bool HasActiveBanner(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Banner.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            AscensionFeatureGate.IsBannerRoomEnabled(combatState.RunState);
    }

    private static bool HasActiveBossSeal(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.BossSeal != null &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Boss &&
            (metadata.IsBossBrand
                ? AscensionFeatureGate.IsDualKingBrandsSinglePlayerEnabled(combatState.RunState)
                : AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState));
    }

    private static async Task ApplyBlockAndArtifactToEnemies(CombatState combatState, decimal block, decimal artifact)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            await ApplyBlockAndArtifact(enemy, block, artifact);
        }
    }

    private static async Task ApplyBlockAndArtifact(Creature creature, decimal block, decimal artifact)
    {
        if (block > 0m)
        {
            await CreatureCmd.GainBlock(creature, block, ValueProp.Move, null, fast: true);
        }

        if (artifact > 0m)
        {
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), creature, artifact, creature, null);
        }
    }

    private static async Task ApplyBlockToEnemies(CombatState combatState, decimal block)
    {
        await ApplyBlockToEnemies(AliveEnemies(combatState), block);
    }

    private static async Task ApplyBlockToEnemies(IEnumerable<Creature> enemies, decimal block)
    {
        foreach (var enemy in enemies)
        {
            await CreatureCmd.GainBlock(enemy, block, ValueProp.Move, null, fast: true);
        }
    }

    private static async Task ApplyStrengthToEnemies(CombatState combatState, decimal amount)
    {
        foreach (var enemy in AliveEnemies(combatState))
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), enemy, amount, enemy, null);
        }
    }

    private static IEnumerable<Creature> AliveEnemies(CombatState combatState)
    {
        return combatState.Enemies.Where(enemy => enemy.IsAlive);
    }
}
