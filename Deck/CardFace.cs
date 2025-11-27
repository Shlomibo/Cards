namespace Deck;

/// <summary>
/// A card face that can be revealed or hidden.
/// </summary>
/// <typeparam name="TCard">The type of the card.</typeparam>
public record CardFace<TCard>
    where TCard : struct
{
#pragma warning disable IDE0032 // Use auto property
    private readonly TCard _card;
#pragma warning restore IDE0032 // Use auto property

    /// <summary>
    /// Gets the card if it is revealed;<br/>
    /// Otherwise, returns the default value.
    /// </summary>
    public TCard Card => IsRevealed
        ? _card
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
        _card = card;
        IsRevealed = isRevealed;
    }
}
