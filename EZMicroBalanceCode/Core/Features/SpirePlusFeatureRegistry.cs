using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Features;

internal static class SpirePlusFeatureRegistry
{
    public static FeatureRegistry CreateDefault() =>
        new FeatureRegistry(
            message => MainFile.Logger.Info(message),
            message => MainFile.Logger.Warn(message))
            .Register(new LothaFeatureModule())
            .Register(new MorviFeatureModule())
            .Register(new UrdaFeatureModule())
            .Register(new VakuuFightFeatureModule())
            .Register(new AscensionFeatureModule());
}
