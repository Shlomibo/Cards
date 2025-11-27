namespace Deck;

/// <summary>
/// A card face that can be revealed or hidden.
/// </summary>
/// <typeparam name="TCard">The type of the card.</typeparam>
public sealed record CardFace<TCard>
    where TCard : struct
{
    /// <summary>
    /// Gets the card if it is revealed;<br/>
    /// Otherwise, returns the default value.
    /// </summary>
    public TCard Card => IsRevealed
        ? field
        : default;

    /// <summary>
    /// Gets or sets a value indicating whether the card is revealed.
    /// </summary>
    public bool IsRevealed { get; set; }

    /// <summary>
    /// Creates a new card face.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="isRevealed"></param>
    public CardFace(TCard card, bool isRevealed = false)
    {
        Card = card;
        IsRevealed = isRevealed;
    }

    /// <summary>
    /// Converts the card face to the underlying card.
    /// </summary>
    /// <param name="cardFace">The card face to convert.</param>
    public static explicit operator TCard(CardFace<TCard> cardFace) => cardFace.Card;

    /// <summary>
    /// Converts the card to a hidden card face.
    /// </summary>
    /// <param name="card">The card to convert.</param>
    public static implicit operator CardFace<TCard>(TCard card) => new(card);
}
