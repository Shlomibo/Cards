using System;
using System.Reactive.Linq;
using DTOs;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;
using LameShithead.States.GameSelection;

namespace LameShithead.States.Game;

public record GamePlayState(
    Context Context,
    IConnection<ShitheadGameState, ShitheadMove> Connection,
    bool IsMaster) : State(Context)
{
    protected override async Task<State> NextStateUnsafe(CancellationToken cancellation)
    {
        TaskCompletionSource<State> resultTaskSource = new();

        using var subscription = Connection.GameState
            .Select(state => Observable.Create((
                IObserver<(StateUpdate<ShitheadGameState> State,
                CancellationToken Cancellation)> observer)
                =>
                {
                    CancellationTokenSource cancellation = new();
                    observer.OnNext((state, cancellation.Token));

                    return () =>
                    {
                        try
                        {
                            cancellation.Cancel();
                        }
                        finally
                        {
                            cancellation.Dispose();
                        }
                    };
                }))
            .Switch()
            .Subscribe(
                x => UpdateGameState(x.State, x.Cancellation),
                ex => resultTaskSource.TrySetResult(new ErrorState(Context, ex)),
                () => resultTaskSource.TrySetResult(new SelectTableNameState(Context, Connection.PlayerName)));

        using (cancellation.Register(subscription.Dispose))
        {
            var result = await resultTaskSource.Task;
            return result;
        }
    }

    private async void UpdateGameState(
        StateUpdate<ShitheadGameState> state,
        CancellationToken cancellation)
    {
        try
        {
            await Context.Console.Clear(cancellation);
            await UpdateGameStateUnsafe(state, cancellation);
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            await Context.Console.WriteLine(
                new Prompt(ex.ToString(), ConsoleColor.Red),
                cancellation);
        }
    }

    private async Task UpdateGameStateUnsafe(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
