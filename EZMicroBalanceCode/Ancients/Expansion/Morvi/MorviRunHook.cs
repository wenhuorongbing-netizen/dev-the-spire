using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed class MorviRunHook : AbstractModel
{
    public MorviRunHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => true;

    public override Task BeforeCombatStart()
    {
        MorviBlessingService.BeforeCombatStart();
        return Task.CompletedTask;
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        return MorviBlessingService.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions);
    }

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        return MorviBlessingService.TryModifyCardRewardAlternatives(player, cardReward, alternatives);
    }

    public override Task AfterRewardTaken(Player player, Reward reward)
    {
        return MorviBlessingService.AfterRewardTaken(player, reward);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return MorviBlessingService.AfterCardPlayed(choiceContext, cardPlay);
    }
}

internal static class MorviBlessingService
{
    private const char ProgressSeparator = ';';
    private const int DebtSettlementStartingGold = 75;
    private const int DebtSettlementRequiredPayments = 3;
    private const int DebtSettlementGoldPayment = 25;
    private const int DebtSettlementHpFallback = 3;

    private sealed class CardRewardContext
    {
        public bool IsNormalActTwoCombatCardReward { get; set; }

        public bool DebtSettlementHandled { get; set; }
    }

    private sealed class CombatState
    {
        public bool MisprintUsedThisCombat { get; set; }

        public bool IsResolvingMisprint { get; set; }
    }

    private sealed record Progress(
        int DebtRemaining,
        int DebtPaid,
        bool DebtRewardPending)
    {
        public static Progress Default => new(0, 0, false);
    }

    private static readonly ConditionalWeakTable<CardReward, CardRewardContext> CardRewardContexts = new();
    private static readonly ConditionalWeakTable<Player, CombatState> CombatStates = new();

