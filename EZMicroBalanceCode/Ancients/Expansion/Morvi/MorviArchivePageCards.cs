using BaseLib.Utils.Attributes;
using EZMicroBalance.EZMicroBalanceCode.Ancients;
using MegaCrit.Sts2.Core.HoverTips;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal abstract class MorviArchivePageCard : CustomCardModel
{
    private static readonly CardKeyword[] ArchivePageKeywords = [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected MorviArchivePageCard(CardType type, TargetType targetType)
        : base(0, type, CardRarity.Token, targetType, showInCardLibrary: false)
    {
    }

    public override string CustomPortraitPath => $"{MainFile.ResPath}/images/card_portraits/big/morvi_archive_pages.png";

    public override string PortraitPath => $"{MainFile.ResPath}/images/card_portraits/morvi_archive_pages.png";

    public override string BetaPortraitPath => PortraitPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => ArchivePageKeywords;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
            AncientCardHelpers.TemporaryHoverTip()
        ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public override int MaxUpgradeLevel => 0;
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveDrawPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DRAW_PAGE";

    public MorviArchiveDrawPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveVeilPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_VEIL_PAGE";

    public MorviArchiveVeilPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(14m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveBurnPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_BURN_PAGE";

    public MorviArchiveBurnPage()
        : base(CardType.Attack, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveDiscountPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE";

    public MorviArchiveDiscountPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MorviBlessingService.ArmOverdueLibraryDiscount(Owner, this);
        return Task.CompletedTask;
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveBraveryPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE";

    public MorviArchiveBraveryPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MorviBraveryPagePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Strength.BaseValue,
            Owner.Creature,
            this);
    }
}

[CustomID(CardId)]
[Pool(typeof(ColorlessCardPool))]
internal sealed class MorviArchiveDexterityPage : MorviArchivePageCard
{
    public const string CardId = "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE";

    public MorviArchiveDexterityPage()
        : base(CardType.Skill, TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MorviDexterityPagePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Dexterity.BaseValue,
            Owner.Creature,
            this);
    }
}
