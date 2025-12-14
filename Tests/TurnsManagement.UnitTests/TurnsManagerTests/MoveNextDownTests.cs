using System;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class MoveNextDownTests : TurnsManagerTestsBase
{
    [Test]
    public void WhenMovingNext()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Down,
            current: _ => 1);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 0,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = testSubject.PlayersCount - 1,
            Previous = 1,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextOnTheFirstPlayer()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Down,
            current: _ => 0);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = testSubject.PlayersCount - 1,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = testSubject.PlayersCount - 2,
            Previous = 0,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextAndThereIsOnlyOnePlayer()
    {
        var testSubject = GetTestSubject(playersCount: 1, direction: TurnsDirection.Down);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 0,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = 0,
            Previous = 0,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextAndNextPlayerLeftTheGame()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Down,
            current: _ => 1);
        testSubject.RemovePlayer(0);

        var expected = new
        {
            testSubject.ActivePlayers,
            Current = testSubject.ActivePlayers[^1],
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = testSubject.ActivePlayers[^2],
            Previous = 1,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }
}
