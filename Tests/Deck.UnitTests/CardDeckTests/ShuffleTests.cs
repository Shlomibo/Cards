using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class ShuffleTests : CardDeckTestsBase
{
    [Test]
    public void WhenShufflingAnEmptyDeck()
    {
        using var x = CreateTestData();

        x.TestSubject.Shuffle();

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenShufflingADeck()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.SequenceEqual(cards).Should().BeTrue();
        x.TestSubject.Shuffle();

        x.TestSubject.SequenceEqual(cards).Should().BeFalse();

        foreach (var card in cards)
        {
            x.TestSubject.Should().Contain(card);
        }
    }
}
