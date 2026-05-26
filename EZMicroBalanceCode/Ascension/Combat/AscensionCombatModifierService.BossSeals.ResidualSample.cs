using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static async Task TrackResidualSamplePhase(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature creature)
    {
        if (creature.Monster is not TestSubject ||
            !creature.HasPower<AdaptablePower>())
        {
            return;
        }

        tracker.TestSubjectPhaseChanges++;
        var samples = ChooseResidualSamples(creature, tracker, metadata);
        if (samples.Contains(TestSubjectSampleKind.StrengthResidue))
        {
            var cap = tracker.TestSubjectPhaseChanges >= 2 ? 6m : 3m;
            tracker.PendingTestSubjectStrengthResidue = Math.Min(cap, Math.Ceiling(creature.GetPowerAmount<StrengthPower>() * 0.3m));
        }

        tracker.PendingTestSubjectSamples.AddRange(samples);
        tracker.TestSubjectAttackCardsThisPhase = 0;
        tracker.TestSubjectSkillCardsThisPhase = 0;
        tracker.TestSubjectDebuffAppliedThisPhase = false;
        MainFile.Logger.Info($"[Spire Plus] Ascension A19 applied: Experimental Record retained {samples.Count} sample(s) for the next phase.");
        await Task.CompletedTask;
    }

    private static IReadOnlyList<TestSubjectSampleKind> ChooseResidualSamples(
        Creature subject,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        var ordered = new List<TestSubjectSampleKind>();
        if (subject.GetPowerAmount<StrengthPower>() > 0)
        {
            ordered.Add(TestSubjectSampleKind.StrengthResidue);
        }

        if (tracker.TestSubjectSkillCardsThisPhase > tracker.TestSubjectAttackCardsThisPhase)
        {
            ordered.Add(TestSubjectSampleKind.SkillAdaptation);
        }

        if (tracker.TestSubjectAttackCardsThisPhase > 0)
        {
            ordered.Add(TestSubjectSampleKind.AttackAdaptation);
        }

        if (tracker.TestSubjectDebuffAppliedThisPhase)
        {
            ordered.Add(TestSubjectSampleKind.AntibodySample);
        }

        ordered.Add(TestSubjectSampleKind.ContaminatedSample);
        var count = metadata.IsBossBrand ? 2 : 1;
        return ordered.Distinct().Take(count).ToList();
    }

    private static async Task TryApplyResidualSamples(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata)
    {
        if (metadata.BossSeal?.Id != BossSealId.ResidualSample ||
            tracker.PendingTestSubjectSamples.Count == 0)
        {
            return;
        }

        var subject = AliveEnemies(combatState).FirstOrDefault(enemy => enemy.Monster is TestSubject);
        if (subject == null || subject.IsDead)
        {
            return;
        }

        var samples = tracker.PendingTestSubjectSamples.Distinct().ToList();
        foreach (var sample in samples)
        {
            await ApplyResidualSample(combatState, tracker, metadata, subject, sample);
        }

        PlayResidualSampleNotice(subject, samples);
        tracker.PendingTestSubjectSamples.Clear();
    }

    private static void PlayResidualSampleNotice(Creature subject, IReadOnlyList<TestSubjectSampleKind> samples)
    {
        if (samples.Count == 0 || subject.IsDead)
        {
            return;
        }

        var line = new LocString("ascension", "BOSS_SEAL_RESIDUAL_SAMPLE_NOTICE");
        line.Add("Samples", string.Join(" / ", samples.Select(SampleName).Select(loc => loc.GetFormattedText())));
        var reasonSeparator = LocManager.Instance.Language == "zhs" ? "；" : "; ";
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

    private static async Task ApplyResidualSample(
        CombatState combatState,
        AscensionCombatTracker tracker,
        AscensionNodeMetadata metadata,
        Creature subject,
        TestSubjectSampleKind sample)
    {
        switch (sample)
        {
            case TestSubjectSampleKind.StrengthResidue:
                var retainedStrength = tracker.PendingTestSubjectStrengthResidue;
                if (retainedStrength > 0m)
                {
                    await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), subject, retainedStrength, subject, null);
                }

                tracker.PendingTestSubjectStrengthResidue = 0m;
                break;
            case TestSubjectSampleKind.SkillAdaptation:
                await PowerCmd.Apply<TestSubjectSkillAdaptationPower>(new BlockingPlayerChoiceContext(), subject, 1m, subject, null);
                break;
            case TestSubjectSampleKind.AttackAdaptation:
                await PowerCmd.Apply<TestSubjectAttackAdaptationPower>(new BlockingPlayerChoiceContext(), subject, 1m, subject, null);
                break;
            case TestSubjectSampleKind.AntibodySample:
                await PowerCmd.Apply<TestSubjectAntibodySamplePower>(new BlockingPlayerChoiceContext(), subject, 1m, subject, null);
                break;
            case TestSubjectSampleKind.ContaminatedSample:
                await PowerCmd.Apply<TestSubjectContaminatedSamplePower>(new BlockingPlayerChoiceContext(), subject, 1m, subject, null);
                break;
        }
    }

    private static void TrackResidualSampleCardPlayed(AscensionCombatTracker tracker, CardModel card)
    {
        if (card.Owner?.IsActiveForHooks != true)
        {
            return;
        }

        if (card.Type == CardType.Attack)
        {
            tracker.TestSubjectAttackCardsThisPhase++;
        }
        else if (card.Type == CardType.Skill)
        {
            tracker.TestSubjectSkillCardsThisPhase++;
        }
    }
}
