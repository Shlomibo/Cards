using System;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class SettingCurrentPlayerTests : TurnsManagerTestsBase
{
    [Test]
    public void WhenSettingCurrentPlayerToTheCurrentPlayer()
    {
        var testSubject = GetTestSubject();
        var expected = new
        {
            testSubject.ActivePlayers,
            testSubject.Current,
            testSubject.Direction,
            testSubject.Next,
            testSubject.InitialPlayersCount,
            testSubject.Previous,
        };

        testSubject.Current = testSubject.Current;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToAnotherPlayer()
    {
        var testSubject = GetTestSubject();
        int setTo = Random.Next(testSubject.PlayersCount - 1) switch
        {
            int i when i < testSubject.Current => i,
            int i => i + 1,
        };
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = setTo,
            testSubject.Direction,
            Next = setTo switch
            {
                int i when i == testSubject.PlayersCount - 1 => 0,
                int i => i + 1,
            },
            testSubject.InitialPlayersCount,
            Previous = setTo switch
            {
                0 => testSubject.PlayersCount - 1,
                int i => i - 1,
            },
        };

        testSubject.Current = setTo;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToZeroAnotherPlayer()
    {
        var testSubject = GetTestSubject(
            current: pCount => Random.Next(1, pCount));
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = 0,
            testSubject.Direction,
            Next = 1,
            testSubject.InitialPlayersCount,
            Previous = testSubject.InitialPlayersCount - 1,
        };

        testSubject.Current = 0;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToTheLastPlayer()
    {
        var testSubject = GetTestSubject(
            current: pCount => Random.Next(pCount - 1));
        var expected = new
        {
            testSubject.ActivePlayers,
            Current = testSubject.InitialPlayersCount - 1,
            testSubject.Direction,
            Next = 0,
            testSubject.InitialPlayersCount,
            Previous = testSubject.InitialPlayersCount - 2,
        };

        testSubject.Current = testSubject.InitialPlayersCount - 1;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToTheLastPlayerBeforeAPlayerThatLeft()
    {
        var testSubject = GetTestSubject();
        int leavingPlayer = Random.Next(testSubject.InitialPlayersCount);
        testSubject.RemovePlayer(leavingPlayer);

        var expected = new
        {
            testSubject.ActivePlayers,
            Current = leavingPlayer switch
            {
                0 => testSubject.ActivePlayers[^1],
                int i => i - 1,
            },
            testSubject.Direction,
            Next = leavingPlayer switch
            {
                int i when i == testSubject.PlayersCount => 0,
                int i => i + 1,
            },
            testSubject.InitialPlayersCount,
            Previous = leavingPlayer switch
            {
                0 => testSubject.ActivePlayers[^2],
                1 => testSubject.ActivePlayers[^1],
                int i => i - 2,
            },
        };

        testSubject.Current = expected.Current;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToTheFirstPlayerAfterAPlayerThatLeft()
    {
        var testSubject = GetTestSubject();
        int leavingPlayer = Random.Next(testSubject.InitialPlayersCount);
        testSubject.RemovePlayer(leavingPlayer);

        var expected = new
        {
            testSubject.ActivePlayers,
            Current = leavingPlayer switch
            {
                int i when i == testSubject.PlayersCount => 0,
                int i => i + 1,
            },
            testSubject.Direction,
            Next = leavingPlayer switch
            {
                int i when i == testSubject.PlayersCount - 1 => 0,
                int i when i == testSubject.PlayersCount => 1,
                int i => i + 2,
            },
            testSubject.InitialPlayersCount,
            Previous = leavingPlayer switch
            {
                0 => testSubject.ActivePlayers[^1],
                int i => i - 1,
            },
        };

        testSubject.Current = expected.Current;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingCurrentPlayerToNegative()
    {
        var testSubject = GetTestSubject();

        testSubject.Invoking(tm => tm.Current = -1)
            .Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void WhenSettingCurrentPlayerToPlayersCount()
    {
        var testSubject = GetTestSubject();

        testSubject.Invoking(tm => tm.Current = tm.InitialPlayersCount)
            .Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void WhenSettingCurrentPlayerToAPlayersThatLeft()
    {
        var testSubject = GetTestSubject();
        int leavingPlayer = Random.Next(testSubject.InitialPlayersCount);
        testSubject.RemovePlayer(leavingPlayer);

        testSubject.Invoking(tm => tm.Current = leavingPlayer)
            .Should().Throw<InvalidOperationException>();
    }
}
