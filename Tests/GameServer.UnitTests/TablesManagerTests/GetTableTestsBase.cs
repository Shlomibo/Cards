using System;

namespace GameServer.UnitTests.TablesManagerTests;

public abstract class GetTableTestsBase : TablesManagerTestsBase
{
    [Test]
    public abstract void WhenGettingNullTableName();
    [Test]
    public abstract void WhenGettingEmptyTableName();
    [Test]
    public abstract void WhenGettingNonExistentTable();
    [Test]
    public abstract void WhenGettingExistingTable();
}
