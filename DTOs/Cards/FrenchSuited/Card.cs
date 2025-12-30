using System;

namespace DTOs.Cards.FrenchSuited;


/// <summary>
/// A playing card in a French-suited deck.
/// </summary>
public record Card
{
    public Value Value { get; init; }

    /// <summary>
    /// The suit of the card, or <see langword="null"/> if the card is a joker.
    /// </summary>
    public Suit? Suit { get; init; }

    /// <summary>
    /// The color of the card.
    /// </summary>
    public Color Color { get; init; }
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

