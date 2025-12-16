using System;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class JumpTests : TurnsManagerTestsBase
{
    [Test]
    public void WhenJumpingUpWithNoWarping()
    {
        var testSubject = GetTestSubject(current: _ => 0);
        var jump = Random.Next(1, testSubject.ActivePlayers.Count);
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Current = testSubject.ActivePlayers[jump],
            Previous = testSubject.ActivePlayers[jump - 1],
            Next = testSubject.ActivePlayers[(jump + 1) % testSubject.PlayersCount],
        };

        testSubject.Jump(jump, TurnsDirection.Up);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingDownWithNoWarping()
    {
        var testSubject = GetTestSubject(current: pCount => pCount - 1);
        var jump = Random.Next(1, testSubject.ActivePlayers.Count);
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Current = testSubject.ActivePlayers[^((jump % testSubject.PlayersCount) + 1)],
            Previous = testSubject.ActivePlayers[^(((jump + 1) % testSubject.PlayersCount) + 1)],
            Next = testSubject.ActivePlayers[^jump],
        };

        testSubject.Jump(jump, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingUpWithWarping()
    {
        var testSubject = GetTestSubject(current: _ => 0);
        var expectedJump = Random.Next(1, testSubject.ActivePlayers.Count);
        var jump = expectedJump + (testSubject.PlayersCount * Random.Next(1, 4));

        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Current = testSubject.ActivePlayers[expectedJump],
            Previous = testSubject.ActivePlayers[expectedJump - 1],
            Next = testSubject.ActivePlayers[(expectedJump + 1) % testSubject.PlayersCount],
        };

        testSubject.Jump(jump, TurnsDirection.Up);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingDownWithWarping()
    {
        var testSubject = GetTestSubject(current: pCount => pCount - 1);
        var expectedJump = Random.Next(1, testSubject.ActivePlayers.Count);
        var jump = expectedJump + (testSubject.PlayersCount * Random.Next(1, 4));
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Current = testSubject.ActivePlayers[^((expectedJump % testSubject.PlayersCount) + 1)],
            Previous = testSubject.ActivePlayers[^(((expectedJump + 1) % testSubject.PlayersCount) + 1)],
            Next = testSubject.ActivePlayers[^expectedJump],
        };

        testSubject.Jump(jump, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    [TestCase([null])]
    public void WhenJumpingZero(TurnsDirection? direction)
    {
        var testSubject = GetTestSubject();
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Current,
            testSubject.Previous,
            testSubject.Next,
        };

        testSubject.Jump(0, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingAndThereIsOnlyOnePlayer()
    {
        var testSubject = GetTestSubject(playersCount: 1);
        var jump = Random.Next(1, testSubject.ActivePlayers.Count);
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Current,
            testSubject.Previous,
            testSubject.Next,
        };

        testSubject.Jump(jump, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingOddNumberAndThereAreOnlyTwoPlayers()
    {
        var testSubject = GetTestSubject(playersCount: 2);
        var jump = 1 + (Random.Next(1, testSubject.ActivePlayers.Count) * 2);
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            Current = testSubject.Next,
            Next = testSubject.Current,
            Previous = testSubject.Current,
        };

        testSubject.Jump(jump, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenJumpingEvenNumberAndThereAreOnlyTwoPlayers()
    {
        var testSubject = GetTestSubject(playersCount: 1);
        var jump = Random.Next(1, testSubject.ActivePlayers.Count) * 2;
        var expected = new
        {
            ActivePlayers = testSubject.ActivePlayers.ToArray(),
            testSubject.Direction,
            testSubject.InitialPlayersCount,
            testSubject.Current,
            testSubject.Previous,
            testSubject.Next,
        };

        testSubject.Jump(jump, TurnsDirection.Down);

        testSubject.Should().BeEquivalentTo(expected);
    }
}
