using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

public sealed partial class ShitheadState
{
    /// <summary>
    /// The state that is visible to a specific Shithead game player.
    /// </summary>
    public sealed class ShitheadPlayerState
    {
        private readonly ShitheadState _state;

        private PlayerState PlayerState => _state._players[PlayerId];
        private SharedPlayerState SharedPlayerState => _state.SharedState.Players[PlayerId];

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState GameState => _state.GameState;

        /// <summary>
        /// Gets the Id of the player.
        /// </summary>
        public int PlayerId { get; }

        /// <summary>
        /// Gets the player's hand.
        /// </summary>
        public IReadonlyDeck<Card> Hand { get; }

        /// <summary>
        /// Gets the player's revealed cards.
        /// </summary>
        public IReadOnlyDictionary<int, Card> RevealedCards => SharedPlayerState.RevealedCards;

        /// <summary>
        /// Gets the player's undercards if they are revealed.
        /// </summary>
        public IReadOnlyDictionary<int, Card?> Undercards => SharedPlayerState.Undercards;

        /// <summary>
        /// Gets a value indicating whether the player has won the game.
        /// </summary>
        public bool Won => PlayerState.Won;

        /// <summary>
        /// Gets a value indicating whether the player has accepted their revealed cards selection.
        /// </summary>
        public bool RevealedCardsAccepted => PlayerState.RevealedCardsAccepted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShitheadPlayerState"/> class.
        /// </summary>
        /// <param name="playerId">The id of the player this state represents.</param>
        /// <param name="state">The game's state.</param>
        public ShitheadPlayerState(int playerId, ShitheadState state)
        {
            PlayerId = playerId;
            _state = state;
            Hand = PlayerState.Hand.AsReadonly();
        }

        /// <summary>
        /// Gets the card at the specified index from the player's hand, revealed cards, or undercards.
        /// </summary>
        /// <param name="cardIndex">The card to get.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item>When the player has cards in his hand - return a card from the hand.</item>
        /// <item>When the player has revealed cards - return a revealed card.</item>
        /// <item>Otherwise, return an undercard if it is revealed.</item>
        /// </list>
        /// </returns>
        public Card GetCard(int cardIndex) => this switch
        {
            { Hand.Count: > 0 } => Hand[cardIndex],
            { RevealedCards.Count: > 0 } => RevealedCards[cardIndex],
            _ => Undercards[cardIndex] ??
                throw new InvalidOperationException("The selected undercard is not revealed yet")
        };
    }
}
