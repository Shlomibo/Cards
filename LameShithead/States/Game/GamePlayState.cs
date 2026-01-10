using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using ConsoleUtils.Output;
using DTOs;
using DTOs.Cards.FrenchSuited;
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
                ConsoleOutput.FromString(ex.ToString())
                    .Stylize(Style.Red.Forward),
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

    private async Task WaitForGameReset(StateUpdate<ShitheadGameState> state, CancellationToken cancellation) =>
        throw new NotImplementedException();

    private async Task PrintPlayersList(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        var tableName = ConsoleOutput.FromString(state.TableName).Stylize(Style.Bold);
        var output = ConsoleOutput.Interpolate($@"Table: {tableName}
======={new string('=', state.TableName.Length)}
");

        output = output.Concat(state.Table.Values.Select(GetPlayerIdentification));

        await Context.Console.WriteLine(output, cancellation);
    }

    private static ConsoleOutput GetPlayerIdentification(Player player)
    {
        var playerName = ConsoleOutput.FromString(player.PlayerName)
            .Stylize(player.State == DTOs.PlayerState.Playing
                ? Style.Bold
                : Style.Dim);

        return ConsoleOutput.Interpolate($"{player.PlayerId,2} {playerName}\n");
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

    private Task WaitForPlayersToSelectTheirRevealedCards(
        StateUpdate<ShitheadGameState> state,
        CancellationToken cancellation)
        =>
        PrintGame(state, cancellation);

    private const int ALIGNMENT = 3;

    private async Task PrintGame(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {


        static ConsoleOutput PlayerHands(StateUpdate<ShitheadGameState> state) =>
            ConsoleOutput.Join(
                "\n\n",
                state.Table.Values
                    .Where(p => p.PlayerId != state.CurrentPlayer.PlayerId)
                    .Select(player => PlayerHand(
                        player,
                        state.State!.SharedState.Players[player.PlayerId])));

        static ConsoleOutput PlayerHand(Player player, OtherPlayersState state)
        {
            var id = GetPlayerIdentification(player);
            var hand = ConsoleOutput.Interpolate($"Cards in hand: {state.CardsCount}");
            var revealedCards = ConsoleOutput.Join("  ", Enumerable.Range(0, 3)
                .Select(i => state.RevealedCards.GetValueOrDefault(i))
                .Select(c => PrintCard(c))
                .Select(c => ConsoleOutput.FromValue(c, alignment: ALIGNMENT)));
            var undercards = ConsoleOutput.Join("  ", Enumerable.Range(0, 3)
                .Select(i => state.RevealedCards.TryGetValue(i, out var card)
                    ? PrintCard(card, "[x]")
                    : PrintCard(null))
                .Select(c => ConsoleOutput.FromValue(c, alignment: ALIGNMENT)));

            if (!state.RevealedCardsAccepted)
            {
                revealedCards = revealedCards.Stylize(Style.Dim);
            }

            return ConsoleOutput.Join("\n",
                id,
                hand,
                revealedCards,
                undercards);
        }
    }

    private async Task WaitForGameToStart(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await PrintPlayersList(state, cancellation);
    }

    private async Task LetMasterStartGame(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await PrintPlayersList(state, cancellation);

        string input;

        do
        {
            input = await Context.Console.ReadLine(cancellation) ?? "";
        }
        while (!input.Equals("ok", StringComparison.OrdinalIgnoreCase)
            && cancellation.IsCancellationRequested);

        if (!cancellation.IsCancellationRequested)
        {
            await Connection.StartGame(cancellation);
        }
    }

    private static readonly ConcurrentDictionary<Card, string> _cardsMemoise = [];

    private static string PrintCard(Card? card, string? nullCard = null)
    {
        return card == null
            ? nullCard ?? "[ ]"
            : _cardsMemoise.GetOrAdd(card, CalculateCardValue);

        static string CalculateCardValue(Card card) => card switch
        {
            { Value: Value.Joker, Color: Color.Red } => "RJ",
            { Value: Value.Joker } => "BJ",
            { Value: var value, Suit: var suit } => PrintValue(value) + PrintSuit(suit!.Value)
        };

        static string PrintValue(Value value) => value switch
        {
            Value.Ace => "A",
            > Value.Ace and <= Value.Ten => ((int)value).ToString(),
            Value.Jack => "J",
            Value.Queen => "Q",
            Value.King => "K",
            _ => throw new ArgumentException("Invalid value"),
        };

        static string PrintSuit(Suit suit) => suit switch
        {
            Suit.Clubs => "♣️",
            Suit.Diamonds => "♦️",
            Suit.Hearts => "♥️",
            Suit.Spades => "♠️",
            _ => throw new ArgumentException("Invalid suit"),
        };
    }
}
