using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionV2MilestoneGuardTests
{
    [LocalSourceFact]
    public void CoreBossSealIntentAndMultiplayerPowerSourcesKeepExpectedShapes()
    {
        var attackIntentSource = ReadLocalCoreText("MonsterMoves", "Intents", "AttackIntent.cs");
        var kinBossSource = ReadLocalCoreText("Models", "Encounters", "TheKinBoss.cs");
        var kinPriestSource = ReadLocalCoreText("Models", "Monsters", "KinPriest.cs");
        var slipperyPowerSource = ReadLocalCoreText("Models", "Powers", "SlipperyPower.cs");
        var platingPowerSource = ReadLocalCoreText("Models", "Powers", "PlatingPower.cs");

        AssertSourceContains(
            attackIntentSource,
            "Hook.ModifyDamage(",
            "ValueProp.Move",
            "ModifyDamageHookType.All");
        AssertSourceContains(
            kinBossSource,
            "new string[3] { \"slot1\", \"slot2\", \"leaderSlot\" }",
            "(kinFollower, \"slot1\")",
            "(ModelDb.Monster<KinFollower>().ToMutable(), \"slot2\")",
            "(ModelDb.Monster<KinPriest>().ToMutable(), \"leaderSlot\")");
        Assert.DoesNotContain("Summon", kinBossSource + kinPriestSource, StringComparison.OrdinalIgnoreCase);
        AssertSourceContains(
            slipperyPowerSource,
            "public override bool ShouldScaleInMultiplayer => true",
            "return amount * (decimal)combatState.Players.Count");
        AssertSourceContains(
            platingPowerSource,
            "public override bool ShouldScaleInMultiplayer => true",
            "base.DynamicVars[\"Decrement\"].BaseValue = base.Owner.CombatState.RunState.Players.Count",
            "return (decimal)((combatState.Players.Count - 1) * 2 + 1) * amount");
    }

    [Fact]
    public void SplitBossSealPowerFilesKeepBehaviorAndReadableLocalization()
    {
        var basePower = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BossSealPower.cs");
        var markerPowers = ReadBossSealMarkerPowerSources();
        var combatStart = ReadRepoText("EZMicroBalanceCode", "Ascension", "Combat", "AscensionCombatModifierService.BossSeals.CombatStart.cs");
        var holyDaze = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "HolyDazePower.cs");
        var boilingCritical = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "BoilingCriticalPower.cs");
        var residualSample = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "ResidualSamplePower.cs");
        var aeonglassHourglass = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "AeonglassHourglassPower.cs");
        var martyrOath = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MartyrOathPowers.cs");
        var misalignedShell = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MisalignedShellPowers.cs");
        var marginalNote = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "MarginalNotePowers.cs");
        var chosenDecree = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "ChosenDecreePowers.cs");
        var aeonglassRuntime = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "AeonglassHourglassRuntimePowers.cs");
        var testSubjectSamples = ReadRepoText("EZMicroBalanceCode", "Ascension", "Powers", "TestSubjectSamplePowers.cs");
        var bossSealSharedPowers = string.Join(Environment.NewLine, basePower, markerPowers);

        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ascension", "Powers", "BossSealPowers.cs");
        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ascension", "Powers", "BossSealMarkerPowers.cs");
        AssertSourceContains(
            basePower,
            "internal abstract class BossSealPower : ModPowerTemplate, ILocalizationProvider",
            "public override PowerType Type => PowerType.Buff",
            "public override PowerStackType StackType => PowerStackType.Single",
            "public override int DisplayAmount => Amount",
            "protected virtual BossSealId? SealId => null",
            "AscensionAssetPaths.GetBossSealIndicator(id)",
            "public override string CustomIconPath => BossSealIconPath",
            "public override string CustomBigIconPath => BossSealIconPath",
            "AscensionAssetPaths.BossSealIndicator");
        AssertSourceContains(
            markerPowers,
            "internal abstract class BossSealMarkerPower : BossSealPower",
            "public override int DisplayAmount => 0",
            "HolyDazeBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.HolyDaze",
            "MartyrOathBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MartyrOath",
            "InkReturnBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.InkReturn",
            "StartledShellBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.StartledShell",
            "SoulTideBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.SoulTide",
            "BoilingCriticalBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.BoilingCritical",
            "MisalignedShellBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MisalignedShell",
            "MarginalNoteBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.MarginalNote",
            "StruggleBaitBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.StruggleBait",
            "AeonglassHourglassBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.AeonglassHourglass",
            "ChosenDecreeBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.ChosenDecree",
            "ResidualSampleBossSealMarkerPower",
            "protected override BossSealId? SealId => BossSealId.ResidualSample",
            "Dedicated Ability");
        AssertSourceContains(
            combatStart,
            "await ApplyBossSealVisibilityMarker(combatState, definition)",
            "FindBossSealVisibilityOwner(combatState, definition.Id)",
            "PowerCmd.Apply<HolyDazeBossSealMarkerPower>",
            "PowerCmd.Apply<MartyrOathBossSealMarkerPower>",
            "PowerCmd.Apply<InkReturnBossSealMarkerPower>",
            "PowerCmd.Apply<StartledShellBossSealMarkerPower>",
            "PowerCmd.Apply<SoulTideBossSealMarkerPower>",
            "PowerCmd.Apply<BoilingCriticalBossSealMarkerPower>",
            "PowerCmd.Apply<MisalignedShellBossSealMarkerPower>",
            "PowerCmd.Apply<MarginalNoteBossSealMarkerPower>",
            "PowerCmd.Apply<StruggleBaitBossSealMarkerPower>",
            "PowerCmd.Apply<AeonglassHourglassBossSealMarkerPower>",
            "PowerCmd.Apply<ChosenDecreeBossSealMarkerPower>",
            "PowerCmd.Apply<ResidualSampleBossSealMarkerPower>");
        Assert.DoesNotContain("internal sealed class HolyDazePower", bossSealSharedPowers, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class BoilingCriticalPower", bossSealSharedPowers, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class ResidualSamplePower", bossSealSharedPowers, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class ChosenDecreeReductionPower", bossSealSharedPowers, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class AeonglassHourglassPower", bossSealSharedPowers, StringComparison.Ordinal);
        foreach (var runtimePower in new[]
                 {
                     "internal sealed class MartyrOathPower",
                     "internal sealed class KaiserCalibrationPower",
                     "internal sealed class DeepThoughtPower",
                     "internal sealed class RoyalMajestyPower",
                     "internal sealed class AeonglassLaserEchoPower",
                     "internal abstract class TestSubjectSamplePower"
                 })
        {
            Assert.DoesNotContain(runtimePower, bossSealSharedPowers, StringComparison.Ordinal);
        }

        AssertSourceContains(
            holyDaze,
            "internal sealed class HolyDazePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.HolyDaze",
            "PowerStackType.Counter",
            "\"圣昏\"",
            "每次受到的伤害最多为[blue]1[/blue]",
            "受击最多[blue]1[/blue]点",
            "\"Holy Daze\"",
            "damage taken from each hit is capped at [blue]1[/blue]",
            "Damage taken is capped at [blue]1[/blue].",
            "ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)",
            "return target == Owner ? 1m : decimal.MaxValue;");

        AssertSourceContains(
            boilingCritical,
            "internal sealed class BoilingCriticalPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.BoilingCritical",
            "\"不可削弱\"",
            "爆发回合",
            "[gold]虚弱[/gold]",
            "[gold]易伤[/gold]",
            "[gold]人工制品[/gold]",
            "\"Unweakenable\"",
            "On the explosion turn",
            "[gold]Weak[/gold]",
            "[gold]Vulnerable[/gold]",
            "[gold]Artifact[/gold]",
            "public override int DisplayAmount => 0");

        AssertSourceContains(
            residualSample,
            "internal sealed class ResidualSamplePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.ResidualSample",
            "\"实验记录\"",
            "下个阶段会保留[blue]{Amount}[/blue]份[gold]残留样本[/gold]",
            "复苏后结算残留样本",
            "\"Experimental Record\"",
            "The next phase keeps [blue]{Amount}[/blue] [gold]Residual Sample[/gold]",
            "Residual samples resolve after respawn.",
            "ShouldPowerBeRemovedAfterOwnerDeath()",
            "return false;");

        AssertSourceContains(
            aeonglassHourglass,
            "internal sealed class AeonglassHourglassPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.AeonglassHourglass",
            "\"时砂回流\"",
            "剩余[blue]{Amount}[/blue]枚时砂",
            "每花费[blue]1[/blue]点能量",
            "\"Time Sand Reflow\"",
            "[blue]{Amount}[/blue] Time Sand remaining",
            "Each energy spent removes [blue]1[/blue]",
            "Eye Lasers hits [blue]1[/blue] extra time");

        AssertSourceContains(
            martyrOath,
            "internal sealed class MartyrOathPower : BossSealPower",
            "internal sealed class MartyrOathStrikePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.MartyrOath",
            "ModifyPowerAmountGivenAdditive",
            "AfterModifyingPowerAmountGiven",
            "ModifyDamageAdditive",
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)");

        AssertSourceContains(
            misalignedShell,
            "internal sealed class KaiserCalibrationPower : BossSealPower",
            "internal sealed class KaiserCalibrationStrikePower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.MisalignedShell",
            "ModifyDamageAdditive",
            "AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)");

        AssertSourceContains(
            marginalNote,
            "internal sealed class DeepThoughtPower : BossSealPower",
            "internal sealed class DeepThoughtCostTaxPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.MarginalNote",
            "DisintegrationPower => amount + Amount",
            "CardPileCmd.AddGeneratedCardToCombat",
            "PlayerCmd.LoseEnergy(sideCostLayers, player)",
            "TryModifyEnergyCostInCombat");

        AssertSourceContains(
            chosenDecree,
            "internal sealed class RoyalMajestyPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.ChosenDecree",
            "private int LayersToSpend => Math.Min(Amount, 2)",
            "ModifyBlockAdditive",
            "AfterModifyingBlockAmount");

        AssertSourceContains(
            aeonglassRuntime,
            "internal sealed class AeonglassLaserEchoPower : BossSealPower",
            "internal sealed class AeonglassPendingWitherPower : BossSealPower",
            "internal sealed class AeonglassLaserEchoUseCounterPower : BossSealPower",
            "protected override BossSealId? SealId => BossSealId.AeonglassHourglass",
            "ModifyAttackHitCount",
            "Owner.Monster is Aeonglass",
            "protected override bool IsVisibleInternal => false");

        AssertSourceContains(
            testSubjectSamples,
            "internal abstract class TestSubjectSamplePower : BossSealPower",
            "internal sealed class TestSubjectSkillAdaptationPower : TestSubjectSamplePower",
            "internal sealed class TestSubjectAttackAdaptationPower : TestSubjectSamplePower",
            "internal sealed class TestSubjectAntibodySamplePower : TestSubjectSamplePower",
            "internal sealed class TestSubjectContaminatedSamplePower : TestSubjectSamplePower",
            "Owner.Monster is not TestSubject",
            "ApplyFinalArtifact",
            "AfterShuffle",
            "CardPileCmd.AddGeneratedCardToCombat");

        foreach (var source in new[] { basePower, markerPowers, holyDaze, boilingCritical, residualSample, aeonglassHourglass })
        {
            AssertNoMojibake(source, "鐏", "鎴", "绗", "鍥", "浼", "澶", "銆", "闂", "鏈", "寮€", "鑾", "缂", "锟", "铏", "鐑", "杈", "绉", "灞");
        }
    }
}
