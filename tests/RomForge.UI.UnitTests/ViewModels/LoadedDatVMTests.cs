using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;
using RomForge.Core.Matching;
using RomForge.Core.Models;
using RomForge.UI.ViewModels;

namespace RomForge.UI.UnitTests.ViewModels;

[TestOf(typeof(LoadedDatVM))]
public class LoadedDatVMTests
{
    private static DatFile MakeDat() =>
        new DatFile { Header = new DatHeader { DatName = "Test", System = "Test" }, Games = [] };

    private static LoadedDatVM MakeVm(params Game[] games)
    {
        LoadedDatVM vm = new LoadedDatVM(MakeDat(), "/test/dat.xml");
        foreach (Game g in games)
            vm.Games.Add(
                new GameRowVM(
                    new MatchResult { Game = g, Status = MatchStatus.Missing },
                    "/imgs",
                    new DatHeader(),
                    []
                )
            );
        return vm;
    }

    private static Game MakeGame(int release, string title, string? publisher = null) =>
        new Game { ReleaseNumber = release, Title = title, Publisher = publisher };

    private static LoadedDatVM MakeVmWithStatuses()
    {
        LoadedDatVM vm = new LoadedDatVM(MakeDat(), "/test/dat.xml");

        // A = Verified (no flags) → StatusSortKey 4
        vm.Games.Add(new GameRowVM(
            new MatchResult { Game = new Game { ReleaseNumber = 1, Title = "A" }, Status = MatchStatus.Verified },
            "/imgs", new DatHeader(), []
        ));
        // B = Missing → StatusSortKey 0
        vm.Games.Add(new GameRowVM(
            new MatchResult { Game = new Game { ReleaseNumber = 2, Title = "B" }, Status = MatchStatus.Missing },
            "/imgs", new DatHeader(), []
        ));
        // C = Verified + IncorrectlyNamed → StatusSortKey 3
        vm.Games.Add(new GameRowVM(
            new MatchResult
            {
                Game = new Game { ReleaseNumber = 3, Title = "C" },
                Status = MatchStatus.Verified,
                IsIncorrectlyNamed = true,
            },
            "/imgs", new DatHeader(), []
        ));
        return vm;
    }

    [Test]
    public void FilteredGames_DefaultOrder_MatchesInsertionOrder()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(3, "Zelda"),
            MakeGame(1, "Mario"),
            MakeGame(2, "Metroid")
        );

        vm.FilteredGames.Select(g => g.ReleaseNumber).Should().ContainInOrder(3, 1, 2);
    }

    [Test]
    public void SortBy_ReleaseNumber_SortsAscending()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(3, "Zelda"),
            MakeGame(1, "Mario"),
            MakeGame(2, "Metroid")
        );

        vm.SortByCommand.Execute("ReleaseNumber");

        vm.FilteredGames.Select(g => g.ReleaseNumber).Should().ContainInOrder(1, 2, 3);
    }

    [Test]
    public void SortBy_ReleaseNumberTwice_SortsDescending()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(3, "Zelda"),
            MakeGame(1, "Mario"),
            MakeGame(2, "Metroid")
        );

        vm.SortByCommand.Execute("ReleaseNumber");
        vm.SortByCommand.Execute("ReleaseNumber");

        vm.FilteredGames.Select(g => g.ReleaseNumber).Should().ContainInOrder(3, 2, 1);
    }

    [Test]
    public void SortBy_Title_SortsAlphabeticallyAscending()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(1, "Zelda"),
            MakeGame(2, "Mario"),
            MakeGame(3, "Metroid")
        );

        vm.SortByCommand.Execute("Title");

        vm.FilteredGames.Select(g => g.Title).Should().ContainInOrder("Mario", "Metroid", "Zelda");
    }

    [Test]
    public void SortBy_TitleDescending_SortsReverseAlphabetically()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(1, "Zelda"),
            MakeGame(2, "Mario"),
            MakeGame(3, "Metroid")
        );

        vm.SortByCommand.Execute("Title");
        vm.SortByCommand.Execute("Title");

        vm.FilteredGames.Select(g => g.Title).Should().ContainInOrder("Zelda", "Metroid", "Mario");
    }

    [Test]
    public void SortBy_Publisher_SortsAscending()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(1, "A", publisher: "Nintendo"),
            MakeGame(2, "B", publisher: "Capcom"),
            MakeGame(3, "C", publisher: "Acclaim")
        );

        vm.SortByCommand.Execute("Publisher");

        vm.FilteredGames.Select(g => g.Publisher).Should().ContainInOrder("Acclaim", "Capcom", "Nintendo");
    }

    [Test]
    public void SortBy_Status_SortsAscendingByPriority()
    {
        // Insertion order: A=Verified(4), B=Missing(0), C=IncorrectlyNamed(3)
        // Ascending by StatusSortKey: B(0), C(3), A(4)
        LoadedDatVM vm = MakeVmWithStatuses();

        vm.SortByCommand.Execute("Status");

        vm.FilteredGames.Select(g => g.Title).Should().ContainInOrder("B", "C", "A");
    }

    [Test]
    public void SortBy_DifferentColumn_ResetsToAscending()
    {
        LoadedDatVM vm = MakeVm(
            MakeGame(3, "Zelda"),
            MakeGame(1, "Mario"),
            MakeGame(2, "Metroid")
        );

        vm.SortByCommand.Execute("ReleaseNumber");
        vm.SortByCommand.Execute("ReleaseNumber");
        vm.SortByCommand.Execute("Title");

        vm.FilteredGames.Select(g => g.Title).Should().ContainInOrder("Mario", "Metroid", "Zelda");
    }

    [Test]
    public void SortBy_ReleaseNumber_UpdatesSortIndicator()
    {
        LoadedDatVM vm = MakeVm(MakeGame(1, "A"));

        vm.ReleaseNumberSortIndicator.Should().Be(string.Empty);

        vm.SortByCommand.Execute("ReleaseNumber");
        vm.ReleaseNumberSortIndicator.Should().Be(" ▲");
        vm.TitleSortIndicator.Should().Be(string.Empty);

        vm.SortByCommand.Execute("ReleaseNumber");
        vm.ReleaseNumberSortIndicator.Should().Be(" ▼");
    }

    [Test]
    public void SortBy_PreservesActiveFilter()
    {
        LoadedDatVM vm = new LoadedDatVM(MakeDat(), "/test/dat.xml");
        vm.Games.Add(new GameRowVM(
            new MatchResult { Game = new Game { ReleaseNumber = 3, Title = "Zelda" }, Status = MatchStatus.Verified },
            "/imgs", new DatHeader(), []
        ));
        vm.Games.Add(new GameRowVM(
            new MatchResult { Game = new Game { ReleaseNumber = 1, Title = "Mario" }, Status = MatchStatus.Missing },
            "/imgs", new DatHeader(), []
        ));
        vm.Games.Add(new GameRowVM(
            new MatchResult { Game = new Game { ReleaseNumber = 2, Title = "Metroid" }, Status = MatchStatus.Verified },
            "/imgs", new DatHeader(), []
        ));
        vm.ShowMissing = false;

        vm.SortByCommand.Execute("ReleaseNumber");

        vm.FilteredGames.Should().HaveCount(2);
        vm.FilteredGames.Select(g => g.ReleaseNumber).Should().ContainInOrder(2, 3);
    }
}
