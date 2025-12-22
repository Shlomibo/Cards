using System;

using AutoFixture;

using AwesomeAssertions;

using DTOs;

using GameEngine;

namespace GameServer.UnitTests.TablesManagerTests;

public class CreateTableTests : TablesManagerTestsBase
{
    [Test]
    public async Task WhenCreatingANewTable()
    {
        var x = GetTestData();
        string name = Fixture.Create<string>();
        string masterName = Fixture.Create<string>();

        var connection = x.TestSubject.CreateTable(
            name,
            masterName);
        var table = x.TestSubject.Tables.Should().ContainKey(name)
            .WhoseValue;

        var expectedMaster = ValidateConnection(
            masterName,
            connection,
            table,
            expectedId: 0);

        table.Should().BeEquivalentTo(new
        {
            TableMaster = expectedMaster,
            TableName = name,
            Game = (Engine<GameState, GameState, GameState, GameMove>?)null,
            GameStarted = false,
        });
    }

    [Test]
    public async Task WhenCreatingANewTableWithAnExistingName()
    {
        var existingTable = Fixture.Create<TableData>();
        var x = GetTestData([existingTable]);

        x.TestSubject.Invoking(tm => tm.CreateTable(
            existingTable.Name,
            Fixture.Create<string>()))
            .Should().Throw<InvalidOperationException>();
    }

    [TestCase("")]
    [TestCase((string?)null)]
    public async Task WhenCreatingANewTableWithNullOrEmptyName(string? name)
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.CreateTable(
            name!,
            Fixture.Create<string>()))
            .Should().Throw<ArgumentException>();
    }

    [TestCase("")]
    [TestCase((string?)null)]
    public async Task WhenCreatingANewTableWithMasterNameNullOrEmpty(string? name)
    {
        var x = GetTestData();

        x.TestSubject.Invoking(tm => tm.CreateTable(
            Fixture.Create<string>(),
            name!))
            .Should().Throw<ArgumentException>();
    }
}
