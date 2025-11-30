using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class CopyToTests : CardDeckTestsBase
{
    [Test]
    public void WhenCopyingEmptyDeck()
    {
        using var x = CreateTestData();
        RandomCard[] cards = [.. CreateCards(10)];
        RandomCard[] target = [.. cards];

        x.TestSubject.CopyTo(target, 0);

        target.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenCopyingEmptyDeckToMiddleOfTarget()
    {
        using var x = CreateTestData();
        RandomCard[] cards = [.. CreateCards(10)];
        RandomCard[] target = [.. cards];

        x.TestSubject.CopyTo(target, Random.Next(10));

        target.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenCountGreaterThanArray()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] target = [];

        x.TestSubject.Invoking(deck => deck.CopyTo(target, Random.Next(10)))
            .Should().Throw<ArgumentException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenCountPlusGreaterThanArray()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        RandomCard[] originalTarget = [.. CreateCards(10)];
        RandomCard[] target = [.. originalTarget];

        x.TestSubject.Invoking(deck => deck.CopyTo(target, 1))
            .Should().Throw<ArgumentException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
        target.Should().BeEquivalentTo(originalTarget);
    }

    [Test]
    public void WhenArrayIsNull()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        RandomCard[] target = null!;

        x.TestSubject.Invoking(deck => deck.CopyTo(target, 1))
            .Should().Throw<ArgumentNullException>();

        x.TestSubject.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenCopyingDeckIntoArray()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] target = [.. CreateCards(10)];

        x.TestSubject.CopyTo(target, 0);

        x.TestSubject.Should().BeEquivalentTo(cards);
        target.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void WhenCopyingDeckIntoTheMiddleArray()
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());
        RandomCard[] originalTarget = [.. CreateCards(15)];
        RandomCard[] target = [.. originalTarget];

        x.TestSubject.CopyTo(target, 5);

        x.TestSubject.Should().BeEquivalentTo(cards);
        target.Take(5).Should().BeEquivalentTo(originalTarget.Take(5));
        target.Skip(5).Should().BeEquivalentTo(cards);
    }
}
