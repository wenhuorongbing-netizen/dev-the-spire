using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static void PlayResidualSampleNotice(Creature subject, IReadOnlyList<TestSubjectSampleKind> samples)
    {
        if (samples.Count == 0 || subject.IsDead)
        {
            return;
        }

        var line = new LocString("ascension", "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE");
        line.Add("Samples", string.Join(" / ", samples.Select(SampleName).Select(loc => loc.GetFormattedText())));
        var reasonSeparator = LocManager.Instance.Language == "zhs" ? "\uFF1B" : "; ";
        line.Add("Reason", string.Join(reasonSeparator, samples.Select(SampleReason).Select(loc => loc.GetFormattedText())));
        TalkCmd.Play(line, subject, VfxColor.Purple, VfxDuration.Long);
    }

    private static LocString SampleName(TestSubjectSampleKind sample) =>
        new("ascension", sample switch
        {
            TestSubjectSampleKind.StrengthResidue => "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH",
            TestSubjectSampleKind.SkillAdaptation => "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL",
            TestSubjectSampleKind.AttackAdaptation => "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK",
            TestSubjectSampleKind.AntibodySample => "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY",
            TestSubjectSampleKind.ContaminatedSample => "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED",
            _ => "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED"
        });

    private static LocString SampleReason(TestSubjectSampleKind sample) =>
        new("ascension", sample switch
        {
            TestSubjectSampleKind.StrengthResidue => "BOSS_SEAL_RESIDUAL_SAMPLE_STRENGTH.reason",
            TestSubjectSampleKind.SkillAdaptation => "BOSS_SEAL_RESIDUAL_SAMPLE_SKILL.reason",
            TestSubjectSampleKind.AttackAdaptation => "BOSS_SEAL_RESIDUAL_SAMPLE_ATTACK.reason",
            TestSubjectSampleKind.AntibodySample => "BOSS_SEAL_RESIDUAL_SAMPLE_ANTIBODY.reason",
            TestSubjectSampleKind.ContaminatedSample => "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED.reason",
            _ => "BOSS_SEAL_RESIDUAL_SAMPLE_CONTAMINATED.reason"
        });
}
