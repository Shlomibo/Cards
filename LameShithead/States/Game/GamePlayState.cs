using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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
    private const string SEPARATOR = """

        ================================================================================
        ================================================================================


        """;

    private static readonly ConsoleOutput Separator = ConsoleOutput.FromString(SEPARATOR)
        .Stylize(Style.Dim);

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

    private Task WaitForGameToEnd(StateUpdate<ShitheadGameState> state, CancellationToken cancellation) =>
        PrintGame(state, cancellation);

    private async Task PlayOutOfTurn(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await PrintGame(state, cancellation);
        var playerState = state.State!.PlayerState;

        List<Option<Func<Task<ShitheadMove>>>> options = [
            new(1, "Take revealed card(s)", TakeRevealedCards(playerState, cancellation), HasRevealedCards(playerState)),
            new(2, "Reveal an undercard", RevealUndercard(playerState, cancellation), CanRevealUndercard(playerState)),
            new(3, "Place a Joker to pass the pile", PlaceJoker(state, cancellation), CanPlaceJoker(playerState)),
        ];

        var move = await GetOptionFromUser(
            "Select your move:",
            options,
            cancellation);

        await Context.Console.WriteLine(SEPARATOR, cancellation);

        await Connection.PlayMove(
            await move(),
            CancellationToken.None);
    }

    private static bool CanPlaceJoker(DTOs.Shithead.PlayerState playerState) => playerState switch
    {
        { Hand: { Length: > 0 } hand } => hand.Any(c => c.Value == Value.Joker),
        { Hand.Length: 0, RevealedCards: { Count: > 0 } revealedCards } => revealedCards.Values.Any(card => card.Value == Value.Joker),
        { Hand.Length: 0, RevealedCards.Count: 0, Undercards: { Count: > 0 } u } => u.Values.Any(c => c?.Value == Value.Joker),
        _ => false,
    };

    private static bool CanRevealUndercard(DTOs.Shithead.PlayerState playerState) =>
        playerState is
        {
            Hand.Length: 0,
            RevealedCards.Count: 0,
            Undercards: { Count: > 0 } undercards
        }
        && !undercards.Values.Any(c => c != null);

    private static bool HasRevealedCards(DTOs.Shithead.PlayerState playerState) =>
        playerState is { Hand.Length: 0 }
            && (playerState.RevealedCards.Count > 0
            || playerState.Undercards.Values.Any(c => c != null));

    private async Task PlayInTurn(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await PrintGame(state, cancellation);
        var playerState = state.State!.PlayerState;

        bool playerHasCardsInHand = playerState.Hand.Length > 0;

        List<Option<Func<Task<ShitheadMove>>>> options = [
            (1, "Take pile", async () => AcceptDiscardPile.Instance),
            new(2, "Select card(s) to place", PlaceCards, playerHasCardsInHand),
            new(3, "Take revealed card(s)", TakeRevealedCards(playerState, cancellation), HasRevealedCards(playerState)),
            new(4, "Reveal an undercard", RevealUndercard(playerState, cancellation), CanRevealUndercard(playerState)),
            new(5, "Place a Joker to pass the pile", PlaceJoker(state, cancellation), CanPlaceJoker(playerState)),
        ];

        var move = await GetOptionFromUser(
            "Select your move:",
            options,
            cancellation);

        await Context.Console.WriteLine(SEPARATOR, cancellation);

        await Connection.PlayMove(
            await move(),
            CancellationToken.None);

        async Task<ShitheadMove> PlaceCards()
        {
            var selectedCards = await GetValueFromUser<int[]>(
                "Select card(s) to place (separate by comma):",
                IsValidCardList,
                cancellation);

            return new PlaceCard { CardIndices = selectedCards };

            bool IsValidCardList(
                string str,
                [NotNullWhen(true)] out int[]? indices,
                [NotNullWhen(false)] out string? error)
            {
                indices = default;
                error = default;

                var parts = str.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var selectedIndices = new List<int>(parts.Length);

                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int parsed))
                    {
                        error = "Please enter a valid number.";
                        return false;
                    }

                    int zeroBasedParsed = parsed - 1;

                    if (zeroBasedParsed < 0 || zeroBasedParsed >= playerState.Hand.Length)
                    {
                        error = $"Please select a valid card index ({parsed} is not valid).";
                        return false;
                    }

                    selectedIndices.Add(zeroBasedParsed);
                }

                if (selectedIndices.Count == 0)
                {
                    error = "Please select at least one card.";
                    return false;
                }

                indices = [.. selectedIndices];
                return true;
            }
        }
    }

    private Func<Task<ShitheadMove>> PlaceJoker(
        StateUpdate<ShitheadGameState> state,
        CancellationToken cancellation)
        =>
        async () =>
        {
            var selectedPlayerId = await GetOptionFromUser(
                "Select a player to pass the pile to:",
                [.. state.Table.Values
                        .Where(p => p.State == DTOs.PlayerState.Playing)
                        .Select(p => (p.PlayerId, p.PlayerName, p.PlayerId))],
                cancellation);

            return new PlaceJoker { PlayerId = selectedPlayerId };
        };

    private Func<Task<ShitheadMove>> RevealUndercard(
        DTOs.Shithead.PlayerState playerState,
        CancellationToken cancellation)
        =>
        async () =>
        {
            var undercardToReveal = await GetValueFromUser<int>(
                "Select an undercard to reveal:",
                IsValidUndercardIndex,
                cancellation);

            return new RevealUndercard { CardIndex = undercardToReveal };

            bool IsValidUndercardIndex(string str, [NotNullWhen(true)] out int index, [NotNullWhen(false)] out string? error)
            {
                index = default;
                error = null;

                if (!int.TryParse(str, out int parsed))
                {
                    error = "Please enter a valid number.";
                    return false;
                }

                if (!playerState.Undercards.ContainsKey(parsed - 1)
                    || playerState.Undercards[parsed - 1] == null)
                {
                    error = "Please select a valid undercard index.";
                    return false;
                }

                index = parsed - 1;
                return true;
            }
        };

    private Func<Task<ShitheadMove>> TakeRevealedCards(
        DTOs.Shithead.PlayerState playerState,
        CancellationToken cancellation)
        =>
        async () =>
        {
            if (playerState.RevealedCards.Count == 0)
            {
                return new TakeUndercard
                {
                    CardIndex = playerState.Undercards
                        .Where(kv => kv.Value != null)
                        .Select(kv => kv.Key)
                        .Single()
                };
            }

            var selectedCards = await GetValueFromUser<int[]>(
                "Select revealed card(s) to take (separate by comma):",
                IsValidRevealedCardsList,
                cancellation);

            return new TakeRevealedCards { CardIndices = selectedCards };

            bool IsValidRevealedCardsList(
                string str,
                [NotNullWhen(true)] out int[]? indices,
                [NotNullWhen(false)] out string? error)
            {
                indices = default;
                error = default;

                var parts = str.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var selectedIndices = new List<int>(parts.Length);

                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int parsed))
                    {
                        error = "Please enter a valid number.";
                        return false;
                    }

                    int zeroBasedParsed = parsed - 1;

                    if (!playerState.RevealedCards.TryGetValue(zeroBasedParsed, out var card) ||
                        card == null)
                    {
                        error = $"Please select a valid revealed card index ({parsed} is not valid).";
                        return false;
                    }

                    selectedIndices.Add(zeroBasedParsed);
                }

                if (selectedIndices.Count == 0)
                {
                    error = "Please select at least one card.";
                    return false;
                }

                indices = [.. selectedIndices];
                return true;
            }
        };

    private async Task LetPlayerRevealCards(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await PrintGame(state, cancellation);

        var selectedCard = await GetOptionFromUser(
            "Reveal a card:",
            [.. state.State!.PlayerState.Undercards
                .Where(kv => kv.Value != null)
                .Select(kv => (kv.Key, kv.Key.ToString(), kv.Key)) ],
                cancellation);

        await Connection.PlayMove(
            new RevealUndercard { CardIndex = selectedCard },
            CancellationToken.None);
    }

    private Task WaitForPlayersToSelectTheirRevealedCards(
        StateUpdate<ShitheadGameState> state,
        CancellationToken cancellation)
        =>
        PrintGame(state, cancellation);

    private const int ALIGNMENT = 3;

    private async Task PrintGame(StateUpdate<ShitheadGameState> state, CancellationToken cancellation)
    {
        await Context.Console.WriteLine(
            ConsoleOutput.Join(Separator,
                PlayersHands(state),
                TableTop(state),
                CurrentPlayer(state)),
            cancellation);

        static ConsoleOutput PlayersHands(StateUpdate<ShitheadGameState> state) =>
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

        static ConsoleOutput TableTop(StateUpdate<ShitheadGameState> state)
        {
            var sharedState = state.State!.SharedState;

            return ConsoleOutput.Interpolate(
                $"""
                Deck size   : {sharedState.DeckSize}
                Pile        : {sharedState.DiscardPile.Length} {TopCard([.. sharedState.DiscardPile])}
                """);
        }

        static ConsoleOutput CurrentPlayer(StateUpdate<ShitheadGameState> state)
        {
            var activeCardsStyle = Style.Yellow.Forward;
            var inactiveCardsStyle = Style.Dim;

            var playerDetails = state.CurrentPlayer;
            var identification = ConsoleOutput.Interpolate($"{playerDetails.PlayerId,2}: {playerDetails.PlayerName}")
                .Stylize(Style.Dim);

            var playerState = state.State!.PlayerState;
            var hand = state.State.PlayerState.Hand.Length == 0
                ? PrintCard(null).Stylize(inactiveCardsStyle)
                : ConsoleOutput
                    .Join(
                        " ",
                        playerState.Hand.Select((c, i) => ConsoleOutput.Interpolate($"{PrintCard(c)}({i + 1})")))
                    .Stylize(activeCardsStyle);

            var revealedCardsStyle = playerState is { Hand.Length: 0, RevealedCards.Count: > 0 }
                ? activeCardsStyle
                : inactiveCardsStyle;

            var revealedCards = ConsoleOutput
                .Join(
                    " ",
                    Enumerable.Range(0, 3)
                        .Select(i => playerState.RevealedCards.TryGetValue(i, out var c)
                            ? ConsoleOutput.Interpolate($"{PrintCard(c)}({i + 1})")
                            : "      "))
                .Stylize(revealedCardsStyle);

            var undercardsStyle = playerState is { Hand.Length: 0, RevealedCards.Count: 0 }
                ? activeCardsStyle
                : inactiveCardsStyle;

            var undercards = ConsoleOutput
                .Join(
                    " ",
                    Enumerable.Range(0, 3)
                        .Select(i => playerState.Undercards.TryGetValue(i, out var c)
                            ? ConsoleOutput.Interpolate($"{PrintCard(c)}({i + 1})")
                            : "      "))
                .Stylize(undercardsStyle);

            return ConsoleOutput.Join("\n\n", identification, hand, revealedCards, undercards);
        }
    }

    private static ConsoleOutput TopCard(ReadOnlySpan<Card> discardPile) =>
        discardPile switch
        {
            [] => "[]",
            [{ Value: Value.Three } top, .. var rest] => ConsoleOutput.Interpolate($"{PrintCard(top)} ({TopCard(Valued(rest))})"),
            [var top, ..] => PrintCard(top),
        };

    private static ReadOnlySpan<Card> Valued(ReadOnlySpan<Card> pile)
    {
        int? nonThreeIndex = null;

        for (int i = 0; i < pile.Length; i++)
        {
            if (pile[0].Value != Value.Three)
            {
                nonThreeIndex = i;
                break;
            }
        }

        return nonThreeIndex switch
        {
            null => [],
            int i => pile[i..],
        };
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

    private static ConsoleOutput PrintCard(Card? card, ConsoleOutput? nullCard = null)
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
