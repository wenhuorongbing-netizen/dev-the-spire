using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using STS2RitsuLib.Scaffolding.Content;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

internal static partial class SpirePlusContentRegistrationService
{
    private static void RegisterAncients(ModContentPackBuilder content)
    {
        content.SharedAncient<EzmbUrda>();
        content.SharedAncient<EzmbMorvi>();
        content.SharedAncient<EzmbLotha>();
    }

    private static void RegisterVakuuEncounter(ModContentPackBuilder content)
    {
        content.Monster<EzmbVakuuTrialMonster>();
        content.GlobalEncounter<EzmbVakuuTrialEncounter>();
    }
}
