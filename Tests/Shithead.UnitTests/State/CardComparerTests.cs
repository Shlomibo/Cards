using AwesomeAssertions;

using Deck.Cards.FrenchSuited;

using NUnit.Framework.Internal;

using Shithead.State;

namespace Shithead.UnitTests.State;

public class CardComparerTests
{
    private static readonly Value[] _normalWildCards = [Value.Two, Value.Three, Value.Ten];
    private static readonly Value[] _faceValueCards = [.. Enum.GetValues<Value>()
        .Where(v => !_normalWildCards.Contains(v)
            && v != Value.Joker)];
    private readonly CardComparer _testSubject = CardComparer.Instance;

    private static Randomizer Random => TestContext.CurrentContext.Random;

    [Test]
    public void WhenComparingWildCards()
    {
        CardComparer.WildCards.Should().BeEquivalentTo([Value.Joker, .. _normalWildCards]);

        foreach (var wildcard in _normalWildCards)
        {
            _testSubject.Compare(wildcard, Value.Joker).Should().BePositive();
            _testSubject.Compare(Value.Joker, wildcard).Should().BeNegative();

            Card card = new(wildcard, Random.NextEnum<Suit>());
            Card joker = new(Value.Joker, Random.NextEnum<Suit>());

            _testSubject.Compare(card, joker).Should().BePositive();
            _testSubject.Compare(joker, card).Should().BeNegative();

            foreach (var other in _normalWildCards)
            {
                _testSubject.Compare(wildcard, other).Should().Be(0);
                _testSubject.Compare(other, wildcard).Should().Be(0);

                Card otherCard = new(other, Random.NextEnum<Suit>());

                _testSubject.Compare(card, otherCard).Should().Be(0);
                _testSubject.Compare(otherCard, card).Should().Be(0);
            }

            foreach (var other in _faceValueCards)
            {
                _testSubject.Compare(wildcard, other).Should().BePositive();
                _testSubject.Compare(other, wildcard).Should().BeNegative();

                Card otherCard = new(other, Random.NextEnum<Suit>());

                _testSubject.Compare(card, otherCard).Should().BePositive();
                _testSubject.Compare(otherCard, card).Should().BeNegative();
            }
        }
    }

    [Test]
    public void WhenComparingNormalCards()
    {
        CardComparer.WildCards.Should().BeEquivalentTo([Value.Joker, .. _normalWildCards]);

        foreach (var cardValue in _faceValueCards)
        {
            _testSubject.Compare(cardValue, Value.Joker).Should().BePositive();
            _testSubject.Compare(Value.Joker, cardValue).Should().BeNegative();

            Card card = new(cardValue, Random.NextEnum<Suit>());
            Card joker = new(Value.Joker, Random.NextEnum<Suit>());

            _testSubject.Compare(card, joker).Should().BePositive();
            _testSubject.Compare(joker, card).Should().BeNegative();

            var value = CardComparer.CardValueRank[cardValue];

            foreach (var other in _normalWildCards)
            {

                _testSubject.Compare(cardValue, other).Should().BeNegative();
                _testSubject.Compare(other, cardValue).Should().BePositive();

                Card otherCard = new(other, Random.NextEnum<Suit>());

                _testSubject.Compare(card, otherCard).Should().BeNegative();
                _testSubject.Compare(otherCard, card).Should().BePositive();
            }

            foreach (var other in _faceValueCards)
            {
                var otherValue = CardComparer.CardValueRank[other];
                Card otherCard = new(other, Random.NextEnum<Suit>());

                switch (value - otherValue)
                {
                    case < 0:
                        _testSubject.Compare(cardValue, other).Should().BeNegative();
                        _testSubject.Compare(other, cardValue).Should().BePositive();

                        _testSubject.Compare(card, otherCard).Should().BeNegative();
                        _testSubject.Compare(otherCard, card).Should().BePositive();
                        break;
                    case 0:
                        _testSubject.Compare(cardValue, other).Should().Be(0);
                        _testSubject.Compare(other, cardValue).Should().Be(0);

                        _testSubject.Compare(card, otherCard).Should().Be(0);
                        _testSubject.Compare(otherCard, card).Should().Be(0);
                        break;
                    case > 0:
                        _testSubject.Compare(cardValue, other).Should().BePositive();
                        _testSubject.Compare(other, cardValue).Should().BeNegative();

                        _testSubject.Compare(card, otherCard).Should().BePositive();
                        _testSubject.Compare(otherCard, card).Should().BeNegative();
                        break;
                }
            }
        }
    }

    [Test]
    public void WhenComparingJokers()
    {
        var joker1 = Card.GetJoker(Random.NextEnum<Color>());
        var joker2 = Card.GetJoker(Random.NextEnum<Color>());

        _testSubject.Compare(Value.Joker, Value.Joker).Should().Be(0);
        _testSubject.Compare(joker1, joker2).Should().Be(0);
    }
}
