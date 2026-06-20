using Godot;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class EzmbVakuuTrialMonster : ModMonsterTemplate
{
    public const string MonsterId = "EZMB_VAKUU_TRIAL_MONSTER";

    private const string OpeningOfferMoveId = "OPENING_OFFER_MOVE";
    private const string KnifeRainMoveId = "KNIFE_RAIN_MOVE";
    private const string GildedHideMoveId = "GILDED_HIDE_MOVE";
    private const string DebtCallMoveId = "DEBT_CALL_MOVE";
    private const float VisualScale = 1.25f;

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 176, 160);

    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 184, 168);

    public override string? CustomVisualsPath => VakuuFightAssetPaths.MonsterVisual;

    public override bool HasDeathSfx => false;

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

    private int OpeningOfferDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

    private int KnifeRainDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

    private int DebtCallDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 26);

    private int GildedHideBlock => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 28, 24);

    protected override NCreatureVisuals? TryCreateCreatureVisuals()
    {
        var texture = ResourceLoader.Load<Texture2D>(VakuuFightAssetPaths.MonsterVisual);
        var visuals = RitsuGodotNodeFactories.CreateFromResource<NCreatureVisuals>(texture);
        if (visuals is null)
        {
            return null;
        }

        ResizeImageVisuals(visuals);
        return visuals;
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await VakuuFightService.EnsureStolenVaultPower(Creature);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var openingOffer = new MoveState(
            OpeningOfferMoveId,
            OpeningOfferMove,
            new SingleAttackIntent(OpeningOfferDamage),
            new DebuffIntent());
        var knifeRain = new MoveState(
            KnifeRainMoveId,
            KnifeRainMove,
            new MultiAttackIntent(KnifeRainDamage, 3));
        var gildedHide = new MoveState(
            GildedHideMoveId,
            GildedHideMove,
            new DefendIntent(),
            new BuffIntent());
        var debtCall = new MoveState(
            DebtCallMoveId,
            DebtCallMove,
            new SingleAttackIntent(DebtCallDamage),
            new DebuffIntent(strong: true));

        openingOffer.FollowUpState = knifeRain;
        knifeRain.FollowUpState = gildedHide;
        gildedHide.FollowUpState = debtCall;
        debtCall.FollowUpState = knifeRain;

        return new MonsterMoveStateMachine(
            [openingOffer, knifeRain, gildedHide, debtCall],
            openingOffer);
    }

    private static void ResizeImageVisuals(NCreatureVisuals visuals)
    {
        var body = visuals.GetNode<Node2D>("%Visuals");
        var bounds = visuals.GetNode<Control>("%Bounds");

        body.Scale = Vector2.One * VisualScale;
        bounds.Position *= VisualScale;
        bounds.Size *= VisualScale;

        if (visuals.HasNode("%CenterPos"))
        {
            visuals.GetNode<Marker2D>("%CenterPos").Position =
                bounds.Position + bounds.Size * new Vector2(0.5f, 0.62f);
        }

        if (visuals.HasNode("%IntentPos"))
        {
            visuals.GetNode<Marker2D>("%IntentPos").Position =
                bounds.Position + bounds.Size * new Vector2(0.5f, 0.0f) + new Vector2(0.0f, -70.0f);
        }

        if (visuals.HasNode("%TalkPos"))
        {
            visuals.GetNode<Marker2D>("%TalkPos").Position =
                bounds.Position + bounds.Size * new Vector2(0.5f, 0.35f);
        }
    }

    private async Task OpeningOfferMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(OpeningOfferDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 0.15f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, 2m, Creature, null);
    }

    private async Task KnifeRainMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(KnifeRainDamage)
            .WithHitCount(3)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 0.1f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task GildedHideMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.15f);
        await CreatureCmd.GainBlock(Creature, GildedHideBlock, ValueProp.Move, null, fast: true);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, 2m, Creature, null);
    }

    private async Task DebtCallMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(DebtCallDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 0.2f)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, 2m, Creature, null);
    }
}
