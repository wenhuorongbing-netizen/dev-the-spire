using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Lotha;

internal sealed partial class EzmbLotha : ModAncientEventTemplate
{
    public override string? CustomBackgroundScenePath => LothaAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => LothaAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => LothaAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => LothaAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => LothaAssetPaths.RunHistoryIconOutline;

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(AncientDialogueLine.sfxFallbackPath),
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
            {
                [CharKey<Ironclad>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Silent>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Defect>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Necrobinder>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)],
                [CharKey<Regent>()] = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
            },
            AgnosticDialogues = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
        };
    }
}
