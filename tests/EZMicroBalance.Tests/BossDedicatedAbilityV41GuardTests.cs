using Xunit;

namespace EZMicroBalance.Tests;

public sealed class BossDedicatedAbilityV41GuardTests
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
    public void WebsiteA19A20ContentUsesV41DedicatedAbilityTerminology()
    {
        var websiteData = ReadRepoText("website", "content-data.js");
        var websiteEnglishAscension = ReadRepoText("website", "assets", "localization", "eng", "ascension.json");
        var websiteChineseAscension = ReadRepoText("website", "assets", "localization", "zhs", "ascension.json");
        var websiteReadme = ReadRepoText("website", "README.md");
        var combinedWebsiteText = string.Join(
            Environment.NewLine,
            websiteData,
            websiteEnglishAscension,
            websiteChineseAscension,
            websiteReadme);

        AssertSourceContains(
            websiteData,
            "Each Boss gets its own dedicated ability",
            "A20 Branded Form behavior remains development-test scope",
            "Dedicated Ability",
            "Branded Form",
            "Vanilla bosses do not have A19 dedicated abilities or A20 Branded Form.",
            "bossSeal(\"aeonglass_hourglass\"",
            "Time Sand Reflow",
            "Plating Wake",
            "Escape Fatigue");

        AssertSourceContains(
            websiteChineseAscension,
            "\u6240\u6709\u9996\u9886\u83b7\u5f97\u4e13\u5c5e\u7279\u6b8a\u80fd\u529b",
            "\u70d9\u5370\u5f62\u6001",
            "\u65f6\u7802\u56de\u6d41",
            "\u591a\u91cd\u62a4\u7532",
            "\u5b9e\u9a8c\u8bb0\u5f55",
            "\u529b\u91cf\u6b8b\u7559",
            "\u6d3b\u529b");

        AssertSourceContains(
            websiteEnglishAscension,
            "Boss Dedicated Abilities",
            "Only the second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]",
            "Time Sand Reflow",
            "Vigor",
            "Plating");

        AssertSourceContains(
            websiteReadme,
            "public-info site",
            "not a release-ready claim",
            "Do not copy original non-art source materials");

        var activeDocs = string.Join(
            Environment.NewLine,
            ReadRepoText("docs", "review.md"),
            ReadRepoText("docs", "toreview.md"));
        Assert.DoesNotContain("Royal Seal", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u5370", activeDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u70d9\u5370", activeDocs, StringComparison.Ordinal);
        AssertRepoPathDoesNotExist("website", "localization_qa.md");
        AssertRepoFileExists("docs", "archive", "implementation-records", "website-localization-qa-20260522.md");

        foreach (var staleTerm in new[]
                 {
                     "Royal Seal",
                     "King Brand",
                     "\u738b\u5370",
                     "\u738b\u70d9\u5370",
                     "\u997f\u566c",
                     "\u7532\u58f3",
                     "\u68a6\u58f3",
                     "\u62a4\u58f3",
                     "Every 5 cards played adds 1 Wither",
                     "Returns 2 Slippery",
                     "8/12/16",
                     "12/16/20"
                 })
        {
            Assert.DoesNotContain(staleTerm, combinedWebsiteText, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void BossCatalogMapsEveryV41AbilityToSpecificEncounter()
    {
        var catalog = ReadRepoText("EZMicroBalanceCode", "Ascension", "Rewards", "BossSealCatalog.Definitions.cs");

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
        var attackIntent = ReadRepoText("source code", "src", "Core", "MonsterMoves", "Intents", "AttackIntent.cs");
        var multiAttackIntent = ReadRepoText("source code", "src", "Core", "MonsterMoves", "Intents", "MultiAttackIntent.cs");
        var martyrOathPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MartyrOathPowers.cs");
        var misalignedShellPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MisalignedShellPowers.cs");
        var aeonglassIntentPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AeonglassIntentPatches.cs");
        var vigorPower = ReadRepoText("source code", "src", "Core", "Models", "Powers", "VigorPower.cs");
        var heatPowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "FiremarkHeatPowers.cs");
        var pressingLinePower = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BannerPressingLinePower.cs");
        var intentRefreshHelper = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Commands.cs");
        var martyr = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MartyrOath.cs");
        var kaiser = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MisalignedShell.cs");
        var mightFiremark = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Firemarks.Might.cs");
        var pressingLine = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.Banners.PressingLine.cs");
        var aeonglassReflow = ReadAeonglassHourglassCombatSources();

        AssertSourceContains(
            attackIntent,
            "Hook.ModifyDamage(",
            "ValueProp.Move",
            "ModifyDamageHookType.All",
            "CardPreviewMode.None");
        AssertSourceContains(multiAttackIntent, "return GetSingleDamage(targets, owner) * Repeats;");

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
            vigorPower,
            "ModifyDamageAdditive",
            "base.Owner != dealer",
            "!props.IsPoweredAttack()",
            "return base.Amount;");
        AssertSourceContains(
            aeonglassIntentPatch,
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))",
            "__instance.Repeats + 1",
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))",
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
            "SavedSpireField<CardModel, bool> StruggleBaitGeneratedEscape",
            "EZMicroBalanceAscensionStruggleBaitGeneratedEscape",
            "SavedSpireField<CardModel, bool> RoyalDecreeMarkedCard",
            "SavedSpireField<CardModel, bool> RoyalDecreePlayedCard",
            "SavedSpireField<CardModel, bool> RoyalDecreePlayedBoundCard");
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
    public void MultiplayerScalingRulesAreEncodedForV41BossAbilities()
    {
        var martyr = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MartyrOath.cs");
        var inkReturn = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.InkReturn.cs");
        var startledShell = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.StartledShell.cs");
        var soulTide = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.SoulTide.cs");
        var boiling = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.BoilingCritical.cs");
        var marginalNote = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MarginalNote.cs");
        var struggleBait = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.StruggleBait.cs");
        var chosenDecree = ReadChosenDecreeCombatSources();
        var turnFlow = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.TurnFlow.cs");
        var turnLifecycle = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.TurnLifecycle.cs");
        var cardEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.CardEvents.cs");
        var combatEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.CombatEvents.cs");
        var rootBudEvents = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "RootBudCombatHook.CombatEvents.cs");
        var aeonglass = ReadAeonglassHourglassCombatSources();
        var residualSample = ReadResidualSampleCombatSources();
        var marginalNotePowers = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MarginalNotePowers.cs");
        var tracker = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatTracker.BossSeals.cs");
        var chosenDecreeAssignStart = SliceBetween(
            chosenDecree,
            "private static void TryAssignChosenDecreeInHandForPlayer",
            "var boundCards = player.Piles");

        AssertSourceContains(
            martyr,
            "const int triggerCap = 2;",
            "creature.Monster is not KinFollower",
            "metadata.IsBossBrand ? 4m : 3m",
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(priest, 1, priest, null)");
        Assert.DoesNotContain("triggerCap = metadata.IsBossBrand ? 3 : 2", martyr, StringComparison.Ordinal);

        AssertSourceContains(
            inkReturn,
            "var ratio = isBossBrand ? 0.35m : 0.25m;",
            "var minimum = isBossBrand ? 5 : 3;",
            "var maximum = isBossBrand ? 18 : 12;",
            "ApplyPowerWithFinalDisplayedGain<SlipperyPower>");

        AssertSourceContains(
            startledShell,
            "TrackStartledShellDamageStart",
            "StartledShellWakeByPlayerDamagePending",
            "result.UnblockedDamage <= 0m",
            "PowerCmd.Apply<PlatingPower>",
            "metadata.IsBossBrand ? 6 : 4",
            "metadata.IsBossBrand ? 10 : 8",
            "var divisor = metadata.IsBossBrand ? 3m : 2m");
        AssertSourceContains(
            combatEvents,
            "BeforeDamageReceived(",
            "metadata.BossSeal?.Id != BossSealId.StartledShell",
            "TrackStartledShellDamageStart(tracker, target)");
        AssertSourceContains(
            rootBudEvents,
            "public override async Task BeforeDamageReceived",
            "AscensionCombatModifierService.BeforeDamageReceived");

        AssertSourceContains(
            soulTide,
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(soulFysh, 1, soulFysh, null)",
            "TrackSoulTideBeckonsBeforePlayerTurnEnd",
            "Count it before Core runs turn-end in-hand effects",
            "metadata.IsBossBrand ? 3m : 2m",
            "combatState.Players.Count(player => player.IsActiveForHooks)",
            "return playerCount <= 1 ? 12 : playerCount == 2 ? 16 : 20;",
            "return playerCount <= 1 ? 8 : playerCount == 2 ? 12 : 16;");
        AssertSourceContains(
            rootBudEvents,
            "public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)",
            "AscensionCombatModifierService.BeforeTurnEnd(state, GetTracker(state), side, participants)");
        AssertSourceContains(
            turnLifecycle,
            "if (side == CombatSide.Player)",
            "metadata.BossSeal?.Id == BossSealId.SoulTide",
            "next player turn starts",
            "await ApplySoulTidePendingBlock(combatState, tracker, metadata);");
        var bossSealPlayerTurnStart = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealPlayerTurnStart(",
            "private static async Task ApplyBossSealSideTurnStart(");
        var bossSealEnemyTurnStart = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealSideTurnStart(",
            "private static async Task ApplyBossSealTurnEnd(");
        var bossSealTurnEnd = SliceBetween(
            turnFlow,
            "private static async Task ApplyBossSealTurnEnd(",
            "// The Branded Form double-follower bonus");
        Assert.Contains("await ApplySoulTidePendingBlock(combatState, tracker, metadata);", bossSealPlayerTurnStart, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealEnemyTurnStart, StringComparison.Ordinal);
        Assert.Contains("case BossSealId.SoulTide:", bossSealTurnEnd, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealTurnEnd, StringComparison.Ordinal);

        AssertSourceContains(
            boiling,
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>",
            "artifactBefore",
            "tracker.BoilingExplosionArtifactAdded",
            "giant.GetPower<WeakPower>()",
            "PowerCmd.Remove(weak)",
            "strength is { Amount: < 0 }",
            "tracker.BoilingExplosionVulnerabilityRound == combatState.RoundNumber",
            "var vulnerable = metadata.IsBossBrand ? 2m : 1m",
            "PowerCmd.Apply<VulnerablePower>",
            "ClearBoilingExplosionFortification",
            "tracker.BoilingExplosionFortified = false",
            "Math.Min(artifact.Amount, artifactToRemove)");
        AssertSourceContains(
            tracker,
            "public bool BoilingExplosionFortified { get; set; }",
            "public int BoilingExplosionArtifactAdded { get; set; }");
        Assert.DoesNotContain("GetTypeForAmount(power.Amount) == PowerType.Debuff", boiling, StringComparison.Ordinal);

        AssertSourceContains(
            marginalNote,
            "var roundRoom = Math.Max(0, 2 - tracker.MarginalDeepThoughtAddedThisRound);",
            "ClampPowerAmount<DeepThoughtPower>(demon, metadata.IsBossBrand ? 3 : 2, demon, null)");
        var deepThought = SliceBetween(
            marginalNotePowers,
            "internal sealed class DeepThoughtPower",
            "internal sealed class DeepThoughtCostTaxPower");
        AssertSourceContains(
            deepThought,
            "private decimal GetSideCostLayers(Player player)",
            "metadata is { IsBossBrand: true, BossSeal.Id: BossSealId.MarginalNote }",
            "Math.Min(layers, 1m)",
            "PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, sideCostLayers",
            "PlayerCmd.LoseEnergy(sideCostLayers, player)");
        Assert.DoesNotContain("PowerCmd.Apply<DeepThoughtCostTaxPower>(choiceContext, player.Creature, 1m", deepThought, StringComparison.Ordinal);
        Assert.DoesNotContain("PlayerCmd.LoseEnergy(1m, player)", deepThought, StringComparison.Ordinal);

        AssertSourceContains(
            struggleBait,
            "targetPlayers.Take(1)",
            "tracker.StruggleBaitVigorGainRound == combatState.RoundNumber",
            "metadata.IsBossBrand ? 3m : 2m",
            "PowerCmd.Apply<VigorPower>");

        AssertSourceContains(
            chosenDecreeAssignStart,
            "HydrateChosenDecreeFromVisibleCards(combatState, tracker);",
            "if (metadata.BossSeal?.Id != BossSealId.ChosenDecree");
        AssertSourceContains(
            chosenDecree,
            "tracker.ChosenDecreeCardsByPlayer.Remove(player)",
            "tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player)",
            "Bound is applied as cards are drawn",
            "of always marking the first Bound card that entered hand",
            "private static bool CanMarkChosenDecree(CardModel card)",
            "card.Type is CardType.Attack or CardType.Skill or CardType.Power",
            "!card.Keywords.Contains(CardKeyword.Unplayable)",
            "ModelDb.Enchantment<RoyalDecreeEnchantment>().CanEnchant(card)",
            "catch (InvalidOperationException ex)",
            "skipped Royal Decree mark for un-enchantable Bound card",
            ".Where(CanMarkChosenDecree)",
            "var affectedPlayers = tracker.ChosenDecreeCardsByPlayer.Keys",
            "foreach (var player in affectedPlayers)",
            "ClearChosenDecreeSavedMarkers(player)",
            "tracker.ChosenDecreePlayersWhoPlayedDecree.Contains(player)",
            "tracker.ChosenDecreePlayersWhoPlayedAnyBound.Contains(player)",
            "tracker.ChosenDecreeMajestyGainedThisRound >= 2",
            "tracker.ChosenDecreeAmalgamStrengthThisRound < 2",
            "tracker.ChosenDecreeRoundCapRound == roundNumber",
            "ClampPowerAmount<RoyalMajestyPower>(queen, metadata.IsBossBrand ? 3 : 2, queen, null)");
        AssertSourceContains(
            turnFlow,
            "ResetChosenDecreeRoundCaps(tracker, combatState.RoundNumber)",
            "TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, player)",
            "await ClearBoilingExplosionFortification(combatState, tracker)",
            "poison, thorns, or delayed damage cannot",
            "ResetMartyrOathTurnCounters(tracker);");
        AssertSourceContains(
            cardEvents,
            "if (card.Owner is { } owner)",
            "TryAssignChosenDecreeInHandForPlayer(combatState, tracker, metadata, owner);");
        Assert.DoesNotContain("TryAssignChosenDecree(combatState, tracker, metadata, card);", cardEvents, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAssignChosenDecreeInHands(combatState, tracker, metadata);", chosenDecree, StringComparison.Ordinal);

        AssertSourceContains(
            aeonglass,
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2;",
            "await ArmAeonglassLaserEchoPreviewIfEligible(combatState, tracker, metadata);",
            "tracker.AeonglassTimeSand -= spent;",
            "if (tracker.AeonglassTimeSand <= 0)",
            "PowerCmd.Remove(timeSand)",
            "PowerCmd.Remove(aeonglass.GetPower<AeonglassLaserEchoPower>())",
            "The extra hit changes damage",
            "tracker.AeonglassLaserEchoesUsed < 2",
            "var pendingWither = tracker.AeonglassTimeSand",
            "tracker.AeonglassExtraWitherFromSands += pendingWither",
            "PowerCmd.Apply<AeonglassPendingWitherPower>",
            "PowerCmd.Apply<AeonglassLaserEchoUseCounterPower>",
            "CardPileCmd.AddToCombatAndPreview<Wither>");

        AssertSourceContains(
            residualSample,
            "PlayResidualSampleNotice(subject, samples);",
            "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE",
            "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason",
            "TalkCmd.Play(line, subject, VfxColor.Purple, VfxDuration.Long);");
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
