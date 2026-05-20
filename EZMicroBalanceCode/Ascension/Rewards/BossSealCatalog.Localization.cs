namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class BossSealCatalog
{
    public static string GetLocalizationKey(BossSealId id)
    {
        return id switch
        {
            BossSealId.HolyDaze => "BOSS_SEAL_HOLY_DAZE",
            BossSealId.MartyrOath => "BOSS_SEAL_MARTYR_OATH",
            BossSealId.InkReturn => "BOSS_SEAL_INK_RETURN",
            BossSealId.StartledShell => "BOSS_SEAL_STARTLED_SHELL",
            BossSealId.SoulTide => "BOSS_SEAL_SOUL_TIDE",
            BossSealId.BoilingCritical => "BOSS_SEAL_BOILING_CRITICAL",
            BossSealId.MisalignedShell => "BOSS_SEAL_MISALIGNED_SHELL",
            BossSealId.MarginalNote => "BOSS_SEAL_MARGINAL_NOTE",
            BossSealId.StruggleBait => "BOSS_SEAL_STRUGGLE_BAIT",
            BossSealId.ChosenDecree => "BOSS_SEAL_CHOSEN_DECREE",
            BossSealId.ResidualSample => "BOSS_SEAL_RESIDUAL_SAMPLE",
            BossSealId.AeonglassStrength => "BOSS_SEAL_AEONGLASS_STRENGTH",
            _ => "BOSS_ROYAL_SEAL"
        };
    }
}
