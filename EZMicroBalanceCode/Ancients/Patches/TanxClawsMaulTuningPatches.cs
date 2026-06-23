using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class TanxClawsMaulTuningPatches : IPatchMethod
{
    static string IPatchMethod.PatchId => "tanx-claws-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Replace Tanx Claws pickup with upgraded Maul transformations";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(Claws), nameof(Claws.AfterObtained))];

    [HarmonyPrefix]
    private static bool Prefix(Claws __instance, ref Task __result)
    {
        __result = TransformIntoUpgradedMauls(__instance);
        return false;
    }

    private static async Task TransformIntoUpgradedMauls(Claws claws)
    {
        var owner = claws.Owner;
        if (owner == null)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("relics", "CLAWS.selectionScreenPrompt"), 0, claws.DynamicVars.Cards.IntValue)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        var originals = await CardSelectCmd.FromDeckForTransformation(
            owner,
            prefs,
            original => new CardTransformation(original, CreateMaulFromOriginal(owner, original, forPreview: true)));
        var transformations = originals
            .Select(original => new CardTransformation(original, CreateMaulFromOriginal(owner, original, forPreview: false)))
            .ToList();

        await CardCmd.Transform(transformations, owner.PlayerRng.Transformations);
        MainFile.Logger.Info($"[Spire Plus] Tanx Claws applied: transformed {transformations.Count} card(s) into upgraded Maul.");
    }

    private static CardModel CreateMaulFromOriginal(Player owner, CardModel original, bool forPreview)
    {
        var maul = forPreview
            ? ModelDb.Card<Maul>().ToMutable()
            : owner.RunState.CreateCard<Maul>(owner);

        // Tanx Claws should always create the threatening version of Maul,
        // regardless of whether the original deck card was already upgraded.
        if (maul.IsUpgradable && !maul.IsUpgraded)
        {
            if (forPreview)
            {
                maul.UpgradeInternal();
            }
            else
            {
                CardCmd.Upgrade(maul, CardPreviewStyle.None);
            }
        }

        if (original.Enchantment != null)
        {
            var enchantment = (EnchantmentModel)original.Enchantment.MutableClone();
            if (enchantment.CanEnchant(maul))
            {
                if (forPreview)
                {
                    maul.EnchantInternal(enchantment, enchantment.Amount);
                    enchantment.ModifyCard();
                }
                else
                {
                    CardCmd.Enchant(enchantment, maul, enchantment.Amount);
                }
            }
        }

        return maul;
    }
}
