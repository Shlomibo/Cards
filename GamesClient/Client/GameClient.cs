using System;
using DTOs;
using DTOs.Responses;

namespace GamesClient.Client;

public abstract class GameClient<TState, TMove> : IClient<TState, TMove>
    where TState : State
{
    public Task<CanJoinTableResponse> CanJoinTable(string tableName, string playerName, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<IConnection<TState, TMove>> CreateTable(string tableName, string playerName, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<IConnection<TState, TMove>> JoinTable(string tableName, string playerName, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
