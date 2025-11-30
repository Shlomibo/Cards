using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class AddTests : CardDeckTestsBase
{
    [Test]
    public void WhenAddingToAnEmptyList()
    {
        using var x = CreateTestData();
        RandomCard newCard = new();

        x.TestSubject.Add(newCard);

        x.TestSubject.Should().ContainSingle()
            .Which.Should().Be(newCard);
    }

    [Test]
    public void WhenAddingToAListWithAnItem()
    {
        RandomCard[] cards = [.. CreateCards(1)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Add(newCard);

        x.TestSubject.Should().BeEquivalentTo([.. cards, newCard]);
    }

    [Test]
    public void WhenAddingToAListWithManyItems()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Add(newCard);

        x.TestSubject.Should().BeEquivalentTo([.. cards, newCard]);
    }

    [Test]
    public void WhenAddingMultipleItems()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] newCards = [.. CreateCards(3)];

        x.TestSubject.Add(newCards);

        x.TestSubject.Should().BeEquivalentTo([.. newCards, .. cards]);
    }

    [Test]
    public void WhenAddingNothing()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] newCards = [];

        x.TestSubject.Add(newCards);

        x.TestSubject.Should().BeEquivalentTo(cards);
    }
}
