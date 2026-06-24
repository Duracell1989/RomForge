using AwesomeAssertions;
using NUnit.Framework;
using RomForge.Core.Matching;
using RomForge.Core.Models;
using RomForge.Core.Operations;
using RomForge.Core.Scanning;

namespace RomForge.Core.UnitTests.Operations;

[TestOf(typeof(RomReArchiver))]
public sealed class RomReArchiverTests
{
    private static MatchResult MakeResult(
        MatchStatus status,
        string? filePath = "/roms/wrong.zip",
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
                    FileExtension = "zip",
                    RomExtension = "gba",
                },
        };

    [Test]
    public void GetReArchiveTarget_WrongArchiveType_ReturnsFromAndToPath()
    {
        MatchResult result = MakeResult(MatchStatus.WrongArchiveType);

        (string From, string To)? target = RomReArchiver.GetReArchiveTarget(result, "%u - %n");

        target.Should().NotBeNull();
        target!.Value.From.Should().Be("/roms/wrong.zip");
        target.Value.To.Should().Be("/roms/0001 - Correct Title.7z");
    }

    [TestCase(MatchStatus.Verified)]
    [TestCase(MatchStatus.Missing)]
    [TestCase(MatchStatus.IncorrectlyNamed)]
    public void GetReArchiveTarget_NotWrongArchiveType_ReturnsNull(MatchStatus status)
    {
        MatchResult result = MakeResult(status);
        RomReArchiver.GetReArchiveTarget(result, "%u - %n").Should().BeNull();
    }

    [Test]
    public void GetReArchiveTarget_EmptyMask_UsesExistingFileName()
    {
        MatchResult result = MakeResult(
            MatchStatus.WrongArchiveType,
            filePath: "/roms/My Game.zip"
        );

        (string From, string To)? target = RomReArchiver.GetReArchiveTarget(result, string.Empty);

        target.Should().NotBeNull();
        target!.Value.To.Should().Be("/roms/My Game.7z");
    }

    [Test]
    public void GetReArchiveTarget_NoScannedRom_ReturnsNull()
    {
        MatchResult result = MakeResult(MatchStatus.WrongArchiveType, filePath: null);
        RomReArchiver.GetReArchiveTarget(result, "%u - %n").Should().BeNull();
    }

    [Test]
    public void GetReArchiveTarget_DefaultExtension_OutputsSevenZ()
    {
        MatchResult result = MakeResult(MatchStatus.WrongArchiveType, filePath: "/roms/game.rar");

        (string From, string To)? target = RomReArchiver.GetReArchiveTarget(result, "%u - %n");

        target!.Value.To.Should().EndWith(".7z");
    }

    [Test]
    public void GetReArchiveTarget_ZipExtension_OutputsZip()
    {
        MatchResult result = MakeResult(MatchStatus.WrongArchiveType, filePath: "/roms/game.7z");

        (string From, string To)? target = RomReArchiver.GetReArchiveTarget(result, "%u - %n", "zip");

        target!.Value.To.Should().EndWith(".zip");
    }
}
