using System;
using DTOs;

namespace GamesClient.Client;

public interface IConnection<TState, TMove> : IDisposable
    where TState : State
{
    string TableName { get; }
    string PlayerName { get; }
    IObservable<StateUpdate<TState>> GameState { get; }


    Task PlayMove(TMove move, CancellationToken cancellation);
    Task StartGame(CancellationToken cancellation);
}