    public static async Task SetSelectedBlessing(Player player, string blessingId)
    {
        var progress = blessingId == MorviBlessingIds.DebtSettlement
            ? new Progress(DebtSettlementRequiredPayments, 0, false)
            : Progress.Default;

        SetState(player, blessingId, progress);
        if (blessingId == MorviBlessingIds.DebtSettlement)
        {
            await PlayerCmd.GainGold(DebtSettlementStartingGold, player);
        }
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientSavedStateFields.MorviStateKey[player] ?? string.Empty;
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static void BeforeCombatStart()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players)
        {
            var combatState = CombatStates.GetOrCreateValue(player);
            combatState.MisprintUsedThisCombat = false;
            combatState.IsResolvingMisprint = false;
        }
    }

    public static bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        if (!IsNormalActTwoCombatReward(player, creationOptions))
        {
            return false;
        }

        if (PrismaticGemRewardScreenContextPatch.CurrentReward is { } currentReward &&
            currentReward.Player == player)
        {
            CardRewardContexts.GetValue(currentReward, _ => new CardRewardContext()).IsNormalActTwoCombatCardReward = true;
        }

        if (GetSelectedBlessing(player) != MorviBlessingIds.OpenBookExam)
        {
            return false;
        }

        var target = cardRewardOptions
            .Select(option => option.Card)
            .FirstOrDefault(card => card.Type is CardType.Attack or CardType.Skill && card.IsUpgradable);
        if (target == null)
        {
            return false;
        }

        CardCmd.Upgrade(target, CardPreviewStyle.None);
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Open-Book Exam upgraded reward option {target.Id.Entry}.");
        return true;
    }

    public static bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (!IsTrackedNormalActTwoCombatReward(cardReward) || alternatives.Count >= 2)
        {
            return false;
        }

        if (GetSelectedBlessing(player) != MorviBlessingIds.DebtSettlement)
        {
            return false;
        }

        var progress = GetProgress(player);
        if (progress.DebtRemaining <= 0 ||
            progress.DebtRewardPending ||
            !CanPayDebtSettlement(player))
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_MORVI_DEBT_SETTLEMENT",
            () => ChooseDebtSettlement(player, cardReward),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    public static async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (reward is not CardReward ||
            GetSelectedBlessing(player) != MorviBlessingIds.DebtSettlement)
        {
            return;
        }

        var progress = GetProgress(player);
        if (!progress.DebtRewardPending)
        {
            return;
        }

        var resolved = await ResolveDebtSettlementCompletion(player);
        if (!resolved)
        {
            return;
        }

        progress = GetProgress(player) with { DebtRewardPending = false };
        SetProgress(player, progress);
    }

    public static async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (GetSelectedBlessing(player) != MorviBlessingIds.MisprintPress)
        {
            return;
        }

        var combatState = CombatStates.GetOrCreateValue(player);
        if (combatState.MisprintUsedThisCombat ||
            combatState.IsResolvingMisprint ||
            cardPlay.Card.IsClone ||
            cardPlay.Card.Type is not (CardType.Attack or CardType.Skill))
        {
            return;
        }

        var copy = cardPlay.Card.CreateClone();
        AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Exhaust);
        var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Play, player);
        if (addResult is not { success: true })
        {
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Misprint Press skipped: generated copy could not enter combat.");
            return;
        }

        combatState.MisprintUsedThisCombat = true;
        combatState.IsResolvingMisprint = true;
        try
        {
            await CardCmd.AutoPlay(choiceContext, copy, null, AutoPlayType.Default, skipXCapture: true);
        }
        finally
        {
            combatState.IsResolvingMisprint = false;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Morvi Misprint Press replayed {cardPlay.Card.Id.Entry} once.");
    }

    private static async Task ChooseDebtSettlement(Player player, CardReward reward)
    {
        if (!IsTrackedNormalActTwoCombatReward(reward) ||
            GetSelectedBlessing(player) != MorviBlessingIds.DebtSettlement)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.DebtSettlementHandled)
        {
            return;
        }

        context.DebtSettlementHandled = true;
        var progress = GetProgress(player);
        if (progress.DebtRemaining <= 0 || progress.DebtRewardPending || !CanPayDebtSettlement(player))
        {
            return;
        }

        await PayDebtSettlement(player);
        var nextRemaining = Math.Max(0, progress.DebtRemaining - 1);
        progress = progress with
        {
            DebtRemaining = nextRemaining,
            DebtPaid = progress.DebtPaid + 1,
            DebtRewardPending = nextRemaining == 0
        };

        SetProgress(player, progress);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Morvi Debt Settlement paid installment {progress.DebtPaid}/{DebtSettlementRequiredPayments}; remaining={progress.DebtRemaining}.");
    }

    private static async Task PayDebtSettlement(Player player)
    {
        var goldPayment = Math.Min(player.Gold, DebtSettlementGoldPayment);
        if (goldPayment > 0)
        {
            await PlayerCmd.LoseGold(goldPayment, player, GoldLossType.Spent);
        }

        if (goldPayment >= DebtSettlementGoldPayment)
        {
            return;
        }

        var hpLoss = Math.Min(DebtSettlementHpFallback, Math.Max(0, player.Creature.CurrentHp - 1));
        if (hpLoss > 0)
        {
            await CreatureCmd.SetCurrentHp(player.Creature, player.Creature.CurrentHp - hpLoss);
        }
    }

    private static async Task<bool> ResolveDebtSettlementCompletion(Player player)
    {
        var rewardCard = CreateRandomRewardCard(player);
        if (rewardCard == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Morvi Debt Settlement deferred payoff: no valid reward card could be generated.");
            return false;
        }

        if (rewardCard.IsUpgradable)
        {
            CardCmd.Upgrade(rewardCard);
        }

        await new RewardsSet(player)
            .WithCustomRewards([new SpecialCardReward(rewardCard, player)])
            .WithSkippingDisallowed()
            .Offer();
        MainFile.Logger.Info($"[EZMicroBalance] Morvi Debt Settlement completed: offered upgraded {rewardCard.Id.Entry}.");
        return true;
    }

    private static bool IsTrackedNormalActTwoCombatReward(CardReward reward) =>
        CardRewardContexts.TryGetValue(reward, out var context) &&
        context.IsNormalActTwoCombatCardReward;

    private static bool CanPayDebtSettlement(Player player) =>
        player.Gold > 0 || player.Creature.CurrentHp > 1;

    private static bool IsNormalActTwoCombatReward(Player player, CardCreationOptions creationOptions)
    {
        return player.RunState.CurrentActIndex == 1 &&
            player.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter &&
            creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward);
    }

    private static CardModel? CreateRandomRewardCard(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, 1, options).FirstOrDefault()?.Card;
    }

    private static Progress GetProgress(Player player)
    {
        var state = AncientSavedStateFields.MorviStateKey[player] ?? string.Empty;
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 4)
        {
            return Progress.Default;
        }

        return new Progress(
            ParseInt(parts[1]),
            ParseInt(parts[2]),
            ParseBool(parts[3]));
    }

    private static void SetProgress(Player player, Progress progress)
    {
        var selectedBlessing = GetSelectedBlessing(player);
        if (!string.IsNullOrWhiteSpace(selectedBlessing))
        {
            SetState(player, selectedBlessing, progress);
        }
    }

    private static void SetState(Player player, string blessingId, Progress progress)
    {
        AncientSavedStateFields.MorviStateKey[player] = string.Join(
            ProgressSeparator,
            blessingId,
            progress.DebtRemaining,
            progress.DebtPaid,
            progress.DebtRewardPending ? 1 : 0);
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
