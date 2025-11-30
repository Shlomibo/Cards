using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class RemoveAtTests : CardDeckTestsBase
{
    [Test]
    public void WhenDeckIsEmpty()
    {
        using var x = CreateTestData();

        x.TestSubject.Invoking(deck => deck.RemoveAt(0))
            .Should().Throw<IndexOutOfRangeException>();
    }

    [Test]
    public void WhenDeckHasOneItem()
    {
        RandomCard[] cards = [.. CreateCards(1)];

        using var x = CreateTestData(cards);

        x.TestSubject.RemoveAt(0);

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenIndexIsNegative()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Invoking(deck => deck.RemoveAt(-1))
            .Should().Throw<IndexOutOfRangeException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenIndexIsAtCount()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Invoking(deck => deck.RemoveAt(cards.Length))
            .Should().Throw<IndexOutOfRangeException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenIndexIsAboveCount()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.Invoking(deck => deck.RemoveAt(cards.Length))
            .Should().Throw<IndexOutOfRangeException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    public void WhenRemoveCardFromADeck(int index)
    {
        RandomCard[] cards = [.. CreateCards(10)];

        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.RemoveAt(index);

        x.TestSubject.Should().BeEquivalentTo(ExpectedCards());

        IEnumerable<RandomCard> ExpectedCards()
        {
            for (int i = 0; i < cards.Length - 1; i++)
            {
                yield return i switch
                {
                    _ when i < index => cards[i],
                    _ => cards[i + 1]
                };
            }
        }
    }
}
