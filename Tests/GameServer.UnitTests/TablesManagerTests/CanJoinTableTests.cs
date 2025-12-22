using System;

using AutoFixture;

using AwesomeAssertions;

namespace GameServer.UnitTests.TablesManagerTests;

[TestFixture]
public class CanJoinTableTests : JoinTableTestsBase
{
    public override void WhenThePlayerNameIsAvailable()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.CanJoinTable(table.Name, Fixture.Create<string>());
        canJoin.Should().BeTrue();
    }

    public override void WhenThePlayerNameIsAvailableButTheGameHasStarted()
    {
        var table = Fixture.Build<TableData>()
            .With(t => t.HasGameStarted, true)
            .Create();

        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.CanJoinTable(table.Name, Fixture.Create<string>());
        canJoin.Should().BeFalse();
    }

    public override void WhenThePlayerNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.CanJoinTable(table.Name, name!);
        canJoin.Should().BeFalse();
    }

    public override void WhenThePlayerNameIsTaken()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.CanJoinTable(
            table.Name,
            table.AllPlayers[Random.Next(table.AllPlayers.Count)]);
        canJoin.Should().BeFalse();
    }

    public override void WhenTheTableDoesNotExist()
    {
        var x = GetTestData();

        bool canJoin = x.TestSubject.CanJoinTable(
            Fixture.Create<string>(),
            Fixture.Create<string>());
        canJoin.Should().BeFalse();
    }

    public override void WhenTheTableNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.CanJoinTable(name!, Fixture.Create<string>());
        canJoin.Should().BeFalse();
    }
}
