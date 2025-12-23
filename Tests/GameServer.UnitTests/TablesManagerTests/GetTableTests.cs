using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

namespace GameServer.UnitTests.TablesManagerTests;

[TestFixture]
public sealed class GetTableTests : GetTableTestsBase
{
    public override void WhenGettingEmptyTableName()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.GetTable(""))
            .Should().Throw<KeyNotFoundException>();
    }

    public override void WhenGettingExistingTable()
    {
        var tableData = Fixture.Create<TableData>();
        var x = GetTestData([tableData]);

        var table = x.TestSubject.GetTable(tableData.Name);

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

        x.TestSubject.Invoking(tm => tm.GetTable(Fixture.Create<string>()))
            .Should().Throw<KeyNotFoundException>();
    }

    public override void WhenGettingNullTableName()
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.GetTable(null!))
            .Should().Throw<ArgumentNullException>();
    }
}
