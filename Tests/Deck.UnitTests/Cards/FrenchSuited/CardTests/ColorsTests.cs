using System;
using System.Diagnostics;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

namespace Deck.UnitTests.Cards.FrenchSuited.CardTests;

public class ColorsTests
{
    [Test]
    public void WhenGettingSuitsColor()
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            var expectedColor = suit switch
            {
                Suit.Hearts => Color.Red,
                Suit.Diamonds => Color.Red,
                Suit.Clubs => Color.Black,
                Suit.Spades => Color.Black,
                _ => throw new UnreachableException(),
            };

            Card.ColorBySuit(suit).Should().Be(expectedColor);
        }
    }
}
