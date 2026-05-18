namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal enum FiremarkKind
{
    Might,
    Giant,
    ForgeArmor,
    ConstantHeal
}

internal enum BannerKind
{
    Vanguard,
    Shieldwall,
    BloodPrize,
    PressingLine,
    LastStand
}

internal enum DeepBranchNodeKind
{
    Risk,
    EnhancedReward
}

internal sealed class AscensionNodeMetadata
{
    public FiremarkKind? Firemark { get; set; }
    public BannerKind? Banner { get; set; }
    public BossSealDefinition? BossSeal { get; set; }
    public DeepBranchNodeKind? DeepBranch { get; set; }
    public bool IsDeepBranchEntry { get; set; }
    public bool IsBossBrand { get; set; }

    public bool HasAny =>
        Firemark.HasValue ||
        Banner.HasValue ||
        BossSeal != null ||
        DeepBranch.HasValue;
}
