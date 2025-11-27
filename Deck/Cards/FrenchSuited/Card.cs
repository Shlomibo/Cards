using System.Diagnostics;

namespace Deck.Cards.FrenchSuited;

/// <summary>
/// A playing card in a French-suited deck.
/// </summary>
public readonly record struct Card : IEquatable<Card>
{
    /// <summary>
    /// The total number of cards in a standard French-suited deck, including jokers.
    /// </summary>
    public const int CardsCount = NoJokersCardsCount + 2;

    /// <summary>
    /// The total number of cards in a standard French-suited deck, excluding jokers.
    /// </summary>
    public const int NoJokersCardsCount = 52;

    private readonly Suit _suit;

    /// <summary>
    /// The value of the card.
    /// </summary>
    public Value Value { get; }

    /// <summary>
    /// The suit of the card, or <see langword="null"/> if the card is a joker.
    /// </summary>
    public readonly Suit? Suit => Value != Value.Joker
        ? _suit
        : null;

    /// <summary>
    /// The color of the card.
    /// </summary>
    public readonly Color Color => ColorBySuit(_suit);

    /// <summary>
    /// Creates a new card with the specified value and suit.
    /// </summary>
    /// <param name="value">The value of the card.</param>
    /// <param name="suit">
    /// The suit of the card.
    /// If the value is <see cref="Value.Joker"/>, the suit determines the color of the joker.
    /// </param>
    public Card(Value value, Suit suit)
    {
        Value = value;
        _suit = suit;
    }

    /// <summary>
    /// Gets the color associated with a suit.
    /// </summary>
    /// <param name="suit">The suit to get the color of.</param>
    /// <returns>The color associated with the suit.</returns>
    public static Color ColorBySuit(Suit suit) =>
        (Color)((int)suit & 0x2);

    /// <summary>
    /// Gets the default suit for a given color.
    /// </summary>
    /// <param name="color">The color to get a suit for.</param>
    /// <returns>A suit which the color is associated with.</returns>
    public static Suit DefaultColorSuit(Color color) =>
        (Suit)(int)color;

    /// <summary>
    /// Creates a new card with the specified value and suit.
    /// </summary>
    /// <param name="value">The value of the card.</param>
    /// <param name="suit">The suit of the card.</param>
    /// <returns>A card with the specified <paramref name="value"/> and <paramref name="suit"/>.</returns>
    public static Card CreateCard(Value value, Suit suit)
    {
        return new Card(value, suit);
    }

    /// <summary>
    /// Creates a joker card with the specified color.
    /// </summary>
    /// <param name="color">The color of the joker.</param>
    /// <returns>A joker card.</returns>
    public static Card GetJoker(Color color) =>
        CreateCard(Value.Joker, DefaultColorSuit(color));

    /// <summary>
    /// Enumerates all cards in a standard French-suited deck.
    /// </summary>
    /// <remarks>
    /// The enumeration order is by suit, starting with <see cref="Suit.Hearts"/>,
    /// then by value, starting with <see cref="Value.Ace"/> through <see cref="Value.King"/>.
    /// <para/>
    /// Lastly, if jokers are included, the black joker is returned first, followed by the red joker.
    /// </remarks>
    /// <param name="excludeJokers">
    /// If set to <see langword="true"/>, jokers will be excluded from the enumeration.
    /// </param>
    /// <returns>An enumerable that enumerates all the cards in a standard French-suited deck.</returns>
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
            yield return GetJoker(Color.Black);
            yield return GetJoker(Color.Red);
        }

    }

    /// <summary>
    /// Enumerates all cards of a specific suit from <see cref="Value.Ace"/> through <see cref="Value.King"/>.
    /// </summary>
    /// <param name="suit">The suit to enumerate.</param>
    /// <returns>An enumerator that enumerates all the cards in the <paramref name="suit"/>.</returns>
    public static IEnumerable<Card> FullSuit(Suit suit)
    {
        for (var value = Value.Ace; value <= Value.King; value++)
        {
            yield return CreateCard(value, suit);
        }
    }

    /// <summary>
    /// Enumerates all cards of a specific value in all four suits.
    /// </summary>
    /// <param name="value">The value to enumerate.</param>
    /// <returns>An enumerable that enumerates all the suits of the specified <paramref name="value"/>.</returns>
    public static IEnumerable<Card> AllSuits(Value value)
    {
        if (value == Value.Joker)
        {
            yield return GetJoker(Color.Black);
            yield return GetJoker(Color.Red);
            yield break;
        }

        yield return CreateCard(value, FrenchSuited.Suit.Clubs);
        yield return CreateCard(value, FrenchSuited.Suit.Spades);
        yield return CreateCard(value, FrenchSuited.Suit.Diamonds);
        yield return CreateCard(value, FrenchSuited.Suit.Hearts);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return Value != Value.Joker
            ? (Value, Suit).GetHashCode()
            : (Value, Color).GetHashCode();
    }

    /// <inheritdoc/>
    public readonly bool Equals(Card other)
    {
        if (Value != other.Value)
        {
            return false;
        }

        return Value != Value.Joker
            ? Suit == other.Suit
            : Color == other.Color;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        const string ACE_OF_SPACES = "🂡";
        const string ACE_OF_HEARTS = "🂱";
        const string ACE_OF_DIAMONDS = "🃁";
        const string ACE_OF_CLUBS = "🃑";

        return Value switch
        {
            Value.Joker when Color == Color.Red => "🂿",
            Value.Joker => "🃏",
            Value.Ace => SuitAce(Suit!.Value),
            _ => CharFor(SuitAce(Suit!.Value), Value),
        };

        static string CharFor(string suitAce, Value value) =>
            new([suitAce[0], (char)(suitAce[1] + (int)value), default]);

        static string SuitAce(Suit suit) =>
            suit switch
            {
                FrenchSuited.Suit.Spades => ACE_OF_SPACES,
                FrenchSuited.Suit.Hearts => ACE_OF_HEARTS,
                FrenchSuited.Suit.Diamonds => ACE_OF_DIAMONDS,
                FrenchSuited.Suit.Clubs => ACE_OF_CLUBS,
                _ => throw new UnreachableException("Unrecognized suit."),
            };
    }
}

