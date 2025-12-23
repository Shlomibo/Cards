using System;

using AutoFixture;

using AwesomeAssertions;

namespace GameServer.UnitTests.TablesManagerTests;

[TestFixture]
public sealed class JoinTableTests : JoinTableTestsBase
{
    public override void WhenThePlayerNameIsAvailable()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        string playerName = Fixture.Create<string>();

        var connection = x.TestSubject.JoinTable(
            table.Name,
            playerName);

        ValidateConnection(
            playerName,
            connection!,
            x.TestSubject.Tables.Values.First(),
            table.AllPlayers.Count);
    }

    public override void WhenThePlayerNameIsAvailableButTheGameHasStarted()
    {
        var table = Fixture.Build<TableData>()
            .With(t => t.HasGameStarted, true)
            .Create();

        var x = GetTestData([table]);
        var playerName = Fixture.Create<string>();

        x.TestSubject.Invoking(tm => tm.JoinTable(
            table.Name,
            playerName))
            .Should().Throw<InvalidOperationException>();

        ValidatePlayerNotAdded(table, x, playerName);
    }

    public override void WhenThePlayerNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        x.TestSubject.Invoking(tm => tm.JoinTable(
            table.Name,
            name!))
            .Should().Throw<ArgumentException>();

        ValidatePlayerNotAdded(table, x, name!);
    }

    public override void WhenThePlayerNameIsTaken()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        var playerName = table.AllPlayers[Random.Next(table.AllPlayers.Count)];

        x.TestSubject.Invoking(tm => tm.JoinTable(
            table.Name,
            playerName))
            .Should().Throw<InvalidOperationException>();

        ValidatePlayerNotAdded(table, x, "");
    }

    public override void WhenTheTableDoesNotExist()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.JoinTable(
            Fixture.Create<string>(),
            Fixture.Create<string>()))
            .Should().Throw<InvalidOperationException>();
    }

    public override void WhenTheTableNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        x.TestSubject.Invoking(tm => tm.JoinTable(
            name!,
            Fixture.Create<string>()))
            .Should().Throw<ArgumentException>();
    }
}
