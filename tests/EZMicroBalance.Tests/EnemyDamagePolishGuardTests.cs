using Xunit;

namespace EZMicroBalance.Tests;

public sealed class EnemyDamagePolishGuardTests
{
    [Fact]
    public void HighPressureEliteDamagePolishPatchesSourceDamageGetters()
    {
        var patch = ReadRepoText("EZMicroBalanceCode", "Ascension", "Patches", "EnemyDamagePolishPatches.cs");

        AssertSourceContains(
            patch,
            "IPatchMethod.PatchId => \"decimillipede-writhe-damage-polish\"",
            "IPatchMethod.PatchId => \"decimillipede-constrict-damage-polish\"",
            "IPatchMethod.PatchId => \"decimillipede-bulk-damage-polish\"",
            "IPatchMethod.PatchId => \"terror-eel-crash-damage-polish\"",
            "IPatchMethod.PatchId => \"terror-eel-thrash-damage-polish\"",
            "IPatchMethod.PatchId => \"phantasmal-gardener-bite-damage-polish\"",
            "IPatchMethod.PatchId => \"phantasmal-gardener-lash-damage-polish\"",
            "ModPatchTarget(typeof(DecimillipedeSegment), \"WritheDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(DecimillipedeSegment), \"ConstrictDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(DecimillipedeSegment), \"BulkDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(TerrorEel), \"CrashDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(TerrorEel), \"ThrashDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(PhantasmalGardener), \"BiteDamage\", MethodType.Getter)",
            "ModPatchTarget(typeof(PhantasmalGardener), \"LashDamage\", MethodType.Getter)",
            "DecimillipedeWritheReduction = 2",
            "DecimillipedeConstrictReduction = 1",
            "DecimillipedeBulkReduction = 2",
            "TerrorEelCrashReduction = 2",
            "TerrorEelThrashReduction = 1",
            "PhantasmalGardenerBiteReduction = 1",
            "PhantasmalGardenerLashReduction = 1",
            "Math.Max(1, damage - reduction)");
    }

    [LocalSourceFact]
    public void HighPressureEliteDamageSourcesUseGetterIntentAndDamageValues()
    {
        var decimillipede = ReadLocalCoreText("Models", "Monsters", "DecimillipedeSegment.cs");
        var terrorEel = ReadLocalCoreText("Models", "Monsters", "TerrorEel.cs");
        var phantasmalGardener = ReadLocalCoreText("Models", "Monsters", "PhantasmalGardener.cs");

        AssertSourceContains(
            decimillipede,
            "private int WritheDamage",
            "new MultiAttackIntent(WritheDamage, 2)",
            "await DamageCmd.Attack(WritheDamage).WithHitCount(2)",
            "private int ConstrictDamage",
            "new SingleAttackIntent(ConstrictDamage)",
            "await DamageCmd.Attack(ConstrictDamage)",
            "private int BulkDamage",
            "new SingleAttackIntent(BulkDamage)",
            "await DamageCmd.Attack(BulkDamage)");

        AssertSourceContains(
            terrorEel,
            "private int CrashDamage",
            "new SingleAttackIntent(CrashDamage)",
            "await DamageCmd.Attack(CrashDamage)",
            "private int ThrashDamage",
            "new MultiAttackIntent(ThrashDamage, ThrashRepeat)",
            "await DamageCmd.Attack(ThrashDamage).WithHitCount(ThrashRepeat)");

        AssertSourceContains(
            phantasmalGardener,
            "private int BiteDamage",
            "new SingleAttackIntent(BiteDamage)",
            "await DamageCmd.Attack(BiteDamage)",
            "private int LashDamage",
            "new SingleAttackIntent(LashDamage)",
            "await DamageCmd.Attack(LashDamage)");
    }
}
