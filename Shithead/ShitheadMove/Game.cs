namespace Shithead.ShitheadMove;

public sealed record PlaceCard : ShitheadMove
{
    /// <summary>
    /// The index of the card in the player's hand
    /// </summary>
    public int[] CardIndices { get; set; } = [];
}

public sealed record PlaceJoker : ShitheadMove
{
    /// <summary>
    /// The Id of the player that accepts the discard pile
    /// </summary>
    public int PlayerId { get; set; }
}

public sealed record AcceptDiscardPile : ShitheadMove;

public sealed record RevealUndercard : ShitheadMove
{
    /// <summary>
    /// The index of the card in the player's hand
    /// </summary>
    public int CardIndex { get; set; }
}

public sealed record TakeUndercards : ShitheadMove
{
    /// <summary>
    /// The index of the card in the player's revealed or undercards lists
    /// </summary>
    public int[] CardIndices { get; set; } = [];
}

public sealed record LeaveGame : ShitheadMove
{
    public int PlayerId { get; set; }
}
