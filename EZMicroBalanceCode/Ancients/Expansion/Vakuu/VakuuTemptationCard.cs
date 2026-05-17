using BaseLib.Utils.Attributes;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal abstract class VakuuContractCard : CustomCardModel
{
    private static readonly CardKeyword[] ContractKeywords = [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected VakuuContractCard()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.None, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/vakuu_temptation.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/vakuu_temptation.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => ContractKeywords;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            HoverTipFactory.FromPower<VakuuStolenVaultPower>(),
            HoverTipFactory.FromPower<VakuuBloodDebtPower>()
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;

    protected async Task SignContract(PlayerChoiceContext choiceContext, decimal hpLoss)
    {
        await VakuuFightService.SignContract(choiceContext, Owner, this, hpLoss);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class VakuuKnifeContract : VakuuContractCard
{
    public const string CardId = "EZMB_VAKUU_KNIFE_CONTRACT";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar("Damage", 22m, ValueProp.Move), new IntVar("HpLoss", 4m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await SignContract(choiceContext, DynamicVars.HpLoss.BaseValue);

        var target = VakuuFightService.FindVakuuCreature(CombatState);
        if (target == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class VakuuTemptation : VakuuContractCard
{
    public const string CardId = "EZMB_VAKUU_TEMPTATION";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new IntVar("Energy", 2m),
            new IntVar("Cards", 2m),
            new IntVar("HpLoss", 5m)
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await SignContract(choiceContext, DynamicVars.HpLoss.BaseValue);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class VakuuShelterContract : VakuuContractCard
{
    public const string CardId = "EZMB_VAKUU_SHELTER_CONTRACT";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar("Block", 24m, ValueProp.Move), new IntVar("HpLoss", 3m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await SignContract(choiceContext, DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}
