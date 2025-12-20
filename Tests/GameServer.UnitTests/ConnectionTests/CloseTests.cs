using System;

using Moq;

namespace GameServer.UnitTests.ConnectionTests;

public class CloseTests : ConnectionTestsBase
{
    [Test]
    public void WhenClosingTheConnection()
    {
        var x = GetTestData();

        x.TestSubject.Close();

        x.Table.Verify(
            table => table.RemovePlayer(x.ConnectionId),
            Times.Once,
            "The connection removed");
    }

    [Test]
    public void WhenClosingTheConnectionTwice()
    {
        var x = GetTestData();

        x.TestSubject.Close();

        x.Table.Verify(
            table => table.RemovePlayer(
                It.IsAny<Guid>()),
            Times.Once,
            "The connection removed just once");
    }
}
