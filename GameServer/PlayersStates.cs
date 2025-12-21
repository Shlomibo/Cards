using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GameServer;

internal class PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove> :
		IReadOnlyDictionary<Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player, TPlayerState>
{
	private readonly Dictionary<int, TPlayerState> playersStates;
	private readonly Dictionary<
			int,
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> playersById;
	private readonly Dictionary<
			Guid,
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> playersByConnectionId;

	public PlayersStates(
			IEnumerable<(
					Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player key,
					TPlayerState value)> playersStates)
	{
		this.playersStates = new Dictionary<int, TPlayerState>(
						playersStates.Select(kv => new KeyValuePair<int, TPlayerState>(kv.key.Id, kv.value)));
		playersById = new Dictionary<int, Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
				playersStates.Select(kv => new KeyValuePair<
						int,
						Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
								kv.key.Id, kv.key)));
		playersByConnectionId = new Dictionary<Guid, Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
				playersStates.Select(kv => new KeyValuePair<
						Guid,
						Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
								kv.key.ConnectionId, kv.key)));
	}

	public TPlayerState this[Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player player] =>
			this[player.Id];

	public TPlayerState this[int playerId] => playersStates[playerId];

	public TPlayerState this[Guid connectionId] => this[playersByConnectionId[connectionId]];

	public IEnumerable<Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> Keys =>
			playersById.Values;

	public IEnumerable<TPlayerState> Values => playersStates.Values;

	public int Count => playersStates.Values.Count;

	public bool ContainsKey(Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player player) =>
			playersById.TryGetValue(player.Id, out var playerById) && player == playerById;

	public bool ContainsKey(int playerId) =>
			playersStates.ContainsKey(playerId);

	public bool ContainsKey(Guid connectionId) =>
			playersByConnectionId.ContainsKey(connectionId);

	public IEnumerator<KeyValuePair<
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player,
			TPlayerState>> GetEnumerator()
	{
		foreach (var kv in playersStates)
		{
			var player = playersById[kv.Key];
			yield return new KeyValuePair<
					Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player,
					TPlayerState>(player, kv.Value);
		}
	}

	public bool TryGetValue(
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player key,
			[MaybeNullWhen(false)] out TPlayerState value)
	{
		value = default;
		bool hasKey = ContainsKey(key);

		if (hasKey)
		{
			value = this[key];
		}

		return hasKey;
	}

	public bool TryGetValue(int playerId, [MaybeNullWhen(false)] out TPlayerState value) =>
			playersStates.TryGetValue(playerId, out value);

	public bool TryGetValue(Guid connectionId, [MaybeNullWhen(false)] out TPlayerState value)
	{
		value = default;
		bool hasKey = ContainsKey(connectionId);

		if (hasKey)
		{
			value = this[connectionId];
		}

		return hasKey;
	}

	IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();
}
