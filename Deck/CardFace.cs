namespace Deck;

public sealed class CardFace<TCard>
    where TCard : new()
{
    #region Fields

#pragma warning disable IDE0032 // Use auto property
    private readonly TCard _card;
#pragma warning restore IDE0032 // Use auto property
    #endregion

    #region Properties

    public TCard Card => IsRevealed
        ? _card
        : new TCard();
    public bool IsRevealed { get; set; }
    #endregion

    #region Ctors
    public CardFace(TCard card, bool isRevealed = false)
    {
        _card = card;
        IsRevealed = isRevealed;
    }
    #endregion
}
