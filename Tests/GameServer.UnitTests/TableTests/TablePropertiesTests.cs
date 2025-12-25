using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

using GameEngine;

namespace GameServer.UnitTests.TableTests;

public class TablePropertiesTests : TableTestsBase
{
    [Test]
    [Repeat(100)]
    public void TableProperties()
    {
        var name = Fixture.Create<string>();
        var master = Fixture.Create<string>();

        var x = GetTestData(name, master);

        x.TestSubject.Should().BeEquivalentTo(new
        {
            TableMaster = new
            {
                Id = 0,
                Name = master,
                x.Players[0].ConnectionId,
            },
            TableName = name,
            Game = (IEngine<GameState, GameState, GameMove>?)null,
            GameStarted = false,
        });

        x.GetTablePlayers()
            .Should().BeEquivalentTo(x.Players);
    }

    [Test]
    [Repeat(100)]
    public void TableDescriptorTest()
    {
        var name = Fixture.Create<string>();
        var master = Fixture.Create<string>();
        var playersCount = Random.Next(1, 6);
        var players = Fixture.CreateMany<string>(playersCount)
            .ToArray();

        var x = GetTestData(name, master, players);

        x.TestSubject.AsTableDescriptor().Should().BeEquivalentTo(new
        {
            Name = name,
            TableMaster = new
            {
                Id = 0,
                Name = master,
                State = PlayerState.Playing,
            },
            Players = players.Prepend(master)
                .Select((p, i) => new
                {
                    Id = i,
                    Name = p,
                    State = PlayerState.Playing,
                })
        });
    }
}
