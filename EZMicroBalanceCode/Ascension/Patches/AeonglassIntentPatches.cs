using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AeonglassLaserEchoIntentLabelPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "aeonglass-laser-echo-intent-label";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Show Aeonglass Laser Echo's extra hit in the multi-attack intent label";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))];

    [HarmonyPrefix]
    private static bool Prefix(MultiAttackIntent __instance, IEnumerable<Creature> targets, Creature owner, ref LocString __result)
    {
        if (!ShouldShowExtraHit(owner))
        {
            return true;
        }

        var label = new LocString("intents", "FORMAT_DAMAGE_MULTI");
        label.Add("Damage", __instance.GetSingleDamage(targets, owner));
        label.Add("Repeat", __instance.Repeats + 1);
        __result = label;
        return false;
    }

    private static bool ShouldShowExtraHit(Creature owner) =>
        owner.Monster is Aeonglass &&
        owner.HasPower<AeonglassLaserEchoPower>();
}

internal sealed class AeonglassLaserEchoIntentDamagePatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "aeonglass-laser-echo-intent-damage";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Include Aeonglass Laser Echo's extra hit in total intent damage";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))];

    [HarmonyPrefix]
    private static bool Prefix(MultiAttackIntent __instance, IEnumerable<Creature> targets, Creature owner, ref int __result)
    {
        if (owner.Monster is not Aeonglass ||
            !owner.HasPower<AeonglassLaserEchoPower>())
        {
            return true;
        }

        __result = __instance.GetSingleDamage(targets, owner) * (__instance.Repeats + 1);
        return false;
    }
}
