using System;

using AutoFixture;

using AwesomeAssertions;

using GameEngine;

using Moq;

namespace GameServer.UnitTests.TablesManagerTests;

public class StartGameTests : TablesManagerTestsBase
{
    [Test]
    public void WhenTableMasterStartsANewGame()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        var gameTable = x.Tables[0];
        var master = gameTable.Object.TableMaster;

        x.TestSubject.StartGame(table.Name, master.ConnectionId, default);

        gameTable.Verify(
            t => t.SetGame(It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()),
            Times.Once);
    }

    [Test]
    public void WhenTableMasterStartsANewGameButAGameWasAlreadyStarted()
    {
        var table = Fixture.Build<TableData>()
            .With(x => x.HasGameStarted, true)
            .Create();

        var x = GetTestData([table]);
        var gameTable = x.Tables[0];
        var master = gameTable.Object.TableMaster;

        x.TestSubject.StartGame(table.Name, master.ConnectionId, default);

        gameTable.Verify(
            t => t.SetGame(It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()),
            Times.Never);
    }

    [Test]
    public void WhenPlayerStartsANewGame()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        var gameTable = x.Tables[0];
        var player = gameTable.Object.GetPlayers().Skip(1).First();


        x.TestSubject.Invoking(tm => tm.StartGame(table.Name, player.ConnectionId, default))
            .Should().Throw<InvalidOperationException>();

        gameTable.Verify(
            t => t.SetGame(It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()),
            Times.Never);
    }

    [Test]
    public void WhenNonPlayerStartsANewGame()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        var gameTable = x.Tables[0];

        x.TestSubject.Invoking(tm => tm.StartGame(
            table.Name,
            Guid.NewGuid(),
            default))
            .Should().Throw<InvalidOperationException>();

        gameTable.Verify(
            t => t.SetGame(It.IsAny<Engine<GameState, GameState, GameState, GameMove>>()),
            Times.Never);
    }

    [Test]
    public void WhenStartingGameInNonExistentTable()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.StartGame(
            Fixture.Create<string>(),
            Guid.NewGuid(),
            default))
            .Should().Throw<ArgumentException>();
    }
}
