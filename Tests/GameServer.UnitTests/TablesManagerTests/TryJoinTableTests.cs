using System;

using AutoFixture;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

namespace GameServer.UnitTests.TablesManagerTests;

[TestFixture]
public class TryJoinTableTests : JoinTableTestsBase
{
    public override void WhenThePlayerNameIsAvailable()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        string playerName = Fixture.Create<string>();

        bool canJoin = x.TestSubject.TryJoinTable(
            table.Name,
            playerName,
            out var connection);

        canJoin.Should().BeTrue();

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

        bool canJoin = x.TestSubject.TryJoinTable(
            table.Name,
            playerName,
            out var connection);

        canJoin.Should().BeFalse();
        ValidatePlayerNotAdded(table, x, playerName, connection);
    }

    public override void WhenThePlayerNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.TryJoinTable(
            table.Name,
            name!,
            out var connection);

        canJoin.Should().BeFalse();
        ValidatePlayerNotAdded(table, x, name!, connection);
    }

    public override void WhenThePlayerNameIsTaken()
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);
        var playerName = table.AllPlayers[Random.Next(table.AllPlayers.Count)];

        bool canJoin = x.TestSubject.TryJoinTable(
            table.Name,
            playerName,
            out var connection);

        canJoin.Should().BeFalse();
        ValidatePlayerNotAdded(table, x, "", connection);
    }

    public override void WhenTheTableDoesNotExist()
    {
        var x = GetTestData();

        bool canJoin = x.TestSubject.TryJoinTable(
            Fixture.Create<string>(),
            Fixture.Create<string>(),
            out var connection);

        using var _ = new AssertionScope();

        canJoin.Should().BeFalse();
        connection.Should().BeNull();
    }

    public override void WhenTheTableNameIsNullOrEmpty(string? name)
    {
        var table = Fixture.Create<TableData>();
        var x = GetTestData([table]);

        bool canJoin = x.TestSubject.TryJoinTable(
            name!,
            Fixture.Create<string>(),
            out var connection);

        using var _ = new AssertionScope();

        canJoin.Should().BeFalse();
        connection.Should().BeNull();
    }
}
