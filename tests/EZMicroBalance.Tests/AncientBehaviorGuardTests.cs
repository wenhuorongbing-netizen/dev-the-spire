using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
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
                id.GetString() == "STS2-RitsuLib" &&
                dependency.TryGetProperty("min_version", out var minVersion) &&
                minVersion.GetString() == "0.4.29");

        var readme = ReadZipText(archive, "EZMicroBalance/README_INSTALL.txt");
        Assert.Contains("Spire Plus manual-test package", readme, StringComparison.Ordinal);
        Assert.Contains($"Archive: {CurrentPackageName()}.zip", readme, StringComparison.Ordinal);
        Assert.Contains("Display name: Spire Plus", readme, StringComparison.Ordinal);
        Assert.Contains("Technical compatibility id: EZMicroBalance", readme, StringComparison.Ordinal);
        Assert.Contains("Extract this archive into the Slay the Spire 2 mods folder exactly as packaged.", readme, StringComparison.Ordinal);
        Assert.Contains("If the game's Mods list shows EZMicroBalance as the mod name, the package is stale or the display-name route regressed.", readme, StringComparison.Ordinal);
        Assert.Contains("STS2-RitsuLib", readme, StringComparison.Ordinal);
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

}
