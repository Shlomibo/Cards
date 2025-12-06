using System;

using AwesomeAssertions;

using GameEngine.UnitTests.MockedGameState;

namespace GameEngine.UnitTests;

public class PlayerTests : TestsBase
{
    [Test]
    public void WhenGameIsCreated([Random(0, State.DefPlayersCount, 2, Distinct = true)] int playerId)
    {
        var player = Engine.Players[playerId];
        UpdatesListener updates = new(player);

        player.Should().BeEquivalentTo(
            new
            {
                PlayerId = playerId,
                State.SharedState,
                State = State.GetPlayerState(playerId),
            });

        updates.UpdatesCount.Should().Be(0);
    }

    [Test]
    public void WhenMoveIsInvalid([Random(0, State.DefPlayersCount, 2, Distinct = true)] int playerId)
    {
        State.IsNextMoveValid = false;
        var player = Engine.Players[playerId];
        UpdatesListener updates = new(player);
        Move move = new();

        player.IsValidMove(move).Should().BeFalse();
        State.LastPlayedPlayer.Should().Be(playerId);
        updates.UpdatesCount.Should().Be(0);

        player.PlayMove(move);
        State.LastPlayedPlayer.Should().Be(playerId);
        updates.UpdatesCount.Should().Be(0);
    }

    [Test]
    public void WhenMoveIsValid([Random(0, State.DefPlayersCount, 2, Distinct = true)] int playerId)
    {
        State.IsNextMoveValid = true;
        var player = Engine.Players[playerId];
        UpdatesListener updates = new(player);
        Move move = new();

        player.IsValidMove(move).Should().BeTrue();
        State.LastPlayedPlayer.Should().Be(playerId);
        updates.UpdatesCount.Should().Be(0);

        player.PlayMove(move);
        State.LastPlayedPlayer.Should().Be(playerId);
        updates.UpdatesCount.Should().Be(1);
    }

    [Test]
    public void WhenTheStateChanges([Random(0, State.DefPlayersCount, 2, Distinct = true)] int playerId)
    {
        var player = Engine.Players[playerId];
        UpdatesListener updates = new(player);
        Move move = new();

        Engine.PlayMove(move);

        updates.UpdatesCount.Should().Be(1);
    }
}

file sealed class UpdatesListener
{
    private int _updatesCount;

    public int UpdatesCount => _updatesCount;

    public UpdatesListener(IPlayer<SharedState, PlayerState, Move> player)
    {
        player.Updated += OnUpdate;
    }

    private void OnUpdate(object? sender, EventArgs? e) =>
        Interlocked.Increment(ref _updatesCount);
}
