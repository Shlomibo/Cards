using AutoFixture;

using GameEngine;

using Moq;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace GameServer.UnitTests.TableTests;

using static It;

using TestSubject = Table<GameState, GameState, GameState, GameMove>;
using ITestGameEngine = IEngine<GameState, GameState, GameMove>;

public abstract class TableTestsBase
{
    protected static Fixture Fixture { get; } = new();
    protected static Randomizer Random => TestContext.CurrentContext.Random;

    private protected static TestData GetTestData(
        string? name = null,
        string? master = null,
        IEnumerable<string>? otherPlayers = null,
        int? otherPlayersCount = null,
        bool setGame = false)
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

        Mock<ITestGameEngine>? gameEngine = !setGame
            ? null
            : CreateGameEngine();

        if (gameEngine != null)
        {
            testSubject.SetGame(gameEngine.Object);
            gameEngine.Invocations.Clear();
        }

        return new TestData(testSubject, players, gameEngine);
    }

    private protected static Mock<ITestGameEngine> CreateGameEngine()
    {
        Engine<GameState, GameState, GameState, GameMove> engineImpl = new(new GameState());
        Mock<ITestGameEngine> mock = new(MockBehavior.Strict);

        mock
            .Setup(m => m.IsValidMove(
                IsAny<GameMove>(),
                IsAny<int?>()))
            .Returns(engineImpl.IsValidMove);
        mock
            .Setup(m => m.Players)
            .Returns(() => engineImpl.Players);
        mock
            .Setup(m => m.PlayMove(
                IsAny<GameMove>(),
                IsAny<int?>()))
            .Callback(engineImpl.PlayMove);
        mock
            .Setup(m => m.RemovePlayer(IsAny<int>()));
        mock
            .Setup(m => m.State)
            .Returns(engineImpl.State);
        mock.SetupAdd(m => m.Updated += IsAny<EventHandler>())
            .Callback((EventHandler handler) => engineImpl.Updated += handler);
        mock.SetupRemove(m => m.Updated -= IsAny<EventHandler>())
            .Callback((EventHandler handler) => engineImpl.Updated -= handler);

        return mock;
    }

    private protected record TestData(
        TestSubject TestSubject,
        IReadOnlyList<TestSubject.Player> Players,
        Mock<ITestGameEngine>? GameEngine)
    {
        public Mock<ITableListener> TableListener { get; } = CreateTableListener(TestSubject);

        public IReadOnlyList<TestSubject.Player> GetTablePlayers() =>
            ((ITable<GameState, GameState, GameState, GameMove>)TestSubject).GetPlayers();

        private static Mock<ITableListener> CreateTableListener(TestSubject testSubject)
        {
            Mock<ITableListener> listener = new(MockBehavior.Loose);

            testSubject.GameUpdated += listener.Object.GameUpdated;
            testSubject.TableUpdated += listener.Object.TableUpdated;

            return listener;
        }
    }
}

internal interface ITableListener
{
    void TableUpdated(object? sender, EventArgs e);
    void GameUpdated(
        object? sender,
        TableGameUpdateEventArgs<GameState, GameState, GameState, GameMove> e);
}
