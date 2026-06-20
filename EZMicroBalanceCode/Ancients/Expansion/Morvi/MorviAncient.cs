using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal sealed partial class EzmbMorvi : ModAncientEventTemplate
{
    public override string? CustomBackgroundScenePath => MorviAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => MorviAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => MorviAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => MorviAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => MorviAssetPaths.RunHistoryIconOutline;

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
