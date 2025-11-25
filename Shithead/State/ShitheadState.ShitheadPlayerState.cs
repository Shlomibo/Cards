using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

public sealed partial class ShitheadState
{
    public sealed class ShitheadPlayerState
    {
        private readonly ShitheadState _state;

        private PlayerState PlayerState => _state._players[PlayerId];
        private SharedPlayerState SharedPlayerState => _state.SharedState.Players[PlayerId];

        public GameState GameState => _state.GameState;
        public int PlayerId { get; }
        public IReadonlyDeck<Card> Hand { get; }
        public IReadOnlyDictionary<int, Card> RevealedCards => SharedPlayerState.RevealedCards;
        public IReadOnlyDictionary<int, Card?> Undercards => SharedPlayerState.Undercards;
        public bool Won => PlayerState.Won;
        public bool RevealedCardsAccepted => PlayerState.RevealedCardsAccepted;


        public ShitheadPlayerState(int playerId, ShitheadState state)
        {
            PlayerId = playerId;
            _state = state;
            Hand = PlayerState.Hand.AsReadonly();
        }

        public Card GetCard(int cardIndex) => this switch
        {
            { Hand.Count: > 0 } => Hand[cardIndex],
            { RevealedCards.Count: > 0 } => RevealedCards[cardIndex],
            _ => Undercards[cardIndex] ??
                throw new InvalidOperationException("The selected undercard is not revealed yet")
        };
    }
}
