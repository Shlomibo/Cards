namespace DTOs.Shithead.Moves;

/// <summary>
/// Removes the selected <paramref name="CardIndex"/> from the player's hand
/// and places it on the revealed cards at <paramref name="TargetIndex"/> location.
/// </summary>
public sealed record SetRevealedCard : ShitheadMove
{
    /// <summary>
    /// Card index in player deck.
    /// </summary>
    public int CardIndex { get; set; }

    /// <summary>
    /// The revealed card index to send the card to.
    /// </summary>
    public int TargetIndex { get; set; }
}

/// <summary>
/// Removes the selected <paramref name="CardIndex"/> from the player's revealed cards
/// and places it back to the player's hand.
/// </summary>
public sealed record UnsetRevealedCard : ShitheadMove
{
    /// <summary>
    /// The index of the revealed card to remove.
    /// </summary>
    public int CardIndex { get; set; }
}

/// <summary>
/// Accepts the revealed cards selection and wait for the others to select theirs.
/// </summary>
public sealed record AcceptSelectedRevealedCards : ShitheadMove;

/// <summary>
/// Reject the previously accepted revealed cards, allowing the player to change their selection.
/// </summary>
public sealed record ReselectRevealedCards : ShitheadMove;
