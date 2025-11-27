namespace Shithead.State;

/// <summary>
/// The current state of a Shithead game.
/// </summary>
public enum GameState
{
    /// <summary>
    /// The players place their revealed cards and wait until the game can start.
    /// </summary>
    Init,

    /// <summary>
    /// The game is ongoing.
    /// </summary>
    GameOn,

    /// <summary>
    /// The game is over.
    /// </summary>
    GameOver,
}
