namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using MegaCrit.Sts2.Core.Models.Monsters;

internal static class EnemyDamagePolish
{
    public const int DecimillipedeWritheReduction = 2;
    public const int DecimillipedeConstrictReduction = 1;
    public const int TerrorEelCrashReduction = 2;
    public const int TerrorEelThrashReduction = 1;
    public const int PhantasmalGardenerBiteReduction = 1;
    public const int PhantasmalGardenerLashReduction = 1;

    public static void ReduceDamage(ref int damage, int reduction)
    {
        damage = Math.Max(1, damage - reduction);
    }
}

[HarmonyPatch(typeof(DecimillipedeSegment), "get_WritheDamage")]
internal static class DecimillipedeWritheDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.DecimillipedeWritheReduction);
    }
}

[HarmonyPatch(typeof(DecimillipedeSegment), "get_ConstrictDamage")]
internal static class DecimillipedeConstrictDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.DecimillipedeConstrictReduction);
    }
}

[HarmonyPatch(typeof(TerrorEel), "get_CrashDamage")]
internal static class TerrorEelCrashDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.TerrorEelCrashReduction);
    }
}

[HarmonyPatch(typeof(TerrorEel), "get_ThrashDamage")]
internal static class TerrorEelThrashDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.TerrorEelThrashReduction);
    }
}

[HarmonyPatch(typeof(PhantasmalGardener), "get_BiteDamage")]
internal static class PhantasmalGardenerBiteDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.PhantasmalGardenerBiteReduction);
    }
}

[HarmonyPatch(typeof(PhantasmalGardener), "get_LashDamage")]
internal static class PhantasmalGardenerLashDamagePolishPatch
{
    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.PhantasmalGardenerLashReduction);
    }
}
