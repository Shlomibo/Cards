using System;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

namespace Deck.UnitTests.Cards.FrenchSuited.CardTests;

public class AllCardsTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void WhenEnumeratingAllCards(bool includingJokers)
    {
        int expectedCount = includingJokers
            ? 54
            : 52;

        var count = Card.AllCards(excludeJokers: !includingJokers)
            .Distinct()
            .Count();

        count.Should().Be(expectedCount);
    }
}
