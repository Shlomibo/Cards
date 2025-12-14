using System;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class RemovePlayerTests : TurnsManagerTestsBase
{
    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenRemovingLowerPlayer(TurnsDirection direction)
    {
        var testSubject = GetTestSubject(direction: direction);
        int removedPlayer = (direction, testSubject.Current) switch
        {
            (TurnsDirection.Up, 0) => testSubject.ActivePlayers[^1],
            (TurnsDirection.Up, int i) => i - 1,
            (_, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
        };
        var originalPlayers = testSubject.ActivePlayers.ToArray();

        var expected = new
        {
            ActivePlayers = originalPlayers.Where(i => i != removedPlayer),
            testSubject.Current,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Next,
            Previous = (direction, testSubject.Previous) switch
            {
                (TurnsDirection.Up, 0) => testSubject.ActivePlayers[^1],
                (TurnsDirection.Up, int i) => i - 1,
                (_, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
            }
        };

        testSubject.RemovePlayer(removedPlayer);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenRemovingCurrentPlayer(TurnsDirection direction)
    {
        var testSubject = GetTestSubject(direction: direction);
        int removedPlayer = testSubject.Current;
        var originalPlayers = testSubject.ActivePlayers.ToArray();

        var expected = new
        {
            ActivePlayers = originalPlayers.Where(i => i != removedPlayer),
            Current = (direction, removedPlayer) switch
            {
                (TurnsDirection.Up, 0) => testSubject.ActivePlayers[^1],
                (TurnsDirection.Up, int i) => i - 1,
                (_, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
            },
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Next,
            Previous = (direction, testSubject.Previous) switch
            {
                (TurnsDirection.Up, 0) => testSubject.ActivePlayers[^1],
                (TurnsDirection.Up, int i) => i - 1,
                (_, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
            }
        };

        testSubject.RemovePlayer(removedPlayer);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenRemovingHigherPlayer(TurnsDirection direction)
    {
        var testSubject = GetTestSubject(direction: direction);
        int removedPlayer = (direction, testSubject.Current) switch
        {
            (TurnsDirection.Up, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
            (_, 0) => testSubject.InitialPlayersCount - 1,
            (_, int i) => i - 1,
        };
        var originalPlayers = testSubject.ActivePlayers.ToArray();

        var expected = new
        {
            ActivePlayers = originalPlayers.Where(i => i != removedPlayer),
            testSubject.Current,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = (direction, testSubject.Next) switch
            {
                (TurnsDirection.Up, int i) => (i + 1) % (testSubject.InitialPlayersCount - 1),
                (_, 0) => testSubject.InitialPlayersCount - 1,
                (_, int i) => i - 1,
            },
            testSubject.Previous,
        };

        testSubject.RemovePlayer(removedPlayer);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenRemovingAnotherPlayer(TurnsDirection direction)
    {
        var testSubject = GetTestSubject(
            playersCount: 6,
            direction: direction);
        int removedPlayer = (direction, testSubject.Current) switch
        {
            (TurnsDirection.Up, int i) => (i + 2) % (testSubject.InitialPlayersCount - 1),
            (_, 0) => testSubject.InitialPlayersCount - 2,
            (_, 1) => testSubject.InitialPlayersCount - 1,
            (_, int i) => i - 2,
        };
        var originalPlayers = testSubject.ActivePlayers.ToArray();

        var expected = new
        {
            ActivePlayers = originalPlayers.Where(i => i != removedPlayer),
            testSubject.Current,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Next,
            testSubject.Previous,
        };

        testSubject.RemovePlayer(removedPlayer);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenRemovingNegativePlayer()
    {
        var testSubject = GetTestSubject();

        testSubject.Invoking(tm => tm.RemovePlayer(-1))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void WhenRemovingNonExistingPlayer()
    {
        var testSubject = GetTestSubject();

        testSubject.Invoking(tm => tm.RemovePlayer(testSubject.InitialPlayersCount))
            .Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void WhenRemovingRemovedPlayer()
    {
        var testSubject = GetTestSubject();
        int removedPlayer = Random.Next(testSubject.InitialPlayersCount);
        testSubject.RemovePlayer(removedPlayer);

        testSubject.Invoking(tm => tm.RemovePlayer(removedPlayer))
            .Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void WhenRemovingTheLastPlayer()
    {
        var testSubject = GetTestSubject(playersCount: 1);

        testSubject.Invoking(tm => tm.RemovePlayer(0))
            .Should().Throw<InvalidOperationException>();
    }
}
