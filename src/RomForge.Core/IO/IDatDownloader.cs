using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;

namespace RomForge.Core.IO;

/// <summary>
/// Downloads updated DAT files and image archives.
/// </summary>
public interface IDatDownloader
{
    /// <summary>
    /// Downloads a DAT file to <paramref name="destDir"/>.
    /// </summary>
    /// <returns>The full path of the downloaded file on success.</returns>
    Task<Result<string>> DownloadDatAsync(
        string url,
        string destDir,
        string? fileName,
        IProgress<int>? progress,
        CancellationToken ct = default
    );

    /// <summary>
    /// Downloads an image archive and extracts PNGs into <paramref name="imgsDestDir"/>,
    /// preserving the ZIP's internal folder structure.
    /// </summary>
    Task<Result> DownloadImagesAsync(
        string url,
        string imgsDestDir,
        IProgress<int>? progress,
        CancellationToken ct = default
    );
}
