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
        await (state switch
        {
            { State: null, Table.Count: >= 2 } when IsMaster => LetMasterStartGame(state, cancellation),
            { State: null } => WaitForGameToStart(state, cancellation),
            { State.SharedState.GameState: GameState.Init, State.PlayerState.RevealedCardsAccepted: true } => WaitForPlayersToSelectTheirRevealedCards(state, cancellation),
            { State.SharedState.GameState: GameState.Init } => LetPlayerRevealCards(state, cancellation),
            { State: { SharedState.GameState: GameState.GameOn, PlayerState.Won: true } } => WaitForGameToEnd(
                state,
                cancellation),
            {
                CurrentPlayer.PlayerId: int currentPlayer,
                State.SharedState: { GameState: GameState.GameOn, CurrentTurnPlayer: int currentTurn },
            }
                when currentPlayer == currentTurn
                =>
                PlayInTurn(state, cancellation),
            { State.SharedState.GameState: GameState.GameOn } => PlayOutOfTurn(state, cancellation),
            { State.SharedState.GameState: not GameState.GameOver } => throw new InvalidOperationException("This 💩 is lame 😢"),
            _ when IsMaster => LetMasterResetGame(state, cancellation),
            _ => WaitForGameReset(state, cancellation),
        });
    }

    private async Task WaitForGameReset(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task LetMasterResetGame(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task WaitForGameToEnd(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task PlayOutOfTurn(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task PlayInTurn(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task LetPlayerRevealCards(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task WaitForPlayersToSelectTheirRevealedCards(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task WaitForGameToStart(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    private async Task LetMasterStartGame(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
