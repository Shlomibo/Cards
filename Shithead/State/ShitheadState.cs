using Deck;
using Deck.Cards.FrenchSuited;

using GameEngine;

using Shithead.Moves;

using TurnsManagement;

namespace Shithead.State;

/// <summary>
/// The state of a Shithead game.
/// </summary>
public sealed partial class ShitheadState : IState<
    ShitheadState,
    ShitheadState.SharedShitheadState,
    ShitheadState.ShitheadPlayerState,
    Move>
{
    private const int MIN_PLAYERS_COUNT = 3;
    private const int MIN_HAND_CARDS = 3;
    private const int DEALT_CARDS = 6;
    private static readonly int SuitSize = Enum.GetNames<Suit>().Length;
    private static readonly CardComparer CardComparer = new();

    #region Fields

    private readonly ITurnsManager _turnsManager;
    private readonly Lock _stateLock = new();
    #endregion

    #region Properties

    internal PlayerState[] PlayerStates { get; }

    /// <summary>
    /// Gets the last move that was played
    /// </summary>
    public (Move move, int? playerId)? LastPlayedMove { get; private set; }

    /// <summary>
    /// Gets the last move that was attempted
    /// </summary>
    public (Move move, int? playerId)? LastMove { get; private set; }

    /// <summary>
    /// Gets the number of players in the game.
    /// </summary>
    public int PlayersCount { get; private set; }

    ShitheadState IState<ShitheadState, SharedShitheadState, ShitheadPlayerState, Move>.GameState => this;

    /// <summary>
    /// Gets the current state of the game.
    /// </summary>
    public GameState GameState { get; internal set; } = GameState.Init;

    /// <summary>
    /// Gets the state that is visible to all players.
    /// </summary>
    public SharedShitheadState SharedState { get; }

    /// <summary>
    /// Gets the deck of cards used in the game.
    /// </summary>
    public CardsDeck Deck { get; }

    /// <summary>
    /// Gets the discard pile.
    /// </summary>
    public CardsDeck DiscardPile { get; } = [];
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ShitheadState"/> class.
    /// </summary>
    /// <param name="playersCount">The initial count of players in the game.</param>
    /// <param name="playingCards">The cards to play with</param>
    public ShitheadState(int playersCount, CardsDeck? playingCards = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(playersCount, MIN_PLAYERS_COUNT);

        Deck = playingCards is [_, ..]
            ? playingCards
            : CardsDeck.FullDeck();

        int maxPlayersCount = Deck.Count / (DEALT_CARDS + PlayerState.UndercardsCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(playersCount, maxPlayersCount);

        PlayersCount = playersCount;

        Deck.Shuffle();
        SharedState = new SharedShitheadState(this);
        _turnsManager = new TurnsManager(playersCount);
        PlayerStates = [.. Enumerable
            .Range(0, playersCount)
            .Select(id => new PlayerState(
                [.. Enumerable.Range(0, PlayerState.UndercardsCount).Select(_ => Deck.Pop())],
                id
            ))];

        Deal();
    }

    internal ShitheadState(
        CardsDeck deck,
        PlayerState[] players,
        GameState gameState,
        ITurnsManager? turnsManager = null)
    {
        ArgumentNullException.ThrowIfNull(players);
        if (turnsManager != null && players.Length != turnsManager.PlayersCount)
        {
            throw new ArgumentException("Turns manager and players count have different players count");
        }

        _turnsManager = turnsManager ?? new TurnsManager(players.Length);
        PlayersCount = _turnsManager.PlayersCount;
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        PlayerStates = players;
        GameState = gameState;

        SharedState = new SharedShitheadState(this);
    }
    #endregion

    #region Methods

    /// <summary>
    /// Gets the state that is visible to the specified player.
    /// </summary>
    /// <param name="playerId">The id of the player.</param>
    /// <returns>The state which is only visible to the <paramref name="playerId"/> player.</returns>
    public ShitheadPlayerState GetPlayerState(int playerId) =>
        new(playerId, this);

    /// <summary>
    /// Determines whether the game is over.
    /// </summary>
    public bool IsGameOver() =>
        GameState == GameState.GameOver;

    /// <summary>
    /// Determines whether the specified move is valid.
    /// </summary>
    /// <param name="move">The move to check.</param>
    /// <param name="player">The player that acts.</param>
    /// <returns>
    /// <see langword="true"/> if the move is valid; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValidMove(Move move, int? player = null)
    {
        lock (_stateLock)
        {
            return GetMove(move, player) is not null;
        }
    }

    /// <summary>
    /// Plays the specified move.
    /// </summary>
    /// <param name="move">The move to play.</param>
    /// <param name="player">The player that acts.</param>
    /// <returns>
    /// <see langword="true"/> if the move was legal; otherwise, <see langword="false"/>.
    /// </returns>
    public bool PlayMove(Move move, int? player = null)
    {
        lock (_stateLock)
        {
            var moveAction = GetMove(move, player);
            moveAction?.Invoke();
            LastMove = (move, player);

            if (moveAction == null)
            {
                return false;
            }
            else
            {
                LastPlayedMove = LastMove;
                return true;
            }
        }
    }

    private void Deal()
    {
        for (int i = 0; i < DEALT_CARDS; i++)
        {
            foreach (var player in PlayerStates)
            {
                player.Hand.Push(Deck.Pop());
            }
        }
    }

    private int SelectStartingPlayer()
    {
        return (from player in PlayerStates
                let lowestCard = (from card in player.Hand
                                  where !CardComparer.WildCards.Contains(card.Value)
                                  orderby card.Value ascending
                                  select (Value?)card.Value).FirstOrDefault()
                where lowestCard.HasValue
                orderby CardComparer.CardValueRank[lowestCard.Value]
                select (int?)player.Id).FirstOrDefault() ?? 0;
    }

    private Action? GetMove(Move move, int? playerId = null)
    {
        if (playerId is null)
        {
            return null;
        }

        var player = PlayerStates[playerId.Value];

        return (GameState, move) switch
        {
            // GameState.Init
            (GameState.Init, SetRevealedCard { CardIndex: var index, TargetIndex: var target })
                when player.CanSetRevealedCard(index, target) => () =>
                {
                    var card = player.Hand[index];
                    player.Hand.RemoveAt(index);

                    player.RevealedCards.Add(target, card);
                }
            ,
            (GameState.Init, UnsetRevealedCard { CardIndex: var index })
                when player.CanUnsetRevealedCard(index) => () =>
                {
                    var card = player.RevealedCards[index];
                    player.RevealedCards.Remove(index);

                    player.Hand.Add(card);
                }
            ,
            (GameState.Init, AcceptSelectedRevealedCards)
                when player.CanAcceptSelectedRevealedCards() => () =>
                {
                    player.RevealedCardsAccepted = true;

                    if (PlayerStates.All(player => player.RevealedCardsAccepted))
                    {
                        _turnsManager.Current = SelectStartingPlayer();
                        GameState = GameState.GameOn;
                    }
                }
            ,
            (GameState.Init, ReselectRevealedCards)
                when player.CanReselectRevealedCards() => () =>
                {
                    player.RevealedCardsAccepted = false;
                }
            ,
            (GameState.Init, _) => null,

            // GameState.GameOn
            (GameState.GameOn, PlaceJoker { PlayerId: var targetPlayerId })
                when player.CanPlaceJoker() &&
                    _turnsManager.ActivePlayers.Contains(targetPlayerId) =>
                () =>
                {
                    player.RemoveJoker();
                    var targetPlayer = PlayerStates[targetPlayerId];

                    targetPlayer.Hand.Push(DiscardPile);
                    DiscardPile.Clear();

                    ReplenishPlayerHand(player);
                    HandlePlayerWin(player);

                    _turnsManager.Current = targetPlayerId;
                }
            ,
            (GameState.GameOn, PlaceCard { CardIndices: var indices })
                when _turnsManager.Current == playerId &&
                    player.CanPlaceCard(indices) &&
                    CanPlaceCard(player.GetCard(indices.First())) =>
                () =>
                {
                    var value = PlayHand(player, indices);
                    HandlePlayerWin(player);
                    bool pileDiscarded = ShouldDiscardPile(value);

                    if (pileDiscarded)
                    {
                        DiscardPile.Clear();
                    }
                    // If the player won, it was removed and the turn belongs to the next player
                    else if (!player.Won)
                    {
                        _turnsManager.MoveNext();
                    }

                    if (value == Value.Eight && !pileDiscarded)
                    {
                        int otherPlayersCount = _turnsManager.ActivePlayers.Count - 1;
                        // We jump the count of eights, plus 1 as the turn should have passed anyway
                        int turnsToJump = indices.Length % otherPlayersCount;

                        if (turnsToJump == 0)
                        {
                            _turnsManager.Current = player.Id;
                        }
                        else
                        {
                            _turnsManager.Jump(turnsToJump);
                        }
                    }
                }
            ,
            // When Player tries to add cards of the same value they got from deck, after finishing
            // their turn
            (GameState.GameOn, PlaceCard { CardIndices: var indices })
                when _turnsManager.Previous == playerId &&
                    player.CanPlaceCard(indices) &&
                    player.GetCard(indices.First()).Value == TopCard()?.Value =>
                () =>
                {
                    var value = PlayHand(player, indices);
                    HandlePlayerWin(player);

                    if (ShouldDiscardPile(value))
                    {
                        DiscardPile.Clear();
                        _turnsManager.Current = playerId.Value;
                    }
                    else if (value == Value.Eight)
                    {
                        _turnsManager.Jump(indices.Length);
                    }
                }
            ,
            (GameState.GameOn, PlaceCard { CardIndices: var indices })
                when player.CanPlaceCard(indices) &&
                    ShouldDiscardPileIfHadThese(indices.Select(i => player.GetCard(i))) =>
                () =>
                {
                    PlayHand(player, indices);
                    DiscardPile.Clear();
                    _turnsManager.Current = player.Id;
                }
            ,
            (GameState.GameOn, AcceptDiscardPile)
                when _turnsManager.Current == playerId &&
                    player.Hand.Count > 0 =>
                () =>
                {
                    player.Hand.Push(DiscardPile);
                    DiscardPile.Clear();
                    _turnsManager.MoveNext();
                }
            ,
            (GameState.GameOn, RevealUndercard { CardIndex: var cardIndex })
                when player.CanRevealUndercard(cardIndex) => () =>
                {
                    player.Undercards[cardIndex].IsRevealed = true;
                }
            ,
            (GameState.GameOn, TakeUndercards { CardIndices: var cardIndices })
                when _turnsManager.Current == playerId &&
                    player.CanTakeUndercards(cardIndices) =>
                () =>
                {
                    if (player.RevealedCards.Count == 0)
                    {
                        int i = cardIndices[0];

                        player.Hand.Push(player.Undercards[i].Card);
                        player.Undercards.Remove(i);
                    }
                    else
                    {
                        foreach (int i in cardIndices)
                        {
                            player.Hand.Push(player.RevealedCards[i]);
                            player.RevealedCards.Remove(i);
                        }
                    }
                }
            ,

            (GameState.GameOn, LeaveGame { PlayerId: var leavingPlayerId })
                when _turnsManager.ActivePlayers.Contains(leavingPlayerId) => () =>
                {
                    RemovePlayer(leavingPlayerId);
                }
            ,
            (GameState.GameOn, _) => null,

            // GameState.GameOver
            (GameState.GameOver, _) => null,

            _ => throw new Exception("Invalid game state"),
        };
    }

    private void RemovePlayer(int leavingPlayerId)
    {
        _turnsManager.RemovePlayer(leavingPlayerId);
        var leavingPlayer = PlayerStates[leavingPlayerId];

        leavingPlayer.DidLeaveGame = true;
        leavingPlayer.Hand.Clear();
        leavingPlayer.RevealedCards.Clear();

        Deck.Add(
            from undercard in leavingPlayer.Undercards.Values
            where !undercard.IsRevealed
            select undercard.Card
        );
        leavingPlayer.Undercards.Clear();
    }

    private void HandlePlayerWin(PlayerState player)
    {
        if (player.Won)
        {
            _turnsManager.RemovePlayer(player.Id);

            if (_turnsManager.ActivePlayers.Count == 1)
            {
                GameState = GameState.GameOver;
            }
        }
    }

    private bool ShouldDiscardPile(Value cardValue)
    {
        var top = DiscardPile.Take(SuitSize).ToArray();

        return cardValue == Value.Ten || (top.Length == SuitSize &&
            top.All(discard => discard.Value == cardValue));
    }

    private bool ShouldDiscardPileIfHadThese(IEnumerable<Card> cards)
    {
        var top = cards
            .Concat(DiscardPile)
            .Take(SuitSize)
            .ToArray();

        if (top.Length < SuitSize)
        {
            return false;
        }

        var topValue = top.First().Value;

        return top.Skip(1).All(card => card.Value == topValue);
    }

    private Value PlayHand(PlayerState player, int[] indices)
    {
        // TODO: change `player.Hand[i]` to a method on `PlayerState` that get the correct card
        var cards = indices.Select(i => player.GetCard(i)).ToArray();

        foreach (var i in indices.OrderByDescending(i => i))
        {
            player.RemoveCard(i);
        }

        DiscardPile.Push(cards);

        var value = cards.First().Value;
        ReplenishPlayerHand(player);

        return value;
    }

    private void ReplenishPlayerHand(PlayerState player)
    {
        while (player.Hand.Count < MIN_HAND_CARDS && Deck.Count > 0)
        {
            var card = Deck.Pop();
            player.Hand.Push(card);
        }
    }

    private bool CanPlaceCard(Card card)
    {
        var cardValue = card.Value;

        if (!Enum.IsDefined(cardValue))
        {
            return false;
        }

        var top = TopCard();

        return cardValue switch
        {
            _ when top is null => true,
            Value.Two => true,
            Value.Three => true,
            Value.Ten => true,
            _ when top.Value.Value == Value.Two => true,
            var value when top.Value.Value == Value.Seven => CardComparer.Compare(value, Value.Seven) <= 0,
            var value => CardComparer.Compare(value, top.Value.Value) >= 0,
        };
    }

    private Card? TopCard()
    {
        foreach (var card in DiscardPile)
        {
            if (card.Value != Value.Three)
            {
                return card;
            }
        }

        return null;
    }
    #endregion
}
