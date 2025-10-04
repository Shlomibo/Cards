namespace GameServer;

public sealed class Table
{
	private readonly Lazy<IReadOnlyDictionary<int, Player>> playersById;
	private readonly Lazy<IReadOnlyDictionary<string, Player>> playersByName;
	private readonly Lazy<IReadOnlyList<Player>> players;

	public Player TableMaster { get; }
	public IReadOnlyList<Player> Players => players.Value;
	public Player this[int playerId] => playersById.Value[playerId];
	public Player this[string playerName] => playersByName.Value[playerName];

	public Table(Player tableMaster, IEnumerable<Player> players)
	{
		TableMaster = tableMaster;

		this.players = new Lazy<IReadOnlyList<Player>>(
			() => players.Prepend(tableMaster)
			.ToArray());
		playersById = new Lazy<IReadOnlyDictionary<int, Player>>(
			() => Players.ToDictionary(player => player.Id));
		playersByName = new Lazy<IReadOnlyDictionary<string, Player>>(
			() => Players.ToDictionary(player => player.Name));
	}

	public record Player(int Id, string Name) { }
}
