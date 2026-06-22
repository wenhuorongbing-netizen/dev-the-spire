using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class BossDedicatedAbilityV41GuardTests
{
    [Fact]
    public void A19A20PlayerFacingTextUsesDedicatedAbilitiesAndBrandedForm()
    {
        var english = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");
        var visibleText = string.Join(Environment.NewLine, english.Values.Concat(simplifiedChinese.Values));

        Assert.Equal("Boss Dedicated Abilities", english["LEVEL_19.title"]);
        Assert.Equal("Branded Form", english["LEVEL_20.title"]);
        Assert.Contains("Each Boss gains its own dedicated ability", english["LEVEL_19.description"], StringComparison.Ordinal);
        Assert.Contains("Only the second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]", english["LEVEL_20.description"], StringComparison.Ordinal);
        Assert.Contains("\u6240\u6709\u9996\u9886\u83b7\u5f97\u4e13\u5c5e\u7279\u6b8a\u80fd\u529b", simplifiedChinese["LEVEL_19.description"], StringComparison.Ordinal);
        Assert.Contains("\u53ea\u6709\u7b2c[blue]3[/blue]\u5e55\u7b2c\u4e8c\u540d\u9996\u9886\u8fdb\u5165[gold]\u70d9\u5370\u5f62\u6001[/gold]", simplifiedChinese["LEVEL_20.description"], StringComparison.Ordinal);
        Assert.Equal("Branded Form", english["BOSS_BRANDED_FORM.title"]);
        Assert.Equal("\u70d9\u5370\u5f62\u6001", simplifiedChinese["BOSS_BRANDED_FORM.title"]);
        Assert.Contains("second Act [blue]3[/blue] Boss", english["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
        Assert.Contains("\u7b2c[blue]3[/blue]\u5e55\u7b2c\u4e8c\u540d\u9996\u9886", simplifiedChinese["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
        Assert.Contains("Attack changes from this ability are shown in intent.", english["BOSS_DEDICATED_ABILITY.description"], StringComparison.Ordinal);
        Assert.Equal(english["BOSS_DEDICATED_ABILITY.title"], english["BOSS_ROYAL_SEAL.title"]);
        Assert.Equal(simplifiedChinese["BOSS_DEDICATED_ABILITY.title"], simplifiedChinese["BOSS_ROYAL_SEAL.title"]);
        Assert.Equal("Experimental Record: {Samples}. {Reason}", english["BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE"]);
        Assert.Equal("\u5b9e\u9a8c\u8bb0\u5f55\uff1a{Samples}\u3002{Reason}", simplifiedChinese["BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE"]);
        Assert.Equal("Strength Residue", english["BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH"]);
        Assert.Equal("\u529b\u91cf\u6b8b\u7559", simplifiedChinese["BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH"]);
        Assert.Contains("Skills", english["BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason"], StringComparison.Ordinal);
        Assert.Contains("\u6280\u80fd\u724c", simplifiedChinese["BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason"], StringComparison.Ordinal);

        foreach (var staleTerm in new[] { "Royal Seal", "King Brand", "\u738b\u5370", "\u738b\u70d9\u5370", "\u9965\u566c", "\u7532\u58f3", "\u68a6\u58f3", "\u62a4\u58f3", "8/12/16", "12/16/20" })
        {
            Assert.DoesNotContain(staleTerm, visibleText, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void BossCatalogMapsEveryV41AbilityToSpecificEncounter()
    {
        var catalog = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");

        AssertSourceContains(
            catalog,
            "EncounterId(\"THE_KIN_BOSS\")",
            "BossSealId.MartyrOath",
            "each hit of its next attack deals 3 extra damage per Oath",
            "source-confirmed two KinFollower deaths",
            "EncounterId(\"VANTOM_BOSS\")",
            "BossSealId.InkReturn",
            "25% of the cleared Slippery",
            "Restores 35% of the cleared Slippery",
            "EncounterId(\"LAGAVULIN_MATRIARCH_BOSS\")",
            "BossSealId.StartledShell",
            "Multiplayer uses the game's boss Plating scaling",
            "EncounterId(\"SOUL_FYSH_BOSS\")",
            "BossSealId.SoulTide",
            "capped Block",
            "EncounterId(\"WATERFALL_GIANT_BOSS\")",
            "BossSealId.BoilingCritical",
            "affected players gain 1 Vulnerable",
            "EncounterId(\"KAISER_CRAB_BOSS\")",
            "BossSealId.MisalignedShell",
            "35%",
            "each hit of its next attack deals 4 extra damage",
            "EncounterId(\"KNOWLEDGE_DEMON_BOSS\")",
            "BossSealId.MarginalNote",
            "side costs",
            "deliberately does not depend on exact unchosen curse identity",
            "EncounterId(\"THE_INSATIABLE_BOSS\")",
            "BossSealId.StruggleBait",
            "source VigorPower",
            "EncounterId(\"AEONGLASS_BOSS\")",
            "BossSealId.AeonglassHourglass",
            "EBB_MOVE",
            "EYE_LASERS_MOVE",
            "INCREASING_INTENSITY_MOVE",
            "EncounterId(\"QUEEN_BOSS\")",
            "BossSealId.ChosenDecree",
            "Royal Decree",
            "EncounterId(\"TEST_SUBJECT_BOSS\")",
            "BossSealId.ResidualSample",
            "2 different Residual Samples");

        Assert.DoesNotContain("Royal Seal", catalog, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand", catalog, StringComparison.Ordinal);
        Assert.DoesNotContain("unchosen-curse identity remains unhooked", catalog, StringComparison.Ordinal);
    }

    [Fact]
    public void DamageChangingDedicatedAbilitiesParticipateInIntentPreview()
    {
        var martyrOathPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MartyrOathPowers.cs");
        var misalignedShellPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MisalignedShellPowers.cs");
        var aeonglassIntentPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AeonglassIntentPatches.cs");
        var heatPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "FiremarkHeatPowers.cs");
        var pressingLinePower = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BannerPressingLinePower.cs");
        var intentRefreshHelper = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Commands.cs");
        var martyr = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MartyrOath.cs");
        var kaiser = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MisalignedShell.cs");
        var mightFiremark = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Firemarks.Might.cs");
        var pressingLine = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Banners.PressingLine.cs");
        var aeonglassReflow = ReadAeonglassHourglassCombatSources();

        var martyrStrike = SliceFrom(
            martyrOathPowers,
            "internal sealed class MartyrOathStrikePower");
        var martyrOath = SliceBetween(
            martyrOathPowers,
            "internal sealed class MartyrOathPower",
            "internal sealed class MartyrOathStrikePower");
        AssertSourceContains(
            martyrOath,
            "private bool _debuffConsumed",
            "AfterModifyingPowerAmountGiven",
            "_debuffConsumed = true",
            "AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)",
            "participants.Contains(Owner)",
            "await PowerCmd.Remove(this)");
        Assert.DoesNotContain("AfterModifyingPowerAmountGiven(PowerModel power)\r\n    {\r\n        await PowerCmd.Remove(Owner.GetPower<MartyrOathStrikePower>());\r\n        await PowerCmd.Remove(this);", martyrOath, StringComparison.Ordinal);
        AssertSourceContains(
            martyrStrike,
            "ModifyDamageAdditive",
            "dealer == Owner && props.IsPoweredAttack()",
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)",
            "command.Attacker == Owner && command.DamageProps.IsPoweredAttack()");
        Assert.DoesNotContain("AfterDamageGiven", martyrStrike, StringComparison.Ordinal);

        var kaiserStrike = SliceFrom(
            misalignedShellPowers,
            "internal sealed class KaiserCalibrationStrikePower");
        AssertSourceContains(
            kaiserStrike,
            "ModifyDamageAdditive",
            "dealer == Owner && props.IsPoweredAttack()",
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)",
            "command.Attacker == Owner && command.DamageProps.IsPoweredAttack()");
        Assert.DoesNotContain("AfterDamageGiven", kaiserStrike, StringComparison.Ordinal);

        AssertSourceContains(
            aeonglassIntentPatch,
            "IPatchMethod.PatchId => \"aeonglass-laser-echo-intent-label\"",
            "new ModPatchTarget(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))",
            "__instance.Repeats + 1",
            "IPatchMethod.PatchId => \"aeonglass-laser-echo-intent-damage\"",
            "new ModPatchTarget(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))",
            "GetSingleDamage(targets, owner) * (__instance.Repeats + 1)");
        AssertSourceContains(
            heatPowers,
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)",
            "command.Attacker == Owner && command.DamageProps.IsPoweredAttack()");
        AssertSourceContains(
            pressingLinePower,
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)",
            "command.Attacker == Owner && command.DamageProps.IsPoweredAttack()");
        AssertSourceContains(
            intentRefreshHelper,
            "RefreshEnemyIntent(Creature? creature)",
            "creature.GetCreatureNode()",
            "await node.UpdateIntent(combatState.Allies)");
        AssertSourceContains(martyr, "await RefreshEnemyIntent(priest);");
        AssertSourceContains(kaiser, "await RefreshEnemyIntent(higherHpClaw);");
        AssertSourceContains(mightFiremark, "await RefreshEnemyIntent(host);");
        AssertSourceContains(pressingLine, "await RefreshEnemyIntent(enemy);");
        AssertSourceContains(
            aeonglassReflow,
            "await RefreshEnemyIntent(aeonglass);",
            "PowerCmd.Remove(aeonglass.GetPower<AeonglassLaserEchoPower>())");
    }

    [LocalSourceFact]
    public void CoreDamageIntentAndVigorPowerStillSupportPreviewHooks()
    {
        var attackIntent = ReadLocalCoreText("MonsterMoves", "Intents", "AttackIntent.cs");
        var multiAttackIntent = ReadLocalCoreText("MonsterMoves", "Intents", "MultiAttackIntent.cs");
        var vigorPower = ReadLocalCoreText("Models", "Powers", "VigorPower.cs");

        AssertSourceContains(
            attackIntent,
            "Hook.ModifyDamage(",
            "ValueProp.Move",
            "ModifyDamageHookType.All",
            "CardPreviewMode.None");
        AssertSourceContains(multiAttackIntent, "return GetSingleDamage(targets, owner) * Repeats;");
        AssertSourceContains(
            vigorPower,
            "ModifyDamageAdditive",
            "base.Owner != dealer",
            "!props.IsPoweredAttack()",
            "return base.Amount;");
    }

    [Fact]
    public void VisibleBossAbilityStateCanRehydrateItsRuntimeTrackerWhereSourceSafe()
    {
        var aeonglass = ReadAeonglassHourglassCombatSources();
        var decree = ReadChosenDecreeCombatSources();
        var struggleBait = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.StruggleBait.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionSavedStateFields.cs");
        var aeonglassRuntimePowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "AeonglassHourglassRuntimePowers.cs");

        AssertSourceContains(
            aeonglass,
            "HydrateAeonglassTimeSandFromVisiblePower",
            "GetPower<AeonglassHourglassPower>()?.Amount",
            "tracker.AeonglassTimeSand = visibleTimeSand",
            "recovered Time Sand tracker from visible power",
            "GetPower<AeonglassPendingWitherPower>()?.Amount",
            "tracker.AeonglassExtraWitherFromSands = pendingWither",
            "GetPower<AeonglassLaserEchoUseCounterPower>()?.Amount",
            "tracker.AeonglassLaserEchoesUsed = usedEchoes");
        AssertSourceContains(
            decree,
            "HydrateChosenDecreeFromVisibleCards",
            "card.Enchantment is RoyalDecreeEnchantment",
            "AscensionSavedStateFields.RoyalDecreeMarkedCard[card]",
            "AscensionSavedStateFields.RoyalDecreePlayedBoundCard[card]",
            "AscensionSavedStateFields.RoyalDecreePlayedCard[card]",
            "tracker.ChosenDecreeCardsByPlayer[player] = markedDecree",
            "recovered Royal Decree tracker from visible card marker");
        AssertSourceContains(
            savedFields,
            "SavedAttachedState<CardModel, bool> StruggleBaitGeneratedEscape",
            "EZMicroBalanceAscensionStruggleBaitGeneratedEscape",
            "SavedAttachedState<CardModel, bool> RoyalDecreeMarkedCard",
            "SavedAttachedState<CardModel, bool> RoyalDecreePlayedCard",
            "SavedAttachedState<CardModel, bool> RoyalDecreePlayedBoundCard");
        AssertSourceContains(
            struggleBait,
            "AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape] = true",
            "AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape]",
            "AscensionSavedStateFields.StruggleBaitGeneratedEscape[escape] = false");
        AssertSourceContains(
            aeonglassRuntimePowers,
            "internal sealed class AeonglassPendingWitherPower",
            "internal sealed class AeonglassLaserEchoUseCounterPower",
            "protected override bool IsVisibleInternal => false");
    }

    [Fact]
    public void BossDedicatedAbilityPowerTextKeepsReadableSimplifiedChineseInlineLocalization()
    {
        var bossPowers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var aeonglassPower = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "AeonglassHourglassPower.cs");
        var residualSampleFlow = ReadResidualSampleCombatSources();
        var royalDecreeEnchantment = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "RoyalDecreeEnchantment.cs");
        var combinedPowerText = string.Join(Environment.NewLine, bossPowers, aeonglassPower, residualSampleFlow, royalDecreeEnchantment);

        AssertSourceContains(
            combinedPowerText,
            "\u4e13\u5c5e\u80fd\u529b\uff1a\u6b89\u8a93",
            "\u6bcf\u6b21\u547d\u4e2d\u989d\u5916\u9020\u6210",
            "\u4e13\u5c5e\u80fd\u529b\uff1a\u58a8\u8fd4",
            "\u5355\u4eba[blue]8[/blue]\u30012\u4eba[blue]12[/blue]\u30013-4\u4eba[blue]16[/blue]",
            "solo [blue]8[/blue], 2 players [blue]12[/blue], 3-4 players [blue]16[/blue]",
            "\u591a\u91cd\u62a4\u7532",
            "\u4eba\u5de5\u5236\u54c1",
            "\u70d9\u5370\u5f62\u6001",
            "\u4e13\u5c5e\u80fd\u529b\uff1a\u65f6\u7802\u56de\u6d41",
            "\u4e0b\u4e00\u6b21\u773c\u90e8\u6fc0\u5149\u989d\u5916\u547d\u4e2d",
            "\u61d2\u60f0\u548c\u67af\u7aed\u7684\u9644\u52a0\u4ee3\u4ef7\u6bcf\u6b21\u77e5\u8bc6\u8bc5\u5492\u6700\u591a\u7ed3\u7b97\u4e00\u6b21",
            "\u5fa1\u4ee4",
            "\u672c\u56de\u5408\u6253\u51fa\u8fd9\u5f20\u724c\uff0c\u53ef\u907f\u514d\u5fa1\u4ee4\u60e9\u7f5a",
            "Sloth and Waste Away side costs resolve at most once per Knowledge curse",
            "\"\\uFF1B\" : \"; \"");

        foreach (var fragment in new[] { "\uFFFD", "\u6d93", "\u9428", "\u951b", "\u95c5", "\u93c3" })
        {
            Assert.DoesNotContain(fragment, combinedPowerText, StringComparison.Ordinal);
        }

        Assert.DoesNotContain("A20\u70d9\u5370", combinedPowerText, StringComparison.Ordinal);
        Assert.DoesNotContain("8/12/16", combinedPowerText, StringComparison.Ordinal);
        Assert.DoesNotContain("12/16/20", combinedPowerText, StringComparison.Ordinal);
    }

    [Fact]
    public void BrandedFormIsOnlySecondActThreeBossMetadata()
    {
        var featureGate = ReadRepoText("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureGate.Systems.cs");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20Patches.cs");
        var bossMapMarkers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionMapService.Markers.BossSeals.cs");
        var activeModifiers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.ActiveModifiers.cs");
        var courtyard = ReadRepoText("EZMicroBalanceCode", "Ascension", "Events", "A20Courtyard.cs");
        var hoverPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionMapBossSealHoverPatches.cs");

        AssertSourceContains(
            featureGate,
            "IsBrandedFormSinglePlayerEnabled(IRunState runState)",
            "A20 Branded Form and second boss routing are pending two-client proof",
            "return runState.Players.Count == 1;");
        AssertSourceContains(
            a20Patch,
            "finalAct.HasSecondBoss",
            "finalAct.SetSecondBossEncounter(secondBoss)",
            "through the vanilla double-boss map path");
        AssertSourceContains(
            bossMapMarkers,
            "var bossMetadata = GetOrCreateMetadata(map.BossMapPoint);",
            "bossMetadata.IsBossBrand = false;",
            "if (map.SecondBossMapPoint == null)",
            "var secondBossMetadata = GetOrCreateMetadata(map.SecondBossMapPoint);",
            "secondBossMetadata.IsBossBrand = true;",
            "[\"boss\"] = \"second\"");
        AssertSourceContains(
            activeModifiers,
            "metadata.IsBossBrand",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(combatState.RunState)",
            "AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState)");
        AssertSourceContains(
            courtyard,
            "private const string BrandedFormKey = \"BOSS_BRANDED_FORM\"",
            "new LocString(\"ascension\", $\"{BrandedFormKey}.title\")",
            "new LocString(\"ascension\", $\"{BrandedFormKey}.description\")");
        AssertSourceContains(hoverPatch, "isBossBrand ? \"BOSS_BRANDED_FORM\" : \"BOSS_DEDICATED_ABILITY\"");
        Assert.DoesNotContain("BOSS_KING_BRAND", courtyard, StringComparison.Ordinal);
        Assert.DoesNotContain("BOSS_KING_BRAND", hoverPatch, StringComparison.Ordinal);
    }

    [Fact]
    public void AscensionManualChecklistContainsExecutableV41BossAbilityRows()
    {
        var checklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var a19A20Section = SliceBetween(checklist, "## A19/A20 Boss Systems", "## Disable and Uninstall");

        AssertSourceContains(
            checklist,
            "Last updated: 2026-05-23");

        AssertSourceContains(
            a19A20Section,
            "Attack-changing dedicated abilities show final enemy intent before damage resolves",
            "damage to each hit of the next attack",
            "Martyr Oath triggers only for The Kin",
            "Ink Return triggers only for Vantom",
            "Plating Wake triggers only for Lagavulin Matriarch",
            "Soul Tide triggers only for Soul Fysh",
            "Unweakenable triggers only for Waterfall Giant's explosion turn",
            "Claw Calibration triggers only for Kaiser Crab",
            "Marginal Note triggers only for Knowledge Demon",
            "Escape Fatigue triggers only for The Insatiable",
            "Time Sand Reflow triggers only for Aeonglass",
            "Royal Decree triggers only for Queen",
            "Experimental Record triggers only for Test Subject phase changes",
            "Branded Form uses a 30% threshold and 5 damage",
            "Branded Form creates 3 Time Sand",
            "No dedicated ability applies to the wrong boss");

        foreach (var staleTerm in new[] { "Royal Seal", "King Brand", "\u738b\u5370", "\u738b\u70d9\u5370", "generic seal" })
        {
            Assert.DoesNotContain(staleTerm, a19A20Section, StringComparison.OrdinalIgnoreCase);
        }
    }
}
