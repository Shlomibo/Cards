namespace Shithead.Moves;

/// <summary>
/// Place card(s) from hand to the discard pile.
/// </summary>
/// <param name="CardIndices">The index of the card(s) in the player's hand to place on the discard pile.</param>
public sealed record PlaceCard(int[] CardIndices) : Move;

/// <summary>
/// If the player has a Joker, removes it (one per move) from the player's hand and
/// pass the discard pile to the selected player.
/// </summary>
/// <param name="PlayerId">The Id of the player to pass the discard pile to.</param>
public sealed record PlaceJoker(int PlayerId) : Move;

/// <summary>
/// Accepts the discard pile.
/// </summary>
public sealed record AcceptDiscardPile : Move;

/// <summary>
/// If the player has no cards in hand, reveals the undercard to the player
/// which allows to take it.
/// </summary>
/// <param name="CardIndex">The index of the undercard to reveal.</param>
public sealed record RevealUndercard(int CardIndex) : Move;

/// <summary>
/// Takes the revealed cards or undercards into the player's hand.
/// </summary>
/// <param name="CardIndices">The index of the card in the player's revealed or undercards lists.</param>
public sealed record TakeUndercards(int[] CardIndices) : Move;

/// <summary>
/// Removes the player from the game.
/// </summary>
/// <param name="PlayerId">The player that is removed.</param>
public sealed record LeaveGame(int PlayerId) : Move;
