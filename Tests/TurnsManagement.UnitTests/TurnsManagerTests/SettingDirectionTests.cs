using System;
using System.Diagnostics;

using AwesomeAssertions;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class SettingDirectionTests : TurnsManagerTestsBase
{
    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenSettingDirectionToSelf(TurnsDirection initialState)
    {
        var testSubject = GetTestSubject(direction: initialState);
        var expected = new
        {
            testSubject.ActivePlayers,
            testSubject.Current,
            testSubject.Direction,
            testSubject.Next,
            testSubject.InitialPlayersCount,
            testSubject.Previous,
        };

        testSubject.Direction = testSubject.Direction;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenSwitchingDirection(TurnsDirection initialState)
    {
        var testSubject = GetTestSubject(direction: initialState);
        var expected = new
        {
            testSubject.ActivePlayers,
            testSubject.Current,
            Direction = testSubject.Direction switch
            {
                TurnsDirection.Up => TurnsDirection.Down,
                TurnsDirection.Down => TurnsDirection.Up,
                _ => throw new UnreachableException(),
            },
            Next = testSubject.Previous,
            testSubject.InitialPlayersCount,
            Previous = testSubject.Next,
        };

        testSubject.SwitchDirection();

        testSubject.Should().BeEquivalentTo(expected);
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenSwitchingDirectionDirectly(TurnsDirection initialState)
    {
        var testSubject = GetTestSubject(direction: initialState);
        var expected = new
        {
            testSubject.ActivePlayers,
            testSubject.Current,
            Direction = testSubject.Direction switch
            {
                TurnsDirection.Up => TurnsDirection.Down,
                TurnsDirection.Down => TurnsDirection.Up,
                _ => throw new UnreachableException(),
            },
            Next = testSubject.Previous,
            testSubject.InitialPlayersCount,
            Previous = testSubject.Next,
        };

        testSubject.Direction = expected.Direction;

        testSubject.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void WhenSettingDirectionToInvalidValue()
    {
        var testSubject = GetTestSubject();

        testSubject.Invoking(tm => tm.Direction = (TurnsDirection)(-32))
            .Should().Throw<ArgumentException>();
    }
}
