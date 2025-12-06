using System;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

namespace Deck.UnitTests.Cards.FrenchSuited.CardTests;

public class SuitsEnumerationTests
{
    [Test]
    public void WhenGettingAllSuitsOfANonJokerValue()
    {
        foreach (var value in Enum.GetValues<Value>().Where(v => v != Value.Joker))
        {
            var allSuits = Card.AllSuits(value)
                .ToArray();

            Card[] expectedCards = [.. Enum.GetValues<Suit>()
                .Select(s => Card.GetCard(value, s))];

            foreach (var expectedCard in expectedCards)
            {
                allSuits.Should().Contain(expectedCard);
            }
        }
    }

    [Test]
    public void WhenGettingAllSuitsOfAJoker()
    {
        var allSuits = Card.AllSuits(Value.Joker)
            .ToArray();

        IEnumerable<Card> expectedCards = Enum.GetValues<Color>()
                .Select(c => Card.GetJoker(c));

        foreach (var expectedCard in expectedCards)
        {
            allSuits.Should().Contain(expectedCard);
        }
    }
}
