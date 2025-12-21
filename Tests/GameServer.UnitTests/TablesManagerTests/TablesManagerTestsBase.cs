using System;

using DTOs;

using GameEngine;

using Moq;

namespace GameServer.UnitTests.TablesManagerTests;

using Player = Table<GameState, GameState, GameState, GameMove>.Player;

public abstract class TablesManagerTestsBase
{
    private protected static TestData GetTestData(IEnumerable<TableData>? tablesData = null)
    {
        Mock<ITable<GameState, GameState, GameState, GameMove>>[] tables = tablesData == null
        ? []
        : [.. tablesData.Select(AsTable)];

        TablesManager<
            ValueTuple,
            GameState,
            GameState,
            GameState,
            GameMove,
            GameState.Serialized,
            GameMove.Serialized> testSubject = new(
                _ => new Engine<GameState, GameState, GameState, GameMove>(new GameState()),
                (s, _) => s.Serialize(),
                m => m.Deserialize(),
                tables.Select(t => KeyValuePair.Create(t.Object.TableName, t.Object)));

        return new TestData(testSubject, tables);

        static Mock<ITable<GameState, GameState, GameState, GameMove>> AsTable(TableData data)
        {
            Player master = new(0, data.MasterPlayer, Guid.NewGuid());
            Player[] players = [
                master,
                .. data.Players.Select((name, i) =>
                    new Player(i + 1, name, Guid.NewGuid()))];
            var byIds = players.ToDictionary(p => p.ConnectionId);
            bool didSetGame = false;

            Mock<ITable<GameState, GameState, GameState, GameMove>> table = new(MockBehavior.Strict);
            table
                .Setup(table => table[It.IsAny<Guid>()])
                .Returns((Guid id) => byIds[id]);
            table
                .Setup(table => table.TableMaster)
                .Returns(master);
            table
                .Setup(table => table.TableName)
                .Returns(data.Name);
            table
                .Setup(table => table.GameStarted)
                .Returns(data.HasGameStarted);
            table
                .Setup(table => table.AddPlayer(It.IsAny<string>()))
                .Returns((string name) =>
                {
                    Player p = new(1 + byIds.Values.Max(p => p.Id), name, Guid.NewGuid());
                    byIds[p.ConnectionId] = p;

                    return p;
                });
            table
                .Setup(table => table.AsTableDescriptor())
                .Returns(new Table(
                    master.AsDescriptor(),
                    byIds.Values
                        .Where(p => p != master)
                        .Select(p => p.AsDescriptor())));
            table
                .Setup(table => table.PlayMove(
                    It.IsAny<GameMove>(),
                    It.IsAny<int?>()));
            table
                .Setup(table => table.RemovePlayer(
                    It.IsAny<Guid>()))
                .Callback((Guid id) =>
                {
                    if (byIds.TryGetValue(id, out var player))
                    {
                        byIds[id] = player with
                        {
                            State = PlayerState.LeftGame,
                        };
                    }
                });
            table
                .Setup(table => table.SetGame(
                    It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()))
                .Callback((Engine<GameState, GameState, GameState, GameMove> _) => didSetGame = true);
            table
                .Setup(table => table.TrySetGame(
                    It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()))
                .Returns((Engine<GameState, GameState, GameState, GameMove> _) =>
                {
                    bool result = !didSetGame;
                    didSetGame = true;

                    return result;
                });

            return table;
        }
    }

    private protected record TestData(
        TablesManager<
            ValueTuple,
            GameState,
            GameState,
            GameState,
            GameMove,
            GameState.Serialized,
            GameMove.Serialized> TestSubject,
        Mock<ITable<GameState, GameState, GameState, GameMove>>[] Tables);
}

internal record TableData(
    string Name,
    string MasterPlayer,
    params IEnumerable<string> Players)
{
    public bool HasGameStarted { get; init; } = true;
}
