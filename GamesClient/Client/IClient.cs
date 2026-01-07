using System;
using DTOs;
using DTOs.Responses;

namespace GamesClient.Client;

/// <summary>
/// A client for connecting to a game server.
/// </summary>
/// <typeparam name="TState">The type of the game state.</typeparam>
/// <typeparam name="TMove">The type of the game move.</typeparam>
public interface IClient<TState, TMove>
    where TState : State
{
    /// <summary>
    /// Creates a new game table and connects to it.
    /// </summary>
    /// <param name="tableName">The name of the table to create.</param>
    /// <param name="playerName">The name of the player creating the table.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A connection to the created table.</returns>///
    Task<IConnection<TState, TMove>> CreateTable(string tableName, string playerName, CancellationToken cancellation);

    /// <summary>
    /// Joins an existing game table.
    /// </summary>
    /// <param name="tableName">The name of the table to join.</param>
    /// <param name="playerName">The name of the player joining the table.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A connection to the joined table.</returns>///
    Task<IConnection<TState, TMove>> JoinTable(string tableName, string playerName, CancellationToken cancellation);

    /// <summary>
    /// Checks if a player can join a table.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <param name="playerName">The name of the player trying to join the table.</param>
    /// <param name="cancellation">A cancellation token.</param>
    /// <returns>A response indicating if the player can join the table.</returns>///
    Task<CanJoinTableResponse> CanJoinTable(string tableName, string playerName, CancellationToken cancellation);
}
