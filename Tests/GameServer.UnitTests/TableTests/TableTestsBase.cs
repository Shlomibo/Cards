using AutoFixture;

using GameEngine;

using NUnit.Framework.Internal;

namespace GameServer.UnitTests.TableTests;

using TestSubject = Table<GameState, GameState, GameState, GameMove>;

public abstract class TableTestsBase
{
    protected static Fixture Fixture { get; } = new();
    protected static Randomizer Random => TestContext.CurrentContext.Random;

    protected static Engine<GameState, GameState, GameState, GameMove> CreateGameEngine() =>
        new(new GameState());

    private protected static TestData GetTestData(
        string? name = null,
        string? master = null,
        IEnumerable<string>? otherPlayers = null,
        int? otherPlayersCount = null,
        Engine<GameState, GameState, GameState, GameMove>? gameEngine = null)
    {
        name ??= Fixture.Create<string>();
        master ??= Fixture.Create<string>();
        otherPlayersCount ??= Random.Next(1, 6);
        otherPlayers ??= Fixture.CreateMany<string>(otherPlayersCount.Value);

        TestSubject testSubject = new(
            name,
            master);

        List<TestSubject.Player> players = new(otherPlayersCount.Value + 1)
        {
            testSubject.TableMaster,
        };

        foreach (var player in otherPlayers)
        {
            players.Add(testSubject.AddPlayer(player));
        }

        if (gameEngine != null)
        {
            testSubject.SetGame(gameEngine);
        }

        return new TestData(testSubject, players);
    }

    private protected record TestData(
        TestSubject TestSubject,
        IReadOnlyList<TestSubject.Player> Players)
    {
        public IReadOnlyList<TestSubject.Player> GetTablePlayers() =>
            ((ITable<GameState, GameState, GameState, GameMove>)TestSubject).GetPlayers();
    }
}
