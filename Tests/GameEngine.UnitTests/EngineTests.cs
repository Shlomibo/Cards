using AwesomeAssertions;

using GameEngine.UnitTests.MockedGameState;

namespace GameEngine.UnitTests;

public class EngineTests : TestsBase
{
    private int _stateUpdatesCount;

    [SetUp]
    public override void Setup()
    {
        _stateUpdatesCount = 0;
        base.Setup();

        Engine.Updated += (s, e) => Interlocked.Increment(ref _stateUpdatesCount);
    }

    [Test]
    public void WhenGameIsCreated()
    {
        Engine.Players.Should().HaveCount(State.PlayersCount);
        Engine.State.Should().BeEquivalentTo(State.SharedState);
        _stateUpdatesCount.Should().Be(0);
    }

    [Test]
    public void WhenAMoveIsInvalid()
    {
        State.IsNextMoveValid = false;
        Move move = new();

        Engine.IsValidMove(move).Should().BeFalse();
        _stateUpdatesCount.Should().Be(0);
        State.LastPlayedPlayer.Should().BeNull();

        Engine.PlayMove(move);
        _stateUpdatesCount.Should().Be(0);
        State.LastPlayedPlayer.Should().BeNull();
    }

    [Test]
    public void WhenAMoveIsValid()
    {
        State.IsNextMoveValid = true;
        Move move = new();

        Engine.IsValidMove(move).Should().BeTrue();
        _stateUpdatesCount.Should().Be(0);
        State.LastPlayedPlayer.Should().BeNull();

        Engine.PlayMove(move);
        _stateUpdatesCount.Should().Be(1);
        State.LastPlayedPlayer.Should().BeNull();
    }
}
