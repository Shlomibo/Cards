using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

namespace GameServer.UnitTests.TableTests;

using TestSubject = Table<GameState, GameState, GameState, GameMove>;

[TestFixture]
public class AddPlayerTests : AddPlayerTestsBase
{
    public override void WhenAPlayerWithTheSameNameExists()
    {
        var x = GetTestData();
        string newPlayer = x.Players[Random.Next(x.Players.Count)].Name;

        x.TestSubject.Invoking(t => t.AddPlayer(newPlayer))
            .Should().Throw<InvalidOperationException>();

        x.GetTablePlayers().Should().BeEquivalentTo(x.Players);
    }

    public override void WhenTheGameHasStarted()
    {
        var x = GetTestData(gameEngine: CreateGameEngine());
        string newPlayer = Fixture.Create<string>();

        x.TestSubject.Invoking(t => t.AddPlayer(newPlayer))
            .Should().Throw<InvalidOperationException>();

        x.GetTablePlayers().Should().BeEquivalentTo(x.Players);
    }

    public override void WhenThePlayerIsNew()
    {
        var x = GetTestData();
        string newPlayer = Fixture.Create<string>();

        var result = x.TestSubject.AddPlayer(newPlayer);

        result.Should().NotBeNull();
        result.ConnectionId.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(new
        {
            Id = x.Players.Count,
            Name = newPlayer,
            State = PlayerState.Playing,
        });

        x.GetTablePlayers().Should().BeEquivalentTo([
            .. x.Players,
            new TestSubject.Player(x.Players.Count, newPlayer, result.ConnectionId)]);
    }

    public override void WhenThePlayerNameIsEmpty()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(t => t.AddPlayer(""))
            .Should().Throw<ArgumentException>();

        x.GetTablePlayers().Should().BeEquivalentTo(x.Players);
    }

    public override void WhenThePlayerNameIsNull()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(t => t.AddPlayer(null!))
            .Should().Throw<ArgumentNullException>();

        x.GetTablePlayers().Should().BeEquivalentTo(x.Players);
    }
}
