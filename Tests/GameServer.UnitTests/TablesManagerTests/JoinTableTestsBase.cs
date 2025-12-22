using System;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

namespace GameServer.UnitTests.TablesManagerTests;

public abstract class JoinTableTestsBase : TablesManagerTestsBase
{
    [Test]
    public abstract void WhenTheTableDoesNotExist();

    [Test]
    public abstract void WhenThePlayerNameIsTaken();

    [Test]
    public abstract void WhenThePlayerNameIsAvailableButTheGameHasStarted();

    [Test]
    public abstract void WhenThePlayerNameIsAvailable();

    [TestCase("")]
    [TestCase((string?)null)]
    public abstract void WhenTheTableNameIsNullOrEmpty(string? name);

    [TestCase("")]
    [TestCase((string?)null)]
    public abstract void WhenThePlayerNameIsNullOrEmpty(string? name);

    private protected static void ValidatePlayerNotAdded(
        TableData table,
        TestData x,
        string playerName,
        Connection<
            GameState,
            GameState,
            GameState,
            GameMove,
            GameState.Serialized,
            GameMove.Serialized>? connection = null)
    {
        using var _ = new AssertionScope();

        connection.Should().BeNull();
        x.TestSubject.Tables.Values.First().AsTableDescriptor()
            .Players.Should().HaveCount(table.AllPlayers.Count)
            .And.NotContainEquivalentOf(new
            {
                Name = playerName
            });
    }
}
