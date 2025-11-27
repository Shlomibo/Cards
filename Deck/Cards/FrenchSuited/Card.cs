namespace Deck.Cards.FrenchSuited;

public readonly record struct Card : IEquatable<Card>
{
    public const int CardsCount = NoJokersCardsCount + 2;
    public const int NoJokersCardsCount = 52;


    public Value Value { get; }
    public readonly Suit? Suit => Value != Value.Joker
        ? field
        : null;

    public readonly Color Color => ColorBySuit(Suit ?? default);


    public Card(Value value, Suit suit)
    {
        Value = value;
        Suit = suit;
    }

    public static Color ColorBySuit(Suit suit) =>
        (Color)((int)suit & 0x2);

    public static Suit DefaultColorSuit(Color color) =>
        (Suit)(int)color;

    public static Card Create(Value value, Suit suit) =>
        new(value, suit);

    public static Card CreateJoker(Color color) =>
        Create(Value.Joker, DefaultColorSuit(color));

    public static IEnumerable<Card> AllCards(bool excludeJokers = false)
    {
        for (Suit suit = default; suit <= FrenchSuited.Suit.Spades; suit++)
        {
            foreach (var card in FullSuit(suit))
            {
                yield return card;
            }
        }

        if (!excludeJokers)
        {
            yield return CreateJoker(Color.Black);
            yield return CreateJoker(Color.Red);
        }

    }

    public static IEnumerable<Card> FullSuit(Suit suit)
    {
        for (var value = Value.Ace; value <= Value.King; value++)
        {
            yield return Create(value, suit);
        }
    }

    public static IEnumerable<Card> AllSuits(Value value)
    {
        yield return Create(value, FrenchSuited.Suit.Clubs);
        yield return Create(value, FrenchSuited.Suit.Spades);
        yield return Create(value, FrenchSuited.Suit.Diamonds);
        yield return Create(value, FrenchSuited.Suit.Hearts);
    }

    // override object.GetHashCode
    public override readonly int GetHashCode()
    {
        return Value != Value.Joker
            ? (Value, Suit).GetHashCode()
            : (Value, Color).GetHashCode();
    }

    public readonly bool Equals(Card other) =>
        Value switch
        {
            _ when Value != other.Value => false,
            Value.Joker => Color == other.Color,
            _ => Suit == other.Suit,
        };

    public override readonly string ToString()
    {
        var value = Value;
        var suit = Suit;

        return value == Value.Joker
            ? $"{Color} Joker"
            : PrintValue(value) + PrintSuit(suit!.Value);

        static string PrintValue(Value value) =>
            value switch
            {
                Value.Joker => "Joker",
                Value.Ace => "A",
                Value.Two => "2",
                Value.Three => "3",
                Value.Four => "4",
                Value.Five => "5",
                Value.Six => "6",
                Value.Seven => "7",
                Value.Eight => "8",
                Value.Nine => "9",
                Value.Ten => "10",
                Value.Jack => "J",
                Value.Queen => "Q",
                Value.King => "K",
                _ => throw new InvalidCastException("Invalid card value"),
            };

        static string PrintSuit(Suit suit) =>
            suit switch
            {
                FrenchSuited.Suit.Hearts => "♥️",
                FrenchSuited.Suit.Diamonds => "♦️",
                FrenchSuited.Suit.Clubs => "♣️",
                FrenchSuited.Suit.Spades => "♠️",
                _ => throw new InvalidCastException("Invalid card suit"),
            };
    }
}

public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
}

public enum Color
{
    Red,
    // So we can bit-mask from suit to color
    Black = 0x2,
}

public enum Value
{
    Joker,
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
}
