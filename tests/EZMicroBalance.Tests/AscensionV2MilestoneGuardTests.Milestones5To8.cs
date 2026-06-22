using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionV2MilestoneGuardTests
{
    [Fact]
    public void Milestones5To8GuardBannersDeepBranchesBossSealsAndBlockedA20Claims()
    {
        var metadata = ReadRepoText("EZMicroBalanceCode", "Ascension", "Map", "AscensionNodeMetadata.cs");
        var mapService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Map");
        var mapUiPatches = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var a20Patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20Patches.cs");
        var a20Courtyard = ReadRepoText("EZMicroBalanceCode", "Ascension", "Events", "A20Courtyard.cs");
        var a20RewardScreenPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AscensionA20RewardScreenPatches.cs");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");
        var marginalNoteSource = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.MarginalNote.cs");
        var aeonglassIntentPatch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "AeonglassIntentPatches.cs");
        var powers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var bossSealSource = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var englishAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");
        var englishEvents = JsonStringMap("EZMicroBalance", "localization", "eng", "events.json");
        var zhsEvents = JsonStringMap("EZMicroBalance", "localization", "zhs", "events.json");
        var apiResearch = ReadRepoText("docs", "features", "ascension-11-20", "api-research.md");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var currentDocs = ReadCurrentFacingDocs(CurrentFacingDocs);

        AssertSourceContains(metadata, "Vanguard", "Shieldwall", "BloodPrize", "PressingLine", "LastStand", "BossSealDefinition?", "DeepBranchNodeKind", "IsBossBrand");
        AssertSourceContains(
            mapService,
            "BannerRoomMapQuestMarker",
            "DeepBranchMinLength = 3",
            "DeepBranchMaxLength = 4",
            "TryInsertDeepBranch",
            "EnumerateDeepBranchColumns(map)",
            "TryMatchExistingDeepBranch",
            "IsDeepBranchRouteSafe(saved, plan)",
            "HasPathAvoiding(parent, reconnect, existingBranchPoints)",
            "runState.Players.Count > 1",
            "BossSealCatalog.TryGetForEncounter",
            "var bossSealsEnabled = AscensionFeatureGate.IsBossSealsEnabled(runState);",
            "var brandedFormEnabled = AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState);",
            "if (!bossSealsEnabled && !brandedFormEnabled)",
            "if (bossSealsEnabled)",
            "if (!brandedFormEnabled)",
            "IsBossBrand = true",
            "vanilla boss map icons reveal the boss order");

        AssertSourceContains(
            a20Patch,
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "finalAct.HasSecondBoss",
            "finalAct.SetSecondBossEncounter(secondBoss)",
            "HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))",
            "AscensionA20CourtyardService.ShouldEnterCourtyard(runState)",
            "AscensionA20CourtyardService.EnterCourtyard(__instance, runState)",
            "vanilla double-boss map path");

        AssertSourceContains(
            a20Courtyard,
            "internal sealed class A20Courtyard : EventModel",
            "public override bool IsAllowed(IRunState runState) => false;",
            "A20_COURTYARD.pages.INITIAL.description",
            "ThatWontSaveToChoiceHistory",
            "ModelDb.Event<A20Courtyard>()",
            "EnterRoomWithoutExitingCurrentRoom(eventRoom, fadeToBlack: true)",
            "SaveManager.Instance.SaveRun(eventRoom, saveProgress: false)",
            "HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))",
            "AscensionAssetPaths.BossSealIndicator",
            "GetSecondBossBrandIconPath(runState)",
            "AscensionAssetPaths.GetBossSealIndicator(definition.Id)",
            "PreloadManager.Cache.GetTexture2D(A20Courtyard.GetSecondBossBrandIconPath(__instance.Owner?.RunState))",
            "BossSealCatalog.GetLocalizationKey(definition.Id)");

        AssertSourceContains(
            a20RewardScreenPatch,
            "HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))",
            "HarmonyPatch(typeof(NRewardsScreen), \"UpdateScreenState\")",
            "IsA20BossOneIntermission",
            "A20_INTERMISSION_HEADER",
            "A20_INTERMISSION_PROCEED",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "TryGetFieldValue",
            "WarnOnce",
            "runState.Map.SecondBossMapPoint != null",
            "runState.CurrentMapCoord == runState.Map.BossMapPoint.coord");

        AssertSourceContains(
            mapUiPatches,
            "BossMapPointHoverPatch",
            "IPatchMethod.PatchId => \"ascension-boss-map-point-hover\"",
            "new ModPatchTarget(typeof(NBossMapPoint), \"OnFocus\")",
            "BOSS_DEDICATED_ABILITY",
            "BOSS_BRANDED_FORM",
            "CreateHoverTip(metadata.BossSeal, metadata.IsBossBrand)",
            "BossSealCatalog.GetLocalizationKey(definition.Id)",
            "PreloadManager.Cache.GetTexture2D(AscensionAssetPaths.GetBossSealIndicator(definition.Id))",
            "sealDescriptionKey = isBossBrand ? \"brand\" : \"summary\"",
            "metadata.IsBossBrand");

        AssertSourceContains(
            combatService,
            "GetVanguardStrength(combatState)",
            "VanguardRemovalRound = 3",
            "GetShieldwallTurnBlock(combatState)",
            "GetBloodPrizeGoldReward(combatState)",
            "GetPressingLinePartialBlock(combatState)",
            "GetLastStandBlock(combatState)",
            "PickBannerTarget(combatState)",
            "MinionPower",
            "room.AddExtraReward(player, new GoldReward(playerReward, player))",
            "metadata.BossSeal != null",
            "metadata.IsBossBrand",
            "HolyDazePower",
            "const int triggerCap = 2;",
            "var strikeDamage = metadata.IsBossBrand ? 4m : 3m;",
            "PowerCmd.Apply<MartyrOathPower>",
            "PowerCmd.Apply<MartyrOathStrikePower>",
            "ApplyPowerWithFinalDisplayedGain<ArtifactPower>(priest, 1, priest, null)",
            "CalculateInkReturnRestoreAmount",
            "tracker.InkReturnLastObservedSlippery",
            "tracker.InkReturnRestoreAmount",
            "ApplyPowerWithFinalDisplayedGain<SlipperyPower>(vantom, slippery, vantom, null)",
            "StartledShellWakeByPlayerDamagePending",
            "wokeFromPlayerDamage",
            "metadata.IsBossBrand ? 6 : 4",
            "metadata.IsBossBrand ? 10 : 8",
            "PowerCmd.Apply<PlatingPower>",
            "var divisor = metadata.IsBossBrand ? 3m : 2m;",
            "await ApplyBoilingExplosionFortification(combatState, tracker, metadata)",
            "await ApplyBoilingExplosionVulnerability(combatState, tracker, metadata, giant)",
            "metadata.IsBossBrand ? 2m : 1m",
            "PowerCmd.Apply<VulnerablePower>",
            "tracker.BoilingExplosionVulnerabilityRound = combatState.RoundNumber",
            "giant.GetPower<WeakPower>()",
            "PowerCmd.Remove(weak)",
            "strength is { Amount: < 0 }",
            "ApplySoulTidePendingBlock",
            "tracker.PendingSoulTideBlock",
            "SoulTideBlockCap",
            "CreatureCmd.GainBlock",
            "var threshold = metadata.IsBossBrand ? 0.30m : 0.35m;",
            "PowerCmd.Apply<KaiserCalibrationStrikePower>",
            "var roundRoom = Math.Max(0, 2 - tracker.MarginalDeepThoughtAddedThisRound)",
            "PowerCmd.Apply<DeepThoughtPower>",
            "tracker.StruggleBaitGeneratedEscapes.Add(escape)",
            "TrackRoyalEscapePlayed",
            "PowerCmd.Apply<VigorPower>",
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2",
            "TrackAeonglassEnergySpent",
            "tracker.AeonglassExtraWitherFromSands",
            "INCREASING_INTENSITY_MOVE",
            "CardPileCmd.AddToCombatAndPreview<Wither>",
            "PowerCmd.Apply<AeonglassLaserEchoPower>",
            "metadata.Banner == BannerKind.BloodPrize",
            "ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: true)",
            "ApplyBloodPrizePenaltyIfExpired(combatState, tracker, includeCurrentRound: false)",
            "AscensionPowerAmountHelper.RemoveTemporaryStrength(enemy, power.Amount)",
            "public static Task BeforeTurnEnd(",
            "side != CombatSide.Player",
            "TrackSoulTideBeckonsBeforePlayerTurnEnd(combatState, tracker, metadata, player)",
            "tracker.SoulTideBeckonSettlementRound != combatState.RoundNumber",
            "combatState.RoundNumber < BountyDeadlineRound",
            "!includeCurrentRound && combatState.RoundNumber <= BountyDeadlineRound",
            "TrackBoilingCriticalSteam",
            "RoyalDecreeEnchantment",
            "TrackAeonglassEnemyMove",
            "SettleAeonglassTimeSand",
            "ApplyAeonglassTimeSandAfterEbb");
        Assert.DoesNotContain("triggerCap = metadata.IsBossBrand ? 3 : 2", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("triggers up to [blue]3[/blue] times", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("SettleSoulTideBeckons(combatState, tracker, metadata)", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("PowerCmd.Apply<StrengthPower>", marginalNoteSource, StringComparison.Ordinal);
        Assert.Contains("if (side == CombatSide.Player)", combatService, StringComparison.Ordinal);
        Assert.Contains("metadata.BossSeal?.Id == BossSealId.SoulTide", combatService, StringComparison.Ordinal);
        Assert.Contains("the next player side starts", combatService, StringComparison.Ordinal);
        var bossSealPlayerTurnStartSlice = SliceBetween(
            combatService,
            "private static async Task ApplyBossSealPlayerTurnStart(",
            "private static async Task ApplyBossSealSideTurnStart(");
        var bossSealEnemyTurnStartSlice = SliceBetween(
            combatService,
            "private static async Task ApplyBossSealSideTurnStart(",
            "private static async Task ApplyBossSealTurnEnd(");
        var bossSealTurnEndSlice = SliceBetween(
            combatService,
            "private static async Task ApplyBossSealTurnEnd(",
            "// The Branded Form double-follower bonus");
        Assert.Contains(
            "await ApplySoulTidePendingBlock(combatState, tracker, metadata);",
            bossSealPlayerTurnStartSlice,
            StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealEnemyTurnStartSlice, StringComparison.Ordinal);
        Assert.Contains("case BossSealId.SoulTide:", bossSealTurnEndSlice, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplySoulTidePendingBlock", bossSealTurnEndSlice, StringComparison.Ordinal);
        var playerTurnStartBannerSlice = SliceBetween(
            combatService,
            "private static async Task ApplyBannerTurnStart(",
            "private static async Task AfterBannerEnemyHpChanged(");
        Assert.DoesNotContain("case BannerKind.Shieldwall", playerTurnStartBannerSlice, StringComparison.Ordinal);
        Assert.Contains("case BannerKind.BloodPrize", playerTurnStartBannerSlice, StringComparison.Ordinal);
        Assert.Contains("includeCurrentRound: false", playerTurnStartBannerSlice, StringComparison.Ordinal);

        AssertSourceContains(
            powers,
            "internal static class AscensionPowerAmountHelper",
            "strength.SetAmount(strength.Amount - (int)amount, silent: true)",
            "AscensionPowerAmountHelper.RemoveTemporaryStrength(Owner, Amount)");
        Assert.Contains("[gold]Intangible[/gold]", powers, StringComparison.Ordinal);
        Assert.Contains("[gold]无形[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("equal [gold]Block[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("等量[gold]格挡[/gold]", powers, StringComparison.Ordinal);
        Assert.DoesNotContain("PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -Amount", powers, StringComparison.Ordinal);
        AssertSourceContains(
            aeonglassIntentPatch,
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))",
            "__instance.Repeats + 1",
            "HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))");

        AssertSourceContains(
            bossSealSource,
            "BossSealImplementationStatus.SourceGuardedPendingLiveVerification",
            "RuntimeEvidence",
            "HolyDaze",
            "BOSS_SEAL_HOLY_DAZE",
            "BOSS_SEAL_STRUGGLE_BAIT",
            "source-confirmed two KinFollower deaths",
            "Restores 35% of the cleared Slippery",
            "natural wake grants 10",
            "clear Weak and attack reduction",
            "claws' HP percentages differ",
            "Unplayed Notes become Deep Thought",
            "Every 3 ability-made Frantic Escapes played gives 3 Vigor",
            "Time Sand Reflow",
            "ResidualSample");
        Assert.DoesNotContain("Brand parameters are not designed for A20 yet", bossSealSource, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "BossRewardTargetOptionCount = 4",
            "TryAddBossSealRewardOption",
            "TryAddA20BossOneCardReward",
            "AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(runState)",
            "runState.Map.SecondBossMapPoint == null",
            "runState.CurrentMapCoord != runState.Map.BossMapPoint.coord",
            "new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player)");
        Assert.Equal("Banner Room", englishAscension["BANNER_ROOM.title"]);
        Assert.Contains("round [blue]3[/blue]", englishAscension["BANNER_VANGUARD.description"], StringComparison.Ordinal);
        Assert.Contains("[blue]{Gold}[/blue] [gold]Gold[/gold]", englishAscension["BANNER_BLOOD_PRIZE.description"], StringComparison.Ordinal);
        Assert.Equal("Boss Dedicated Ability", englishAscension["BOSS_DEDICATED_ABILITY.title"]);
        Assert.Equal("Boss Dedicated Abilities", englishAscension["LEVEL_19.title"]);
        Assert.Equal("Branded Form", englishAscension["LEVEL_20.title"]);
        Assert.Contains("Attack changes from this ability are shown in intent", englishAscension["BOSS_DEDICATED_ABILITY.description"], StringComparison.Ordinal);
        Assert.Equal("Branded Form", englishAscension["BOSS_BRANDED_FORM.title"]);
        Assert.Contains("second Act [blue]3[/blue] Boss enters [gold]Branded Form[/gold]", englishAscension["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
        Assert.Equal("Holy Daze", englishAscension["BOSS_SEAL_HOLY_DAZE.title"]);
        Assert.Contains("capped at [blue]2[/blue]", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("+[blue]4[/blue] damage per Oath", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]1[/blue] [gold]Artifact[/gold]", englishAscension["BOSS_SEAL_MARTYR_OATH.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]35%[/blue]", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("max [blue]18[/blue]", englishAscension["BOSS_SEAL_INK_RETURN.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]10[/blue]", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("one-third", englishAscension["BOSS_SEAL_STARTLED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("Team cap: solo [blue]12[/blue], 2 players [blue]16[/blue], 3-4 players [blue]20[/blue]", englishAscension["BOSS_SEAL_SOUL_TIDE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] turns of [gold]Vulnerable[/gold]", englishAscension["BOSS_SEAL_BOILING_CRITICAL.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]30%[/blue] HP difference", englishAscension["BOSS_SEAL_MISALIGNED_SHELL.brand"], StringComparison.Ordinal);
        Assert.Contains("Deep Thought", englishAscension["BOSS_SEAL_MARGINAL_NOTE.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]3[/blue] [gold]Vigor[/gold]", englishAscension["BOSS_SEAL_STRUGGLE_BAIT.brand"], StringComparison.Ordinal);
        Assert.Contains("Eye Lasers", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("[blue]3[/blue] Time Sand", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("extra [gold]Wither[/gold]", englishAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.brand"], StringComparison.Ordinal);
        Assert.Contains("Playing the Decree has no extra penalty", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("non-Decree Bound card gives Queen [blue]1[/blue] [gold]Majesty[/gold]", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("Play it for player Block", englishAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("Majesty cap becomes [blue]3[/blue]", englishAscension["BOSS_SEAL_CHOSEN_DECREE.brand"], StringComparison.Ordinal);
        Assert.Contains("\u6253\u51fa\u5fa1\u4ee4\u724c\u4e0d\u4f1a\u89e6\u53d1\u989d\u5916\u60e9\u7f5a", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u738b\u4ee4", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u6253\u51fa\u5b83\u83b7\u5f97\u683c\u6321", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.Contains("[blue]2[/blue] different samples", englishAscension["BOSS_SEAL_RESIDUAL_SAMPLE.brand"], StringComparison.Ordinal);
        foreach (var key in englishAscension.Keys.Where(key => key.StartsWith("BOSS_SEAL_", StringComparison.Ordinal)))
        {
            Assert.True(zhsAscension.ContainsKey(key), $"Missing zhs Boss Seal key: {key}");
        }

        Assert.Equal("Courtyard Ahead", englishAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("Enter the Courtyard", englishAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("\u6218\u65d7\u623f", zhsAscension["BANNER_ROOM.title"]);
        Assert.Equal("\u9996\u9886\u4e13\u5c5e\u80fd\u529b", zhsAscension["BOSS_DEDICATED_ABILITY.title"]);
        Assert.Equal("\u70d9\u5370\u5f62\u6001", zhsAscension["BOSS_BRANDED_FORM.title"]);
        Assert.Contains("\u7b2c[blue]3[/blue]\u5e55\u7b2c\u4e8c\u540d\u9996\u9886\u8fdb\u5165[gold]\u70d9\u5370\u5f62\u6001[/gold]", zhsAscension["BOSS_BRANDED_FORM.description"], StringComparison.Ordinal);
        Assert.Equal("\u524d\u65b9\u4e2d\u5ead", zhsAscension["A20_INTERMISSION_HEADER"]);
        Assert.Equal("\u8fdb\u5165\u4e2d\u5ead", zhsAscension["A20_INTERMISSION_PROCEED"]);
        Assert.Equal("Courtyard Before the Second King", englishEvents["A20_COURTYARD.title"]);
        Assert.Contains("{SealSummary}", englishEvents["A20_COURTYARD.pages.INITIAL.description"], StringComparison.Ordinal);
        Assert.Equal("\u7b2c\u4e8c\u738b\u524d\u7684\u4e2d\u5ead", zhsEvents["A20_COURTYARD.title"]);
        Assert.Contains("{SealSummary}", zhsEvents["A20_COURTYARD.pages.READY.description"], StringComparison.Ordinal);

        var hasV2BossSealTable = ReadSourceTree("EZMicroBalanceCode", "Ascension").Contains("BossSealDefinition", StringComparison.Ordinal);
        if (!hasV2BossSealTable)
        {
            Assert.Contains("source-guarded through supported hooks", apiResearch, StringComparison.Ordinal);
            Assert.Contains("Armor/Rage/Barrier/Chaos", apiResearch, StringComparison.Ordinal);
            Assert.Contains("Boss 2 Branded Form metadata", apiResearch, StringComparison.Ordinal);
        }

        Assert.Contains("A20 creates the final-act second Boss through the vanilla double-boss map path when the A20 gate is active.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("A20 Boss 1 reward screen offers one Boss card reward before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Boss 1 reward screen opens the A20 courtyard event before the second Boss.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seal / King Brand", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("Royal Seal / 王印", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotContain("King Brand / 王烙印", currentDocs, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"(?i)\bA20\b[^\r\n.]*\b(?:release-ready|fully verified|complete)\b", currentDocs);
        Assert.DoesNotMatch(@"(?i)\bA11-A20\b[^\r\n.]*\b(?:release-ready|fully verified)\b", currentDocs);
    }

}