/// <summary>
/// The suit of a French-suited playing card.
/// </summary>
public enum Suit
{
    /// <summary>
    /// Hearts suit as 🂱.
    /// </summary>
    Hearts,

    /// <summary>
    /// Diamonds suit as 🃁.
    /// </summary>
    Diamonds,

    /// <summary>
    /// Clubs suit as 🃑.
    /// </summary>
    Clubs,

    /// <summary>
    /// Spades suit as 🂡.
    /// </summary>
    Spades,
}

/// <summary>
/// The color of a French-suited playing card.
/// </summary>
public enum Color
{
    /// <summary>
    /// Red color.
    /// </summary>
    Red,

    // So we can bit-mask from suit to color
    /// <summary>
    /// Black color.
    /// </summary>
    Black = 0x2,
}

/// <summary>
/// The value of a French-suited playing card.
/// </summary>
public enum Value
{
    /// <summary>
    /// Joker card as 🃏 or 🂿.
    /// </summary>
    Joker,

    /// <summary>
    /// Ace card as 🂡, 🂱, 🃁, or 🃑.
    /// </summary>
    Ace,

    /// <summary>
    /// Two card as 🂢, 🂲, 🃂, or 🃒.
    /// </summary>
    Two,

    /// <summary>
    /// Three card as 🂣, 🂳, 🃃, or 🃓.
    /// </summary>
    Three,

    /// <summary>
    /// Four card as 🂤, 🂴, 🃄, or 🃔.
    /// </summary>
    Four,

    /// <summary>
    /// Five card as 🂥, 🂵, 🃅, or 🃕.
    /// </summary>
    Five,

    /// <summary>
    /// Six card as 🂦, 🂶, 🃆, or 🃖.
    /// </summary>
    Six,

    /// <summary>
    /// Seven card as 🂧, 🂷, 🃇, or 🃗.
    /// </summary>
    Seven,

    /// <summary>
    /// Eight card as 🂨, 🂸, 🃈, or 🃘.
    /// </summary>
    Eight,

    /// <summary>
    /// Nine card as 🂩, 🂹, 🃉, or 🃙.
    /// </summary>
    Nine,

    /// <summary>
    /// Ten card as 🂪, 🂺, 🃊, or 🃚.
    /// </summary>
    Ten,

    /// <summary>
    /// Jack card as 🂫, 🂻, 🃋, or 🃛.
    /// </summary>
    Jack,

    /// <summary>
    /// Queen card as 🂭, 🂽, 🃍, or 🃝.
    /// </summary>
    Queen,

    /// <summary>
    /// King card as 🂮, 🂾, 🃎, or 🃞.
    /// </summary>
    King,
}
