using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseCoverageGuardTests
{
    [Fact]
    public void ActiveLocalizationJsonFilesAllParse()
    {
        var localizationFiles = Directory.GetFiles(RepoPath("EZMicroBalance", "localization"), "*.json", SearchOption.AllDirectories);
        var failures = new List<string>();

        foreach (var file in localizationFiles)
        {
            try
            {
                JsonDocument.Parse(File.ReadAllText(file, Encoding.UTF8)).Dispose();
            }
            catch (JsonException ex)
            {
                failures.Add($"{ToRepoRelativePath(file)}: {ex.Message}");
            }
        }

        Assert.True(failures.Count == 0, "Invalid localization JSON:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void SourceDeclaredCustomLocalizationKeysExistInEnglishAndSimplifiedChinese()
    {
        var allSource = ReadSourceTree("EZMicroBalanceCode");
        var failures = new List<string>();

        foreach (var match in Regex.Matches(allSource, @"new\s+LocString\(\s*""(?<table>[^""]+)""\s*,\s*""(?<key>[^""]+)""").Cast<Match>())
        {
            var table = match.Groups["table"].Value;
            var key = match.Groups["key"].Value;
            if (table == "intents")
            {
                continue;
            }

            var english = JsonStringMap("EZMicroBalance", "localization", "eng", $"{table}.json");
            var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", $"{table}.json");
            if (!english.ContainsKey(key) || !simplifiedChinese.ContainsKey(key))
            {
                failures.Add($"{table}:{key}");
            }
        }

        foreach (var id in Regex.Matches(allSource, @"public\s+const\s+string\s+CardId\s*=\s*""(?<id>EZMB_[^""]+)""").Cast<Match>().Select(match => match.Groups["id"].Value))
        {
            var english = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
            var simplifiedChinese = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
            foreach (var suffix in new[] { "title", "description" })
            {
                var key = $"{id}.{suffix}";
                if (!english.ContainsKey(key) || !simplifiedChinese.ContainsKey(key))
                {
                    failures.Add($"cards:{key}");
                }
            }
        }

        Assert.True(failures.Count == 0, "Missing active localization keys:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }
}
