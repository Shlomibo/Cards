using System;

using AwesomeAssertions;

using DTOs;

using Moq;

namespace GameServer.UnitTests.ConnectionTests;

public class StateUpdatedTests : ConnectionTestsBase
{
    [Test]
    public void WhenRegisteringANewHandler()
    {
        var state = RandomState();
        var x = GetTestData(state);
        var handler = GetEventsHandler();

        // Set state in connection
        x.Table.Raise(
            table => table.GameUpdated += null,
            x.TestSubject,
            new TableGameUpdateEventArgs<GameState, GameState, GameState, GameMove>(
                state,
                [(x.Table.Object[x.ConnectionId], state)]
            ));

        x.TestSubject.StateUpdated += handler.Object.StateUpdated;

        handler.Verify(
            h => h.StateUpdated(
                x.TestSubject,
                It.Is<StateUpdatedEventArgs<GameState.Serialized>>(s => Matches(s, new
                {
                    State = new StateUpdate<GameState.Serialized>(
                        x.TableName,
                        currentPlayer: x.CurrentPlayer,
                        new Player[] { new(0, x.CurrentPlayer.PlayerName, PlayerState.Playing) },
                        state.Serialize())
                }))),
            Times.Once,
            "When a handlers is being registered, it is called with the current game state");
    }

    [Test]
    public void WhenRegisteringANullHandler()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(conn => conn.StateUpdated += null)
            .Should().NotThrow();
    }

    [Test]
    public void WhenTheStateUpdatesAndThereAreHandlersRegistered()
    {
        var x = GetTestData();
        var handler = GetEventsHandler();

        x.TestSubject.StateUpdated += handler.Object.StateUpdated;

        handler.Reset();
        var state = RandomState();

        StateUpdate<GameState.Serialized> update = new(
            x.TableName,
            currentPlayer: x.CurrentPlayer,
            [new(0, x.CurrentPlayer.PlayerName, PlayerState.Playing)],
            state.Serialize());

        x.Table.Raise(
            table => table.GameUpdated += null,
            x.TestSubject,
            new TableGameUpdateEventArgs<GameState, GameState, GameState, GameMove>(
                state,
                [(x.Table.Object[x.ConnectionId], state)]
            ));

        handler.Verify(
            h => h.StateUpdated(
                x.TestSubject,
                It.Is<StateUpdatedEventArgs<GameState.Serialized>>(s => Matches(s, new
                {
                    State = update,
                }))),
            Times.Once,
            "When a handlers is being registered, it is called with the current game state");
    }

    [Test]
    public void WhenTheStateUpdatesAndThereAreNoHandlersRegistered()
    {
        var x = GetTestData();

        var state = RandomState();

        StateUpdate<GameState.Serialized> update = new(
            x.TableName,
            currentPlayer: x.CurrentPlayer,
            [new(0, x.CurrentPlayer.PlayerName, PlayerState.Playing)],
            state.Serialize());

        x.Table.Raise(
            table => table.GameUpdated += null,
            x.TestSubject,
            new TableGameUpdateEventArgs<GameState, GameState, GameState, GameMove>(
                state,
                [(x.Table.Object[x.ConnectionId], state)]
            ));
    }
}
