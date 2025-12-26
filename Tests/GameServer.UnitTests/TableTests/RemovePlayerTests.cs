using System;

using AwesomeAssertions;

using DTOs;

using Moq;

namespace GameServer.UnitTests.TableTests;

public class RemovePlayerTests : TableTestsBase
{
    [Test]
    public void WhenRemovingExistentPlayerAndNoGameHasStarted()
    {
        var x = GetTestData();
        int removedPlayerId = Random.Next(1, x.Players.Count);
        var removedPlayer = x.Players[removedPlayerId];

        x.TestSubject.RemovePlayer(removedPlayer.ConnectionId);

        x.GetTablePlayers().Should().NotContainEquivalentOf(removedPlayer);
        var playerDescriptor = x.TestSubject.AsTableDescriptor().Players
            .FirstOrDefault(p => p.Id == removedPlayerId);

        playerDescriptor.Should().BeEquivalentTo(new
        {
            Id = removedPlayerId,
            Name = removedPlayer.Name,
            State = PlayerState.LeftGame,
        });
    }

    [Test]
    public void WhenRemovingExistentPlayerAndAGameHasStarted()
    {
        var x = GetTestData(setGame: true);
        int removedPlayerId = Random.Next(1, x.Players.Count);
        var removedPlayer = x.Players[removedPlayerId];

        x.TestSubject.RemovePlayer(removedPlayer.ConnectionId);

        x.GetTablePlayers().Should().NotContainEquivalentOf(removedPlayer);
        var playerDescriptor = x.TestSubject.AsTableDescriptor().Players
            .FirstOrDefault(p => p.Id == removedPlayerId);

        playerDescriptor.Should().BeEquivalentTo(new
        {
            Id = removedPlayerId,
            Name = removedPlayer.Name,
            State = PlayerState.LeftGame,
        });

        x.GameEngine!.Verify(
            game => game.RemovePlayer(removedPlayerId),
            Times.Once);
    }

    [Test]
    public void WhenRemovingTheTableMaster()
    {
        var x = GetTestData(setGame: true);
        var master = x.TestSubject.TableMaster;

        x.TestSubject.Invoking(t => t.RemovePlayer(master.ConnectionId))
            .Should().Throw<InvalidOperationException>();

        x.GetTablePlayers().Should().ContainEquivalentOf(master);
        var playerDescriptor = x.TestSubject.AsTableDescriptor().Players
                                            .FirstOrDefault();

        playerDescriptor.Should().BeEquivalentTo(new
        {
            Id = 0,
            Name = master.Name,
            State = PlayerState.Playing,
        });

        x.GameEngine!.Verify(
            game => game.RemovePlayer(0),
            Times.Never);
    }

    [Test]
    public void WhenRemovingNonExistentPlayer()
    {
        var x = GetTestData(setGame: true);

        x.TestSubject.RemovePlayer(Guid.NewGuid());

        x.GameEngine!.Verify(
            game => game.RemovePlayer(It.IsAny<int>()),
            Times.Never);
    }

    [Test]
    public void WhenRemovingRemovedPlayer()
    {
        var x = GetTestData(setGame: true);
        int removedPlayerId = Random.Next(1, x.Players.Count);
        var removedPlayer = x.Players[removedPlayerId];

        x.TestSubject.RemovePlayer(removedPlayer.ConnectionId);

        x.GameEngine!.Invocations.Clear();
        x.TestSubject.RemovePlayer(removedPlayer.ConnectionId);

        x.GameEngine!.Verify(
            game => game.RemovePlayer(removedPlayerId),
            Times.Never);
    }
}
