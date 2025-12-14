namespace TurnsManagement;

/// <summary>
/// Manages turns for a given number of players.
/// </summary>
public interface ITurnsManager
{
    /// <summary>
    /// Gets the total number of players.
    /// </summary>
	int InitialPlayersCount { get; }

    /// <summary>
    /// Gets the list of active players' IDs.
    /// </summary>
    /// <value></value>
	IReadOnlyList<int> ActivePlayers { get; }

    /// <summary>
    /// Gets the id of the player whose turn was before the current one.
    /// </summary>
    int Previous { get; }
    /// <summary>
    /// Gets or sets id of the player whose turn it is currently.
    /// </summary>
	int Current { get; set; }

    /// <summary>
    /// Gets the id of the player whose turn is next.
    /// </summary>
    int Next { get; }

    /// <summary>
    /// Gets or sets the direction in which turns are managed.
    /// </summary>
	TurnsDirection Direction { get; set; }

    /// <summary>
    /// Advances to the next player's turn.
    /// </summary>
    /// <returns>The id of the next player.</returns>
	int MoveNext();

    /// <summary>
    /// Jumps a number of turns in the specified direction.
    /// </summary>
    /// <param name="skippedTurns">The count of turns to skip.</param>
    /// <param name="direction">The direction in which the turns are skipped.</param>
    /// <returns>The id of the next player.</returns>
	int Jump(int skippedTurns, TurnsDirection? direction = null);

    /// <summary>
    /// Switches the direction of turns.
    /// </summary>
    /// <returns>The current direction.</returns>
	TurnsDirection SwitchDirection();

    /// <summary>
    /// Removes a player from the turns management.
    /// </summary>
    /// <param name="playerId">The id of the player to remove.</param>
	void RemovePlayer(int playerId);
}

/// <summary>
/// The direction in which turns are managed.
/// </summary>
/// <remarks>
/// The terms "Up" and "Down" are used to avoid confusion with clockwise and counter-clockwise,
/// but they are arbitrary and can be interpreted as needed.
/// </remarks>
public enum TurnsDirection
{
    /// <summary>
    /// An up direction.
    /// </summary>
    Up,

    /// <summary>
    /// A down direction.
    /// </summary>
    Down,
}

/// <summary>
/// Extension methods for ITurnsManager.
/// </summary>
public static class TurnsManagerExtensions
{
    extension(ITurnsManager turnsManager)
    {
        /// <summary>
        /// Gets the total number of active players.
        /// </summary>
        public int PlayersCount => turnsManager.ActivePlayers.Count;
    }
}
