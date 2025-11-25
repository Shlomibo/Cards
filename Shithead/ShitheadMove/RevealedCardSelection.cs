namespace Shithead.ShitheadMove;

public sealed record RevealedCardSelection : ShitheadMove
{
    /// <summary>
    /// Card index in player deck
    /// </summary>
    public int CardIndex { get; set; }

    /// <summary>
    /// The revealed card index to send the card to
    /// </summary>
    public int TargetIndex { get; set; }
}

public sealed record UnsetRevealedCard : ShitheadMove
{
    /// <summary>
    /// Card index in player revealed deck
    /// </summary>
    public int CardIndex { get; set; }
}

public sealed record AcceptSelectedRevealedCards : ShitheadMove;

public sealed record ReselectRevealedCards : ShitheadMove;
