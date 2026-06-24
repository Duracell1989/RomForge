using AwesomeAssertions;
using NUnit.Framework;
using RomForge.Core.Matching;
using RomForge.Core.Models;
using RomForge.Core.Operations;
using RomForge.Core.Scanning;

namespace RomForge.Core.UnitTests.Operations;

[TestOf(typeof(RomRenamer))]
public sealed class RomRenamerTests
{
    private static MatchResult MakeResult(
        MatchStatus status,
        string? filePath = "/roms/Wrong Name.7z",
        int release = 1,
        string title = "Correct Title"
    ) =>
        new()
        {
            Game = new Game { ReleaseNumber = release, Title = title },
            Status = status,
            ScannedRom = filePath is null
                ? null
                : new ScannedRom
                {
                    FilePath = filePath,
                    FileExtension = "7z",
                    RomExtension = "gba",
                },
        };

    [Test]
    public void GetRenameTarget_IncorrectlyNamed_ReturnsFromAndToPath()
    {
        MatchResult result = MakeResult(MatchStatus.IncorrectlyNamed);

        (string From, string To)? target = RomRenamer.GetRenameTarget(result, "%u - %n");

        target.Should().NotBeNull();
        target!.Value.From.Should().Be("/roms/Wrong Name.7z");
        target.Value.To.Should().Be("/roms/0001 - Correct Title.7z");
    }

    [TestCase(MatchStatus.Verified)]
    [TestCase(MatchStatus.Missing)]
    [TestCase(MatchStatus.WrongArchiveType)]
    public void GetRenameTarget_NotIncorrectlyNamed_ReturnsNull(MatchStatus status)
    {
        MatchResult result = MakeResult(status);

        RomRenamer.GetRenameTarget(result, "%u - %n").Should().BeNull();
    }

    [Test]
    public void GetRenameTarget_EmptyMask_ReturnsNull()
    {
        MatchResult result = MakeResult(MatchStatus.IncorrectlyNamed);

        RomRenamer.GetRenameTarget(result, string.Empty).Should().BeNull();
    }

    [Test]
    public void GetRenameTarget_NoScannedRom_ReturnsNull()
    {
        MatchResult result = MakeResult(MatchStatus.IncorrectlyNamed, filePath: null);

        RomRenamer.GetRenameTarget(result, "%u - %n").Should().BeNull();
    }

    [Test]
    public void GetRenameTarget_PreservesArchiveExtension()
    {
        MatchResult result = new()
        {
            Game = new Game { ReleaseNumber = 1, Title = "Game" },
            Status = MatchStatus.IncorrectlyNamed,
            ScannedRom = new ScannedRom
            {
                FilePath = "/roms/old.zip",
                FileExtension = "zip",
                RomExtension = "gba",
            },
        };

        (string From, string To)? target = RomRenamer.GetRenameTarget(result, "%u - %n");

        target!.Value.To.Should().EndWith(".zip");
    }
}
