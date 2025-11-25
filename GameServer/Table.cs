using GameEngine;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GameServer;

internal sealed class Table<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove>
{
    private readonly object _lock = new();
    private readonly HashSet<string> _playerNames = [ ];
    private readonly Dictionary<int, string> _playerNamesByIds = [ ];
    private readonly Dictionary<int, Guid> _playerConnectionIdsByIds = [ ];
    private readonly Dictionary<Guid, int> _playerIdsByConnectionId = [ ];
    private Engine<TGameState, TSharedState, TPlayerState, TGameMove>? _game;

    public event EventHandler? TableUpdated;
    public event EventHandler<TableGameUpdateEventArgs<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove>>? GameUpdated;

    public Player TableMaster => this[0];

    public string TableName { get; }
    public Engine<TGameState, TSharedState, TPlayerState, TGameMove>? Game => _game;

    [MemberNotNullWhen(true, nameof(Game))]
    public bool GameStarted => Game != null;

    public Player this[int playerId] => new(
        playerId,
        name: _playerNamesByIds[playerId],
        connectionId: _playerConnectionIdsByIds[playerId]);

    public Player this[Guid connectionId]
    {
        get
        {
            int playerId = _playerIdsByConnectionId[connectionId];

            return new Player(
                playerId,
                name: _playerNamesByIds[playerId],
                connectionId);
        }
    }

    public Table(string tableName, string tableMasterName)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);
        ArgumentNullException.ThrowIfNull(tableMasterName);

        TableName = tableName;
        _playerNames.Add(tableMasterName);
        AddPlayerWithId(0, tableMasterName);
    }

    public Player AddPlayer(string name)
    {
        if (!_playerNames.Add(name))
        {
            throw new ArgumentException("A player with the same name already exists", nameof(name));
        }

        lock (_lock)
        {
            int id = 1 + _playerConnectionIdsByIds.Keys.Max();

            return AddPlayerWithId(id, name);
        }
    }

    public void RemovePlayer(int id)
    {
        if (_playerConnectionIdsByIds.TryGetValue(id, out var connId))
        {
            _playerIdsByConnectionId.Remove(connId);
        }

        _playerConnectionIdsByIds.Remove(id);
        _playerNamesByIds.Remove(id);

        OnTableUpdate();
    }

    private void OnTableUpdate()
    {
        TableUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void RemovePlayer(Guid connectionId)
    {
        if (_playerIdsByConnectionId.TryGetValue(connectionId, out int id))
        {
            RemovePlayer(id);
        }
    }

    public void RemovePlayer(Player player) =>
        RemovePlayer(player.Id);

    public void PlayMove(TGameMove move, int? playerId = null) =>
        _game?.PlayMove(move, playerId);
    public void PlayMove(TGameMove move, Guid connectionId)
    {
        if (_playerIdsByConnectionId.TryGetValue(connectionId, out int id))
        {
            _game?.PlayMove(move, id);
        }
    }

    public void PlayMove(TGameMove move, Player player) => PlayMove(move, player.Id);

    public bool TrySetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game)
    {
        if (game != _game && game == null)
        {
            return false;
        }

        try
        {
            SetGame(game);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game)
    {
        if (game != _game)
        {
            if (_game != null)
            {
                _game.Updated -= OnGameUpdated;
            }

            _game = game ?? throw new ArgumentNullException(nameof(game));
            _game.Updated += OnGameUpdated;
        }

        OnGameUpdated(this, EventArgs.Empty);

        void OnGameUpdated(object? _, EventArgs args) => GameUpdated?.Invoke(
                this,
                new TableGameUpdateEventArgs<TGameState, TSharedState, TPlayerState, TGameMove>(
                    game.State,
                    game.Players.Select(player => (this[player.PlayerId], player.State))));
    }

    private Player AddPlayerWithId(int id, string name)
    {
        var connectionId = Guid.NewGuid();

        _playerConnectionIdsByIds.Add(id, connectionId);
        _playerIdsByConnectionId.Add(connectionId, id);
        _playerNamesByIds.Add(id, name);

        OnTableUpdate();

        return this[id];
    }

    public Table AsTableDescriptor() =>
        new(
            TableMaster.AsDescriptor(),
            players: from kv in _playerNamesByIds
                     let id = kv.Key
                     where id != TableMaster.Id
                     let name = kv.Value
                     select new Table.Player(id, name));

    public readonly record struct Player : IEquatable<Player>
    {
        public static Player NoPlayer { get; } = default;
        public readonly int Id { get; }
        public readonly string Name { get; }
        public readonly Guid ConnectionId { get; }

        public Player(int id, string name, Guid connectionId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Id = id;
            Name = name;
            ConnectionId = connectionId;
        }

        public Table.Player AsDescriptor() =>
            new(Id, Name);

        public override string ToString() => $"({Id}): {Name}";
    }
}

internal class TableGameUpdateEventArgs<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove> : EventArgs
{
    public TSharedState SharedState { get; }
    public PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove> PlayersStates
    { get; }

    public TableGameUpdateEventArgs(
        TSharedState sharedState,
        IEnumerable<(Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player, TPlayerState)> playersStates)
    {
        SharedState = sharedState;
        PlayersStates = new PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove>(playersStates);
    }
}

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
