using System;
using DTOs;
using DTOs.Responses;

namespace GamesClient.Client;

public interface IClient<TState, TMove>
    where TState : State
{
    Task<IConnection<TState, TMove>> CreateTable(string tableName, string playerName, CancellationToken cancellation);
    Task<IConnection<TState, TMove>> JoinTable(string tableName, string playerName, CancellationToken cancellation);

    Task<CanJoinTableResponse> CanJoinTable(string tableName, string playerName, CancellationToken cancellation);
}
