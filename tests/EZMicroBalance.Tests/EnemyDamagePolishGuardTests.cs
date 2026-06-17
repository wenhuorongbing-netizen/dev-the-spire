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
            "HarmonyPatch(typeof(DecimillipedeSegment), \"get_WritheDamage\")",
            "HarmonyPatch(typeof(DecimillipedeSegment), \"get_ConstrictDamage\")",
            "HarmonyPatch(typeof(DecimillipedeSegment), \"get_BulkDamage\")",
            "HarmonyPatch(typeof(TerrorEel), \"get_CrashDamage\")",
            "HarmonyPatch(typeof(TerrorEel), \"get_ThrashDamage\")",
            "HarmonyPatch(typeof(PhantasmalGardener), \"get_BiteDamage\")",
            "HarmonyPatch(typeof(PhantasmalGardener), \"get_LashDamage\")",
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
