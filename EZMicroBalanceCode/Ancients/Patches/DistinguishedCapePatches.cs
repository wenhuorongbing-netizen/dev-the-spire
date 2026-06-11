using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class DistinguishedCapeVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "distinguished-cape-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override DistinguishedCape canonical vars with HP loss and Apparition count";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(DistinguishedCape), "CanonicalVars", MethodType.Getter)];
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[]
        {
            new DynamicVar("HpPercent", 30m),
            new HpLossVar(DistinguishedCapePickupPatch.MinimumMaxHpLoss),
            new CardsVar(DistinguishedCapePickupPatch.ApparitionsToAdd)
        };
        return false;
    }
}

internal sealed class DistinguishedCapeEventOptionPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "distinguished-cape-event-option";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Replace unaffordable DistinguishedCape in Vakuu options when max HP is too low";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Vakuu), "GenerateInitialOptions")];
    [HarmonyPostfix]
    private static IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> Postfix(
        IReadOnlyList<MegaCrit.Sts2.Core.Events.EventOption> __result,
        Vakuu __instance)
    {
        var owner = __instance.Owner;
        if (owner == null || DistinguishedCapePickupPatch.CanPayMaxHpCost(owner.Creature.MaxHp))
        {
            return __result;
        }

        var options = __result.ToList();
        var capeIndex = options.FindIndex(option => option.Relic is DistinguishedCape);
        if (capeIndex < 0)
        {
            return __result;
        }

        var replacement = CreateVakuuSecondPoolReplacement(__instance, options);
        if (replacement != null)
        {
            options[capeIndex] = replacement;
            MainFile.Logger.Info(
                $"[Spire Plus] DistinguishedCape replaced in Vakuu options: current max HP {owner.Creature.MaxHp} cannot pay max HP cost {DistinguishedCapePickupPatch.CalculateMaxHpLoss(owner.Creature.MaxHp)}.");
            return options.ToArray();
        }

        options[capeIndex] = CreateLockedCapeOption(__instance, options[capeIndex], owner.Creature.MaxHp);
        MainFile.Logger.Warn(
            $"[Spire Plus] DistinguishedCape shown locked in Vakuu options: no same-pool replacement was available for current max HP {owner.Creature.MaxHp}.");
        return options.ToArray();
    }

    private static MegaCrit.Sts2.Core.Events.EventOption? CreateVakuuSecondPoolReplacement(
        Vakuu vakuu,
        IReadOnlyCollection<MegaCrit.Sts2.Core.Events.EventOption> currentOptions)
    {
        var currentKeys = currentOptions
            .Select(option => option.TextKey)
            .ToHashSet(StringComparer.Ordinal);

        var candidates = vakuu.AllPossibleOptions
            .Where(IsPayableVakuuSecondPoolOption)
            .Where(option => !currentKeys.Contains(option.TextKey))
            .ToList();

        return candidates.Count == 0
            ? null
            : vakuu.Rng.NextItem(candidates);
    }

    private static bool IsPayableVakuuSecondPoolOption(MegaCrit.Sts2.Core.Events.EventOption option)
    {
        return option.Relic is PreservedFog or SereTalon;
    }

    private static MegaCrit.Sts2.Core.Events.EventOption CreateLockedCapeOption(
        Vakuu eventModel,
        MegaCrit.Sts2.Core.Events.EventOption originalOption,
        int currentMaxHp)
    {
        var description = new LocString("relics", "DISTINGUISHED_CAPE.unpayableOption");
        description.Add("Cost", (decimal)DistinguishedCapePickupPatch.CalculateMaxHpLoss(currentMaxHp));

        var lockedOption = new MegaCrit.Sts2.Core.Events.EventOption(
            eventModel,
            null,
            originalOption.Title,
            description,
            originalOption.TextKey,
            originalOption.HoverTips);

        if (originalOption.Relic != null)
        {
            lockedOption.WithRelic(originalOption.Relic);
        }

        return lockedOption;
    }
}

internal sealed class DistinguishedCapePickupPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "distinguished-cape-pickup";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Replace DistinguishedCape obtain with max HP loss and Apparition cards";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(DistinguishedCape), nameof(DistinguishedCape.AfterObtained))];
    public const decimal MaxHpLossPercent = 0.30m;

    public const int MinimumMaxHpLoss = 18;

    public const int ApparitionsToAdd = 3;

    [HarmonyPrefix]
    private static bool Prefix(DistinguishedCape __instance, ref Task __result)
    {
        __result = LoseMaxHpAndAddApparitions(__instance);
        return false;
    }

    public static int CalculateMaxHpLoss(int currentMaxHp)
    {
        var proportionalLoss = (int)Math.Ceiling(currentMaxHp * MaxHpLossPercent);
        return Math.Max(proportionalLoss, MinimumMaxHpLoss);
    }

    public static bool CanPayMaxHpCost(int currentMaxHp)
    {
        return currentMaxHp > CalculateMaxHpLoss(currentMaxHp);
    }

    private static async Task LoseMaxHpAndAddApparitions(DistinguishedCape cape)
    {
        var creature = cape.Owner.Creature;
        var maxHpLoss = CalculateMaxHpLoss(creature.MaxHp);
        if (!CanPayMaxHpCost(creature.MaxHp))
        {
            MainFile.Logger.Warn($"[Spire Plus] DistinguishedCape blocked: current max HP {creature.MaxHp} cannot pay max HP cost {maxHpLoss}.");
            return;
        }

        var newMaxHp = creature.MaxHp - maxHpLoss;

        if (creature.CurrentHp > newMaxHp)
        {
            await CreatureCmd.SetCurrentHp(creature, newMaxHp);
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), creature, maxHpLoss, isFromCard: false);

        var results = new List<CardPileAddResult>();
        for (var i = 0; i < ApparitionsToAdd; i++)
        {
            var apparition = cape.Owner.RunState.CreateCard<Apparition>(cape.Owner);
            results.Add(await CardPileCmd.Add(apparition, PileType.Deck));
        }

        SpirePlusFeedback.PreviewDeckAdds(results, cape, 2f);
        MainFile.Logger.Info($"[Spire Plus] DistinguishedCape applied: lost {maxHpLoss} max HP and added {results.Count} Apparition card(s).");
    }
}
