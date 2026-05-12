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

    public override Task AfterActEntered()
    {
        return UrdaBlessingService.AfterActEntered();
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.AfterRoomEntered(room);
    }
}

[HarmonyPatch(typeof(CardReward), nameof(CardReward.OnSkipped))]
internal static class UrdaCardRewardSkippedPatch
{
    private static void Postfix(CardReward __instance)
    {
        TaskHelper.RunSafely(UrdaBlessingService.AfterCardRewardSkipped(__instance));
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

        public bool SeedbedCheckRecorded { get; set; }

        public bool HumusSkipHandled { get; set; }
    }

    private sealed record Progress(
        int SeedbedChecks,
        int SeedbedAccepted,
        bool SeedbedTransformed,
        int HumusSkips,
        bool HumusCompleted,
        bool MoltingActive,
        int MossRoomMask)
    {
        public static Progress Default => new(0, 0, false, 0, false, false, 0);
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
        if (!IsTrackedNormalActOneCombatReward(cardReward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.Seedbed ||
            alternatives.Count >= 2)
        {
            return false;
        }

        var progress = GetProgress(player);
        if (progress.SeedbedTransformed ||
            progress.SeedbedChecks >= MaxSeedbedChecks)
        {
            return false;
        }

        var context = CardRewardContexts.GetValue(cardReward, _ => new CardRewardContext());
        if (!context.SeedbedCheckRecorded)
        {
            context.SeedbedCheckRecorded = true;
            progress = progress with { SeedbedChecks = progress.SeedbedChecks + 1 };
            SetProgress(player, progress);
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_SEEDBED",
            () => AcceptSeedbed(player),
            PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    public static async Task AfterCardRewardSkipped(CardReward reward)
    {
        var player = reward.Player;
        if (!IsTrackedNormalActOneCombatReward(reward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.HumusPact)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.HumusSkipHandled)
        {
            return;
        }

        context.HumusSkipHandled = true;
        var progress = GetProgress(player);
        if (progress.HumusCompleted)
        {
            return;
        }

        progress = progress with { HumusSkips = progress.HumusSkips + 1 };
        SetProgress(player, progress);
        await PlayerCmd.GainGold(HumusGoldPerSkip, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Humus Pact applied: skipped normal combat card reward {progress.HumusSkips}/{HumusRequiredSkips}; gained {HumusGoldPerSkip} gold.");

        if (progress.HumusSkips >= HumusRequiredSkips)
        {
            progress = progress with { HumusCompleted = true };
            SetProgress(player, progress);
            await ResolveHumusCompletion(player);
        }
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
        if (progress.SeedbedTransformed || progress.SeedbedAccepted >= MaxSeedbedChecks)
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

        progress = progress with { SeedbedAccepted = progress.SeedbedAccepted + 1 };
        if (progress.SeedbedAccepted >= MaxSeedbedChecks)
        {
            progress = progress with { SeedbedTransformed = true };
            await CreatureCmd.GainMaxHp(player.Creature, SeedbedCompletionMaxHpGain);
        }

        SetProgress(player, progress);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seedbed applied: accepted {progress.SeedbedAccepted}/{MaxSeedbedChecks}; transformed={progress.SeedbedTransformed}.");
    }

    private static async Task ResolveHumusCompletion(Player player)
    {
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

        var rewardCard = CreateRandomRewardCard(player);
        if (rewardCard == null)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Humus Pact skipped upgraded card reward: no valid reward card could be generated.");
            return;
        }

        if (rewardCard.IsUpgradable)
        {
            CardCmd.Upgrade(rewardCard);
        }

        await RewardsCmd.OfferCustom(player, [new SpecialCardReward(rewardCard, player)]);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Humus Pact completed: removed {selectedRemovals.Count} card(s) and offered upgraded {rewardCard.Id.Entry}.");
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
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster);
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

        return new Progress(
            ParseInt(parts[1]),
            ParseInt(parts[2]),
            ParseBool(parts[3]),
            ParseInt(parts[4]),
            ParseBool(parts[5]),
            ParseBool(parts[6]),
            ParseInt(parts[7]));
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
            progress.MoltingActive ? 1 : 0,
            progress.MossRoomMask);
    }

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}
