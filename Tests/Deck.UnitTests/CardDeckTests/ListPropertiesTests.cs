using System;

using AwesomeAssertions;

namespace Deck.UnitTests.CardDeckTests;

public class ListPropertiesTests : CardDeckTestsBase
{
    [Test]
    public void WhenTheDeckIsEmpty()
    {
        using var x = CreateTestData();

        x.TestSubject.Should().BeEmpty();
        x.TestSubject.As<IList<RandomCard>>().IsReadOnly.Should().BeFalse();

        x.TestSubject.Invoking(deck => deck[-1])
            .Should().Throw<IndexOutOfRangeException>("getting any negative index throws");

        x.TestSubject.Invoking(deck => deck[-1] = new RandomCard())
        .Should().Throw<IndexOutOfRangeException>("getting any negative index throws");

        x.TestSubject.Invoking(deck => deck[0])
            .Should().Throw<IndexOutOfRangeException>("getting any index throws");

        x.TestSubject.Invoking(deck => deck[0] = new RandomCard())
            .Should().Throw<IndexOutOfRangeException>("setting any index throws");
    }

    [Test]
    public void WhenTheDeckHasOneCard()
    {
        RandomCard[] cards = [.. CreateCards(1)];
        using var x = CreateTestData(cards);

        x.TestSubject.As<IList<RandomCard>>().IsReadOnly.Should().BeFalse();

        x.TestSubject.Invoking(deck => deck[-1])
            .Should().Throw<IndexOutOfRangeException>("getting any negative index throws");
        x.TestSubject.Invoking(deck => deck[-1] = new RandomCard())
            .Should().Throw<IndexOutOfRangeException>("setting any negative index throws");

        x.TestSubject.Should().ContainSingle()
            .Which.Should().Be(cards[0]);

        RandomCard newCard = new();
        (x.TestSubject[0] = newCard).Should().Be(newCard);
        x.TestSubject.Should().HaveElementAt(0, newCard);


        x.TestSubject.Invoking(deck => deck[1])
            .Should().Throw<IndexOutOfRangeException>("getting any index above 0 throws");
        x.TestSubject.Invoking(deck => deck[1] = new RandomCard())
            .Should().Throw<IndexOutOfRangeException>("setting any index above 0 throws");
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
    public void WhenTheDeckHasCards(int index)
    {
        RandomCard[] cards = [.. CreateCards(10)];
        using var x = CreateTestData(cards.Reverse());

        x.TestSubject.As<IList<RandomCard>>().IsReadOnly.Should().BeFalse();

        x.TestSubject.Invoking(deck => deck[-1])
            .Should().Throw<IndexOutOfRangeException>("getting any negative index throws");
        x.TestSubject.Invoking(deck => deck[-1] = new RandomCard())
            .Should().Throw<IndexOutOfRangeException>("setting any negative index throws");

        x.TestSubject.Should().HaveElementAt(index, cards[index]);

        RandomCard newCard = new();
        (x.TestSubject[index] = newCard).Should().Be(newCard);
        x.TestSubject.Should().HaveElementAt(index, newCard);


        x.TestSubject.Invoking(deck => deck[10])
            .Should().Throw<IndexOutOfRangeException>("getting any index above 9 throws");
        x.TestSubject.Invoking(deck => deck[10] = new RandomCard())
            .Should().Throw<IndexOutOfRangeException>("setting any index above 9 throws");
    }
}
