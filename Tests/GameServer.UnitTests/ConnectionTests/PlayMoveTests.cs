using System;

using AutoFixture;

using Moq;

namespace GameServer.UnitTests.ConnectionTests;

public class PlayMoveTests : ConnectionTestsBase
{
    [Test]
    [Repeat(100)]
    public void PlayMove()
    {
        var move = Fixture.Create<GameMove.Serialized>();
        var x = GetTestData();

        x.TestSubject.PlayMove(move);

        x.Table.Verify(
            table => table.PlayMove(
                It.Is<GameMove>(gm => Matches(gm, move.Deserialize())),
                0),
            Times.Once,
            "The move was deserialized and passed to the table.");
    }
}
