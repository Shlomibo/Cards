using System;

using AutoFixture;

using AwesomeAssertions;

using Moq;

namespace GameServer.UnitTests.TableTests;

using GameUpdateEventArgs = TableGameUpdateEventArgs<GameState, GameState, GameState, GameMove>;

public class PlayMoveTests : TableTestsBase
{
    [TestCase(true)]
    [TestCase(false)]
    public void WhenNoGameHasStarted(bool withUserId)
    {
        var x = GetTestData();
        int? playerId = withUserId
            ? x.Players[Random.Next(x.Players.Count)].Id
            : null;

        GameMove move = Fixture.Create<ValidMove>();

        x.TestSubject.Invoking(t => t.PlayMove(move, playerId))
            .Should().NotThrow();
        x.TableListener.Verify(
            l => l.GameUpdated(
                It.IsAny<object?>(),
                It.IsAny<GameUpdateEventArgs>()),
            Times.Never);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenTheMoveIsIllegal(bool withUserId)
    {
        var x = GetTestData(setGame: true);
        int? playerId = withUserId
            ? x.Players[Random.Next(x.Players.Count)].Id
            : null;

        GameMove move = Fixture.Create<InvalidMove>();

        x.TestSubject.PlayMove(move, playerId);
        x.TableListener.Verify(
            l => l.GameUpdated(
                It.IsAny<object?>(),
                It.IsAny<GameUpdateEventArgs>()),
            Times.Never);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WhenTheMoveIsLegal(bool withUserId)
    {
        var x = GetTestData(setGame: true);
        int? playerId = withUserId
            ? x.Players[Random.Next(x.Players.Count)].Id
            : null;

        GameMove move = Fixture.Create<ValidMove>();

        x.TestSubject.PlayMove(move, playerId);
        x.TableListener.Verify(
            l => l.GameUpdated(
                It.IsAny<object?>(),
                It.IsAny<GameUpdateEventArgs>()),
            Times.Once);

        x.GameEngine!.Object.State.DidMove.Should().BeTrue();
    }
}
