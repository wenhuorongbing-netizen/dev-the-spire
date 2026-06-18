using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
{
    [Fact]
    public void ActiveCurrentDocsInlineCodeBackticksAreBalancedOutsideFences()
    {
        var docs = Directory.GetFiles(RepoPath("docs"), "*.md", SearchOption.AllDirectories)
            .Where(path => !ToRepoRelativePath(path).StartsWith("docs/archive/", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var failures = new List<string>();

        foreach (var path in docs)
        {
            var inFence = false;
            var lineNumber = 0;
            foreach (var line in File.ReadLines(path, Encoding.UTF8))
            {
                lineNumber++;
                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    inFence = !inFence;
                    continue;
                }

                if (inFence)
                {
                    continue;
                }

                var tickCount = line.Count(ch => ch == '`');
                if (tickCount % 2 != 0)
                {
                    failures.Add($"{ToRepoRelativePath(path)}:{lineNumber} has unbalanced inline backticks: {line}");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private static IEnumerable<(string Key, string Value)> ActiveAncientValues(IEnumerable<IReadOnlyDictionary<string, string>> maps)
    {
        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (key.Contains("EZMB_URDA", StringComparison.Ordinal) ||
                    key.Contains("EZMB_MORVI", StringComparison.Ordinal) ||
                    key.Contains("EZMB_LOTHA", StringComparison.Ordinal) ||
                    key.Contains("EZMB_VAKUU", StringComparison.Ordinal) ||
                    key.Contains("URDA_", StringComparison.Ordinal) ||
                    key.Contains("MORVI_", StringComparison.Ordinal) ||
                    key.Contains("LOTHA_", StringComparison.Ordinal) ||
                    key.Contains("VAKUU_", StringComparison.Ordinal) ||
                    key.Contains("VAKUU.pages.INITIAL.options.ezmb_vakuu_fight", StringComparison.Ordinal))
                {
                    yield return (key, value);
                }
            }
        }
    }

    private static IEnumerable<string> LegacyUrdaOptionRelicKeys()
    {
        yield return "EZMICROBALANCE-UrdaHumusPactOptionRelic";
        yield return "EZMICROBALANCE-UrdaMoltingOptionRelic";
        yield return "EZMICROBALANCE-UrdaMossMapOptionRelic";
        yield return "EZMICROBALANCE-UrdaSeedbedOptionRelic";
    }

    private static IEnumerable<string> CanonicalOptionRelicKeys()
    {
        foreach (var key in new[]
        {
            "URDA_HUMUS_PACT",
            "URDA_MOLTING",
            "URDA_MOSS_MAP",
            "URDA_SEEDBED",
            "URDA_TRIAL_BRANCH",
            "URDA_SHALLOW_ROOT_RELIC",
            "URDA_ROOTED_ROUTE",
            "URDA_AFTER_RAIN",
            "URDA_ROOT_SIGHT",
            "URDA_SEED_BANK",
            "MORVI_FORBIDDEN_LOAN",
            "MORVI_MISPRINT_PRESS",
            "MORVI_RED_INK_OVERDRAFT",
            "MORVI_OVERDUE_LIBRARY",
            "MORVI_OPEN_BOOK_EXAM",
            "MORVI_PAPERSTORM",
            "MORVI_BLUEPRINT_PROOF",
            "MORVI_DEBT_SETTLEMENT",
            "LOTHA_MIRROR_REBUTTAL",
            "LOTHA_MIRROR_HALL_ECHO",
            "LOTHA_PRESUMPTION",
            "LOTHA_CLOSED_COURT",
            "LOTHA_DEFERRED_VERDICT",
            "LOTHA_DEATH_REPRIEVE",
            "LOTHA_SINGLE_SENTENCE",
            "LOTHA_PUBLIC_EVIDENCE",
            "VAKUU_FIGHT"
        })
        {
            yield return $"EZMICROBALANCE-{key}_OPTION_RELIC";
        }
    }

    private static void AssertNonEmpty(
        IReadOnlyDictionary<string, string> eng,
        IReadOnlyDictionary<string, string> zhs,
        string key)
    {
        Assert.True(eng.TryGetValue(key, out var engValue), $"Missing English localization key: {key}");
        Assert.True(zhs.TryGetValue(key, out var zhsValue), $"Missing zhs localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(engValue), $"Empty English localization key: {key}");
        Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs localization key: {key}");
    }

    private static string[] DynamicVariableNames(string value) =>
        Regex.Matches(value, @"\{(?<name>[A-Za-z0-9_]+)(?::[^}]*)?\}")
            .Select(match => match.Groups["name"].Value)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

    private static void AssertBalancedRichTextTags(string fileName, string key, string value)
    {
        foreach (var tag in new[] { "blue", "gold" })
        {
            var open = Regex.Matches(value, $@"\[{tag}\]").Count;
            var close = Regex.Matches(value, $@"\[/{tag}\]").Count;

            Assert.True(
                open == close,
                $"{fileName}:{key} has unbalanced [{tag}] rich-text tags.");
        }
    }

}
