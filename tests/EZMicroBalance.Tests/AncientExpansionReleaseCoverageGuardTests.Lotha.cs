using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientExpansionReleaseCoverageGuardTests
{
    [Fact]
    public void LothaIsDefaultOnGatedLocalizedAndPowerSafe()
    {
        var featureRegistry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var lothaGate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureGate.cs");
        var lothaSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var lothaRunHook = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha");
        var lothaOptionRelics = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaOptionRelics.cs");
        var lothaPower = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var ritsuRegistration = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "SpirePlusContentRegistrationService.cs");
        var lothaScene = ReadRepoText("EZMicroBalance", "scenes", "events", "background_scenes", "ezmb_lotha.tscn");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        Assert.Contains("LothaFeatureModule", featureRegistry, StringComparison.Ordinal);
        AssertSourceContains(savedFields, "SavedAttachedState<Player, string> LothaStateKey", "SavedAttachedState<CardModel, string> LothaDeckStateKey", "SavedAttachedState<CardModel, bool> LothaMirrorRebuttalCard");
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
        AssertSourceContains(lothaSource, "ModAncientEventTemplate", "HarmonyPatch(typeof(Glory), nameof(Glory.GetUnlockedAncients))", "LothaFeatureGate.ShouldForceLotha", "ExpectedInitialOptionCount = 3", "candidates.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()", "AncientInitialOptionReroll.CanOffer", "OptionWithRelic<LothaMirrorRebuttalOptionRelic>", "OptionWithRelic<LothaPublicEvidenceOptionRelic>", "CardSelectCmd.FromDeckGeneric", "LothaBlessingService.MarkMirrorRebuttalCard", "HoverTipFactory.FromPower<LothaPresumptionPower>()", "HoverTipFactory.FromPower<LothaVerdictPower>()", "HoverTipFactory.FromPower<LothaDeathReprievePower>()", "HoverTipFactory.FromPower<LothaEnlightenmentPower>()", "HoverTipFactory.Static(StaticHoverTip.Energy)", "HoverTipFactory.Static(StaticHoverTip.Block)", "LothaAssetPaths.MapIcon", "LothaAssetPaths.RunHistoryIcon", "LothaAssetPaths.BackgroundScene");
        AssertSourceContains(lothaRunHook, "ShouldReceiveCombatHooks => true", "public override int ModifyCardPlayCount", "public override bool ShouldPlay", "public override Task AfterSideTurnEnd", "public override Task AfterDamageReceived", "public override bool TryModifyRewardsLate", "public override bool TryModifyEnergyCostInCombat", "public override bool TryModifyStarCost", "public override Task AfterCombatEnd", "ModifyPowerAmountGivenAdditive", "TryModifyPowerAmountReceived", "AfterPowerAmountChanged", "LothaExtraPlayCount = 2", "SingleSentenceRemainingPlayLimit = 4", "MirrorRebuttalExtraPlayCount = 1", "MirrorHallEchoExtraPlayCount = 1", "ClosedCourtFirstTurnCards = 4", "ClosedCourtFirstTurnEnergy = 2", "ClosedCourtSecondPulseTurn = 4", "ClosedCourtSecondPulseCards = 2", "ClosedCourtSecondPulseEnergy = 2", "PresumptionCards = 2", "PresumptionEnergy = 1", "PresumptionBlock = 8", "PresumptionHpLoss = 8", "DeferredVerdictTurn = 4", "DeferredVerdictStacks = 3", "DeferredVerdictEnergy = 4", "DeferredVerdictCards = 4", "DeferredVerdictExtraPlayCount = 1", "DeferredVerdictEarlyEndHeal = 4", "DeathReprieveCards = 10", "DeathReprieveEnergy = 10", "PowerFallbackCards = 1", "IsPowerReplacementCostZeroCard", "PowerReplacementCardPendingBenefit", "cost 0 and draw 1", "CardType.Attack or CardType.Skill", "!card.IsClone", "cardPlay.IsAutoPlay", "card.Type == CardType.Power && !card.IsClone", "ApplyPowerReplacementBenefit", "RecordMirrorHallEchoType", "PowerCmd.Apply<LothaPresumptionPower>", "PowerCmd.Apply<LothaVerdictPower>", "PowerCmd.Apply<LothaEnlightenmentPower>", "PowerCmd.Decrement(verdict)", "PowerCmd.ModifyAmount(choiceContext, enlightenment, -consumed", "CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal", "CreatureCmd.Damage(", "rewards.RemoveAll(reward => reward is CardReward)", "IsPublicEvidenceDebuffApplication", "IsPublicEvidenceExcludedDamageDebuff", "power is PoisonPower", "power.GetTypeForAmount(amount) == PowerType.Debuff", "ShouldDieLate(Creature creature)", "ShouldDie(Creature creature)", "AfterPreventingDeath(Creature creature)", "CreatureCmd.Kill(player.Creature, force: true)", "AncientSavedStateFields.LothaStateKey", "AncientSavedStateFields.LothaDeckStateKey", "AncientSavedStateFields.LothaMirrorRebuttalCard");
        Assert.DoesNotContain("MirrorRebuttalMinimumBlock", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryReplayMirrorRebuttalCopy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryCreateMirrorHallEcho", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeathReprieveHealPercent", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryBurstDeferredVerdict", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeferredVerdictDamagePerStack", lothaRunHook, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(SharedRelicPool))]", lothaOptionRelics, StringComparison.Ordinal);
        Assert.Equal(8, Regex.Matches(ritsuRegistration, @"content\.Relic<SharedRelicPool, Lotha[A-Za-z]+OptionRelic>\(\);").Count);
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
}
