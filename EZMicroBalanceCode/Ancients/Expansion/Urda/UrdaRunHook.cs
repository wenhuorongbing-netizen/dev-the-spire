using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRunHook : AbstractModel
{
    public UrdaRunHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => false;

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        return UrdaBlessingService.MarkCardRewardIfNormalActOneCombat(player, creationOptions);
    }

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        return UrdaBlessingService.TryModifyCardRewardAlternatives(player, cardReward, alternatives);
    }

    public override Task AfterRewardTaken(Player player, Reward reward)
    {
        return UrdaBlessingService.AfterRewardTaken(player, reward);
    }

    public override Task AfterActEntered()
    {
        return UrdaBlessingService.AfterActEntered();
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.AfterRoomEntered(room);
    }
}

internal static class UrdaBlessingService
{
    private const char ProgressSeparator = ';';
    private const int MaxSeedbedChecks = 4;
    private const int SeedbedMaxHpCost = 2;
    private const int SeedbedCompletionMaxHpGain = 10;
    private const int HumusGoldPerSkip = 15;
    private const int HumusRequiredSkips = 3;
    private const int MossMapMonsterGold = 25;
    private const int MossMapEventHeal = 5;
    private const int MossMapRestMaxHp = 3;

    private sealed class CardRewardContext
    {
        public bool IsNormalActOneCombatCardReward { get; set; }

        public bool HumusPactHandled { get; set; }
    }

    private sealed record Progress(
        int SeedbedChecks,
        int SeedbedAccepted,
        bool SeedbedTransformed,
        int HumusSkips,
        bool HumusCompleted,
        bool HumusCompletionPending,
        bool MoltingActive,
        int MossRoomMask)
    {
        public static Progress Default => new(0, 0, false, 0, false, false, false, 0);
    }

    private static readonly ConditionalWeakTable<CardReward, CardRewardContext> CardRewardContexts = new();

    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        SetState(player, blessingId, Progress.Default);
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientSavedStateFields.UrdaStateKey[player] ?? string.Empty;
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static bool MarkCardRewardIfNormalActOneCombat(
        Player player,
        CardCreationOptions creationOptions)
    {
        if (PrismaticGemRewardScreenContextPatch.CurrentReward is not { } currentReward ||
            currentReward.Player != player ||
            !IsNormalActOneCombatReward(player, creationOptions))
        {
            return false;
        }

        CardRewardContexts.GetValue(currentReward, _ => new CardRewardContext()).IsNormalActOneCombatCardReward = true;
        return false;
    }

    public static bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (!IsTrackedNormalActOneCombatReward(cardReward) || alternatives.Count >= 2)
        {
            return false;
        }

