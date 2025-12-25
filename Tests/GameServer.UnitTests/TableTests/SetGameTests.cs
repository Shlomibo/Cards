using System;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

using Moq;

namespace GameServer.UnitTests.TableTests;

public class SetGameTests : TableTestsBase
{
    [Test]
    public void WhenThereIsNoCurrentGameAndSettingNewGameToNull()
    {
        var x = GetTestData();

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeFalse();
            x.TestSubject.Game.Should().BeNull();
        }

        x.TestSubject.Invoking(x => x.SetGame(null))
            .Should().NotThrow();

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeFalse();
            x.TestSubject.Game.Should().BeNull();
        }
    }

    [Test]
    public void WhenThereIsNoCurrentGameAndSettingNewGameToAGame()
    {
        var x = GetTestData();
        var game = CreateGameEngine();

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeFalse();
            x.TestSubject.Game.Should().BeNull();
        }

        x.TestSubject.SetGame(game.Object);

        game.VerifyAdd(
            game => game.Updated += It.IsAny<EventHandler>(),
            Times.Once);

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeTrue();
            x.TestSubject.Game.Should().Be(game.Object);
        }
    }

    [Test]
    public void WhenThereIsAGameAndSettingNewGameToNull()
    {
        var x = GetTestData(setGame: true);

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeTrue();
            x.TestSubject.Game.Should().Be(x.GameEngine!.Object);
        }

        x.TestSubject.SetGame(null);

        x.GameEngine!.VerifyRemove(
            game => game.Updated -= It.IsAny<EventHandler>(),
            Times.Once);

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeFalse();
            x.TestSubject.Game.Should().BeNull();
        }
    }

    [Test]
    public void WhenThereIsAGameAndSettingNewGameToTheSameGame()
    {
        var x = GetTestData(setGame: true);
        x.TestSubject.SetGame(x.GameEngine!.Object);

        x.GameEngine.VerifyAdd(
            game => game.Updated += It.IsAny<EventHandler>(),
            Times.Never);

        x.GameEngine.VerifyRemove(
            game => game.Updated -= It.IsAny<EventHandler>(),
            Times.Never);
    }

    [Test]
    public void WhenThereIsAGameAndSettingNewGameToANewGame()
    {
        var game = CreateGameEngine();
        var x = GetTestData(setGame: true);

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeTrue();
            x.TestSubject.Game.Should().Be(x.GameEngine!.Object);
        }

        x.TestSubject.SetGame(game.Object);

        game.VerifyAdd(
            game => game.Updated += It.IsAny<EventHandler>(),
            Times.Once);

        x.GameEngine!.VerifyRemove(
            game => game.Updated -= It.IsAny<EventHandler>(),
            Times.Once);

        using (new AssertionScope())
        {
            x.TestSubject.GameStarted.Should().BeTrue();
            x.TestSubject.Game.Should().Be(game.Object);
        }
    }
}
