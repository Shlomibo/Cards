using System.Diagnostics;

using AwesomeAssertions;

using NUnit.Framework.Internal;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public class InitializationTests : TurnsManagerTestsBase
{
    [Test]
    public void WhenInitializingWithANegativeNumberOfPlayers()
    {
        Creation(() => new TurnsManager(-1)).Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void WhenInitializingWithZeroPlayers()
    {
        Creation(() => new TurnsManager(0)).Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void WhenInitializingWithOnePlayers()
    {
        TurnsManager testSubject = new(1);

        testSubject.Should().BeEquivalentTo(new
        {
            InitialPlayersCount = 1,
            Current = 0,
            Previous = 0,
            Next = 0,
            Direction = default(TurnsDirection),
        });
    }

    [Test]
    public void WhenInitializingWithManyPlayers([Random(2, 6, 1)] int playersCount)
    {
        TurnsManager testSubject = new(playersCount);

        testSubject.Should().BeEquivalentTo(new
        {
            InitialPlayersCount = playersCount,
            Current = 0,
            Previous = playersCount - 1,
            Next = 1,
            Direction = default(TurnsDirection),
        });
    }

    [TestCase(TurnsDirection.Up)]
    [TestCase(TurnsDirection.Down)]
    public void WhenInitializingWithDirection(TurnsDirection direction)
    {
        int playersCount = Random.Next(2, 6);
        TurnsManager testSubject = new(playersCount, direction);

        testSubject.Should().BeEquivalentTo(new
        {
            InitialPlayersCount = playersCount,
            Current = 0,
            Previous = direction switch
            {
                TurnsDirection.Down => 1,
                TurnsDirection.Up => playersCount - 1,
                _ => throw new UnreachableException(),
            },
            Next = direction switch
            {
                TurnsDirection.Down => playersCount - 1,
                TurnsDirection.Up => 1,
                _ => throw new UnreachableException(),
            },
            Direction = direction,
        });
    }

    [Test]
    public void WhenInitializingWithInvalidDirection()
    {
        Creation(() => new TurnsManager(3, (TurnsDirection)(-23))).Should()
            .Throw<ArgumentException>();
    }

    private static Func<TurnsManager> Creation(Func<TurnsManager> factory) =>
        factory;
}
