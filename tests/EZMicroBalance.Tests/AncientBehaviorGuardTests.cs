using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void SovereignBladeJadeBoonsApplyOnPlayAndAreExplainedByForge()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SovereignBladeForgePatches.cs");
        var cardsEng = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var cardsZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var staticEng = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var staticZhs = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");

        AssertSourceContains(
            source,
            "public const decimal Amount = 3m",
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
            "await original;",
            "PowerCmd.Apply<StrengthPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<DexterityPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<PlatingPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<RegenPower>(choiceContext, owner, Amount, owner, blade)",
            "PowerCmd.Apply<VigorPower>(choiceContext, owner, Amount, owner, blade)",
            "[HarmonyPatch(typeof(CardModel), \"get_HoverTips\")]",
            "HoverTipFactory.FromPower<StrengthPower>((int)Amount)",
            "HoverTipFactory.FromPower<DexterityPower>((int)Amount)",
            "HoverTipFactory.FromPower<PlatingPower>((int)Amount)",
            "HoverTipFactory.FromPower<RegenPower>((int)Amount)",
            "HoverTipFactory.FromPower<VigorPower>((int)Amount)");

        AssertSovereignBladeText(cardsEng["SOVEREIGN_BLADE.description"], "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(cardsZhs["SOVEREIGN_BLADE.description"], "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
        AssertSovereignBladeText(staticEng["FORGE.description"], "Sovereign Blade", "Strength", "Dexterity", "Plating", "Regen", "Vigor");
        AssertSovereignBladeText(staticZhs["FORGE.description"], "\u541b\u738b\u4e4b\u5251", "\u529b\u91cf", "\u654f\u6377", "\u8986\u7532", "\u518d\u751f", "\u6d3b\u529b");
    }

    [ReleaseArtifactFact]
    public void PrivateBetaZipContainsOnlyInstallableActiveModFiles()
    {
        var packagePath = CurrentPackageZipPath();
        Assert.True(File.Exists(packagePath), $"Missing private beta package: {packagePath}");

        using var archive = ZipFile.OpenRead(packagePath);
        var entries = archive.Entries
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .Select(entry => entry.FullName.Replace('\\', '/'))
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "EZMicroBalance/EZMicroBalance.dll",
                "EZMicroBalance/EZMicroBalance.json",
                "EZMicroBalance/EZMicroBalance.pck",
                "EZMicroBalance/README_INSTALL.txt"
            ],
            entries);

        using var manifest = JsonDocument.Parse(ReadZipText(archive, "EZMicroBalance/EZMicroBalance.json"));
        Assert.Equal("EZMicroBalance", manifest.RootElement.GetProperty("id").GetString());
        Assert.Contains(
            manifest.RootElement.GetProperty("dependencies").EnumerateArray(),
            dependency => dependency.ValueKind == JsonValueKind.Object &&
                dependency.TryGetProperty("id", out var id) &&
                id.GetString() == "BaseLib" &&
                dependency.TryGetProperty("min_version", out var minVersion) &&
                minVersion.GetString() == "v3.1.4");

        var readme = ReadZipText(archive, "EZMicroBalance/README_INSTALL.txt");
        Assert.Contains("Spire Plus manual-test package", readme, StringComparison.Ordinal);
        Assert.Contains($"Archive: {CurrentPackageName()}.zip", readme, StringComparison.Ordinal);
        Assert.Contains("Display name: Spire Plus", readme, StringComparison.Ordinal);
        Assert.Contains("Technical compatibility id: EZMicroBalance", readme, StringComparison.Ordinal);
        Assert.Contains("Extract this archive into the Slay the Spire 2 mods folder exactly as packaged.", readme, StringComparison.Ordinal);
        Assert.Contains("If the game's Mods list shows EZMicroBalance as the mod name, the package is stale or the display-name route regressed.", readme, StringComparison.Ordinal);
        Assert.Contains("BaseLib", readme, StringComparison.Ordinal);
        Assert.Contains("EzDailyContent disabled or absent", readme, StringComparison.Ordinal);
        Assert.Contains("EZMicroBalance is a technical folder/id only; player-facing screens should say Spire Plus.", readme, StringComparison.Ordinal);
        Assert.Contains("Ancient selections now grant visible marker relics", readme, StringComparison.Ordinal);
        Assert.Contains("manual-test build, not release-ready", readme, StringComparison.Ordinal);
        Assert.Contains("Save/load, death/failure paths, and co-op still need manual proof", readme, StringComparison.Ordinal);
        Assert.Contains("Ascension 21-30 and custom-character content are not included", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("source-safe", readme, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Found 16 SavedSpireFields", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            allSource,
            "[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.AfterObtained))",
            "ModPatchTarget(typeof(RelicCmd), nameof(RelicCmd.Obtain)",
            "[HarmonyPatch(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))]",
            "[HarmonyPatch(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))]",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.ModifyMaxEnergy))",
            "ModPatchTarget(typeof(SealOfGold), nameof(SealOfGold.AfterSideTurnStart))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.AfterCreated))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.FromSerializable))",
            "ModPatchTarget(typeof(Debt), nameof(Debt.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), nameof(Debt.HasTurnEndInHandEffect), MethodType.Getter)",
            "ModPatchTarget(typeof(Debt), \"OnTurnEndInHand\")",
            "ModPatchTarget(typeof(CardModel), \"OnPlay\")",
            "ModPatchTarget(typeof(CardCmd), nameof(CardCmd.Exhaust))",
            "[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]",
            "[HarmonyPatch(typeof(CardReward), nameof(CardReward.Populate))]",
            "[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]",
            "ModPatchTarget(typeof(DistinguishedCape), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Vakuu), \"GenerateInitialOptions\")",
            "ModPatchTarget(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(VelvetChoker), \"get_DisplayAmount\")]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))]",
            "[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]",
            "[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]",
            "[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))]",
            "[HarmonyPatch(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))]",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "[HarmonyPatch(typeof(ForgeCmd), nameof(ForgeCmd.Forge))]",
            "[HarmonyPatch(typeof(SovereignBlade), \"OnPlay\")]",
            "[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))]",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "[HarmonyPatch(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))]",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))",
            "[HarmonyPatch(typeof(CookRestSiteOption), \"get_IsEnabled\")]",
            "[HarmonyPatch(typeof(CookRestSiteOption), \"get_Description\")]",
            "[HarmonyPatch(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))]",
            "[HarmonyPatch(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]",
            "[HarmonyPatch(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))]",
            "[HarmonyPatch(typeof(Folly), \"get_CanonicalKeywords\")]",
            "ModPatchTarget(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))",
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "ModPatchTarget(typeof(Fiddle), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ShouldDraw))",
            "ModPatchTarget(typeof(CardPileCmd), nameof(CardPileCmd.Draw)",
            "[HarmonyPatch(typeof(IronClub), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BrilliantScarf), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), \"get_CanonicalVars\")]",
            "[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]",
            "[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))");
    }

    [Fact]
    public void PrismaticGemRerollStateIsScreenScopedCounterSafeAndReplacesAllSlots()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var apiDiscovery = ReadRepoText("docs", "features", "ancients-rework-v4", "api-discovery.md");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");

        Assert.Equal("Gain 1 Energy. Every second standard card reward contains only off-color cards.", relics["PRISMATIC_GEM.description"]);

        AssertSourceContains(
            source,
            "[ThreadStatic]",
            "Stack<CardReward>",
            "HarmonyFinalizer",
            "PrismaticGemRewardScreenContextPatch.CurrentReward",
            "HarmonyPrefix",
            "player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted)",
            "foreach (var listener in runState.IterateHookListeners(null))",
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "if (listenerModified)",
            "modifiers.Add(listener)",
            "Prismatic replacement sits between Core's early and late reward hooks",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "CleanupSupersededPrismaticReplacements(cardRewardOptions)",
            "ConditionalWeakTable<CardReward, RewardScreenState>",
            "public CardModel? PrismaticReplacement { get; set; }",
            "var madeTriggerDecision = !screenState.HasTriggerDecision",
            "if (madeTriggerDecision)",
            "screenState.CounterAtDecision = AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] + 1",
            "AncientSavedStateFields.PrismaticGemNormalRewardCounter[prismaticGem] = screenState.CounterAtDecision",
            "screenState.ShouldReplaceAllSlots = screenState.CounterAtDecision % 2 == 0",
            "else if (!isNormalCardReward)",
            "creationOptions.Source == CardCreationSource.Encounter",
            "creationOptions.RarityOdds == CardRarityOddsType.RegularEncounter",
            "creationOptions.CustomCardPool == null",
            "creationOptions.CardPoolFilter == null",
            "!creationOptions.CardPools.All(pool => pool.IsColorless)",
            "ReplaceAllRewardSlots",
            "for (var slotIndex = 0; slotIndex < cardRewardOptions.Count; slotIndex++)",
            "RestoreCounterAfterFailedReplacement(prismaticGem, screenState)",
            "reward.ModifyCard(replacement, prismaticGem)",
            "TrackPrismaticReplacement(reward, replacement)",
            "RewardResultHints.GetValue(reward, _ => new RewardResultHintState()).PrismaticReplacement = replacement",
            "if (!RewardResultHints.TryGetValue(reward, out var hintState)",
            "ReferenceEquals(reward.Card, hintState.PrismaticReplacement)",
            "AncientCardHelpers.RemoveUnpiledRunCard(hintState.PrismaticReplacement)",
            "excludedIds.Add(replacement.Id)",
            "player.RunState.RemoveCard(originalCard)",
            "RemoveUnpiledReplacements(replacements)",
            "AncientCardHelpers.RemoveUnpiledRunCard(replacement)",
            ".Where(card => type == null || card.Type == type)",
            "GetOffColorRewardPool(player, originalCard.Rarity, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, null, originalCard.Type, excludedIds)",
            "GetOffColorRewardPool(player, originalCard.Rarity, null, excludedIds)",
            "GetOffColorRewardPool(player, null, null, excludedIds)");
        AssertBefore(
            source,
            "listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions)",
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)");
        AssertBefore(
            source,
            "TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions)",
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)");
        AssertBefore(
            source,
            "listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions)",
            "CleanupSupersededPrismaticReplacements(cardRewardOptions)");

        AssertSourceContains(
            source,
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "PRISMATIC_GEM.countHint.title",
            "PRISMATIC_GEM.countHint.nextNormal",
            "PRISMATIC_GEM.countHint.nextOffColor",
            "NCardRewardSelectionScreen",
            "PRISMATIC_GEM.rewardScreenHint",
            "BannerNodePath = \"UI/Banner\"",
            "TryGetCompatibleBannerField",
            "typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType)",
            "TryApplyBannerNodeHint",
            "GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath)",
            "InfoOnce(",
            "WarnOnce(",
            "visible all-off-color cards and the Prismatic Gem relic hover count remain available");

        Assert.DoesNotContain("ShouldReplaceRightmostSlot", source, StringComparison.Ordinal);
        Assert.DoesNotContain("var slotIndex = cardRewardOptions.Count - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.TryModifyCardRewardOptions))]", source, StringComparison.Ordinal);

        AssertSourceContains(
            apiDiscovery,
            "CardReward.Reroll()",
            "Rerolls reuse the same `CardReward` state",
            "eligible normal rewards increment the saved counter once",
            "ineligible rewards store a non-trigger decision and do not increment",
            "Trigger screens regenerate all-slot off-color replacements",
            "reward-screen banner hint");
    }

    [Fact]
    public void VelvetChokerSoftLimitCountsOnlyManualFirstFromHandPlaysAndResetsEachTurn()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Equal("Gain 1 Energy. Each turn, the 7th and later cards played from your hand cost 1 more.", relics["VELVET_CHOKER.description"]);

        AssertSourceContains(
            source,
            "new DynamicVar[] { new CardsVar(7), new EnergyVar(1) }",
            "VelvetChokerShouldPlayPatch",
            "__result = true",
            "CardEnergyCost.GetWithModifiers",
            "modifiers.HasFlag(CostModifiers.Global)",
            "__result += VelvetChokerSoftLimitTracker.ExtraEnergyCost",
            "PlayerCombatState.HasEnoughResourcesFor",
            "UnplayableReason.EnergyCostTooHigh",
            "CardModel.SpendResources",
            "CapturedXValue = Math.Max(0, result.Item1 - VelvetChokerSoftLimitTracker.ExtraEnergyCost)",
            "!cardPlay.IsAutoPlay",
            "cardPlay.IsFirstInSeries",
            "!cardPlay.Card.IsClone",
            "cardPlay.Card.Owner == __instance.Owner",
            "card.IsClone",
            "card.Pile?.Type != PileType.Hand",
            "HandPlayedThisTurn(choker) >= FreeHandPlaysPerTurn",
            "BeforeSideTurnStart",
            "side == __instance.Owner.Creature.Side",
            "AfterRoomEntered",
            "AfterCombatEnd");

        var shouldTax = SliceBetween(
            source,
            "public static bool ShouldTax(CardModel card)",
            "private static Player? TryGetOwner");
        var tryGetOwner = SliceBetween(
            source,
            "private static Player? TryGetOwner",
            "public static void Increment");
        AssertSourceContains(
            shouldTax,
            "if (!CombatManager.Instance.IsInProgress",
            "SuppressedCostCards.Contains(card)",
            "card.Pile?.Type != PileType.Hand",
            "var owner = TryGetOwner(card)",
            "owner?.GetRelic<VelvetChoker>()");
        Assert.True(
            shouldTax.IndexOf("CombatManager.Instance.IsInProgress", StringComparison.Ordinal) <
            shouldTax.IndexOf("TryGetOwner(card)", StringComparison.Ordinal),
            "Velvet Choker cost checks must reject non-combat/card-library contexts before reading CardModel.Owner.");
        AssertSourceContains(
            tryGetOwner,
            "return card.Owner",
            "catch (MegaCrit.Sts2.Core.Models.Exceptions.CanonicalModelException)",
            "return null");
        Assert.DoesNotContain("card.Owner?.GetRelic<VelvetChoker>()", shouldTax, StringComparison.Ordinal);

        AssertSourceContains(
            turnSource,
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, item.Card.CanPlay)",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(item.Card, () => AncientCardHelpers.EffectiveCost(item.Card))",
            "VelvetChokerSoftLimitTracker.SuppressCostFor(card, card.SpendResources)");

        Assert.Contains("| Velvet Choker |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("copied, autoplayed, or repeated plays do not advance the counter", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DistinguishedCapeUsesV43MaxHpMathAndCannotBeSelectedWhenUnableToPay()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        Assert.Equal("On pickup, lose 30% of current Max HP, at least 18. Add 3 Apparitions.", relics["DISTINGUISHED_CAPE.description"]);
        Assert.DoesNotContain("cannot reduce Max HP below 1", relics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);
        Assert.Contains("Add 3 Apparitions", relics["DISTINGUISHED_CAPE.description"], StringComparison.Ordinal);

        Assert.Equal(24, DistinguishedCapeLossForTest(80));
        Assert.Equal(21, DistinguishedCapeLossForTest(70));
        Assert.Equal(18, DistinguishedCapeLossForTest(60));
        Assert.Equal(18, DistinguishedCapeLossForTest(30));
        Assert.Equal(18, DistinguishedCapeLossForTest(19));
        Assert.Equal(18, DistinguishedCapeLossForTest(18));
        Assert.Equal(18, DistinguishedCapeLossForTest(10));
        Assert.Equal(18, DistinguishedCapeLossForTest(1));
        Assert.True(CanPayDistinguishedCapeCostForTest(80));
        Assert.True(CanPayDistinguishedCapeCostForTest(19));
        Assert.False(CanPayDistinguishedCapeCostForTest(18));
        Assert.False(CanPayDistinguishedCapeCostForTest(10));

        AssertSourceContains(
            source,
            "public const decimal MaxHpLossPercent = 0.30m",
            "public const int MinimumMaxHpLoss = 18",
            "public const int ApparitionsToAdd = 3",
            "var proportionalLoss = (int)Math.Ceiling(currentMaxHp * MaxHpLossPercent)",
            "return Math.Max(proportionalLoss, MinimumMaxHpLoss)",
            "public static bool CanPayMaxHpCost(int currentMaxHp)",
            "return currentMaxHp > CalculateMaxHpLoss(currentMaxHp)",
            "static string IPatchMethod.PatchId => \"distinguished-cape-event-option\"",
            "CreateVakuuSecondPoolReplacement",
            "vakuu.AllPossibleOptions",
            "option.Relic is PreservedFog or SereTalon",
            "return options.ToArray()",
            "vakuu.Rng.NextItem(candidates)",
            "CreateLockedCapeOption",
            "DISTINGUISHED_CAPE.unpayableOption",
            "await CreatureCmd.SetCurrentHp(creature, newMaxHp)",
            "await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, maxHpLoss, isFromCard: false)",
            "CreateCard<Apparition>");

        Assert.DoesNotContain("currentMaxHp - 1", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ThatWillKillPlayerIf(_ => false)", source, StringComparison.Ordinal);
        Assert.DoesNotContain(".Where(option => option.Relic is not DistinguishedCape)", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AccessTools.Field(typeof(AncientEventModel)", source, StringComparison.Ordinal);

        var distinguishedCapeSection = SliceBetween(source, "internal sealed class DistinguishedCapePickupPatch", "[HarmonyPatch(typeof(PreservedFog)");
        Assert.DoesNotContain("CreatureCmd.Damage", distinguishedCapeSection, StringComparison.Ordinal);
        Assert.DoesNotContain("ValueProp", distinguishedCapeSection, StringComparison.Ordinal);

        Assert.Contains("| Distinguished Cape |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("max HP loss is not damage", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("cannot be selected when current Max HP is not greater than the v4.3 cost", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DistinguishedCapeUnaffordableVakuuPathPreservesVisibleOptionCount()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DistinguishedCapePatches.cs");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");
        var replacementBranch = SliceBetween(
            source,
            "private static IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> Postfix(",
            "private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement");
        var replacementFactory = SliceBetween(
            source,
            "private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement",
            "private static bool IsPayableVakuuSecondPoolOption");
        var payablePredicate = SliceBetween(
            source,
            "private static bool IsPayableVakuuSecondPoolOption",
            "private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption");
        var lockedFallback = SliceBetween(
            source,
            "private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption",
            "class DistinguishedCapePickupPatch : IPatchMethod");

        AssertSourceContains(
            replacementBranch,
            "var options = __result.ToList();",
            "var capeIndex = options.FindIndex(option => option.Relic is DistinguishedCape);",
            "var replacement = CreateVakuuSecondPoolReplacement(__instance, options);",
            "options[capeIndex] = replacement;",
            "return options.ToArray();",
            "options[capeIndex] = CreateLockedCapeOption(__instance, options[capeIndex], owner.Creature.MaxHp);",
            "return options.ToArray();");

        Assert.Equal(2, Regex.Matches(replacementBranch, @"options\[capeIndex\]\s*=").Count);
        Assert.Equal(2, Regex.Matches(replacementBranch, @"return\s+options\.ToArray\(\);").Count);
        foreach (var countChangingApi in new[] { ".Add(", ".AddRange(", ".Insert(", ".InsertRange(", ".Clear(", ".Remove(", ".RemoveAt(", ".RemoveAll(", ".Where(", ".Take(", ".Skip(" })
        {
            Assert.DoesNotContain(countChangingApi, replacementBranch, StringComparison.Ordinal);
        }

        AssertSourceContains(
            replacementFactory,
            ".Select(option => option.TextKey)",
            ".ToHashSet(StringComparer.Ordinal)",
            "vakuu.AllPossibleOptions",
            ".Where(IsPayableVakuuSecondPoolOption)",
            ".Where(option => !currentKeys.Contains(option.TextKey))",
            "vakuu.Rng.NextItem(candidates)");

        AssertSourceContains(
            payablePredicate,
            "return option.Relic is PreservedFog or SereTalon;");

        AssertSourceContains(
            lockedFallback,
            "DISTINGUISHED_CAPE.unpayableOption",
            "description.Add(\"Cost\", (decimal)DistinguishedCapePickupPatch.CalculateMaxHpLoss(currentMaxHp))",
            "new MegaCrit.Sts2.Core.Events.EventOption(",
            "null,",
            "originalOption.Title",
            "originalOption.TextKey",
            "originalOption.HoverTips",
            "lockedOption.WithRelic(originalOption.Relic)");

        Assert.Contains("Vakuu must still show three normal reward options", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("localized locked Cape only as a defensive fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("low-Max-HP Vakuu still shows three normal choices", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void PrismaticGemRewardScreenHintHasGuardedBannerFallbackDiagnostics()
    {
        var hintPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintPatch.cs");
        var sharedBanner = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.cs");
        var fieldSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.Field.cs");
        var nodeSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "PrismaticGemRewardScreenHintBanner.Node.cs");
        var source = string.Join(Environment.NewLine, hintPatch, sharedBanner, fieldSource, nodeSource);
        var applyHint = SliceFrom(hintPatch, "private static void ApplyRewardScreenHint");
        var fieldFallback = SliceBetween(
            fieldSource,
            "private static bool TryApplyBannerFieldHint(",
            "private static bool TryGetCompatibleBannerField(");
        var nodeFallback = SliceBetween(
            nodeSource,
            "private static bool TryApplyBannerNodeHint(",
            "private static void ConfirmBannerNodeHintAfterFieldSuccess(");
        var testPlan = ReadRepoText("docs", "test-plan.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            source,
            "private static readonly System.Reflection.FieldInfo? BannerField",
            "if (TryApplyBannerFieldHint(screen, hintText))",
            "if (TryApplyBannerNodeHint(screen, hintText))",
            "TryGetCompatibleBannerField(out var bannerField, out var reason)",
            "if (BannerField == null)",
            "typeof(MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner).IsAssignableFrom(BannerField.FieldType)",
            "bannerField.GetValue(screen)",
            "screen.GetNodeOrNull<MegaCrit.Sts2.Core.Nodes.CommonUi.NCommonBanner>(BannerNodePath)",
            "private _banner field unavailable",
            "private _banner field resolved but did not contain a banner instance",
            "private _banner access failed",
            "fallback applied through {BannerNodePath} node lookup",
            "fallback unavailable",
            "reward-screen hint unavailable",
            "visible all-off-color cards and the Prismatic Gem relic hover count remain available");

        Assert.Equal(Regex.Matches(fieldFallback, @"return false;").Count, Regex.Matches(fieldFallback, @"WarnOnce\(").Count);
        Assert.Equal(Regex.Matches(nodeFallback, @"return false;").Count, Regex.Matches(nodeFallback, @"WarnOnce\(").Count);
        Assert.Contains("WarnOnce(", applyHint, StringComparison.Ordinal);
        Assert.Contains("InfoOnce(", fieldFallback, StringComparison.Ordinal);
        Assert.Contains("catch (Exception exception)", fieldFallback, StringComparison.Ordinal);
        Assert.Contains("catch (Exception exception)", nodeFallback, StringComparison.Ordinal);
        Assert.DoesNotContain("BannerField!.GetValue", source, StringComparison.Ordinal);
        Assert.DoesNotContain("BannerField.GetValue(screen)", source, StringComparison.Ordinal);
        Assert.DoesNotContain("catch {", source, StringComparison.Ordinal);

        Assert.Contains("banner fallback diagnostics", testPlan, StringComparison.Ordinal);
        Assert.Contains("manual-test coverage", testPlan, StringComparison.Ordinal);
        Assert.Contains("the reward-screen hint logs a fallback if the banner cannot be updated", testPlan, StringComparison.Ordinal);
        Assert.Contains("If the trigger reward banner is not visible", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("PrismaticGem reward-screen hint fallback", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("relic hover count plus every visible reward card being off-color", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("visible all-off-color cards and relic hover count remain the available confirmation surfaces", manualMatrix, StringComparison.Ordinal);
    }

    private static string ReadSereTalonVisualSource() =>
        string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"));

    [Fact]
    public void AncientDirectDeckGainFeedbackFlashesSourceRelicAndCardPreview()
    {
        var feedbackSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "SpirePlusFeedback.cs");
        var ancientSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            feedbackSource,
            "RelicTriggerSfx = \"event:/sfx/ui/relic_activate_general\"",
            "sourceRelic.Flash()",
            "NRelicFlashVfx.Create(sourceRelic)",
            "AboveTopBarVfxContainer.AddChildSafely(flashVfx)",
            "public static void ConfirmRelicPayoff(RelicModel? sourceRelic)",
            "models.Insert(0, sourceRelic)",
            "CardCmd.PreviewCardPileAdd(successfulAdds, seconds)",
            "NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short)");

        AssertSourceContains(
            ancientSource,
            "SpirePlusFeedback.PreviewDeckAdds(results, paelsHorn, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, jewelryBox, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(result, preservedFog, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, cape, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(results, sealOfGold, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, paelsTooth)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResults, player.GetRelic<UrdaMoltingOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedbedOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaTrialBranchOptionRelic>(), 2f)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<UrdaSeedBankOptionRelic>(), 2f)",
            "SpirePlusFeedback.ConfirmRelicPayoff(eliteRoot)",
            "SpirePlusFeedback.PreviewDeckAdds(addResult, player.GetRelic<MorviForbiddenLoanOptionRelic>(), 2f)");
    }

    [Fact]
    public void JeweledMaskCustomEnchantmentIsPowerOnlyPersistentAndCombatStartScoped()
    {
        var enchantment = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "JeweledMaskFreePower.cs");
        var pickupSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var combatSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            enchantment,
            "CustomEnchantmentModel, ILocalizationProvider",
            "public override bool HasExtraCardText => true",
            "return cardType == CardType.Power",
            "Card.EnergyCost.SetCustomBaseCost(0)");

        AssertSourceContains(
            pickupSource,
            "card => card.Type == CardType.Power && card.Enchantment == null",
            "DraftGeneratedPowerForJeweledMask(owner)",
            "owner.RunState.RemoveCard(unselected)",
            "await CardPileCmd.Add(selected, PileType.Deck)",
            "CardCmd.Enchant<JeweledMaskFreePower>(selected, 1m)");

        AssertSourceContains(
            combatSource,
            "[HarmonyPatch(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))]",
            "combatState.RoundNumber > 1",
            "AncientCardHelpers.IsJeweledMaskPower",
            "await CardPileCmd.Add(markedPower, PileType.Hand)",
            "marked power already in hand",
            "no marked power in draw pile or hand");

        Assert.Contains("Jeweled Mask free power", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Permanent 0-cost enchantment survives save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void JewelryBoxApotheosisMarkerIsScopedToCreatedCardsAndHoverPreviews()
    {
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");

        AssertSourceContains(
            source,
            "CreateNonInnateApotheosis(jewelryBox.Owner)",
            "JewelryBoxApotheosisMarker.Mark(result.cardAdded)",
            "ConditionalWeakTable<CardModel, MarkerState>",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card] = true",
            "if (card is not Apotheosis)",
            "AncientCardHelpers.RemoveKeywords(card, CardKeyword.Innate)",
            "AncientSavedStateFields.JewelryBoxNonInnateApotheosis[card]",
            "[HarmonyPatch(typeof(Apotheosis), \"get_CanonicalKeywords\")]",
            "JewelryBoxApotheosisMarker.IsMarked(__instance)",
            "keyword => keyword != CardKeyword.Innate",
            "CreateNonInnateApotheosisHoverTips",
            "[HarmonyPatch(typeof(JewelryBox), \"get_ExtraHoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTips\")]",
            "[HarmonyPatch(typeof(RelicModel), \"get_HoverTipsExcludingRelic\")]");
    }

    [Fact]
    public void PaelsToothSavedCounterAndStoredCardReturnAreGuarded()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            savedFields,
            "SavedSpireField<PaelsTooth, int>",
            "EZMicroBalanceNonBossCombatCounter");

        AssertSourceContains(
            source,
            "AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0",
            "[HarmonyPatch(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))]",
            "if (paelsTooth.Owner.Creature.IsDead)",
            "if (paelsTooth.SerializableCards.Count == 0)",
            "if (room.RoomType == RoomType.Boss)",
            "ClearStoredCards(paelsTooth, \"act boss combat ended\")",
            "var counter = AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] + 1",
            "if (counter < 2)",
            "ChooseAndReturnStoredCard(paelsTooth)",
            "CardModel.FromSerializable(savedCard)",
            "CardSelectCmd.FromChooseABundleScreen",
            "CardCmd.Upgrade(selected, CardPreviewStyle.MessyLayout)",
            "paelsTooth.SerializableCards.Remove(selectedPreview.Saved)",
            "ClearStoredCards(paelsTooth, \"act transition\")");

        Assert.Contains("| Pael's Tooth |", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Pael's Tooth stored cards", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Stored removed cards and combat counter survive save/load.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void DebtAndFollyPlayerTextMatchSourceBehavior()
    {
        var debtSource = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "DebtAndCardPatches.cs");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var cards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");

        Assert.Equal("Debt", cards["DEBT.title"]);
        Assert.Equal("Exhaust. When Exhausted, lose 5 Gold.", cards["DEBT.description"]);
        Assert.DoesNotContain("turn", cards["DEBT.description"], StringComparison.OrdinalIgnoreCase);

        Assert.Equal("Folly", cards["FOLLY.title"]);
        Assert.Equal("Unplayable. Innate. Eternal.", cards["FOLLY.description"]);
        Assert.DoesNotContain("Ethereal", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Retain", cards["FOLLY.description"], StringComparison.OrdinalIgnoreCase);

        AssertSourceContains(
            debtSource,
            "DebtFromSavePatch",
            "__result = new CardKeyword[] { CardKeyword.Exhaust }",
            "__result = new DynamicVar[] { new GoldVar(5) }",
            "DebtTurnEndEffectPatch",
            "__result = false",
            "DebtTurnEndInHandPatch",
            "__result = Task.CompletedTask",
            "debt.ExhaustOnNextPlay = true",
            "Math.Min(5, debt.Owner.Gold)",
            "PlayerCmd.LoseGold(goldToLose, debt.Owner)");

        AssertSourceContains(
            vakuSource,
            "new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 4)",
            "AncientCardHelpers.RemoveKeywords(folly, CardKeyword.Ethereal, CardKeyword.Retain)",
            "__result = new[] { CardKeyword.Unplayable, CardKeyword.Eternal, CardKeyword.Innate }");
    }

    [Fact]
    public void TemporaryGeneratedCardPathsCleanUpOrSelfExpire()
    {
        var turnSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var vakuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            turnSource,
            "CardFactory.GetDistinctForCombat(owner, attackPool, 1, owner.RunState.Rng.CombatCardGeneration)",
            "AncientCardHelpers.ApplyTemporaryCostReduction(generated, 1)",
            "AncientCardHelpers.ApplyKeywords(generated, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(generated, PileType.Hand, owner)",
            "AncientCardHelpers.RemoveUnpiledCombatCard(generated, combatState)",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "__result = Task.CompletedTask",
            "[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))]",
            "CardSelectCmd.FromChooseACardScreen(choiceContext, new[] { topCard }, player, canSkip: true)",
            "if (selected != topCard)",
            "await CardCmd.Exhaust(choiceContext, topCard)",
            "PowerCmd.Apply<StrengthPower>");

        AssertSourceContains(
            vakuSource,
            "var copy = cardPlay.Card.CreateClone()",
            "AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1)",
            "AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust)",
            "await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner)",
            "ConditionalWeakTable<MusicBox, State>",
            "MusicBoxStateTracker.MarkUsed(musicBox)",
            "MusicBoxStateTracker.Reset(__instance)");

        Assert.Contains("skipped card does not linger", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Top draw-pile card can be exhausted for Strength or kept.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Creates a discounted Ethereal Exhaust copy.", manualMatrix, StringComparison.Ordinal);
    }

    [Fact]
    public void MeatCleaverCookRestSiteOptionIsSafeAndScoped()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "MeatCleaverCookPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var staticHovers = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");
        var zhsStaticHovers = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Equal("Adds a [gold]Cleaver[/gold] option to rest sites: remove [blue]2[/blue] cards and lose [blue]5[/blue] HP.", relics["MEAT_CLEAVER.description"]);
        Assert.Equal("Cleaver", restSite["OPTION_COOK.name"]);
        Assert.Equal("Remove 2 cards. Lose 5 HP.", restSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("Requires at least 2 removable cards and more than 5 HP.", restSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("At a [gold]Rest Site[/gold], [gold]remove[/gold] [blue]2[/blue] cards from your [gold]Deck[/gold] and lose [blue]5[/blue] HP.", staticHovers["COOK.description"]);
        Assert.Equal("Cleaver", staticHovers["COOK.title"]);
        Assert.Equal("\u5728\u4f11\u606f\u5904\u52a0\u5165[gold]\u5207\u8089[/gold]\u9009\u9879\uff1a\u79fb\u9664[blue]2[/blue]\u5f20\u724c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsRelics["MEAT_CLEAVER.description"]);
        Assert.Equal("\u5207\u8089", zhsRestSite["OPTION_COOK.name"]);
        Assert.Equal("\u79fb\u96642\u5f20\u724c\u3002\u5931\u53bb5\u70b9\u751f\u547d\u3002", zhsRestSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("\u9700\u8981\u81f3\u5c112\u5f20\u53ef\u79fb\u9664\u724c\u4e14\u751f\u547d\u503c\u5927\u4e8e5\u3002", zhsRestSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("\u5728[gold]\u4f11\u606f\u5904[/gold]\u4ece\u4f60\u7684[gold]\u724c\u7ec4[/gold]\u4e2d[gold]\u79fb\u9664[/gold][blue]2[/blue]\u5f20\u724c\uff0c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsStaticHovers["COOK.description"]);
        Assert.Equal("\u5207\u8089", zhsStaticHovers["COOK.title"]);
        Assert.DoesNotContain("gain [green]9[/green] Max HP", staticHovers["COOK.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u83b7\u5f97[green]9[/green]\u70b9\u6700\u5927\u751f\u547d", zhsStaticHovers["COOK.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Cooking", relics["MEAT_CLEAVER.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRelics["MEAT_CLEAVER.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRestSite["OPTION_COOK.name"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsStaticHovers["COOK.title"], StringComparison.Ordinal);

        AssertSourceContains(
            source,
            "public const int CardsToRemove = 2",
            "public const int HpToLose = 5",
            "owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner)",
            "__result = false",
            "if (owner.GetRelic<MeatCleaver>() == null)",
            "__instance.IsEnabled ? \"OPTION_COOK.ezDescription\" : \"OPTION_COOK.ezDescriptionDisabled\"",
            "owner.Creature.CurrentHp > HpToLose",
            "Cards.Count(card => card.IsRemovable) >= CardsToRemove",
            "Cancelable = true",
            "RequireManualConfirmation = true",
            "if (cards.Count != CardsToRemove)",
            "await CardPileCmd.RemoveFromDeck(card)",
            "await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose)");

        Assert.DoesNotContain("MaxHp", source, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when too few removable cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when HP is not greater than 5.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Expected: Cleaver / \u5207\u8089 option removes 2 removable cards and costs 5 HP.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify no other rest-site source is affected unexpectedly.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Expected: Cook option", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u70f9\u996a", manualChecklist, StringComparison.Ordinal);
    }

    private static void AssertSovereignBladeText(string value, params string[] requiredTerms)
    {
        Assert.True(CountOccurrences(value, "[blue]3[/blue]") >= 5, "Sovereign Blade text should show all five 3-point jade boons.");
        foreach (var term in requiredTerms)
        {
            Assert.Contains(term, value, StringComparison.Ordinal);
        }
    }

    private static int DistinguishedCapeLossForTest(int currentMaxHp)
    {
        var proportionalLoss = (int)Math.Ceiling(currentMaxHp * 0.30m);
        return Math.Max(proportionalLoss, 18);
    }

    private static bool CanPayDistinguishedCapeCostForTest(int currentMaxHp)
    {
        return currentMaxHp > DistinguishedCapeLossForTest(currentMaxHp);
    }

}
