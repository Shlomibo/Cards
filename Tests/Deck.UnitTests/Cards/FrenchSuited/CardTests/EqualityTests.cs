using System;

using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

namespace Deck.UnitTests.Cards.FrenchSuited.CardTests;

public class EqualityTests
{
    [Test]
    public void WhenTheCardIsJoker()
    {
        var suits = Enum.GetValues<Suit>();

        foreach (var lSuit in suits)
        {
            foreach (var rSuit in suits)
            {
                Card left = new(Value.Joker, lSuit);
                Card right = new(Value.Joker, rSuit);

                bool expectedEquality = left.Color == right.Color;

                EqualityTest(left, right, expectedEquality);
            }
        }
    }

    [Test]
    public void WhenTheCardIsNotJoker()
    {
        Value[] values = [.. Enum.GetValues<Value>()
            .Where(v => v != Value.Joker)];
        var suits = Enum.GetValues<Suit>();

        foreach (var lValue in values)
        {
            foreach (var rValue in values)
            {
                foreach (var lSuit in suits)
                {
                    foreach (var rSuit in suits)
                    {
                        Card left = new(lValue, lSuit);
                        Card right = new(rValue, rSuit);

                        bool expectedEquality = lValue == rValue
                            && lSuit == rSuit;

                        EqualityTest(left, right, expectedEquality);
                    }
                }
            }
        }
    }

    private static void EqualityTest(Card left, Card right, bool expectedEquality)
    {
        left.Equals(right).Should().Be(expectedEquality);
        (left == right).Should().Be(expectedEquality);
        (left != right).Should().Be(!expectedEquality);

        if (expectedEquality)
        {
            left.GetHashCode().Should().Be(right.GetHashCode());
        }
    }
}
