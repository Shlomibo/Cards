using System;

using AutoFixture;

using AwesomeAssertions;

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
        var playersCount = Random.Next(1, 6);

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
            Game = (Engine<GameState, GameState, GameState, GameMove>?)null,
            GameStarted = false,
        });

        x.GetTablePlayers()
            .Should().BeEquivalentTo(x.Players);
    }
}
