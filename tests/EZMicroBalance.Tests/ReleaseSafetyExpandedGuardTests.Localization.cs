using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseSafetyExpandedGuardTests
{
    [Fact]
    public void SimplifiedChineseLocalizationContainsNoVisibleAsciiWords()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var failures = new List<string>();

        foreach (var file in ActiveSimplifiedChineseLocalizationFiles(zhsRoot))
        {
            var relativePath = ToRepoRelativePath(file);
            using var document = JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8));
            foreach (var (key, value) in JsonStringValues(document.RootElement))
            {
                if ((relativePath.EndsWith("EZMicroBalance/localization/zhs/settings_ui.json", StringComparison.Ordinal) ||
                     relativePath.EndsWith("EZMicroBalance/localization/settings_ui/zhs.json", StringComparison.Ordinal)) &&
                    (key == "EZMICROBALANCE.mod_title" || key == "SPIREPLUS.mod_title"))
                {
                    continue;
                }

                var visibleValue = RemoveLocalizationPlaceholders(value);
                visibleValue = Regex.Replace(visibleValue, @"\[(?:/)?[A-Za-z][^\]]*\]", string.Empty, RegexOptions.CultureInvariant);
                foreach (Match match in Regex.Matches(visibleValue, @"[A-Za-z][A-Za-z0-9_-]*", RegexOptions.CultureInvariant))
                {
                    if (match.Value is "I" or "II" or "III")
                    {
                        continue;
                    }

                    failures.Add($"{relativePath}:{key} contains raw ASCII word `{match.Value}` in `{value}`");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private static string RemoveLocalizationPlaceholders(string value)
    {
        var previous = value;
        while (true)
        {
            var next = Regex.Replace(previous, @"\{[^{}]*\}", string.Empty, RegexOptions.CultureInvariant);
            if (next == previous)
            {
                return next;
            }

            previous = next;
        }
    }

    [Fact]
    public void SimplifiedChineseLocalizationContainsNoKnownMojibakeFragments()
    {
        var zhsRoot = RepoPath("EZMicroBalance", "localization", "zhs");
        var allText = string.Join(
            Environment.NewLine,
            ActiveSimplifiedChineseLocalizationFiles(zhsRoot)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => File.ReadAllText(path, Encoding.UTF8)));

        var fragments = new[]
        {
            "\uFFFD",
            "\u6D93",
            "\u9470",
            "\u7487",
            "\u941C",
            "\u940F",
            "\u95BB",
            "\u5A11",
            "\u934B",
            "\u5A75",
            "\u951F",
            "\u59AB",
            "\u951B",
            "\u947E",
            "\u93B5",
            "\u95B2",
            "\u7039",
            "\u7EC1",
            "\u93C0",
            "\u7481",
            "\u934A",
            "\u6769",
            "\u9410",
            "\u5BEE\u509D\u58CA",
            "\u9417",
            "\u93B0"
        };
        var matches = fragments
            .Where(fragment => allText.Contains(fragment, StringComparison.Ordinal))
            .ToArray();

        Assert.True(matches.Length == 0, "Found mojibake fragments in active zhs localization: " + string.Join(", ", matches));
    }

    private static IEnumerable<string> ActiveSimplifiedChineseLocalizationFiles(string zhsRoot)
    {
        var localizationRoot = RepoPath("EZMicroBalance", "localization");
        return Directory.GetFiles(zhsRoot, "*.json", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(localizationRoot, "zhs.json", SearchOption.AllDirectories))
            .Distinct(StringComparer.Ordinal);
    }
}
