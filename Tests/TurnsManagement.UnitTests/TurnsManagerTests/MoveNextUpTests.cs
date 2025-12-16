using System;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class MoveNextUpTests : TurnsManagerTestsBase
{
    [Test]
    public void WhenMovingNext()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Up,
            current: _ => 0);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 1,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = 2,
            Previous = 0,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextOnTheLastPlayer()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Up,
            current: pCount => pCount - 1);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 0,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = 1,
            Previous = testSubject.InitialPlayersCount - 1,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextAndThereIsOnlyOnePlayer()
    {
        var testSubject = GetTestSubject(playersCount: 1, direction: TurnsDirection.Up);
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
    public void WhenMovingNextAndThereAreOnlyTwoPlayers()
    {
        var testSubject = GetTestSubject(playersCount: 2, direction: TurnsDirection.Up);
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = (testSubject.Current + 1) % 2,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = testSubject.Current,
            Previous = testSubject.Current,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenMovingNextAndNextPlayerLeftTheGame()
    {
        var testSubject = GetTestSubject(
            direction: TurnsDirection.Up,
            current: _ => 0);
        testSubject.RemovePlayer(1);

        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 2,
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Next = 3 >= testSubject.InitialPlayersCount
                ? 0
                : 3,
            Previous = 0,
        };

        testSubject.MoveNext();
        testSubject.Should().BeEquivalentTo(expected);
    }
}
