using Xunit;

namespace EZMicroBalance.Tests;

public sealed class LothaPolishGuardTests
{
    private static readonly (string AncientKey, string RelicKey)[] LothaDescriptionKeys =
    [
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description", "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description", "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description", "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description", "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description", "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description", "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description", "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description", "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description")
    ];

    private static readonly string[] MojibakeFragments =
    [
        "\uFFFD",
        "\u951F?"
    ];

    [Fact]
    public void CombatLifecycleUsesScopedCombatStateInsteadOfGlobalRunStateLookup()
    {
        var lifecycle = ReadLothaSource();

        Assert.DoesNotContain("RunManager.Instance.DebugOnlyGetState()", lifecycle, StringComparison.Ordinal);
        AssertSourceContains(
            lifecycle,
            "CombatManager.Instance.DebugOnlyGetState()",
            "activeCombatState.Players.Where(player => player.IsActiveForHooks)",
            "room.CombatState.RunState.Players.Where(player => player.IsActiveForHooks).ToList()");
    }

    [Fact]
    public void MirrorRebuttalUsesChosenDeckCardAndPowerZeroReplacement()
    {
        var ancient = ReadLothaSource();
        var runHook = ReadLothaSource();
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "CardSelectCmd.FromDeckGeneric",
            "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt",
            "LothaBlessingService.IsMirrorRebuttalDeckCardCandidate",
            "LothaBlessingService.MarkMirrorRebuttalCard",
            "HoverTipFactory.Static(StaticHoverTip.ReplayStatic)",
            "HoverTipFactory.Static(StaticHoverTip.Energy)");
        AssertSourceContains(
            savedFields,
            "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard",
            "\"EZMicroBalanceLothaMirrorRebuttalCard\"");
        AssertSourceContains(
            runHook,
            "MirrorRebuttalExtraPlayCount = 1",
            "TryMoveMirrorRebuttalCardToHand",
            "CardPileCmd.Add(selectedCard, PileType.Hand)",
            "PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand",
            "CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top)",
            "could not move selected card",
            "addResult.cardAdded.Pile?.Type == PileType.Hand",
            "IsMirrorRebuttalCombatCard",
            "AncientSavedStateFields.LothaMirrorRebuttalCard[deckCard]",
            "TryResolveMirrorRebuttalPowerFallback",
            "IsPowerReplacementCostZeroCard",
            "PowerReplacementCardPendingBenefit",
            "used the Power-card replacement benefit: cost 0");

        Assert.DoesNotContain("MirrorRebuttalMinimumBlock", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("MirrorRebuttalArmed", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("after unblocked damage", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.OrdinalIgnoreCase);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "Choose [blue]1[/blue] mirror card from your deck",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "cost [blue]0[/blue]",
            "play [blue]1[/blue] extra time");
    }

    [Fact]
    public void MirrorHallEchoRecordsLastTurnTypeAndRejectsCopyCardPlaceholder()
    {
        var runHook = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            runHook,
            "public override Task AfterSideTurnEnd",
            "MirrorHallEchoRecordedType",
            "MirrorHallEchoArmedType",
            "MirrorHallEchoExtraPlayCount = 1",
            "RecordMirrorHallEchoType",
            "CombatManager.Instance.History.CardPlaysFinished",
            "!entry.CardPlay.IsAutoPlay",
            "!entry.CardPlay.Card.IsClone",
            "entry.HappenedThisTurn(player.Creature.CombatState)",
            "TryResolveMirrorHallEchoPowerFallback",
            "IsPowerReplacementCostZeroCard",
            "Lotha Mirror Hall Echo used the Power-card replacement benefit: cost 0 and draw 1");

