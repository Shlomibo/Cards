namespace GameServer;

public sealed class Table
{
    private readonly Lazy<IReadOnlyDictionary<int, Player>> _playersById;
    private readonly Lazy<IReadOnlyDictionary<string, Player>> _playersByName;
    private readonly Lazy<IReadOnlyList<Player>> _players;

    public Player TableMaster { get; }
    public IReadOnlyList<Player> Players => _players.Value;
    public Player this[int playerId] => _playersById.Value[playerId];
    public Player this[string playerName] => _playersByName.Value[playerName];

    public Table(Player tableMaster, IEnumerable<Player> players)
    {
        TableMaster = tableMaster;

        _players = new Lazy<IReadOnlyList<Player>>(() => [.. players.Prepend(tableMaster)]);
        _playersById = new Lazy<IReadOnlyDictionary<int, Player>>(
            () => Players.ToDictionary(player => player.Id));
        _playersByName = new Lazy<IReadOnlyDictionary<string, Player>>(
            () => Players.ToDictionary(player => player.Name));
    }

    public readonly record struct Player(int Id, string Name);
}
