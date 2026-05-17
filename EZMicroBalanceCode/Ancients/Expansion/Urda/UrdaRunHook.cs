using System.Runtime.CompilerServices;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Map;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaRunHook : AbstractModel
{
    public UrdaRunHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => true;

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

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.BeforeRoomEntered(room);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        UrdaBlessingService.SyncPersistentState(card.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterActEntered()
    {
        return UrdaBlessingService.AfterActEntered();
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return UrdaBlessingService.AfterRoomEntered(room);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return UrdaBlessingService.AfterCardPlayed(choiceContext, cardPlay);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        return UrdaBlessingService.AfterCombatVictory(room);
    }

    public override bool ShouldDieLate(Creature creature)
    {
        return UrdaBlessingService.ShouldDieLate(creature);
    }

    public override Task AfterPreventingDeath(Creature creature)
    {
        return UrdaBlessingService.AfterPreventingDeath(creature);
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
    private const int TrialBranchOfferCount = 4;
    private const int TrialBranchCombats = 3;
    private const int TrialBranchRequiredSuccesses = 2;
    private const int ShallowRootRelicChoices = 2;
    private const int ShallowRootInitialGold = 75;
    private const int ShallowRootEliteGold = 35;
    private const int ShallowRootSettlementMaxHpLoss = 6;
    private const int RootedRouteMaxTargetFloor = 7;
    private const int RootedRouteCardRewards = 3;
    private const int RootedRouteWitherHpLoss = 8;
    private const int RootedRouteWitherGold = 25;
    private const int AfterRainBlock = 15;
    private const int AfterRainDraw = 1;
    private const int AfterRainWounds = 2;
    private const int AfterRainMaxHpLoss = 3;
    private const int AfterRainCompensationHeal = 8;
    private const int AfterRainCompensationGold = 75;
    private const int AfterRainEliteGold = 20;
    private const int AfterRainEliteGoldLimit = 2;
    private const int RootSightStartingEyes = 5;
    private const int SeedBankMaxSeeds = 3;
    private const int SeedBankMaxSettlementCards = 2;
    private static readonly HashSet<RoomType> RootSightUnknownBlacklist =
    [
        RoomType.Shop,
        RoomType.Treasure,
        RoomType.RestSite,
        RoomType.Boss
    ];

    private static Player? RootSightSelectionPlayer;

    private sealed class CardRewardContext
    {
        public bool IsNormalActOneCombatCardReward { get; set; }

        public bool HumusPactHandled { get; set; }

        public bool SeedBankHandled { get; set; }
    }

    private sealed record Progress(
        int SeedbedChecks,
        int SeedbedAccepted,
        bool SeedbedTransformed,
        int HumusSkips,
        bool HumusCompleted,
        bool HumusCompletionPending,
        bool MoltingActive,
        int MossRoomMask,
        int TrialCombats,
        int TrialSuccessfulCombats,
        bool TrialPlayedThisCombat,
        bool TrialSettled,
        bool ShallowRelicPending,
        bool ShallowRelicRooted,
        string ShallowRelicId,
        string RootedRouteCoord,
        bool RootedRouteResolved,
        bool RootedRouteWithered,
        bool AfterRainSpent,
        bool AfterRainCompensated,
        int AfterRainEliteGoldCount,
        int RootSightEyes,
        bool RootSightFirstPotionGranted,
        string RootSightMarkedCoords,
        string SeedBankCardIds,
        bool SeedBankSettled,
        string RootSightPreviewRecords)
    {
        public static Progress Default => new(
            0,
            0,
            false,
            0,
            false,
            false,
            false,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            string.Empty,
            string.Empty,
            false,
            false,
            false,
            false,
            0,
            0,
            false,
            string.Empty,
            string.Empty,
            false,
            string.Empty);
    }

    private sealed record RootSightPreview(
        int ActIndex,
        string Coord,
        MapPointType PointType,
        RoomType RoomType,
        string ModelId);

    private static readonly ConditionalWeakTable<CardReward, CardRewardContext> CardRewardContexts = new();

    public static void SetSelectedBlessing(Player player, string blessingId)
    {
        SetState(player, blessingId, Progress.Default);
    }

    public static string GetSelectedBlessing(Player player)
    {
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
        var separatorIndex = state.IndexOf(ProgressSeparator);
        return separatorIndex < 0 ? state : state[..separatorIndex];
    }

    public static void SyncPersistentState(Player? player)
    {
        if (player == null)
        {
            return;
        }

        AncientPlayerState.SyncDeck(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
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
            UrdaBlessingIds.SeedBank => TryAddSeedBankAlternative(player, cardReward, alternatives),
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

    private static bool TryAddSeedBankAlternative(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        var progress = GetProgress(player);
        if (progress.SeedBankSettled || GetSeedBankCardIds(progress).Count >= SeedBankMaxSeeds)
        {
            return false;
        }

        alternatives.Add(new CardRewardAlternative(
            "EZMB_URDA_SEED_BANK_STORE",
            () => ChooseSeedBankStore(player, cardReward),
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

    private static async Task ChooseSeedBankStore(Player player, CardReward reward)
    {
        if (!IsTrackedNormalActOneCombatReward(reward) ||
            GetSelectedBlessing(player) != UrdaBlessingIds.SeedBank)
        {
            return;
        }

        var context = CardRewardContexts.GetValue(reward, _ => new CardRewardContext());
        if (context.SeedBankHandled)
        {
            return;
        }

        context.SeedBankHandled = true;
        var progress = GetProgress(player);
        var seedIds = GetSeedBankCardIds(progress);
        if (progress.SeedBankSettled || seedIds.Count >= SeedBankMaxSeeds)
        {
            return;
        }

        var rewardCards = reward.Cards.ToList();
        if (rewardCards.Count == 0)
        {
            return;
        }

        var selected = rewardCards.Count == 1
            ? rewardCards[0]
            : (await CardSelectCmd.FromSimpleGrid(
                new BlockingPlayerChoiceContext(),
                rewardCards,
                player,
                new CardSelectorPrefs(UrdaLoc("urda_seed_bank.storeSelectionPrompt"), 1))).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        seedIds.Add(selected.Id.ToString());
        progress = progress with { SeedBankCardIds = string.Join(",", seedIds.Take(SeedBankMaxSeeds)) };
        SetProgress(player, progress);
        RefreshSeedBankRelicStatus(player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seed Bank stored {selected.Id.Entry}; stored {seedIds.Count}/{SeedBankMaxSeeds}. The source-safe slice consumes this card reward to store the Seed.");
    }

    public static async Task ApplyTrialBranch(Player player)
    {
        var offers = CreateTrialBranchOffers(player);
        if (offers.Count == 0)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Trial Branch could not create source-safe card offers.");
            return;
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            offers,
            player,
            new CardSelectorPrefs(UrdaLoc("urda_trial_branch.selectionScreenPrompt"), 1)
            {
                RequireManualConfirmation = true
            })).FirstOrDefault();

        foreach (var offer in offers.Where(offer => offer != selected))
        {
            AncientCardHelpers.RemoveUnpiledRunCard(offer);
        }

        if (selected == null)
        {
            return;
        }

        if (selected.IsUpgradable)
        {
            CardCmd.Upgrade(selected, CardPreviewStyle.None);
        }

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        if (addResult.success)
        {
            AncientSavedStateFields.UrdaTrialPlantCard[addResult.cardAdded] = true;
            CardCmd.PreviewCardPileAdd(addResult, 2f);
            SetProgress(player, GetProgress(player) with
            {
                TrialCombats = 0,
                TrialSuccessfulCombats = 0,
                TrialPlayedThisCombat = false,
                TrialSettled = false
            });
            MainFile.Logger.Info($"[EZMicroBalance] Urda Trial Branch added upgraded Trial Plant card {selected.Id.Entry}.");
        }
        else
        {
            AncientCardHelpers.RemoveUnpiledRunCard(selected);
        }
    }

    public static async Task ApplyShallowRootRelic(Player player)
    {
        var relics = new List<RelicModel>();
        for (var i = 0; i < ShallowRootRelicChoices; i++)
        {
            var relic = RelicFactory.PullNextRelicFromFront(
                player,
                RelicRarity.Common,
                candidate => relics.All(existing => existing.Id != candidate.Id)).ToMutable();
            relics.Add(relic);
        }

        var selected = await RelicSelectCmd.FromChooseARelicScreen(player, relics);
        if (selected == null)
        {
            return;
        }

        await RelicCmd.Obtain(selected, player);
        await PlayerCmd.GainGold(ShallowRootInitialGold, player);
        SetProgress(player, GetProgress(player) with
        {
            ShallowRelicPending = true,
            ShallowRelicRooted = false,
            ShallowRelicId = selected.Id.ToString()
        });
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Shallow-Root Relic granted {selected.Id.Entry} and {ShallowRootInitialGold} Gold.");
    }

    public static void ApplyRootedRoute(Player player)
    {
        var progress = GetProgress(player);
        var target = FindRootedRouteTarget(player);
        if (target == null)
        {
            SetProgress(player, progress with { RootedRouteWithered = true });
            MainFile.Logger.Warn("[EZMicroBalance] Urda Rooted Route could not find a source-safe reachable Act 1 normal combat target.");
            return;
        }

        EnsureQuestMarker<UrdaRootedRouteMapQuestMarker>(target);
        SetProgress(player, progress with
        {
            RootedRouteCoord = FormatCoord(target.coord),
            RootedRouteResolved = false,
            RootedRouteWithered = false
        });
        MainFile.Logger.Info($"[EZMicroBalance] Urda Rooted Route marked reachable normal combat node {target.coord.col},{target.coord.row}.");
    }

    public static async Task ApplyRootSight(Player player)
    {
        SetProgress(player, GetProgress(player) with
        {
            RootSightEyes = RootSightStartingEyes,
            RootSightFirstPotionGranted = false,
            RootSightMarkedCoords = string.Empty,
            RootSightPreviewRecords = string.Empty
        });
        RefreshRootSightRelicStatus(player);
        await Task.CompletedTask;
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

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            var progress = GetProgress(player);
            if (progress.MoltingActive)
            {
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

            if (selectedBlessing == UrdaBlessingIds.ShallowRootRelic)
            {
                await SettleUnrootedShallowRelicAtActTwo(player);
            }

            if (selectedBlessing == UrdaBlessingIds.AfterRain)
            {
                await CompensateAfterRainAtActTwo(player);
            }

            RefreshSeedBankRelicStatus(player);
            RefreshRootSightRelicStatus(player);
        }
    }

    public static async Task BeforeRoomEntered(AbstractRoom room)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null || runState.CurrentActIndex != 0)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            if (selectedBlessing == UrdaBlessingIds.RootedRoute)
            {
                await CheckRootedRouteBeforeRoom(player);
            }

            RefreshSeedBankRelicStatus(player);
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

        foreach (var player in runState.Players.Where(player =>
            player.IsActiveForHooks &&
            GetSelectedBlessing(player) == UrdaBlessingIds.MossMap))
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

    public static Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player == null ||
            !player.IsActiveForHooks ||
            GetSelectedBlessing(player) != UrdaBlessingIds.TrialBranch ||
            cardPlay.Card.DeckVersion is not { } deckCard ||
            !AncientSavedStateFields.UrdaTrialPlantCard[deckCard])
        {
            return Task.CompletedTask;
        }

        var progress = GetProgress(player);
        if (progress.TrialSettled || progress.TrialCombats >= TrialBranchCombats)
        {
            return Task.CompletedTask;
        }

        SetProgress(player, progress with { TrialPlayedThisCombat = true });
        return Task.CompletedTask;
    }

    public static async Task AfterCombatVictory(CombatRoom room)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        foreach (var player in runState.Players.Where(player => player.IsActiveForHooks))
        {
            var selectedBlessing = GetSelectedBlessing(player);
            if (selectedBlessing == UrdaBlessingIds.TrialBranch)
            {
                await ResolveTrialBranchCombat(player);
            }

            if (runState.CurrentActIndex != 0)
            {
                continue;
            }

            if (selectedBlessing == UrdaBlessingIds.ShallowRootRelic && room.RoomType == RoomType.Elite)
            {
                await RootShallowRelicFromElite(player);
            }

            if (selectedBlessing == UrdaBlessingIds.AfterRain && room.RoomType == RoomType.Elite)
            {
                await GrantAfterRainEliteGold(player);
            }

            if (selectedBlessing == UrdaBlessingIds.RootedRoute)
            {
                await TryResolveRootedRouteReward(player);
            }
        }
    }

    public static bool ShouldDieLate(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            player.RunState.CurrentActIndex != 0 ||
            GetSelectedBlessing(player) != UrdaBlessingIds.AfterRain)
        {
            return true;
        }

        return GetProgress(player).AfterRainSpent;
    }

    public static async Task AfterPreventingDeath(Creature creature)
    {
        if (!creature.IsPlayer ||
            creature.Player is not { } player ||
            player.RunState.CurrentActIndex != 0 ||
            GetSelectedBlessing(player) != UrdaBlessingIds.AfterRain)
        {
            return;
        }

        var progress = GetProgress(player);
        if (progress.AfterRainSpent)
        {
            await CreatureCmd.SetCurrentHp(creature, 1m);
            return;
        }

        SetProgress(player, progress with { AfterRainSpent = true });
        await CreatureCmd.SetCurrentHp(creature, 1m);
        await CreatureCmd.GainBlock(creature, AfterRainBlock, ValueProp.Move, null, fast: true);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), AfterRainDraw, player);
        if (player.Creature.CombatState is { } combatState)
        {
            for (var i = 0; i < AfterRainWounds; i++)
            {
                var wound = combatState.CreateCard<Wound>(player);
                await CardPileCmd.AddGeneratedCardToCombat(wound, PileType.Discard, player);
            }
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, AfterRainMaxHpLoss, isFromCard: false);
        MainFile.Logger.Info("[EZMicroBalance] Urda After the Rain prevented lethal Act 1 damage and spent the blessing.");
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

    private static async Task ResolveTrialBranchCombat(Player player)
    {
        var progress = GetProgress(player);
        if (progress.TrialSettled || progress.TrialCombats >= TrialBranchCombats)
        {
            return;
        }

        progress = progress with
        {
            TrialCombats = progress.TrialCombats + 1,
            TrialSuccessfulCombats = progress.TrialSuccessfulCombats + (progress.TrialPlayedThisCombat ? 1 : 0),
            TrialPlayedThisCombat = false
        };

        if (progress.TrialCombats < TrialBranchCombats)
        {
            SetProgress(player, progress);
            MainFile.Logger.Info(
                $"[EZMicroBalance] Urda Trial Branch tracked combat {progress.TrialCombats}/{TrialBranchCombats}; successes={progress.TrialSuccessfulCombats}/{TrialBranchRequiredSuccesses}.");
            return;
        }

        var trialCard = FindTrialPlantCard(player);
        if (progress.TrialSuccessfulCombats >= TrialBranchRequiredSuccesses)
        {
            if (trialCard != null)
            {
                AncientSavedStateFields.UrdaTrialPlantCard[trialCard] = false;
            }

            SetProgress(player, progress with { TrialSettled = true });
            MainFile.Logger.Info("[EZMicroBalance] Urda Trial Branch completed successfully; Trial Plant marker cleared.");
            return;
        }

        if (trialCard != null)
        {
            AncientSavedStateFields.UrdaTrialPlantCard[trialCard] = false;
            await CardPileCmd.RemoveFromDeck(trialCard);
        }

        SetProgress(player, progress with { TrialSettled = true });
        MainFile.Logger.Info("[EZMicroBalance] Urda Trial Branch failed; marked card removed from deck.");
    }

    private static async Task RootShallowRelicFromElite(Player player)
    {
        var progress = GetProgress(player);
        if (!progress.ShallowRelicPending || progress.ShallowRelicRooted)
        {
            return;
        }

        progress = progress with
        {
            ShallowRelicPending = false,
            ShallowRelicRooted = true
        };
        SetProgress(player, progress);
        await PlayerCmd.GainGold(ShallowRootEliteGold, player);
        MainFile.Logger.Info($"[EZMicroBalance] Urda Shallow-Root Relic rooted after Act 1 Elite; gained {ShallowRootEliteGold} Gold.");
    }

    private static async Task SettleUnrootedShallowRelicAtActTwo(Player player)
    {
        var progress = GetProgress(player);
        if (!progress.ShallowRelicPending || progress.ShallowRelicRooted)
        {
            return;
        }

        var relic = FindRelicById(player, progress.ShallowRelicId);
        if (relic != null)
        {
            await RelicCmd.Remove(relic);
        }

        await PlayerCmd.GainGold(ShallowRootInitialGold, player);
        SetProgress(player, progress with
        {
            ShallowRelicPending = false,
            ShallowRelicRooted = false
        });
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Shallow-Root Relic Act 2 fallback settled: removed pending relic and refunded {ShallowRootInitialGold} Gold.");
    }

    private static async Task CheckRootedRouteBeforeRoom(Player player)
    {
        var progress = GetProgress(player);
        if (progress.RootedRouteResolved ||
            progress.RootedRouteWithered ||
            string.IsNullOrWhiteSpace(progress.RootedRouteCoord))
        {
            return;
        }

        var runState = player.RunState;
        var current = runState.CurrentMapPoint;
        var target = FindPointByCoord(runState, progress.RootedRouteCoord);
        if (current == null || target == null)
        {
            await WitherRootedRoute(player, progress, "missing current or target map point");
            return;
        }

        if (SameCoord(current.coord, target.coord))
        {
            return;
        }

        var path = current.BFS_FindPath(target).ToList();
        if (target.coord.row < current.coord.row || path.Count == 0)
        {
            await WitherRootedRoute(player, progress, "target is no longer reachable from the current route");
        }
    }

    private static async Task WitherRootedRoute(Player player, Progress progress, string reason)
    {
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            RootedRouteWitherHpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);
        await PlayerCmd.GainGold(RootedRouteWitherGold, player);
        SetProgress(player, progress with { RootedRouteWithered = true });
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Rooted Route withered ({reason}); lost {RootedRouteWitherHpLoss} HP and gained {RootedRouteWitherGold} Gold.");
    }

    private static async Task TryResolveRootedRouteReward(Player player)
    {
        var progress = GetProgress(player);
        if (progress.RootedRouteResolved ||
            progress.RootedRouteWithered ||
            string.IsNullOrWhiteSpace(progress.RootedRouteCoord) ||
            player.RunState.CurrentMapPoint is not { } current ||
            !SameCoordString(current.coord, progress.RootedRouteCoord))
        {
            return;
        }

        var cards = CreateRootedRouteRewardCards(player);
        if (cards.Count > 0)
        {
            await new RewardsSet(player)
                .WithCustomRewards(cards.Select<CardModel, Reward>(card => new SpecialCardReward(card, player)).ToList())
                .WithSkippingDisallowed()
                .Offer();
        }

        await TryGivePotion(player);
        SetProgress(player, progress with { RootedRouteResolved = true });
        RemoveQuestMarker<UrdaRootedRouteMapQuestMarker>(current);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Rooted Route resolved at {current.coord.col},{current.coord.row}; offered {cards.Count} source-safe single-card reward(s).");
    }

    private static async Task GrantAfterRainEliteGold(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainSpent || progress.AfterRainEliteGoldCount >= AfterRainEliteGoldLimit)
        {
            return;
        }

        SetProgress(player, progress with { AfterRainEliteGoldCount = progress.AfterRainEliteGoldCount + 1 });
        await PlayerCmd.GainGold(AfterRainEliteGold, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda After the Rain Elite bonus granted {AfterRainEliteGold} Gold ({progress.AfterRainEliteGoldCount + 1}/{AfterRainEliteGoldLimit}).");
    }

    private static async Task CompensateAfterRainAtActTwo(Player player)
    {
        var progress = GetProgress(player);
        if (progress.AfterRainSpent || progress.AfterRainCompensated)
        {
            return;
        }

        SetProgress(player, progress with { AfterRainCompensated = true });
        await CreatureCmd.Heal(player.Creature, AfterRainCompensationHeal);
        await PlayerCmd.GainGold(AfterRainCompensationGold, player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda After the Rain Act 2 compensation granted {AfterRainCompensationHeal} HP and {AfterRainCompensationGold} Gold.");
    }

    internal static int GetRootSightEyes(Player player) =>
        GetProgress(player).RootSightEyes;

    internal static bool IsRootSightSelectionActive =>
        RootSightSelectionPlayer != null;

    internal static bool TryBeginRootSightSelection(Player player)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0 ||
            player.RunState.CurrentActIndex != 0)
        {
            return false;
        }

        if (player.RunState.Players.Count > 1)
        {
            MainFile.Logger.Warn("[EZMicroBalance] Urda Root Eyes preview is single-player only until host-authoritative map preview sync is implemented.");
            return false;
        }

        RootSightSelectionPlayer = player;
        RefreshRootSightRelicStatus(player);
        MainFile.Logger.Info("[EZMicroBalance] Urda Root Eyes selection started; choose a reachable Monster, Unknown, or Elite map node.");
        return true;
    }

    internal static bool CanRootSightTarget(MapPoint point) =>
        RootSightSelectionPlayer != null &&
        IsRootSightTarget(RootSightSelectionPlayer, point);

    internal static async Task TryCommitRootSightSelection(MapPoint point)
    {
        var player = RootSightSelectionPlayer;
        if (player == null)
        {
            return;
        }

        if (!IsRootSightTarget(player, point))
        {
            MainFile.Logger.Info(
                $"[EZMicroBalance] Urda Root Eyes ignored invalid map target {point.coord.col},{point.coord.row} ({point.PointType}).");
            return;
        }

        if (!TryCreateRootSightPreview(player.RunState, point, out var preview))
        {
            MainFile.Logger.Warn(
                $"[EZMicroBalance] Urda Root Eyes could not create a preview for {point.coord.col},{point.coord.row} ({point.PointType}).");
            return;
        }

        RootSightSelectionPlayer = null;
        var progress = GetProgress(player);
        var coord = FormatCoord(point.coord);
        var marked = GetCoordSet(progress.RootSightMarkedCoords);
        marked.Add(coord);

        var previews = GetRootSightPreviews(progress.RootSightPreviewRecords)
            .Where(existing => existing.ActIndex != player.RunState.CurrentActIndex || existing.Coord != coord)
            .Append(preview)
            .ToList();
        progress = progress with
        {
            RootSightEyes = Math.Max(0, progress.RootSightEyes - 1),
            RootSightMarkedCoords = string.Join("|", marked),
            RootSightPreviewRecords = FormatRootSightPreviews(previews)
        };
        SetProgress(player, progress);
        EnsureQuestMarker<UrdaRootSightMapQuestMarker>(point);
        RefreshRootSightRelicStatus(player);

        if (!progress.RootSightFirstPotionGranted)
        {
            await TryGivePotion(player);
            SetProgress(player, GetProgress(player) with { RootSightFirstPotionGranted = true });
        }

        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Root Eyes previewed {preview.RoomType} {preview.ModelId} at {point.coord.col},{point.coord.row}; eyes left={progress.RootSightEyes}.");
    }

    internal static bool TryGetRootSightHoverTip(MapPoint point, out HoverTip hoverTip)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
            {
                var progress = GetProgress(player);
                if (TryFindRootSightPreview(progress, runState.CurrentActIndex, FormatCoord(point.coord), out var preview) &&
                    TryGetRootSightPreviewTitle(preview, out var title))
                {
                    hoverTip = new HoverTip(title, new LocString("ancients", "EZMB_URDA.root_sight.map_hover.preview_description"));
                    return true;
                }
            }
        }

        if (CanRootSightTarget(point))
        {
            hoverTip = new HoverTip(
                new LocString("ancients", "EZMB_URDA.root_sight.selection_hover.title"),
                new LocString("ancients", "EZMB_URDA.root_sight.selection_hover.description"));
            return true;
        }

        hoverTip = new HoverTip(
            new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title"),
            new LocString("ancients", "EZMB_URDA.root_sight.map_hover.description"));
        return point.Quests.Any(quest => quest is UrdaRootSightMapQuestMarker);
    }

    internal static bool TryGetRootSightRoomTypeForCurrentPoint(
        RunManager runManager,
        MapPointType pointType,
        out RoomType roomType)
    {
        roomType = RoomType.Unassigned;
        if (!TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var preview))
        {
            return false;
        }

        roomType = preview.RoomType;
        return true;
    }

    internal static bool TryGetRootSightModelForCurrentPoint(
        RunManager runManager,
        RoomType roomType,
        MapPointType pointType,
        out AbstractModel? model)
    {
        model = null;
        if (!TryFindRootSightPreviewForCurrentPoint(runManager, pointType, out var preview) ||
            preview.RoomType != roomType)
        {
            return false;
        }

        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            if (roomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                if (eventModel == null)
                {
                    return false;
                }

                runManager.DebugOnlyGetState()?.AddVisitedEvent(eventModel);
                model = eventModel;
                return true;
            }

            var encounter = ModelDb.GetByIdOrNull<EncounterModel>(id);
            if (encounter == null)
            {
                return false;
            }

            model = encounter.ToMutable();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task TryExtractSeedBankFromRelicClick(Player player)
    {
        var progress = GetProgress(player);
        var seedIds = GetSeedBankCardIds(progress);
        if (progress.SeedBankSettled || seedIds.Count == 0)
        {
            return;
        }

        var cards = seedIds
            .Select(TryGetStoredCard)
            .OfType<CardModel>()
            .Select(card => player.RunState.CreateCard(card, player))
            .ToList();
        if (cards.Count == 0)
        {
            SetProgress(player, progress with
            {
                SeedBankCardIds = string.Empty,
                SeedBankSettled = true
            });
            return;
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            cards,
            player,
            new CardSelectorPrefs(UrdaLoc("urda_seed_bank.settlementSelectionPrompt"), 0, Math.Min(SeedBankMaxSettlementCards, cards.Count))
            {
                Cancelable = true,
                RequireManualConfirmation = true
            })).ToList();

        if (selected.Count == 0)
        {
            foreach (var card in cards)
            {
                AncientCardHelpers.RemoveUnpiledRunCard(card);
            }

            MainFile.Logger.Info("[EZMicroBalance] Urda Seed Bank extraction was canceled; stored Seeds remain available.");
            return;
        }

        foreach (var unchosen in cards.Where(card => !selected.Contains(card)))
        {
            AncientCardHelpers.RemoveUnpiledRunCard(unchosen);
        }

        for (var i = 0; i < selected.Count && i < SeedBankMaxSettlementCards; i++)
        {
            var card = selected[i];
            if (i == 0 && card.IsUpgradable)
            {
                CardCmd.Upgrade(card, CardPreviewStyle.None);
            }

            var addResult = await CardPileCmd.Add(card, PileType.Deck);
            if (addResult.success)
            {
                CardCmd.PreviewCardPileAdd(addResult, 2f);
            }
            else
            {
                AncientCardHelpers.RemoveUnpiledRunCard(card);
            }
        }

        SetProgress(player, progress with
        {
            SeedBankCardIds = string.Empty,
            SeedBankSettled = true
        });
        RefreshSeedBankRelicStatus(player);
        MainFile.Logger.Info(
            $"[EZMicroBalance] Urda Seed Bank extracted by relic click: added {Math.Min(selected.Count, SeedBankMaxSettlementCards)} Seed card(s).");
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

    private static List<CardModel> CreateTrialBranchOffers(Player player)
    {
        var pool = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(card =>
                card.Rarity is CardRarity.Common or CardRarity.Uncommon &&
                card.Type is not (CardType.Status or CardType.Curse or CardType.Quest) &&
                card.CanBeGeneratedByModifiers)
            .ToList();
        if (pool.Count == 0)
        {
            return [];
        }

        var options = new CardCreationOptions(pool, CardCreationSource.Other, CardRarityOddsType.Uniform)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, TrialBranchOfferCount, options)
            .Select(result => result.Card)
            .ToList();
    }

    private static List<CardModel> CreateRootedRouteRewardCards(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        var cards = CardFactory.CreateForReward(player, RootedRouteCardRewards, options)
            .Select(result => result.Card)
            .ToList();
        if (cards.FirstOrDefault() is { IsUpgradable: true } first)
        {
            CardCmd.Upgrade(first, CardPreviewStyle.None);
        }

        return cards;
    }

    private static CardModel? CreateRandomRewardCard(Player player)
    {
        var options = CardCreationOptions.ForRoom(player, RoomType.Monster)
            .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoUpgradeRoll);
        return CardFactory.CreateForReward(player, 1, options).FirstOrDefault()?.Card;
    }

    private static MapPoint? FindRootedRouteTarget(Player player)
    {
        var runState = player.RunState;
        var current = runState.CurrentMapPoint ?? runState.Map.StartingMapPoint;
        return EnumerateReachable(current)
            .Where(point =>
                point.coord.row > current.coord.row &&
                point.coord.row <= RootedRouteMaxTargetFloor &&
                point.PointType == MapPointType.Monster)
            .OrderBy(point => point.coord.row)
            .ThenBy(point => point.coord.col)
            .FirstOrDefault();
    }

    private static IEnumerable<MapPoint> EnumerateReachable(MapPoint start)
    {
        var seen = new HashSet<MapPoint>();
        var queue = new Queue<MapPoint>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            foreach (var child in point.Children.OrderBy(child => child.coord.row).ThenBy(child => child.coord.col))
            {
                if (!seen.Add(child))
                {
                    continue;
                }

                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    private static bool IsRootSightTarget(Player player, MapPoint point)
    {
        var progress = GetProgress(player);
        if (GetSelectedBlessing(player) != UrdaBlessingIds.RootSight ||
            progress.RootSightEyes <= 0 ||
            player.RunState.CurrentActIndex != 0 ||
            point.PointType is not (MapPointType.Monster or MapPointType.Unknown or MapPointType.Elite) ||
            GetCoordSet(progress.RootSightMarkedCoords).Contains(FormatCoord(point.coord)))
        {
            return false;
        }

        var current = player.RunState.CurrentMapPoint ?? player.RunState.Map.StartingMapPoint;
        return MapTravel.GetTravelablePointsFrom(player.RunState, current)
            .Any(candidate => SameCoord(candidate.coord, point.coord));
    }

    private static bool TryCreateRootSightPreview(
        IRunState runState,
        MapPoint point,
        out RootSightPreview preview)
    {
        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        var roomType = point.PointType switch
        {
            MapPointType.Monster => RoomType.Monster,
            MapPointType.Elite => RoomType.Elite,
            MapPointType.Unknown => RollRootSightUnknownRoomType(runState, point),
            _ => RoomType.Unassigned
        };
        var modelId = roomType switch
        {
            RoomType.Monster or RoomType.Elite => runState.Act.PullNextEncounter(roomType).Id.ToString(),
            RoomType.Event => TryPeekNextValidEvent(runState)?.Id.ToString() ?? string.Empty,
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return false;
        }

        preview = new RootSightPreview(
            runState.CurrentActIndex,
            FormatCoord(point.coord),
            point.PointType,
            roomType,
            modelId);
        return true;
    }

    private static RoomType RollRootSightUnknownRoomType(IRunState runState, MapPoint point)
    {
        var blacklist = RunManager.BuildRoomTypeBlacklist(runState.CurrentMapPointHistoryEntry, point.Children)
            .Concat(RootSightUnknownBlacklist)
            .ToHashSet();
        return runState.Odds.UnknownMapPoint.Roll(blacklist, runState);
    }

    private static EventModel? TryPeekNextValidEvent(IRunState runState)
    {
        if (runState is not RunState concreteRunState ||
            TryGetActRoomSet(runState.Act) is not { events.Count: > 0 } rooms)
        {
            return null;
        }

        for (var i = 0; i < rooms.events.Count; i++)
        {
            var candidate = rooms.events[(rooms.eventsVisited + i) % rooms.events.Count];
            if (candidate.IsAllowed(runState) && !concreteRunState.VisitedEventIds.Contains(candidate.Id))
            {
                return candidate;
            }
        }

        return rooms.NextEvent;
    }

    private static RoomSet? TryGetActRoomSet(ActModel act) =>
        AccessTools.Field(typeof(ActModel), "_rooms")?.GetValue(act) as RoomSet;

    private static bool TryFindRootSightPreviewForCurrentPoint(
        RunManager runManager,
        MapPointType pointType,
        out RootSightPreview preview)
    {
        preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
        var runState = runManager.DebugOnlyGetState();
        var current = runState?.CurrentMapPoint;
        if (runState == null || current == null)
        {
            return false;
        }

        var coord = FormatCoord(current.coord);
        foreach (var player in runState.Players.Where(player => GetSelectedBlessing(player) == UrdaBlessingIds.RootSight))
        {
            var progress = GetProgress(player);
            if (TryFindRootSightPreview(progress, runState.CurrentActIndex, coord, out preview) &&
                preview.PointType == pointType)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryFindRootSightPreview(
        Progress progress,
        int actIndex,
        string coord,
        out RootSightPreview preview)
    {
        var match = GetRootSightPreviews(progress.RootSightPreviewRecords)
            .FirstOrDefault(candidate => candidate.ActIndex == actIndex && candidate.Coord == coord);
        if (match == null)
        {
            preview = new RootSightPreview(0, string.Empty, MapPointType.Unassigned, RoomType.Unassigned, string.Empty);
            return false;
        }

        preview = match;
        return true;
    }

    private static bool TryGetRootSightPreviewTitle(RootSightPreview preview, out LocString title)
    {
        title = new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title");
        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            if (preview.RoomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                if (eventModel == null)
                {
                    return false;
                }

                title = eventModel.Title;
                return true;
            }

            var encounter = ModelDb.GetByIdOrNull<EncounterModel>(id);
            if (encounter == null)
            {
                return false;
            }

            title = encounter.Title;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static MapPoint? FindPointByCoord(IRunState runState, string coordText)
    {
        return TryParseCoord(coordText, out var col, out var row)
            ? runState.Map.GetPoint(col, row)
            : null;
    }

    private static string FormatCoord(MapCoord coord) => $"{coord.col}:{coord.row}";

    private static bool SameCoordString(MapCoord coord, string coordText) =>
        TryParseCoord(coordText, out var col, out var row) &&
        coord.col == col &&
        coord.row == row;

    private static bool SameCoord(MapCoord left, MapCoord right) =>
        left.col == right.col && left.row == right.row;

    private static bool TryParseCoord(string value, out int col, out int row)
    {
        col = 0;
        row = 0;
        var parts = value.Split(':');
        return parts.Length == 2 &&
            int.TryParse(parts[0], out col) &&
            int.TryParse(parts[1], out row);
    }

    internal static IReadOnlyList<CardModel> GetSeedBankStoredCards(Player player) =>
        GetSeedBankCardIds(GetProgress(player))
            .Select(TryGetStoredCard)
            .OfType<CardModel>()
            .ToList();

    internal static int GetSeedBankStoredCount(Player player) =>
        GetSeedBankCardIds(GetProgress(player)).Count;

    internal static bool IsSeedBankSettled(Player player) =>
        GetProgress(player).SeedBankSettled;

    private static List<string> GetSeedBankCardIds(Progress progress) =>
        SplitList(progress.SeedBankCardIds, ',').Take(SeedBankMaxSeeds).ToList();

    private static HashSet<string> GetCoordSet(string value) =>
        SplitList(value, '|').ToHashSet(StringComparer.Ordinal);

    private static IEnumerable<string> SplitList(string value, char separator) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static IReadOnlyList<RootSightPreview> GetRootSightPreviews(string value) =>
        SplitList(value, '|')
            .Select(TryParseRootSightPreview)
            .OfType<RootSightPreview>()
            .ToList();

    private static RootSightPreview? TryParseRootSightPreview(string value)
    {
        var parts = value.Split('~', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 5 ||
            !int.TryParse(parts[0], out var actIndex) ||
            !Enum.TryParse<MapPointType>(parts[2], out var pointType) ||
            !Enum.TryParse<RoomType>(parts[3], out var roomType) ||
            string.IsNullOrWhiteSpace(parts[1]) ||
            string.IsNullOrWhiteSpace(parts[4]))
        {
            return null;
        }

        return new RootSightPreview(actIndex, parts[1], pointType, roomType, parts[4]);
    }

    private static string FormatRootSightPreviews(IEnumerable<RootSightPreview> previews) =>
        string.Join("|", previews.Select(preview =>
            $"{preview.ActIndex}~{preview.Coord}~{preview.PointType}~{preview.RoomType}~{preview.ModelId}"));

    private static CardModel? TryGetStoredCard(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (id.Contains('.', StringComparison.Ordinal))
        {
            try
            {
                return ModelDb.GetByIdOrNull<CardModel>(ModelId.Deserialize(id));
            }
            catch
            {
                return null;
            }
        }

        return ModelDb.AllCards.FirstOrDefault(card => card.Id.Entry == id);
    }

    private static CardModel? FindTrialPlantCard(Player player) =>
        PileType.Deck.GetPile(player).Cards.FirstOrDefault(card => AncientSavedStateFields.UrdaTrialPlantCard[card]);

    private static RelicModel? FindRelicById(Player player, string id) =>
        player.Relics.FirstOrDefault(relic =>
            relic.Id.ToString().Equals(id, StringComparison.Ordinal) ||
            relic.Id.Entry.Equals(id, StringComparison.Ordinal));

    private static void RefreshSeedBankRelicStatus(Player player)
    {
        var relic = player.Relics.OfType<UrdaSeedBankOptionRelic>().FirstOrDefault();
        if (relic == null)
        {
            return;
        }

        var progress = GetProgress(player);
        var storedCount = GetSeedBankCardIds(progress).Count;
        relic.Status = progress.SeedBankSettled
            ? RelicStatus.Disabled
            : storedCount > 0
                ? RelicStatus.Active
                : RelicStatus.Normal;
        relic.RefreshStoredSeedDisplay();
    }

    private static void RefreshRootSightRelicStatus(Player player)
    {
        var relic = player.Relics.OfType<UrdaRootSightOptionRelic>().FirstOrDefault();
        if (relic == null)
        {
            return;
        }

        var progress = GetProgress(player);
        relic.Status = progress.RootSightEyes > 0 && player.RunState.CurrentActIndex == 0
            ? RelicStatus.Active
            : RelicStatus.Disabled;
        relic.RefreshRootSightDisplay();
    }

    private static void EnsureQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        if (point.Quests.Any(quest => quest is TMarker))
        {
            return;
        }

        point.AddQuest(ModelDb.GetById<TMarker>(ModelDb.GetId<TMarker>()));
    }

    private static void RemoveQuestMarker<TMarker>(MapPoint point)
        where TMarker : AbstractModel
    {
        var marker = point.Quests.FirstOrDefault(quest => quest is TMarker);
        if (marker != null)
        {
            point.RemoveQuest(marker);
        }
    }

    private static LocString UrdaLoc(string suffix) =>
        new("ancients", $"EZMB_URDA.pages.INITIAL.options.{suffix}");

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
        var state = AncientPlayerState.Get(
            player,
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
        var parts = state.Split(ProgressSeparator);
        if (parts.Length < 8)
        {
            return Progress.Default;
        }

        var hasHumusPendingField = parts.Length >= 9;
        var baseIndex = hasHumusPendingField ? 9 : 8;
        return new Progress(
            ParseInt(parts[1]),
            ParseInt(parts[2]),
            ParseBool(parts[3]),
            ParseInt(parts[4]),
            ParseBool(parts[5]),
            hasHumusPendingField && ParseBool(parts[6]),
            ParseBool(parts[hasHumusPendingField ? 7 : 6]),
            ParseInt(parts[hasHumusPendingField ? 8 : 7]),
            ParseInt(GetPart(parts, baseIndex)),
            ParseInt(GetPart(parts, baseIndex + 1)),
            ParseBool(GetPart(parts, baseIndex + 2)),
            ParseBool(GetPart(parts, baseIndex + 3)),
            ParseBool(GetPart(parts, baseIndex + 4)),
            ParseBool(GetPart(parts, baseIndex + 5)),
            GetPart(parts, baseIndex + 6),
            GetPart(parts, baseIndex + 7),
            ParseBool(GetPart(parts, baseIndex + 8)),
            ParseBool(GetPart(parts, baseIndex + 9)),
            ParseBool(GetPart(parts, baseIndex + 10)),
            ParseBool(GetPart(parts, baseIndex + 11)),
            ParseInt(GetPart(parts, baseIndex + 12)),
            ParseInt(GetPart(parts, baseIndex + 13)),
            ParseBool(GetPart(parts, baseIndex + 14)),
            GetPart(parts, baseIndex + 15),
            GetPart(parts, baseIndex + 16),
            ParseBool(GetPart(parts, baseIndex + 17)),
            GetPart(parts, baseIndex + 18));
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
        AncientPlayerState.Set(
            player,
            string.Join(
                ProgressSeparator,
                blessingId,
                progress.SeedbedChecks,
                progress.SeedbedAccepted,
                progress.SeedbedTransformed ? 1 : 0,
                progress.HumusSkips,
                progress.HumusCompleted ? 1 : 0,
                progress.HumusCompletionPending ? 1 : 0,
                progress.MoltingActive ? 1 : 0,
                progress.MossRoomMask,
                progress.TrialCombats,
                progress.TrialSuccessfulCombats,
                progress.TrialPlayedThisCombat ? 1 : 0,
                progress.TrialSettled ? 1 : 0,
                progress.ShallowRelicPending ? 1 : 0,
                progress.ShallowRelicRooted ? 1 : 0,
                SanitizeStateField(progress.ShallowRelicId),
                SanitizeStateField(progress.RootedRouteCoord),
                progress.RootedRouteResolved ? 1 : 0,
                progress.RootedRouteWithered ? 1 : 0,
                progress.AfterRainSpent ? 1 : 0,
                progress.AfterRainCompensated ? 1 : 0,
                progress.AfterRainEliteGoldCount,
                progress.RootSightEyes,
                progress.RootSightFirstPotionGranted ? 1 : 0,
                SanitizeStateField(progress.RootSightMarkedCoords),
                SanitizeStateField(progress.SeedBankCardIds),
                progress.SeedBankSettled ? 1 : 0,
                SanitizeStateField(progress.RootSightPreviewRecords)),
            AncientSavedStateFields.UrdaStateKey,
            AncientSavedStateFields.UrdaDeckStateKey);
    }

    private static string GetPart(string[] parts, int index) =>
        index >= 0 && index < parts.Length ? parts[index] : string.Empty;

    private static string SanitizeStateField(string value) =>
        (value ?? string.Empty).Replace(ProgressSeparator, '_');

    private static int ParseInt(string value) =>
        int.TryParse(value, out var parsed) ? Math.Max(0, parsed) : 0;

    private static bool ParseBool(string value) =>
        value == "1" || bool.TryParse(value, out var parsed) && parsed;
}

internal sealed class UrdaRootedRouteMapQuestMarker : AbstractModel
{
    public UrdaRootedRouteMapQuestMarker()
    {
    }

    public override bool ShouldReceiveCombatHooks => false;
}

internal sealed class UrdaRootSightMapQuestMarker : AbstractModel
{
    public UrdaRootSightMapQuestMarker()
    {
    }

    public override bool ShouldReceiveCombatHooks => false;
}
