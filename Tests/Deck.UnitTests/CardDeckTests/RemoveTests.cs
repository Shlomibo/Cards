using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class RemoveTests : CardDeckTestsBase
{
    [Test]
    public void WhenDeckIsEmpty()
    {
        using var x = CreateTestData();

        x.TestSubject.Remove(new RandomCard());

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenDeckDoesNotContainTheCard()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Remove(new RandomCard());

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenDeckContainsTheCard()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        var card = cards[Random.Next(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Remove(card);

        x.TestSubject.Should().BeEquivalentTo(cards.Where(c => c != card));
    }

    [Test]
    public void WhenDeckContainsTheTwice()
    {
        RandomCard card = new();


        RandomCard[] cards = [
            card,
            .. CreateCards(8),
            card];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Remove(card);

        x.TestSubject.Should().BeEquivalentTo(cards.Skip(1),
            "only the first instance is removed");
    }
}
