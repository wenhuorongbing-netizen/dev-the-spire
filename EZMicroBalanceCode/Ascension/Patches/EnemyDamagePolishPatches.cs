namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

using MegaCrit.Sts2.Core.Models.Monsters;
using STS2RitsuLib.Patching.Models;

internal static class EnemyDamagePolish
{
    public const int DecimillipedeWritheReduction = 2;
    public const int DecimillipedeConstrictReduction = 1;
    public const int DecimillipedeBulkReduction = 2;
    public const int TerrorEelCrashReduction = 2;
    public const int TerrorEelThrashReduction = 1;
    public const int PhantasmalGardenerBiteReduction = 1;
    public const int PhantasmalGardenerLashReduction = 1;

    public static void ReduceDamage(ref int damage, int reduction)
    {
        damage = Math.Max(1, damage - reduction);
    }
}

internal sealed class DecimillipedeWritheDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "decimillipede-writhe-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Decimillipede Writhe damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(DecimillipedeSegment), "WritheDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.DecimillipedeWritheReduction);
    }
}

internal sealed class DecimillipedeConstrictDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "decimillipede-constrict-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Decimillipede Constrict damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(DecimillipedeSegment), "ConstrictDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.DecimillipedeConstrictReduction);
    }
}

internal sealed class DecimillipedeBulkDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "decimillipede-bulk-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Decimillipede Bulk damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(DecimillipedeSegment), "BulkDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.DecimillipedeBulkReduction);
    }
}

internal sealed class TerrorEelCrashDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "terror-eel-crash-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Terror Eel Crash damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(TerrorEel), "CrashDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.TerrorEelCrashReduction);
    }
}

internal sealed class TerrorEelThrashDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "terror-eel-thrash-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Terror Eel Thrash damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(TerrorEel), "ThrashDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.TerrorEelThrashReduction);
    }
}

internal sealed class PhantasmalGardenerBiteDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "phantasmal-gardener-bite-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Phantasmal Gardener Bite damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PhantasmalGardener), "BiteDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.PhantasmalGardenerBiteReduction);
    }
}

internal sealed class PhantasmalGardenerLashDamagePolishPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "phantasmal-gardener-lash-damage-polish";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reduce Phantasmal Gardener Lash damage and matching intent value";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PhantasmalGardener), "LashDamage", MethodType.Getter)];

    private static void Postfix(ref int __result)
    {
        EnemyDamagePolish.ReduceDamage(ref __result, EnemyDamagePolish.PhantasmalGardenerLashReduction);
    }
}
