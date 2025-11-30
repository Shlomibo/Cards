using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class IndexOfTests : CardDeckTestsBase
{
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
    public void WhenGettingACard(int index)
    {
        RandomCard[] cards = [.. CreateCards(10)];

        using var x = CreateTestData(cards.Reverse());

        int result = x.TestSubject.IndexOf(cards[index]);

        result.Should().Be(index);
    }

    [Test]
    public void WhenGettingTheOnlyCard()
    {
        RandomCard[] cards = [.. CreateCards(1)];

        using var x = CreateTestData(cards.Reverse());

        int index = x.TestSubject.IndexOf(cards[0]);

        index.Should().Be(0);
    }

    [Test]
    public void WhenGettingNonExistentCard()
    {
        RandomCard[] cards = [];

        using var x = CreateTestData(cards.Reverse());

        int index = x.TestSubject.IndexOf(new RandomCard());

        index.Should().Be(-1);
    }
}
