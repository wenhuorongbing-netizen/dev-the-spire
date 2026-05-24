using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AncientExpansionReleaseCoverageGuardTests
{
    [Fact]
    public void MorviV22IsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var morviGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureGate.cs");
        var morviInitializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var morviAncient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var morviOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviOptionRelics.cs");
        var morviPowers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var morviBlessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingIds.cs");
        var morviSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi");
        var morviCards = morviSource;
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        Assert.Contains("MorviInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        Assert.Contains("MorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalanceMorviStateKey", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviBorrowedAncientCard", savedFields, StringComparison.Ordinal);
        Assert.Contains("MorviOpenBookSealedCard", savedFields, StringComparison.Ordinal);

        AssertSourceContains(
            morviGate,
            "DisableEnvironmentVariable = \"SPIREPLUS_DISABLE_MORVI\"",
            "LegacyDisableEnvironmentVariable = \"EZMB_DISABLE_MORVI\"",
            "ForceAncientEnvironmentVariable = \"SPIREPLUS_FORCE_ANCIENT\"",
            "LegacyForceAncientEnvironmentVariable = \"EZMB_FORCE_ANCIENT\"",
            "ForceBlessingEnvironmentVariable = \"SPIREPLUS_FORCE_MORVI_BLESSING\"",
            "LegacyForceBlessingEnvironmentVariable = \"EZMB_FORCE_MORVI_BLESSING\"",
            "ShouldForceMorvi",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable)");
        Assert.DoesNotContain("return IsTruthy(value);", morviGate, StringComparison.Ordinal);

        AssertSourceContains(
            morviInitializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModHelper.SubscribeForCombatStateHooks",
            "default-on",
            "ModelDb.GetById<MorviRunHook>",
            "ModelDb.GetById<MorviCombatHook>");
        AssertSourceContains(
            morviSource,
            "CustomAncientModel",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.IsMorviEnabled(unlockState)",
            "MorviFeatureGate.ShouldForceMorvi",
            "ModelDb.AncientEvent<EzmbMorvi>()",
            "ExpectedInitialOptionCount = 3",
            "Where(IsCurrentlyAvailableOption)",
            "MorviBlessingService.HasForbiddenLoanCandidates(Owner)",
            "MorviBlessingService.TrySetSelectedBlessing",
            "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "AncientInitialOptionReroll.CanOffer",
            "OptionWithRelic<MorviForbiddenLoanOptionRelic>",
            "MorviBlessingIds.ForbiddenLoan",
            "MorviBlessingIds.MisprintPress",
            "MorviBlessingIds.RedInkOverdraft",
            "MorviBlessingIds.OverdueLibrary",
            "MorviBlessingIds.OpenBookExam",
            "MorviBlessingIds.Paperstorm",
            "MorviBlessingIds.BlueprintProof",
            "MorviBlessingIds.DebtSettlement",
            "MorviAssetPaths.MapIcon",
            "MorviAssetPaths.BackgroundScene");
        Assert.DoesNotContain("Glory.GetUnlockedAncients", morviSource, StringComparison.Ordinal);

        AssertSourceContains(
            morviBlessings,
            "morvi_forbidden_loan",
            "morvi_misprint_press",
            "morvi_red_ink_overdraft",
            "morvi_overdue_library",
            "morvi_open_book_exam",
            "morvi_paperstorm",
            "morvi_blueprint_proof",
            "morvi_debt_settlement");
        AssertSourceContains(
            morviSource,
            "public override bool ShouldReceiveCombatHooks => true",
            "BeforeCombatStart",
            "player.IsActiveForHooks",
            "CardType.Attack or CardType.Skill",
            "!card.IsClone",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "CardCmd.Downgrade(card)",
            "ForbiddenLoanKeepGoldCost = 180",
            "RedInkOverdraftGoldPerDebt = 12",
            "OverdueLibraryPageCount = 3",
            "OpenBookDraw = 5",
            "PaperstormWastePaperCount = 4",
            "BlueprintProofStacks = 3",
            "DebtSettlementImmediateGold = 220",
            "DebtSettlementStartingDebt = 320",
            "DebtSettlementCombatDue = 40",
            "maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0",
            "FindOpenBookSealedCards(player, combatState)",
            "DebtRemaining = Math.Max(0, progress.DebtRemaining - due)");
        Assert.DoesNotContain("CreateClone", morviSource, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", morviSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", morviSource, StringComparison.Ordinal);

        AssertSourceContains(
            morviCards,
            "MorviArchiveDrawPage",
            "MorviArchiveVeilPage",
            "MorviArchiveBurnPage",
            "MorviArchiveDiscountPage",
            "MorviArchiveBraveryPage",
            "MorviArchiveDexterityPage",
            "MorviRedInkOverdraftCard",
            "MorviWastePaper");
        AssertSourceContains(
            morviOptionRelics,
            "MorviForbiddenLoanOptionRelic",
            "MorviDebtSettlementOptionRelic",
            "IsAllowed(IRunState runState) => false");
        AssertSourceContains(
            morviPowers,
            "MorviDebtPower",
            "MorviProofreadPower",
            "MorviOpenBookPower",
            "MorviOverdraftPower",
            "MorviPaperstormPower");

        foreach (var key in new[]
        {
            "EZMB_MORVI.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_forbidden_loan.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_paperstorm.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.title",
            "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.title"
        })
        {
            Assert.True(engAncients.ContainsKey(key), $"Missing English Morvi localization: {key}");
            Assert.True(zhsAncients.ContainsKey(key), $"Missing zhs Morvi localization: {key}");
        }

        foreach (var key in new[]
        {
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title",
            "EZMB_MORVI_RED_INK_OVERDRAFT.title",
            "EZMB_MORVI_WASTE_PAPER.title"
        })
        {
            Assert.True(engCards.ContainsKey(key), $"Missing English Morvi card localization: {key}");
            Assert.True(zhsCards.ContainsKey(key), $"Missing zhs Morvi card localization: {key}");
        }
    }
    [Fact]
    public void LothaIsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var lothaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureGate.cs");
        var lothaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var lothaRunHook = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var lothaOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaOptionRelics.cs");
        var lothaPower = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var lothaScene = ReadRepoText("EZMicroBalance", "scenes", "events", "background_scenes", "ezmb_lotha.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        Assert.Contains("LothaInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(savedFields, "SavedSpireField<Player, string> LothaStateKey", "SavedSpireField<CardModel, string> LothaDeckStateKey", "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard");
        AssertSourceContains(
            lothaGate,
            "DisableEnvironmentVariable = \"SPIREPLUS_DISABLE_LOTHA\"",
            "LegacyDisableEnvironmentVariable = \"EZMB_DISABLE_LOTHA\"",
            "ForceAncientEnvironmentVariable = \"SPIREPLUS_FORCE_ANCIENT\"",
            "LegacyForceAncientEnvironmentVariable = \"EZMB_FORCE_ANCIENT\"",
            "ForceBlessingEnvironmentVariable = \"SPIREPLUS_FORCE_LOTHA_BLESSING\"",
            "LegacyForceBlessingEnvironmentVariable = \"EZMB_FORCE_LOTHA_BLESSING\"",
            "ShouldForceLotha",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(DisableEnvironmentVariable)",
            "AncientFeatureGate.IsTruthyEnvironmentVariable(LegacyDisableEnvironmentVariable)");
        AssertSourceContains(lothaSource, "CustomAncientModel", "HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))", "LothaFeatureGate.ShouldForceLotha", "ExpectedInitialOptionCount = 3", "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()", "AncientInitialOptionReroll.CanOffer", "OptionWithRelic<LothaMirrorRebuttalOptionRelic>", "OptionWithRelic<LothaPublicEvidenceOptionRelic>", "CardSelectCmd.FromDeckGeneric", "LothaBlessingService.MarkMirrorRebuttalCard", "HoverTipFactory.FromPower<LothaPresumptionPower>()", "HoverTipFactory.FromPower<LothaVerdictPower>()", "HoverTipFactory.FromPower<LothaDeathReprievePower>()", "HoverTipFactory.FromPower<LothaEnlightenmentPower>()", "HoverTipFactory.Static(StaticHoverTip.Energy)", "HoverTipFactory.Static(StaticHoverTip.Block)", "LothaAssetPaths.MapIcon", "LothaAssetPaths.RunHistoryIcon", "LothaAssetPaths.BackgroundScene");
        AssertSourceContains(lothaRunHook, "ShouldReceiveCombatHooks => true", "public override int ModifyCardPlayCount", "public override bool ShouldPlay", "public override Task AfterSideTurnEnd", "public override Task AfterDamageReceived", "public override bool TryModifyRewardsLate", "public override bool TryModifyEnergyCostInCombat", "public override bool TryModifyStarCost", "public override Task AfterCombatEnd", "ModifyPowerAmountGiven", "TryModifyPowerAmountReceived", "AfterPowerAmountChanged", "LothaExtraPlayCount = 2", "SingleSentenceRemainingPlayLimit = 4", "MirrorRebuttalExtraPlayCount = 1", "MirrorHallEchoExtraPlayCount = 1", "ClosedCourtFirstTurnCards = 4", "ClosedCourtFirstTurnEnergy = 2", "ClosedCourtSecondPulseTurn = 4", "ClosedCourtSecondPulseCards = 2", "ClosedCourtSecondPulseEnergy = 2", "PresumptionCards = 2", "PresumptionEnergy = 1", "PresumptionBlock = 8", "PresumptionHpLoss = 8", "DeferredVerdictTurn = 4", "DeferredVerdictStacks = 3", "DeferredVerdictEnergy = 4", "DeferredVerdictCards = 4", "DeferredVerdictExtraPlayCount = 1", "DeferredVerdictEarlyEndHeal = 4", "DeathReprieveCards = 10", "DeathReprieveEnergy = 10", "PowerFallbackCards = 1", "IsPowerReplacementCostZeroCard", "PowerReplacementCardPendingBenefit", "cost 0 and draw 1", "CardType.Attack or CardType.Skill", "!card.IsClone", "cardPlay.IsAutoPlay", "card.Type == CardType.Power && !card.IsClone", "ApplyPowerReplacementBenefit", "RecordMirrorHallEchoType", "PowerCmd.Apply<LothaPresumptionPower>", "PowerCmd.Apply<LothaVerdictPower>", "PowerCmd.Apply<LothaEnlightenmentPower>", "PowerCmd.Decrement(verdict)", "PowerCmd.ModifyAmount(choiceContext, enlightenment, -consumed", "CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal", "CreatureCmd.Damage(", "rewards.RemoveAll(reward => reward is CardReward)", "IsPublicEvidenceDebuffApplication", "IsPublicEvidenceExcludedDamageDebuff", "power is PoisonPower", "power.GetTypeForAmount(amount) == PowerType.Debuff", "ShouldDieLate(Creature creature)", "ShouldDie(Creature creature)", "AfterPreventingDeath(Creature creature)", "CreatureCmd.Kill(player.Creature, force: true)", "AncientSavedStateFields.LothaStateKey", "AncientSavedStateFields.LothaDeckStateKey", "AncientSavedStateFields.LothaMirrorRebuttalCard");
        Assert.DoesNotContain("MirrorRebuttalMinimumBlock", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryReplayMirrorRebuttalCopy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryCreateMirrorHallEcho", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeathReprieveHealPercent", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryBurstDeferredVerdict", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeferredVerdictDamagePerStack", lothaRunHook, StringComparison.Ordinal);
        Assert.Equal(8, Regex.Matches(lothaOptionRelics, @"\[Pool\(typeof\(SharedRelicPool\)\)\]").Count);
        AssertSourceContains(lothaPower, "internal sealed class LothaVerdictPower", "internal sealed class LothaPresumptionPower", "internal sealed class LothaDeathReprievePower", "internal sealed class LothaSingleSentencePower", "internal sealed class LothaEnlightenmentPower", "PowerType.Buff", "PowerStackType.Counter", "PowerStackType.Single", "LothaAssetPaths.VerdictPowerIcon", "LothaAssetPaths.PresumptionPowerIcon", "LothaAssetPaths.DeathReprievePowerIcon", "LothaAssetPaths.SingleSentencePowerIcon", "LothaAssetPaths.EnlightenmentPowerIcon");
        AssertSourceContains(lothaScene, "[node name=\"EzmbLothaBackground\" type=\"Control\"]", "type=\"TextureRect\"", "ezmb_lotha.png");

        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_LOTHA.title",
                "EZMICROBALANCE-EZMB_LOTHA.epithet",
                "EZMICROBALANCE-EZMB_LOTHA.talk.firstVisitEver.0-0.ancient",
                "EZMICROBALANCE-EZMB_LOTHA.talk.ANY.0-0r.ancient",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.title",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description",
                "EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"
            ],
            engAncients,
            zhsAncients,
            "Lotha Ancient localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.title",
                "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.title"
            ],
            engRelics,
            zhsRelics,
            "Lotha option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.title",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.description",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.title",
                "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description",
                "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription"
            ],
            engPowers,
            zhsPowers,
            "Lotha power localization");

        Assert.Contains("[gold]Attack[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u653b\u51fb\u724c[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]4[/blue]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u80fd\u529b\u724c[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.Contains("stacks apply twice", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("non-damaging [gold]negative status[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Enlightenment[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("\u5c42\u6570\u7ffb\u500d", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("\u975e\u4f24\u5bb3\u7c7b[gold]\u8d1f\u9762\u72b6\u6001[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u5f00\u609f[/gold]", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Verdict[/gold]", engPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u88c1\u51b3[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Innocent[/gold]", engPowers["EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u65e0\u7f6a[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Death Reprieve[/gold]", engPowers["EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u6b7b\u5211\u7f13\u671f[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Single Sentence[/gold]", engPowers["EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u5355\u724c\u5ba3\u5224[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Enlightenment[/gold]", engPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]\u5f00\u609f[/gold]", zhsPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]0[/blue]", engRelics["EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]0[/blue]", zhsRelics["EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"], StringComparison.Ordinal);
        foreach (var relativePath in new[]
        {
            "EZMicroBalance/images/events/ezmb_lotha.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_map_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon.png",
            "EZMicroBalance/images/ancients/lotha/ezmb_lotha_run_history_icon_outline.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_rebuttal.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_mirror_hall_echo.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_presumption.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_closed_court.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_deferred_verdict.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_death_reprieve.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_single_sentence.png",
            "EZMicroBalance/images/ancients/lotha/options/lotha_public_evidence.png",
            "EZMicroBalance/images/powers/lotha_verdict.png",
            "EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn"
        })
        {
            AssertRepoFileExists(relativePath.Split('/'));
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void VakuuFightIsSinglePlayerGatedLocalizedAndResumeSafe()
    {
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");
        var command = ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusAncientLiveTestConsoleCmd.cs");
        var vakuuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var victory = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var noReward = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.NoRewardResume.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var monster = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTrialMonster.cs");
        var optionRelic = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightOptionRelic.cs");
        var assetPaths = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightAssetPaths.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engEncounters = JsonStringMap("EZMicroBalance", "localization", "eng", "encounters.json");
        var zhsEncounters = JsonStringMap("EZMicroBalance", "localization", "zhs", "encounters.json");
        var engMonsters = JsonStringMap("EZMicroBalance", "localization", "eng", "monsters.json");
        var zhsMonsters = JsonStringMap("EZMicroBalance", "localization", "zhs", "monsters.json");

        AssertSourceContains(
            gate,
            "EZMB_ENABLE_VAKUU_FIGHT",
            "SPIREPLUS_ENABLE_VAKUU_FIGHT",
            "EZMB_DISABLE_VAKUU_FIGHT",
            "SPIREPLUS_DISABLE_VAKUU_FIGHT",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_VAKUU_FIGHT",
            "SPIREPLUS_FORCE_VAKUU_FIGHT",
            "ShouldForceVakuu",
            "ShouldForceFight",
            "ShouldForceFightForRun",
            "ArmCommandForceFight",
            "ConsumeCommandForceFightForRun",
            "HasCommandForceFightForRun",
            "ClearCommandForceFightWhenBeginEventCompletes",
            "finally",
            "ShouldEnableFight",
            "runState.Players.Count == 1");
        AssertSourceContains(
            patch,
            "[HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))]",
            "ModelDb.AncientEvent<MegaCrit.Sts2.Core.Models.Events.Vakuu>()",
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Models.Events.Vakuu), \"GenerateInitialOptions\")]",
            "VakuuFightFeatureGate.ShouldForceFightForRun(runState)",
            "VakuuFightFeatureGate.ConsumeCommandForceFightForRun(runState)",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.BeginEvent))]",
            "VakuuFightFeatureGate.HasCommandForceFightForRun(runState)",
            "VakuuFightFeatureGate.ClearCommandForceFightWhenBeginEventCompletes(__result, runState)",
            "[HarmonyPatch(typeof(EventModel), nameof(EventModel.Resume))]",
            "[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OfferRoomEndRewards))]");
        var vakuuCommandSource = string.Join(Environment.NewLine, command, vakuuSource);
        Assert.False(
            Regex.IsMatch(
                vakuuCommandSource,
                @"\b(?:System\s*\.\s*)?Environment\s*\.\s*SetEnvironmentVariable\s*\(",
                RegexOptions.CultureInvariant),
            "Vakuu command force fight must not mutate process environment variables.");
        Assert.DoesNotContain("ForceVakuuFightEnvironmentForCommand", vakuuCommandSource, StringComparison.Ordinal);
        AssertSourceContains(
            vakuuSource,
            "EventOption.FromRelic",
            "SetEventState",
            "EventNodeBackingField",
            "ClearEventNode(vakuu)",
            "EnterRoomWithoutExitingCurrentRoom(combatRoom, fadeToBlack: true)",
            "ModelDb.Encounter<EzmbVakuuTrialEncounter>()");
        AssertSourceContains(
            vakuuSource,
            "EnsureStolenVaultPower",
            "PowerCmd.ModifyAmount",
            "SignContract",
            "BreakLock(choiceContext, combatState, \"contract\")");
        AssertSourceContains(
            vakuuSource,
            "internal sealed class VakuuStolenVaultPower",
            "internal sealed class VakuuBloodDebtPower",
            "public override PowerStackType StackType => PowerStackType.Counter",
            "public override int DisplayAmount => Amount");
        Assert.True(
            Regex.Matches(vakuuSource, "public override int DisplayAmount => Amount").Count >= 2,
            "Vakuu fight amount-bearing powers should show their live counter values.");
        AssertSourceContains(
            noReward,
            "ProceedFromNoRewardVictory",
            "ProceedFromMissingParentStackNoRewardVictory",
            "RunManager.Instance.ProceedFromTerminalRewardsScreen()");
        AssertSourceContains(
            victory,
            "targetChoiceCount = encounter.VictoryChoiceCount",
            "choice.Relic.Owner = owner",
            "encounter.VictoryGold",
            "Nonupeipe",
            "Tanx",
            "GetLothaAct3AncientRelicChoices",
            "LothaFeatureGate.IsLothaEnabled",
            "LothaBlessingService.GetSelectedBlessing(owner)",
            "IsEligibleLothaVictoryChoice(owner, blessingId)",
            "LothaBlessingService.HasMirrorRebuttalCandidates(owner)",
            "LothaRewardSelectionService.SelectBlessing<T>",
            "LothaMirrorRebuttalOptionRelic",
            "LothaPublicEvidenceOptionRelic",
            "RelicCmd.Obtain(mutableRelic, owner)",
            "vakuu.StartPreFinished()");
        Assert.DoesNotContain("LinkedRewardSet", patch + victory, StringComparison.Ordinal);
        Assert.DoesNotContain("ExtraRewards", patch + victory, StringComparison.Ordinal);
        AssertSourceContains(
            encounter,
            "CustomEncounterModel",
            "base(RoomType.Monster, autoAdd: false)",
            "CustomScenePath => VakuuFightAssetPaths.EncounterScene",
            "HasScene => true",
            "ShouldGiveRewards => false",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()",
            "IsValidForAct(ActModel act) => false");
        AssertSourceContains(
            monster,
            "CustomMonsterModel",
            "public const string MonsterId = \"EZMB_VAKUU_TRIAL_MONSTER\"",
            "CustomVisualPath => VakuuFightAssetPaths.MonsterVisual",
            "VisualScale = 1.25f",
            "public override async Task AfterAddedToRoom()",
            "VakuuFightService.EnsureStolenVaultPower(Creature)",
            "GenerateMoveStateMachine",
            "WeakPower",
            "VulnerablePower",
            "StrengthPower");
        AssertSourceContains(
            assetPaths,
            "OptionIcon => $\"{MainFile.ResPath}/images/ancients/vakuu/options/vakuu_fight.png\"",
            "MonsterVisual => $\"{MainFile.ResPath}/images/monsters/vakuu_trial.png\"");
        Assert.DoesNotContain("MonsterVisual => OptionIcon", assetPaths, StringComparison.Ordinal);
        AssertSourceContains(
            optionRelic,
            "[Pool(typeof(SharedRelicPool))]",
            "PackedIconPath => VakuuFightAssetPaths.OptionIcon",
            "IsAllowed(IRunState runState) => false",
            "IsAllowedAtNeow(Player player) => false",
            "IsAllowedInShops => false");

        AssertLocalizedKeys(
            [
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.title",
                "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description",
                "EZMB_VAKUU_FIGHT.pages.VICTORY.description",
                "EZMB_VAKUU_FIGHT.pages.DONE.description"
            ],
            engAncients,
            zhsAncients,
            "Vakuu fight Ancient localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.title",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description",
                "EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.flavor"
            ],
            engRelics,
            zhsRelics,
            "Vakuu fight option relic localization");
        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.title",
                "EZMICROBALANCE-EZMB_VAKUU_TRIAL_ENCOUNTER.loss"
            ],
            engEncounters,
            zhsEncounters,
            "Vakuu fight encounter localization");
        AssertLocalizedKeys(
            [
                "EZMB_VAKUU_TRIAL_MONSTER.name",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.OPENING_OFFER.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.KNIFE_RAIN.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.GILDED_HIDE.title",
                "EZMB_VAKUU_TRIAL_MONSTER.moves.DEBT_CALL.title"
            ],
            engMonsters,
            zhsMonsters,
            "Vakuu monster localization");

        AssertRepoFileExists("EZMicroBalance", "images", "ancients", "vakuu", "options", "vakuu_fight.png");
        AssertRepoFileExists("EZMicroBalance", "images", "monsters", "vakuu_trial.png");
        AssertRepoFileExists("EZMicroBalance", "images", "encounters", "vakuu_trial_backdrop.png");
        AssertRepoFileExists("EZMicroBalance", "scenes", "encounters", "ezmb_vakuu_trial.tscn");
        Assert.Contains("res://EZMicroBalance/images/ancients/vakuu/options/vakuu_fight.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/monsters/vakuu_trial.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/encounters/vakuu_trial_backdrop.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/encounters/ezmb_vakuu_trial.tscn", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/encounters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/encounters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/eng/monsters.json", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/localization/zhs/monsters.json", exportPreset, StringComparison.Ordinal);
    }
}
