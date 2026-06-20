using MegaCrit.Sts2.Core.Models.CardPools;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

public sealed class Root : RootFamilyCard
{
    public const string CardId = "EZMB_ROOT";

    public Root()
        : base(2, rootblightLevel: 1, showInCardLibrary: true)
    {
    }
}

public sealed class DeepRoot : RootFamilyCard
{
    public const string CardId = "EZMB_DEEP_ROOT";

    public DeepRoot()
        : base(3, rootblightLevel: 2, showInCardLibrary: true)
    {
    }
}

public sealed class RootblightIII : RootFamilyCard
{
    public const string CardId = "EZMB_ROOTBLIGHT_III";

    public RootblightIII()
        : base(4, rootblightLevel: 3, showInCardLibrary: true)
    {
    }
}
