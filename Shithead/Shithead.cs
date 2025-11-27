using Shithead.State;

namespace Shithead;

/// <summary>
/// Utilities for creating and managing Shithead games.
/// </summary>
public static class ShitheadGame
{
    /// <summary>
    /// Creates a new Shithead game engine with the specified number of players.
    /// </summary>
    /// <param name="playersCount">The number of players.</param>
    /// <returns>A newly initialized game engine.</returns>
    public static ShitheadEngine CreateGame(int playersCount)
    {
        return new(new ShitheadState(playersCount));
    }
}
