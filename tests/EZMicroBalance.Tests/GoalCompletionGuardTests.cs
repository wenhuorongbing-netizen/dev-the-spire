using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class GoalCompletionGuardTests
{
    private static readonly string[] GuardedDocs =
    [
        "docs/goal.md",
        "docs/review.md",
        "docs/issues.md"
    ];

    private static readonly string[] FalseCompletionClaims =
    [
        "goal completed",
        "goal 已完成",
        "release-ready",
        "full multiplayer support",
        "feature complete",
        "fully implemented"
    ];

    [Fact]
    public void GoalReviewAndIssuesDoNotContainUnguardedCompletionClaims()
    {
        var offenders = GuardedDocs
            .SelectMany(path => ReadNonFencedLines(path)
                .Where(line => ContainsFalseCompletionClaim(line.Text) && !IsGuardedNegativeOrInstructionalLine(line.Text))
                .Select(line => $"{path}:{line.Number}: {line.Text}"))
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "False completion or release-ready claims must stay blocked while live/manual gates are pending:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders));
    }

    [Fact]
    public void PendingReleaseTraceabilityBlocksReleaseReadyClaims()
    {
        var traceability = ReadRepoText("docs", "specs", "release-traceability-matrix.md");
        var issues = ReadRepoText("docs", "issues.md");
        var pendingGateTerms = new[]
        {
            "pending",
            "Manual-test candidate",
            "Development-test surface",
            "Hidden by default",
            "Do not advertise full support",
            "Manual Proof Gates"
        };

        var pendingTerms = pendingGateTerms
            .Where(term =>
                traceability.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                issues.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.NotEmpty(pendingTerms);

        var releaseReadyOffenders = new[] { "docs/review.md", "docs/issues.md" }
            .SelectMany(path => ReadNonFencedLines(path)
                .Where(line => ContainsReleaseReadyClaim(line.Text) && !IsGuardedNegativeOrInstructionalLine(line.Text))
                .Select(line => $"{path}:{line.Number}: {line.Text}"))
            .ToArray();

        Assert.True(
            releaseReadyOffenders.Length == 0,
            "release-ready claims are forbidden while release traceability or issues still contain pending/manual gates:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, releaseReadyOffenders));
    }

    [Fact]
    public void GoalKeepsLiveProofAndRuntimeRowClosureRules()
    {
        var goal = ReadRepoText("docs", "goal.md");

        Assert.Contains("live proof required", goal, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("source review", goal, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("runtime rows", goal, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("No release-ready claim is made", goal, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("live loader, clicked UI, save/load, Vakuu, co-op, and Future Peek live proof", goal, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ManualProofGatesInIssuesBlockCurrentReleaseReadyLanguage()
    {
        var issues = ReadRepoText("docs", "issues.md");
        Assert.Contains("## Manual Proof Gates", issues, StringComparison.Ordinal);

        var offenders = ReadNonFencedLines("docs/issues.md")
            .Where(line => Regex.IsMatch(line.Text, @"\b(current|currently|now|ready)\b.*\brelease-ready\b|\brelease-ready\b.*\b(current|currently|now|ready)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            .Where(line => !IsGuardedNegativeOrInstructionalLine(line.Text))
            .Select(line => $"docs/issues.md:{line.Number}: {line.Text}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            "docs/issues.md cannot claim current release-ready status while Manual Proof Gates remain:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, offenders));
    }

    private static bool ContainsFalseCompletionClaim(string line) =>
        FalseCompletionClaims.Any(claim => line.Contains(claim, StringComparison.OrdinalIgnoreCase));

    private static bool ContainsReleaseReadyClaim(string line) =>
        line.Contains("release-ready", StringComparison.OrdinalIgnoreCase);

    private static bool IsGuardedNegativeOrInstructionalLine(string line)
    {
        var normalized = line.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return true;
        }

        var guardedFragments = new[]
        {
            "not ",
            "no ",
            "cannot",
            "can't",
            "must not",
            "do not",
            "does not",
            "forbid",
            "forbidden",
            "reject",
            "rejected",
            "pending",
            "missing",
            "unless",
            "without",
            "fails",
            "block",
            "guard",
            "禁止",
            "不能",
            "不得",
            "不允许",
            "除非",
            "没有",
            "仍",
            "未"
        };

        return guardedFragments.Any(fragment => normalized.Contains(fragment, StringComparison.Ordinal));
    }

    private static IEnumerable<(int Number, string Text)> ReadNonFencedLines(string repoRelativePath)
    {
        var inFence = false;
        var lines = ReadRepoText(repoRelativePath.Split('/')).Replace("\r\n", "\n").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
            {
                inFence = !inFence;
                continue;
            }

            if (!inFence)
            {
                yield return (i + 1, line);
            }
        }
    }
}