        return GetSelectedBlessing(player) switch
        {
            UrdaBlessingIds.Seedbed => TryAddSeedbedAlternative(player, alternatives),
            UrdaBlessingIds.HumusPact => TryAddHumusPactAlternative(player, cardReward, alternatives),
            _ => false
        };
    }

    public static async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (reward is not CardReward ||
            GetSelectedBlessing(player) != UrdaBlessingIds.HumusPact)
        {
            return;
        }

        var progress = GetProgress(player);
        if (!progress.HumusCompletionPending)
        {
            return;
        }

        var resolved = await ResolveHumusCompletion(player);
        if (!resolved)
        {
            return;
        }

        progress = GetProgress(player) with { HumusCompletionPending = false };
        SetProgress(player, progress);
    }

    private static bool TryAddSeedbedAlternative(
        Player player,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.SeedbedTransformed ||
            progress.SeedbedAccepted >= MaxSeedbedChecks ||
            !CanPaySeedbedCost(player))
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_SEEDBED",
            () => AcceptSeedbed(player),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private static bool TryAddHumusPactAlternative(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.HumusCompleted || progress.HumusCompletionPending)
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_HUMUS_PACT",
            () => ChooseHumusPact(player, cardReward),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private static async Task ChooseHumusPact(Player player, CardReward reward)
    {
        if (!IsTrackedNormalActOneCombatReward(reward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.HumusPact)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.HumusPactHandled)
        {
            return;
        }

        context.HumusPactHandled = true;
        var progress = GetProgress(player);
        if (progress.HumusCompleted || progress.HumusCompletionPending)
        {
            return;
        }

        progress = progress with { HumusSkips = progress.HumusSkips + 1 };
        if (progress.HumusSkips >= HumusRequiredSkips)
        {
            progress = progress with { HumusCompleted = true, HumusCompletionPending = true };
        }

        SetProgress(player, progress);
        await PlayerCmd.GainGold(HumusGoldPerSkip, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Humus Pact applied: composted normal combat card reward {progress.HumusSkips}/{HumusRequiredSkips}; gained {HumusGoldPerSkip} gold.");
    }

    public static async Task ApplyMolting(Player player)
    {
        var progress = GetProgress(player);
        progress = progress with { MoltingActive = true };
        SetProgress(player, progress);

        var removedCards = new List<CardModel>();
        var strike = FindStarterCard(player, "Strike");
        if (strike != null)
        {
            removedCards.Add(strike);
        }

        var defend = FindStarterCard(player, "Defend");
        if (defend != null)
        {
            removedCards.Add(defend);
        }

        foreach (var card in removedCards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        var husks = new[]
        {
            player.RunState.CreateCard<WitheredHusk>(player),
            player.RunState.CreateCard<WitheredHusk>(player)
        };

        var addResults = new List<CardPileAddResult>();
        foreach (var husk in husks)
        {
            addResults.Add(await CardPileCmd.Add(husk, PileType.Deck));
        }

        CardCmd.PreviewCardPileAdd(addResults, 2f);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Molting applied: removed {removedCards.Count} starter card(s) and added 2 Withered Husk cards.");
    }

    public static async Task AfterActEntered()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null || runState.CurrentActIndex < 1)
        {
            return;
        }

        foreach (var player in runState.Players)
        {
            var progress = GetProgress(player);
            if (!progress.MoltingActive)
            {
                continue;
            }

            var husks = PileType.Deck.GetPile(player).Cards.OfType<WitheredHusk>().Cast<CardModel>().ToList();
            foreach (var husk in husks)
            {
                await CardPileCmd.RemoveFromDeck(husk, showPreview: false);
            }

            if (husks.Count > 0)
            {
                MainFile.Logger.Info(
                    $"[EZMicroBalance] Urda Molting applied: removed {husks.Count} Withered Husk card(s) at Act {runState.CurrentActIndex + 1} start.");
            }
        }
    }

    public static async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is not (RoomType.Monster or RoomType.Event or RoomType.Shop or RoomType.Elite or RoomType.RestSite))
        {
            return;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null || runState.CurrentActIndex != 0)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.MossMap))
        {
            var progress = GetProgress(player);
            var roomMask = GetRoomMask(room.RoomType);
            if ((progress.MossRoomMask & roomMask) != 0)
            {
                continue;
            }

            progress = progress with { MossRoomMask = progress.MossRoomMask | roomMask };
            SetProgress(player, progress);
            await ApplyMossMapRoomReward(player, room.RoomType);
        }
    }

    private static async Task AcceptSeedbed(Player player)
    {
        var progress = GetProgress(player);
        if (progress.SeedbedTransformed ||
            progress.SeedbedAccepted >= MaxSeedbedChecks ||
            !CanPaySeedbedCost(player))
        {
            return;
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), player.Creature, SeedbedMaxHpCost, isFromCard: false);
        var seedling = player.RunState.CreateCard<UrdaSeedling>(player);
        if (progress.SeedbedAccepted == 0 && seedling.IsUpgradable)
        {
            CardCmd.Upgrade(seedling);
        }

        var addResult = await CardPileCmd.Add(seedling, PileType.Deck);
        if (addResult.success)
        {
            CardCmd.PreviewCardPileAdd(addResult, 2f);
        }
        else
        {
            AncientCardHelpers.RemoveUnpiledRunCard(seedling);
        }

        progress = progress with
        {
            SeedbedChecks = progress.SeedbedChecks + 1,
            SeedbedAccepted = progress.SeedbedAccepted + 1
        };
        if (progress.SeedbedAccepted >= MaxSeedbedChecks)
        {
            progress = progress with { SeedbedTransformed = true };
            await CreatureCmd.SetMaxHp(player.Creature, player.Creature.MaxHp + SeedbedCompletionMaxHpGain);
        }

        SetProgress(player, progress);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seedbed applied: accepted {progress.SeedbedAccepted}/{MaxSeedbedChecks}; transformed={progress.SeedbedTransformed}.");
    }

    private static async Task<bool> ResolveHumusCompletion(Player player)
    {
        var rewardCard = CreateRandomRewardCard(player);
        if (rewardCard == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Humus Pact deferred upgraded card reward: no valid reward card could be generated.");
            return false;
        }

        var removalPrefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, 2)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };

        var selectedRemovals = (await CardSelectCmd.FromDeckForRemoval(player, removalPrefs)).ToList();
        foreach (var card in selectedRemovals)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }

        if (rewardCard.IsUpgradable)
        {
            CardCmd.Upgrade(rewardCard);
        }

        await new RewardsSet(player)
            .WithCustomRewards([new SpecialCardReward(rewardCard, player)])
            .WithSkippingDisallowed()
            .Offer();
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Humus Pact completed: removed {selectedRemovals.Count} card(s) and offered upgraded {rewardCard.Id.Entry}.");
        return true;
    }

    private static async Task ApplyMossMapRoomReward(Player player, RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Monster:
                await PlayerCmd.GainGold(MossMapMonsterGold, player);
                break;
            case RoomType.Event:
                await CreatureCmd.Heal(player.Creature, MossMapEventHeal);
                break;
            case RoomType.Shop:
                await TryGivePotion(player);
                break;
            case RoomType.Elite:
                UpgradeRandomCard(player);
                break;
            case RoomType.RestSite:
                await CreatureCmd.GainMaxHp(player.Creature, MossMapRestMaxHp);
                break;
        }

        MainFile.Logger.Info($"[EZMicroBalance] Urda Moss Map applied: first Act 1 {roomType} room reward granted.");
    }

    private static async Task TryGivePotion(Player player)
    {
        if (!player.HasOpenPotionSlots)
        {
            MainFile.Logger.Info("[EZMicroBalance] Urda Moss Map skipped shop potion: no open potion slot.");
            return;
        }

        var potion = PotionFactory.CreateRandomPotionOutOfCombat(player, player.PlayerRng.Rewards).ToMutable();
        await PotionCmd.TryToProcure(potion, player);
    }

    private static void UpgradeRandomCard(Player player)
    {
        var target = PileType.Deck.GetPile(player).Cards
            .Where(card => card.IsUpgradable)
            .ToList()
            .StableShuffle(player.PlayerRng.Rewards)
            .FirstOrDefault();
        if (target == null)
        {
            MainFile.Logger.Info("[EZMicroBalance] Urda Moss Map skipped elite upgrade: no upgradable card.");
            return;
        }

        CardCmd.Upgrade(target, CardPreviewStyle.HorizontalLayout);
    }

    private static CardModel? CreateRandomRewardCard(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, 1, options).FirstOrDefault()?.Card;
    }

    private static CardModel? FindStarterCard(Player player, string prefix)
    {
        return PileType.Deck.GetPile(player).Cards.FirstOrDefault(card =>
            card.IsRemovable &&
            (card.GetType().Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
             card.Id.Entry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
             card.Id.Entry.Contains($"_{prefix}", StringComparison.OrdinalIgnoreCase)));
    }

    private static bool IsTrackedNormalActOneCombatReward(CardReward reward) =>
        CardRewardContexts.TryGetValue(reward, out var context) &&
        context.IsNormalActOneCombatCardReward;

    private static bool CanPaySeedbedCost(Player player) =>
        player.Creature.MaxHp > SeedbedMaxHpCost;

    private static bool IsNormalActOneCombatReward(Player player, CardCreationOptions creationOptions)
    {
        return player.RunState.CurrentActIndex == 0 &&
            player.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter &&
            creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward);
    }

    private static int GetRoomMask(RoomType roomType) => 1 << (int)roomType;

    private static Progress GetProgress(Player player)
    {
        var state = AncientSavedStateFields.UrdaStateKey[player] ?? string.Empty;
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 8)
        {
            return Progress.Default;
        }

        var hasHumusPendingField = parts.Length >= 9;
        return new Progress(
            ParseInt(parts[1]),
            ParseInt(parts[2]),
            ParseBool(parts[3]),
            ParseInt(parts[4]),
            ParseBool(parts[5]),
            hasHumusPendingField && ParseBool(parts[6]),
            ParseBool(parts[hasHumusPendingField ? 7 : 6]),
            ParseInt(parts[hasHumusPendingField ? 8 : 7]));
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
        AncientSavedStateFields.UrdaStateKey[player] = string.Join(
            ProgressSeparator,
            blessingId,
            progress.SeedbedChecks,
            progress.SeedbedAccepted,
            progress.SeedbedTransformed ? 1 : 0,
            progress.HumusSkips,
            progress.HumusCompleted ? 1 : 0,
            progress.HumusCompletionPending ? 1 : 0,
            progress.MoltingActive ? 1 : 0,
            progress.MossRoomMask);
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
