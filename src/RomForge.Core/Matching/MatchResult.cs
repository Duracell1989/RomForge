using RomForge.Core.Models;
using RomForge.Core.Scanning;

namespace RomForge.Core.Matching;

public sealed class MatchResult
{
    public required Game Game { get; init; }
    public required MatchStatus Status { get; init; }
    public ScannedRom? ScannedRom { get; init; }
}
