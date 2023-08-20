namespace GameServer
{
	public sealed class Table
	{
		private readonly Lazy<IReadOnlyDictionary<int, Player>> playersById;
		private readonly Lazy<IReadOnlyDictionary<string, Player>> playersByName;
		private readonly Lazy<IReadOnlyList<Player>> players;

		public Player TableMaster { get; }
		public IReadOnlyList<Player> Players => this.players.Value;
		public Player this[int playerId] => this.playersById.Value[playerId];
		public Player this[string playerName] => this.playersByName.Value[playerName];

		public Table(Player tableMaster, IEnumerable<Player> players)
		{
			this.TableMaster = tableMaster;

			this.players = new Lazy<IReadOnlyList<Player>>(
				() => players.Prepend(tableMaster)
				.ToArray());
			this.playersById = new Lazy<IReadOnlyDictionary<int, Player>>(
				() => this.Players.ToDictionary(player => player.Id));
			this.playersByName = new Lazy<IReadOnlyDictionary<string, Player>>(
				() => this.Players.ToDictionary(player => player.Name));
		}

		public record Player(int Id, string Name) { }
	}
}
