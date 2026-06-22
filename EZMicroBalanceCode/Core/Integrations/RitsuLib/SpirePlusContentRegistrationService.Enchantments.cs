using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    private static void RegisterEnchantments(ModContentPackBuilder content)
    {
        content.Enchantment<JeweledMaskFreePower>();
        content.Enchantment<UrdaTrialBranchEnchantment>();
        content.Enchantment<FissionEnchantment>();
        content.Enchantment<RoyalDecreeEnchantment>();
    }
}
