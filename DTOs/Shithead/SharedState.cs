using System;
using DTOs.Cards.FrenchSuited;

namespace DTOs.Shithead;

/// <summary>
/// The state that is shared and known to all players.
/// </summary>
public class SharedState
{
    /// <summary>
    /// The current game state.
    /// </summary>
    public GameState GameState { get; init; }

    /// <summary>
    /// The states of all players in the game.
    /// </summary>
    public required OtherPlayersState[] Players { get; init; }

    /// <summary>
    /// The IDs of the active players in the game.
    /// </summary>
    public required int[] ActivePlayers { get; init; }

    /// <summary>
    /// The amount of cards left in the deck.
    /// </summary>
    public int DeckSize { get; init; }

    /// <summary>
    /// The discard pile of cards.
    /// </summary>
    public required Card[] DiscardPile { get; init; }

    /// <summary>
    /// The ID of the player whose turn it is.
    /// </summary>
    public int CurrentTurnPlayer { get; init; }

    /// <summary>
    /// The last attempted move in the game, or <see langword="null"/> if none.
    /// </summary>
    public AttemptedMove? LastMove { get; init; }

    /// <summary>
    /// The last successfully played move in the game, or <see langword="null"/> if none.
    /// </summary>
    public AttemptedMove? LastPlayedMove { get; init; }
}
