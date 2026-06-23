using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void PaelsToothSavedCounterAndStoredCardReturnAreGuarded()
    {
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var source = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            savedFields,
            "SavedAttachedState<PaelsTooth, int>",
            "EZMicroBalanceNonBossCombatCounter");

        AssertSourceContains(
            source,
            "AncientSavedStateFields.PaelsToothNonBossCombatCounter[paelsTooth] = 0",
            "IPatchMethod.PatchId => \"paels-tooth-after-combat-end\"",
            "ModPatchTarget(typeof(PaelsTooth), nameof(PaelsTooth.AfterCombatEnd))",
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
}
