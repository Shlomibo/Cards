using System;
using System.Diagnostics;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

namespace Deck.UnitTests.Cards.FrenchSuited.CardTests;

public class CreationTests
{
    [Test]
    public void WhenCreatingNormalCards()
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var value in Enum.GetValues<Value>().Where(v => v != Value.Joker))
            {
                var expectedColor = suit switch
                {
                    Suit.Hearts => Color.Red,
                    Suit.Clubs => Color.Black,
                    Suit.Diamonds => Color.Red,
                    Suit.Spades => Color.Black,
                    _ => throw new UnreachableException(),
                };

                var ctorCard = new Card(value, suit);
                var createdCard = Card.GetCard(value, suit);

                var expectedCard = new
                {
                    Value = value,
                    Suit = suit,
                    Color = expectedColor,
                };

                ctorCard.Should().BeEquivalentTo(expectedCard);
                createdCard.Should().BeEquivalentTo(expectedCard);
            }
        }
    }

    [Test]
    public void WhenCreatingJoker()
    {
        foreach (var color in Enum.GetValues<Color>())
        {
            var joker = Card.GetJoker(color);
            var expectedCard = new
            {
                Value = Value.Joker,
                Color = color,
            };

            joker.Should().BeEquivalentTo(expectedCard);
        }
    }
}
