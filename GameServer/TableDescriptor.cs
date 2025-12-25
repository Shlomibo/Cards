using DTOs;

namespace GameServer;

/// <summary>
/// A gaming table.
/// </summary>
public sealed class Table
{
    private readonly Lazy<IReadOnlyDictionary<int, Player>> _playersById;
    private readonly Lazy<IReadOnlyDictionary<string, Player>> _playersByName;
    private readonly Lazy<IReadOnlyList<Player>> _players;

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets he master of the table.
    /// </summary>
    public Player TableMaster { get; }

    /// <summary>
    /// Gets the players around the table.
    /// </summary>
    public IReadOnlyList<Player> Players => _players.Value;

    /// <summary>
    /// Gets a player by id.
    /// </summary>
    /// <param name="playerId">The player's id.</param>
    public Player this[int playerId] => _playersById.Value[playerId];

    /// <summary>
    /// Gets a player by id or name.
    /// </summary>
    /// <param name="playerName">The player's name.</param>
    public Player this[string playerName] => _playersByName.Value[playerName];

    /// <summary>
    /// Initializes a new instance of the <see cref="Table"/> class.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="tableMaster">The master of the table.</param>
    /// <param name="players">The other players around the table.</param>
    public Table(string name, Player tableMaster, IEnumerable<Player> players)
    {
        Name = name;
        TableMaster = tableMaster;

        _players = new Lazy<IReadOnlyList<Player>>(() => [tableMaster, .. players]);
        _playersById = new Lazy<IReadOnlyDictionary<int, Player>>(
            () => Players.ToDictionary(player => player.Id));
        _playersByName = new Lazy<IReadOnlyDictionary<string, Player>>(
            () => Players.ToDictionary(player => player.Name));
    }

    /// <summary>
    /// A player around the table.
    /// </summary>
    /// <param name="Id">The id of the player.</param>
    /// <param name="Name">The display name of the player.</param>
    /// <param name="State">The state of the player.</param>
    public readonly record struct Player(int Id, string Name, PlayerState State);
}
