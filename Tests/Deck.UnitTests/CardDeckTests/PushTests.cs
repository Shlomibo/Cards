using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class PushTests : CardDeckTestsBase
{
    [Test]
    public void WhenPushingASingleItem()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Push(newCard);

        x.TestSubject.Should().BeEquivalentTo([.. cards, newCard]);
    }

    [Test]
    public void WhenPushingMultipleItems()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] newCards = [.. CreateCards(3)];

        x.TestSubject.Push(newCards);

        x.TestSubject.Should().BeEquivalentTo([.. cards, .. newCards]);
    }

    [Test]
    public void WhenPushingNothing()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] newCards = [];

        x.TestSubject.Push(newCards);

        x.TestSubject.Should().BeEquivalentTo(cards);
    }
}
