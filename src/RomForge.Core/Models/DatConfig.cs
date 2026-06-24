using System.Collections.Generic;

namespace RomForge.Core.Models;

public sealed record DatConfig
{
    public string? RomFolderPath { get; init; }
    public string ArchiveFormat { get; init; } = "7z";
    public List<LanguageBit> LanguageBits { get; init; } = [];
}
