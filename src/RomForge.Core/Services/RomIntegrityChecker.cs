using System;
using System.Collections.Generic;
using System.IO;
using RomForge.Core.Matching;

namespace RomForge.Core.Services;

/// <summary>
/// Checks whether previously-scanned ROM files still exist on disk.
/// </summary>
public static class RomIntegrityChecker
{
    /// <summary>
    /// Returns every <see cref="MatchResult"/> whose backing file has genuinely been deleted
    /// while its containing directory is still present. Only <see cref="MatchStatus.Verified"/>
    /// results with a non-null <see cref="Core.Matching.MatchResult.ScannedRom"/> are checked.
    /// A file whose whole containing directory is missing is treated as an offline volume
    /// (e.g. an unmounted external drive), not a deletion, and is deliberately not reported.
    /// </summary>
    public static IReadOnlyList<MatchResult> FindStaleResults(IReadOnlyList<MatchResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        List<MatchResult> stale = new List<MatchResult>();
        foreach (MatchResult result in results)
        {
            if (result.Status != MatchStatus.Verified || result.ScannedRom is null)
                continue;

            string filePath = result.ScannedRom.FilePath;

            // If the containing directory is gone, the volume is almost certainly offline
            // (e.g. an external drive was unmounted) rather than the file being deleted.
            // Reporting it as stale would let the caller overwrite good scan results with
            // "Missing", losing the verified state until a full re-scan. Skip it instead.
            string? directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                continue;

            if (!File.Exists(filePath))
                stale.Add(result);
        }
        return stale;
    }
}
