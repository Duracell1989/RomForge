using System;
using System.Collections.Generic;
using System.IO;
using FluentResults;
using RomForge.Core.Models;

namespace RomForge.Core.IO;

public sealed record OfflineListConfig
{
    public string? RomFolderPath { get; init; }
    public string? ArchiveFormat { get; init; }
    public IReadOnlyList<LanguageBit> LanguageBits { get; init; } = [];
}

public static class OfflineListConfigReader
{
    public static Result<OfflineListConfig> Read(string iniPath)
    {
        try
        {
            Dictionary<string, string> options = ParseOptionSection(iniPath);

            List<LanguageBit> bits = [];
            for (int n = 1; n <= 26; n++)
            {
                if (options.TryGetValue($"l{n}", out string? raw))
                {
                    string label = raw.Trim('"').Trim();
                    if (!string.IsNullOrEmpty(label))
                        bits.Add(new LanguageBit(BitIndex: n, Label: label));
                }
            }

            options.TryGetValue("RomFolder", out string? romFolder);
            options.TryGetValue("ArchiveFormat", out string? archiveFormat);

            return Result.Ok(new OfflineListConfig
            {
                RomFolderPath = string.IsNullOrEmpty(romFolder) ? null : romFolder,
                ArchiveFormat = string.IsNullOrEmpty(archiveFormat) ? null : archiveFormat,
                LanguageBits = bits,
            });
        }
        catch (Exception ex)
        {
            return Result.Fail($"Could not read OfflineList config: {ex.Message}");
        }
    }

    private static Dictionary<string, string> ParseOptionSection(string iniPath)
    {
        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        bool inOptionSection = false;

        foreach (string rawLine in File.ReadLines(iniPath))
        {
            string line = rawLine.Trim();

            if (line.StartsWith('['))
            {
                inOptionSection = line.Equals("[Option]", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!inOptionSection || !line.Contains('='))
                continue;

            int eq = line.IndexOf('=');
            string key = line[..eq].Trim();
            string value = line[(eq + 1)..].Trim();
            result[key] = value;
        }

        return result;
    }
}
