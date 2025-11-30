using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class TopTests : CardDeckTestsBase
{
    [Test]
    public void WhenDeckHasManyCardsAndTryingToPop()
    {
        RandomCard[] cards = [.. CreateCards(10)];

        using var x = CreateTestData(cards);

        bool hasTop = x.TestSubject.TryPop(out var popped);

        hasTop.Should().BeTrue();
        popped.Should().Be(cards[^1]);

        x.TestSubject.Should().BeEquivalentTo(cards.Take(cards.Length - 1));
    }

    [Test]
    public void WhenDeckHasOneCardAndTryingToPop()
    {
        RandomCard[] cards = [.. CreateCards(1)];

        using var x = CreateTestData(cards);

        bool hasTop = x.TestSubject.TryPop(out var popped);

        hasTop.Should().BeTrue();
        popped.Should().Be(cards[0]);

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenDeckHasNoCardsAndTryingToPop()
    {
        RandomCard[] cards = [];

        using var x = CreateTestData(cards);

        bool hasTop = x.TestSubject.TryPop(out var popped);

        hasTop.Should().BeFalse();
        popped.Should().Be(default(RandomCard));

        x.TestSubject.Should().BeEmpty();
    }
    [Test]
    public void WhenDeckHasManyCardsAndPopping()
    {
        RandomCard[] cards = [.. CreateCards(10)];

        using var x = CreateTestData(cards);

        var popped = x.TestSubject.Pop();

        popped.Should().Be(cards[^1]);

        x.TestSubject.Should().BeEquivalentTo(cards.Take(cards.Length - 1));
    }

    [Test]
    public void WhenDeckHasOneCardAndPopping()
    {
        RandomCard[] cards = [.. CreateCards(1)];

        using var x = CreateTestData(cards);

        var popped = x.TestSubject.Pop();

        popped.Should().Be(cards[0]);

        x.TestSubject.Should().BeEmpty();
    }

    [Test]
    public void WhenDeckHasNoCardsAndPopping()
    {
        RandomCard[] cards = [];

        using var x = CreateTestData(cards);

        x.TestSubject.Invoking(deck => deck.Pop())
            .Should().Throw<InvalidOperationException>();

        x.TestSubject.Should().BeEmpty();
    }
}
