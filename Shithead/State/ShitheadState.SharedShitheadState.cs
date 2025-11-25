using Deck;
using Deck.Cards.FrenchSuited;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shithead.State;

public sealed partial class ShitheadState
{
    public sealed class SharedShitheadState
    {
        private readonly ShitheadState _state;

        public IReadOnlyList<SharedPlayerState> Players { get; }
        public IReadOnlyList<int> ActivePlayers => _state._turnsManager.ActivePlayers;
        public int DeckSize => _state.Deck.Count;
        public IReadonlyDeck<Card> DiscardPile { get; }
        public GameState GameState => _state.GameState;
        public int CurrentTurnPlayer => _state._turnsManager.Current;
        public (ShitheadMove.ShitheadMove move, int? playerId)? LastMove => _state._lastMove;

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

            public int Count => _sharedState._state._players.Length;


            public PlayersView(SharedShitheadState sharedState)
            {
                _sharedState = sharedState;
                _sharedPlayers = new Lazy<SharedPlayerState[]>(
                    () => [.. _sharedState._state._players
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

    public sealed class SharedPlayerState
    {
        private readonly ShitheadState _gameState;

        private PlayerState Player => _gameState._players[Id];

        public bool Won => Player.Won;

        public IReadOnlyDictionary<int, Card> RevealedCards => Player.RevealedCards;

        public IReadOnlyDictionary<int, Card?> Undercards { get; }

        public int Id { get; }

        public int CardsCount => Player.Hand.Count;
        public bool RevealedCardsAccepted => Player.RevealedCardsAccepted;

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
