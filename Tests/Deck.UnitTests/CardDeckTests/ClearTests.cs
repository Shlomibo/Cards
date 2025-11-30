using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class ClearTests : CardDeckTestsBase
{
    [Test]
    public void WhenDeckIsEmpty()
    {
        using var x = CreateTestData();

        x.TestSubject.Clear();

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenDeckContainsCards()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards);

        x.TestSubject.Clear();

        x.TestSubject.Should().BeEmpty();
    }
}
