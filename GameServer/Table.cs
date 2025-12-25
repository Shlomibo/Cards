using DTOs;

using GameEngine;

using System.Diagnostics.CodeAnalysis;

namespace GameServer;

internal sealed partial class Table<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove> : ITable<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove>
{
    private readonly Lock _lock = new();
    private readonly HashSet<string> _playerNames = [];
    private readonly Dictionary<int, string> _playerNamesByIds = [];
    private readonly Dictionary<int, Guid> _playerConnectionIdsByIds = [];
    private readonly Dictionary<Guid, int> _playerIdsByConnectionId = [];

    public event EventHandler? TableUpdated;
    public event EventHandler<TableGameUpdateEventArgs<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove>>? GameUpdated;

    public Player TableMaster => this[0];

    public string TableName { get; }
    public IEngine<TSharedState, TPlayerState, TGameMove>? Game { get; private set; }

    [MemberNotNullWhen(true, nameof(Game))]
    public bool GameStarted => Game != null;

    IReadOnlyList<Player> ITable<TGameState, TSharedState, TPlayerState, TGameMove>.GetPlayers() =>
        [.. _playerIdsByConnectionId
            .OrderBy(kv => kv.Key)
            .Select(kv => new Player(
                kv.Value,
                _playerNamesByIds[kv.Value],
                _playerConnectionIdsByIds[kv.Value]
            ))];

    private Player this[int playerId] => new(
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
        ArgumentException.ThrowIfNullOrEmpty(name);
        Player result;

        lock (_lock)
        {
            if (GameStarted)
            {
                throw new InvalidOperationException("The game has already started.");
            }
            if (!_playerNames.Add(name))
            {
                throw new InvalidOperationException("A player with the same name already exists");
            }

            int id = 1 + _playerConnectionIdsByIds.Keys.Max();

            result = AddPlayerWithId(id, name);
        }

        OnTableUpdate();
        return result;
    }

    public bool CanAddPlayer(string name) =>
        !string.IsNullOrEmpty(name)
        && !GameStarted
        && !_playerNames.Contains(name);

    private void RemovePlayer(int id)
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

    public void PlayMove(TGameMove move, int? playerId = null) =>
        Game?.PlayMove(move, playerId);

    public void SetGame(IEngine<TSharedState, TPlayerState, TGameMove>? game)
    {
        if (game != Game)
        {
            Game?.Updated -= OnGameUpdated;

            Game = game;
            Game?.Updated += OnGameUpdated;

            if (Game != null)
            {
                OnGameUpdated(this, EventArgs.Empty);
            }
        }

        void OnGameUpdated(object? _, EventArgs args) => GameUpdated?.Invoke(
                this,
                new TableGameUpdateEventArgs<TGameState, TSharedState, TPlayerState, TGameMove>(
                    Game!.State,
                    Game!.Players.Select(player => (this[player.PlayerId], player.State))));
    }

    private Player AddPlayerWithId(int id, string name)
    {
        var connectionId = Guid.NewGuid();

        _playerConnectionIdsByIds.Add(id, connectionId);
        _playerIdsByConnectionId.Add(connectionId, id);
        _playerNamesByIds.Add(id, name);

        return this[id];
    }

    public Table AsTableDescriptor() =>
        new(
            TableName,
            TableMaster.AsDescriptor(),
            players: from kv in _playerNamesByIds
                     let id = kv.Key
                     where id != TableMaster.Id
                     let name = kv.Value
                     select new Table.Player(id, name, PlayerState.Playing));
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
