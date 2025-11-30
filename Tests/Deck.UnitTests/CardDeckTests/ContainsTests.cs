using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class ContainsTests : CardDeckTestsBase
{
    [Test]
    public void WhenDeckHasTheCard()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Should().Contain(cards[Random.Next(0, 10)]);

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenDeckDoesntHaveTheCard()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Should().NotContain(new RandomCard());

        x.TestSubject.Should().BeEquivalentTo(cards);
    }
}
