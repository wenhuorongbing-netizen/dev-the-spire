using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionFeatureGuardTests
{
    [Fact]
    public void BossSealCatalogAvoidsHardRuntimeReferencesToOptionalEarlyAccessBossTypes()
    {
        var bossSealSource = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var combatService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Combat");

        AssertSourceContains(
            bossSealSource,
            "private const string EncounterCategory = \"ENCOUNTER\"",
            "private static ModelId EncounterId(string entry)",
            "EncounterId(\"AEONGLASS_BOSS\")",
            "EncounterId(\"QUEEN_BOSS\")",
            "EncounterId(\"TEST_SUBJECT_BOSS\")");

        Assert.DoesNotContain("using MegaCrit.Sts2.Core.Models.Encounters", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("ModelDb.GetId<", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("DoormakerBoss", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("QueenBoss", bossSealSource, StringComparison.Ordinal);
        Assert.DoesNotContain("TestSubjectBoss", bossSealSource, StringComparison.Ordinal);

        AssertSourceContains(
            combatService,
            "BossSealId.AeonglassHourglass",
            "enemy.Monster is Aeonglass",
            "tracker.AeonglassTimeSand = metadata.IsBossBrand ? 3 : 2",
            "PowerCmd.Apply<AeonglassHourglassPower>",
            "TrackAeonglassEnergySpent",
            "SettleAeonglassTimeSand",
            "tracker.AeonglassExtraWitherFromSands",
            "INCREASING_INTENSITY_MOVE",
            "CardPileCmd.AddToCombatAndPreview<Wither>",
            "PowerCmd.Apply<AeonglassLaserEchoPower>",
            "Time Sand Reflow created");

        Assert.DoesNotContain(
            "var boss = AliveEnemies(combatState)\n                .OrderByDescending(enemy => enemy.MaxHp)\n                .FirstOrDefault();",
            combatService.Replace("\r\n", "\n"),
            StringComparison.Ordinal);
        Assert.DoesNotContain("enemy.Monster is Doormaker", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<HungerPower>", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<ScrutinyPower>", combatService, StringComparison.Ordinal);
        Assert.DoesNotContain("HasPower<GraspPower>", combatService, StringComparison.Ordinal);
    }
}
