using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

using Moq;

namespace GameServer.UnitTests.ConnectionTests;

public abstract class ConnectionTestsBase
{
    protected static Fixture Fixture { get; } = CreateFixture();

    protected static GameState RandomState() =>
        Fixture.Create<GameState>();

    protected static bool Matches<T, TExpected>(T? value, TExpected? expected)
    {
        try
        {
            value.Should().BeEquivalentTo(
                expected,
                opt => opt.IncludingAllRuntimeProperties());
            return true;
        }
        catch (Exception ex)
        {
            TestContext.Error.WriteLine(ex.ToString());
            return false;
        }
    }

    protected static Mock<IEventsHandler> GetEventsHandler() =>
        new(MockBehavior.Loose);
    private protected static TestData GetTestData(GameState? state = null)
    {
        state ??= new();

        string tableName = Fixture.Create<string>();
        var player = Fixture.Create<CurrentPlayer>();
        Table.Player tablePlayer = new(0, player.PlayerName, PlayerState.Playing);
        var id = player.ConnectionId;

        Mock<ITable<GameState, GameState, GameState, GameMove>> table = new(MockBehavior.Strict);
        table
            .Setup(table => table.TableName)
            .Returns(tableName);
        table
            .Setup(table => table.GameStarted)
            .Returns(true);
        table
            .Setup(table => table[It.IsAny<Guid>()])
            .Returns((Guid pId) => pId != id
                ? throw new ArgumentException("id")
                : new Table<GameState, GameState, GameState, GameMove>.Player(0, player.PlayerName, id));
        table
            .Setup(table => table.AsTableDescriptor())
            .Returns(new Table(
                tableName,
                tablePlayer,
                []));
        table
            .Setup(table => table.PlayMove(
                It.IsAny<GameMove>(),
                It.IsAny<int?>()));
        table
            .Setup(table => table.RemovePlayer(
                It.IsAny<Guid>()));

        Connection<GameState, GameState, GameState, GameMove, GameState.Serialized, GameMove.Serialized> testSubject =
            new(table.Object,
            id,
            (state, _) => state.Serialize(),
            move => move.Deserialize());

        return new TestData(
            testSubject,
            table,
            id,
            tableName,
            player);
    }

    private static Fixture CreateFixture()
    {
        Fixture fixture = new();
        fixture.Customize<CurrentPlayer>(builder => builder
            .With(x => x.PlayerId, 0));
        fixture.Register(() => fixture.Create<bool>()
            ? (GameMove.Serialized)fixture.Create<ValidMove.Serialized>()
            : fixture.Create<InvalidMove.Serialized>());

        return fixture;
    }

    private protected record TestData(
        Connection<GameState, GameState, GameState, GameMove, GameState.Serialized, GameMove.Serialized> TestSubject,
        Mock<ITable<GameState, GameState, GameState, GameMove>> Table,
        Guid ConnectionId,
        string TableName,
        CurrentPlayer CurrentPlayer);

    public interface IEventsHandler
    {
        void StateUpdated(object? sender, StateUpdatedEventArgs<GameState.Serialized> e);

        void Closed(object? sender, EventArgs e);
    }
}
