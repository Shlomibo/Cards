using System;
using DTOs;

namespace GamesClient.Client;

/// <summary>
/// A connection to a game table.
/// </summary>
/// <typeparam name="TState">The type of the game state.</typeparam>
/// <typeparam name="TMove">The type of the game move.</typeparam>
public interface IConnection<TState, TMove> : IDisposable
    where TState : State
{
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    string TableName { get; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    string PlayerName { get; }

    /// <summary>
    /// An observable stream of game state updates.
    /// </summary>
    IObservable<StateUpdate<TState>> GameState { get; }

    /// <summary>
    /// Plays a move in the game.
    /// </summary>
    /// <param name="move">The move to play.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PlayMove(TMove move, CancellationToken cancellation);

    /// <summary>
    /// Starts the game if the current table is the table-master.
    /// </summary>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartGame(CancellationToken cancellation);
}
