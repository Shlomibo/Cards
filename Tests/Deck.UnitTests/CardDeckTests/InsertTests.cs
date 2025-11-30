using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class InsertTests : CardDeckTestsBase
{
    [Test]
    public void WhenInsertingToIndexBelowZero()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Invoking(deck => deck.Insert(-1, new RandomCard()))
            .Should().Throw<IndexOutOfRangeException>();

        x.TestSubject.Should().BeEquivalentTo(cards, "deck was not changed");
    }

    [Test]
    public void WhenInsertingToIndexZero()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Insert(0, newCard);

        x.TestSubject.Skip(1).Should().BeEquivalentTo(cards, "rest of deck was not changed");
        x.TestSubject.Should().HaveElementAt(0, newCard, "first card is new card");
    }

    [Test]
    public void WhenInsertingToIndexAtCount()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Insert(cards.Length, newCard);

        x.TestSubject.Take(10).Should().BeEquivalentTo(cards, "the beginning of the deck was not changed");
        x.TestSubject.Should().HaveElementAt(cards.Length, newCard, "last card is new card");
    }

    [Test]
    public void WhenInsertingToIndexAboveCount()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Invoking(deck => deck.Insert(cards.Length + 1, new RandomCard()))
            .Should().Throw<IndexOutOfRangeException>();

        x.TestSubject.Should().BeEquivalentTo(cards, "deck was not changed");
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    public void WhenInsertingToIndexInBetween(int index)
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard newCard = new();

        x.TestSubject.Insert(index, newCard);

        x.TestSubject.Should().BeEquivalentTo(ExpectedCardsEnumeration());

        IEnumerable<RandomCard> ExpectedCardsEnumeration()
        {
            for (int i = 0; i <= cards.Length; i++)
            {
                yield return i switch
                {
                    _ when i < index => cards[i],
                    _ when i == index => newCard,
                    _ => cards[i - 1],
                };
            }
        }
    }
}
