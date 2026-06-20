namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed class UrdaTrialBranchEnchantment : ModEnchantmentTemplate, ILocalizationProvider
{
    private const string CombatsLeftVar = "CombatsLeft";
    private const string PlayedThisCombatVar = "PlayedThisCombat";
    private const string PlaysLeftVar = "PlaysLeft";

    public override bool HasExtraCardText => true;

    public override bool ShowAmount => true;

    public override int DisplayAmount => DynamicVars[CombatsLeftVar].IntValue;

    public override string? CustomIconPath => UrdaAssetPaths.TrialBranchOptionIcon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(CombatsLeftVar, 3m),
        new IntVar(PlayedThisCombatVar, 0m),
        new IntVar(PlaysLeftVar, 3m)
    ];

    public List<(string, string)>? Localization => LocManager.Instance.Language == "zhs"
        ? new CardModifierLoc(
            "试炼枝条",
            "接下来 [blue]{CombatsLeft}[/blue] 场战斗中，每场都要打出这张牌。当前战斗：[blue]{PlayedThisCombat}[/blue]/[blue]1[/blue]；还需成功打出 [blue]{PlaysLeft}[/blue] 次。漏掉任意一场会移除它。",
            "[gold]试炼枝条[/gold]：接下来 [blue]{CombatsLeft}[/blue] 场每场都要打出。当前战斗 [blue]{PlayedThisCombat}[/blue]/[blue]1[/blue]；还需 [blue]{PlaysLeft}[/blue] 次。漏掉会移除。")
        : new CardModifierLoc(
            "Trial Branch",
            "Play this card in every one of the next [blue]{CombatsLeft}[/blue] combats. Current combat: [blue]{PlayedThisCombat}[/blue]/[blue]1[/blue]; [blue]{PlaysLeft}[/blue] successful plays remain. Missing any combat removes it.",
            "[gold]Trial Branch[/gold]: play this in every one of the next [blue]{CombatsLeft}[/blue] combats. Current combat [blue]{PlayedThisCombat}[/blue]/[blue]1[/blue]; [blue]{PlaysLeft}[/blue] plays remain. Missing a combat removes it.");

    public override bool CanEnchantCardType(CardType cardType) =>
        cardType is CardType.Attack or CardType.Skill or CardType.Power;

    public void SetProgress(int combatsLeft, int playedThisCombat, int playsLeft)
    {
        DynamicVars[CombatsLeftVar].BaseValue = Math.Max(0, combatsLeft);
        DynamicVars[PlayedThisCombatVar].BaseValue = Math.Clamp(playedThisCombat, 0, 1);
        DynamicVars[PlaysLeftVar].BaseValue = Math.Max(0, playsLeft);
    }
}
