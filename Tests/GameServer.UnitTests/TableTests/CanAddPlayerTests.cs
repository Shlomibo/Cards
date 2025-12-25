using System;

using AutoFixture;

using AwesomeAssertions;

namespace GameServer.UnitTests.TableTests;

[TestFixture]
public class CanAddPlayerTests : AddPlayerTestsBase
{
    public override void WhenAPlayerWithTheSameNameExists()
    {
        var x = GetTestData();
        string newPlayer = x.Players[Random.Next(x.Players.Count)].Name;

        x.TestSubject.CanAddPlayer(newPlayer)
            .Should().BeFalse();
    }

    public override void WhenTheGameHasStarted()
    {
        var x = GetTestData(setGame: true);
        string newPlayer = Fixture.Create<string>();

        x.TestSubject.CanAddPlayer(newPlayer)
            .Should().BeFalse();
    }

    public override void WhenThePlayerIsNew()
    {
        var x = GetTestData();
        string newPlayer = Fixture.Create<string>();

        x.TestSubject.CanAddPlayer(newPlayer)
            .Should().BeTrue();
    }

    public override void WhenThePlayerNameIsEmpty()
    {
        var x = GetTestData();

        x.TestSubject.CanAddPlayer("")
            .Should().BeFalse();
    }

    public override void WhenThePlayerNameIsNull()
    {
        var x = GetTestData();

        x.TestSubject.CanAddPlayer(null!)
            .Should().BeFalse();
    }
}
