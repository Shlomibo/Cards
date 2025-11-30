using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class AsReadonlyTests : CardDeckTestsBase
{
    [Test]
    public void WhenGettingReadonlyDeckOfDeck()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        var ro = x.TestSubject.AsReadonly();

        ro.Should().NotBeOfType<IDeck<RandomCard>>()
            .And.NotBeOfType<IList<RandomCard>>()
            .And.NotBeOfType<ICollection<RandomCard>>()
            .And.BeEquivalentTo(cards);
    }
}
