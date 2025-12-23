using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

namespace GameServer.UnitTests.TablesManagerTests;

[TestFixture]
public sealed class TryGetTableTests : GetTableTestsBase
{
    public override void WhenGettingEmptyTableName()
    {
        var x = GetTestData();

        bool result = x.TestSubject.TryGetTable("", out var table);

        result.Should().BeFalse();
        table.Should().BeNull();
    }

    public override void WhenGettingExistingTable()
    {
        var tableData = Fixture.Create<TableData>();
        var x = GetTestData([tableData]);

        bool result = x.TestSubject.TryGetTable(tableData.Name, out var table);

        result.Should().BeTrue();
        table.Should().BeEquivalentTo(new
        {
            TableMaster = new
            {
                Id = 0,
                Name = tableData.MasterPlayer,
                State = PlayerState.Playing,
            },
            Players = tableData.AllPlayers.Select((p, i) => new
            {
                Id = i,
                Name = p,
                State = PlayerState.Playing,
            }),
        });
    }

    public override void WhenGettingNonExistentTable()
    {
        var tableData = Fixture.Create<TableData>();
        var x = GetTestData([tableData]);

        bool result = x.TestSubject.TryGetTable(Fixture.Create<string>(), out var table);

        result.Should().BeFalse();
        table.Should().BeNull();
    }

    public override void WhenGettingNullTableName()
    {
        var x = GetTestData();

        bool result = x.TestSubject.TryGetTable(null!, out var table);

        result.Should().BeFalse();
        table.Should().BeNull();
    }
}
