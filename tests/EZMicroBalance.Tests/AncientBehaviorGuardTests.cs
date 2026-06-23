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
                minVersion.GetString() == "0.4.34");

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
        Assert.DoesNotContain("Found 16 PreviousSavedStates", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void PatchTargetsAreDeclaredForImplementedAncientSurfaces()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode", "Ancients");

        AssertSourceContains(
            allSource,
            "IPatchMethod.PatchId => \"paels-horn-after-obtained\"",
            "ModPatchTarget(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.AfterObtained))",
            "ModPatchTarget(typeof(RelicCmd), nameof(RelicCmd.Obtain)",
            "IPatchMethod.PatchId => \"sozu-initial-potion-gate\"",
            "ModPatchTarget(typeof(Sozu), nameof(Sozu.ShouldProcurePotion))",
            "IPatchMethod.PatchId => \"ectoplasm-initial-gold-gate\"",
            "ModPatchTarget(typeof(Ectoplasm), nameof(Ectoplasm.ModifyGoldGained))",
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
            "ModPatchTarget(typeof(VelvetChoker), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(VelvetChoker), \"DisplayAmount\", MethodType.Getter)",
            "ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.ShouldPlay))",
            "ModPatchTarget(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))",
            "ModPatchTarget(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.SpendResources))",
            "ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCardPlayed))",
            "ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.BeforeSideTurnStart))",
            "ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterRoomEntered))",
            "ModPatchTarget(typeof(VelvetChoker), nameof(VelvetChoker.AfterCombatEnd))",
            "IPatchMethod.PatchId => \"paels-tooth-after-obtained\"",
            "ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterObtained))",
            "IPatchMethod.PatchId => \"paels-tooth-after-combat-end\"",
            "ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))",
            "IPatchMethod.PatchId => \"sovereign-blade-forge-exhaust\"",
            "ModPatchTarget(typeof(ForgeCmd), nameof(ForgeCmd.Forge))",
            "IPatchMethod.PatchId => \"sovereign-blade-on-play-jade-boons\"",
            "ModPatchTarget(typeof(SovereignBlade), \"OnPlay\")",
            "IPatchMethod.PatchId => \"paels-tooth-act-transition\"",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.AfterActEntered))",
            "ModPatchTarget(typeof(AbstractModel), nameof(AbstractModel.BeforeSideTurnStart))",
            "ModPatchTarget(typeof(Crossbow), nameof(Crossbow.AfterSideTurnStart)",
            "IPatchMethod.PatchId => \"toasty-mittens-before-hand-draw\"",
            "ModPatchTarget(typeof(ToastyMittens), nameof(ToastyMittens.BeforeHandDraw))",
            "IPatchMethod.PatchId => \"whispering-earring-auto-pre-play\"",
            "ModPatchTarget(typeof(WhisperingEarring), nameof(WhisperingEarring.AfterAutoPrePlayPhaseEnteredLate))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))",
            "ModPatchTarget(typeof(CookRestSiteOption), \"IsEnabled\", MethodType.Getter)",
            "ModPatchTarget(typeof(CookRestSiteOption), \"Description\", MethodType.Getter)",
            "ModPatchTarget(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))",
            "IPatchMethod.PatchId => \"jewelry-box-after-obtained\"",
            "ModPatchTarget(typeof(JewelryBox), nameof(JewelryBox.AfterObtained))",
            "IPatchMethod.PatchId => \"jewelry-box-apotheosis-keywords\"",
            "ModPatchTarget(typeof(Apotheosis), \"CanonicalKeywords\", MethodType.Getter)",
            "ModPatchTarget(typeof(JewelryBox), \"ExtraHoverTips\", MethodType.Getter)",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)",
            "ModPatchTarget(typeof(RelicModel), nameof(RelicModel.HoverTipsExcludingRelic), MethodType.Getter)",
            "IPatchMethod.PatchId => \"preserved-fog-after-obtained\"",
            "ModPatchTarget(typeof(PreservedFog), nameof(PreservedFog.AfterObtained))",
            "IPatchMethod.PatchId => \"preserved-fog-folly-keywords\"",
            "ModPatchTarget(typeof(Folly), \"CanonicalKeywords\", MethodType.Getter)",
            "ModPatchTarget(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))",
            "IPatchMethod.PatchId => \"jeweled-mask-combat-start\"",
            "ModPatchTarget(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))",
            "ModPatchTarget(typeof(Fiddle), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ModifyHandDrawLate))",
            "ModPatchTarget(typeof(Fiddle), nameof(Fiddle.ShouldDraw))",
            "ModPatchTarget(typeof(CardPileCmd), nameof(CardPileCmd.Draw)",
            "ModPatchTarget(typeof(IronClub), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(BrilliantScarf), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(BeautifulBracelet), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))",
            "ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))",
            "ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))",
            "ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))",
            "ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.CanonicalKeywords), MethodType.Getter)",
            "ModPatchTarget(typeof(BrightestFlame), \"CanonicalVars\", MethodType.Getter)",
            "ModPatchTarget(typeof(CardModel), nameof(CardModel.OnPlayWrapper))");
    }

}
