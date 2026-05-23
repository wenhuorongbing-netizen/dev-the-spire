using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

[HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetIntentLabel))]
internal static class AeonglassLaserEchoIntentLabelPatch
{
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

[HarmonyPatch(typeof(MultiAttackIntent), nameof(MultiAttackIntent.GetTotalDamage))]
internal static class AeonglassLaserEchoIntentDamagePatch
{
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
