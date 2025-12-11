using Deck;
using Deck.Cards.FrenchSuited;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shithead.State;

public sealed partial class ShitheadState
{
    /// <summary>
    /// The state of the Shithead game that is shared among all players.
    /// </summary>
    public sealed class SharedShitheadState
    {
        private readonly ShitheadState _state;

        /// <summary>
        /// Gets the players in the game.
        /// </summary>
        public IReadOnlyList<SharedPlayerState> Players { get; }

        /// <summary>
        /// Gets the ids of the players that are still playing.
        /// </summary>
        public IReadOnlyList<int> ActivePlayers => _state.TurnsManager.ActivePlayers;

        /// <summary>
        /// Gets the size of the deck.
        /// </summary>
        public int DeckSize => _state.Deck.Count;

        /// <summary>
        /// Gets the discard pile.
        /// </summary>
        public IReadonlyDeck<Card> DiscardPile { get; }

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState GameState => _state.GameState;

        /// <summary>
        /// Gets the id of the player whose turn it is.
        /// </summary>
        public int CurrentTurnPlayer => _state.TurnsManager.Current;

        /// <summary>
        /// Gets the last attempted move in the game.
        /// </summary>
        public (Moves.Move move, int? playerId)? LastMove => _state.LastMove;

        /// <summary>
        /// Gets the last played move in the game.
        /// </summary>
        public (Moves.Move move, int? playerId)? LastPlayedMove => _state.LastPlayedMove;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedShitheadState"/> class.
        /// </summary>
        /// <param name="state">The game's state.</param>
        public SharedShitheadState(ShitheadState state)
        {
            _state = state;
            Players = new PlayersView(this);
            DiscardPile = state.DiscardPile.AsReadonly();
        }

        private sealed class PlayersView : IReadOnlyList<SharedPlayerState>
        {
            private readonly SharedShitheadState _sharedState;
            private readonly Lazy<SharedPlayerState[]> _sharedPlayers;

            private SharedPlayerState[] Players => _sharedPlayers.Value;

            public SharedPlayerState this[int index] => Players[index];

            public int Count => _sharedState._state.PlayerStates.Length;


            public PlayersView(SharedShitheadState sharedState)
            {
                _sharedState = sharedState;
                _sharedPlayers = new Lazy<SharedPlayerState[]>(
                    () => [.. _sharedState._state.PlayerStates
                        .Select(player => new SharedPlayerState(_sharedState._state, player.Id))]);
            }

            public IEnumerator<SharedPlayerState> GetEnumerator()
            {
                foreach (var player in Players)
                {
                    yield return player;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    /// <summary>
    /// The state of a Shithead game player that is visible to other players.
    /// </summary>
    public sealed class SharedPlayerState
    {
        private readonly ShitheadState _gameState;

        private PlayerState Player => _gameState.PlayerStates[Id];

        /// <summary>
        /// Gets a value indicating whether the player has won the game.
        /// </summary>
        public bool Won => Player.Won;

        /// <summary>
        /// Gets the player's revealed cards.
        /// </summary>
        public IReadOnlyDictionary<int, Card> RevealedCards => Player.RevealedCards;

        /// <summary>
        /// Gets the player's undercards if they are revealed.
        /// </summary>
        public IReadOnlyDictionary<int, Card?> Undercards { get; }

        /// <summary>
        /// Gets the Id of the player.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the number of cards in the player's hand.
        /// </summary>
        public int CardsCount => Player.Hand.Count;

        /// <summary>
        /// Gets a value indicating whether the player has accepted their revealed cards selection.
        /// </summary>
        public bool RevealedCardsAccepted => Player.RevealedCardsAccepted;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedPlayerState"/> class.
        /// </summary>
        /// <param name="state">The game's state.</param>
        /// <param name="id">
        /// The id of the player that the current <see cref="SharedPlayerState"/> views.
        /// </param>
        public SharedPlayerState(ShitheadState state, int id)
        {
            _gameState = state;
            Id = id;
            Undercards = new UndercardsView(this);
        }

        private sealed class UndercardsView : IReadOnlyDictionary<int, Card?>
        {
            private readonly SharedPlayerState _playerState;

            private Dictionary<int, CardFace<Card>> Undercards =>
                _playerState.Player.Undercards;

            public Card? this[int key] => UndercardValue(Undercards[key]);

            public IEnumerable<int> Keys => Undercards.Keys;

            public IEnumerable<Card?> Values => Undercards.Values.Select(UndercardValue);

            public int Count => Undercards.Count;

            public UndercardsView(SharedPlayerState playerState)
            {
                _playerState = playerState;
            }

            public bool ContainsKey(int key) => Undercards.ContainsKey(key);

            public IEnumerator<KeyValuePair<int, Card?>> GetEnumerator()
            {
                foreach (var kv in Undercards)
                {
                    yield return new KeyValuePair<int, Card?>(kv.Key, UndercardValue(kv.Value));
                }
            }

            public bool TryGetValue(int key, [MaybeNullWhen(false)] out Card? value)
            {
                value = default;
                bool gotValue = false;

                if (Undercards.TryGetValue(key, out var card))
                {
                    value = UndercardValue(card);
                    gotValue = true;
                }

                return gotValue;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private static Card? UndercardValue(CardFace<Card> undercard) =>
                undercard.IsRevealed
                    ? undercard.Card
                    : null;
        }
    }
}