        Assert.DoesNotContain("TryCreateMirrorHallEcho", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("copy.EnergyCost", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardKeyword.Ethereal", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardKeyword.Exhaust", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"],
            "At turn end",
            "remember the last",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "play [blue]1[/blue] extra time",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void PowerReplacementEligibilityUsesActualPlayedPowerNotHandOrderOrPendingPreview()
    {
        var runHook = ReadLothaSource();

        Assert.DoesNotContain("IsCurrentEligiblePowerInHand", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("FirstOrDefault(IsPowerCard)", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("!ReferenceEquals(combatState.PowerReplacementCardPendingBenefit, cardPlay.Card)", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "CanUseMirrorRebuttalPowerReplacement(cardPlay.Card, combatState)",
            "CanUseMirrorHallEchoPowerReplacement(cardPlay.Card, combatState)",
            "CanUseDeferredVerdictPowerReplacement(cardPlay.Card, player, combatState)",
            "CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState)",
            "CanUseMirrorHallEchoPowerReplacement(card, combatState)",
            "CanUseDeferredVerdictPowerReplacement(card, player, combatState)",
            "CanUseSingleSentencePowerReplacement(card, combatState)",
            "combatState.PowerReplacementCardPendingBenefit = card",
            "combatState.PowerReplacementCardPendingBenefit = null");
    }

    [Fact]
    public void PresumptionUsesPersistentInnocentStateAndEnemyAttackDamageBreak()
    {
        var ancient = ReadLothaSource();
        var runHook = ReadLothaSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaPresumptionPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            powers,
            "internal sealed class LothaPresumptionPower",
            "PowerType.Buff",
            "PowerStackType.Single");
        AssertSourceContains(
            runHook,
            "PresumptionCards = 2",
            "PresumptionEnergy = 1",
            "PresumptionBlock = 8",
            "PresumptionHpLoss = 8",
            "BeforeCombatStart",
            "PowerCmd.Apply<LothaPresumptionPower>",
            "CardPileCmd.Draw(choiceContext, PresumptionCards, player)",
            "PlayerCmd.GainEnergy(PresumptionEnergy, player)",
            "CreatureCmd.GainBlock(player.Creature, PresumptionBlock",
            "public override Task AfterDamageReceived",
            "IsUnblockedEnemyAttackDamage",
            "result.UnblockedDamage > 0",
            "dealer is { IsEnemy: true }",
            "cardSource == null",
            "props.HasFlag(ValueProp.Move)",
            "PowerCmd.Remove<LothaPresumptionPower>",
            "CreatureCmd.Damage(");

        Assert.DoesNotContain("PresumptionUsed", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("At the start of your first turn each combat, gain [blue]8[/blue] [gold]Block[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description"],
            "[gold]Innocent[/gold]",
            "draw [blue]2[/blue] cards",
            "gain [blue]1[/blue] [gold]Energy[/gold]",
            "gain [blue]8[/blue] [gold]Block[/gold]",
            "unblocked enemy [gold]Attack[/gold] damage",
            "lose [blue]8[/blue] HP");
    }

    [Fact]
    public void ClosedCourtSuppressesOnlyCombatCardRewardsAndUsesSplitTurnResourcePlan()
    {
        var runHook = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            runHook,
            "ClosedCourtFirstTurnCards = 4",
            "ClosedCourtFirstTurnEnergy = 2",
            "ClosedCourtSecondPulseTurn = 4",
            "ClosedCourtSecondPulseCards = 2",
            "ClosedCourtSecondPulseEnergy = 2",
            "TryApplyClosedCourtTurnStart",
            "PlayerCmd.GainEnergy(ClosedCourtFirstTurnEnergy, player)",
            "PlayerCmd.GainEnergy(ClosedCourtSecondPulseEnergy, player)",
            "TryModifyRewardsLate",
            "room is not CombatRoom",
            "rewards.RemoveAll(reward => reward is CardReward)",
            "gold, potion, and relic rewards remain");

        Assert.DoesNotContain("ClosedCourtEnergy = 1", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ClosedCourtCards = 2", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is GoldReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is PotionReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is RelicReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("gain [blue]1[/blue] [gold]Energy[/gold] and draw [blue]2[/blue] cards", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"],
            "Post-combat card rewards no longer appear",
            "Turn [blue]1[/blue]",
            "draw [blue]4[/blue]",
            "gain [blue]2[/blue] [gold]Energy[/gold]",
            "Turn [blue]4[/blue]",
            "draw [blue]2[/blue]");
        Assert.DoesNotContain("[gold]Gold[/gold], potions, and relics remain", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("draw until your hand has", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void DeferredVerdictUsesTurnFourStacksAndDoesNotLeakPower()
    {
        var runHook = ReadLothaSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            runHook,
            "DeferredVerdictTurn = 4",
            "DeferredVerdictStacks = 3",
            "DeferredVerdictEnergy = 4",
            "DeferredVerdictCards = 4",
            "DeferredVerdictExtraPlayCount = 1",
            "DeferredVerdictEarlyEndHeal = 4",
            "PowerFallbackCards = 1",
            "IsPowerReplacementCostZeroCard",
            "combatState.DeferredVerdictActiveThisTurn = true",
            "PowerCmd.Apply<LothaVerdictPower>",
            "PowerCmd.Decrement(verdict)",
            "PowerCmd.Remove<LothaVerdictPower>(player.Creature)",
            "CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal");
        var deferredExtraPlayBlock = SliceBetween(
            runHook,
            "selectedBlessing == LothaBlessingIds.DeferredVerdict",
            "selectedBlessing == LothaBlessingIds.SingleSentence");
        AssertSourceContains(
            runHook,
            "private static bool IsDeferredVerdictConsumerCard(CardModel card) =>",
            "private static bool IsDeferredVerdictExtraPlayCard(CardModel card) =>",
            "IsEligibleCard(card)");
        Assert.Contains("IsDeferredVerdictExtraPlayCard(card)", deferredExtraPlayBlock, StringComparison.Ordinal);
        Assert.DoesNotContain("IsDeferredVerdictConsumerCard(card)", deferredExtraPlayBlock, StringComparison.Ordinal);
        AssertSourceContains(
            powers,
            "internal sealed class LothaVerdictPower",
            "PowerType.Buff",
            "PowerStackType.Counter");

        Assert.DoesNotContain("DeferredVerdictReadyThisTurn", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryBurstDeferredVerdict", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeferredVerdictDamagePerStack", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
            "draw [blue]4[/blue]",
            "gain [blue]4[/blue] [gold]Energy[/gold]",
            "[blue]3[/blue] [gold]Verdict[/gold]",
            "each non-Status card spends [blue]1[/blue] [gold]Verdict[/gold]",
            "[gold]Power[/gold] cards cost [blue]0[/blue]",
            "draw [blue]1[/blue]",
            "heal [blue]4[/blue] HP");
        Assert.DoesNotContain("[gold]Power[/gold] cards instead gain [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("PowerFallbackEnergy = 1", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("private const int PowerFallbackEnergy", runHook, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathReprieveIsNotTwentyFivePercentHealOnlyPlaceholder()
    {
        var ancient = ReadLothaSource();
        var runHook = ReadLothaSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaDeathReprievePower>()");
        AssertSourceContains(
            powers,
            "internal sealed class LothaDeathReprievePower",
            "PowerType.Buff",
            "PowerStackType.Single");
        AssertSourceContains(
            runHook,
            "DeathReprieveCards = 10",
            "DeathReprieveEnergy = 10",
            "ShouldDieLate(Creature creature)",
            "ShouldDie(Creature creature)",
            "AfterPreventingDeath(Creature creature)",
            "CreatureCmd.SetCurrentHp(creature, 1m)",
            "StartDeathReprieveTurn",
            "ResolveDeathReprieveTurnEnd",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "modifiedCost = 0",
            "CardPileCmd.Draw(choiceContext, DeathReprieveCards, player)",
            "PlayerCmd.GainEnergy(DeathReprieveEnergy, player)",
            "CreatureCmd.Kill(player.Creature, force: true)");

        Assert.DoesNotContain("DeathReprieveHealPercent", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("25%", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("25%", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description"],
            "set HP to [blue]1[/blue]",
            "Take one final turn",
            "draw [blue]10[/blue]",
            "gain [blue]10[/blue] [gold]Energy[/gold]",
            "cards cost [blue]0[/blue]",
            "if any enemies remain, die");
    }

    [Fact]
    public void SingleSentenceCapIgnoresAutoplayClonesAndExtraExecutions()
    {
        var runHook = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");

        AssertSourceContains(
            runHook,
            "LothaExtraPlayCount = 2",
            "SingleSentenceRemainingPlayLimit = 4",
            "SingleSentenceReadyDisplayAmount = SingleSentenceRemainingPlayLimit + 1",
            "TryResolveSingleSentencePowerFallback",
            "SingleSentencePowerFallbackUsedThisTurn",
            "IsPowerReplacementCostZeroCard",
            "HydrateSingleSentenceFromPower",
            "EnsureSingleSentencePower",
            "SetSingleSentencePowerAmount",
            "LothaSingleSentencePower",
            "Lotha Single Sentence Power fallback cost 0, drew 1 card, and did not consume the sentence",
            "autoPlayType == AutoPlayType.None",
            "combatState.AutoPlayCardPendingModifier = card",
            "return true;",
            "TrackSingleSentenceRemainingPlays",
            "!cardPlay.IsFirstInSeries",
            "cardPlay.IsAutoPlay",
            "cardPlay.Card.IsClone",
            "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)");

        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"],
            "first [gold]Attack[/gold] or [gold]Skill[/gold]",
            "plays [blue]2[/blue] extra times",
            "play up to [blue]4[/blue] more cards",
            "[gold]Power[/gold] cards do not count",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description"],
            "ready",
            "counter becomes [blue]4[/blue]",
            "remaining card plays");
        Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("exactly one card", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SingleSentenceBranchesAreGuardedBeforeAndAfterTheRuling()
    {
        var cardRules = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardRules.cs");
        var cardPlayDispatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardPlayDispatch.cs");
        var cardPlayCount = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardPlayCount.cs");
        var cardEligibility = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardEligibility.cs");
        var powerReplacement = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.PowerReplacement.cs");
        var singleSentence = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.SingleSentence.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        var shouldPlay = SliceFrom(cardRules, "public static bool ShouldPlay");
        var powerFallback = SliceBetween(singleSentence, "private static async Task TryResolveSingleSentencePowerFallback", "private static void TrackSingleSentenceRemainingPlays");
        var playTracker = SliceBetween(singleSentence, "private static void TrackSingleSentenceRemainingPlays", "private static async Task EnsureSingleSentencePower");
        var powerEligibility = SliceFrom(singleSentence, "private static bool CanUseSingleSentencePowerReplacement");
        var eligibleCard = SliceBetween(cardEligibility, "private static bool IsEligibleCard", "private static bool IsDeferredVerdictConsumerCard");
        var powerCard = SliceFrom(powerReplacement, "private static bool IsPowerCard");

        AssertSourceContains(
            powerFallback,
            "combatState.SingleSentenceUsedThisTurn ||",
            "combatState.SingleSentencePowerFallbackUsedThisTurn ||",
            "!cardPlay.IsFirstInSeries ||",
            "cardPlay.IsAutoPlay ||",
            "!CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState)",
            "combatState.SingleSentencePowerFallbackUsedThisTurn = true",
            "await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner)");
        AssertBefore(powerFallback, "combatState.SingleSentenceUsedThisTurn ||", "await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner)");

        AssertSourceContains(
            powerEligibility,
            "!combatState.SingleSentenceUsedThisTurn",
            "!combatState.SingleSentencePowerFallbackUsedThisTurn",
            "IsPowerCard(card)");
        Assert.DoesNotContain("CardType.Attack", powerEligibility, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Skill", powerEligibility, StringComparison.Ordinal);

        AssertSourceContains(
            eligibleCard,
            "card.Type is CardType.Attack or CardType.Skill",
            "!card.IsClone");
        Assert.DoesNotContain("CardType.Power", eligibleCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Status", eligibleCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Curse", eligibleCard, StringComparison.Ordinal);

        AssertSourceContains(
            powerCard,
            "card.Type == CardType.Power",
            "!card.IsClone");
        Assert.DoesNotContain("CardType.Attack", powerCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Skill", powerCard, StringComparison.Ordinal);

        AssertSourceContains(
            playTracker,
            "!combatState.SingleSentenceUsedThisTurn ||",
            "!cardPlay.IsFirstInSeries ||",
            "cardPlay.IsAutoPlay ||",
            "cardPlay.Card.IsClone",
            "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)",
            "combatState.SingleSentenceRulingCard = null",
            "combatState.SingleSentenceRemainingCardsPlayedThisTurn++");
        AssertBefore(playTracker, "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)", "combatState.SingleSentenceRemainingCardsPlayedThisTurn++");

        AssertSourceContains(
            shouldPlay,
            "autoPlayType == AutoPlayType.None",
            "combatState.AutoPlayCardPendingModifier = card",
            "GetSelectedBlessing(player) != LothaBlessingIds.SingleSentence",
            "!combatState.SingleSentenceUsedThisTurn",
            "var canPlay = combatState.SingleSentenceRemainingCardsPlayedThisTurn < SingleSentenceRemainingPlayLimit",
            "SetSingleSentencePowerAmount(player, 0)",
            "return canPlay;");
        Assert.DoesNotContain("<= SingleSentenceRemainingPlayLimit", shouldPlay, StringComparison.Ordinal);
        Assert.DoesNotContain("public static int ModifyCardPlayCount", cardRules, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task AfterCardPlayed", cardRules, StringComparison.Ordinal);
        AssertSourceContains(
            cardPlayDispatch,
            "public static async Task AfterCardPlayed",
            "TryResolveMirrorRebuttalPowerFallback(choiceContext, cardPlay, combatState)",
            "TryResolveMirrorHallEchoPowerFallback(choiceContext, cardPlay, combatState)",
            "TryResolveDeferredVerdictCard(choiceContext, cardPlay, combatState)",
            "TryResolveSingleSentencePowerFallback(choiceContext, cardPlay, combatState)",
            "TrackSingleSentenceRemainingPlays(cardPlay, combatState)");
        AssertSourceContains(
            cardPlayCount,
            "public static int ModifyCardPlayCount",
            "LothaExtraPlayCount = 2",
            "MirrorRebuttalExtraPlayCount = 1",
            "selectedBlessing == LothaBlessingIds.SingleSentence",
            "LogExtraPlayAttempt(");

        var engText = engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        var zhsText = zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        AssertSourceContains(
            engText,
            "first [gold]Attack[/gold] or [gold]Skill[/gold]",
            "plays [blue]2[/blue] extra times",
            "play up to [blue]4[/blue] more cards",
            "[gold]Power[/gold] cards do not count",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        AssertSourceContains(
            zhsText,
            "[gold]",
            "[/gold]",
            "[blue]4[/blue]",
            "[blue]0[/blue]",
            "[blue]1[/blue]");
        foreach (var text in new[] { engText, zhsText })
        {
            Assert.DoesNotContain("exactly one card", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Block", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Strength", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Energy refund", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("格挡", text, StringComparison.Ordinal);
            Assert.DoesNotContain("力量", text, StringComparison.Ordinal);
            Assert.DoesNotContain("获得[blue]1[/blue]点[gold]能量[/gold]", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PublicEvidenceUsesNonDamageDebuffPolicyAndVisibleEnlightenment()
    {
        var ancient = ReadLothaSource();
        var runHook = ReadLothaSource();
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var poison = ReadRepoText("source code", "src", "Core", "Models", "Powers", "PoisonPower.cs");
        var weak = ReadRepoText("source code", "src", "Core", "Models", "Powers", "WeakPower.cs");
        var vulnerable = ReadRepoText("source code", "src", "Core", "Models", "Powers", "VulnerablePower.cs");
        var frail = ReadRepoText("source code", "src", "Core", "Models", "Powers", "FrailPower.cs");
        var helper = SliceBetween(runHook, "private static bool IsPublicEvidenceDebuffApplication", "private static bool IsPublicEvidenceExcludedDamageDebuff");
        var publicEvidenceSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.PublicEvidence.cs");
        var excludedDamageDebuffs = SliceFrom(publicEvidenceSource, "private static bool IsPublicEvidenceExcludedDamageDebuff");
        var givenHook = SliceBetween(runHook, "public static decimal ModifyPowerAmountGiven", "public static bool TryModifyPowerAmountReceived");
        var receivedHook = SliceBetween(runHook, "public static bool TryModifyPowerAmountReceived", "public static async Task AfterPowerAmountChanged");
        var changedHook = SliceBetween(runHook, "public static async Task AfterPowerAmountChanged", "private static async Task ConsumePublicEvidenceEnlightenmentAtTurnStart");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            runHook,
            "ModifyPowerAmountGiven",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged",
            "PublicEvidenceEnlightenmentGain = 1",
            "PublicEvidenceConsumeLimit = 3",
            "PublicEvidenceBlockPerEnlightenment = 4",
            "PublicEvidenceCardsPerEnlightenment = 1",
            "amount * 2m",
            "PowerCmd.Apply<LothaEnlightenmentPower>",
            "RemoveOnePublicEvidenceEnlightenment",
            "ConsumePublicEvidenceEnlightenmentAtTurnStart",
            "IsPublicEvidenceDebuffApplication(power, amount)",
            "IsPublicEvidenceDebuffApplication(canonicalPower, amount)",
            "power.GetTypeForAmount(amount) == PowerType.Debuff");
        AssertSourceContains(
            helper,
            "power.GetTypeForAmount(amount) == PowerType.Debuff",
            "!IsPublicEvidenceExcludedDamageDebuff(power)");
        AssertSourceContains(
            excludedDamageDebuffs,
            "power is PoisonPower",
            "or ConstrictPower",
            "or DemisePower",
            "or DisintegrationPower",
            "or DoomPower",
            "or MagicBombPower",
            "or StranglePower",
            "or TheGambitPower",
            "Core v0.106.1 models these as Debuffs");
        AssertSourceContains(
            powers,
            "internal sealed class LothaEnlightenmentPower",
            "PowerType.Buff",
            "PowerStackType.Counter");

        Assert.DoesNotContain("HasVisibleDebuff", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("PublicEvidenceUsedThisTurn", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            givenHook,
            "target is not { IsEnemy: true }",
            "!giver.IsPlayer",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            receivedHook,
            "!target.IsPlayer",
            "applier is not { IsEnemy: true }",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            changedHook,
            "applier is { IsPlayer: true, Player: { } applyingPlayer }",
            "power.Owner.IsEnemy",
            "applier is { IsEnemy: true }",
            "power.Owner is { IsPlayer: true, Player: { } targetPlayer }");
        AssertSourceContains(
            poison,
            "public override PowerType Type => PowerType.Debuff",
            "AfterSideTurnStart",
            "CreatureCmd.Damage",
            "ValueProp.Unblockable | ValueProp.Unpowered");
        AssertSourceContains(
            weak,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            vulnerable,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            frail,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyBlockMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        Assert.DoesNotContain("CreatureCmd.Damage", weak, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", vulnerable, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", frail, StringComparison.Ordinal);

        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "Your non-damaging [gold]negative status[/gold] stacks apply twice",
            "grant [blue]1[/blue] [gold]Enlightenment[/gold]",
            "Enemy non-damaging [gold]negative status[/gold] stacks on you also apply twice",
            "remove [blue]1[/blue] [gold]Enlightenment[/gold]",
            "spend up to [blue]3[/blue] [gold]Enlightenment[/gold]",
            "each spent stack draws [blue]1[/blue] and gives [blue]4[/blue] [gold]Block[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "[gold]",
            "[/gold]",
            "[blue]3[/blue]",
            "[blue]1[/blue]",
            "[blue]4[/blue]");
        Assert.Equal(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            engRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
        Assert.Equal(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            zhsRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
    }

    [Fact]
    public void PublicEvidenceDocsCloseDebuffAmbiguityWithoutPoisonClaim()
    {
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var issue = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            riskRegister,
            "source-closed / live-pending",
            "WeakPower",
            "VulnerablePower",
            "FrailPower",
            "PoisonPower",
            "ConstrictPower",
            "DoomPower",
            "damage/kill Debuffs");
        Assert.DoesNotContain("Define exact source-backed debuff list and ownership.", riskRegister, StringComparison.Ordinal);

        foreach (var doc in new[] { sourceDesign, manualChecklist, issue })
        {
            AssertSourceContains(
                doc,
                "non-damaging negative",
                "Weak",
                "Vulnerable",
                "Frail",
                "Poison",
                "damage-over-time",
                "countdown damage");
        }

        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\uFFFD", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void LothaLocalizationHoverAndRichTextAreReadable()
    {
        var ancient = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaPresumptionPower>()",
            "HoverTipFactory.FromPower<LothaVerdictPower>()",
            "HoverTipFactory.FromPower<LothaDeathReprievePower>()",
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.ReplayStatic)",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");

        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.title",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.description",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription"
            ],
            engPowers,
            zhsPowers,
            "Lotha power localization");

        foreach (var (ancientKey, relicKey) in LothaDescriptionKeys)
        {
            Assert.Equal(engAncients[ancientKey], engRelics[relicKey]);
            Assert.Equal(zhsAncients[ancientKey], zhsRelics[relicKey]);
        }

        foreach (var value in LothaOptionValues(zhsAncients, zhsRelics, zhsPowers))
        {
            AssertNoMojibake(value, MojibakeFragments);
            Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
            Assert.DoesNotContain("閺€", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]1[/blue]点[gold]能量[/gold]并抽[blue]1[/blue]张牌", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]2[/blue]点[gold]能量[/gold]并抽[blue]2[/blue]张牌", value, StringComparison.Ordinal);
        }

        foreach (var value in LothaOptionValues(engAncients, engRelics, engPowers))
        {
            Assert.DoesNotContain("option art marker", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Every third", value, StringComparison.Ordinal);
            Assert.DoesNotContain("replayed [blue]2[/blue] times as [gold]Exhaust[/gold] copies", value, StringComparison.Ordinal);
            Assert.DoesNotContain("25%", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] cards instead gain [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("exactly one card", value, StringComparison.OrdinalIgnoreCase);
        }

        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[blue]1[/blue]",
            "[blue]0[/blue]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
            "[gold]Verdict[/gold]",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "[gold]negative status[/gold]",
            "[gold]Enlightenment[/gold]",
            "[gold]Block[/gold]",
            "[blue]3[/blue]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"],
            "[gold]裁决[/gold]",
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"],
            "[gold]开悟[/gold]",
            "[gold]格挡[/gold]");
    }
    private static IEnumerable<string> LothaOptionValues(params IReadOnlyDictionary<string, string>[] maps)
    {
        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (key.Contains("LOTHA", StringComparison.Ordinal) || key.Contains("lotha", StringComparison.Ordinal))
                {
                    yield return value;
                }
            }
        }
    }

    private static string ReadLothaSource() =>
        ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
}
