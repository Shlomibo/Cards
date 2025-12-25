using System;

namespace GameServer.UnitTests.TableTests;

public abstract class AddPlayerTestsBase : TableTestsBase
{
    [Test]
    public abstract void WhenThePlayerIsNew();

    [Test]
    public abstract void WhenAPlayerWithTheSameNameExists();

    [Test]
    public abstract void WhenTheGameHasStarted();

    [Test]
    public abstract void WhenThePlayerNameIsNull();

    [Test]
    public abstract void WhenThePlayerNameIsEmpty();
}
