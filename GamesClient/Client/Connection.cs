using System;
using DTOs;

namespace GamesClient.Client;

public sealed class Connection<TState, TMove> : IConnection<TState, TMove>
    where TState : State
{
    // private StateUpdate<TState>? _lastState;

    public string TableName { get; }

    public string PlayerName { get; }

    public IObservable<StateUpdate<TState>> GameState { get; } = null!;

    internal Connection(string tableName, string playerName)
    {
        TableName = tableName;
        PlayerName = playerName;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task PlayMove(TMove move, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task StartGame(CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
