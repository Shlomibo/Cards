namespace DTOs.Shithead.Moves;

/// <summary>
/// Place card(s) from hand to the discard pile.
/// </summary>
public sealed record PlaceCard : ShitheadMove
{
    /// <summary>
    /// The index of the card(s) in the player's hand to place on the discard pile.
    /// </summary>
    public required int[] CardIndices { get; set; }
}

/// <summary>
/// If the player has a Joker, removes it (one per move) from the player's hand and
/// pass the discard pile to the selected player.
/// </summary>
public sealed record PlaceJoker : ShitheadMove
{
    /// <summary>
    /// The Id of the player to pass the discard pile to.
    /// </summary>
    public int PlayerId { get; set; }
}

/// <summary>
/// Accepts the discard pile.
/// </summary>
public sealed record AcceptDiscardPile : ShitheadMove;

/// <summary>
/// If the player has no cards in hand, reveals the undercard to the player
/// which allows to take it.
/// </summary>
public sealed record RevealUndercard : ShitheadMove
{
    /// <summary>
    /// The index of the undercard to reveal.
    /// </summary>
    public int CardIndex { get; set; }
}

/// <summary>
/// Takes the revealed cards or undercards into the player's hand.
/// </summary>
public sealed record TakeRevealedCards : ShitheadMove
{
    /// <summary>
    /// The index of the card in the player's revealed or undercards lists.
    /// </summary>
    public required int[] CardIndices { get; set; }
}

/// <summary>
/// Takes the revealed cards or undercards into the player's hand.
/// </summary>
public sealed record TakeUndercard : ShitheadMove
{
    /// <summary>
    /// The index of the card in the player's revealed or undercards lists.
    /// </summary>
    public int CardIndex { get; set; }
}

/// <summary>
/// Removes the player from the game.
/// </summary>
public sealed record LeaveGame : ShitheadMove
{
    /// <summary>
    /// The player that is removed.
    /// </summary>
    public int PlayerId { get; set; }
}
